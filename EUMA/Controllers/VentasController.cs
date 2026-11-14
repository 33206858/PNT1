using EUMA.Data;
using EUMA.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EUMA.Controllers;

public class VentasController : Controller
{
    private readonly AppDbContext _context;

    public VentasController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(EstadoPedido? estado)
    {
        var pedidos = _context.Pedidos.Include(p => p.Cliente).Include(p => p.Detalles).AsQueryable();
        if (estado != null)
        {
            pedidos = pedidos.Where(p => p.Estado == estado);
        }
        ViewBag.Estado = estado;
        return View(await pedidos.OrderByDescending(p => p.Fecha).ThenByDescending(p => p.Id).ToListAsync());
    }

    public async Task<IActionResult> Details(int id)
    {
        var pedido = await _context.Pedidos
            .Include(p => p.Cliente)
            .Include(p => p.Detalles)
            .ThenInclude(d => d.Producto)
            .FirstOrDefaultAsync(p => p.Id == id);
        if (pedido == null)
        {
            return NotFound();
        }
        return View(pedido);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> MarcarEnviado(int id)
    {
        var pedido = await _context.Pedidos.FirstOrDefaultAsync(p => p.Id == id);
        if (pedido == null)
        {
            return NotFound();
        }
        if (pedido.Estado != EstadoPedido.Pagado || pedido.Entrega != FormaEntrega.EnvioADomicilio)
        {
            TempData["Error"] = "Solo se puede enviar un pedido pagado con envío a domicilio.";
            return RedirectToAction(nameof(Details), new { id = id });
        }
        pedido.Estado = EstadoPedido.Enviado;
        await _context.SaveChangesAsync();
        TempData["Mensaje"] = "El pedido se marcó como enviado.";
        return RedirectToAction(nameof(Details), new { id = id });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> MarcarEntregado(int id)
    {
        var pedido = await _context.Pedidos.FirstOrDefaultAsync(p => p.Id == id);
        if (pedido == null)
        {
            return NotFound();
        }
        if (pedido.Estado != EstadoPedido.Enviado &&
            (pedido.Estado != EstadoPedido.Pagado || pedido.Entrega != FormaEntrega.RetiroEnLocal))
        {
            TempData["Error"] = "Solo se puede entregar un pedido enviado o uno pagado con retiro en el local.";
            return RedirectToAction(nameof(Details), new { id = id });
        }
        pedido.Estado = EstadoPedido.Entregado;
        await _context.SaveChangesAsync();
        TempData["Mensaje"] = "El pedido se marcó como entregado.";
        return RedirectToAction(nameof(Details), new { id = id });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancelar(int id)
    {
        var pedido = await _context.Pedidos
            .Include(p => p.Detalles)
            .ThenInclude(d => d.Producto)
            .FirstOrDefaultAsync(p => p.Id == id);
        if (pedido == null)
        {
            return NotFound();
        }
        if (!pedido.Cancelar())
        {
            TempData["Error"] = "Solo se puede cancelar un pedido pendiente de pago o pagado.";
            return RedirectToAction(nameof(Details), new { id = id });
        }
        await _context.SaveChangesAsync();
        TempData["Mensaje"] = "El pedido se canceló y se devolvió el stock.";
        return RedirectToAction(nameof(Details), new { id = id });
    }
}
