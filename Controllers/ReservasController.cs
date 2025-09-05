using Microsoft.AspNetCore.Mvc;
using ReservasSalas.Data;
using Microsoft.EntityFrameworkCore;
using ReservasSalas.Filters;

namespace ReservasSalas.Controllers
{
    [Route("reservas")]
    [ApiController]
    [AuthorizeUser] 
    public class ReservasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ReservasController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetReservas([FromQuery] string mes)
        {
            var fecha = DateTime.Parse(mes + "-01");
            var inicio = new DateTime(fecha.Year, fecha.Month, 1);
            var fin = inicio.AddMonths(1).AddDays(-1);

            var reservas = await _context.Reservas
                .Where(r => r.FechaReserva >= inicio && r.FechaReserva <= fin)
                .Select(r => r.FechaReserva.ToString("yyyy-MM-dd"))
                .ToListAsync();

            return Ok(new { reservas });
        }
    }
}
