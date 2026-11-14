using EUMA.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EUMA.Controllers;

public class ClientesController : Controller
{
    private readonly AppDbContext _context;

    public ClientesController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(string? buscar)
    {
        var clientes = _context.Clientes.Include(c => c.Pedidos).AsQueryable();
        buscar = (buscar ?? "").Trim();
        if (buscar != "")
        {
            clientes = clientes.Where(c => c.Apellido.Contains(buscar) || c.Email.Contains(buscar));
        }
        ViewBag.Buscar = buscar;
        return View(await clientes.OrderBy(c => c.Apellido).ThenBy(c => c.Nombre).ToListAsync());
    }

    public async Task<IActionResult> Details(int id)
    {
        var cliente = await _context.Clientes
            .Include(c => c.Pedidos)
            .ThenInclude(p => p.Detalles)
            .FirstOrDefaultAsync(c => c.Id == id);
        if (cliente == null)
        {
            return NotFound();
        }
        return View(cliente);
    }
}
