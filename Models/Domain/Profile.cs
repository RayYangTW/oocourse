using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace personal_project.Models.Domain
{
  public class Profile
  {
    public long id { get; set; }
    public string? avatar { get; set; }
    [NotMapped]
    public IFormFile? avatarFile { get; set; }
    public string? name { get; set; }
    public string? nickname { get; set; }
    public string? gender { get; set; }
    public string? interest { get; set; }

    // FK
    public long userId { get; set; }
    // public User? user { get; set; }

  }
}