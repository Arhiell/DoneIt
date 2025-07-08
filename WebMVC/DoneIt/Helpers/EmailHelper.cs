using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

// Helpers para enviar correos electrónicos
namespace DoneIt.Helpers
{
    public static class EmailHelper
    {
        public static async Task EnviarCorreo(string destino, string asunto, string cuerpo)
        {
            var remitente = Environment.GetEnvironmentVariable("EMAIL_REMITENTE"); //Asegurarse de configurar esta variable de entorno
            var password = Environment.GetEnvironmentVariable("EMAIL_PASSWORD"); //Asegurarse de configurar esta variable de entorno

            var mensaje = new MailMessage(remitente, destino, asunto, cuerpo);

            using var smtp = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(remitente, password)
            };

            await smtp.SendMailAsync(mensaje);
        }
    }
}
