using System.ComponentModel.DataAnnotations;

namespace EUMA.Models;

public class Carrito
{
    public List<ItemCarrito> Items { get; set; } = new List<ItemCarrito>();

    // Si el producto ya está en el carrito, suma la cantidad.
    // El nombre y el precio salen del producto de la base, nunca del navegador.
    public void Agregar(Producto producto, int cantidad)
    {
        ItemCarrito? item = Items.FirstOrDefault(i => i.ProductoId == producto.Id);
        if (item == null)
        {
            item = new ItemCarrito();
            item.ProductoId = producto.Id;
            item.Cantidad = 0;
            Items.Add(item);
        }
        item.Nombre = producto.Nombre;
        item.Precio = producto.Precio;
        item.Cantidad = item.Cantidad + cantidad;
    }

    public void CambiarCantidad(int productoId, int cantidad)
    {
        ItemCarrito? item = Items.FirstOrDefault(i => i.ProductoId == productoId);
        if (item != null)
        {
            item.Cantidad = cantidad;
        }
    }

    public void Quitar(int productoId)
    {
        Items.RemoveAll(i => i.ProductoId == productoId);
    }

    public void Vaciar()
    {
        Items.Clear();
    }

    public decimal Total()
    {
        decimal total = 0;
        foreach (ItemCarrito item in Items)
        {
            total = total + item.Subtotal();
        }
        return total;
    }
}
