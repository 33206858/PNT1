using System.ComponentModel.DataAnnotations;

namespace EUMA.Models;

public class ItemCarrito
{
    public int ProductoId { get; set; }
    public string Nombre { get; set; } = "";
    public decimal Precio { get; set; }
    public int Cantidad { get; set; }

    public decimal Subtotal()
    {
        return Precio * Cantidad;
    }
}
