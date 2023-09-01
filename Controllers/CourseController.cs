using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using personal_project.Data;
using personal_project.Helpers;
using personal_project.Models.Dtos;
using personal_project.Models.ResponseModels;
using static personal_project.Models.Dtos.CourseDetailDto;

namespace personal_project.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class CourseController : ControllerBase
  {
    private readonly ILogger<CourseController> _logger;
    private readonly WebDbContext _db;
    private readonly IMapper _mapper;
    private readonly GetUserDataFromJWTHelper _jwtHelper;


    public CourseController(ILogger<CourseController> logger, WebDbContext db, IMapper mapper, GetUserDataFromJWTHelper jwtHelper)
    {
      _logger = logger;
      _db = db;
      _mapper = mapper;
      _jwtHelper = jwtHelper;
    }

    [HttpGet("search/all")]
    public async Task<IActionResult> SearchCourse()
    {
      var courseData = await _db.Teachers
                      .Select(data => data)
                      .ToListAsync();
      return Ok(courseData);
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchCourse(string keyword)
    {
      var courseData = await _db.Teachers
          .Where(data =>
              data.courseName.Contains(keyword) ||
              data.courseCategory.Contains(keyword) ||
              data.courseLocation.Contains(keyword) ||
              data.courseWay.Contains(keyword))
          .ToListAsync();

      if (courseData.Count() > 0)
      {
        return Ok(courseData);
      }
      else
      {
        return NoContent();
      }
    }

    [HttpGet("search/{id}")]
    public async Task<IActionResult> GetSearchCourseDetail(long id)
    {
      try
      {
        var afterThreeHours = DateTime.Now.AddHours(3);
        var courseData = await _db.Teachers
                            .Where(data => data.id == id)
                            .Include(data => data.courses
                                                  .Where(data => data.isBooked == false)
                                                  .Where(data => data.startTime >= afterThreeHours))
                            .FirstOrDefaultAsync();

        if (courseData != null)
        {
          var resultDto = _mapper.Map<CourseDetailDTO>(courseData);
          foreach (var course in resultDto.courses)
          {
            course.startTime = ConvertDateTimeFormatHelper.ConvertDateTimeFormat(course.startTime);
            course.endTime = ConvertDateTimeFormatHelper.ConvertDateTimeFormat(course.endTime);
          }
          resultDto.courses = resultDto.courses.OrderBy(course => course.startTime).ToList();
          return Ok(resultDto);
        }
        return NoContent();
      }
      catch (Exception ex)
      {
        System.Console.WriteLine(ex);
        return StatusCode(500);
      }
    }

    [HttpGet("{roomId}")]
    public async Task<IActionResult> GetCourseDetailByRoomId(string roomId)
    {
      var user = await _jwtHelper.GetUserDataFromJWTAsync(Request.Headers["Authorization"]);
      if (user is null)
        return BadRequest("Can't find user.");

      var courseData = await _db.Courses
                            .Where(data => data.roomId == roomId)
                            .Include(data => data.teacher)
                            .FirstOrDefaultAsync();

      if (courseData is null)
        return BadRequest("User has no booking course");

      var responseData = _mapper.Map<BookingDetailDto>(courseData);
      responseData.startTime = ConvertDateTimeFormatHelper.ConvertDateTimeFormat(responseData.startTime);
      responseData.endTime = ConvertDateTimeFormatHelper.ConvertDateTimeFormat(responseData.endTime);

      return Ok(responseData);
    }

    [HttpGet("access")]
    public async Task<IActionResult> GetAccessToOnlineCourse([FromQuery] string id)
    {
      var user = await _jwtHelper.GetUserDataFromJWTAsync(Request.Headers["Authorization"]);
      if (user is null)
        return BadRequest("Can't find user.");

      // var courseAccessListData = await _db.CourseAccessLists
      //                       .Where(data => data.roomId == roomId)
      //                       .FirstOrDefaultAsync();
      // if (courseAccessListData.teacherUserId == user.id || courseAccessListData.userId == user.id)
      // {
      //   return Ok("User is on the list.");
      // }
      // return NotFound("User is not on the list.");


      var courseAccessList = await _db.CourseAccessLists
                            .Where(data => data.roomId == id)
                            .ToListAsync();
      foreach (var data in courseAccessList)
      {
        if (data.teacherUserId == user.id || data.userId == user.id)
        {
          return Ok("User is on the access list.");
        }
        return NotFound("User is not on the access list.");
      }
      return NotFound("User is not on the access list.");
    }
  }
}