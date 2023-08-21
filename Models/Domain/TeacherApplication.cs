using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
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
    public string? country { get; set; }
    public string? location { get; set; }
    public string? experience { get; set; }
    public string? description { get; set; }
    public DateTime createdTime { get; set; } = DateTime.Now;
    public DateTime updatedTime { get; set; } = DateTime.Now;
    public Boolean isApproved { get; set; } = false;
    public string? status { get; set; } = "unapproved";

    public ICollection<Certification> certifications { get; set; } = new List<Certification>();
    [NotMapped]
    public List<IFormFile>? certificationFiles { get; set; }

    // FK
    public long userId { get; set; }
    //public User? user { get; set; }
  }
}