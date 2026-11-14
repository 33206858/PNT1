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
    public async Task<IActionResult> Index(string? numero, string? email)
    {
        ViewBag.Numero = numero;
        ViewBag.Email = email;

        // El número llega como texto y lo convertimos con int.TryParse,
        // así el mensaje de error queda en castellano.
        int numeroPedido = 0;
        if (string.IsNullOrWhiteSpace(numero))
        {
            ModelState.AddModelError("numero", "Ingresá un número de pedido mayor que cero.");
        }
        else if (!int.TryParse(numero.Trim(), out numeroPedido))
        {
            ModelState.AddModelError("numero", "Ingresá un número de pedido entero válido.");
        }
        else if (numeroPedido <= 0)
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

        // El email se compara sin espacios y en minúsculas, igual que se guardó al comprar.
        string emailNormalizado = "";
        if (email != null)
        {
            emailNormalizado = email.Trim().ToLower();
        }
        var pedido = await _context.Pedidos
            .Include(p => p.Cliente)
            .Include(p => p.Detalles)
            .ThenInclude(d => d.Producto)
            .FirstOrDefaultAsync(p => p.Id == numeroPedido && p.Cliente.Email == emailNormalizado);
        if (pedido == null)
        {
            ModelState.AddModelError("", "No se encontró el pedido. Revisá el número y el email.");
            return View();
        }
        ViewBag.Pedido = pedido;
        return View();
    }
}
