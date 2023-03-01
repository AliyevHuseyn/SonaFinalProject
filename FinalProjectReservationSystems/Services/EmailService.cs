using FinalProjectReservationSystems.Abstractions.Services;
using System.Net;
using System.Net.Mail;

namespace FinalProjectReservationSystems.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void Send(string sendTo, string subject, string body,bool isBodyHtml = true )
        {
            using SmtpClient smtp = new SmtpClient(_configuration["Email:Host"], Convert.ToInt32(_configuration["Email:Port"]));
            smtp.EnableSsl = true;
            smtp.Credentials = new NetworkCredential(_configuration["Email:Login"], _configuration["Email:Password"]);


            MailAddress from = new MailAddress(_configuration["Email:Login"], "Sona Hotel");
            MailAddress to = new MailAddress(sendTo);

            using MailMessage message = new MailMessage(from, to);
            message.Subject = subject;
            message.Body = body;
            message.IsBodyHtml = isBodyHtml;
            smtp.Send(message);
        }

    }
}
