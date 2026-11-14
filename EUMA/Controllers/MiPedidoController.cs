using EUMA.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EUMA.Controllers;

public class MiPedidoController : Controller
{
    private readonly AppDbContext _context;

    public MiPedidoController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(int? numero, string? email)
    {
        ViewBag.Numero = numero;
        ViewBag.Email = email;
        // Reemplaza el error de conversión por un mensaje en castellano.
        if (ModelState.ContainsKey("numero") && ModelState["numero"]!.Errors.Count > 0)
        {
            ModelState.Remove("numero");
            ModelState.AddModelError("numero", "Ingresá un número de pedido entero válido.");
        }
        else if (numero == null || numero <= 0)
        {
            ModelState.AddModelError("numero", "Ingresá un número de pedido mayor que cero.");
        }
        if (string.IsNullOrWhiteSpace(email))
        {
            ModelState.AddModelError("email", "Ingresá el email de la compra.");
        }
        if (!ModelState.IsValid)
        {
            return View();
        }
        string emailNormalizado = email!.Trim().ToLower();
        var pedido = await _context.Pedidos
            .Include(p => p.Cliente)
            .Include(p => p.Detalles)
            .ThenInclude(d => d.Producto)
            .FirstOrDefaultAsync(p => p.Id == numero && p.Cliente.Email == emailNormalizado);
        if (pedido == null)
        {
            ModelState.AddModelError("", "No se encontró el pedido. Revisá el número y el email.");
            return View();
        }
        ViewBag.Pedido = pedido;
        return View();
    }
}
