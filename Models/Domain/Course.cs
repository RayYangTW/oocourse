using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace personal_project.Models.Domain
{
  public class Course
  {
    public long id { get; set; }
    public DateTime? startTime { get; set; }
    public DateTime? endTime { get; set; }
    public double? price { get; set; }
    public bool isBooked { get; set; } = false;
    public string? roomId { get; set; } = null;
    public string? courseLink { get; set; } = null;

    // FK
    public long teacherId { get; set; }
    public Teacher? teacher { get; set; }

    // one to one
    public CourseAccessList? courseAccessList { get; set; }

    // one to many
    public ICollection<Booking> bookings { get; set; } = new List<Booking>();
  }
}