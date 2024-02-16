using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace SocialNetwork.Services.TokenService
{
    public class TokenService : ITokenService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;

        public TokenService(IHttpContextAccessor httpContextAccessor, DataContext context, IConfiguration configuration)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
            _configuration = configuration;
        }

        public async Task<ServiceResponse<string>> RefreshToken()
        {
            var serviceResponse = new ServiceResponse<string>();
            var refreshTokenCookie = _httpContextAccessor.HttpContext.Request.Cookies["refreshToken"];
            var refreshCookieValue = refreshTokenCookie.Split(".");
            var refreshCookieOwner = refreshCookieValue[1];

            var metaData = await _context.MetaDatas.FindAsync(Convert.ToInt32(refreshCookieOwner));

            if (string.IsNullOrEmpty(refreshTokenCookie)) {
                serviceResponse.Data = null;
                serviceResponse.Success = false;
                serviceResponse.Message = "Missing refresh token.";
                return serviceResponse;

            } else if (metaData == null || !metaData.RefreshToken.Equals(refreshTokenCookie, StringComparison.OrdinalIgnoreCase)) {
                serviceResponse.Data = null;
                serviceResponse.Success = false;
                serviceResponse.Message = "Invalid refresh token.";
                return serviceResponse;
            }
            if (metaData.TokenExpires < DateTime.Now) {
                serviceResponse.Data = null;
                serviceResponse.Success = false;
                serviceResponse.Message = "Token expired.";
                return serviceResponse;
            }
            string token = CreateToken(metaData);
            var newRefreshToken = CreateRefreshToken(metaData.Id);
            await SetRefreshToken(newRefreshToken, metaData);
            serviceResponse.Data = token;
            serviceResponse.Success = true;
            serviceResponse.Message = "Refresh token updated successfully";
            return serviceResponse;
        }

        public RefreshToken CreateRefreshToken(int userId)
        {
            var refreshToken = new RefreshToken
            {
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)) + "." + Convert.ToString(userId),
                Expires = DateTime.Now.AddDays(7)
            };
            return refreshToken;
        }

        public async Task SetRefreshToken(RefreshToken newRefreshToken, MetaData metaData)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = newRefreshToken.Expires
            };
            _httpContextAccessor.HttpContext.Response.Cookies.Append("refreshToken", newRefreshToken.Token, cookieOptions);
            metaData.RefreshToken = newRefreshToken.Token;
            metaData.TokenCreated = DateTime.SpecifyKind(Convert.ToDateTime(newRefreshToken.Created), DateTimeKind.Utc);
            metaData.TokenExpires = DateTime.SpecifyKind(Convert.ToDateTime(newRefreshToken.Expires), DateTimeKind.Utc);
            await _context.SaveChangesAsync();
        }

        public string CreateToken(MetaData metaData)
        {
            List<Claim> claims = new List<Claim> { new Claim(ClaimTypes.Email, metaData.Email) };
            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:DefaultToken").Value));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds
            );
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
        }
    }
}
