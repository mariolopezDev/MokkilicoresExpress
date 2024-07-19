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
                var clientes = await GetClientesFromApiAsync();

                Console.WriteLine($"Clientes en caché: {clientes?.Count}"); // Log para debug
                //console each cliente en cache
                foreach (var clientesss in clientes)
                {
                    Console.WriteLine($"Cliente en caché: {clientesss.Id} - {clientesss.Identificacion}"); // Log para debug
                }
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

        private async Task<List<Cliente>> GetClientesFromApiAsync()
        {
            List<Cliente> clientes;

            try
            {
                clientes = await _httpClient.GetFromJsonAsync<List<Cliente>>("/api/Cliente");
                
                // Actualiza el cache con los nuevos datos
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromSeconds(300)); // Cache por 30 segundos
                _cache.Set(ClienteCacheKey, clientes, cacheEntryOptions);
                Console.WriteLine("Datos de clientes obtenidos del API y actualizados en cache");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener clientes del API: {ex.Message}");
                // Si hay un error al obtener del API, intenta usar los datos en cache si existen
                if (!_cache.TryGetValue(ClienteCacheKey, out clientes))
                {
                    clientes = new List<Cliente>(); // Si no hay datos en cache, usa una lista vacía
                    Console.WriteLine("No se pudieron obtener datos del API ni del cache. Usando lista vacía.");
                }
                else
                {
                    Console.WriteLine("Usando datos de clientes del cache debido a error en el API");
                }
            }

            return clientes;
        }

        public IActionResult Create()
        {
            var clienteId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            Console.WriteLine($"Antes de crear la dirección: {clienteId}");
            Console.WriteLine($"Antes de crear la dirección: {clienteId}");

            if (string.IsNullOrEmpty(clienteId))
            {
                return Unauthorized();
            }
            Console.WriteLine($"Antes de crear la dirección_: {clienteId}");
            Console.WriteLine($"Antes de crear la dirección_: {clienteId}");
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
                var clientes = await GetClientesFromApiAsync();
                var cliente = clientes?.FirstOrDefault(c => c.Identificacion == userIdentificacion);

                Console.WriteLine($"userIdentificacion: {userIdentificacion}");
            Console.WriteLine($"cliente: {cliente?.Id} - {cliente?.Identificacion}");
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
            Console.WriteLine($"Editando dirección para usuario: {userIdentificacion}");

            try
            {
                // Obtener todas las direcciones del usuario
                var direcciones = await _httpClient.GetFromJsonAsync<List<Direccion>>($"/api/Direccion/Usuario/{userIdentificacion}");
                
                // Buscar la dirección específica por id
                var direccion = direcciones.FirstOrDefault(d => d.Id == id);
                
                if (direccion == null)
                {
                    return NotFound();
                }
                var clientes = await GetClientesFromApiAsync();
                var cliente = clientes?.FirstOrDefault(c => c.Identificacion == userIdentificacion);

                if (cliente == null || (direccion.ClienteId != cliente.Id && !User.IsInRole("Admin")))
                {
                    return Unauthorized();
                }

                return View(direccion);
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error al obtener la dirección: {ex.Message}");
                // Puedes decidir qué hacer aquí: redirigir a una página de error, mostrar un mensaje, etc.
                return RedirectToAction("Error", "Home", new { message = "No se pudo obtener la dirección." });
            }
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
                var clientes = await GetClientesFromApiAsync();
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
            var clientes = await GetClientesFromApiAsync();
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
            var clientes = await GetClientesFromApiAsync();
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