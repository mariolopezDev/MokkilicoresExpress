namespace MokkilicoresExpress.Models.ViewModels
{
    public class EditPedidoViewModel
    {
        public Pedido Pedido { get; set; }
        public Cliente Cliente { get; set; }
        public List<Inventario> Inventarios { get; set; }
        public List<Direccion> Direcciones { get; set; }
    }
}
