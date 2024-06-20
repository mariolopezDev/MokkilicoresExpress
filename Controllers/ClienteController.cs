using Microsoft.AspNetCore.Mvc;
using MokkilicoresExpress.Models;

using Microsoft.Extensions.Caching.Memory;

namespace MokkilicoresExpress.Controllers
{
    public class ClienteController : Controller
    {
        private readonly IMemoryCache _cache;
        private const string ClienteCacheKey = "Clientes";

        public ClienteController(IMemoryCache cache)
        {
            _cache = cache;
        }

        public IActionResult Index()
        {
            var clientes = _cache.GetOrCreate(ClienteCacheKey, entry => {
                return new List<Cliente>();
            });
            return View(clientes);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Cliente cliente)
        {
            var clientes = _cache.Get<List<Cliente>>(ClienteCacheKey);
            clientes.Add(cliente);
            _cache.Set(ClienteCacheKey, clientes);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(string id)
        {
            var clientes = _cache.Get<List<Cliente>>(ClienteCacheKey);
            var cliente = clientes.FirstOrDefault(c => c.Identificacion == id);
            if (cliente == null)
            {
                return NotFound();
            }
            return View(cliente);
        }
        [HttpPost]
        public IActionResult Edit(Cliente cliente)
        {
            var clientes = _cache.Get<List<Cliente>>(ClienteCacheKey);
            var existingCliente = clientes.FirstOrDefault(c => c.Identificacion == cliente.Identificacion);
            if (existingCliente != null)
            {
                // Actualizar los datos del cliente
                existingCliente.NombreCompleto = cliente.NombreCompleto;
                existingCliente.Provincia = cliente.Provincia;
                existingCliente.Canton = cliente.Canton;
                existingCliente.Distrito = cliente.Distrito;
                existingCliente.DineroCompradoTotal = cliente.DineroCompradoTotal;
                existingCliente.DineroCompradoUltimoAnio = cliente.DineroCompradoUltimoAnio;
                existingCliente.DineroCompradoUltimosSeisMeses = cliente.DineroCompradoUltimosSeisMeses;

                // Actualizar la caché
                _cache.Set(ClienteCacheKey, clientes, new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(60)));

                // Redirigir a la vista de índice
                return RedirectToAction(nameof(Index));
            }

            // Si no se encuentra el cliente, volver a la vista de edición con un mensaje de error
            ModelState.AddModelError("", "Cliente no encontrado");
            return View(cliente);
        }

        public IActionResult Search(string searchTerm)
        {
            var clientes = _cache.Get<List<Cliente>>(ClienteCacheKey);
            var filteredClientes = string.IsNullOrWhiteSpace(searchTerm) 
                ? clientes 
                : clientes.Where(c => c.NombreCompleto.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) 
                                    || c.Identificacion.Contains(searchTerm)).ToList();

            if (HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(filteredClientes);
            }

            return View("Index", filteredClientes);
        }
        public IActionResult Delete(string id)
        {
            var clientes = _cache.Get<List<Cliente>>(ClienteCacheKey);
            var cliente = clientes.FirstOrDefault(c => c.Identificacion == id);
            if (cliente != null)
            {
                // Eliminar el cliente de la lista
                clientes.Remove(cliente);

                // Actualizar la caché
                _cache.Set(ClienteCacheKey, clientes, new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(60)));

                // Redirigir a la vista de índice
                return RedirectToAction(nameof(Index));
            }

            // Si no se encuentra el cliente, volver a la vista de índice con un mensaje de error
            ModelState.AddModelError("", "Cliente no encontrado");
            return RedirectToAction(nameof(Index), new { error = "ClienteNotFound" });
        }
    }
}

