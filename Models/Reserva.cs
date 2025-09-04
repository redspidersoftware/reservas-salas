namespace ReservasSalas.Models
{
    public class Reserva
    {
        public int Id { get; set; }
        public int SalaId { get; set; }
        public int UsuarioId { get; set; }
        public DateTime FechaReserva { get; set; }
    }
}
