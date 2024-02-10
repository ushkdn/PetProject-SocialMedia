using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace SocialNetwork.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService) {
            _authService = authService;
        }

        [HttpPost("Register")]
        public async Task<ActionResult<ServiceResponse<GetUserDto>>> Register(RegisterUserDto newUser) {
            return Ok(await _authService.Register(newUser));
        }
        [HttpPost("LoginIn")]
        public async Task<ActionResult<ServiceResponse<GetUserDto>>> Login(LoginInUserDto user) {
            return Ok(await _authService.LoginIn(user));
        }
    }
}
