using Microsoft.AspNetCore.Mvc;
using MokkilicoresExpress.Models;
using Microsoft.AspNetCore.Authorization;

using Microsoft.Extensions.Caching.Memory;


namespace MokkilicoresExpress.Controllers
{
    public class InventarioController : Controller
    {
        private readonly IMemoryCache _cache;
        private const string InventarioCacheKey = "Inventario";

        public InventarioController(IMemoryCache cache)
        {
            _cache = cache;

            // Inicializar el inventario en la caché si aún no está presente
            if (!_cache.TryGetValue(InventarioCacheKey, out List<Inventario> _))
            {
                List<Inventario> initialInventario = new List<Inventario>
                {
                    new Inventario { Id = 1, CantidadEnExistencia = 100, BodegaId = 1, FechaIngreso = DateTime.Now, FechaVencimiento = DateTime.Now.AddYears(1), TipoLicor = "Vodka" },
                    new Inventario { Id = 2, CantidadEnExistencia = 150, BodegaId = 2, FechaIngreso = DateTime.Now, FechaVencimiento = DateTime.Now.AddYears(1), TipoLicor = "Whiskey" }
                };
                _cache.Set(InventarioCacheKey, initialInventario, new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(60)));
            }
        }

        public IActionResult Index()
        {
            var inventario = _cache.Get<List<Inventario>>(InventarioCacheKey);
            return View(inventario);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
       
        public IActionResult Create(Inventario inventario)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (ModelState.IsValid)
            {
                var inventarioList = _cache.Get<List<Inventario>>(InventarioCacheKey);
                inventarioList.Add(inventario);
                _cache.Set(InventarioCacheKey, inventarioList, new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(60)));
                return RedirectToAction(nameof(Index));
            }
            return View(inventario);
        }

        public IActionResult Details(int id)
        {
            var inventario = _cache.Get<List<Inventario>>(InventarioCacheKey);
            var item = inventario.FirstOrDefault(x => x.Id == id);
            if (item == null)
            {
                return NotFound();
            }
            return View(item);
        }

        public IActionResult Edit(int id)
        {
            var inventario = _cache.Get<List<Inventario>>(InventarioCacheKey);
            var item = inventario.FirstOrDefault(x => x.Id == id);
            if (item == null)
            {
                return NotFound();
            }
            return View(item);
        }

        [HttpPost]
        public IActionResult Edit(Inventario inventario)
        {
            var inventarioList = _cache.Get<List<Inventario>>(InventarioCacheKey);
            var index = inventarioList.FindIndex(i => i.Id == inventario.Id);
            if (index != -1)
            {
                inventarioList[index] = inventario; // Actualiza el elemento en la lista
                _cache.Set(InventarioCacheKey, inventarioList, new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(60)));
                return RedirectToAction(nameof(Index));
            }
            return NotFound();
        }

        public IActionResult Delete(int id)
        {
            var inventarioList = _cache.Get<List<Inventario>>(InventarioCacheKey);
            var itemIndex = inventarioList.FindIndex(i => i.Id == id);
            if (itemIndex != -1)
            {
                inventarioList.RemoveAt(itemIndex);
                _cache.Set(InventarioCacheKey, inventarioList, new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(60)));
                return RedirectToAction(nameof(Index));
            }
            return NotFound();
        }

        // Procesa la búsqueda de un artículo de inventario
        public IActionResult Search(string searchTerm)
        {
            // Obtener la lista de inventario desde la caché
            var inventarioList = _cache.Get<List<Inventario>>(InventarioCacheKey);

            // Filtrar los elementos basados en el término de búsqueda, ignorando mayúsculas y minúsculas
            var filteredItems = string.IsNullOrWhiteSpace(searchTerm)
                ? inventarioList
                : inventarioList.Where(i => i.TipoLicor.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)).ToList();

            // Verificar si la solicitud es un AJAX request para devolver JSON
            if (HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(filteredItems);
            }

            // Devolver la vista 'Index' con los elementos filtrados
            return View("Index", filteredItems);
        }

    }
}
