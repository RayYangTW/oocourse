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
using personal_project.Services;

namespace personal_project.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class UserController : ControllerBase
  {
    private readonly ILogger<UserController> _logger;
    private readonly WebDbContext _db;
    private readonly IUserService _userService;

    public UserController(ILogger<UserController> logger, WebDbContext db, IUserService userService)
    {
      _logger = logger;
      _db = db;
      _userService = userService;
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
      var result = await _userService.SignupAsync(signupUser);
      if (result.statusCode == 200)
        return Ok(result.data);
      return StatusCode(result.statusCode, result.message);
    }

    [AllowAnonymous]
    [HttpPost("signin")]
    public async Task<IActionResult> Signin(SigninUser signinUser)
    {
      var result = await _userService.SigninAsync(signinUser);
      if (result.statusCode == 200)
        return Ok(result.data);
      return StatusCode(result.statusCode, result.message);
    }

    [Authorize]
    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
      var httpContext = Request.HttpContext;
      var result = await _userService.GetProfileAsync(httpContext, _db);
      if (result.statusCode == 200)
        return Ok(result.data);
      return StatusCode(result.statusCode, result.message);
    }

    [Authorize]
    [HttpPost("profile")]
    public async Task<IActionResult> UploadProfile([FromForm] Models.Domain.Profile userProfile)
    {
      var httpContext = Request.HttpContext;
      var result = await _userService.UploadProfileAsync(httpContext, _db, userProfile);
      if (result.statusCode == 200)
        return Ok(result.message);
      return StatusCode(result.statusCode, result.message);
    }

    [Authorize]
    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfile([FromForm] Models.Domain.Profile userProfile)
    {
      var httpContext = Request.HttpContext;
      var avatarFile = userProfile.avatarFile;
      var result = await _userService.UpdateProfileAsync(httpContext, _db, userProfile, avatarFile);

      if (result is not null)
        return Ok(result);

      return NotFound("Profile not found for the user.");
    }

    [Authorize]
    [HttpGet("bookings")]
    public async Task<IActionResult> GetBookings()
    {
      var httpContext = Request.HttpContext;
      var result = await _userService.GetBookingsAsync(httpContext, _db);
      if (result is null)
        return NoContent();
      return Ok(result);
    }

    [HttpGet("applicationDefaultData")]
    public async Task<IActionResult> GetApplicationDefaultData()
    {
      var httpContext = Request.HttpContext;
      var result = await _userService.GetApplicationDefaultDataAsync(httpContext, _db);
      if (result.statusCode == 200)
        return Ok(result.data);
      return StatusCode(result.statusCode, result.message);
    }
  }
}