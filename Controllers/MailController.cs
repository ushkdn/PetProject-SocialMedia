using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using MimeKit.Text;
using MailKit.Security;
using SocialNetwork.Services.EmailService;

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
        public ActionResult<ServiceResponse<string>> VerifyEmail(string securityCode)
        {
            return Ok(_emailService.VerifyEmail(securityCode));
        }
    }
}
