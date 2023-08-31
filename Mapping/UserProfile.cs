using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using personal_project.Models.Domain;
using personal_project.Models.User;

namespace personal_project.Mapping
{
  public class UserProfile : AutoMapper.Profile
  {
    public UserProfile()
    {
      CreateMap<User, BaseUser>();
    }
  }
}