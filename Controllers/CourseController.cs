using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using personal_project.Data;

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
  }
}