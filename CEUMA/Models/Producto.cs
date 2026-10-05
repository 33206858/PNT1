using System.ComponentModel.DataAnnotations;

namespace CEUMA.Models;

public enum Origen { Argentina = 0, ElaboracionPropia = 1, Importado = 2 }

public class Producto
{
    public int Id { get; set; }
    [Required, StringLength(50), RegularExpression(@"[A-Za-z0-9-]+")]
    public string Codigo { get; set; } = "";
    [Required, StringLength(100, MinimumLength = 3)]
    public string Nombre { get; set; } = "";
    [StringLength(1000)]
    public string? Descripcion { get; set; }
    public int CategoriaId { get; set; }
    public Categoria Categoria { get; set; } = null!;
    [EnumDataType(typeof(Origen))]
    public Origen Origen { get; set; }
    [Required, StringLength(30)]
    public string Presentacion { get; set; } = "";
    [Range(typeof(decimal), "0.01", "9999999999999999.99")]
    public decimal Precio { get; set; }
    [Range(0, int.MaxValue)]
    public int Stock { get; set; }
    [StringLength(255)]
    public string? Imagen { get; set; }
    public bool Activo { get; set; } = true;
    public ICollection<DetallePedido> DetallesPedido { get; set; } = new List<DetallePedido>();
}
