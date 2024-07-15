using Microsoft.AspNetCore.Mvc;
using MokkilicoresExpress.Models;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace MokkilicoresExpress.Controllers
{
    public class DireccionController : Controller
    {
        private readonly HttpClient _httpClient;

        public DireccionController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
        }

        public async Task<IActionResult> Index()
        {
            var direcciones = await _httpClient.GetFromJsonAsync<List<Direccion>>("/api/Direccion");
            return View(direcciones);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Direccion direccion)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/Direccion", direccion);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }
            ModelState.AddModelError("", "Error al crear dirección");
            return View(direccion);
        }

        public async Task<IActionResult> Details(int id)
        {
            var direccion = await _httpClient.GetFromJsonAsync<Direccion>($"/api/Direccion/{id}");
            if (direccion == null)
            {
                return NotFound();
            }
            return View(direccion);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var direccion = await _httpClient.GetFromJsonAsync<Direccion>($"/api/Direccion/{id}");
            if (direccion == null)
            {
                return NotFound();
            }
            return View(direccion);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Direccion direccion)
        {
            var response = await _httpClient.PutAsJsonAsync($"/api/Direccion/{direccion.Id}", direccion);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }
            ModelState.AddModelError("", "Error al editar dirección");
            return View(direccion);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var response = await _httpClient.DeleteAsync($"/api/Direccion/{id}");
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }
            ModelState.AddModelError("", "Error al eliminar dirección");
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Search(string searchTerm)
        {
            var direcciones = await _httpClient.GetFromJsonAsync<List<Direccion>>("/api/Direccion");

            var filteredDirecciones = string.IsNullOrWhiteSpace(searchTerm) ? 
                direcciones : 
                direcciones.Where(d => d.Provincia.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                                       d.Canton.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                                       d.Distrito.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)).ToList();

            if (HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(filteredDirecciones);
            }

            return View("Index", filteredDirecciones);
        }
    }
}
