using System.ComponentModel.DataAnnotations;

namespace MokkilicoresExpress.Models
{
    public class Cliente
    {
        [Required]
        public string Identificacion { get; set; }
        [Required]
        [StringLength(100)]
        public string NombreCompleto { get; set; }
        public string Provincia { get; set; }
        public string Canton { get; set; }
        public string Distrito { get; set; }
        public decimal DineroCompradoTotal { get; set; }
        public decimal DineroCompradoUltimoAnio { get; set; }
        public decimal DineroCompradoUltimosSeisMeses { get; set; }
    }
}
