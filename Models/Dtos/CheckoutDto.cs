using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace personal_project.Models.Dtos
{
  public class CheckoutDto
  {
    public long id { get; set; }
    public string? productName { get; set; }
    public int productQty { get; set; }
    public double productPrice { get; set; }
    public long courseId { get; set; }
  }
}