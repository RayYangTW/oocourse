using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace personal_project.Models.ResponseModels
{
  public class CourseDetailResponseModel
  {
    public long id { get; set; }
    public string? courseImage { get; set; }
    public string? courseName { get; set; }
    public string? courseWay { get; set; }
    public string? courseLanguage { get; set; }
    public string? courseCategory { get; set; }
    public string? courseLocation { get; set; }
    public string? courseReminder { get; set; }
    public List<CourseTimeData>? courseTimeDatas { get; set; }
  }

  public class CourseTimeData
  {
    public DateTime? startTime { get; set; }
    public DateTime? endTime { get; set; }
    public double? price { get; set; }
    public bool isBooked { get; set; }
    public string? roomId { get; set; }
  }
}