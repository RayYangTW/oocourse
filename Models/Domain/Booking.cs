using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace personal_project.Models.Domain
{
  public class Booking
  {
    public long id { get; set; }
    public string? orderId { get; set; }
    public string? status { get; set; }
    public DateTime bookingTime { get; set; }

    // FK
    public long? userId { get; set; }
    public User? user { get; set; }
    public long courseId { get; set; }
    public Course? course { get; set; }

  }
}