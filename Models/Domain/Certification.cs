using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace personal_project.Models.Domain
{
  public class Certification
  {
    public long id { get; set; }
    public string? certification { get; set; }
    [NotMapped]
    public IFormFile? certificationFile { get; set; }
    public long UserId { get; set; } // FK
    // public User? user { get; set; } // navigation
  }
}