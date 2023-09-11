using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace personal_project.Services
{
  public interface IEmailService
  {
    public Task SendDenyApplicationMailAsync(long id);
    public Task SendApproveApplicationMailAsync(long id);
  }
}