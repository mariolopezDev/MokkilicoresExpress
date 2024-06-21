using Microsoft.Extensions.Caching.Memory;
using MokkilicoresExpress.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace MokkilicoresExpress.Middleware
{
    public class AuthenticationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IMemoryCache _cache;
        private readonly ILogger<AuthenticationMiddleware> _logger;



        public AuthenticationMiddleware(RequestDelegate next, IMemoryCache cache, ILogger<AuthenticationMiddleware> logger)
        {
            _next = next;
            _cache = cache;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Permite el paso sin autenticación a la página de inicio de sesión
            if (context.Request.Path.StartsWithSegments("/Home/Login"))
            {
                await _next(context);
                return;
            }

            // Requiere autenticación para cualquier otra ruta
            if (context.Request.Method == "POST" && context.Request.HasFormContentType)
            {
                var formData = await context.Request.ReadFormAsync();
                var clientId = formData["clientId"].ToString();
                var password = formData["password"].ToString();

                _logger.LogInformation("Intento de acceso con ClientId: {ClientId} y Password: {Password}", clientId, password);

                if (!string.IsNullOrWhiteSpace(clientId) && !string.IsNullOrWhiteSpace(password))
                {
                    var clientes = _cache.Get<List<Cliente>>("Clientes") ?? new List<Cliente>();
                    var cliente = clientes.FirstOrDefault(c => c.Identificacion == clientId && IsValidPassword(c, password));

                    if (cliente != null)
                    {
                        _logger.LogInformation("Acceso concedido para el cliente con ID: {ClientId}", clientId);
                        context.Items["Cliente"] = cliente;  // Almacenar el cliente en el contexto para su uso posterior
                        await _next(context);
                    }
                    else
                    {
                        _logger.LogWarning("Acceso no autorizado para ClientId: {ClientId}", clientId);
                        context.Response.StatusCode = 401;  // Unauthorized
                        await context.Response.WriteAsync("No autorizado: Cliente no encontrado o contraseña incorrecta");
                    }
                }
                else
                {
                    _logger.LogWarning("Solicitud incorrecta a {Path}, se espera un POST con formulario", context.Request.Path);
                    context.Response.StatusCode = 400;  // Bad Request
                    await context.Response.WriteAsync("Se requiere el formulario con 'clientId' y 'password'");
                }
            }
            else
            {
                // Si la solicitud no es POST o no contiene datos de formulario, emitir error
                context.Response.StatusCode = 400;  // Bad Request
                await context.Response.WriteAsync("Se requiere un método POST con datos de formulario para la autenticación");
            }
        }

        private bool IsValidPassword(Cliente cliente, string password)
        {
            var expectedPassword = cliente.Identificacion + cliente.Nombre.Substring(0, 2).ToLower() + char.ToUpper(cliente.Apellido[0]);
            _logger.LogInformation("Verificando contraseña: ingresada {Ingresada}, esperada {Esperada}", password, expectedPassword);
            return password == expectedPassword;
        }
    }
}
