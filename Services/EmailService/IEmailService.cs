namespace SocialNetwork.Services.EmailService
{
    public interface IEmailService
    {
        ServiceResponse<string> SendEmail(string recipient);
        ServiceResponse<string> VerifyEmail(string securityCode);
    }
}
