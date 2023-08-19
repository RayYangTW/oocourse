using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace personal_project.Models.Domain
{
  public class TeacherAvailableTime
  {
    public long id { get; set; }
    public DateTime startTime { get; set; }
    public DateTime endTime { get; set; }
    public bool isBooked { get; set; } = false;

    public long teacherId { get; set; }
    public Teacher? teacher { get; set; }
  }
}