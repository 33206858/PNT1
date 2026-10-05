using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CEUMA.Models;

public enum EstadoPedido { Carrito = 0, Pendiente = 1, Entregado = 2, Cancelado = 3 }

public class Pedido
{
    public int Id { get; set; }
    public DateTime Fecha { get; set; } = DateTime.UtcNow;
    public int? ClienteId { get; set; }
    public Cliente? Cliente { get; set; }
    [EnumDataType(typeof(EstadoPedido))]
    public EstadoPedido Estado { get; set; } = EstadoPedido.Carrito;
    [StringLength(300)]
    public string? Observaciones { get; set; }
    public ICollection<DetallePedido> Detalles { get; set; } = new List<DetallePedido>();
    // Requiere cargar los detalles al consultar el pedido.
    [NotMapped]
    public decimal Total => Detalles.Sum(d => d.Subtotal);
}
