using Microsoft.AspNetCore.Mvc;
using MokkilicoresExpress.Models;
using System.Collections.Generic;

namespace MokkilicoresExpress.Controllers
{
    public class PedidoController : Controller
    {
        private readonly List<Pedido> _pedidos;

        public PedidoController(List<Pedido> pedidos)
        {
            _pedidos = pedidos;
        }

        // Muestra la lista de pedidos
        public IActionResult Index()
        {
            return View(_pedidos);
        }

        // Retorna la vista para crear un nuevo pedido
        public IActionResult Create()
        {
            return View();
        }

        // Procesa la creación de un nuevo pedido
        [HttpPost]
        public IActionResult Create(Pedido pedido)
        {
            _pedidos.Add(pedido);
            return RedirectToAction(nameof(Index));
        }

        // Retorna la vista para editar un pedido existente
        public IActionResult Edit(int id)
        {
            var pedido = _pedidos.Find(p => p.Id == id);
            if (pedido == null)
            {
                return NotFound();
            }
            return View(pedido);
        }

        // Procesa la actualización de un pedido
        [HttpPost]
        public IActionResult Edit(Pedido pedido)
        {
            var item = _pedidos.Find(p => p.Id == pedido.Id);
            if (item != null)
            {
                item.ProductoId = pedido.ProductoId;
                item.Cantidad = pedido.Cantidad;
                item.CostoSinIVA = pedido.CostoSinIVA;
                item.Estado = pedido.Estado;
                return RedirectToAction(nameof(Index));
            }
            return NotFound();
        }

        // Procesa la eliminación de un pedido
        public IActionResult Delete(int id)
        {
            var pedido = _pedidos.Find(p => p.Id == id);
            if (pedido != null)
            {
                _pedidos.Remove(pedido);
                return RedirectToAction(nameof(Index));
            }
            return NotFound();
        }
    }
}
