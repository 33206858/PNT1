using System.ComponentModel.DataAnnotations;

namespace EUMA.Models;

public class Cliente
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 50 caracteres.")]
    public string Nombre { get; set; } = "";

    [Required(ErrorMessage = "El apellido es obligatorio.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "El apellido debe tener entre 2 y 50 caracteres.")]
    public string Apellido { get; set; } = "";

    [Required(ErrorMessage = "El email es obligatorio.")]
    [EmailAddress(ErrorMessage = "El email no es válido.")]
    [StringLength(254)]
    public string Email { get; set; } = "";

    [Required(ErrorMessage = "El teléfono es obligatorio.")]
    [StringLength(20, MinimumLength = 8, ErrorMessage = "El teléfono debe tener entre 8 y 20 caracteres.")]
    [RegularExpression(@"[0-9 +\-]+", ErrorMessage = "El teléfono solo puede tener números, espacios, + y guiones.")]
    [Display(Name = "Teléfono")]
    public string Telefono { get; set; } = "";

    public ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();

    public string NombreCompleto()
    {
        return Apellido + ", " + Nombre;
    }
}
