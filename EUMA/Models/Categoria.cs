using System.ComponentModel.DataAnnotations;

namespace EUMA.Models;

public class Categoria
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 50 caracteres.")]
    public string Nombre { get; set; } = "";

    [StringLength(200, ErrorMessage = "La descripción puede tener hasta 200 caracteres.")]
    [Display(Name = "Descripción")]
    public string? Descripcion { get; set; }

    public ICollection<Producto> Productos { get; set; } = new List<Producto>();
}
