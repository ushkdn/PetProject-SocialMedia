namespace SocialNetwork.Services.EmailService
{
    public interface IEmailService
    {
        Task<ServiceResponse<string>> SendEmail(string topic,string recipient);
        Task<ServiceResponse<string>> VerifyEmail(string securityCode);
        Task<ServiceResponse<string>> ResendEmail(string securityCode);
    }
}
