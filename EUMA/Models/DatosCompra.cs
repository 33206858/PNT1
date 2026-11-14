using System.ComponentModel.DataAnnotations;

namespace EUMA.Models;

// Datos del formulario "Finalizar compra". No se guarda en la base:
// con estos datos se crean (o se buscan) el Cliente y el Pedido.
public class DatosCompra
{
    [Required(ErrorMessage = "Ingresá tu nombre.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 50 caracteres.")]
    public string Nombre { get; set; } = "";

    [Required(ErrorMessage = "Ingresá tu apellido.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "El apellido debe tener entre 2 y 50 caracteres.")]
    public string Apellido { get; set; } = "";

    [Required(ErrorMessage = "Ingresá tu email.")]
    [EmailAddress(ErrorMessage = "El email no es válido.")]
    [StringLength(254)]
    public string Email { get; set; } = "";

    [Required(ErrorMessage = "Ingresá tu teléfono.")]
    [StringLength(20, MinimumLength = 8, ErrorMessage = "El teléfono debe tener entre 8 y 20 caracteres.")]
    [RegularExpression(@"[0-9 +\-]+", ErrorMessage = "El teléfono solo puede tener números, espacios, + y guiones.")]
    [Display(Name = "Teléfono")]
    public string Telefono { get; set; } = "";

    [Display(Name = "Forma de entrega")]
    public FormaEntrega Entrega { get; set; }

    [StringLength(100, ErrorMessage = "La dirección puede tener hasta 100 caracteres.")]
    [Display(Name = "Dirección")]
    public string? Direccion { get; set; }

    [StringLength(60, ErrorMessage = "La localidad puede tener hasta 60 caracteres.")]
    public string? Localidad { get; set; }

    [StringLength(8, MinimumLength = 4, ErrorMessage = "El código postal debe tener entre 4 y 8 caracteres.")]
    [Display(Name = "Código postal")]
    public string? CodigoPostal { get; set; }

    [StringLength(300, ErrorMessage = "Las observaciones pueden tener hasta 300 caracteres.")]
    public string? Observaciones { get; set; }
}
