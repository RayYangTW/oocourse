using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using personal_project.Data;
using personal_project.Helpers;
using personal_project.Models.Domain;
using personal_project.Models.Dtos;
using personal_project.Models.FormModels;
using personal_project.Models.ResultModels;

namespace personal_project.Services
{
  public class TeacherService : ITeacherService
  {
    private readonly WebDbContext _db;
    private readonly IFileUploadService _fileUploadService;
    private readonly IMapper _mapper;

    public TeacherService(WebDbContext db, IFileUploadService fileUploadService, IMapper mapper)
    {
      _db = db;
      _fileUploadService = fileUploadService;
      _mapper = mapper;
    }

    public async Task<TeacherResult> SaveTeacherApplicationAsync(TeacherApplication newApplication)
    {
      try
      {
        await _db.TeacherApplications.AddAsync(newApplication);
        await _db.SaveChangesAsync();
        return new TeacherResult
        {
          statusCode = 200,
        };
      }
      catch (Exception ex)
      {
        return new TeacherResult
        {
          statusCode = 400,
          message = "Error of saving TeacherApplication"
        };
      }
    }

    public async Task<bool> CheckIfApplicationExists(User user)
    {
      var applicationExists = await _db.TeacherApplications
                                    .Where(data => data.userId == user.id && data.status == "unapproved")
                                    .AnyAsync();
      if (applicationExists is true)
        return true;
      return false;
    }

    public async Task<TeacherApplication> GetTeacherRoleApplicationAsync(User user)
    {
      var myTeacherRoleApplication = await _db.TeacherApplications
                                              .Where(data => data.userId == user.id && data.status == "unapproved")
                                              .FirstOrDefaultAsync();
      return myTeacherRoleApplication;
    }

    public async Task<TeacherResult> PublishTeacherCourseAsync(User user, TeacherPublishCourseFormModel course)
    {
      try
      {
        var existingTeacherData = await _db.Teachers
            .Where(data => data.userId == user.id)
            .AnyAsync();

        if (existingTeacherData)
          return new TeacherResult { statusCode = 403, message = "Teacher already has own course, please don't repeat publish." };

        var newTeacher = new Models.Domain.Teacher
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
          string bucketName = "teach-web-s3-bucket";
          string filesLocateDomain = "https://d3n4wxuzv8xzhg.cloudfront.net/";
          string fileToS3Path = "teacher/course/images/";

          var filePath = await _fileUploadService.UploadFileToS3Async(uploadCourseImage, bucketName, fileToS3Path);

          newTeacher.courseImage = filesLocateDomain + filePath;
        }

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
            teacher.courses.Add(newCourse);
          }
        }
        await _db.SaveChangesAsync();

        return new TeacherResult { statusCode = 200, message = "publish success." };
      }
      catch (Exception ex)
      {
        return new TeacherResult { statusCode = 400, message = ex.Message };
      }
    }

    public async Task<TeacherResult> UpdateTeacherCourseAsync(User user, TeacherPublishCourseFormModel course)
    {
      try
      {
        var existingTeacherData = await _db.Teachers.FirstOrDefaultAsync(data => data.userId == user.id);

        if (existingTeacherData is null)
          return new TeacherResult { statusCode = 404, message = "No course data for the teacher" };

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
          string bucketName = "teach-web-s3-bucket";
          string filesLocateDomain = "https://d3n4wxuzv8xzhg.cloudfront.net/";
          string fileToS3Path = "teacher/course/images/";

          var filePath = await _fileUploadService.UploadFileToS3Async(uploadCourseImage, bucketName, fileToS3Path);

          existingTeacherData.courseImage = filesLocateDomain + filePath;
        }

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
            teacher.courses.Add(newCourse);
          }
        }
        await _db.SaveChangesAsync();

        return new TeacherResult { statusCode = 200, message = "Update success." };
      }
      catch (Exception ex)
      {
        return new TeacherResult { statusCode = 400, message = ex.Message };
      }
    }

    public async Task<TeacherResult> GetTeacherFormDataAsync(User user)
    {
      try
      {
        var formData = await _db.Teachers
                            .Where(data => data.userId == user.id)
                            .FirstOrDefaultAsync();
        if (formData is not null)
          return new TeacherResult { statusCode = 200, data = formData };
        return new TeacherResult { statusCode = 204 };
      }
      catch (Exception ex)
      {
        return new TeacherResult { statusCode = 400, message = ex.Message };
      }
    }

    public async Task<TeacherResult> GetMyCoursesAsync(User user)
    {
      try
      {
        var coursesData = await _db.Courses
                                .Where(data => data.teacher.userId == user.id)
                                .Where(data => data.startTime >= DateTime.Now)
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

        return new TeacherResult { statusCode = 200, data = responseData };
      }
      catch (Exception ex)
      {
        return new TeacherResult { statusCode = 400, message = ex.Message };
      }
    }

    public async Task<TeacherResult> GetTeachingFeeAsync(User user, string start, string end)
    {
      try
      {
        if (!DateTime.TryParse(start, out DateTime startDate) || !DateTime.TryParse(end, out DateTime endDate))
          return new TeacherResult { statusCode = 400, message = "Invalid date format." };


        var estimatedAmountData = await _db.Courses
                                        .Where(data => data.teacher.userId == user.id)
                                        .Where(data => data.startTime >= startDate && data.endTime <= endDate.AddDays(1))
                                        .SumAsync(data => data.price);
        var originEstimatedAmount = estimatedAmountData;
        estimatedAmountData = estimatedAmountData - estimatedAmountData * 0.05;

        var teachingFeeData = await _db.Courses
                                        .Where(data => data.teacher.userId == user.id)
                                        .Where(data => data.isBooked == true)
                                        .Where(data => data.startTime >= startDate && data.endTime <= endDate.AddDays(1))
                                        .Where(data => data.bookings.Any(booking => booking.status == "paid"))
                                        .SumAsync(data => data.price);
        var originTeachingFee = teachingFeeData;
        teachingFeeData = teachingFeeData - teachingFeeData * 0.05;


        decimal achievementRate = 0;
        if (estimatedAmountData != 0)
        {
          achievementRate = Math.Round((decimal)((teachingFeeData / estimatedAmountData)), 2);
        }

        var platformFeeOfTeachingFee = Math.Round((decimal)(originTeachingFee * 0.05));

        var platformFeeOfEstimatedAmount = Math.Round((decimal)(originEstimatedAmount * 0.05));

        return new TeacherResult
        {
          statusCode = 200,
          data = new
          {
            originTeachingFee = originTeachingFee,
            originEstimatedAmount = originEstimatedAmount,
            estimatedAmountData = estimatedAmountData,
            teachingFeeData = teachingFeeData,
            achievementRate = achievementRate,
            platformFeeOfTeachingFee = platformFeeOfTeachingFee,
            platformFeeOfEstimatedAmount = platformFeeOfEstimatedAmount,
          }
        };
      }
      catch (Exception ex)
      {
        return new TeacherResult { statusCode = 400, message = ex.Message };
      }
    }

    public async Task<TeacherResult> GetCourseDataAsync(User user, string start, string end)
    {
      try
      {
        if (!DateTime.TryParse(start, out DateTime startDate) || !DateTime.TryParse(end, out DateTime endDate))
          return new TeacherResult { statusCode = 400, message = "Invalid date format." };

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

        return new TeacherResult
        {
          statusCode = 200,
          data = new
          {
            taughtCourseAmount = taughtCourseAmount,
            openCourseAmount = openCourseAmount,
            achievementRate = achievementRate,
          }
        };
      }
      catch (Exception ex)
      {
        return new TeacherResult { statusCode = 400, message = ex.Message };
      }
    }

    public async Task<TeacherResult> GetTeachingTimeAsync(User user, string start, string end)
    {
      try
      {
        if (!DateTime.TryParse(start, out DateTime startDate) || !DateTime.TryParse(end, out DateTime endDate))
          return new TeacherResult { statusCode = 400, message = "Invalid date format." };

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

        return new TeacherResult
        {
          statusCode = 200,
          data = new
          {
            totalDuration = totalDuration
          }
        };
      }
      catch (Exception ex)
      {
        return new TeacherResult { statusCode = 400, message = ex.Message };
      }
    }

    public async Task<TeacherResult> GetOfferingCoursesAsync(User user)
    {
      try
      {
        var afterThreeHours = DateTime.Now.AddHours(3);
        var offeringCoursesData = await _db.Courses
                                          .Where(data => data.teacher.userId == user.id)
                                          .Where(data => data.startTime >= afterThreeHours)
                                          .Include(data => data.teacher)
                                          .ToListAsync();
        if (offeringCoursesData is null || offeringCoursesData.Count() <= 0)
          return new TeacherResult { statusCode = 204 };

        var resultDto = _mapper.Map<List<CourseOfferingDto>>(offeringCoursesData);
        foreach (var course in resultDto)
        {
          course.startTime = ConvertDateTimeFormatHelper.ConvertDateTimeFormat(course.startTime);
          course.endTime = ConvertDateTimeFormatHelper.ConvertDateTimeFormat(course.endTime);
        }
        resultDto = resultDto.OrderBy(course => course.startTime).ToList();
        return new TeacherResult { statusCode = 200, data = resultDto };
      }
      catch (Exception ex)
      {
        return new TeacherResult { statusCode = 400, message = ex.Message };
      }
    }

    public async Task<TeacherResult> DeleteCourseAsync(User user, long courseId)
    {
      try
      {
        var courseData = await _db.Courses
                                .Where(data => data.id == courseId)
                                .Where(data => data.teacher.userId == user.id)
                                .FirstOrDefaultAsync();
        if (courseData is null)
          return new TeacherResult { statusCode = 204 };

        _db.Courses.Remove(courseData);
        await _db.SaveChangesAsync();

        return new TeacherResult { statusCode = 200 };
      }
      catch (Exception ex)
      {
        return new TeacherResult { statusCode = 400, message = ex.Message };
      }
    }
  }
}