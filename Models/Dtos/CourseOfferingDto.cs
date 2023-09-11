using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace personal_project.Models.Dtos
{
  public class CourseOfferingDto
  {
    public long id { get; set; }
    public string? startTime { get; set; }
    public string? endTime { get; set; }
    public double? price { get; set; }
    public string? courseName { get; set; }

  }
}