using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using personal_project.Data;
using personal_project.Helpers;
using personal_project.Models.Domain;
using personal_project.Models.Dtos;
using personal_project.Models.ResultModels;

namespace personal_project.Services
{
  public class BookingService : IBookingService
  {
    private readonly ILogger<BookingService> _logger;
    private readonly WebDbContext _db;
    private readonly IMapper _mapper;
    private readonly GetUserDataFromJWTHelper _jwtHelper;
    private readonly IConfiguration _config;

    public BookingService(ILogger<BookingService> logger, WebDbContext db, IMapper mapper, GetUserDataFromJWTHelper jwtHelper, IConfiguration config)
    {
      _logger = logger;
      _db = db;
      _mapper = mapper;
      _jwtHelper = jwtHelper;
      _config = config;
    }
    public async Task<BookingDetailDto> GetBookingDataAsync(long courseId, string authorizationHeader)
    {
      try
      {
        var user = await _jwtHelper.GetUserDataFromJWTAsync(authorizationHeader);
        if (user is null)
          return null;

        var courseData = await _db.Courses
            .Where(data => data.id == courseId)
            .Include(data => data.teacher)
            .FirstOrDefaultAsync();

        if (courseData is null)
          return null;

        if (courseData.teacher.userId == user.id)
          return null;

        var responseData = _mapper.Map<BookingDetailDto>(courseData);
        responseData.startTime = ConvertDateTimeFormatHelper.ConvertDateTimeFormat(responseData.startTime);
        responseData.endTime = ConvertDateTimeFormatHelper.ConvertDateTimeFormat(responseData.endTime);

        return responseData;
      }
      catch (Exception ex)
      {
        throw;
      }
    }

    public async Task<BookingResult> PostBookingDataAsync(long courseId, string authorizationHeader)
    {
      try
      {
        var existingBookingData = await _db.Bookings
            .Where(data => data.courseId == courseId)
            .AnyAsync();
        if (existingBookingData)
          return new BookingResult
          {
            StatusCode = 403,
            Message = "The course has already been booked."
          };


        var user = await _jwtHelper.GetUserDataFromJWTAsync(authorizationHeader);
        if (user is null)
          return new BookingResult
          {
            StatusCode = 400,
            Message = "Can't find user."
          };

        var courseData = await _db.Courses
            .Where(data => data.id == courseId)
            .Include(data => data.teacher)
            .FirstOrDefaultAsync();
        if (courseData is null)
          return new BookingResult
          {
            StatusCode = 400,
            Message = "Not found the course data."
          };

        var newBookingData = new Booking
        {
          status = "booked",
          bookingTime = DateTime.Now,
          user = user,
          course = courseData
        };

        await _db.Bookings.AddAsync(newBookingData);

        // Generate roomId
        var newRoomId = GenerateRandomStringHelper.GenerateRandomString(12);

        // update isBooked field
        courseData.isBooked = true;

        courseData.roomId = newRoomId;

        if (courseData.teacher.courseWay.Contains("實體") || courseData.teacher.courseWay.Contains("線下"))
        {
          courseData.courseLink = _config["Host"] + "/course/offline.html?id=" + newRoomId;
        }
        else
        {
          courseData.courseLink = _config["Host"] + "/course/online.html?id=" + newRoomId;
        }
        // Save to db
        await _db.SaveChangesAsync();

        return new BookingResult
        {
          StatusCode = 200,
          Data = new
          {
            Status = newBookingData.status,
            UserId = user.id,
            CourseId = courseData.id
          }
        };
      }
      catch (Exception ex)
      {
        throw;
      }
    }
  }
}