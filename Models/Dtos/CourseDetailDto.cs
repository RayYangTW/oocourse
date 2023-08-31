using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace personal_project.Models.Dtos
{
  public class CourseDetailDto : AutoMapper.Profile
  {
    public class CourseDetailDTO
    {
      public long id { get; set; }
      public string? courseImage { get; set; }
      public string? courseName { get; set; }
      public string? courseWay { get; set; }
      public string? courseLanguage { get; set; }
      public string? courseCategory { get; set; }
      public string? courseLocation { get; set; }
      public string? courseIntro { get; set; }
      public string? courseReminder { get; set; }

      public long userId { get; set; }

      public List<CourseDTO>? courses { get; set; }
    }

    public class CourseDTO
    {
      public long id { get; set; }
      public string? startTime { get; set; }
      public string? endTime { get; set; }
      public double? price { get; set; }
      public bool isBooked { get; set; }
      public string? roomId { get; set; }
    }

  }
}