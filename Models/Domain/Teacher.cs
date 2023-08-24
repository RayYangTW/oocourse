using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace personal_project.Models.Domain
{
  public class Teacher
  {
    public long id { get; set; }
    public string? courseImage { get; set; }
    public string? courseName { get; set; }
    public string? courseWay { get; set; }
    public string? courseLanguage { get; set; }
    public string? courseCategory { get; set; }
    public string? courseLocation { get; set; }
    public string? courseReminder { get; set; }

    public long userId { get; set; }


    // one to many
    public ICollection<ChatRecord> chatRecords { get; set; } = new List<ChatRecord>();
    public ICollection<Comment> comments { get; set; } = new List<Comment>();
    public ICollection<Course> courses { get; set; } = new List<Course>();
  }
}