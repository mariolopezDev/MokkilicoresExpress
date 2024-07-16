using System.ComponentModel.DataAnnotations;

namespace MokkilicoresExpress.Models
{
    public class Pedido
    {
        public int Id { get; set; }
        
        [Required]
        public int ClienteId { get; set; }
        
        [Required]
        public int InventarioId { get; set; }
        
        [Required]
        public int Cantidad { get; set; }
        
        [Required]
        public decimal CostoSinIVA { get; set; }
        
        public decimal CostoTotal 
        { 
            get { return CostoSinIVA * 1.13M; } // IVA de 13% 
        }
        
        [Required]
        [StringLength(50)]
        public string Estado { get; set; } // Ej: "En proceso", "Facturado", etc.
    }
}
