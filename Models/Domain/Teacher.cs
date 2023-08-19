using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace personal_project.Models.Domain
{
  public class Teacher
  {
    public long id { get; set; }
    public string? avatar { get; set; }
    public string? courseName { get; set; }
    public string? courseWay { get; set; }
    public string? courseCategory { get; set; }
    public string? courseLocation { get; set; }
    public string? courseRemider { get; set; }

    // FK
    public long userId { get; set; }
    public User? user { get; set; }

    // one to one
    public Course? course { get; set; }
  }
}