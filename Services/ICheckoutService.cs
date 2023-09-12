using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using personal_project.Models.Dtos;
using personal_project.Models.Payments.LinePayConfirm;
using personal_project.Models.ResultModels;

namespace personal_project.Services
{
  public interface ICheckoutService
  {
    Task<LinePayResult> PayByLinePayAsync(CheckoutDto checkoutDto, string authorizationHeader);

    Task<CheckoutResult> ProcessCheckoutAsync(string orderId, string authorizationHeader);
    Task<BookingDetailDto> UpdateDataAfterLinePayConfirmAsync(string orderId, LinePayConfirm response);
  }
}