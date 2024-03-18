namespace SocialNetwork.Controllers
{
    [Route("api/email")]
    [ApiController]
    public class MailController : ControllerBase
    {
        private readonly IEmailService _emailService;

        public MailController(IEmailService emailService)
        {
            _emailService = emailService;
        }

        [HttpPost("{id}/verify-email")]
        public async Task<ActionResult<ServiceResponse<string>>> VerifyEmail([FromRoute] string id, [FromBody] string securityCode)
        {
            return Ok(await _emailService.VerifyEmail(id, securityCode));
        }

        [HttpPost("{id}/resend-code")]
        public async Task<ActionResult<ServiceResponse<string>>> ResendCode([FromRoute] string id)
        {
            return Ok(await _emailService.ResendCode(id));
        }
    }
}