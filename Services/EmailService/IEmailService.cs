namespace SocialNetwork.Services.EmailService
{
    public interface IEmailService
    {
        Task<ServiceResponse<string>> SendEmail(string recipient);
        Task<ServiceResponse<string>> VerifyEmail(string securityCode);
    }
}
