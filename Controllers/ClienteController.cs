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
            try
            {
                var response = await _httpClient.GetAsync("/api/Cliente");
                if (response.IsSuccessStatusCode)
                {
                    var clientes = await response.Content.ReadFromJsonAsync<List<Cliente>>();
                    return View(clientes);
                }
                else
                {
                    TempData["ErrorMensaje"] = "Error al obtener clientes: {response.StatusCode}";
                    return View(new List<Cliente>());
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMensaje"] = "Error al obtener clientes: {ex.Message}";
                return View(new List<Cliente>());
            }
        }

        public IActionResult Create()
        {
            return View();
        }
        
        [HttpPost]
        public async Task<IActionResult> Create(Cliente cliente)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("/api/Cliente", cliente);
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<Cliente>();
                    TempData["SuccessMessage"] = "Cliente creado exitosamente";
                    return RedirectToAction(nameof(Index));
                }
                var errorResult = await response.Content.ReadFromJsonAsync<Cliente>();
                ModelState.AddModelError("", $"Error al crear cliente: {errorResult.Nombre}");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Solo administradores pueden crear clientes");
            }
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


            TempData["SuccessMessage"] = "Error al editar, solo el usuario dueño de la cuenta o el admin puede editar";
                    return RedirectToAction(nameof(Index));
            ModelState.AddModelError("", "Error al editar cliente");
            //return View(cliente);
        }

        public async Task<IActionResult> Delete(int id)
                {
                    try
                    {
                        var response = await _httpClient.DeleteAsync($"/api/Cliente/{id}");
                        if (response.IsSuccessStatusCode)
                        {
                            TempData["SuccessMessage"] = "Cliente eliminado exitosamente";
                        }
                        else
                        {
                            var errorResult = await response.Content.ReadFromJsonAsync<Cliente>();
                            TempData["ErrorMessage"] = "Error al eliminar cliente, solo el admin puede eliminar";
                        }
                    }
                    catch (Exception ex)
                    {
                        TempData["ErrorMessage"] = $"Error al eliminar cliente: {ex.Message}";
                    }
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
