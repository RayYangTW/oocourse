using System.Security.Cryptography;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using personal_project.Data;
using personal_project.Helpers;
using personal_project.Models.Dtos;
using personal_project.Models.Payments;
using System.Text;
using System.Net.Http.Headers;
using Microsoft.EntityFrameworkCore;
using personal_project.Models.Domain;
using personal_project.Models.Payments.LinePayConfirm;

namespace personal_project.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class CheckoutController : ControllerBase
  {
    private readonly ILogger<CheckoutController> _logger;
    private readonly WebDbContext _db;
    private readonly IMapper _mapper;
    private readonly GetUserDataFromJWTHelper _jwtHelper;
    private readonly IConfiguration _config;
    private readonly IHttpClientFactory _httpClientFactory;

    public CheckoutController(ILogger<CheckoutController> logger, WebDbContext db, IMapper mapper, GetUserDataFromJWTHelper jwtHelper, IConfiguration config, IHttpClientFactory httpClientFactory)
    {
      _logger = logger;
      _db = db;
      _mapper = mapper;
      _jwtHelper = jwtHelper;
      _config = config;
      _httpClientFactory = httpClientFactory;
    }

    [HttpPost("linepay")]
    public async Task<IActionResult> PayByLinePay([FromForm] CheckoutDto checkoutDto)
    {
      // Load env variables
      DotNetEnv.Env.Load();
      var channelId = System.Environment.GetEnvironmentVariable("LINEPAY_CHANNEL_ID");
      var secretKey = System.Environment.GetEnvironmentVariable("LINEPAY_CHANNEL_SECRET_KEY");
      var version = System.Environment.GetEnvironmentVariable("LINEPAY_VERSION");
      var site = System.Environment.GetEnvironmentVariable("LINEPAY_SITE");
      var confirmUrl = System.Environment.GetEnvironmentVariable("LINEPAY_RETURN_HOST") + System.Environment.GetEnvironmentVariable("LINEPAY_RETURN_CONFIRM_URL");
      var cancelUrl = System.Environment.GetEnvironmentVariable("LINEPAY_RETURN_HOST") + System.Environment.GetEnvironmentVariable("LINEPAY_RETURN_CANCEL_URL");

      // Check if repeat order
      var existingBookingData = await _db.Bookings
                                      .Where(data => data.courseId == checkoutDto.courseId)
                                      .AnyAsync();
      if (existingBookingData)
        return StatusCode(403, "The course has already been booked.");

      // Find user from User
      var user = await _jwtHelper.GetUserDataFromJWTAsync(Request.Headers["Authorization"]);
      if (user is null)
        return BadRequest("Can't find user.");

      // Find the course from Course
      var course = await _db.Courses
                      .Where(data => data.id == checkoutDto.courseId)
                      .FirstOrDefaultAsync();
      if (course is null)
        return BadRequest("Can't find the course.");

      // Prepare RequestBody
      var orderId = DateTime.Now.ToString("yyyyMMddHHmmssfff");
      var newLinepay = new LinePay
      {
        amount = (int)checkoutDto.productPrice * checkoutDto.productQty,
        currency = "TWD",
        orderId = orderId,
        redirectUrls = new RedirectUrls(),
        packages = new List<Models.Payments.Package>()
      };

      var newRedirectUrls = new RedirectUrls
      {
        confirmUrl = confirmUrl,
        cancelUrl = cancelUrl
      };
      newLinepay.redirectUrls = newRedirectUrls;

      var newPackage = new Models.Payments.Package
      {
        id = orderId,
        amount = (int)checkoutDto.productPrice * checkoutDto.productQty,
        products = new List<Models.Payments.Product>()
      };

      var newProduct = new Models.Payments.Product
      {
        name = checkoutDto.productName,
        quantity = checkoutDto.productQty,
        price = (int)checkoutDto.productPrice
      };
      newPackage.products.Add(newProduct);
      newLinepay.packages.Add(newPackage);

      // Prepare Signature
      var uri = "/payments/request";
      var nonce = DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString();

      var requestBody = JsonConvert.SerializeObject(newLinepay);
      string signature = CreateLinePaySignature(secretKey, version, uri, nonce, requestBody);

      // POST Api to LinePay server
      var url = site + "/" + version + uri;
      // Setting Headers
      HttpClient client = CreateLinePayHeaders(channelId, nonce, signature);
      // setting httpContent
      var httpContent = new StringContent(requestBody, Encoding.UTF8, "application/json");

      try
      {
        var linePayResponseMessage = await client.PostAsync(url, httpContent);
        var responseBody = await linePayResponseMessage.Content.ReadAsStringAsync();
        var response = JsonConvert.DeserializeObject<linePayResponse>(responseBody);

        // When payment api response success, db insert a new row update order status and return redirect url to frontend
        if (response.returnCode == "0000")
        {
          var newBookingData = new Booking
          {
            status = "unpaid",
            orderId = orderId,
            bookingTime = DateTime.Now,
            user = user,
            course = course
          };

          await _db.Bookings.AddAsync(newBookingData);
          await _db.SaveChangesAsync();

          return Ok(new { redirectUrl = response.info.paymentUrl.web.ToString() });
        }

        return BadRequest("錯誤，返回碼：" + response.returnCode);
      }
      catch (Exception ex)
      {
        return BadRequest(ex.Message);
      }
    }

    [HttpGet("linepay/confirm")]
    public async Task<IActionResult> GetConfirmLinePay(string transactionId, string orderId)
    {
      // Load env variables
      DotNetEnv.Env.Load();
      var channelId = System.Environment.GetEnvironmentVariable("LINEPAY_CHANNEL_ID");
      var secretKey = System.Environment.GetEnvironmentVariable("LINEPAY_CHANNEL_SECRET_KEY");
      var version = System.Environment.GetEnvironmentVariable("LINEPAY_VERSION");
      var site = System.Environment.GetEnvironmentVariable("LINEPAY_SITE");
      var confirmUrl = System.Environment.GetEnvironmentVariable("LINEPAY_RETURN_HOST") + System.Environment.GetEnvironmentVariable("LINEPAY_RETURN_CONFIRM_URL");
      var cancelUrl = System.Environment.GetEnvironmentVariable("LINEPAY_RETURN_HOST") + System.Environment.GetEnvironmentVariable("LINEPAY_RETURN_CANCEL_URL");

      var booking = await _db.Bookings
                          .Where(data => data.orderId == orderId)
                          .Include(data => data.course)
                          .FirstOrDefaultAsync();

      var courseData = await _db.Courses
                            .Where(data => data.id == booking.courseId)
                            .Include(data => data.teacher)
                            .FirstOrDefaultAsync();

      var newConfirmBody = new ConfirmBody
      {
        amount = (int)booking.course.price,
        currency = "TWD"
      };
      var requestBody = JsonConvert.SerializeObject(newConfirmBody);
      var uri = $"/payments/{transactionId}/confirm";
      var nonce = DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString();

      string signature = CreateLinePaySignature(secretKey, version, uri, nonce, requestBody);

      // Setting Headers
      HttpClient client = CreateLinePayHeaders(channelId, nonce, signature);

      // Setting httpContent
      var httpContent = new StringContent(requestBody, Encoding.UTF8, "application/json");

      // Setting url
      var url = site + "/" + version + uri;
      try
      {
        var linePayResponseMessage = await client.PostAsync(url, httpContent);
        var responseBody = await linePayResponseMessage.Content.ReadAsStringAsync();

        var response = JsonConvert.DeserializeObject<LinePayConfirm>(responseBody);

        if (response.returnCode == "0000")
        {
          booking.status = "paid";

          courseData.isBooked = true;

          // Generate roomId
          var newRoomId = GenerateRandomStringHelper.GenerateRandomString(12);

          courseData.roomId = newRoomId;

          if (courseData.teacher.courseWay.Contains("實體") || courseData.teacher.courseWay.Contains("線下"))
          {
            courseData.courseLink = _config["Host"] + "course/offline.html?id=" + newRoomId;
          }
          else
          {
            courseData.courseLink = _config["Host"] + "course/online.html?id=" + newRoomId;
          }

          await _db.SaveChangesAsync();

          var responseData = _mapper.Map<BookingDetailDto>(courseData);

          return Ok(responseData);
        }
        return BadRequest("錯誤，返回碼：" + response.returnCode);

      }
      catch (Exception ex)
      {
        return BadRequest(ex.Message);
      }
    }


    /**********************************
    Methods
    **********************************/
    private static string CreateLinePaySignature(string? secretKey, string? version, string uri, string nonce, string requestBody)
    {
      var stringToSignature = secretKey + "/" + version + uri + requestBody + nonce;
      var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey));
      var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(stringToSignature));
      var signature = Convert.ToBase64String(hashBytes);
      return signature;
    }

    private HttpClient CreateLinePayHeaders(string? channelId, string nonce, string signature)
    {
      var client = _httpClientFactory.CreateClient();
      client.DefaultRequestHeaders.Add("X-LINE-ChannelId", channelId);
      client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
      client.DefaultRequestHeaders.Add("X-LINE-Authorization-Nonce", nonce);
      client.DefaultRequestHeaders.Add("X-LINE-Authorization", signature);
      return client;
    }


  }
}