using GameTrilha.API.Services.Interfaces;
using GameTrilha.API.SetupConfigurations.Models;
using Microsoft.Extensions.Options;
using Resend;
using System.Reflection;

namespace GameTrilha.API.Services;

public class EmailSenderService : IEmailSenderService
{
    private readonly IResend _resend;
    private readonly EmailClientOptions _emailClientOptions;


    public EmailSenderService(IResend resend, IOptions<EmailClientOptions> emailClientOptions)
    {
        _resend = resend;
        _emailClientOptions = emailClientOptions.Value;
    }


    public async Task SendRecoverPasswordAsync(string email, string name, string code)
    {
        string template;
        await using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("GameTrilha.API.Assets.email-recover-password.html"))
        {
            TextReader tr = new StreamReader(stream);
            template = tr.ReadToEnd();
        }

        template = template.Replace("{@NOME_USUARIO}", name);
        template = template.Replace("{@CODIGO}", code);

        var message = new EmailMessage
        {
            From = $"no-reply@{_emailClientOptions.Domain}",
            Subject = "Recuperação de senha Jogo da Trilha",
            HtmlBody = template,
            To = new EmailAddressList { email }
        };

        await _resend.EmailSendAsync(message);
    }
}