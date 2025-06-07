using Necli.entities;
using Necli.persistencia.Interfaces;

namespace Necli.persistencia;

public class UsuarioRepository: IUsuarioRepository

{
    private readonly NecliDbContext _context;

    public UsuarioRepository(NecliDbContext context)
    {
        _context = context;
    }
    public bool CrearCuentaYUsuario(Usuario usuario, Cuenta cuenta)
    {
        try
        {
            _context.Usuarios.Add(usuario);
            _context.SaveChanges();

            cuenta.UsuarioId = usuario.Id;
            _context.Cuentas.Add(cuenta);
            _context.SaveChanges();

            return true;
        }
        catch
        {
            return false;
        }
    }
   

    public Usuario ConsultarUsuario(string id)
    {
        return _context.Usuarios.FirstOrDefault(u => u.Id == id);
    }


    public bool ActualizarUsuario(Usuario usuario)
    {
        var Existeusuario = _context.Usuarios.Find(usuario.Id);
       
        Existeusuario.NombreUsuario = usuario.NombreUsuario;
        Existeusuario.ApellidoUsuario = usuario.ApellidoUsuario;
        Existeusuario.Correo = usuario.Correo;
        
            return _context.SaveChanges() > 0;
    }
    public bool CorreoEnUsoPorOtroUsuario(string correo, string idUsuarioActual)
    {
        return _context.Usuarios
            .Any(u => u.Correo == correo && u.Id != idUsuarioActual);
    }
    public Usuario? ObtenerUsuarioPorCorreo(string correo)
    {
        return _context.Usuarios.FirstOrDefault(u => u.Correo == correo);
    }


}
