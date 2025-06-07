using Necli.servises.Dtos;

namespace Necli.servises.Interfaces;


  public interface IUsuarioService
  {
    bool CrearCuentaYUsuario(CuentaYUsuarioDto dto);
    UsuarioConsultaDto ConsultarUsuario(string id);
    bool ActualizarUsuario(ActulizarUsuarioDto dto);
    long ObtenerNumeroCuentaPorUsuarioId(string usuarioId);
    UsuarioDto? ValidarUsuario(string correo, string contrasena);
    bool VerificarCorreo(string token);
    string EncriptarContraseña(string contraseña);
    bool VerificarContraseña(string contraseña, string hashAlmacenado);


  }


