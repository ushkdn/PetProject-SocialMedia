namespace SocialNetwork.Services.EmailService
{
    public interface IEmailService
    {
        Task<ServiceResponse<string>> SendEmail(string topic, string recipient);

        Task<ServiceResponse<string>> VerifyEmail(int id, string securityCode);

        Task<ServiceResponse<string>> ResendEmail(int id);
    }
}