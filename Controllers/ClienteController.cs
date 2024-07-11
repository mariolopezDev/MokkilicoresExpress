using Microsoft.AspNetCore.Mvc;
using MokkilicoresExpress.Models;
using MokkilicoresExpress.Services;

namespace MokkilicoresExpress.Controllers
{
    public class ClienteController : Controller
    {
        private readonly ClienteService _clienteService;

        public ClienteController(ClienteService clienteService)
        {
            _clienteService = clienteService;
        }

        public IActionResult Index()
        {
            var clientes = _clienteService.GetAll();
            return View(clientes);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Cliente cliente)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _clienteService.Add(cliente);
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (System.Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
            return View(cliente);
        }

        public IActionResult Details(string id)
        {
            var cliente = _clienteService.GetById(id);
            if (cliente == null)
            {
                return NotFound();
            }
            return View(cliente);
        }

        public IActionResult Edit(string id)
        {
            var cliente = _clienteService.GetById(id);
            if (cliente == null)
            {
                return NotFound();
            }
            return View(cliente);
        }

        [HttpPost]
        public IActionResult Edit(Cliente cliente)
        {
            if (ModelState.IsValid)
            {
                bool updated = _clienteService.Update(cliente);
                if (updated)
                {
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError("", "No se pudo actualizar el cliente");
            }
            return View(cliente);
        }

        public IActionResult Delete(string id)
        {
            bool deleted = _clienteService.Delete(id);
            if (!deleted)
            {
                return NotFound();
            }
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Search(string searchTerm)
        {
            var filteredClientes = _clienteService.Search(searchTerm);

            // Verificar si la solicitud es un AJAX request para devolver JSON
            if (HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(filteredClientes);
            }

            // Devolver la vista 'Index' con los clientes filtrados
            return View("Index", filteredClientes);
        }

    }
}
