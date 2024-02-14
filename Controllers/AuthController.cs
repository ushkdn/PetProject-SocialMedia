using Microsoft.AspNetCore.Authorization;

namespace SocialNetwork.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        //TODO: Add ForgotPassword
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("Register")]
        public async Task<ActionResult<ServiceResponse<GetUserDto>>> Register(RegisterUserDto request)
        {
            return Ok(await _authService.Register(request));
        }
        [HttpPost("LoginIn")]
        public async Task<ActionResult<ServiceResponse<GetUserDto>>> Login(LoginInUserDto request)
        {
            return Ok(await _authService.LoginIn(request));
        }
        [HttpPost("test"), Authorize]
        public IActionResult Test()
        {
            return Ok("test");
        }
    }
}
