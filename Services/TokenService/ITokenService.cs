using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace SocialNetwork.Services.TokenService
{
    public interface ITokenService
    {
        Task<ServiceResponse<string>> RefreshToken();
        RefreshToken CreateRefreshToken(int userId);
        void SetRefreshToken(RefreshToken newRefreshToken, ref User user);
        string CreateToken(LoginInUserDto user);
    }
}
