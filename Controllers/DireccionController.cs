using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using MokkilicoresExpress.Models;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MokkilicoresExpress.Controllers
{
    public class DireccionController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _cache;
        private const string ClienteCacheKey = "Clientes";

        public DireccionController(IHttpClientFactory httpClientFactory, IMemoryCache cache)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
            _cache = cache;
        }

        public async Task<IActionResult> Index()
        {
            var direcciones = await _httpClient.GetFromJsonAsync<List<Direccion>>("/api/Direccion");
            return View(direcciones);
        }

        public IActionResult Create()
        {
            var clienteId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(clienteId))
            {
                return Unauthorized();
            }

            var direccion = new Direccion { ClienteId = int.Parse(clienteId) };
            return View(direccion);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Direccion direccion)
        {
            if (ModelState.IsValid)
            {
                var response = await _httpClient.PostAsJsonAsync("/api/Direccion", direccion);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError("", "Error al crear la dirección");
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

            var clienteId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(clienteId) || direccion.ClienteId != int.Parse(clienteId))
            {
                return Unauthorized();
            }

            return View(direccion);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Direccion direccion)
        {
            if (ModelState.IsValid)
            {
                var response = await _httpClient.PutAsJsonAsync($"/api/Direccion/{direccion.Id}", direccion);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError("", "Error al editar la dirección");
            }

            return View(direccion);
        }

        public async Task<IActionResult> Details(int id)
        {
            var direccion = await _httpClient.GetFromJsonAsync<Direccion>($"/api/Direccion/{id}");
            if (direccion == null)
            {
                return NotFound();
            }

            var clienteId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(clienteId) || direccion.ClienteId != int.Parse(clienteId))
            {
                return Unauthorized();
            }

            return View(direccion);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var direccion = await _httpClient.GetFromJsonAsync<Direccion>($"/api/Direccion/{id}");
            if (direccion == null)
            {
                return NotFound();
            }

            var clienteId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(clienteId) || direccion.ClienteId != int.Parse(clienteId))
            {
                return Unauthorized();
            }

            var response = await _httpClient.DeleteAsync($"/api/Direccion/{id}");
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", "Error al eliminar la dirección");
            return RedirectToAction(nameof(Index));
        }
    }
}
