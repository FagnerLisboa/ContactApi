using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ContactApi.Models;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

[Route("api/[controller]")]
[ApiController]
public class ContactController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public ContactController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [Authorize] // Exige autenticação JWT para acessar este endpoint
    [HttpPost("send")]
    public async Task<IActionResult> SendEmail([FromBody] ContactFormModel contactForm)
    {
        if (ModelState.IsValid)
        {
            // Lê a chave API do appsettings.json
            var apiKey = _configuration["SendGrid:ApiKey"];

            if (string.IsNullOrEmpty(apiKey))
            {
                return StatusCode(500, new { message = "Erro de configuração: a chave API não foi fornecida." });
            }

            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("fnlisboa.dev@gmail.com", "Fagner Lisboa");
            var subject = contactForm.Subject;
            var to = new EmailAddress("fnlisboa.dev@gmail.com", "Fagner");
            var plainTextContent = $"Nome: {contactForm.Name}\nEmail: {contactForm.Email}\n\n{contactForm.Body}";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, plainTextContent);

            var response = await client.SendEmailAsync(msg);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return Ok(new { message = "Email enviado com sucesso!" });
            }
            else
            {
                return StatusCode((int)response.StatusCode, new { message = "Erro ao enviar email." });
            }
        }

        return BadRequest(new { message = "Dados inválidos no formulário." });
    }
}
