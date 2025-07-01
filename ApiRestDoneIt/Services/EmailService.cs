using MailKit.Net.Smtp;
using MimeKit;

namespace ApiRestDoneIt.Services
{
    public class EmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task EnviarCorreoRecuperacion(string destinatario, string token)
        {
            var mensaje = new MimeMessage();
            mensaje.From.Add(new MailboxAddress("DoneIt App", _config["Email:From"]));
            mensaje.To.Add(MailboxAddress.Parse(destinatario));
            mensaje.Subject = "Recuperación de contraseña";

            var link = $"{_config["Frontend:ResetPasswordUrl"]}?token={token}";

            mensaje.Body = new TextPart("plain")
            {
                Text = $"Hola, para restablecer tu contraseña hacé clic en el siguiente enlace:\n{link}\nEste enlace es válido por 1 hora."
            };

            using var smtp = new SmtpClient();  
            await smtp.ConnectAsync(_config["Email:Smtp"], int.Parse(_config["Email:Port"]), MailKit.Security.SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_config["Email:From"], _config["Email:Password"]);
            await smtp.SendAsync(mensaje);
            await smtp.DisconnectAsync(true);
        }
    }
}
