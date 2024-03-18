namespace SocialNetwork.Services.AuthService
{
    public interface IAuthService
    {
        Task<ServiceResponse<GetUserDto>> Register(RegisterUserDto request);

        Task<ServiceResponse<string>> LogIn(LogInUserDto request);

        Task<ServiceResponse<string>> ForgotPassword(string email);

        Task<ServiceResponse<string>> ResetPassword(string id, ResetPasswordDto request);
    }
}