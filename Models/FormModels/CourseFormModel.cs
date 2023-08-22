using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace personal_project.Models.FormModels
{
  public class CourseFormModel
  {
    public DateTime? startTime { get; set; }
    public DateTime? endTime { get; set; }
    public double? price { get; set; }
  }
}