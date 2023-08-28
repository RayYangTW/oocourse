using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace personal_project.Models.Payments
{
  public class Info
  {
    public PaymentUrl paymentUrl { get; set; }
    public long transactionId { get; set; }
    public string paymentAccessToken { get; set; }
  }

  public class PaymentUrl
  {
    public string web { get; set; }
    public string app { get; set; }
  }

  public class linePayResponse
  {
    public string returnCode { get; set; }
    public string returnMessage { get; set; }
    public Info info { get; set; }
  }
}