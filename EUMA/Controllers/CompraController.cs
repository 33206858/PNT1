using System.Text.Json;
using EUMA.Data;
using EUMA.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EUMA.Controllers;

// Compra: confirmar el carrito con los datos del cliente y pagar (pago simulado).
public class CompraController : Controller
{
    private readonly AppDbContext _context;

    public CompraController(AppDbContext context)
    {
        _context = context;
    }

    // Formulario "Finalizar compra".
    public IActionResult Finalizar()
    {
        Carrito carrito = LeerCarrito();
        if (carrito.Items.Count == 0)
        {
            TempData["Error"] = "Tu carrito está vacío.";
            return RedirectToAction("Index", "Carrito");
        }

        ViewBag.Carrito = carrito;
        return View(new DatosCompra());
    }

    // Confirma la compra: crea (o busca) el cliente, crea el pedido y descuenta el stock.
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Finalizar(DatosCompra datos)
    {
        Carrito carrito = LeerCarrito();

        // 1) No se puede comprar con el carrito vacío.
        if (carrito.Items.Count == 0)
        {
            TempData["Error"] = "Tu carrito está vacío.";
            return RedirectToAction("Index", "Carrito");
        }

        // 2) Con envío a domicilio, la dirección, la localidad y el código postal son obligatorios.
        if (datos.Entrega == FormaEntrega.EnvioADomicilio)
        {
            if (string.IsNullOrWhiteSpace(datos.Direccion))
            {
                ModelState.AddModelError("Direccion", "Ingresá la dirección para el envío.");
            }
            if (string.IsNullOrWhiteSpace(datos.Localidad))
            {
                ModelState.AddModelError("Localidad", "Ingresá la localidad para el envío.");
            }
            if (string.IsNullOrWhiteSpace(datos.CodigoPostal))
            {
                ModelState.AddModelError("CodigoPostal", "Ingresá el código postal para el envío.");
            }
        }

        // 3) Si hay errores, se vuelve a mostrar el formulario con lo que escribió.
        if (!ModelState.IsValid)
        {
            ViewBag.Carrito = carrito;
            return View(datos);
        }

        // 4) Se controla cada producto contra la base: que exista, esté activo y haya stock.
        //    Los productos se guardan en una lista, en el mismo orden que los items del carrito.
        var productos = new List<Producto>();
        foreach (ItemCarrito item in carrito.Items)
        {
            var producto = await _context.Productos.FirstOrDefaultAsync(p => p.Id == item.ProductoId);
            if (producto == null || !producto.Activo)
            {
                TempData["Error"] = item.Nombre + " ya no está disponible. Quitalo del carrito para seguir.";
                return RedirectToAction("Index", "Carrito");
            }
            if (!producto.HayStock(item.Cantidad))
            {
                TempData["Error"] = "No hay stock suficiente de " + producto.Nombre + ". Disponible: " + producto.Stock + ". Cambiá la cantidad para seguir.";
                return RedirectToAction("Index", "Carrito");
            }
            productos.Add(producto);
        }

        // 5) Cliente: se busca por email (sin espacios y en minúsculas). Si no existe, se crea.
        //    Si ya existe, se usa ese cliente y no se cambian sus datos.
        string email = datos.Email.Trim().ToLower();
        var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.Email == email);
        if (cliente == null)
        {
            cliente = new Cliente();
            cliente.Nombre = datos.Nombre.Trim();
            cliente.Apellido = datos.Apellido.Trim();
            cliente.Email = email;
            cliente.Telefono = datos.Telefono.Trim();
            _context.Clientes.Add(cliente);
        }

        // 6) El pedido. Los datos de envío solo se guardan si es envío a domicilio.
        var pedido = new Pedido();
        pedido.Cliente = cliente;
        pedido.Fecha = DateTime.Now;
        pedido.Estado = EstadoPedido.PendienteDePago;
        pedido.Entrega = datos.Entrega;
        pedido.Observaciones = datos.Observaciones;
        if (datos.Entrega == FormaEntrega.EnvioADomicilio)
        {
            pedido.Direccion = datos.Direccion;
            pedido.Localidad = datos.Localidad;
            pedido.CodigoPostal = datos.CodigoPostal;
        }

        // 7) Un renglón por producto, con el precio actual de la base, y se descuenta el stock.
        for (int i = 0; i < carrito.Items.Count; i++)
        {
            ItemCarrito item = carrito.Items[i];
            Producto producto = productos[i];

            var detalle = new DetallePedido();
            detalle.Producto = producto;
            detalle.Cantidad = item.Cantidad;
            detalle.PrecioUnitario = producto.Precio;
            pedido.Detalles.Add(detalle);

            producto.Stock = producto.Stock - item.Cantidad;
        }

        // 8) Se guarda TODO junto con un solo SaveChanges: cliente, pedido, renglones y stock.
        //    Si algo falla, no se guarda nada: EF lo hace todo en una sola transacción.
        _context.Pedidos.Add(pedido);
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            TempData["Error"] = "No se pudo confirmar la compra. Probá de nuevo.";
            ViewBag.Carrito = carrito;
            return View(datos);
        }

        // 9) Recién ahora, con todo guardado, se vacía el carrito.
        carrito.Vaciar();
        GuardarCarrito(carrito);

        return RedirectToAction(nameof(Pago), new { id = pedido.Id });
    }

    // Página de pago: número de pedido, resumen y botón "Pagar".
    public async Task<IActionResult> Pago(int id)
    {
        var pedido = await _context.Pedidos
            .Include(p => p.Detalles)
            .ThenInclude(d => d.Producto)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (pedido == null)
        {
            return NotFound();
        }

        return View(pedido);
    }

    // Pago simulado: no se cobra dinero real, solo cambia el estado del pedido.
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Pagar(int id)
    {
        var pedido = await _context.Pedidos.FirstOrDefaultAsync(p => p.Id == id);
        if (pedido == null)
        {
            return NotFound();
        }

        // Pedido.Pagar() solo funciona si el pedido está pendiente de pago.
        if (pedido.Pagar())
        {
            await _context.SaveChangesAsync();
            TempData["Mensaje"] = "¡Pago realizado! Tu número de pedido es " + pedido.Id + ". Guardalo para consultar tu pedido.";
        }
        else
        {
            TempData["Error"] = "Este pedido ya no se puede pagar.";
        }

        return RedirectToAction(nameof(Pago), new { id = id });
    }

    // Lee el carrito de la sesión (igual que en CarritoController).
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

    // Guarda el carrito en la sesión (igual que en CarritoController).
    private void GuardarCarrito(Carrito carrito)
    {
        HttpContext.Session.SetString("Carrito", JsonSerializer.Serialize(carrito));
        HttpContext.Session.SetInt32("CantidadCarrito", carrito.Items.Count);
    }
}
