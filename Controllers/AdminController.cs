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
  public class AdminController : ControllerBase
  {

    private readonly ILogger<AdminController> _logger;
    private readonly WebDbContext _db;
    private readonly IMapper _mapper;


    public AdminController(ILogger<AdminController> logger, WebDbContext db, IMapper mapper)
    {
      _logger = logger;
      _db = db;
      _mapper = mapper;
    }

    // GET: api/Admin/teacher/applications
    [HttpGet("teacher/applications")]
    public async Task<IActionResult> GetAllTeacherApplications()
    {
      try
      {
        var result = await _db.TeacherApplications
            .Where(data => data.isApproved == false && data.status != "denied")
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
        return Ok(application);
      }
      catch (Exception ex)
      {
        return BadRequest(ex.Message);
      }
    }
  }
}