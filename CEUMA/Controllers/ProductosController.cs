using Microsoft.AspNetCore.Mvc;
using CEUMA.Data;
using Microsoft.EntityFrameworkCore;

namespace CEUMA.Controllers;

public class ProductosController : Controller
{
    private readonly AppDbContext _context;

    public ProductosController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var productos = await _context.Productos
            .Where(p => p.Activo)
            .OrderBy(p => p.Nombre)
            .ToListAsync();

        return View(productos);
    }
}
