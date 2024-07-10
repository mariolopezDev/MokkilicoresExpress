using Microsoft.AspNetCore.Mvc;
using MokkilicoresExpress.Models;
using Microsoft.AspNetCore.Authorization;
using MokkilicoresExpress.Services;

using Microsoft.Extensions.Caching.Memory;


namespace MokkilicoresExpress.Controllers
{
    public class InventarioController : Controller
    {
        private readonly InventarioService _inventarioService;

        public InventarioController(InventarioService inventarioService)
        {
            _inventarioService = inventarioService;
        }

        public IActionResult Index()
        {
            var inventario = _inventarioService.GetAll();
            return View(inventario);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        //[Authorize]
        public IActionResult Create(Inventario inventario)
        {
            if (ModelState.IsValid)
            {
                _inventarioService.Add(inventario);
                return RedirectToAction(nameof(Index));
            }
            return View(inventario);
        }
       public IActionResult Details(int id)
        {
            var item = _inventarioService.GetById(id);
            if (item == null)
            {
                return NotFound();
            }
            return View(item);
        }

        public IActionResult Edit(int id)
        {
            var item = _inventarioService.GetById(id);
            if (item == null)
            {
                return NotFound();
            }
            return View(item);
        }

        [HttpPost]
        public IActionResult Edit(Inventario inventario)
        {
            if (ModelState.IsValid)
            {
                bool updated = _inventarioService.Update(inventario.Id, inventario);
                if (updated)
                {
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError("", "No se pudo actualizar el inventario");
            }
            return View(inventario);
        }

        public IActionResult Delete(int id)
        {
            bool deleted = _inventarioService.Delete(id);
            if (!deleted)
            {
                return NotFound();
            }
            return RedirectToAction(nameof(Index));
        }

        // Procesa la búsqueda de un artículo de inventario
        public IActionResult Search(string searchTerm)
        {
            // Usar el servicio para obtener los elementos filtrados
            var filteredItems = _inventarioService.SearchInventory(searchTerm);

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
