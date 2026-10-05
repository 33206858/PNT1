using System.ComponentModel.DataAnnotations;

namespace CEUMA.Models;

public class Categoria
{
    public int Id { get; set; }
    [Required, StringLength(60, MinimumLength = 3)]
    public string Nombre { get; set; } = "";
    [StringLength(200)]
    public string? Descripcion { get; set; }
    public ICollection<Producto> Productos { get; set; } = new List<Producto>();
}
