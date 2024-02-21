﻿namespace SocialNetwork.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
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
        [HttpPost("Log-In")]
        public async Task<ActionResult<ServiceResponse<GetUserDto>>> Login(LoginInUserDto request)
        {
            return Ok(await _authService.LoginIn(request));
        }
        [HttpPost("Forgot-Password")]
        public async Task<ActionResult<ServiceResponse<string>>> ForgotPassword([EmailAddress] string email)
        {
            return Ok(await _authService.ForgotPassword(email));
        }
        [HttpPost("Reset-Password")]
        public async Task<ActionResult<ServiceResponse<string>>> ResetPassword(string email, ResetPasswordDto request)
        {
            return Ok(await _authService.ResetPassword(email, request));
        }
    }
}
