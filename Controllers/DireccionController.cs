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
            
            var userIdentificacion = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Console.WriteLine($"User Identificacion: {userIdentificacion}"); // Log para debug

            if (!User.IsInRole("Admin"))
            {
                // Buscar el cliente correspondiente en la caché usando la identificación del usuario
                var clientes = await GetClientesFromCacheOrApiAsync();
                var cliente = clientes?.FirstOrDefault(c => c.Identificacion == userIdentificacion);
                Console.WriteLine($"Cliente encontrado: {cliente?.Id} - {cliente?.Identificacion}"); // Log para debug

                if (cliente != null)
                {
                    direcciones = direcciones?.Where(d => d.ClienteId == cliente.Id).ToList();
                    Console.WriteLine($"d.clienteId: {direcciones?.FirstOrDefault()?.ClienteId}"); // Log para debug
                    Console.WriteLine($"Direcciones filtradas: {direcciones?.Count}"); // Log para debug
                    Console.WriteLine($"Direcciones filtradas: {direcciones?.Count}"); // Log para debug
                }
                else
                {
                    direcciones = new List<Direccion>(); // No hay direcciones para mostrar si no se encuentra el cliente
                     Console.WriteLine("No se encontró el cliente"); // Log para debug
                }
            }

            return View(direcciones);
        }

        private async Task<List<Cliente>> GetClientesFromCacheOrApiAsync()
        {
            if (!_cache.TryGetValue(ClienteCacheKey, out List<Cliente> clientes))
            {
                clientes = await _httpClient.GetFromJsonAsync<List<Cliente>>("/api/Cliente");
                var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(5));
                _cache.Set(ClienteCacheKey, clientes, cacheEntryOptions);
            }
            return clientes;
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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Direccion direccion)
        {
            if (ModelState.IsValid)
            {
                var userIdentificacion = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var clientes = await GetClientesFromCacheOrApiAsync();
                var cliente = clientes?.FirstOrDefault(c => c.Identificacion == userIdentificacion);

                if (cliente == null)
                {
                    return Unauthorized();
                }

                direccion.ClienteId = cliente.Id;

                var response = await _httpClient.PostAsJsonAsync("/api/Direccion", direccion);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError("", "Error al crear la dirección");
            }

            return View(direccion);
        }
        [HttpGet("edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            

            var userIdentificacion = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var direccion = await _httpClient.GetFromJsonAsync<Direccion>($"/api/Direccion/{userIdentificacion}");
            
            if (direccion == null)
            {
                return NotFound();
            }

            var clientes = await GetClientesFromCacheOrApiAsync();
            var cliente = clientes?.FirstOrDefault(c => c.Identificacion == userIdentificacion);

            if (cliente == null || (direccion.ClienteId != cliente.Id && !User.IsInRole("Admin")))
            {
                return Unauthorized();
            }

            return View(direccion);
        }

        [HttpPost("edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Direccion direccion)
        {
            if (id != direccion.Id)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                var userIdentificacion = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var clientes = await GetClientesFromCacheOrApiAsync();
                var cliente = clientes?.FirstOrDefault(c => c.Identificacion == userIdentificacion);

                if (cliente == null || (direccion.ClienteId != cliente.Id && !User.IsInRole("Admin")))
                {
                    return Unauthorized();
                }

                var response = await _httpClient.PutAsJsonAsync($"/api/Direccion/{id}", direccion);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError("", "Error al editar la dirección");
            }

            return View(direccion);
        }

        [HttpGet("details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var direccion = await _httpClient.GetFromJsonAsync<Direccion>($"/api/Direccion/{id}");
            if (direccion == null)
            {
                return NotFound();
            }

            var userIdentificacion = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var clientes = await GetClientesFromCacheOrApiAsync();
            var cliente = clientes?.FirstOrDefault(c => c.Identificacion == userIdentificacion);

            if (cliente == null || (direccion.ClienteId != cliente.Id && !User.IsInRole("Admin")))
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

            var userIdentificacion = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var clientes = await GetClientesFromCacheOrApiAsync();
            var cliente = clientes?.FirstOrDefault(c => c.Identificacion == userIdentificacion);

            if (cliente == null || (direccion.ClienteId != cliente.Id && !User.IsInRole("Admin")))
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