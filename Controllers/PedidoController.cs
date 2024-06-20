using Microsoft.AspNetCore.Mvc;
using MokkilicoresExpress.Models;

using Microsoft.Extensions.Caching.Memory;

namespace MokkilicoresExpress.Controllers
{
    public class PedidoController : Controller
    {
        private readonly IMemoryCache _cache;
        private const string PedidoCacheKey = "Pedidos";

        public PedidoController(IMemoryCache cache)
        {
            _cache = cache;
        }

        public IActionResult Index()
        {
            var pedidos = _cache.GetOrCreate(PedidoCacheKey, entry => {
                return new List<Pedido>();
            });
            return View(pedidos);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Pedido pedido)
        {
            var pedidos = _cache.Get<List<Pedido>>(PedidoCacheKey);
            pedidos.Add(pedido);
            _cache.Set(PedidoCacheKey, pedidos);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            var pedidos = _cache.Get<List<Pedido>>(PedidoCacheKey);
            var pedido = pedidos.FirstOrDefault(p => p.Id == id);
            if (pedido == null)
            {
                return NotFound();
            }
            return View(pedido);
        }

        [HttpPost]
        public IActionResult Edit(Pedido pedido)
        {
            var pedidos = _cache.Get<List<Pedido>>(PedidoCacheKey);
            var existingPedido = pedidos.FirstOrDefault(p => p.Id == pedido.Id);
            if (existingPedido != null)
            {
                existingPedido.ProductoId = pedido.ProductoId;
                existingPedido.Cantidad = pedido.Cantidad;
                existingPedido.CostoSinIVA = pedido.CostoSinIVA;
                existingPedido.Estado = pedido.Estado;
                _cache.Set(PedidoCacheKey, pedidos);
                return RedirectToAction(nameof(Index));
            }
            return NotFound();
        }

        public IActionResult Delete(int id)
        {
            var pedidos = _cache.Get<List<Pedido>>(PedidoCacheKey);
            var pedido = pedidos.FirstOrDefault(p => p.Id == id);
            if (pedido != null)
            {
                pedidos.Remove(pedido);
                _cache.Set(PedidoCacheKey, pedidos);
                return RedirectToAction(nameof(Index));
            }
            return NotFound();
        }
        public IActionResult Search(string searchTerm)
        {
            var pedidos = _cache.Get<List<Pedido>>(PedidoCacheKey);
            var filteredPedidos = string.IsNullOrWhiteSpace(searchTerm) 
                ? pedidos 
                : pedidos.Where(p => p.ProductoId.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)).ToList();

            if (HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(filteredPedidos);
            }

            return View("Index", filteredPedidos);
        }
    }
}

