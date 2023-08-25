using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using personal_project.Models.Domain;
using personal_project.Models.Dtos;

namespace personal_project.Mapping
{
  public class BookingsProfile : AutoMapper.Profile
  {
    public BookingsProfile()
    {
      CreateMap<Booking, BookingsResponseDto>()
      .ForMember(dest => dest.courseName, opt => opt.MapFrom(src => src.course.teacher.courseName))
          .ForMember(dest => dest.startTime, opt => opt.MapFrom(src => src.course.startTime))
          .ForMember(dest => dest.endTime, opt => opt.MapFrom(src => src.course.endTime))
          .ForMember(dest => dest.roomId, opt => opt.MapFrom(src => src.course.roomId));
    }
  }
}