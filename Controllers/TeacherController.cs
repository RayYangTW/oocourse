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

    // POST: api/teacher/application
    [Authorize]
    [HttpPost("application")]
    public async Task<IActionResult> ApplyForTeacherRole([FromForm] TeacherApplicationFormModel application)
    {
      var user = await _jwtHelper.GetUserDataFromJWTAsync(Request.Headers["Authorization"]);
      if (user is null)
        return BadRequest("Missing authorization token.");

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
        userId = user.id
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
                  certification = filesLocateDomain + fileToS3
                };
                newApplication.certifications.Add(newCertifications);
              }
            }
          }
        }
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
      var teacher = await _jwtHelper.GetUserDataFromJWTAsync(Request.Headers["Authorization"]);
      if (teacher is null)
        return BadRequest("Missing authorization token.");

      var newTeacher = new Teacher
      {
        courseName = course.courseName,
        courseCategory = course.courseCategory,
        courseLocation = course.courseLocation,
        courseWay = course.courseWay,
        courseReminder = course.courseReminder,
        userId = teacher.id
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
        // Process course's data
        foreach (var courseData in course.courses)
        {
          var newCourse = new Course
          {
            startTime = courseData.startTime,
            endTime = courseData.endTime,
            price = courseData.price,
            teacherId = teacher.id
          };
          await _db.Courses.AddAsync(newCourse);
        }

        await _db.Teachers.AddAsync(newTeacher);
        await _db.SaveChangesAsync();

        return Ok(newTeacher);
      }
      catch (Exception ex)
      {
        return BadRequest(ex.Message);
      }

    }
  }
}