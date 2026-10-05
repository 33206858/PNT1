using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CEUMA.Models;

public class DetallePedido
{
    public int Id { get; set; }
    public int PedidoId { get; set; }
    public Pedido Pedido { get; set; } = null!;
    public int ProductoId { get; set; }
    public Producto Producto { get; set; } = null!;
    [Range(1, int.MaxValue)]
    public int Cantidad { get; set; }
    [Range(typeof(decimal), "0.01", "9999999999999999.99")]
    public decimal PrecioUnitario { get; set; }
    [NotMapped]
    public decimal Subtotal => Cantidad * PrecioUnitario;
}
