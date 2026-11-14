using System.Text.Json;
using EUMA.Data;
using EUMA.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EUMA.Controllers;

// Carrito de compras. Vive en la sesión del visitante, no en la base de datos.
public class CarritoController : Controller
{
    private readonly AppDbContext _context;

    public CarritoController(AppDbContext context)
    {
        _context = context;
    }

    // Muestra el carrito.
    public IActionResult Index()
    {
        Carrito carrito = LeerCarrito();
        return View(carrito);
    }

    // Agrega un producto. Si ya estaba, suma la cantidad.
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Agregar(int productoId, int cantidad)
    {
        var producto = await _context.Productos.FirstOrDefaultAsync(p => p.Id == productoId && p.Activo);
        if (producto == null)
        {
            TempData["Error"] = "El producto no está disponible.";
            return RedirectToAction("Index", "Home");
        }

        // Si escriben 0, un negativo o un decimal (1.5), cantidad llega como 0 o menos.
        if (cantidad < 1)
        {
            TempData["Error"] = "La cantidad tiene que ser un número entero desde 1.";
            return RedirectToAction("Detalle", "Home", new { id = productoId });
        }

        Carrito carrito = LeerCarrito();

        // Lo que ya había de este producto en el carrito más lo nuevo no puede superar el stock.
        int yaEnCarrito = 0;
        ItemCarrito? item = carrito.Items.FirstOrDefault(i => i.ProductoId == productoId);
        if (item != null)
        {
            yaEnCarrito = item.Cantidad;
        }
        if (!producto.HayStock(yaEnCarrito + cantidad))
        {
            TempData["Error"] = "No hay stock suficiente de " + producto.Nombre + ". Disponible: " + producto.Stock + " (en tu carrito: " + yaEnCarrito + ").";
            return RedirectToAction("Detalle", "Home", new { id = productoId });
        }

        // El nombre y el precio los toma Carrito.Agregar del producto de la base.
        carrito.Agregar(producto, cantidad);
        GuardarCarrito(carrito);
        TempData["Mensaje"] = "Agregaste " + producto.Nombre + " al carrito.";
        return RedirectToAction(nameof(Index));
    }

    // Cambia la cantidad de un producto del carrito.
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> CambiarCantidad(int productoId, int cantidad)
    {
        if (cantidad < 1)
        {
            TempData["Error"] = "La cantidad tiene que ser un número entero desde 1. Para sacar un producto, usá Quitar.";
            return RedirectToAction(nameof(Index));
        }

        var producto = await _context.Productos.FirstOrDefaultAsync(p => p.Id == productoId && p.Activo);
        if (producto == null)
        {
            TempData["Error"] = "Ese producto ya no está disponible. Quitalo del carrito.";
            return RedirectToAction(nameof(Index));
        }

        if (!producto.HayStock(cantidad))
        {
            TempData["Error"] = "No hay stock suficiente de " + producto.Nombre + ". Disponible: " + producto.Stock + ".";
            return RedirectToAction(nameof(Index));
        }

        Carrito carrito = LeerCarrito();
        carrito.CambiarCantidad(productoId, cantidad);
        GuardarCarrito(carrito);
        return RedirectToAction(nameof(Index));
    }

    // Quita un producto del carrito.
    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Quitar(int productoId)
    {
        Carrito carrito = LeerCarrito();
        carrito.Quitar(productoId);
        GuardarCarrito(carrito);
        TempData["Mensaje"] = "Quitaste el producto del carrito.";
        return RedirectToAction(nameof(Index));
    }

    // Lee el carrito de la sesión. Si no hay ninguno, devuelve uno vacío.
    private Carrito LeerCarrito()
    {
        string? texto = HttpContext.Session.GetString("Carrito");
        if (texto == null)
        {
            return new Carrito();
        }
        Carrito? carrito = JsonSerializer.Deserialize<Carrito>(texto);
        if (carrito == null)
        {
            return new Carrito();
        }
        return carrito;
    }

    // Guarda el carrito en la sesión como texto (JSON).
    private void GuardarCarrito(Carrito carrito)
    {
        HttpContext.Session.SetString("Carrito", JsonSerializer.Serialize(carrito));
        // Guardamos aparte cuántos productos distintos hay, para mostrarlo en el menú.
        HttpContext.Session.SetInt32("CantidadCarrito", carrito.Items.Count);
    }
}
