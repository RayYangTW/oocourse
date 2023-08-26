using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using personal_project.Models.Domain;
using personal_project.Models.Dtos;

namespace personal_project.Mapping
{
  public class CourseProfile : AutoMapper.Profile
  {
    public CourseProfile()
    {
      CreateMap<Course, CoursesResponseDto>()
        .ForMember(dest => dest.courseName, opt => opt.MapFrom(src => src.teacher.courseName));
    }
  }
}