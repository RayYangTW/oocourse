using System.Security.Cryptography;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using personal_project.Models.Dtos;
using personal_project.Services;
using Microsoft.AspNetCore.Authorization;

namespace personal_project.Controllers
{
  [ApiController]
  [Authorize]
  [Route("api/[controller]")]
  public class CheckoutController : ControllerBase
  {
    private readonly ILinePayService _linePayService;
    private readonly ICheckoutService _checkoutService;

    public CheckoutController(ILinePayService linePayService, ICheckoutService checkoutService)
    {
      _linePayService = linePayService;
      _checkoutService = checkoutService;
    }

    [HttpPost("linepay")]
    public async Task<IActionResult> PayByLinePay([FromForm] CheckoutDto checkoutDto)
    {
      var authorizationHeader = Request.Headers["Authorization"].ToString();
      var result = await _checkoutService.PayByLinePayAsync(checkoutDto, authorizationHeader);
      if (result.statusCode == 200)
        return Ok(result.data);
      return StatusCode(result.statusCode, result.message);
    }

    [HttpGet("linepay/confirm")]
    public async Task<IActionResult> GetConfirmLinePay(string transactionId, string orderId)
    {
      var authorizationHeader = Request.Headers["Authorization"].ToString();
      var checkBooking = await _checkoutService.ProcessCheckoutAsync(orderId, authorizationHeader);
      var newConfirmBody = await _linePayService.PrepareConfirmBodyAsync(orderId);
      var response = await _linePayService.SendLinePayConfirmRequestAsync(newConfirmBody, transactionId);
      var result = await _checkoutService.UpdateDataAfterLinePayConfirmAsync(orderId, response);
      if (result is not null)
        return Ok(result);
      return BadRequest("Error of getting confirmed by LinePay.");
    }
  }
}