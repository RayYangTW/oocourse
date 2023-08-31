using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using personal_project.Models.Domain;

namespace personal_project.Mapping
{
  public class ProfileProfile : AutoMapper.Profile
  {
    public ProfileProfile()
    {
      CreateMap<Profile, ProfileDto>();
    }
  }
}