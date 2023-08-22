using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace personal_project.Models.FormModels
{
  public class TeacherApplicationFormModel
  {
    public string? name { get; set; }
    public string? email { get; set; }
    public string? category { get; set; }
    public string? language { get; set; }
    public string? country { get; set; }
    public string? location { get; set; }
    public string? experience { get; set; }
    public string? description { get; set; }
    [NotMapped]
    public List<IFormFile>? certificationFiles { get; set; }
  }
}