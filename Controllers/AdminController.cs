using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using personal_project.Data;
using personal_project.Models.FormModels;
using personal_project.Services;

namespace personal_project.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class AdminController : ControllerBase
  {
    private readonly IAdminService _adminService;

    public AdminController(IAdminService adminService)
    {
      _adminService = adminService;
    }

    // Get: api/Admin/checkAuthorize
    [Authorize(Roles = "admin")]
    [HttpGet("checkAuthorize")]
    public async Task<IActionResult> CheckAdminAuthorize()
    {
      return Ok("pass authorize.");
    }


    // GET: api/Admin/teacher/applications
    [Authorize(Roles = "admin")]
    [HttpGet("teacher/applications")]
    public async Task<IActionResult> GetAllTeacherApplications()
    {

      var applications = await _adminService.GetAllUnapprovedTeacherApplicationsAsync();
      if (applications is not null)
        return Ok(applications);
      return BadRequest();
    }

    // GET: api/Admin/teacher/applications/{id}
    [HttpGet("teacher/applications/{id}")]
    public async Task<IActionResult> GetTeacherApplicationById(long id)
    {
      var application = await _adminService.GetTeacherApplicationByIdAsync(id);
      if (application is not null)
        return Ok(application);
      return BadRequest();
    }

    [HttpPost("teacher/applications/approve/{id}")]
    public async Task<IActionResult> ApproveTeacherApplication(long id)
    {
      var application = await _adminService.ApproveTeacherApplicationAsync(id);
      if (application is not null)
        return Ok(application);
      return BadRequest();
    }

    [HttpPost("teacher/applications/deny/{id}")]
    public async Task<IActionResult> DenyTeacherApplication(long id)
    {
      var application = await _adminService.DenyTeacherApplicationAsync(id);
      if (application is not null)
        return Ok(application);
      return BadRequest();
    }
    [Authorize(Roles = "admin")]
    [HttpGet("platformData")]
    public async Task<IActionResult> GetPlatformData()
    {
      var result = await _adminService.GetPlatformDataAsync();
      if (result is not null)
        return Ok(result.data);
      return BadRequest();
    }
    [Authorize(Roles = "admin")]
    [HttpGet("transactionData")]
    public async Task<IActionResult> GetTransactionData(string start, string end)
    {
      var result = await _adminService.GetTransactionDataAsync(start, end);
      if (result.statusCode == 200)
        return Ok(result.data);
      return BadRequest(result.message);
    }

    [Authorize(Roles = "admin")]
    [HttpGet("courseData")]
    public async Task<IActionResult> GetCourseData(string start, string end)
    {
      var result = await _adminService.GetCourseDataAsync(start, end);
      if (result.statusCode == 200)
        return Ok(result.data);
      return BadRequest(result.message);
    }

    [HttpGet("courseCategory")]
    public async Task<IActionResult> GetCourseCategories()
    {
      var result = await _adminService.GetCourseCategoriesAsync();
      if (result is not null)
        return Ok(result);
      return BadRequest();
    }

    [HttpPost("courseCategory")]
    public async Task<IActionResult> AddCourseCategory(CourseCategoryFormModel category)
    {
      var result = await _adminService.AddCourseCategory(category);
      if (result.statusCode == 200)
        return Ok(result.message);
      return StatusCode(result.statusCode, result.message);
    }
  }
}