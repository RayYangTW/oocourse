using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace personal_project.Models.ResultModels
{
  public class BookingResult
  {
    public int StatusCode { get; set; }
    public string? Message { get; set; }
    public object? Data { get; set; }
  }
}