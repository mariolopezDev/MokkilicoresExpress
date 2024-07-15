using Microsoft.AspNetCore.Mvc;
using MokkilicoresExpress.Models;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace MokkilicoresExpress.Controllers
{
    public class ClienteController : Controller
    {
        private readonly HttpClient _httpClient;

        public ClienteController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
        }

        public async Task<IActionResult> Index()
        {
            var clientes = await _httpClient.GetFromJsonAsync<List<Cliente>>("/api/Cliente");
            return View(clientes);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Cliente cliente)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/Cliente", cliente);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }
            ModelState.AddModelError("", "Error al crear cliente");
            return View(cliente);
        }


        public async Task<IActionResult> Details(int id)
        {
            var cliente = await _httpClient.GetFromJsonAsync<Cliente>($"/api/Cliente/{id}");
            if (cliente == null)
            {
                return NotFound();
            }
            return View(cliente);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var cliente = await _httpClient.GetFromJsonAsync<Cliente>($"/api/Cliente/{id}");
            if (cliente == null)
            {
                return NotFound();
            }
            return View(cliente);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Cliente cliente)
        {
            var response = await _httpClient.PutAsJsonAsync($"/api/Cliente/{cliente.Id}", cliente);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }
            ModelState.AddModelError("", "Error al editar cliente");
            return View(cliente);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var response = await _httpClient.DeleteAsync($"/api/Cliente/{id}");
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }
            ModelState.AddModelError("", "Error al eliminar cliente");
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Search(string searchTerm)
        {
            var clientes = await _httpClient.GetFromJsonAsync<List<Cliente>>("/api/Cliente");

            // Realizar la búsqueda si searchTerm no está vacío
            var filteredClientes = string.IsNullOrWhiteSpace(searchTerm) ? 
                clientes : 
                clientes.Where(c => c.Nombre.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                                    c.Apellido.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                                    c.Identificacion.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)).ToList();

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
