using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace personal_project.Models.ResultModels
{
  public class TeacherResult
  {
    public int statusCode { get; set; }
    public string? message { get; set; }
    public object? data { get; set; }
  }
}