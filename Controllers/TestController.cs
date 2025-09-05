using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using System.Threading.Tasks;
using ReservasSalas.Models; // Asegúrate de que apunte a donde está tu clase Usuario

namespace ReservasSalas.Controllers
{
    public class TestController : Controller
    {
        public async Task<IActionResult> EnviarPrueba()
        {
            // 🔹 Usuario de prueba
            var user = new Usuario
            {
                Nombre = "Prueba",
                Mail = "ochentainueve@gmail.com", // <-- cámbialo por un correo tuyo
                TokenConfirmacion = Guid.NewGuid().ToString()
            };

            await EnviarCorreoConfirmacion(user);

            return Content("✅ Correo de prueba enviado. Revisa tu bandeja.");
        }

        private async Task EnviarCorreoConfirmacion(Usuario user)
        {
            if (string.IsNullOrEmpty(user.Mail)) return;

            string link = Url.Action("Confirmar", "Auth", new { token = user.TokenConfirmacion }, Request.Scheme)!;

            using var smtp = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new System.Net.NetworkCredential("ochentainueve@gmail.com", "pmtm nyya mwzb oghb"), // ⚠️ tu App Password
                EnableSsl = true,
            };

           var mail = new MailMessage
            {
                From = new MailAddress("no-reply@reservas.com", "ReservasSalas"),
                Subject = "Confirma tu cuenta",
                Body = $"Hola {user.Nombre},\n\nPor favor confirma tu cuenta haciendo clic en el enlace: {link}",
                IsBodyHtml = false
            };

        mail.To.Add(user.Mail);

        try
{
    await smtp.SendMailAsync(mail);
    Console.WriteLine($"Correo enviado a {user.Mail}");
}
catch (Exception ex)
{
    Console.WriteLine("Error enviando correo: " + ex.Message);
}
        }
    }
}
