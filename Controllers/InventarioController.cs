using Microsoft.AspNetCore.Mvc;
using MokkilicoresExpress.Models;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace MokkilicoresExpress.Controllers
{
    public class InventarioController : Controller
    {
        private readonly HttpClient _httpClient;

        public InventarioController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
        }

        public async Task<IActionResult> Index()
        {
            var inventarios = await _httpClient.GetFromJsonAsync<List<Inventario>>("/api/Inventario");
            return View(inventarios);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Inventario inventario)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/Inventario", inventario);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }
            ModelState.AddModelError("", "Error al crear inventario");
            return View(inventario);
        }

        public async Task<IActionResult> Details(int id)
        {
            var inventario = await _httpClient.GetFromJsonAsync<Inventario>($"/api/Inventario/{id}");
            if (inventario == null)
            {
                return NotFound();
            }
            return View(inventario);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var inventario = await _httpClient.GetFromJsonAsync<Inventario>($"/api/Inventario/{id}");
            if (inventario == null)
            {
                return NotFound();
            }
            return View(inventario);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Inventario inventario)
        {
            var response = await _httpClient.PutAsJsonAsync($"/api/Inventario/{inventario.Id}", inventario);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }
            ModelState.AddModelError("", "Error al editar inventario");
            return View(inventario);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var response = await _httpClient.DeleteAsync($"/api/Inventario/{id}");
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }
            ModelState.AddModelError("", "Error al eliminar inventario");
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Search(string searchTerm)
        {
            var inventarios = await _httpClient.GetFromJsonAsync<List<Inventario>>("/api/Inventario");

            var filteredInventarios = string.IsNullOrWhiteSpace(searchTerm) ? 
                inventarios : 
                inventarios.Where(i => i.TipoLicor.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)).ToList();

            if (HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(filteredInventarios);
            }

            return View("Index", filteredInventarios);
        }
    }
}
