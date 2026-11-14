namespace EUMA.Models;

// Datos para la página de error (viene de la plantilla del proyecto).
public class ErrorViewModel
{
    public string? RequestId { get; set; }

    public bool ShowRequestId
    {
        get
        {
            return !string.IsNullOrEmpty(RequestId);
        }
    }
}
