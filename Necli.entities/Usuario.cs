using System.ComponentModel.DataAnnotations.Schema;

namespace Necli.entities;
public class Usuario
{
    public string Id { get; set; }
    public string NombreUsuario { get; set; }
    public string ApellidoUsuario { get; set; }
    public string Correo { get; set; }
    public string Contraseña { get; set; }
    public TipoUsuario TipoUsuario { get; set; }
    public DateTime FechaNacimiento { get; set; }
    public DateTime FechaCreacion { get; set; }
    public bool CorreoVerificado { get; set; } = false;
    public string TokenVerificacion { get; set; }
    public ICollection<Cuenta> Cuentas { get; set; }
}
