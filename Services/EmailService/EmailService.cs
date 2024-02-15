using MailKit.Net.Smtp;
using MimeKit;
using MimeKit.Text;
using MailKit.Security;
using SocialNetwork.Services.EmailService;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace SocialNetwork.Services.EmailService
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private string sentSecurityCode { get; set; }

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public ServiceResponse<string> SendEmail(string recipient)
        {
            var serviceResponse = new ServiceResponse<string>();
            var email = new MimeMessage();
            sentSecurityCode =CreateSecurityCode();
            email.From.Add(MailboxAddress.Parse(_configuration.GetSection("EmailConfiguration:AdminEmail").Value));
            email.To.Add(MailboxAddress.Parse(recipient));
            email.Subject = "Security code to complete registration.";
            email.Body = new TextPart(TextFormat.Plain) { Text = sentSecurityCode };

            var smtp = new SmtpClient();

            smtp.Connect(
                _configuration.GetSection("EmailConfiguration:Host").Value,
                Convert.ToInt32(_configuration.GetSection("EmailConfiguration:Port").Value),
                SecureSocketOptions.StartTls
            );

            smtp.Authenticate(
                _configuration.GetSection("EmailConfiguration:AdminEmail").Value,
               _configuration.GetSection("EmailConfiguration:AdminPassword").Value
            );

            smtp.Send(email);
            smtp.Disconnect(true);
            serviceResponse.Data = null;
            serviceResponse.Success = true;
            serviceResponse.Message = "Security code sent to your email.";
            return serviceResponse;
        }

        public ServiceResponse<string> VerifyEmail([FromBody]string securityCode)
        {
            var serviceResponse = new ServiceResponse<string>();
            try {
                if (securityCode != sentSecurityCode) {
                    throw new Exception("Invalid security code.");
                }
                serviceResponse.Data = null;
                serviceResponse.Success = true;
                serviceResponse.Message = "Email successfully confirmed.";

            }catch(Exception ex) {
                serviceResponse.Data = null;
                serviceResponse.Success = false;
                serviceResponse.Message = "Wrong security code.";
            }
            return serviceResponse;
        }
        private string CreateSecurityCode()
        {
            Random rnd = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

            char[] result = new char[5];

            for (int i = 0; i < 5; i++) {
                result[i] = chars[rnd.Next(chars.Length)];
            }

            return new string(result);
        }
    }
}
