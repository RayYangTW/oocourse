using System.Security.AccessControl;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using personal_project.Data;
using personal_project.Models.Domain;
using personal_project.Models.FormModels;
using personal_project.Services;

namespace personal_project.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class AdminController : ControllerBase
  {

    private readonly ILogger<AdminController> _logger;
    private readonly WebDbContext _db;
    private readonly IMapper _mapper;
    private readonly IEmailService _emailService;


    public AdminController(ILogger<AdminController> logger, WebDbContext db, IMapper mapper, IEmailService emailService)
    {
      _logger = logger;
      _db = db;
      _mapper = mapper;
      _emailService = emailService;
    }

    // Get: api/Admin/checkAuthorize
    [Authorize(Roles = "admin")]
    [HttpGet("checkAuthorize")]
    public async Task<IActionResult> CheckAdminAuthorize()
    {
      return Ok("pass authorize.");
    }


    // GET: api/Admin/teacher/applications
    [HttpGet("teacher/applications")]
    public async Task<IActionResult> GetAllTeacherApplications()
    {
      try
      {
        var result = await _db.TeacherApplications
            .Where(data => data.isApproved == false && data.status == "unapproved")
            .OrderBy(data => data.createdTime)
            .Select(data => new
            {
              id = data.id,
              createdTime = data.createdTime.ToString("yyyy/MM/dd"),
              description = data.description
            })
            .ToListAsync();
        return Ok(result);
      }
      catch (Exception ex)
      {
        return BadRequest(ex.Message);
      }
    }

    // GET: api/Admin/teacher/applications/{id}
    [HttpGet("teacher/applications/{id}")]
    public async Task<IActionResult> GetTeacherApplicationById(long id)
    {
      try
      {
        var result = await _db.TeacherApplications
            .Where(data => data.id == id)
            .Include(a => a.certifications)
            .Select(a => a)
            .SingleOrDefaultAsync();

        return Ok(result);
      }
      catch (Exception ex)
      {
        return BadRequest(ex.Message);
      }
    }

    [HttpPost("teacher/applications/approve/{id}")]
    public async Task<IActionResult> ApproveTeacherApplication(long id)
    {
      var application = await _db.TeacherApplications
                        .Where(data => data.id == id)
                        .FirstOrDefaultAsync();
      if (application is not null)
      {
        application.isApproved = true;
        application.status = "approved";
      }

      var userId = application.userId;
      var user = await _db.Users
                  .Where(data => data.id == userId)
                  .FirstOrDefaultAsync();
      if (user is not null && user.role is not "teacher")
        user.role = "teacher";

      try
      {
        await _db.SaveChangesAsync();
        await _emailService.SendApproveApplicationMailAsync(id);
        return Ok(application);
      }
      catch (Exception ex)
      {
        return BadRequest(ex.Message);
      }
    }

    [HttpPost("teacher/applications/deny/{id}")]
    public async Task<IActionResult> DenyTeacherApplication(long id)
    {
      var application = await _db.TeacherApplications
                        .Where(data => data.id == id)
                        .FirstOrDefaultAsync();
      if (application is not null)
      {
        application.status = "denied";
      }

      try
      {
        await _db.SaveChangesAsync();
        await _emailService.SendDenyApplicationMailAsync(id);
        return Ok(application);
      }
      catch (Exception ex)
      {
        return BadRequest(ex.Message);
      }
    }
    [Authorize(Roles = "admin")]
    [HttpGet("platformData")]
    public async Task<IActionResult> GetPlatformData()
    {
      var userData = await _db.Users
                              .Where(data => data.role != "admin")
                              .CountAsync();

      var teacherData = await _db.Users
                              .Where(data => data.role == "teacher")
                              .CountAsync();

      var offlineCourseData = await _db.Courses
                                .Where(data => data.teacher.courseWay == "實體" || data.teacher.courseWay == "線下")
                                .CountAsync();

      var onlineCourseData = await _db.Courses
                                .Where(data => data.teacher.courseWay == "線上")
                                .CountAsync();

      var courseAmountData = await _db.Courses
                                      .CountAsync();

      var courseIsBookedData = await _db.Courses
                                        .Where(data => data.isBooked == true)
                                        .CountAsync();

      return Ok(new
      {
        userData = userData,
        teacherData = teacherData,
        offlineCourseData = offlineCourseData,
        onlineCourseData = onlineCourseData,
        courseAmountData = courseAmountData,
        courseIsBookedData = courseIsBookedData
      });
    }
    [Authorize(Roles = "admin")]
    [HttpGet("transactionData")]
    public async Task<IActionResult> GetTransactionData(string start, string end)
    {
      if (!DateTime.TryParse(start, out DateTime startDate) || !DateTime.TryParse(end, out DateTime endDate))
      {
        return BadRequest("Invalid date format.");
      }

      var turnoverData = await _db.Courses
                                  .Where(data => data.isBooked == true)
                                  .Where(data => data.startTime >= startDate && data.endTime <= endDate.AddDays(1))
                                  .Where(data => data.bookings.Any(booking => booking.status == "paid"))
                                  .SumAsync(data => data.price);

      var transactionData = await _db.Courses
                                  .Where(data => data.isBooked == true)
                                  .Where(data => data.startTime >= startDate && data.endTime <= endDate.AddDays(1))
                                  .Where(data => data.bookings.Any(booking => booking.status == "paid"))
                                  .CountAsync();

      var revenueData = Math.Round((decimal)(turnoverData) * 0.05M);

      return Ok(new
      {
        turnoverData = turnoverData,
        transactionData = transactionData,
        revenueData = revenueData
      });

    }

    [Authorize(Roles = "admin")]
    [HttpGet("courseData")]
    public async Task<IActionResult> GetCourseData(string start, string end)
    {
      if (!DateTime.TryParse(start, out DateTime startDate) || !DateTime.TryParse(end, out DateTime endDate))
      {
        return BadRequest("Invalid date format.");
      }

      var courseOfferingData = await _db.Courses
                                  .Where(data => data.startTime >= startDate && data.endTime <= endDate.AddDays(1))
                                  .CountAsync();

      var courseFinishedData = await _db.Courses
                                  .Where(data => data.isBooked == true)
                                  .Where(data => data.startTime >= startDate && data.endTime <= endDate.AddDays(1))
                                  .Where(data => data.bookings.Any(booking => booking.status == "paid"))
                                  .CountAsync();


      decimal achievementRate = 0;
      if (courseOfferingData != 0)
      {
        achievementRate = Math.Round(((decimal)courseFinishedData / courseOfferingData), 2);
      }

      return Ok(new
      {
        courseOfferingData = courseOfferingData,
        courseFinishedData = courseFinishedData,
        achievementRate = achievementRate
      });
    }

    [HttpGet("courseCategory")]
    public async Task<IActionResult> GetCourseCategories()
    {
      try
      {
        var categoryData = await _db.CourseCategories
                                  .Select(data => data)
                                  .OrderBy(data => data.category)
                                  .ToListAsync();
        return Ok(categoryData);
      }
      catch (Exception ex)
      {
        return BadRequest(ex.Message);
      }
    }

    [HttpPost("courseCategory")]
    public async Task<IActionResult> AddCourseCategory(CourseCategoryFormModel category)
    {
      var categoryDataExists = await _db.CourseCategories
                                        .Where(data => data.category == category.category)
                                        .AnyAsync();
      if (categoryDataExists is true)
        return StatusCode(409, "Category record repeat.");

      var newCategory = new CourseCategory
      {
        category = category.category
      };

      try
      {
        await _db.CourseCategories.AddAsync(newCategory);
        await _db.SaveChangesAsync();
        return Ok();
      }
      catch (Exception ex)
      {
        return BadRequest(ex.Message);
      }

    }
  }
}