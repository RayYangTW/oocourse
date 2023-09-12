using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using personal_project.Models.Dtos;
using personal_project.Models.ResultModels;

namespace personal_project.Services
{
  public interface IBookingService
  {
    Task<BookingDetailDto> GetBookingDataAsync(long courseId, string authorizationHeader);
    Task<BookingResult> PostBookingDataAsync(long courseId, string authorizationHeader);
  }
}