using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using personal_project.Models.Domain;
using personal_project.Models.Dtos;
using static personal_project.Models.Dtos.CourseDetailDto;

namespace personal_project.Services
{
  public interface ICourseService
  {
    Task<List<Teacher>> GetAllTeachersAsync();
    Task<List<Teacher>> GetTeachersByKeywordAsync(string keyword);
    Task<CourseDetailDTO> GetCourseDetailAsync(long id);
    Task<BookingDetailDto> GetCourseDetailByRoomIdAsync(string roomId, string authorizationHeader);
    Task<bool> HasAccessToOnlineCourseAsync(long userId, string roomId);

  }
}