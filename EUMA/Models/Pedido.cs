using System.ComponentModel.DataAnnotations;

namespace EUMA.Models;

public enum EstadoPedido
{
    [Display(Name = "Pendiente de pago")]
    PendienteDePago = 0,
    Pagado = 1,
    Enviado = 2,
    Entregado = 3,
    Cancelado = 4
}

public enum FormaEntrega
{
    [Display(Name = "Envío a domicilio")]
    EnvioADomicilio = 0,
    [Display(Name = "Retiro en el local")]
    RetiroEnLocal = 1
}

public class Pedido
{
    public int Id { get; set; }

    public DateTime Fecha { get; set; } = DateTime.Now;

    public int ClienteId { get; set; }
    public Cliente Cliente { get; set; } = null!;

    public EstadoPedido Estado { get; set; } = EstadoPedido.PendienteDePago;

    [Display(Name = "Forma de entrega")]
    public FormaEntrega Entrega { get; set; }

    [StringLength(100)]
    [Display(Name = "Dirección")]
    public string? Direccion { get; set; }

    [StringLength(60)]
    public string? Localidad { get; set; }

    [StringLength(8)]
    [Display(Name = "Código postal")]
    public string? CodigoPostal { get; set; }

    [StringLength(300)]
    public string? Observaciones { get; set; }

    [Display(Name = "Fecha de pago")]
    public DateTime? FechaPago { get; set; }

    public ICollection<DetallePedido> Detalles { get; set; } = new List<DetallePedido>();

    // Para que dé bien, hay que cargar los Detalles con Include.
    public decimal Total()
    {
        decimal total = 0;
        foreach (DetallePedido detalle in Detalles)
        {
            total = total + detalle.Subtotal();
        }
        return total;
    }

    // Solo se puede pagar un pedido pendiente de pago.
    public bool Pagar()
    {
        if (Estado != EstadoPedido.PendienteDePago)
        {
            return false;
        }
        Estado = EstadoPedido.Pagado;
        FechaPago = DateTime.Now;
        return true;
    }

    // Solo se cancela si está pendiente de pago o pagado, y devuelve el stock.
    // Hay que cargar los Detalles con su Producto (Include y ThenInclude).
    public bool Cancelar()
    {
        if (Estado != EstadoPedido.PendienteDePago && Estado != EstadoPedido.Pagado)
        {
            return false;
        }
        foreach (DetallePedido detalle in Detalles)
        {
            detalle.Producto.Stock = detalle.Producto.Stock + detalle.Cantidad;
        }
        Estado = EstadoPedido.Cancelado;
        return true;
    }
}
