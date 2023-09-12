using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace personal_project.Models.Domain
{
  public class CourseAccessList
  {
    public long id { get; set; }
    public string? roomId { get; set; }
    public long teacherUserId { get; set; }
    public long userId { get; set; }

    // FK
    public long courseId { get; set; }
    public Course? course { get; set; }
  }
}