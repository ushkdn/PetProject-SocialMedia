namespace SocialNetwork.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MailController : ControllerBase
    {
        private readonly IEmailService _emailService;

        public MailController(IEmailService emailService)
        {
            _emailService = emailService;
        }

        [HttpPost("Verify-Email")]
        public async Task<ActionResult<ServiceResponse<string>>> VerifyEmail(string email,string securityCode)
        {
            return Ok(await _emailService.VerifyEmail(email, securityCode));
        }
        [HttpPost("Resend-SecurityCode")]
        public async Task<ActionResult<ServiceResponse<string>>> ResendSecurityCode(string email)
        {
            return Ok(await _emailService.ResendEmail(email));
        }
    }
}
