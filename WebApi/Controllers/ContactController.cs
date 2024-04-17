using System;
using System.IO;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using MimeKit.Text;
using Microsoft.Extensions.Configuration;
using WebApi.Models;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private static int _successfulEmailCount = 0;
        private readonly IConfiguration _configuration;

        public EmailController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        [Route("send")]
        public async Task<IActionResult> SendEmail([FromBody] ContactMessage model)
        {
            try
            {
                
                if (string.IsNullOrEmpty(model.Email))
                {
                    return BadRequest("Sender's email address is missing.");
                }
                var emailMessage = new MimeMessage();
                string senderName = model.Name;
                string senderEmailAddress = model.Email;
                emailMessage.From.Add(new MailboxAddress(senderName, senderEmailAddress));
                emailMessage.To.Add(new MailboxAddress(_configuration["EMAIL_RECIPIENT_NAME"], _configuration["EMAIL_RECIPIENT_ADDRESS"]));
                emailMessage.Subject = "Contact Form Submission from " + model.Name;

                var bodyBuilder = new BodyBuilder();
                bodyBuilder.TextBody = model.Message;

                emailMessage.Body = bodyBuilder.ToMessageBody();

                using (var client = new SmtpClient())
                {
                    client.ServerCertificateValidationCallback = (s, c, h, e) => true; // Ignore SSL certificate errors
                    await client.ConnectAsync(_configuration["SMTP_SERVER"], int.Parse(_configuration["SMTP_PORT"]), SecureSocketOptions.SslOnConnect);

                    await client.AuthenticateAsync(_configuration["SMTP_USERNAME"], _configuration["SMTP_PASSWORD"]);

                    await client.SendAsync(emailMessage);
                    await client.DisconnectAsync(true);
                }
                _successfulEmailCount++;

                return Ok(new { status = "success" });
            }
            catch (Exception ex)
            {
                // For debugging purposes
                Console.WriteLine("Exception: " + ex);
                return StatusCode(500, "An error occurred while sending the email.");
            }
        }

        [HttpGet]
        [Route("count")]
        public IActionResult GetEmailCount()
        {
            try
            {
                // Return the email count as a JSON response
                return Ok(new { emailCount = _successfulEmailCount });
            }
            catch (Exception ex)
            {
                // For debugging purposes
                Console.WriteLine("Exception: " + ex);
                return StatusCode(500, "An error occurred while counting the emails.");
            }
        }
    }
}
