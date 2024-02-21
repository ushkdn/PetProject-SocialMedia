using static Org.BouncyCastle.Crypto.Engines.SM2Engine;

namespace SocialNetwork.Services.AuthService
{
    public class AuthService : IAuthService
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;
        private readonly ITokenService _tokenService;
        private readonly IEmailService _emailService;
        private readonly ICacheService _cacheService;

        public AuthService(IMapper mapper, DataContext context, ITokenService tokenService, IEmailService emailService, ICacheService cacheService)
        {
            _mapper = mapper;
            _context = context;
            _tokenService = tokenService;
            _emailService = emailService;
            _cacheService = cacheService;
        }

        public async Task<ServiceResponse<GetUserDto>> Register(RegisterUserDto request)
        {
            var serviceResponse = new ServiceResponse<GetUserDto>();
            try {
                var user = new User();
                var metaData = new MetaData();
                var EmailCheck = await _context.MetaDatas.Where(x => x.Email == request.Email).FirstOrDefaultAsync();
                if (EmailCheck != null) {
                    throw new Exception("This email is already taken.");
                }
                string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
                user = _mapper.Map<User>(request);
                metaData.Email = request.Email;
                metaData.PasswordHash = passwordHash;
                _context.Users.Add(user);
                metaData.Id = user.Id;
                _context.MetaDatas.Add(metaData);
                serviceResponse.Data = _mapper.Map<GetUserDto>(user);
                serviceResponse.Success = true;
                serviceResponse.Message = "You have successfully registered.";
                await _context.SaveChangesAsync();
                _emailService.SendEmail("Security code to complete registration.", request.Email);
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
                var metaData = new MetaData();
                metaData = await _context.MetaDatas.Where(x => x.Email == request.Email).FirstOrDefaultAsync() ?? throw new Exception("Wrong email or password.");
                if (!BCrypt.Net.BCrypt.Verify(request.Password, metaData.PasswordHash)) {
                    throw new Exception("Wrong email or password.");
                }
                if (metaData.IsVerified == false) {
                    throw new Exception("You have not verified your email.");
                }
                string token = _tokenService.CreateToken(metaData);
                var refreshToken = _tokenService.CreateRefreshToken(metaData.Id);
                await _tokenService.SetRefreshToken(refreshToken, metaData);
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
        public async Task<ServiceResponse<string>> ForgotPassword(string email)
        {
            var serviceResponse = new ServiceResponse<string>();
            try {
                var metaData = await _context.MetaDatas.Where(x => x.Email == email).FirstOrDefaultAsync() ?? throw new Exception("User not found.");
                _emailService.SendEmail("Security code for password recovery.", email);
                serviceResponse.Data = null;
                serviceResponse.Success = true;
                serviceResponse.Message = "Security code sent to your email.";

            } catch (Exception ex) {
                serviceResponse.Data = null;
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }
            return serviceResponse;
        }
        public async Task<ServiceResponse<string>> ResetPassword(string email, ResetPasswordDto request)
        {
            var serviceResponse = new ServiceResponse<string>();
            try {
                var user = await _context.MetaDatas.Where(x => x.Email == email).FirstOrDefaultAsync() ?? throw new Exception("User not found.");
                if (user.IsVerified == false) {
                    throw new Exception("You have not verified your email.");
                }
                var cacheCode = await _cacheService.GetData<string>($"SecurityCode:{email}") ?? throw new Exception("Security code has expired.");
                if (cacheCode != request.SecurityCode) {
                    throw new Exception("Invalid security code.");
                }
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
                await _context.SaveChangesAsync();
                serviceResponse.Data = null;
                serviceResponse.Success = true;
                serviceResponse.Message = "You have successfully updated your password.";

            } catch (Exception ex) {
                serviceResponse.Data = null;
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }
            return serviceResponse;
        }
    }
}
