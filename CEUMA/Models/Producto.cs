using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace CEUMA.Models;

public enum Origen { Argentina = 0, [Display(Name = "Elaboración propia")] ElaboracionPropia = 1, Importado = 2 }

public class Producto
{
    public int Id { get; set; }
    [Required, StringLength(50), RegularExpression(@"[A-Za-z0-9-]+")]
    [Display(Name = "Código")]
    public string Codigo { get; set; } = "";
    [Required, StringLength(100, MinimumLength = 3)]
    public string Nombre { get; set; } = "";
    [StringLength(1000)]
    [Display(Name = "Descripción")]
    public string? Descripcion { get; set; }
    [Display(Name = "Categoría"), Range(1, int.MaxValue, ErrorMessage = "Seleccioná una categoría.")]
    public int CategoriaId { get; set; }
    [ValidateNever]
    public Categoria Categoria { get; set; } = null!;
    [EnumDataType(typeof(Origen))]
    public Origen Origen { get; set; }
    [Required, StringLength(30)]
    [Display(Name = "Presentación")]
    public string Presentacion { get; set; } = "";
    [Range(typeof(decimal), "0.01", "9999999999999999.99", ParseLimitsInInvariantCulture = true, ConvertValueInInvariantCulture = true)]
    public decimal Precio { get; set; }
    [Range(0, int.MaxValue)]
    public int Stock { get; set; }
    [StringLength(255)]
    public string? Imagen { get; set; }
    public bool Activo { get; set; } = true;
    public ICollection<DetallePedido> DetallesPedido { get; set; } = new List<DetallePedido>();
}
