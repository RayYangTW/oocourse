using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using personal_project.Models.Domain;
using static personal_project.Models.Dtos.CourseDetailDto;

namespace personal_project.Mapping
{
  public class CourseDetailProfile : AutoMapper.Profile
  {
    public CourseDetailProfile()
    {
      CreateMap<Teacher, CourseDetailDTO>()
       .ForMember(dest => dest.courses, opt => opt.MapFrom(src => src.courses));

      CreateMap<Course, CourseDTO>();
    }
  }
}