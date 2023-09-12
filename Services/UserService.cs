using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using personal_project.Data;
using personal_project.Helpers;
using personal_project.Models.Domain;
using personal_project.Models.Dtos;
using personal_project.Models.ResultModels;
using personal_project.Models.User;

namespace personal_project.Services
{
  public class UserService : IUserService
  {
    private readonly ILogger<UserService> _logger;
    private readonly WebDbContext _db;
    private readonly IMapper _mapper;
    private readonly GetUserDataFromJWTHelper _jwtHelper;
    private readonly IConfiguration _config;
    private readonly IEmailService _emailService;
    private readonly IFileUploadService _fileUploadService;

    public UserService(ILogger<UserService> logger, WebDbContext db, IMapper mapper, GetUserDataFromJWTHelper jwtHelper, IConfiguration config, IEmailService emailService, IFileUploadService fileUploadService)
    {
      _logger = logger;
      _db = db;
      _mapper = mapper;
      _jwtHelper = jwtHelper;
      _config = config;
      _emailService = emailService;
      _fileUploadService = fileUploadService;
    }
    public async Task<UserResult> SignupAsync(BaseUser signupUser)
    {
      if (signupUser is null)
        return new UserResult
        {
          statusCode = 400,
          message = "No request"
        };
      if (signupUser.email is null || signupUser.password is null)
        return new UserResult
        {
          statusCode = 400,
          message = "Email and Password fields all are required!"
        };

      // Check if email exists.
      var emailExists = await _db.Users.AnyAsync(u => u.email == signupUser.email);
      if (emailExists)
        return new UserResult
        {
          statusCode = 403,
          message = "Email existed!"
        };

      //// To store user info
      // Hash the password
      var hashPassword = BCrypt.Net.BCrypt.HashPassword(signupUser.password);

      // new User
      var newUser = new User
      {
        provider = "native",
        email = signupUser.email,
        password = hashPassword
      };

      // store it
      await _db.Users.AddAsync(newUser);
      await _db.SaveChangesAsync();

      // To set default value for profile
      var userId = await _db.Users
                    .Where(data => data.email == signupUser.email)
                    .Select(data => data.id)
                    .SingleOrDefaultAsync();

      var newProfile = new Models.Domain.Profile
      {
        userId = userId,
        name = "",
        nickname = "",
        gender = "",
        interest = ""
      };

      await _db.Profiles.AddAsync(newProfile);
      await _db.SaveChangesAsync();

      //// To create JWT
      var newBaseUser = _mapper.Map<BaseUser>(newUser);
      var jwt = JwtHelper.CreateJWT(newBaseUser);

      //// Return response data
      return new UserResult
      {
        statusCode = 200,
        data = new
        {
          access_token = jwt,
          access_expired = JwtHelper.GetTokenExpirationTime(jwt),
          user = new
          {
            id = newUser.id,
            email = newUser.email,
            provider = newUser.provider
          }
        }
      };

    }

    public async Task<UserResult> SigninAsync(SigninUser signinUser)
    {
      // ===Native Signin===
      if (signinUser.provider.ToLower() == "native")
      {
        // Find if user exist
        var user = await _db.Users
                            .Where(data => data.email == signinUser.email)
                            .Include(data => data.profile)
                            .FirstOrDefaultAsync();

        if (user is null)
          return new UserResult
          {
            statusCode = 403,
            message = "Email didn't exist!"
          };

        // Check password verify hashpassword
        var passwordCheck = BCrypt.Net.BCrypt.Verify(signinUser.password, user.password);
        if (!passwordCheck)
          return new UserResult
          {
            statusCode = 403,
            message = "Incorrect Email or Password!"
          };

        // Generate jwt
        var newBaseUser = _mapper.Map<BaseUser>(user);
        var jwt = JwtHelper.CreateJWT(newBaseUser);

        // Get user role from JWT
        var readJwt = new JwtSecurityTokenHandler().ReadJwtToken(jwt);
        var userRole = readJwt.Claims.FirstOrDefault(a => a.Type == "role")?.Value;

        //// Return response data
        return new UserResult
        {
          statusCode = 200,
          data = new
          {
            access_token = jwt,
            access_expired = JwtHelper.GetTokenExpirationTime(jwt),
            user = new
            {
              id = user.id,
              email = user.email,
              provider = user.provider,
              role = userRole,
              isProfileCompleted = user.isProfileCompleted,
              userName = user.profile.name
            }
          }
        };
      }
      return new UserResult
      {
        statusCode = 400,
        message = "Unknown signin source!"
      };
    }

    public async Task<UserResult> GetProfileAsync(HttpContext httpContext, WebDbContext _db)
    {
      var user = await JwtHelper.GetUserFromJWTAsync(httpContext, _db);
      if (user is null)
        return new UserResult
        {
          statusCode = 404,
          message = "Can't find user."
        };

      // Get user from Profiles
      var profile = await _db.Profiles.FirstOrDefaultAsync(p => p.userId == user.id);
      if (profile is null)
        return new UserResult
        {
          statusCode = 404,
          message = "Profile not found for the user."
        };

      return new UserResult
      {
        statusCode = 200,
        data = new
        {
          name = profile.name,
          nickname = profile.nickname,
          avatar = profile.avatar,
          gender = profile.gender,
          interest = profile.interest
        }
      };
    }

    public async Task<UserResult> UploadProfileAsync(HttpContext httpContext, WebDbContext _db, Models.Domain.Profile userProfile)
    {
      var user = await JwtHelper.GetUserFromJWTAsync(httpContext, _db);
      if (user is null)
        return new UserResult
        {
          statusCode = 404,
          message = "Can't find user."
        };

      var existingProfile = await _db.Profiles.FirstOrDefaultAsync(p => p.userId == user.id);

      if (existingProfile is not null)
      {
        existingProfile.name = userProfile.name;
        existingProfile.nickname = userProfile.nickname;
        existingProfile.avatar = userProfile.avatar;
        existingProfile.gender = userProfile.gender;
        existingProfile.interest = userProfile.interest;

        await _db.SaveChangesAsync();
        return new UserResult
        {
          statusCode = 200,
          message = "Profile uploaded."
        };
      }

      var newProfile = new Models.Domain.Profile
      {
        name = userProfile.name,
        nickname = userProfile.nickname,
        avatar = userProfile.avatar,
        gender = userProfile.gender,
        interest = userProfile.interest,
        user = user
      };

      await _db.Profiles.AddAsync(newProfile);
      await _db.SaveChangesAsync();

      // update user.isProfileCompleted to true
      user.isProfileCompleted = true;
      await _db.SaveChangesAsync();
      return new UserResult
      {
        statusCode = 200,
        message = "New profile saved."
      };
    }

    public async Task<ProfileDto> UpdateProfileAsync(HttpContext httpContext, WebDbContext _db, Models.Domain.Profile userProfile, IFormFile avatarFile)
    {
      var user = await JwtHelper.GetUserFromJWTAsync(httpContext, _db);
      if (user is null)
        return null;

      var existingProfile = await _db.Profiles.FirstOrDefaultAsync(p => p.userId == user.id);
      if (existingProfile is null)
        return null;

      existingProfile.name = userProfile.name;
      existingProfile.nickname = userProfile.nickname;
      existingProfile.gender = userProfile.gender;
      existingProfile.interest = userProfile.interest;

      //處理單張照片上傳
      var uploadAvatar = userProfile.avatarFile;
      if (uploadAvatar != null)
      {
        string bucketName = "teach-web-s3-bucket";
        string filesLocateDomain = "https://d3n4wxuzv8xzhg.cloudfront.net/";
        string fileToS3Path = "user/profile/avatar/";

        var avatarPath = await _fileUploadService.UploadFileToS3Async(uploadAvatar, bucketName, fileToS3Path);

        existingProfile.avatar = filesLocateDomain + avatarPath;
      }

      await _db.SaveChangesAsync();

      user.isProfileCompleted = true;
      await _db.SaveChangesAsync();

      var responseData = _mapper.Map<ProfileDto>(existingProfile);

      return responseData;
    }

    public async Task<List<BookingsResponseDto>> GetBookingsAsync(HttpContext httpContext, WebDbContext _db)
    {
      var user = await JwtHelper.GetUserFromJWTAsync(httpContext, _db);
      if (user is null)
        return null;

      var bookingsData = await _db.Bookings
                              .Where(data => data.userId == user.id)
                              .Where(data => data.status == "paid")
                              .Where(data => data.course.startTime >= DateTime.Now)
                              .Include(data => data.course)
                              .Include(data => data.course.teacher)
                              .ToListAsync();
      if (bookingsData.Count() <= 0)
        return null;

      var responseData = _mapper.Map<List<BookingsResponseDto>>(bookingsData);

      foreach (var booking in responseData)
      {
        booking.startTime = ConvertDateTimeFormatHelper.ConvertDateTimeFormat(booking.startTime);
        booking.endTime = ConvertDateTimeFormatHelper.ConvertDateTimeFormat(booking.endTime);
      }
      responseData = responseData.OrderBy(data => data.startTime).ToList();

      return responseData;
    }

    public async Task<UserResult> GetApplicationDefaultDataAsync(HttpContext httpContext, WebDbContext _db)
    {
      var user = await JwtHelper.GetUserFromJWTAsync(httpContext, _db);
      if (user is null)
        return new UserResult
        {
          statusCode = 404,
          message = "Can't find user."
        };

      var categoryData = await _db.CourseCategories
                                .OrderBy(data => data.category)
                                .ToListAsync();

      return new UserResult
      {
        statusCode = 200,
        data = new
        {
          userEmail = user.email,
          categoryData = categoryData
        }
      };
    }
  }
}