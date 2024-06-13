using System;

namespace MokkilicoresExpress.Models
{
    public class Pedido
    {
        public int Id { get; set; }
        public string ProductoId { get; set; }
        public int Cantidad { get; set; }
        public decimal CostoSinIVA { get; set; }
        public decimal CostoTotal { get { return CostoSinIVA * 1.13M; } } // IVA de 13%
        public string Estado { get; set; } // Ej: "En proceso", "Facturado", etc.
    }
}
