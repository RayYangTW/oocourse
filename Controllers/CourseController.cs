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
using personal_project.Services;
using static personal_project.Models.Dtos.CourseDetailDto;

namespace personal_project.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class CourseController : ControllerBase
  {

    private readonly GetUserDataFromJWTHelper _jwtHelper;
    private readonly ICourseService _courseService;


    public CourseController(GetUserDataFromJWTHelper jwtHelper, ICourseService courseService)
    {
      _jwtHelper = jwtHelper;
      _courseService = courseService;
    }

    [HttpGet("search/all")]
    public async Task<IActionResult> SearchCourse()
    {
      var courseData = await _courseService.GetAllTeachersAsync();
      return Ok(courseData);
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchCourse(string keyword)
    {
      var courseData = await _courseService.GetTeachersByKeywordAsync(keyword);
      if (courseData is null)
        return NoContent();
      return Ok(courseData);
    }

    [HttpGet("search/{id}")]
    public async Task<IActionResult> GetSearchCourseDetail(long id)
    {
      try
      {
        var courseDetail = await _courseService.GetCourseDetailAsync(id);

        if (courseDetail is not null)
          return Ok(courseDetail);
        return NoContent();
      }
      catch (Exception ex)
      {
        return BadRequest();
      }
    }

    [HttpGet("{roomId}")]
    public async Task<IActionResult> GetCourseDetailByRoomId(string roomId)
    {
      var authorizationHeader = Request.Headers["Authorization"].ToString();
      var courseDetail = await _courseService.GetCourseDetailByRoomIdAsync(roomId, authorizationHeader);

      if (courseDetail is not null)
        return Ok(courseDetail);
      return BadRequest("Can't find user or user has no booking course.");
    }

    [HttpGet("access")]
    public async Task<IActionResult> GetAccessToOnlineCourse([FromQuery] string roomId)
    {
      var authorizationHeader = Request.Headers["Authorization"].ToString();
      var user = await _jwtHelper.GetUserDataFromJWTAsync(authorizationHeader);
      if (user is null)
        return BadRequest("Can't find user.");

      var hasAccess = await _courseService.HasAccessToOnlineCourseAsync(user.id, roomId);
      if (hasAccess)
        return Ok("User is on the access list.");

      return NotFound("User is not on the access list.");
    }
  }
}