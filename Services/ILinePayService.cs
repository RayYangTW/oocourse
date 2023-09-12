using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using personal_project.Models.Domain;
using personal_project.Models.Dtos;
using personal_project.Models.Payments;
using personal_project.Models.Payments.LinePayConfirm;
using personal_project.Models.ResultModels;

namespace personal_project.Services
{
  public interface ILinePayService
  {
    LinePay PrepareLinePayRequest(CheckoutDto checkoutDto, string confirmUrl, string cancelUrl);
    Task<LinePayResult> SendLinePayRequestAsync(LinePay newLinepay, string secretKey, string version, string site, string channelId, User user, Course course);
    Task<ConfirmBody> PrepareConfirmBodyAsync(string orderId);
    Task<LinePayConfirm> SendLinePayConfirmRequestAsync(ConfirmBody newConfirmBody, string transactionId);
  }
}