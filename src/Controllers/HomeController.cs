using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using src.Model;
using System;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace src.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public HomeController(IConfiguration configuration)
        {
            _configuration = configuration;            
        }

        [HttpPost("send")]
        public IActionResult SendMessage([FromBody] WhatsAppMessage message)
        {
            var accountSid = _configuration["ApiSettings:SID"];
            var authToken = _configuration["ApiSettings:KEY"];

            TwilioClient.Init(accountSid, authToken);

            var from = new PhoneNumber("whatsapp:+14155238886"); // Número do Twilio
            var to = new PhoneNumber($"whatsapp:{message.To}");

            var sentMessage = MessageResource.Create(
                body: message.Body,
                from: from,
                to: to
            );

            return Ok(new { sid = sentMessage.Sid, status = sentMessage.Status });
        }
    }
}
