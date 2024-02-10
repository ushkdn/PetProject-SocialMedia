using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace SocialNetwork.Services.AuthService
{
    public class AuthService : IAuthService
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;
        public AuthService(IMapper mapper, DataContext context, IConfiguration configuration)
        {
            _mapper = mapper;
            _context= context;
            _configuration = configuration;
        }
        public async Task<ServiceResponse<GetUserDto>> Register(RegisterUserDto request) 
        {
            var serviceResponse = new ServiceResponse<GetUserDto>();
            try {
                var user = new User();
                var EmailCheck= await _context.Users.Where(x => x.Email == request.Email).FirstOrDefaultAsync();
                if (EmailCheck != null) {
                    throw new Exception("This email is already taken.");
                }
                string passwordHash=BCrypt.Net.BCrypt.HashPassword(request.Password);
                user = _mapper.Map<User>(request);
                user.PasswordHash = passwordHash;
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                serviceResponse.Data = _mapper.Map<GetUserDto>(user);
                serviceResponse.Success = true;
                serviceResponse.Message = "You have successfully registered.";

            } catch(Exception ex) {
                serviceResponse.Data = null;
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }
            return serviceResponse;

        }
        public async Task<ServiceResponse<string>> LoginIn (LoginInUserDto request) {
            var serviceResponse = new ServiceResponse<string>();
            try {
                var user = new User();
                user = await _context.Users.Where(x => x.Email == request.Email).FirstOrDefaultAsync() ?? throw new Exception("Wrong email or password.");
                if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash)) {
                    throw new Exception("Wrong email or password.");
                }
                string token = CreateToken(request);
                var refreshToken = CreateRefreshToken();
                SetRefreshToken(refreshToken);
                serviceResponse.Data = token;
                serviceResponse.Success = true;
                serviceResponse.Message = "You are successfully logged in.";

            } catch (Exception ex) {
                serviceResponse.Data = null;
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }
            return serviceResponse;

        }
        private RefreshToken CreateRefreshToken()
        {
            var refreshToken = new RefreshToken {
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                Expires = DateTime.Now.AddDays(7)
            };
            return refreshToken;
        }

        private void SetRefreshToken(RefreshToken newRefreshToken)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = newRefreshToken.Expires
            };
            IQueryCollection..Append("refreshToken", newRefreshToken.Token, cookieOptions);
        }
       
        private string CreateToken(LoginInUserDto user) {
            List<Claim> claims = new List<Claim> { new Claim(ClaimTypes.Email, user.Email), new Claim(ClaimTypes.Role, "Client") };
            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:DefaultToken").Value));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials:creds
            );
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
        }

    }
}
