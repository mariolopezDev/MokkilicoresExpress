namespace MokkilicoresExpress.Models.ViewModels
{
    public class CreatePedidoViewModel
    {
        public Pedido Pedido { get; set; } = new Pedido();
        public List<Cliente> Clientes { get; set; } = new List<Cliente>();
        public List<Inventario> Inventarios { get; set; } = new List<Inventario>();
    }
}
