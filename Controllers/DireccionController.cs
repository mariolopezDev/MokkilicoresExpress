using Microsoft.AspNetCore.Mvc;
using MokkilicoresExpress.Models;
using MokkilicoresExpress.Services;

namespace MokkilicoresExpress.Controllers
{
    public class DireccionController : Controller
    {
        private readonly DireccionService _direccionService;

        public DireccionController(DireccionService direccionService)
        {
            _direccionService = direccionService;
        }

        public IActionResult Index()
        {
            var direcciones = _direccionService.GetAll();
            return View(direcciones);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Direccion direccion)
        {
            if (ModelState.IsValid)
            {
                _direccionService.Add(direccion);
                return RedirectToAction(nameof(Index));
            }
            return View(direccion);
        }

        public IActionResult Details(int id)
        {
            var direccion = _direccionService.GetById(id);
            if (direccion == null)
            {
                return NotFound();
            }
            return View(direccion);
        }

        public IActionResult Edit(int id)
        {
            var direccion = _direccionService.GetById(id);
            if (direccion == null)
            {
                return NotFound();
            }
            return View(direccion);
        }

        [HttpPost]
        public IActionResult Edit(Direccion direccion)
        {
            if (ModelState.IsValid && _direccionService.Update(direccion))
            {
                return RedirectToAction(nameof(Index));
            }
            return View(direccion);
        }

        public IActionResult Delete(int id)
        {
            if (_direccionService.Delete(id))
            {
                return RedirectToAction(nameof(Index));
            }
            return NotFound();
        }

        public IActionResult Search(string searchTerm)
        {
            var filteredDirecciones = _direccionService.Search(searchTerm);
            return View("Index", filteredDirecciones);
        }
    }
}
