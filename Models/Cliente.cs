using System.ComponentModel.DataAnnotations;

namespace MokkilicoresExpress.Models
{
    public class Cliente
    {
        public int Id { get; set; }
        [Required]
        public string Identificacion { get; set; }
        [Required]
        [StringLength(100)]
        public string Nombre { get; set; }
        [Required]
        [StringLength(100)]
        public string Apellido { get; set; }
        public decimal DineroCompradoTotal { get; set; }
        public decimal DineroCompradoUltimoAnio { get; set; }
        public decimal DineroCompradoUltimosSeisMeses { get; set; }

        public string NombreCompleto => $"{Nombre} {Apellido}";
        public List<Direccion> Direcciones { get; set; } = new List<Direccion>();

    }
}
