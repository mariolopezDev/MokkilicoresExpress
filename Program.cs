using MokkilicoresExpress.Models;  
using System.Collections.Generic;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Inicializar datos en memoria
//builder.Services.AddSingleton(new List<Inventario>());
//builder.Services.AddSingleton(new List<Cliente>());
//builder.Services.AddSingleton(new List<Pedido>());

builder.Services.AddSingleton<List<Inventario>>(new List<Inventario>());
builder.Services.AddSingleton<List<Cliente>>(new List<Cliente>());
builder.Services.AddSingleton<List<Pedido>>(new List<Pedido>());


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
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
