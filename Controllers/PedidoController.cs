using Microsoft.AspNetCore.Mvc;
using MokkilicoresExpress.Models;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace MokkilicoresExpress.Controllers
{
    public class PedidoController : Controller
    {
        private readonly HttpClient _httpClient;

        public PedidoController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
        }

        public async Task<IActionResult> Index()
        {
            var pedidos = await _httpClient.GetFromJsonAsync<List<Pedido>>("/api/Pedido");
            return View(pedidos);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Pedido pedido)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/Pedido", pedido);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }
            ModelState.AddModelError("", "Error al crear pedido");
            return View(pedido);
        }

        public async Task<IActionResult> Details(int id)
        {
            var pedido = await _httpClient.GetFromJsonAsync<Pedido>($"/api/Pedido/{id}");
            if (pedido == null)
            {
                return NotFound();
            }
            return View(pedido);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var pedido = await _httpClient.GetFromJsonAsync<Pedido>($"/api/Pedido/{id}");
            if (pedido == null)
            {
                return NotFound();
            }
            return View(pedido);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Pedido pedido)
        {
            var response = await _httpClient.PutAsJsonAsync($"/api/Pedido/{pedido.Id}", pedido);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }
            ModelState.AddModelError("", "Error al editar pedido");
            return View(pedido);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var response = await _httpClient.DeleteAsync($"/api/Pedido/{id}");
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }
            ModelState.AddModelError("", "Error al eliminar pedido");
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Search(string searchTerm)
        {
            var pedidos = await _httpClient.GetFromJsonAsync<List<Pedido>>("/api/Pedido");

            var filteredPedidos = string.IsNullOrWhiteSpace(searchTerm) ? 
                pedidos : 
                pedidos.Where(p => p.ProductoId.ToString().Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                                    p.Estado.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)).ToList();

            if (HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(filteredPedidos);
            }

            return View("Index", filteredPedidos);
        }
    }
}
