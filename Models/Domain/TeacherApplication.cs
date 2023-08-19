using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace personal_project.Models.Domain
{
  public class TeacherApplication
  {
    public long id { get; set; }
    public string? name { get; set; }
    public string? email { get; set; }
    public string? category { get; set; }
    public string? language { get; set; }
    public string? location { get; set; }
    public string? experience { get; set; }
    public string? certification { get; set; }
    public string? description { get; set; }
    public bool isApproved { get; set; } = false;

    public long userId { get; set; }
    public User? user { get; set; }
  }
}