using Necli.entities;

namespace Necli.servises.Dtos;

public class ActulizarUsuarioDto
{
    public string Id { get; set; }
    public string NombreUsuario { get; set; }
    public string ApellidoUsuario { get; set; }
    public string Correo { get; set; }
    public TipoUsuario TipoUsuario { get; set; }
}
