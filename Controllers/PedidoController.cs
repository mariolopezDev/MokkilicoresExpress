using Microsoft.AspNetCore.Mvc;
using MokkilicoresExpress.Models;
using MokkilicoresExpress.Services;
using Microsoft.AspNetCore.Authorization;

namespace MokkilicoresExpress.Controllers
{
    public class PedidoController : Controller
    {
        private readonly PedidoService _pedidoService;

        public PedidoController(PedidoService pedidoService)
        {
            _pedidoService = pedidoService;
        }

        public IActionResult Index()
        {
            var pedidos = _pedidoService.GetAll();
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
            if (ModelState.IsValid)
            {
                _pedidoService.Add(pedido);
                return RedirectToAction(nameof(Index));
            }
            return View(pedido);
        }

        public IActionResult Details(int id)
        {
            var pedido = _pedidoService.GetById(id);
            if (pedido == null)
            {
                return NotFound();
            }
            return View(pedido);
        }

        public IActionResult Edit(int id)
        {
            var pedido = _pedidoService.GetById(id);
            if (pedido == null)
            {
                return NotFound();
            }
            return View(pedido);
        }

        [HttpPost]
        public IActionResult Edit(Pedido pedido)
        {
            if (ModelState.IsValid && _pedidoService.Update(pedido))
            {
                return RedirectToAction(nameof(Index));
            }
            ModelState.AddModelError("", "Pedido no encontrado");
            return View(pedido);
        }

        public IActionResult Delete(int id)
        {
            if (_pedidoService.Delete(id))
            {
                return RedirectToAction(nameof(Index));
            }
            ModelState.AddModelError("", "Pedido no encontrado");
            return RedirectToAction(nameof(Index), new { error = "PedidoNotFound" });
        }

        public IActionResult Search(string searchTerm)
        {
            var filteredPedidos = _pedidoService.Search(searchTerm);
            if (HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(filteredPedidos);
            }
            return View("Index", filteredPedidos);
        }
    }
}
