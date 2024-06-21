using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using MokkilicoresExpress.Models;
using System.Security.Claims;
using Microsoft.Extensions.Caching.Memory;


namespace MokkilicoresExpress.Controllers
{
    public class AccountController : Controller
    {
        private readonly IMemoryCache _cache;
        private const string ClienteCacheKey = "Clientes";

        public AccountController(IMemoryCache cache)
        {
            _cache = cache;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string clientId, string password)
        {
            var clientes = _cache.Get<List<Cliente>>(ClienteCacheKey);
            var cliente = clientes?.FirstOrDefault(c => c.Identificacion == clientId);
            if (cliente == null || !ValidatePassword(cliente, password))
            {
                ViewBag.ErrorMessage = "Credenciales inválidas";
                return View();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, $"{cliente.Nombre} {cliente.Apellido}"),
                new Claim(ClaimTypes.NameIdentifier, cliente.Identificacion)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        private bool ValidatePassword(Cliente cliente, string password)
        {
            var expectedPassword = cliente.Identificacion + cliente.Nombre.Substring(0, 2).ToLower() + cliente.Apellido[0].ToString().ToUpper();
            return password == expectedPassword;
        }
    }
}
