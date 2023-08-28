using System.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace personal_project.Models.Payments
{
  public class Package
  {
    public string? id { get; set; }
    public int amount { get; set; }
    public List<Product>? products { get; set; } = new List<Product>();
  }

  public class Product
  {
    public string? name { get; set; }
    public int quantity { get; set; }
    public int price { get; set; }
  }

  public class RedirectUrls
  {
    public string? confirmUrl { get; set; }
    public string? cancelUrl { get; set; }
  }

  public class LinePay
  {
    public int amount { get; set; }
    public string? currency { get; set; }
    public string? orderId { get; set; }
    public List<Package>? packages { get; set; } = new List<Package>();
    public RedirectUrls? redirectUrls { get; set; } = new RedirectUrls();
  }

  public class ConfirmBody
  {
    public int amount { get; set; }
    public string? currency { get; set; }
  }
}