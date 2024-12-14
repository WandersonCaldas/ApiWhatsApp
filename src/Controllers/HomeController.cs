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
            var to = new PhoneNumber($"whatsapp:{NormalizePhoneNumber(message.To)}");

            var sentMessage = MessageResource.Create(
                body: message.Body,
                from: from,
                to: to
            );

            return Ok(new { sid = sentMessage.Sid, status = sentMessage.Status });
        }

        private static string NormalizePhoneNumber(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                throw new ArgumentException("O número de telefone não pode ser vazio.");

            // Remove caracteres não numéricos
            var digitsOnly = new string(phoneNumber.Where(char.IsDigit).ToArray());

            // Verifica se o número já tem o código do país
            if (!digitsOnly.StartsWith("55"))
            {
                digitsOnly = "55" + digitsOnly; // Adiciona o código do Brasil (55)
            }

            // Remove excesso de "9" após o DDD, se necessário
            if (digitsOnly.Length == 13 && digitsOnly.StartsWith("55"))
            {
                // Exemplo: 55619996646496 (excesso) → 556196646496 (ajustado)
                var ddd = digitsOnly.Substring(2, 2); // Primeiro pega o DDD (ex.: "61")
                var rest = digitsOnly.Substring(4);  // Resto do número após o DDD
                if (rest.Length == 9 && rest.StartsWith("9")) // Número tem 9 dígitos?
                {
                    rest = rest.Substring(1); // Remove o "9" excedente
                }
                digitsOnly = "55" + ddd + rest;
            }

            // Retorna o número no formato +55XXXXXXXXXXX
            return "+" + digitsOnly;
        }
    }    
}
