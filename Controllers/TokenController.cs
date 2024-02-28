namespace SocialNetwork.Controllers
{
    [Route("api/token")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly ITokenService _tokenService;

        public TokenController(ITokenService tokenService)
        {
            _tokenService = tokenService;
        }

        [HttpPost]
        public async Task<ActionResult<ServiceResponse<string>>> RefreshToken()
        {
            return Ok(await _tokenService.RefreshToken());
        }
    }
}
