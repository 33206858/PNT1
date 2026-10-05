using System.ComponentModel.DataAnnotations;

namespace CEUMA.Models;

public class Cliente
{
    public int Id { get; set; }
    [Required, StringLength(50, MinimumLength = 2)]
    public string Nombre { get; set; } = "";
    [Required, StringLength(50, MinimumLength = 2)]
    public string Apellido { get; set; } = "";
    [Required, EmailAddress, StringLength(254)]
    public string Email { get; set; } = "";
    [Required, StringLength(20, MinimumLength = 8), RegularExpression(@"[0-9 +\-]+")]
    public string Telefono { get; set; } = "";
    [StringLength(200)]
    public string? Direccion { get; set; }
    [StringLength(100)]
    public string? Localidad { get; set; }
    public ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
}
