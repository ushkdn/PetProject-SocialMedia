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
			try
			{
				var metaData = new MetaData();
				var EmailCheck = await _context.MetaDatas.Where(x => x.Email == request.Email).FirstOrDefaultAsync();
				if (EmailCheck != null)
				{
					throw new Exception("This email is already taken.");
				}
				string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
				var user = _mapper.Map<User>(request);
				metaData.OwnerId = user.Id;
				metaData.Email = request.Email;
				metaData.PasswordHash = passwordHash;
				_context.Users.Add(user);
				_context.MetaDatas.Add(metaData);
				serviceResponse.Data = _mapper.Map<GetUserDto>(user);
				serviceResponse.Success = true;
				serviceResponse.Message = "You have successfully registered.";
				await _context.SaveChangesAsync();
				await _emailService.SendEmail("Security code to complete registration.", request.Email);
			}
			catch (Exception ex)
			{
				serviceResponse.Success = false;
				serviceResponse.Message = ex.Message;
			}
			return serviceResponse;
		}

		public async Task<ServiceResponse<string>> LogIn(LogInUserDto request)
		{
			var serviceResponse = new ServiceResponse<string>();
			try
			{
				var metaData = new MetaData();
				metaData = await _context.MetaDatas.Where(x => x.Email == request.Email).FirstOrDefaultAsync() ?? throw new Exception("Wrong email or password.");
				if (!BCrypt.Net.BCrypt.Verify(request.Password, metaData.PasswordHash))
				{
					throw new Exception("Wrong email or password.");
				}
				if (metaData.IsVerified == false)
				{
					throw new Exception("You have not verified your email.");
				}
				string token = _tokenService.CreateToken(metaData);
				var refreshToken = _tokenService.CreateRefreshToken();
				await _tokenService.SetRefreshToken(refreshToken, metaData);
				serviceResponse.Data = token;
				serviceResponse.Success = true;
				serviceResponse.Message = "You are successfully logged in.";
			}
			catch (Exception ex)
			{
				serviceResponse.Success = false;
				serviceResponse.Message = ex.Message;
			}
			return serviceResponse;
		}

		public async Task<ServiceResponse<string>> ForgotPassword(string email)
		{
			var serviceResponse = new ServiceResponse<string>();
			try
			{
				var metaData = await _context.MetaDatas.Where(x => x.Email == email).FirstOrDefaultAsync() ?? throw new Exception("User not found.");
				await _emailService.SendEmail("Security code for password recovery.", email);
				serviceResponse.Success = true;
				serviceResponse.Message = "Security code sent to your email.";
			}
			catch (Exception ex)
			{
				serviceResponse.Success = false;
				serviceResponse.Message = ex.Message;
			}
			return serviceResponse;
		}

		public async Task<ServiceResponse<string>> ResetPassword(int id, ResetPasswordDto request)
		{
			var serviceResponse = new ServiceResponse<string>();
			try
			{
				var metaData = await _context.MetaDatas.Where(x => x.OwnerId == id).FirstOrDefaultAsync() ?? throw new Exception("User not found.");
				if (metaData.IsVerified == false)
				{
					throw new Exception("You have not verified your email.");
				}
				var cacheCode = await _cacheService.GetData<string>($"code:{metaData.Email}") ?? throw new Exception("Security code has expired.");
				if (cacheCode != request.Code)
				{
					throw new Exception("Invalid security code.");
				}
				metaData.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
				await _context.SaveChangesAsync();
				serviceResponse.Success = true;
				serviceResponse.Message = "You have successfully updated your password.";
			}
			catch (Exception ex)
			{
				serviceResponse.Success = false;
				serviceResponse.Message = ex.Message;
			}
			return serviceResponse;
		}
	}
}