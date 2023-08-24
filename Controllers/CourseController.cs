using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using personal_project.Data;
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
                                                  .Where(data => data.startTime >= afterThreeHours))
                            .FirstOrDefaultAsync();

        if (courseData != null)
        {
          var resultDto = _mapper.Map<CourseDetailDTO>(courseData);
          foreach (var course in resultDto.courses)
          {
            course.startTime = ConvertToCustomFormat(course.startTime);
            course.endTime = ConvertToCustomFormat(course.endTime);
          }
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


    public static string ConvertToCustomFormat(string originalDate)
    {
      if (DateTime.TryParse(originalDate, out DateTime parsedDate))
      {
        return parsedDate.ToString("yyyy/MM/dd - HH:mm");
      }
      System.Console.WriteLine("轉換失敗...");
      return originalDate;  // 如果轉換失敗，則回傳原始日期。
    }
  }
}