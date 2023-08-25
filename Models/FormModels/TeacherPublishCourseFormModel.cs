using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace personal_project.Models.FormModels
{
  public class TeacherPublishCourseFormModel
  {
    public string? courseImage { get; set; }
    public string? courseName { get; set; }
    public string? courseWay { get; set; }
    public string? courseCategory { get; set; }
    public string? courseLocation { get; set; }
    public string? courseIntro { get; set; }

    public string? courseReminder { get; set; }
    public List<CourseFormModel>? courses { get; set; }

    [NotMapped]
    public IFormFile? courseImageFile { get; set; }

  }
}