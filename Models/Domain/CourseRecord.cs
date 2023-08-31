using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace personal_project.Models.Domain
{
  public class CourseRecord
  {
    public long id { get; set; }
    public string? roomId { get; set; }

    // FK
    public long ChatRecordId { get; set; }
    public ChatRecord? chatRecord { get; set; }

  }
}