namespace SocialNetwork.Services.EmailService
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly DataContext _context;
        private readonly ICacheService _cacheService;

        public EmailService(IConfiguration configuration, DataContext context, ICacheService cacheService)
        {
            _configuration = configuration;
            _context = context;
            _cacheService = cacheService;
        }

        public async Task<ServiceResponse<string>> ResendEmail(int id)
        {
            var serviceResponse = new ServiceResponse<string>();
            try {
                var metaData = await _context.MetaDatas.Where(x => x.MetaDataOwnerId == id).FirstOrDefaultAsync() ?? throw new Exception("You are not registered.");
                if (metaData.IsVerified == false) {
                    await SendEmail("Security code to complete registration.", metaData.Email);
                } else {
                    await SendEmail("Security code for password recovery.", metaData.Email);
                }
                serviceResponse.Success = true;
                serviceResponse.Message = "New security code has been sent to your email.";
            } catch (Exception ex) {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }
            return serviceResponse;
        }

        public async Task<ServiceResponse<string>> SendEmail(string topic, string recipient)
        {
            var serviceResponse = new ServiceResponse<string>();
            var email = new MimeMessage();
            var securityCode = CreateSecurityCode();
            await _cacheService.SetData($"code:{recipient}", securityCode, DateTime.Now.AddMinutes(3));
            email.From.Add(MailboxAddress.Parse(_configuration.GetSection("EmailConfiguration:AdminEmail").Value));
            email.To.Add(MailboxAddress.Parse(recipient));
            email.Subject = topic;
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
            serviceResponse.Success = true;
            serviceResponse.Message = "Security code sent to your email.";
            return serviceResponse;
        }

        public async Task<ServiceResponse<string>> VerifyEmail(int id, string securityCode)
        {
            var serviceResponse = new ServiceResponse<string>();
            try {
                var metaData = await _context.MetaDatas.Where(x => x.MetaDataOwnerId == id).FirstOrDefaultAsync() ?? throw new Exception("You are not registered.");
                if (metaData.IsVerified == true) {
                    throw new Exception("Your email has already been confirmed.");
                }
                var cacheCode = await _cacheService.GetData<string>($"code:{metaData.Email}") ?? throw new Exception("Security code has expired.");
                if (cacheCode != securityCode) {
                    throw new Exception("Wrong security code.");
                }
                metaData.IsVerified = true;
                await _context.SaveChangesAsync();
                serviceResponse.Success = true;
                serviceResponse.Message = "Email successfully confirmed.";
            } catch (Exception ex) {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }
            return serviceResponse;
        }

        private string CreateSecurityCode()
        {
            Random rnd = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

            char[] result = new char[8];

            for (int i = 0; i < 8; i++) {
                result[i] = chars[rnd.Next(chars.Length)];
            }
            return new string(result);
        }
    }
}