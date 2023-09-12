using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using personal_project.Data;
using personal_project.Helpers;
using personal_project.Models.Domain;
using personal_project.Models.Dtos;
using static personal_project.Models.Dtos.CourseDetailDto;

namespace personal_project.Services
{
  public class CourseService : ICourseService
  {
    private readonly ILogger<CourseService> _logger;
    private readonly WebDbContext _db;
    private readonly IMapper _mapper;
    private readonly GetUserDataFromJWTHelper _jwtHelper;
    public CourseService(ILogger<CourseService> logger, WebDbContext db, IMapper mapper, GetUserDataFromJWTHelper jwtHelper)
    {
      _logger = logger;
      _db = db;
      _mapper = mapper;
      _jwtHelper = jwtHelper;
    }
    public async Task<List<Teacher>> GetAllTeachersAsync()
    {
      return await _db.Teachers.ToListAsync();
    }

    public async Task<CourseDetailDTO> GetCourseDetailAsync(long id)
    {
      var afterThreeHours = DateTime.Now.AddHours(3);
      try
      {
        var courseData = await _db.Teachers
                            .Where(data => data.id == id)
                            .Include(data => data.courses
                                                  .Where(data => data.isBooked == false)
                                                  .Where(data => data.startTime >= afterThreeHours))
                            .FirstOrDefaultAsync();

        if (courseData is not null)
        {
          var resultDto = _mapper.Map<CourseDetailDTO>(courseData);
          foreach (var course in resultDto.courses)
          {
            course.startTime = ConvertDateTimeFormatHelper.ConvertDateTimeFormat(course.startTime);
            course.endTime = ConvertDateTimeFormatHelper.ConvertDateTimeFormat(course.endTime);
          }
          resultDto.courses = resultDto.courses.OrderBy(course => course.startTime).ToList();
          return resultDto;
        }
        return null;
      }
      catch (Exception ex)
      {
        throw new Exception();
      }
    }

    public async Task<List<Teacher>> GetTeachersByKeywordAsync(string keyword)
    {
      var courseData = await _db.Teachers
                                .Where(data =>
                                    data.courseName.Contains(keyword) ||
                                    data.courseCategory.Contains(keyword) ||
                                    data.courseLocation.Contains(keyword) ||
                                    data.courseWay.Contains(keyword))
                                .ToListAsync();

      if (courseData.Count() > 0)
        return courseData;

      return null;
    }

    public async Task<BookingDetailDto> GetCourseDetailByRoomIdAsync(string roomId, string authorizationHeader)
    {
      try
      {
        var user = await _jwtHelper.GetUserDataFromJWTAsync(authorizationHeader);
        if (user is null)
          return null;

        var courseData = await _db.Courses
            .Where(data => data.roomId == roomId)
            .Include(data => data.teacher)
            .FirstOrDefaultAsync();

        if (courseData is null)
          return null;

        var responseData = _mapper.Map<BookingDetailDto>(courseData);
        responseData.startTime = ConvertDateTimeFormatHelper.ConvertDateTimeFormat(responseData.startTime);
        responseData.endTime = ConvertDateTimeFormatHelper.ConvertDateTimeFormat(responseData.endTime);

        return responseData;
      }
      catch (Exception ex)
      {
        throw;
      }
    }

    public async Task<bool> HasAccessToOnlineCourseAsync(long userId, string roomId)
    {
      var courseAccess = await _db.CourseAccessLists
          .Where(data => data.roomId == roomId)
          .FirstOrDefaultAsync();

      if (courseAccess is not null && (courseAccess.teacherUserId == userId || courseAccess.userId == userId))
        return true;

      return false;
    }
  }
}