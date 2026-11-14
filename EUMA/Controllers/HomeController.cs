using EUMA.Data;
using EUMA.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EUMA.Controllers;

public class HomeController : Controller
{
    private readonly AppDbContext _context;

    public HomeController(AppDbContext context)
    {
        _context = context;
    }

    // Catálogo (página de inicio): productos activos, con filtro por categoría y búsqueda.
    public async Task<IActionResult> Index(int? categoriaId, string? buscar)
    {
        var consulta = _context.Productos.Where(p => p.Activo);

        if (categoriaId != null)
        {
            consulta = consulta.Where(p => p.CategoriaId == categoriaId);
        }

        if (!string.IsNullOrWhiteSpace(buscar))
        {
            buscar = buscar.Trim();
            if (buscar.Length > 50)
            {
                buscar = buscar.Substring(0, 50);
            }
            consulta = consulta.Where(p => p.Nombre.Contains(buscar) || p.Codigo.Contains(buscar));
        }

        var productos = await consulta.OrderBy(p => p.Nombre).ToListAsync();

        // Datos para el menú de categorías y para que el formulario recuerde lo elegido.
        ViewBag.Categorias = await _context.Categorias.OrderBy(c => c.Nombre).ToListAsync();
        ViewBag.CategoriaId = categoriaId;
        ViewBag.Buscar = buscar;

        return View(productos);
    }

    // Detalle de un producto. Solo se muestran los productos activos.
    public async Task<IActionResult> Detalle(int id)
    {
        var producto = await _context.Productos
            .Include(p => p.Categoria)
            .FirstOrDefaultAsync(p => p.Id == id && p.Activo);

        if (producto == null)
        {
            // 404 le avisa al navegador que la página no existe.
            Response.StatusCode = 404;
            return View("NoEncontrado");
        }

        return View(producto);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        var modelo = new ErrorViewModel();
        modelo.RequestId = HttpContext.TraceIdentifier;
        return View(modelo);
    }
}
