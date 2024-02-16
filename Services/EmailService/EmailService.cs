using MailKit.Net.Smtp;
using MimeKit;
using MimeKit.Text;
using MailKit.Security;

namespace SocialNetwork.Services.EmailService
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly DataContext _context;
        public EmailService(IConfiguration configuration, DataContext context)
        {
            _configuration = configuration;
            _context = context;
        }
        public async Task<ServiceResponse<string>> SendEmail(string recipient)
        {
            var serviceResponse = new ServiceResponse<string>();
            var email = new MimeMessage();
            var securityCode = CreateSecurityCode();
            var metaData = await _context.MetaDatas.Where(x => x.Email == recipient).FirstAsync();
            metaData.SecurityCode = securityCode;
            email.From.Add(MailboxAddress.Parse(_configuration.GetSection("EmailConfiguration:AdminEmail").Value));
            email.To.Add(MailboxAddress.Parse(recipient));
            email.Subject = "Security code to complete registration.";
            email.Body = new TextPart(TextFormat.Plain) { Text = securityCode };

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
            await _context.SaveChangesAsync();
            serviceResponse.Data = null;
            serviceResponse.Success = true;
            serviceResponse.Message = "Security code sent to your email.";
            return serviceResponse;
        }

        public async Task<ServiceResponse<string>> VerifyEmail(string securityCode)
        {
            var serviceResponse = new ServiceResponse<string>();
            var metaData = await _context.MetaDatas.Where(x=>x.SecurityCode==securityCode).FirstAsync();
            try {
                if (metaData.SecurityCode!=securityCode) {
                    throw new Exception("Wrong security code.");
                }
                metaData.IsVerified = true;
                await _context.SaveChangesAsync();
                serviceResponse.Data = null;
                serviceResponse.Success = true;
                serviceResponse.Message = "Email successfully confirmed.";

            } catch (Exception ex) {
                serviceResponse.Data = null;
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
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
