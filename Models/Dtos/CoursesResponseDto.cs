using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace personal_project.Models.Dtos
{
  public class CoursesResponseDto
  {
    public string? courseName { get; set; }
    public string? startTime { get; set; }
    public string? endTime { get; set; }
    public string? roomId { get; set; }
  }
}