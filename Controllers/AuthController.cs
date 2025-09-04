using Microsoft.AspNetCore.Mvc;
using ReservasSalas.Data;
using ReservasSalas.Models;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;


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
           if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
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
            if (!ModelState.IsValid)
                return View(model);

            model.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);

            _context.Usuarios.Add(model);
            await _context.SaveChangesAsync();

            await EnviarCorreoConfirmacion(model);

            return RedirectToAction("Login");
        }

        private async Task EnviarCorreoConfirmacion(Usuario user)
        {
            string link = Url.Action("Confirmar", "Auth", new { token = user.TokenConfirmacion }, Request.Scheme)!;

            using var smtp = new SmtpClient("smtp.gmail.com") // ⚠️ Cambia por tu SMTP real
            {
                Port = 587,
                Credentials = new System.Net.NetworkCredential("ochentainueve@gmail.com", "pmtm nyya mwzb oghb"),
                EnableSsl = true,
            };

            var mail = new MailMessage("no-reply@reservas.com", user.Mail,
                "Confirma tu cuenta",
                $"Haz clic aquí para confirmar: {link}");

            await smtp.SendMailAsync(mail);
        }

        public async Task<IActionResult> Confirmar(string token)
        {
            var user = await _context.Usuarios.FirstOrDefaultAsync(u => u.TokenConfirmacion == token);
            if (user == null) return NotFound();

            user.Confirmado = true;
            await _context.SaveChangesAsync();

            return RedirectToAction("Login");
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

    }
}
