using Necli.entities;

namespace Necli.persistencia.Interfaces;

public interface IUsuarioRepository
{
    bool CrearCuentaYUsuario(Usuario usuario, Cuenta cuenta);
    Usuario ConsultarUsuario(string id);
    bool ActualizarUsuario(Usuario usuario);
    bool CorreoEnUsoPorOtroUsuario(string correo, string idUsuarioActual);
    Usuario? ObtenerUsuarioPorCorreo(string correo);
}
