namespace SocialNetwork.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<ServiceResponse<GetUserDto>>> Register([FromBody]RegisterUserDto request)
        {
            return Ok(await _authService.Register(request));
        }
        [HttpPost("log-in")]
        public async Task<ActionResult<ServiceResponse<GetUserDto>>> Login([FromBody]LoginInUserDto request)
        {
            return Ok(await _authService.LoginIn(request));
        }
        [HttpPost("forgot-password")]
        public async Task<ActionResult<ServiceResponse<string>>> ForgotPassword([FromBody,EmailAddress] string email)
        {
            return Ok(await _authService.ForgotPassword(email));
        }
        [HttpPost("{id}/reset-password")]
        public async Task<ActionResult<ServiceResponse<string>>> ResetPassword([FromRoute]int id, [FromBody]ResetPasswordDto request)
        {
            return Ok(await _authService.ResetPassword(id, request));
        }
    }
}
