using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReservasSalas.Models
{
    public class Usuario
    {
        public int Id { get; set; }

        [Required]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Mail { get; set; } = string.Empty;

        [Required]
        [Range(1, 9)]
        public int Portal { get; set; }

        [Required]
        [RegularExpression("^[B1-4]$", ErrorMessage = "Piso debe ser B, 1, 2, 3 o 4")]
        public string Piso { get; set; } = string.Empty;

        [Required]
        [RegularExpression("^[a-dA-D]$", ErrorMessage = "Letra debe ser a, b, c o d")]
        public string Letra { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        public bool Confirmado { get; set; } = false;
        public string? TokenConfirmacion { get; set; } = Guid.NewGuid().ToString();
    }
}
