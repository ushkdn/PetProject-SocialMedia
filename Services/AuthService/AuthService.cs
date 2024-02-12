namespace SocialNetwork.Services.AuthService
{
    public class AuthService : IAuthService
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;
        private readonly ITokenService _tokenService;

        public AuthService(IMapper mapper, DataContext context, ITokenService tokenService)
        {
            _mapper = mapper;
            _context = context;
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
                string token = _tokenService.CreateToken(user);
                var refreshToken = _tokenService.CreateRefreshToken(user.Id);
                await _tokenService.SetRefreshToken(refreshToken, user);
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
