using Microsoft.IdentityModel.Tokens;
using SocialNetwork.Services.TokenService;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace SocialNetwork.Services.AuthService
{
    public class AuthService : IAuthService, ITokenService
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITokenService _tokenService;
        public AuthService(IMapper mapper, DataContext context, IConfiguration configuration, IHttpContextAccessor httpContextAccessor, ITokenService tokenService)
        {
            _mapper = mapper;
            _context = context;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _tokenService = tokenService;
        }
        public async Task<ServiceResponse<GetUserDto>> Register(RegisterUserDto request)
        {
            var serviceResponse = new ServiceResponse<GetUserDto>();
            try {
                var user = new User();
                var EmailCheck = await _context.Users.Where(x => x.Email == request.Email).FirstOrDefaultAsync();
                if (EmailCheck != null) {
                    throw new Exception("This email is already taken.");
                }
                string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
                user = _mapper.Map<User>(request);
                user.PasswordHash = passwordHash;
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                serviceResponse.Data = _mapper.Map<GetUserDto>(user);
                serviceResponse.Success = true;
                serviceResponse.Message = "You have successfully registered.";

            } catch (Exception ex) {
                serviceResponse.Data = null;
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }
            return serviceResponse;

        }
        public async Task<ServiceResponse<string>> LoginIn(LoginInUserDto request)
        {
            var serviceResponse = new ServiceResponse<string>();
            try {
                var user = new User();
                user = await _context.Users.Where(x => x.Email == request.Email).FirstOrDefaultAsync() ?? throw new Exception("Wrong email or password.");
                if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash)) {
                    throw new Exception("Wrong email or password.");
                }
                string token = _tokenService.CreateToken(request);
                var refreshToken = CreateRefreshToken(user.Id);
                _tokenService.SetRefreshToken(refreshToken, ref user);
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

        

    }
}
