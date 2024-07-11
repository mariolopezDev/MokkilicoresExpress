using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MokkilicoresExpress.Models
{
    public class Direccion
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Cliente")]
        public int ClienteId { get; set; }
        public Cliente Cliente { get; set; }

        [Required]
        public string Provincia { get; set; }

        [Required]
        public string Canton { get; set; }

        [Required]
        public string Distrito { get; set; }

        [Required]
        public string PuntoEnWaze { get; set; } // URL al punto en Waze

        public bool EsCondominio { get; set; }
    }
}
