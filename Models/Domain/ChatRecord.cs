using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace personal_project.Models.Domain
{
  public class ChatRecord
  {
    public long id { get; set; }
    public string? message { get; set; }
    public DateTime createdTime { get; set; }
    public long userId { get; set; }
    public long teacherId { get; set; }
  }
}