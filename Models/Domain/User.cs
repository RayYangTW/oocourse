using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace personal_project.Models.Domain
{
  public class User
  {
    public long id { get; set; }
    public string? provider { get; set; }
    public string? email { get; set; }
    public string? password { get; set; }
    public string? role { get; set; } = "user";


    // one to one
    public TeacherApplication? teacherApplication { get; set; }
    public Teacher? teacher { get; set; }
    public Profile? profile { get; set; }
  }
}