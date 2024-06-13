using Microsoft.AspNetCore.Mvc;
using MokkilicoresExpress.Models;
using System.Collections.Generic;

namespace MokkilicoresExpress.Controllers
{
    public class ClienteController : Controller
    {
        private readonly List<Cliente> _clientes;

        public ClienteController(List<Cliente> clientes)
        {
            _clientes = clientes;
        }

        // Muestra la lista de clientes
        public IActionResult Index()
        {
            return View(_clientes);
        }

        // Retorna la vista para crear un nuevo cliente
        public IActionResult Create()
        {
            return View();
        }

        // Procesa la creación de un nuevo cliente
        [HttpPost]
        public IActionResult Create(Cliente cliente)
        {
            _clientes.Add(cliente);
            return RedirectToAction(nameof(Index));
        }

        // Retorna la vista para editar un cliente existente
        public IActionResult Edit(string id)
        {
            var cliente = _clientes.Find(c => c.Identificacion == id);
            if (cliente == null)
            {
                return NotFound();
            }
            return View(cliente);
        }

        // Procesa la actualización de un cliente
        [HttpPost]
        public IActionResult Edit(Cliente cliente)
        {
            var item = _clientes.Find(c => c.Identificacion == cliente.Identificacion);
            if (item != null)
            {
                item.NombreCompleto = cliente.NombreCompleto;
                item.Provincia = cliente.Provincia;
                item.Canton = cliente.Canton;
                item.Distrito = cliente.Distrito;
                item.DineroCompradoTotal = cliente.DineroCompradoTotal;
                item.DineroCompradoUltimoAnio = cliente.DineroCompradoUltimoAnio;
                item.DineroCompradoUltimosSeisMeses = cliente.DineroCompradoUltimosSeisMeses;
                return RedirectToAction(nameof(Index));
            }
            return NotFound();
        }

        // Procesa la eliminación de un cliente
        public IActionResult Delete(string id)
        {
            var cliente = _clientes.Find(c => c.Identificacion == id);
            if (cliente != null)
            {
                _clientes.Remove(cliente);
                return RedirectToAction(nameof(Index));
            }
            return NotFound();
        }
    }
}
