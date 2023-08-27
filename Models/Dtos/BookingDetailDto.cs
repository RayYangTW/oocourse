using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace personal_project.Models.Dtos
{
  public class BookingDetailDto
  {
    public string? courseImage { get; set; }
    public string? courseName { get; set; }
    public string? courseWay { get; set; }
    public string? courseLocation { get; set; }
    public string? courseReminder { get; set; }
    public string? courseIntro { get; set; }
    public string? startTime { get; set; }
    public string? endTime { get; set; }
    public double price { get; set; }
    public string? roomId { get; set; }
    public string? courseLink { get; set; }

  }
}