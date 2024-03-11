namespace SocialNetwork.Services.TokenService
{
    public class TokenService : ITokenService
    {
        private readonly IHttpContextAccessor _http;
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;

        public TokenService(IHttpContextAccessor http, DataContext context, IConfiguration configuration)
        {
            _http = http;
            _context = context;
            _configuration = configuration;
        }

        public async Task<ServiceResponse<string>> RefreshToken()
        {
            var serviceResponse = new ServiceResponse<string>();
            var refreshTokenCookie = _http.HttpContext.Request.Cookies["refreshToken"];
            try {
                var metaData = await _context.MetaDatas.Where(x => x.RefreshToken == refreshTokenCookie).FirstOrDefaultAsync();

                if (string.IsNullOrEmpty(refreshTokenCookie)) {
                    throw new Exception("Missing refresh token.");
                }
                if (metaData == null || !metaData.RefreshToken.Equals(refreshTokenCookie, StringComparison.OrdinalIgnoreCase)) {
                    throw new Exception("Invalid refresh token.");
                }
                if (metaData.TokenExpires < DateTime.Now) {
                    throw new Exception("Token expired.");
                }

                string token = CreateToken(metaData);
                var newRefreshToken = CreateRefreshToken();
                await SetRefreshToken(newRefreshToken, metaData);

                serviceResponse.Data = token;
                serviceResponse.Success = true;
                serviceResponse.Message = "Refresh token updated successfully";
            } catch (Exception ex) {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }
            return serviceResponse;
        }

        public RefreshToken CreateRefreshToken()
        {
            var refreshToken = new RefreshToken
            {
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                Created = DateTime.UtcNow,
                Expires = DateTime.UtcNow.AddDays(7)
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
            _http.HttpContext.Response.Cookies.Append("refreshToken", newRefreshToken.Token, cookieOptions);
            metaData.RefreshToken = newRefreshToken.Token;
            metaData.TokenCreated = newRefreshToken.Created;
            metaData.TokenExpires = newRefreshToken.Expires;
            await _context.SaveChangesAsync();
        }

        public string CreateToken(MetaData metaData)
        {
            List<Claim> claims = new List<Claim> { new Claim("Id", $"{metaData.MetaDataOwnerId}"), new Claim(ClaimTypes.Email, metaData.Email), new Claim(ClaimTypes.Role, "Client") };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:DefaultToken").Value));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
        }
    }
}