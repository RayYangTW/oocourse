using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace personal_project.Models.User
{
  public class SigninUser : BaseUser
  {
    public string? provider { get; set; }
    public string? access_token { get; set; }
  }
}