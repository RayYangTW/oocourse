using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using personal_project.Models.Domain;
using personal_project.Models.Dtos;

namespace personal_project.Mapping
{
  public class BookingDetailProfile : AutoMapper.Profile
  {
    public BookingDetailProfile()
    {
      CreateMap<Course, BookingDetailDto>()
        .ForMember(dest => dest.startTime, opt => opt.MapFrom(src => src.startTime))
        .ForMember(dest => dest.endTime, opt => opt.MapFrom(src => src.endTime))
        .ForMember(dest => dest.price, opt => opt.MapFrom(src => src.price))
        .ForMember(dest => dest.roomId, opt => opt.MapFrom(src => src.roomId))
        .ForMember(dest => dest.courseImage, opt => opt.MapFrom(src => src.teacher.courseImage))
        .ForMember(dest => dest.courseName, opt => opt.MapFrom(src => src.teacher.courseName))
        .ForMember(dest => dest.courseWay, opt => opt.MapFrom(src => src.teacher.courseWay))
        .ForMember(dest => dest.courseLocation, opt => opt.MapFrom(src => src.teacher.courseLocation))
        .ForMember(dest => dest.courseReminder, opt => opt.MapFrom(src => src.teacher.courseReminder));

      CreateMap<Teacher, BookingDetailDto>()
        .ForMember(dest => dest.courseImage, opt => opt.MapFrom(src => src.courseImage))
        .ForMember(dest => dest.courseName, opt => opt.MapFrom(src => src.courseName))
        .ForMember(dest => dest.courseWay, opt => opt.MapFrom(src => src.courseWay))
        .ForMember(dest => dest.courseLocation, opt => opt.MapFrom(src => src.courseLocation))
        .ForMember(dest => dest.courseReminder, opt => opt.MapFrom(src => src.courseReminder));
    }
  }
}