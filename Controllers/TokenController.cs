namespace SocialNetwork.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly ITokenService _tokenService;
        public TokenController(ITokenService tokenService)
        {
            _tokenService = tokenService;
        }
        [HttpPost("Refresh-Token")]
        public async Task<ActionResult<ServiceResponse<string>>> RefreshToken()
        {
            return Ok(await _tokenService.RefreshToken());
        }
    }
}
