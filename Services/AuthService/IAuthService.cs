﻿namespace SocialNetwork.Services.AuthService
{
    public interface IAuthService
    {
        Task<ServiceResponse<GetUserDto>> Register(RegisterUserDto request);
        Task<ServiceResponse<string>> LoginIn(LoginInUserDto request);
        Task<ServiceResponse<string>> ForgotPassword(string email);
        Task<ServiceResponse<string>> ResetPassword(string email, ResetPasswordDto request);
    }
}

