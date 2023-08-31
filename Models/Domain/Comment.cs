using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace personal_project.Models.Domain
{
  public class Comment
  {
    public long id { get; set; }
    public string? comment { get; set; }
    public double rate { get; set; }
    public DateTime createdTime { get; set; } = DateTime.Now;
    public DateTime updatedTime { get; set; } = DateTime.Now;

    //FK
    public long userId { get; set; }
    public User? user { get; set; }
    public long teacherId { get; set; }
    public Teacher? teacher { get; set; }
  }
}