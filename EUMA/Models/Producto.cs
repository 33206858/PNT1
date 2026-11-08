using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace EUMA.Models;

public class Producto
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El código es obligatorio.")]
    [StringLength(50, ErrorMessage = "El código puede tener hasta 50 caracteres.")]
    [RegularExpression(@"[A-Za-z]{2,5}-[0-9]{3,6}", ErrorMessage = "Usá de 2 a 5 letras, un guion y de 3 a 6 números. Ejemplo: ESE-1001.")]
    [Display(Name = "Código")]
    public string Codigo { get; set; } = "";

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 100 caracteres.")]
    public string Nombre { get; set; } = "";

    [StringLength(1000, ErrorMessage = "La descripción puede tener hasta 1000 caracteres.")]
    [Display(Name = "Descripción")]
    public string? Descripcion { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Elegí una categoría.")]
    [Display(Name = "Categoría")]
    public int CategoriaId { get; set; }

    // La categoría se elige con CategoriaId: este objeto no viene del formulario.
    [ValidateNever]
    public Categoria Categoria { get; set; } = null!;

    [Required(ErrorMessage = "La presentación es obligatoria.")]
    [StringLength(30, ErrorMessage = "La presentación puede tener hasta 30 caracteres.")]
    [Display(Name = "Presentación")]
    public string Presentacion { get; set; } = "";

    [Range(typeof(decimal), "0.01", "99999.99", ParseLimitsInInvariantCulture = true, ConvertValueInInvariantCulture = true, ErrorMessage = "El precio debe ser mayor que 0 y hasta 99999.99. Usá punto para los decimales.")]
    public decimal Precio { get; set; }

    [Range(0, 99999, ErrorMessage = "El stock debe ser un número entero entre 0 y 99999.")]
    public int Stock { get; set; }

    [StringLength(255, ErrorMessage = "El nombre del archivo puede tener hasta 255 caracteres.")]
    [Display(Name = "Imagen (nombre del archivo)")]
    public string? Imagen { get; set; }

    public bool Activo { get; set; } = true;

    public ICollection<DetallePedido> DetallesPedido { get; set; } = new List<DetallePedido>();

    public bool HayStock(int cantidad)
    {
        return Stock >= cantidad;
    }
}
