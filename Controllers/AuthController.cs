using Microsoft.AspNetCore.Mvc;
using ReservasSalas.Data;
using ReservasSalas.Models;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;

namespace ReservasSalas.Controllers
{
    public class AuthController : Controller
    {
        private readonly AppDbContext _context;

        public AuthController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /Auth/Login
        public IActionResult Login() => View();

        // POST: /Auth/Login
        [HttpPost]
        public async Task<IActionResult> Login(string mail, string password)
        {
            var user = await _context.Usuarios.FirstOrDefaultAsync(u => u.Mail == mail);

            if (user == null || !VerifyPassword(password, user.PasswordHash))
            {
                ModelState.AddModelError("", "Usuario o contraseña incorrectos");
                return View();
            }

            if (!user.Confirmado)
            {
                ModelState.AddModelError("", "Debe confirmar su correo antes de entrar.");
                return View();
            }

            HttpContext.Session.SetString("UsuarioId", user.Id.ToString());
            return RedirectToAction("Index", "Home");
        }

        // GET: /Auth/Register
        public IActionResult Register() => View();

        // POST: /Auth/Register
        [HttpPost]
        public async Task<IActionResult> Register(Usuario model, string password)
        {
            if (!ModelState.IsValid) return View(model);

            model.PasswordHash = HashPassword(password);
            model.TokenConfirmacion = Guid.NewGuid().ToString();

            _context.Usuarios.Add(model);
            await _context.SaveChangesAsync();

            try
            {
                await EnviarCorreoConfirmacion(model);
                TempData["Mensaje"] = "Cuenta creada correctamente ✅. Revisa tu correo para confirmar tu cuenta.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "No se pudo enviar el correo: " + ex.Message;
            }

            return RedirectToAction("Login", "Auth");
        }

        private async Task EnviarCorreoConfirmacion(Usuario user)
        {
            string link = Url.Action("Confirmar", "Auth", new { token = user.TokenConfirmacion }, Request.Scheme);

            using var smtp = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new System.Net.NetworkCredential("ochentainueve@gmail.com", "pmtm nyya mwzb oghb"),
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

            await smtp.SendMailAsync(mail);
            Console.WriteLine($"Correo enviado a {user.Mail}");
        }

        // Confirmación de correo
        [HttpGet]
        public async Task<IActionResult> Confirmar(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                TempData["Error"] = "Token inválido ❌";
                return RedirectToAction("Login", "Auth");
            }

            var user = await _context.Usuarios.FirstOrDefaultAsync(u => u.TokenConfirmacion == token);
            if (user == null)
            {
                TempData["Error"] = "Token no encontrado ❌";
                return RedirectToAction("Login", "Auth");
            }

            user.Confirmado = true;
            user.TokenConfirmacion = null;
            _context.Usuarios.Update(user);
            await _context.SaveChangesAsync();

            TempData["Mensaje"] = "Cuenta confirmada correctamente ✅. Ahora puedes iniciar sesión.";
            return RedirectToAction("Login", "Auth");
        }

        // Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        #region PBKDF2
        private string HashPassword(string password)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(16);
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100_000, HashAlgorithmName.SHA256);
            byte[] hash = pbkdf2.GetBytes(32);
            return Convert.ToBase64String(salt) + ":" + Convert.ToBase64String(hash);
        }

        private bool VerifyPassword(string password, string storedHash)
        {
            var parts = storedHash.Split(':');
            if (parts.Length != 2) return false;

            byte[] salt = Convert.FromBase64String(parts[0]);
            byte[] hash = Convert.FromBase64String(parts[1]);

            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100_000, HashAlgorithmName.SHA256);
            byte[] computedHash = pbkdf2.GetBytes(32);

            return computedHash.SequenceEqual(hash);
        }
        #endregion
    }
}
