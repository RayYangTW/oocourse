using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using personal_project.Models.Domain;
using personal_project.Models.FormModels;
using personal_project.Models.ResultModels;

namespace personal_project.Services
{
  public interface IAdminService
  {
    Task<AdminResult> GetAllUnapprovedTeacherApplicationsAsync();
    Task<AdminResult> GetTeacherApplicationByIdAsync(long id);
    Task<AdminResult> ApproveTeacherApplicationAsync(long id);
    Task<AdminResult> DenyTeacherApplicationAsync(long id);
    Task<AdminResult> GetPlatformDataAsync();
    Task<AdminResult> GetTransactionDataAsync(string start, string end);
    Task<AdminResult> GetCourseDataAsync(string start, string end);
    Task<List<CourseCategory>> GetCourseCategoriesAsync();
    Task<AdminResult> AddCourseCategory(CourseCategoryFormModel category);
  }
}