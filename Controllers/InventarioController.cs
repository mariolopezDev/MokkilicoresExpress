using Microsoft.AspNetCore.Mvc;
using MokkilicoresExpress.Models;
using System.Collections.Generic;

namespace MokkilicoresExpress.Controllers
{
    public class InventarioController : Controller
    {
        private readonly List<Inventario> _inventario;

        public InventarioController(List<Inventario> inventario)
        {
            _inventario = inventario;
        }

        // Muestra la lista de inventario
        public IActionResult Index()
        {
            return View(_inventario);
        }

        // Retorna la vista para crear un nuevo artículo de inventario
        public IActionResult Create()
        {
            return View();
        }

        // Procesa la creación de un nuevo artículo de inventario
        [HttpPost]
        public IActionResult Create(Inventario inventario)
        {
            _inventario.Add(inventario);
            return RedirectToAction(nameof(Index));
        }

        // Retorna la vista para editar un artículo de inventario existente
        public IActionResult Edit(int id)
        {
            var item = _inventario.Find(x => x.Id == id);
            if (item == null)
            {
                return NotFound();
            }
            return View(item);
        }

        // Procesa la actualización de un artículo de inventario
        [HttpPost]
        public IActionResult Edit(Inventario inventario)
        {
            var item = _inventario.Find(x => x.Id == inventario.Id);
            if (item != null)
            {
                item.CantidadEnExistencia = inventario.CantidadEnExistencia;
                item.BodegaId = inventario.BodegaId;
                item.FechaIngreso = inventario.FechaIngreso;
                item.FechaVencimiento = inventario.FechaVencimiento;
                item.TipoLicor = inventario.TipoLicor;
                return RedirectToAction(nameof(Index));
            }
            return NotFound();
        }

        // Procesa la eliminación de un artículo de inventario
        public IActionResult Delete(int id)
        {
            var item = _inventario.Find(x => x.Id == id);
            if (item != null)
            {
                _inventario.Remove(item);
                return RedirectToAction(nameof(Index));
            }
            return NotFound();
        }
    }
}
