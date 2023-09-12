using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using personal_project.Data;

namespace personal_project.Services
{
  public class EmailService : IEmailService
  {
    private readonly WebDbContext _db;
    private readonly IMapper _mapper;


    public EmailService(WebDbContext db, IMapper mapper)
    {
      _db = db;
      _mapper = mapper;
    }

    string host = "smtp.gmail.com"; // 送信郵件主機
    int port = 587; // 送信郵件主機連接埠
    string mailServerName = "Server"; // 寄信者名稱
    string mailServerAddress = "oocourse_mail_server@gmail.com"; // 寄送者信箱

    public async Task SendApproveApplicationMailAsync(long id)
    {
      DotNetEnv.Env.Load();
      var account = System.Environment.GetEnvironmentVariable("EMAIL_ACCOUNT");
      var password = System.Environment.GetEnvironmentVariable("EMAIL_PASSWORD");

      var teacherApplication = await _db.TeacherApplications
                                        .Where(data => data.id == id)
                                        .FirstOrDefaultAsync();
      if (teacherApplication is null)
        return;

      string targetAddress = teacherApplication.email;
      string subject = "OOcourse-教師資格審核已通過"; // 信件主旨
      string body = "<p>您好：</p><p>很開心在此通知，</p><p>您在<strong>OOcourse</strong>的教師資格已通過。</p><p>您已經可以開設課程。<br></p><p><br></p><p>此為系統自動通知信，請勿直接回信。</p>"; // 信件內容

      MimeMessage message = new();
      message.From.Add(new MailboxAddress(mailServerName, mailServerAddress));
      message.To.Add(MailboxAddress.Parse(targetAddress));
      message.Subject = subject;
      message.Body = new TextPart("html")
      {
        Text = body
      };

      using SmtpClient client = new();
      client.Connect(host, port, false);
      client.Authenticate(account, password);
      client.Send(message);
      client.Disconnect(true);
    }

    public async Task SendDenyApplicationMailAsync(long id)
    {
      DotNetEnv.Env.Load();
      var account = System.Environment.GetEnvironmentVariable("EMAIL_ACCOUNT");
      var password = System.Environment.GetEnvironmentVariable("EMAIL_PASSWORD");

      var teacherApplication = await _db.TeacherApplications
                                        .Where(data => data.id == id)
                                        .FirstOrDefaultAsync();
      if (teacherApplication is null)
        return;

      string targetAddress = teacherApplication.email;
      string subject = "OOcourse-教師資格審核未通過"; // 信件主旨
      string body = "<p>您好：</p><p>很遺憾在此通知，</p><p>您在<strong>OOcourse</strong>的教師資格未通過。</p><p>若有更新資訊，歡迎再次申請。<br></p><p><br></p><p>此為系統自動通知信，請勿直接回信。</p>"; // 信件內容

      MimeMessage message = new();
      message.From.Add(new MailboxAddress(mailServerName, mailServerAddress));
      message.To.Add(MailboxAddress.Parse(targetAddress));
      message.Subject = subject;
      message.Body = new TextPart("html")
      {
        Text = body
      };

      using SmtpClient client = new();
      client.Connect(host, port, false);
      client.Authenticate(account, password);
      client.Send(message);
      client.Disconnect(true);
    }
  }
}