using System.ComponentModel.DataAnnotations;

namespace MokkilicoresExpress.Models
{
    public class Direccion
    {
        public int Id { get; set; }

        [Required]
        public int ClienteId { get; set; }

        [Required]
        public string Provincia { get; set; }

        [Required]
        public string Canton { get; set; }

        [Required]
        public string Distrito { get; set; }

        [Required]
        [Url]
        public string PuntoEnWaze { get; set; }

        [Required]
        public bool EsCondominio { get; set; }

        [Required]
        public bool EsPrincipal { get; set; }
    }
}
