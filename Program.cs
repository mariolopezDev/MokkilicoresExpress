using MokkilicoresExpress.Models;  
using System.Collections.Generic;

var builder = WebApplication.CreateBuilder(args);

// Agregar servicios al contenedor de servicios
builder.Services.AddControllersWithViews();

// Agregar servicio de memoria caché
builder.Services.AddMemoryCache();


// Inicializar datos en memoria
//builder.Services.AddSingleton<List<Inventario>>(new List<Inventario>());
//builder.Services.AddSingleton<List<Cliente>>(new List<Cliente>());
//builder.Services.AddSingleton<List<Pedido>>(new List<Pedido>());


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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
