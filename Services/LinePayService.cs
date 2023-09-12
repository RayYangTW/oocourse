using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using personal_project.Data;
using personal_project.Models.Domain;
using personal_project.Models.Dtos;
using personal_project.Models.Payments;
using personal_project.Models.Payments.LinePayConfirm;
using personal_project.Models.ResultModels;

namespace personal_project.Services
{
  public class LinePayService : ILinePayService
  {
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly WebDbContext _db;

    public LinePayService(IHttpClientFactory httpClientFactory, WebDbContext db)
    {
      _httpClientFactory = httpClientFactory;
      _db = db;
    }

    string channelId = DotNetEnv.Env.GetString("LINEPAY_CHANNEL_ID");
    string secretKey = DotNetEnv.Env.GetString("LINEPAY_CHANNEL_SECRET_KEY");
    string version = DotNetEnv.Env.GetString("LINEPAY_VERSION");
    string site = DotNetEnv.Env.GetString("LINEPAY_SITE");
    string confirmUrl = DotNetEnv.Env.GetString("LINEPAY_RETURN_HOST") + System.Environment.GetEnvironmentVariable("LINEPAY_RETURN_CONFIRM_URL");
    string cancelUrl = DotNetEnv.Env.GetString("LINEPAY_RETURN_HOST") + System.Environment.GetEnvironmentVariable("LINEPAY_RETURN_CANCEL_URL");

    public LinePay PrepareLinePayRequest(CheckoutDto checkoutDto, string confirmUrl, string cancelUrl)
    {
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

      return newLinepay;
    }


    public async Task<LinePayResult> SendLinePayRequestAsync(LinePay newLinepay, string secretKey, string version, string site, string channelId, User user, Course course)
    {
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
            orderId = newLinepay.orderId,
            bookingTime = DateTime.Now,
            user = user,
            course = course
          };

          await _db.Bookings.AddAsync(newBookingData);
          await _db.SaveChangesAsync();

          return new LinePayResult
          {
            statusCode = 200,
            data = new { redirectUrl = response.info.paymentUrl.web.ToString() }
          };
        }
        return new LinePayResult
        {
          statusCode = 400,
          message = "錯誤，返回碼：" + response.returnCode
        };
      }
      catch (Exception ex)
      {
        return new LinePayResult
        {
          statusCode = 400,
          message = ex.Message
        };
      }
    }

    public async Task<ConfirmBody> PrepareConfirmBodyAsync(string orderId)
    {
      var booking = await _db.Bookings
                        .Where(data => data.orderId == orderId)
                        .Include(data => data.course)
                        .FirstOrDefaultAsync();

      var newConfirmBody = new ConfirmBody
      {
        amount = (int)booking.course.price,
        currency = "TWD"
      };

      return newConfirmBody;
    }

    public async Task<LinePayConfirm> SendLinePayConfirmRequestAsync(ConfirmBody newConfirmBody, string transactionId)
    {
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

      var linePayResponseMessage = await client.PostAsync(url, httpContent);
      var responseBody = await linePayResponseMessage.Content.ReadAsStringAsync();
      var response = JsonConvert.DeserializeObject<LinePayConfirm>(responseBody);

      return response;
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