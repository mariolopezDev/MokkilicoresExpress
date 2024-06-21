using Microsoft.AspNetCore.Mvc;
using MokkilicoresExpress.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Authorization;



namespace MokkilicoresExpress.Controllers
{
    public class PedidoController : Controller
    {
        private readonly IMemoryCache _cache;
        private const string PedidoCacheKey = "Pedidos";

        public PedidoController(IMemoryCache cache)
        {
            _cache = cache;

            if (!_cache.TryGetValue(PedidoCacheKey, out List<Pedido> _))
            {
                List<Pedido> initialPedidos = new List<Pedido>
                {
                    new Pedido { Id = 1, ProductoId = "P001", Cantidad = 10, CostoSinIVA = 1000, Estado = "Pendiente" },
                    new Pedido { Id = 2, ProductoId = "P002", Cantidad = 20, CostoSinIVA = 2000, Estado = "Entregado" }
                };
                _cache.Set(PedidoCacheKey, initialPedidos, new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(60)));
            }
        }

        public IActionResult Index()
        {
            var pedidos = _cache.GetOrCreate(PedidoCacheKey, entry => new List<Pedido>());
            return View(pedidos);
        }

        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [Authorize]
        public IActionResult Create(Pedido pedido)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (ModelState.IsValid)
            {
                var pedidos = _cache.Get<List<Pedido>>(PedidoCacheKey);
                pedidos.Add(pedido);
                _cache.Set(PedidoCacheKey, pedidos, new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(60)));
                return RedirectToAction(nameof(Index));
            }
            return View(pedido);
        }

        public IActionResult Details(int id)
        {
            var pedidos = _cache.Get<List<Pedido>>(PedidoCacheKey);
            var pedido = pedidos?.FirstOrDefault(p => p.Id == id);
            if (pedido == null)
            {
                return NotFound();
            }
            return View(pedido);
        }

        public IActionResult Edit(int id)
        {
            var pedidos = _cache.Get<List<Pedido>>(PedidoCacheKey);
            var pedido = pedidos?.FirstOrDefault(p => p.Id == id);
            if (pedido == null)
            {
                return NotFound();
            }
            return View(pedido);
        }

        [HttpPost]
        public IActionResult Edit(Pedido pedido)
        {
            if (ModelState.IsValid)
            {
                var pedidos = _cache.Get<List<Pedido>>(PedidoCacheKey);
                var existingPedido = pedidos?.FirstOrDefault(p => p.Id == pedido.Id);
                if (existingPedido != null)
                {
                    existingPedido.ProductoId = pedido.ProductoId;
                    existingPedido.Cantidad = pedido.Cantidad;
                    existingPedido.CostoSinIVA = pedido.CostoSinIVA;
                    existingPedido.Estado = pedido.Estado;
                    _cache.Set(PedidoCacheKey, pedidos, new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(60)));
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError("", "Pedido no encontrado");
            }
            return View(pedido);
        }

        public IActionResult Delete(int id)
        {
            var pedidos = _cache.Get<List<Pedido>>(PedidoCacheKey);
            var pedido = pedidos?.FirstOrDefault(p => p.Id == id);
            if (pedido != null)
            {
                pedidos.Remove(pedido);
                _cache.Set(PedidoCacheKey, pedidos, new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(60)));
                return RedirectToAction(nameof(Index));
            }
            ModelState.AddModelError("", "Pedido no encontrado");
            return RedirectToAction(nameof(Index), new { error = "PedidoNotFound" });
        }

        public IActionResult Search(string searchTerm)
        {
            var pedidos = _cache.Get<List<Pedido>>(PedidoCacheKey);
            var filteredPedidos = string.IsNullOrWhiteSpace(searchTerm)
                ? pedidos
                : pedidos.Where(p => p.ProductoId.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
                    || p.Estado.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)).ToList();

            if (HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(filteredPedidos);
            }

            return View("Index", filteredPedidos);
        }
    }
}
