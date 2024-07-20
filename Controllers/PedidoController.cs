using Microsoft.AspNetCore.Mvc;
using MokkilicoresExpress.Models;
using MokkilicoresExpress.Models.ViewModels;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Linq;
using System.Security.Claims;

namespace MokkilicoresExpress.Controllers
{
    public class PedidoController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<PedidoController> _logger;

        public PedidoController(IHttpClientFactory httpClientFactory, ILogger<PedidoController> logger)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var isAdmin = User.IsInRole("Admin");
            List<Pedido> pedidos;

            if (isAdmin)
            {
                // Si es admin, obtener todos los pedidos
                try
                {
                    pedidos = await _httpClient.GetFromJsonAsync<List<Pedido>>("/api/Pedido");
                }
                catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    _logger.LogWarning("No se encontraron pedidos.");
                    pedidos = new List<Pedido>();
                }
            }
            else
            {
                // Si no es admin, obtener solo los pedidos del usuario actual
                var userIdentificacion = User.FindFirstValue(ClaimTypes.NameIdentifier);
                Cliente cliente;
                try
                {
                    cliente = await _httpClient.GetFromJsonAsync<Cliente>($"/api/Cliente/Usuario/{userIdentificacion}");
                }
                catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    _logger.LogWarning("Cliente no encontrado para el usuario con identificación: {UserIdentificacion}", userIdentificacion);
                    return NotFound("Cliente no encontrado");
                }

                try
                {
                    pedidos = await _httpClient.GetFromJsonAsync<List<Pedido>>($"/api/Pedido/Cliente/{cliente.Id}");
                }
                catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    _logger.LogWarning("No se encontraron pedidos para el cliente con ID: {ClienteId}", cliente.Id);
                    pedidos = new List<Pedido>();
                }
            }

            if (!pedidos.Any())
            {
                return View(new List<PedidoDetailsViewModel>());
            }

            var inventarioIds = pedidos.Select(p => p.InventarioId).Distinct();
            List<Inventario> inventarios;
            try
            {
                inventarios = await _httpClient.GetFromJsonAsync<List<Inventario>>($"/api/Inventario?ids={string.Join(",", inventarioIds)}");
            }
            catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                _logger.LogWarning("No se encontraron inventarios para los IDs: {InventarioIds}", string.Join(", ", inventarioIds));
                inventarios = new List<Inventario>();
            }

            var clienteIds = pedidos.Select(p => p.ClienteId).Distinct();
            List<Cliente> clientes;
            try
            {
                clientes = await _httpClient.GetFromJsonAsync<List<Cliente>>($"/api/Cliente?ids={string.Join(",", clienteIds)}");
            }
            catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                _logger.LogWarning("No se encontraron clientes para los IDs: {ClienteIds}", string.Join(", ", clienteIds));
                clientes = new List<Cliente>();
            }

            var viewModel = pedidos.Select(p => new PedidoDetailsViewModel
            {
                Pedido = p,
                Cliente = clientes.FirstOrDefault(c => c.Id == p.ClienteId),
                Inventario = inventarios.FirstOrDefault(i => i.Id == p.InventarioId)
            }).ToList();

            return View(viewModel);
        }

        public async Task<IActionResult> Create()
        {
            _logger.LogInformation("Cargando formulario para crear un nuevo pedido.");

            var userIdentificacion = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Cliente cliente = null;
            try
            {
                cliente = await _httpClient.GetFromJsonAsync<Cliente>($"/api/Cliente/Usuario/{userIdentificacion}");
            }
            catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                _logger.LogWarning("No se encontró cliente con identificación: {UserIdentificacion}", userIdentificacion);
            }

            var clientes = new List<Cliente>();
            if (cliente != null)
            {
                clientes.Add(cliente);
            }

            List<Inventario> inventarios;
            try
            {
                inventarios = await _httpClient.GetFromJsonAsync<List<Inventario>>("/api/Inventario");
            }
            catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                _logger.LogWarning("No se encontraron inventarios.");
                inventarios = new List<Inventario>();
            }

            List<Direccion> direcciones;
            try
            {
                direcciones = await _httpClient.GetFromJsonAsync<List<Direccion>>($"/api/Direccion/Usuario/{userIdentificacion}");
            }
            catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                _logger.LogWarning("No se encontraron direcciones para el usuario: {UserIdentificacion}", userIdentificacion);
                direcciones = new List<Direccion>();
            }

            var viewModel = new CreatePedidoViewModel
            {
                Clientes = clientes,
                Inventarios = inventarios,
                Direcciones = direcciones,
                Pedido = new Pedido()
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Pedido pedido)
        {
            _logger.LogInformation("Inicio del proceso de creación de un pedido.");
            _logger.LogDebug("Datos del pedido recibidos del formulario: {Pedido}", pedido);

            if (ModelState.IsValid)
            {
                _logger.LogInformation($"Intentando crear el pedido con Cliente ID: {pedido.ClienteId} y Inventario ID: {pedido.InventarioId}");
                var response = await _httpClient.PostAsJsonAsync("/api/Pedido", pedido);
                
                _logger.LogInformation($"Respuesta del servidor: {response.StatusCode}");
                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Pedido creado con éxito, redirigiendo al índice.");
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"Error al crear pedido: {errorContent}");
                    ModelState.AddModelError("", $"Error al crear pedido: {errorContent}");
                }
            }
            else
            {
                _logger.LogWarning("Validación del modelo falló.");
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    _logger.LogWarning($"Error de Validación: {error.ErrorMessage}");
                }
            }

            _logger.LogInformation("Recargando datos para el formulario tras error.");
            var clientes = await _httpClient.GetFromJsonAsync<List<Cliente>>("/api/Cliente");
            var inventarios = await _httpClient.GetFromJsonAsync<List<Inventario>>("/api/Inventario");
            var viewModel = new CreatePedidoViewModel
            {
                Pedido = pedido,
                Clientes = clientes,
                Inventarios = inventarios
            };

            return View(viewModel);
        }

        public async Task<IActionResult> Details(int id)
        {
            _logger.LogInformation($"Obteniendo detalles del pedido con ID: {id}.");
            var pedido = await _httpClient.GetFromJsonAsync<Pedido>($"/api/Pedido/{id}");
            if (pedido == null)
            {
                _logger.LogWarning($"Pedido con ID: {id} no encontrado.");
                return NotFound();
            }

            var cliente = await _httpClient.GetFromJsonAsync<Cliente>($"/api/Cliente/{pedido.ClienteId}");
            var inventario = await _httpClient.GetFromJsonAsync<Inventario>($"/api/Inventario/{pedido.InventarioId}");

            var viewModel = new PedidoDetailsViewModel
            {
                Pedido = pedido,
                Cliente = cliente,
                Inventario = inventario
            };

            return View(viewModel);
        }

        public async Task<IActionResult> Edit(int id)
        {
            _logger.LogInformation($"Cargando información para editar el pedido con ID: {id}.");
            var pedido = await _httpClient.GetFromJsonAsync<Pedido>($"/api/Pedido/{id}");
            if (pedido == null)
            {
                _logger.LogWarning($"Pedido con ID: {id} no encontrado para editar.");
                return NotFound();
            }
            return View(pedido);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Pedido pedido)
        {
            _logger.LogInformation($"Editando el pedido con ID: {pedido.Id}.");
            var response = await _httpClient.PutAsJsonAsync($"/api/Pedido/{pedido.Id}", pedido);
            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Pedido editado con éxito.");
                return RedirectToAction(nameof(Index));
            }
            var errorContent = await response.Content.ReadAsStringAsync();
            _logger.LogError($"Error al editar pedido: {errorContent}");
            ModelState.AddModelError("", $"Error al editar pedido: {errorContent}");
            return View(pedido);
        }

        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation($"Eliminando pedido con ID: {id}.");
            var response = await _httpClient.DeleteAsync($"/api/Pedido/{id}");
            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Pedido eliminado con éxito.");
                return RedirectToAction(nameof(Index));
            }
            _logger.LogError("Error al eliminar pedido.");
            ModelState.AddModelError("", "Error al eliminar pedido");
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Search(string searchTerm)
        {
            _logger.LogInformation($"Buscando pedidos que coincidan con: {searchTerm}.");
            var pedidos = await _httpClient.GetFromJsonAsync<List<Pedido>>("/api/Pedido");
            var filteredPedidos = string.IsNullOrWhiteSpace(searchTerm) ? 
                pedidos : 
                pedidos.Where(p => p.InventarioId.ToString().Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                                   p.Estado.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)).ToList();

            _logger.LogInformation($"Se encontraron {filteredPedidos.Count} pedidos que coinciden con la búsqueda.");
            if (HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(filteredPedidos);
            }

            return View("Index", filteredPedidos);
        }
    }
}
