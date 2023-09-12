using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using personal_project.Data;
using personal_project.Helpers;
using personal_project.Models.Domain;
using personal_project.Models.Dtos;
using personal_project.Models.Payments.LinePayConfirm;
using personal_project.Models.ResultModels;

namespace personal_project.Services
{
  public class CheckoutService : ICheckoutService
  {
    private readonly WebDbContext _db;
    private readonly GetUserDataFromJWTHelper _jwtHelper;
    private readonly ILinePayService _linePayService;
    private readonly IConfiguration _config;
    private readonly IMapper _mapper;

    public CheckoutService(WebDbContext db, GetUserDataFromJWTHelper jwtHelper, ILinePayService linePayService, IConfiguration config, IMapper mapper)
    {
      _db = db;
      _jwtHelper = jwtHelper;
      _linePayService = linePayService;
      _config = config;
      _mapper = mapper;
    }

    string channelId = DotNetEnv.Env.GetString("LINEPAY_CHANNEL_ID");
    string secretKey = DotNetEnv.Env.GetString("LINEPAY_CHANNEL_SECRET_KEY");
    string version = DotNetEnv.Env.GetString("LINEPAY_VERSION");
    string site = DotNetEnv.Env.GetString("LINEPAY_SITE");
    string confirmUrl = DotNetEnv.Env.GetString("LINEPAY_RETURN_HOST") + System.Environment.GetEnvironmentVariable("LINEPAY_RETURN_CONFIRM_URL");
    string cancelUrl = DotNetEnv.Env.GetString("LINEPAY_RETURN_HOST") + System.Environment.GetEnvironmentVariable("LINEPAY_RETURN_CANCEL_URL");

    public async Task<LinePayResult> PayByLinePayAsync(CheckoutDto checkoutDto, string authorizationHeader)
    {
      // Check if repeat order
      var existingBookingData = await _db.Bookings
                                      .Where(data => data.courseId == checkoutDto.courseId && data.status == "paid")
                                      .AnyAsync();
      if (existingBookingData)
        return new LinePayResult
        {
          statusCode = 403,
          message = "The course has already been booked."
        };

      // Find user from User
      var user = await _jwtHelper.GetUserDataFromJWTAsync(authorizationHeader);
      if (user is null)
        return new LinePayResult
        {
          statusCode = 404,
          message = "Can't find user."
        };

      // Find the course from Course
      var course = await _db.Courses
                      .Where(data => data.id == checkoutDto.courseId)
                      .FirstOrDefaultAsync();
      if (course is null)
        return new LinePayResult
        {
          statusCode = 404,
          message = "Can't find the course."
        };


      var newLinepay = _linePayService.PrepareLinePayRequest(checkoutDto, confirmUrl, cancelUrl);
      var result = await _linePayService.SendLinePayRequestAsync(newLinepay, secretKey, version, site, channelId, user, course);

      return result;
    }

    public async Task<BookingDetailDto> UpdateDataAfterLinePayConfirmAsync(string orderId, LinePayConfirm response)
    {
      var booking = await _db.Bookings
                      .Where(data => data.orderId == orderId)
                      .Include(data => data.course)
                      .FirstOrDefaultAsync();

      var userData = await _db.Users
                          .Where(data => data.id == booking.userId)
                          .FirstOrDefaultAsync();

      var courseData = await _db.Courses
                            .Where(data => data.id == booking.courseId)
                            .Include(data => data.teacher)
                            .FirstOrDefaultAsync();

      if (response.returnCode == "0000")
      {
        booking.status = "paid";

        courseData.isBooked = true;

        // Generate roomId
        var newRoomId = GenerateRandomStringHelper.GenerateRandomString(12);

        courseData.roomId = newRoomId;

        // Generate courseLink
        if (courseData.teacher.courseWay.Contains("實體") || courseData.teacher.courseWay.Contains("線下"))
        {
          courseData.courseLink = _config["Host"] + "/course/offline.html?id=" + newRoomId;
        }
        else
        {
          courseData.courseLink = _config["Host"] + "/course/online.html?id=" + newRoomId;
        }

        // update CourseAccessList
        var newList = new CourseAccessList
        {
          roomId = newRoomId,
          teacherUserId = courseData.teacher.userId,
          userId = userData.id,
          course = courseData
        };

        await _db.CourseAccessLists.AddAsync(newList);

        await _db.SaveChangesAsync();

        var responseData = _mapper.Map<BookingDetailDto>(courseData);

        return responseData;
      }
      return null;
    }

    public async Task<CheckoutResult> ProcessCheckoutAsync(string orderId, string authorizationHeader)
    {
      // Check if repeat course
      // Get course Id
      var bookingData = await _db.Bookings
                            .Where(data => data.orderId == orderId)
                            .FirstAsync();
      if (bookingData is null)
        return new CheckoutResult
        {
          statusCode = 404,
          message = "Can't find the order."
        };

      var repeatCourse = await _db.Bookings
                              .Where(data => data.courseId == bookingData.courseId)
                              .Where(data => data.status == "paid")
                              .AnyAsync();
      if (repeatCourse is true)
        return new CheckoutResult
        {
          statusCode = 403,
          message = "Repeat courseId."
        };

      // Check if repeat order
      var repeatOrder = await _db.Bookings
                                      .Where(data => data.orderId == orderId && data.status == "paid")
                                      .AnyAsync();
      if (repeatOrder)
        return new CheckoutResult
        {
          statusCode = 403,
          message = "Repeat orderId."
        };

      // Find user from User
      var user = await _jwtHelper.GetUserDataFromJWTAsync(authorizationHeader);
      if (user is null)
        return new CheckoutResult
        {
          statusCode = 400,
          message = "Can't find user."
        };

      return new CheckoutResult
      {
        statusCode = 200
      };
    }
  }
}