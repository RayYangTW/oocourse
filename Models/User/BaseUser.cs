using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace personal_project.Models.User
{
  public class BaseUser
  {
    public string? email { get; set; }
    public string? password { get; set; }
    public string? role { get; set; }

  }
}