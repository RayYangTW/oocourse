using System.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using personal_project.Models.Domain;
using personal_project.Models.ResultModels;
using personal_project.Models.FormModels;

namespace personal_project.Services
{
  public interface ITeacherService
  {
    Task<TeacherResult> SaveTeacherApplicationAsync(TeacherApplication newApplication);
    Task<bool> CheckIfApplicationExists(User user);
    Task<TeacherApplication> GetTeacherRoleApplicationAsync(User user);
    Task<TeacherResult> PublishTeacherCourseAsync(User user, TeacherPublishCourseFormModel course);
    Task<TeacherResult> UpdateTeacherCourseAsync(User user, TeacherPublishCourseFormModel course);
    Task<TeacherResult> GetTeacherFormDataAsync(User user);
    Task<TeacherResult> GetMyCoursesAsync(User user);
    Task<TeacherResult> GetTeachingFeeAsync(User user, string start, string end);
    Task<TeacherResult> GetCourseDataAsync(User user, string start, string end);
    Task<TeacherResult> GetTeachingTimeAsync(User user, string start, string end);
    Task<TeacherResult> GetOfferingCoursesAsync(User user);
    Task<TeacherResult> DeleteCourseAsync(User user, long courseId);

  }
}