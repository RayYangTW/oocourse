using System.Reflection.Emit;
using System.Security.Cryptography;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using personal_project.Data;
using personal_project.Helpers;
using personal_project.Models.Domain;
using personal_project.Models.FormModels;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using personal_project.Models.Dtos;
using personal_project.Services;

namespace personal_project.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class TeacherController : ControllerBase
  {
    private readonly ILogger<TeacherController> _logger;
    private readonly WebDbContext _db;
    private readonly IMapper _mapper;
    private readonly GetUserDataFromJWTHelper _jwtHelper;
    private readonly IAmazonS3 _s3Client;
    private readonly IFileUploadService _fileUploadService;
    private readonly ITeacherService _teacherService;

    public TeacherController(
        ILogger<TeacherController> logger,
        WebDbContext db,
        IMapper mapper,
        GetUserDataFromJWTHelper jwtHelper,
        IAmazonS3 s3Client,
        IFileUploadService fileUploadService,
        ITeacherService teacherService)
    {
      _logger = logger;
      _db = db;
      _mapper = mapper;
      _jwtHelper = jwtHelper;
      _s3Client = s3Client;
      _fileUploadService = fileUploadService;
      _teacherService = teacherService;
    }

    // Global variables
    string bucketName = "teach-web-s3-bucket";
    string filesLocateDomain = "https://d3n4wxuzv8xzhg.cloudfront.net/";
    string certificationFileToS3Path = "teacher/certification/";
    string courseImageFileToS3Path = "teacher/course/images/";

    // GET: api/teacher/checkAuthorize
    [Authorize(Roles = "teacher")]
    [HttpGet("checkAuthorize")]
    public async Task<IActionResult> CheckTeacherAuthorize()
    {
      return Ok("pass authorize.");
    }

    // POST: api/teacher/application
    [Authorize]
    [HttpPost("application")]
    public async Task<IActionResult> ApplyForTeacherRole([FromForm] TeacherApplicationFormModel application)
    {
      var user = await _jwtHelper.GetUserDataFromJWTAsync(Request.Headers["Authorization"]);
      if (user is null)
        return BadRequest("Can't find user.");

      var applicationExists = await _teacherService.CheckIfApplicationExists(user);
      if (applicationExists is true)
        return StatusCode(403, "Teacher role application is already exist.");

      var newApplication = new TeacherApplication
      {
        name = application.name,
        email = application.email,
        category = application.category,
        language = application.language,
        country = application.country,
        location = application.location,
        experience = application.experience,
        description = application.description,
        isApproved = false,
        certifications = new List<Certification>(),
        user = user
      };

      var uploadCertifications = application.certificationFiles;
      var applicationToSave = await _fileUploadService.UploadCertificationsAsync(uploadCertifications, newApplication, user, bucketName, certificationFileToS3Path, filesLocateDomain);
      var result = await _teacherService.SaveTeacherApplicationAsync(applicationToSave);
      if (result.statusCode == 200)
        return Ok(applicationToSave);
      return StatusCode(result.statusCode, result.message);
    }

    [Authorize]
    [HttpGet("application")]
    public async Task<IActionResult> GetTeacherRoleApplication()
    {
      var user = await _jwtHelper.GetUserDataFromJWTAsync(Request.Headers["Authorization"]);
      if (user is null)
        return BadRequest("Can't find user.");

      var myTeacherRoleApplication = await _teacherService.GetTeacherRoleApplicationAsync(user);
      if (myTeacherRoleApplication is not null)
        return Ok(myTeacherRoleApplication);
      return NotFound("Can't get the application");
    }

    //POST: api/teacher/publishCourse
    [Authorize]
    [HttpPost("publishCourse")]
    public async Task<IActionResult> PublishCourse([FromForm] TeacherPublishCourseFormModel course)
    {
      var user = await _jwtHelper.GetUserDataFromJWTAsync(Request.Headers["Authorization"]);
      if (user is null)
        return BadRequest("Can't find user.");

      var result = await _teacherService.PublishTeacherCourseAsync(user, course);
      return StatusCode(result.statusCode, result.message);
    }

    [HttpPut("updateCourse")]
    public async Task<IActionResult> UpdateCourse([FromForm] TeacherPublishCourseFormModel course)
    {
      var user = await _jwtHelper.GetUserDataFromJWTAsync(Request.Headers["Authorization"]);
      if (user is null)
        return BadRequest("Can't find user.");

      var result = await _teacherService.UpdateTeacherCourseAsync(user, course);
      return StatusCode(result.statusCode, result.message);
    }


    [HttpGet("getTeacherFormData")]
    public async Task<IActionResult> GetTeacherFormData()
    {

      var user = await _jwtHelper.GetUserDataFromJWTAsync(Request.Headers["Authorization"]);
      if (user is null)
        return BadRequest("Can't find user.");

      var result = await _teacherService.GetTeacherFormDataAsync(user);

      if (result.statusCode == 200)
        return Ok(result.data);
      else if (result.statusCode == 204)
        return NoContent();
      else
        return BadRequest(result.message);
    }

    [HttpGet("myCourses")]
    public async Task<IActionResult> GetMyCourses()
    {

      var user = await _jwtHelper.GetUserDataFromJWTAsync(Request.Headers["Authorization"]);
      if (user is null)
        return BadRequest("Can't find user.");

      var result = await _teacherService.GetMyCoursesAsync(user);
      if (result.statusCode == 200)
        return Ok(result.data);
      return BadRequest(result.message);
    }

    [HttpGet("teachingFeeData")]
    public async Task<IActionResult> GetTeachingFee(string start, string end)
    {

      var user = await _jwtHelper.GetUserDataFromJWTAsync(Request.Headers["Authorization"]);
      if (user is null)
        return BadRequest("Can't find user.");

      var result = await _teacherService.GetTeachingFeeAsync(user, start, end);
      if (result.statusCode == 200)
        return Ok(result.data);
      return BadRequest(result.message);
    }

    [HttpGet("CourseData")]
    public async Task<IActionResult> GetCourseData(string start, string end)
    {

      var user = await _jwtHelper.GetUserDataFromJWTAsync(Request.Headers["Authorization"]);
      if (user is null)
        return BadRequest("Can't find user.");

      var result = await _teacherService.GetCourseDataAsync(user, start, end);
      if (result.statusCode == 200)
        return Ok(result.data);
      return BadRequest(result.message);
    }

    [HttpGet("teachingTimeData")]
    public async Task<IActionResult> GetTeachingTime(string start, string end)
    {

      var user = await _jwtHelper.GetUserDataFromJWTAsync(Request.Headers["Authorization"]);
      if (user is null)
        return BadRequest("Can't find user.");

      var result = await _teacherService.GetTeachingTimeAsync(user, start, end);
      if (result.statusCode == 200)
        return Ok(result.data);
      return BadRequest(result.message);
    }

    [HttpGet("offeringCourses")]
    public async Task<IActionResult> GetOfferingCourses()
    {
      var user = await _jwtHelper.GetUserDataFromJWTAsync(Request.Headers["Authorization"]);
      if (user is null)
        return BadRequest("Can't find user.");

      var result = await _teacherService.GetOfferingCoursesAsync(user);
      if (result.statusCode == 200)
        return Ok(result.data);
      return BadRequest(result.message);
    }

    [HttpDelete("cancelCourse/{courseId}")]
    public async Task<IActionResult> DeleteCourse(long courseId)
    {
      var user = await _jwtHelper.GetUserDataFromJWTAsync(Request.Headers["Authorization"]);
      if (user is null)
        return BadRequest("Can't find user.");

      var result = await _teacherService.DeleteCourseAsync(user, courseId);
      if (result.statusCode == 200)
        return Ok();
      else if (result.statusCode == 204)
        return NoContent();
      else
        return BadRequest(result.message);
    }
  }
}