using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using personal_project.Data;
using personal_project.Helpers;
using personal_project.Models.Domain;
using personal_project.Models.Dtos;
using personal_project.Models.User;

namespace personal_project.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class UserController : ControllerBase
  {
    private readonly ILogger<UserController> _logger;
    private readonly WebDbContext _db;
    private readonly IMapper _mapper;
    private readonly IAmazonS3 _s3Client;

    public UserController(ILogger<UserController> logger, WebDbContext db, IMapper mapper, IAmazonS3 s3Client)
    {
      _logger = logger;
      _db = db;
      _mapper = mapper;
      _s3Client = s3Client;
    }

    // Global variables
    string bucketName = "teach-web-s3-bucket";
    string filesLocateDomain = "https://d3n4wxuzv8xzhg.cloudfront.net/";
    string fileToS3Path = "user/profile/avatar/";

    // GET: api/user/checkAuthorize
    [Authorize]
    [HttpGet("checkAuthorize")]
    public async Task<IActionResult> CheckUserAuthorize()
    {
      return Ok("pass authorize.");
    }

    [AllowAnonymous]
    [HttpPost("signup")]
    public async Task<IActionResult> Signup(BaseUser signupUser)
    {
      if (signupUser is null)
        return BadRequest("No request");
      if (signupUser.email is null || signupUser.password is null)
        return BadRequest("Email and Password fields all are required!");

      // Check if email exists.
      var emailExists = await _db.Users.AnyAsync(u => u.email == signupUser.email);
      if (emailExists)
        return StatusCode(403, "Email existed!");

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
      var jwt = CreateJWT(newBaseUser);

      //// Return response data
      return Ok(new
      {
        access_token = jwt,
        access_expired = GetTokenExpirationTime(jwt),
        user = new
        {
          id = newUser.id,
          email = newUser.email,
          provider = newUser.provider
        }
      });

    }

    [AllowAnonymous]
    [HttpPost("signin")]
    public async Task<IActionResult> Signin(SigninUser signinUser)
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
          return StatusCode(403, "Email didn't exist!");

        // Check password verify hashpassword
        var passwordCheck = BCrypt.Net.BCrypt.Verify(signinUser.password, user.password);
        if (!passwordCheck)
          return StatusCode(403, "Incorrect Email or Password!");

        // Generate jwt
        var newBaseUser = _mapper.Map<BaseUser>(user);
        var jwt = CreateJWT(newBaseUser);

        // Get user role from JWT
        var readJwt = new JwtSecurityTokenHandler().ReadJwtToken(jwt);
        var userRole = readJwt.Claims.FirstOrDefault(a => a.Type == "role")?.Value;

        //// Return response data
        return Ok(new
        {
          access_token = jwt,
          access_expired = GetTokenExpirationTime(jwt),
          user = new
          {
            id = user.id,
            email = user.email,
            provider = user.provider,
            role = userRole,
            isProfileCompleted = user.isProfileCompleted,
            userName = user.profile.name
          }
        });
      }
      return BadRequest("Unknown signin source!");
    }

    [Authorize]
    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
      var user = await GetUserFromJWTAsync();
      if (user is null)
        return BadRequest("Can't find user.");

      // Get user from Profiles
      var profile = await _db.Profiles.FirstOrDefaultAsync(p => p.userId == user.id);
      if (profile is null)
        // return NotFound("Profile not found for the user.");
        return Ok(new { test = 123 });

      return Ok(new
      {
        name = profile.name,
        nickname = profile.nickname,
        avatar = profile.avatar,
        gender = profile.gender,
        interest = profile.interest
      });
    }

    [Authorize]
    [HttpPost("profile")]
    public async Task<IActionResult> UploadProfile([FromForm] Models.Domain.Profile userProfile)
    {
      var user = await GetUserFromJWTAsync();
      if (user is null)
        return BadRequest("Can't find user.");

      var existingProfile = await _db.Profiles.FirstOrDefaultAsync(p => p.userId == user.id);

      if (existingProfile is not null)
      {
        existingProfile.name = userProfile.name;
        existingProfile.nickname = userProfile.nickname;
        existingProfile.avatar = userProfile.avatar;
        existingProfile.gender = userProfile.gender;
        existingProfile.interest = userProfile.interest;

        await _db.SaveChangesAsync();
        return Ok();
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
      return Ok();
    }

    [Authorize]
    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfile([FromForm] Models.Domain.Profile userProfile)
    {
      var user = await GetUserFromJWTAsync();
      if (user is null)
        return BadRequest("Can't find user.");

      var existingProfile = await _db.Profiles.FirstOrDefaultAsync(p => p.userId == user.id);
      if (existingProfile is null)
        return NotFound("Profile not found for the user.");

      existingProfile.name = userProfile.name;
      existingProfile.nickname = userProfile.nickname;
      existingProfile.gender = userProfile.gender;
      existingProfile.interest = userProfile.interest;

      //處理單張照片上傳
      var uploadAvatar = userProfile.avatarFile;
      if (uploadAvatar != null)
      {
        var ext = System.IO.Path.GetExtension(uploadAvatar.FileName);
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
          var fileToS3 = fileToS3Path + GenerateFilenameHelper.GenerateFileRandomName() + ext;
          var objectRequest = new PutObjectRequest()
          {
            BucketName = bucketName,
            Key = fileToS3,
            InputStream = uploadAvatar.OpenReadStream()
          };
          await _s3Client.PutObjectAsync(objectRequest);
          existingProfile.avatar = filesLocateDomain + fileToS3;
        }
      }

      await _db.SaveChangesAsync();

      user.isProfileCompleted = true;
      await _db.SaveChangesAsync();

      var responseData = _mapper.Map<ProfileDto>(existingProfile);

      return Ok(responseData);
    }

    [Authorize]
    [HttpGet("bookings")]
    public async Task<IActionResult> GetBookings()
    {
      var user = await GetUserFromJWTAsync();
      if (user is null)
        return BadRequest("Can't find user.");

      var bookingsData = await _db.Bookings
                              .Where(data => data.userId == user.id)
                              .Where(data => data.course.startTime >= DateTime.Now)
                              .Include(data => data.course)
                              .Include(data => data.course.teacher)
                              .ToListAsync();

      var responseData = _mapper.Map<List<BookingsResponseDto>>(bookingsData);

      foreach (var booking in responseData)
      {
        booking.startTime = ConvertDateTimeFormatHelper.ConvertDateTimeFormat(booking.startTime);
        booking.endTime = ConvertDateTimeFormatHelper.ConvertDateTimeFormat(booking.endTime);
      }
      responseData = responseData.OrderBy(data => data.startTime).ToList();

      return Ok(responseData);
    }

    [HttpGet("applicationDefaultData")]
    public async Task<IActionResult> GetApplicationDefaultData()
    {
      var user = await GetUserFromJWTAsync();
      if (user is null)
        return BadRequest("Can't find user.");

      var categoryData = await _db.CourseCategories
                                  .OrderBy(data => data.category)
                                  .ToListAsync();

      return Ok(new
      {
        userEmail = user.email,
        categoryData = categoryData
      });
    }

    /********************************************************
                            Methods
    *********************************************************/
    //// Method: to create JWT
    private string CreateJWT(BaseUser user)
    {
      // put the info that you want to store in the JWT
      List<Claim> claims = new List<Claim>
      {
        new Claim("email", user.email),
        new Claim("role", user.role)
      };
      // store the key from the .env
      DotNetEnv.Env.Load();
      var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(System.Environment.GetEnvironmentVariable("JWT_KEY")));
      var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
      var token = new JwtSecurityToken(
        issuer: System.Environment.GetEnvironmentVariable("JWT_ISSUER"),
        audience: System.Environment.GetEnvironmentVariable("JWT_AUDIENCE"),
        claims: claims,
        // jwt expire time
        expires: DateTime.Now.AddHours(8),
        signingCredentials: creds
      );
      var jwt = new JwtSecurityTokenHandler().WriteToken(token);
      return jwt;
    }
    //// Method: to get the token expire time
    public static long GetTokenExpirationTime(string token)
    {
      var handler = new JwtSecurityTokenHandler();
      var jwtSecurityToken = handler.ReadJwtToken(token);
      var tokenExp = jwtSecurityToken.Claims.First(claim => claim.Type.Equals("exp")).Value;
      var ticks = long.Parse(tokenExp);
      return ticks;
    }
    //// Method: to get the user from JWT
    private async Task<User> GetUserFromJWTAsync()
    {
      string token = Request.Headers["Authorization"];
      if (string.IsNullOrEmpty(token) || !token.StartsWith("Bearer "))
      {
        return null;
      }

      token = token.Substring("Bearer ".Length);

      var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

      var userEmail = jwt.Claims.FirstOrDefault(a => a.Type == "email")?.Value;

      if (userEmail is not null)
      {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.email == userEmail);
        return user;
      }

      return null;
    }
  }
}