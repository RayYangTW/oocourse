using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using personal_project.Data;
using personal_project.Models.Domain;
using personal_project.Models.Dtos;
using personal_project.Models.ResultModels;
using personal_project.Models.User;

namespace personal_project.Services
{
  public interface IUserService
  {
    Task<UserResult> SignupAsync(BaseUser signupUser);
    Task<UserResult> SigninAsync(SigninUser signinUser);
    Task<UserResult> GetProfileAsync(HttpContext httpContext, WebDbContext _db);
    Task<UserResult> UploadProfileAsync(HttpContext httpContext, WebDbContext _db, Models.Domain.Profile userProfile);
    Task<ProfileDto> UpdateProfileAsync(HttpContext httpContext, WebDbContext _db, Models.Domain.Profile userProfile, IFormFile avatarFile);
    Task<List<BookingsResponseDto>> GetBookingsAsync(HttpContext httpContext, WebDbContext _db);
    Task<UserResult> GetApplicationDefaultDataAsync(HttpContext httpContext, WebDbContext _db);
  }
}