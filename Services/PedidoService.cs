using MokkilicoresExpress.Models;

namespace MokkilicoresExpress.Services
{
    public class PedidoService
    {
        private List<Pedido> _pedidos;

        public PedidoService()
        {
            _pedidos = new List<Pedido>
            {
                new Pedido { Id = 1, ProductoId = "P001", Cantidad = 10, CostoSinIVA = 1000, Estado = "Pendiente" },
                new Pedido { Id = 2, ProductoId = "P002", Cantidad = 20, CostoSinIVA = 2000, Estado = "Entregado" }
            };
        }

        public List<Pedido> GetAll()
        {
            return _pedidos;
        }

        public Pedido GetById(int id)
        {
            return _pedidos.FirstOrDefault(p => p.Id == id);
        }

        public void Add(Pedido pedido)
        {
            pedido.Id = _pedidos.Max(p => p.Id) + 1;
            _pedidos.Add(pedido);
        }

        public bool Update(Pedido pedido)
        {
            var existingPedido = _pedidos.FirstOrDefault(p => p.Id == pedido.Id);
            if (existingPedido != null)
            {
                existingPedido.ProductoId = pedido.ProductoId;
                existingPedido.Cantidad = pedido.Cantidad;
                existingPedido.CostoSinIVA = pedido.CostoSinIVA;
                existingPedido.Estado = pedido.Estado;
                return true;
            }
            return false;
        }

        public bool Delete(int id)
        {
            var pedido = _pedidos.FirstOrDefault(p => p.Id == id);
            if (pedido != null)
            {
                _pedidos.Remove(pedido);
                return true;
            }
            return false;
        }

        public List<Pedido> Search(string searchTerm)
        {
            return string.IsNullOrWhiteSpace(searchTerm) ? 
                   _pedidos : 
                   _pedidos.Where(p => p.ProductoId.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
                                   || p.Estado.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)).ToList();
        }
    }
}
