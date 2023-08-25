using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using personal_project.Data;
using personal_project.Helpers;
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


    public CourseController(ILogger<CourseController> logger, WebDbContext db, IMapper mapper)
    {
      _logger = logger;
      _db = db;
      _mapper = mapper;
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

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCourseDetail(long id)
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
  }
}