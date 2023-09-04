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

    public TeacherController(
        ILogger<TeacherController> logger,
        WebDbContext db,
        IMapper mapper,
        GetUserDataFromJWTHelper jwtHelper,
        IAmazonS3 s3Client)
    {
      _logger = logger;
      _db = db;
      _mapper = mapper;
      _jwtHelper = jwtHelper;
      _s3Client = s3Client;
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

      long userId = user.id;

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

      try
      {
        var uploadCertifications = application.certificationFiles;
        if (uploadCertifications is not null)
        {
          foreach (var formFile in uploadCertifications)
          {
            if (formFile.Length > 0)
            {
              var ext = Path.GetExtension(formFile.FileName);
              var bucketExists = await AmazonS3Util.DoesS3BucketExistV2Async(_s3Client, bucketName);
              if (!bucketExists)
              {
                var bucketRequest = new PutBucketRequest()
                {
                  BucketName = bucketName,
                  UseClientRegion = true
                };
                await _s3Client.PutBucketAsync(bucketRequest);
              }
              else
              {
                var fileToS3 = certificationFileToS3Path + GenerateFilenameHelper.GenerateFileRandomName() + application.name + ext;
                var objectRequest = new PutObjectRequest()
                {
                  BucketName = bucketName,
                  Key = fileToS3,
                  InputStream = formFile.OpenReadStream()
                };
                await _s3Client.PutObjectAsync(objectRequest);
                var newCertifications = new Certification
                {
                  certification = filesLocateDomain + fileToS3,
                  userId = user.id
                };
                newApplication.certifications.Add(newCertifications);
              }
            }
          }
        }
        // user.teacherApplication = newApplication;
        await _db.TeacherApplications.AddAsync(newApplication);
        await _db.SaveChangesAsync();
      }
      catch (Exception ex)
      {
        return BadRequest(new
        {
          error = ex.Message
        });
      }
      return Ok(newApplication);
    }

    //POST: api/teacher/publishCourse
    [Authorize]
    [HttpPost("publishCourse")]
    public async Task<IActionResult> PublishCourse([FromForm] TeacherPublishCourseFormModel course)
    {
      var user = await _jwtHelper.GetUserDataFromJWTAsync(Request.Headers["Authorization"]);
      if (user is null)
        return BadRequest("Can't find user.");

      var existingTeacherData = await _db.Teachers
                                      .Where(data => data.userId == user.id)
                                      .AnyAsync();

      if (existingTeacherData)
        return StatusCode(403, "Teacher already has own course, please don't repeat publish.");

      var newTeacher = new Teacher
      {
        courseName = course.courseName,
        courseCategory = course.courseCategory,
        courseLocation = course.courseLocation,
        courseWay = course.courseWay,
        courseLanguage = course.courseLanguage,
        courseIntro = course.courseIntro,
        courseReminder = course.courseReminder,
        userId = user.id
      };

      //處理單張照片上傳
      var uploadCourseImage = course.courseImageFile;
      if (uploadCourseImage != null)
      {
        var ext = System.IO.Path.GetExtension(uploadCourseImage.FileName);
        var bucketExists = await AmazonS3Util.DoesS3BucketExistV2Async(_s3Client, bucketName);
        if (!bucketExists)
        {
          var bucketRequest = new PutBucketRequest()
          {
            BucketName = bucketName,
            UseClientRegion = true
          };
          await _s3Client.PutBucketAsync(bucketRequest);
        }
        else
        {
          var fileToS3 = courseImageFileToS3Path + GenerateFilenameHelper.GenerateFileRandomName() + ext;
          var objectRequest = new PutObjectRequest()
          {
            BucketName = bucketName,
            Key = fileToS3,
            InputStream = uploadCourseImage.OpenReadStream()
          };
          await _s3Client.PutObjectAsync(objectRequest);
          newTeacher.courseImage = filesLocateDomain + fileToS3;
        }
      }

      try
      {
        await _db.Teachers.AddAsync(newTeacher);
        await _db.SaveChangesAsync();

        var teacher = await _db.Teachers
                        .Where(data => data.userId == user.id)
                        .FirstOrDefaultAsync();

        // Process course's data
        foreach (var courseData in course.courses)
        {
          if (courseData.startTime is not null && courseData.endTime is not null)
          {
            var newCourse = new Course
            {
              startTime = courseData.startTime,
              endTime = courseData.endTime,
              price = courseData.price,
            };
            // await _db.Courses.AddAsync(newCourse);
            teacher.courses.Add(newCourse);
          }
        }
        await _db.SaveChangesAsync();

        return Ok("publish success.");
      }
      catch (Exception ex)
      {
        return BadRequest(ex.Message);
      }
    }

    [HttpPut("updateCourse")]
    public async Task<IActionResult> UpdateCourse([FromForm] TeacherPublishCourseFormModel course)
    {
      var user = await _jwtHelper.GetUserDataFromJWTAsync(Request.Headers["Authorization"]);
      if (user is null)
        return BadRequest("Can't find user.");

      var existingTeacherData = await _db.Teachers.FirstOrDefaultAsync(data => data.userId == user.id);

      if (existingTeacherData is null)
        return NotFound("No course data for the teacher");

      existingTeacherData.courseName = course.courseName;
      existingTeacherData.courseCategory = course.courseCategory;
      existingTeacherData.courseLocation = course.courseLocation;
      existingTeacherData.courseWay = course.courseWay;
      existingTeacherData.courseLanguage = course.courseLanguage;
      existingTeacherData.courseIntro = course.courseIntro;
      existingTeacherData.courseReminder = course.courseReminder;

      //處理單張照片上傳
      var uploadCourseImage = course.courseImageFile;
      if (uploadCourseImage != null)
      {
        var ext = System.IO.Path.GetExtension(uploadCourseImage.FileName);
        var bucketExists = await AmazonS3Util.DoesS3BucketExistV2Async(_s3Client, bucketName);
        if (!bucketExists)
        {
          var bucketRequest = new PutBucketRequest()
          {
            BucketName = bucketName,
            UseClientRegion = true
          };
          await _s3Client.PutBucketAsync(bucketRequest);
        }
        else
        {
          var fileToS3 = courseImageFileToS3Path + GenerateFilenameHelper.GenerateFileRandomName() + ext;
          var objectRequest = new PutObjectRequest()
          {
            BucketName = bucketName,
            Key = fileToS3,
            InputStream = uploadCourseImage.OpenReadStream()
          };
          await _s3Client.PutObjectAsync(objectRequest);
          existingTeacherData.courseImage = filesLocateDomain + fileToS3;
        }
      }
      try
      {
        await _db.SaveChangesAsync();

        var teacher = await _db.Teachers
                        .Where(data => data.userId == user.id)
                        .FirstOrDefaultAsync();

        // Process course's data
        foreach (var courseData in course.courses)
        {
          if (courseData.startTime is not null && courseData.endTime is not null)
          {
            var newCourse = new Course
            {
              startTime = courseData.startTime,
              endTime = courseData.endTime,
              price = courseData.price,
            };
            // await _db.Courses.AddAsync(newCourse);
            teacher.courses.Add(newCourse);
          }
        }
        await _db.SaveChangesAsync();

        return Ok();
      }
      catch (Exception ex)
      {
        return BadRequest(ex.Message);
      }
    }


    [HttpGet("getTeacherFormData")]
    public async Task<IActionResult> GetTeacherFormData()
    {
      try
      {
        var user = await _jwtHelper.GetUserDataFromJWTAsync(Request.Headers["Authorization"]);
        if (user is null)
          return BadRequest("Can't find user.");

        var formData = await _db.Teachers
                            .Where(data => data.userId == user.id)
                            .FirstOrDefaultAsync();
        if (formData is not null)
          return Ok(formData);
        return NoContent();
      }
      catch (Exception ex)
      {
        return BadRequest(ex.Message);
      }
    }

    [HttpGet("myCourses")]
    public async Task<IActionResult> GetMyCourses()
    {
      try
      {
        var user = await _jwtHelper.GetUserDataFromJWTAsync(Request.Headers["Authorization"]);
        if (user is null)
          return BadRequest("Can't find user.");

        var coursesData = await _db.Courses
                                .Where(data => data.teacher.userId == user.id)
                                .Where(data => data.isBooked == true)
                                .Include(data => data.teacher)
                                .ToListAsync();

        var responseData = _mapper.Map<List<CoursesResponseDto>>(coursesData);

        foreach (var course in responseData)
        {
          course.startTime = ConvertDateTimeFormatHelper.ConvertDateTimeFormat(course.startTime);
          course.endTime = ConvertDateTimeFormatHelper.ConvertDateTimeFormat(course.endTime);
        }
        responseData = responseData.OrderBy(data => data.startTime).ToList();

        return Ok(responseData);
      }
      catch (Exception ex)
      {
        return BadRequest(ex.Message);
      }
    }

    [HttpGet("teachingFeeData")]
    public async Task<IActionResult> GetTeachingFee(string start, string end)
    {
      try
      {
        var user = await _jwtHelper.GetUserDataFromJWTAsync(Request.Headers["Authorization"]);
        if (user is null)
          return BadRequest("Can't find user.");

        if (!DateTime.TryParse(start, out DateTime startDate) || !DateTime.TryParse(end, out DateTime endDate))
        {
          return BadRequest("Invalid date format.");
        }

        var estimatedAmountData = await _db.Courses
                                        .Where(data => data.teacher.userId == user.id)
                                        .Where(data => data.startTime >= startDate && data.endTime <= endDate.AddDays(1))
                                        .SumAsync(data => data.price);

        var teachingFeeData = await _db.Courses
                                        .Where(data => data.teacher.userId == user.id)
                                        .Where(data => data.isBooked == true)
                                        .Where(data => data.startTime >= startDate && data.endTime <= endDate.AddDays(1))
                                        .Where(data => data.bookings.Any(booking => booking.status == "paid"))
                                        .SumAsync(data => data.price);

        decimal achievementRate = 0;
        if (estimatedAmountData != 0)
        {
          achievementRate = Math.Round((decimal)((teachingFeeData / estimatedAmountData)), 2);
        }

        var platformFeeOfTeachingFee = Math.Round((decimal)(teachingFeeData * 0.05));

        var platformFeeOfEstimatedAmount = Math.Round((decimal)(estimatedAmountData * 0.05), 2);

        return Ok(new
        {
          estimatedAmountData = estimatedAmountData,
          teachingFeeData = teachingFeeData,
          achievementRate = achievementRate,
          platformFeeOfTeachingFee = platformFeeOfTeachingFee,
          platformFeeOfEstimatedAmount = platformFeeOfEstimatedAmount,
        });
      }
      catch (Exception ex)
      {
        return BadRequest(ex.Message);
      }
    }

    [HttpGet("CourseData")]
    public async Task<IActionResult> GetCourseData(string start, string end)
    {
      try
      {
        var user = await _jwtHelper.GetUserDataFromJWTAsync(Request.Headers["Authorization"]);
        if (user is null)
          return BadRequest("Can't find user.");

        if (!DateTime.TryParse(start, out DateTime startDate) || !DateTime.TryParse(end, out DateTime endDate))
        {
          return BadRequest("Invalid date format.");
        }

        var taughtCourseAmount = await _db.Courses
                                        .Where(data => data.teacher.userId == user.id)
                                        .Where(data => data.isBooked == true)
                                        .Where(data => data.startTime >= startDate && data.endTime <= endDate.AddDays(1))
                                        .Where(data => data.bookings.Any(booking => booking.status == "paid"))
                                        .CountAsync();

        var openCourseAmount = await _db.Courses
                                        .Where(data => data.teacher.userId == user.id)
                                        .Where(data => data.startTime >= startDate && data.endTime <= endDate.AddDays(1))
                                        .CountAsync();
        decimal achievementRate = 0;
        if (openCourseAmount != 0)
        {
          achievementRate = Math.Round(((decimal)taughtCourseAmount / (decimal)openCourseAmount), 2);
        }

        return Ok(new
        {
          taughtCourseAmount = taughtCourseAmount,
          openCourseAmount = openCourseAmount,
          achievementRate = achievementRate,
        });
      }
      catch (Exception ex)
      {
        return BadRequest(ex.Message);
      }
    }

    [HttpGet("teachingTimeData")]
    public async Task<IActionResult> GetTeachingTime(string start, string end)
    {
      try
      {
        var user = await _jwtHelper.GetUserDataFromJWTAsync(Request.Headers["Authorization"]);
        if (user is null)
          return BadRequest("Can't find user.");

        if (!DateTime.TryParse(start, out DateTime startDate) || !DateTime.TryParse(end, out DateTime endDate))
        {
          return BadRequest("Invalid date format.");
        }

        var teachingTimeData = await _db.Courses
                                        .Where(data => data.teacher.userId == user.id)
                                        .Where(data => data.isBooked == true)
                                        .Where(data => data.startTime >= startDate && data.endTime <= endDate.AddDays(1))
                                        .Where(data => data.bookings.Any(booking => booking.status == "paid"))
                                        .Select(data => data.duration)
                                        .ToListAsync();

        TimeSpan? totalDuration = TimeSpan.Zero;

        foreach (var duration in teachingTimeData)
        {
          totalDuration += duration;
        }

        return Ok(new
        {
          totalDuration = totalDuration
        });
      }
      catch (Exception ex)
      {
        return BadRequest(ex.Message);
      }
    }

  }
}