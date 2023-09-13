using System.Diagnostics.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using personal_project.Data;
using personal_project.Helpers;
using personal_project.Models.Domain;
using personal_project.Models.Dtos;
using personal_project.Services;
using Microsoft.AspNetCore.Authorization;

namespace personal_project.Controllers
{
  [ApiController]
  [Authorize]
  [Route("api/[controller]")]
  public class BookingController : ControllerBase
  {
    private readonly ILogger<BookingController> _logger;
    private readonly WebDbContext _db;
    private readonly IMapper _mapper;
    private readonly GetUserDataFromJWTHelper _jwtHelper;
    private readonly IConfiguration _config;
    private readonly IBookingService _bookingService;

    public BookingController(ILogger<BookingController> logger, WebDbContext db, IMapper mapper, GetUserDataFromJWTHelper jwtHelper, IConfiguration config, IBookingService bookingService)
    {
      _logger = logger;
      _db = db;
      _mapper = mapper;
      _jwtHelper = jwtHelper;
      _config = config;
      _bookingService = bookingService;
    }

    [HttpGet("{courseId}")]
    public async Task<IActionResult> GetBookingData(long courseId)
    {
      var authorizationHeader = Request.Headers["Authorization"].ToString();
      var bookingData = await _bookingService.GetBookingDataAsync(courseId, authorizationHeader);

      if (bookingData is not null)
        return Ok(bookingData);

      return NotFound("Course not found or user is not authorized.");
    }

    [HttpPost("{courseId}")]
    public async Task<IActionResult> PostBookingData(long courseId)
    {
      var authorizationHeader = Request.Headers["Authorization"].ToString();
      var bookingResult = await _bookingService.PostBookingDataAsync(courseId, authorizationHeader);

      if (bookingResult.StatusCode == 200)
      {
        return Ok(bookingResult.Data);
      }
      else
      {
        return StatusCode(bookingResult.StatusCode, bookingResult.Message);
      }

    }
  }
}