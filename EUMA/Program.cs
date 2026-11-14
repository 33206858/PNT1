using EUMA.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Base de datos: SQL Server con la cadena de conexión "ConexionEUMA" de appsettings.json.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConexionEUMA")));

// MVC: controladores con vistas.
builder.Services.AddControllersWithViews();

// La sesión guarda el carrito de cada visitante.
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();

var app = builder.Build();

// Fuera de desarrollo, un error muestra la página de error en lugar del detalle técnico.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Los inputs HTML y jQuery Validate usan punto decimal: por eso la cultura es en-US.
app.UseRequestLocalization("en-US");
app.UseHttpsRedirection();
app.UseRouting();
app.UseSession();

app.UseAuthorization();

app.MapStaticAssets();

// Ruta por defecto: /Controlador/Accion/id. Sin nada en la URL va a Home/Index (el catálogo).
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
