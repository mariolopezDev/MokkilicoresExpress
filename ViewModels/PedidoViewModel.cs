namespace MokkilicoresExpress.Models
{
    public class PedidoViewModel
    {
        public int Id { get; set; }
        public int ProductoId { get; set; }
        public string NombreProducto { get; set; }  // Nombre descriptivo del producto
        public string TipoLicor { get; set; }  // Tipo de licor
        public decimal PrecioUnitario { get; set; }  // Precio por unidad
        public int Cantidad { get; set; }
        public decimal CostoSinIVA { get; set; }
        public decimal CostoTotal { get { return CostoSinIVA * 1.13M; } }
        public string Estado { get; set; }
        public int ClienteId { get; set; }
        public string NombreCliente { get; set; }  // Nombre del cliente asociado al pedido
        public DateTime FechaPedido { get; set; }
    }
}
