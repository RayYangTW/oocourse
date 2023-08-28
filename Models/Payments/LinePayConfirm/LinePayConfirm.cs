using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace personal_project.Models.Payments.LinePayConfirm
{
  public class Info
  {
    public long transactionId { get; set; }
    public string orderId { get; set; }
    public List<PayInfo> payInfo { get; set; }
    public List<Package> packages { get; set; }
  }

  public class Package
  {
    public string id { get; set; }
    public int amount { get; set; }
    public int userFeeAmount { get; set; }
    public List<Product> products { get; set; }
  }

  public class PayInfo
  {
    public string method { get; set; }
    public int amount { get; set; }
    public string maskedCreditCardNumber { get; set; }
  }

  public class Product
  {
    public string name { get; set; }
    public int quantity { get; set; }
    public int price { get; set; }
  }

  public class LinePayConfirm
  {
    public string returnCode { get; set; }
    public string returnMessage { get; set; }
    public Info info { get; set; }
  }
}