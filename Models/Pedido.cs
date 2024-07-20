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
        public int DireccionId { get; set; }
        
        [Required]
        public int Cantidad { get; set; }
        
        [Required]
        public decimal CostoSinIVA { get; set; }
        public decimal PorcentajeDescuento { get; set; }
        
        public decimal CostoTotal 
        { 
            get { return CostoSinIVA - (CostoSinIVA * PorcentajeDescuento) * 0.13m; }
        }
        
        [Required]
        [StringLength(50)]
        public string Estado { get; set; } // Ej: "En proceso", "Facturado", etc.
    }
}
