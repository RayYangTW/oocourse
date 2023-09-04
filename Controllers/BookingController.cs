using System.Diagnostics.Contracts;
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

namespace personal_project.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class BookingController : ControllerBase
  {
    private readonly ILogger<BookingController> _logger;
    private readonly WebDbContext _db;
    private readonly IMapper _mapper;
    private readonly GetUserDataFromJWTHelper _jwtHelper;
    private readonly IConfiguration _config;

    public BookingController(ILogger<BookingController> logger, WebDbContext db, IMapper mapper, GetUserDataFromJWTHelper jwtHelper, IConfiguration config)
    {
      _logger = logger;
      _db = db;
      _mapper = mapper;
      _jwtHelper = jwtHelper;
      _config = config;
    }

    [HttpGet("{courseId}")]
    public async Task<IActionResult> GetBookingData(long courseId)
    {
      try
      {
        var user = await _jwtHelper.GetUserDataFromJWTAsync(Request.Headers["Authorization"]);
        if (user is null)
          return BadRequest("Can't find user.");

        var amITheTeacher = await _db.Courses
                                  .Where(data => data.id == courseId)
                                  .Where(data => data.teacher.userId == user.id)
                                  .AnyAsync();
        if (amITheTeacher is true)
          return StatusCode(403, "Can't book your own course.");

        var courseData = await _db.Courses
                              .Where(data => data.id == courseId)
                              .FirstOrDefaultAsync();

        var teacherData = await _db.Teachers
                                .Where(data => data.id == courseData.teacherId)
                                .FirstOrDefaultAsync();

        var responseData = new BookingDetailDto();

        _mapper.Map(courseData, responseData);
        _mapper.Map(teacherData, responseData);

        responseData.startTime = ConvertDateTimeFormatHelper.ConvertDateTimeFormat(responseData.startTime);
        responseData.endTime = ConvertDateTimeFormatHelper.ConvertDateTimeFormat(responseData.endTime);

        if (responseData is not null)
          return Ok(responseData);
        return NoContent();
      }
      catch (Exception ex)
      {
        return BadRequest(ex.Message);
      }
    }

    [HttpPost("{courseId}")]
    public async Task<IActionResult> PostBookingData(long courseId)
    {
      try
      {
        var existingBookingData = await _db.Bookings
                                        .Where(data => data.courseId == courseId)
                                        .AnyAsync();
        if (existingBookingData)
          return StatusCode(403, "The course has already been booked.");

        var user = await _jwtHelper.GetUserDataFromJWTAsync(Request.Headers["Authorization"]);
        if (user is null)
          return BadRequest("Can't find user.");

        var courseData = await _db.Courses
                                .Where(data => data.id == courseId)
                                .Include(data => data.teacher)
                                .FirstOrDefaultAsync();
        if (courseData is null)
          return NotFound("Not found the course data.");

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

        return Ok(new
        {
          status = newBookingData.status,
          userId = user.id,
          courseId = courseData.id
        });
      }
      catch (Exception ex)
      {
        return BadRequest(ex.Message);
      }

    }
  }
}