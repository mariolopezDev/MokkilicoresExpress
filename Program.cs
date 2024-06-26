using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Caching.Memory;
using MokkilicoresExpress.Models;  
using System.Collections.Generic;


var builder = WebApplication.CreateBuilder(args);

// Agregar servicios al contenedor de servicios
builder.Services.AddControllersWithViews();
// Agregar servicio de memoria caché
builder.Services.AddMemoryCache();
// Configurar autenticación con cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
    });

// Agregar el servicio de acceso al contexto HTTP
builder.Services.AddHttpContextAccessor();


var app = builder.Build();

// Configuración de la aplicación
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthorization();
app.UseAuthentication();



app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");



// Inicializar la caché con datos de prueba
InitializeCache(app.Services);


app.Run();


// Inicializar la caché con datos de prueba
// Este método se ejecuta al inicio de la aplicación para cargar datos de prueba en la caché    
void InitializeCache(IServiceProvider services)
{
    var cache = services.GetRequiredService<IMemoryCache>();
    const string ClienteCacheKey = "Clientes";

    if (!cache.TryGetValue(ClienteCacheKey, out List<Cliente> _))
    {
        List<Cliente> initialClientes = new List<Cliente>
        {
            new Cliente { Identificacion = "123456789", Nombre = "Mario", Apellido = "Lopez", Provincia = "Cartago", Canton = "La Union", Distrito = "Tres Rios", DineroCompradoTotal = 0, DineroCompradoUltimoAnio = 0, DineroCompradoUltimosSeisMeses = 0 },
            new Cliente { Identificacion = "88889999", Nombre = "Admin", Apellido = "Admin", Provincia = "Heredia", Canton = "Belén", Distrito = "La Asunción", DineroCompradoTotal = 0, DineroCompradoUltimoAnio = 0, DineroCompradoUltimosSeisMeses = 0 }
        };
        cache.Set(ClienteCacheKey, initialClientes, new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(60)));
    }
}
