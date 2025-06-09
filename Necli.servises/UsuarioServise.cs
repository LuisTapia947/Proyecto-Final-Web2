using Necli.entities;
using Necli.persistencia.Interfaces;
using Necli.servises.Dtos;
using Necli.servises.Exceptions;
using Necli.servises.Interfaces;
using System.Security.Cryptography;

namespace Necli.servises;

public class UsuarioService : IUsuarioService
{
    private readonly NecliDbContext _context;
    private readonly ICorreoServise _correoService;
    private readonly IUsuarioRepository _usuarioRepository;

    public UsuarioService(NecliDbContext context, ICorreoServise correoService, IUsuarioRepository usuarioRepository)
    {
        _context = context;
        _correoService = correoService;
        _usuarioRepository = usuarioRepository;
    }
    public bool CrearCuentaYUsuario(CuentaYUsuarioDto dto)
    {
        try
        {

            if (_context.Usuarios.Any(u => u.Correo == dto.Correo))
                throw new NegocioException("El correo ya está en uso.");


            if (_context.Cuentas.Any(c => c.Numero == dto.Numero))
                throw new NegocioException("El número de cuenta ya está en uso.");

            var contraseñaEncriptada = EncriptarContraseña(dto.Contraseña);

            var usuario = new Usuario
            {
                Id = dto.Id,
                NombreUsuario = dto.NombreUsuario,
                ApellidoUsuario = dto.ApellidoUsuario,
                Correo = dto.Correo,
                Contraseña = contraseñaEncriptada,
                TipoUsuario = dto.TipoUsuario,
                FechaNacimiento = dto.FechaNacimiento,
                TokenVerificacion = Guid.NewGuid().ToString("N"),
                CorreoVerificado = false
            };



            var cuenta = new Cuenta
            {
                Numero = dto.Numero,
                UsuarioId = usuario.Id,
                Saldo = dto.SaldoInicial
            };

            var creado = _usuarioRepository.CrearCuentaYUsuario(usuario, cuenta);

            if (creado)
            {

                var link = $"https://localhost:7297/api/Usuario/confirmar?token={usuario.TokenVerificacion}";
                _correoService.EnviarCorreoVerificacionAsync(dto.Correo, link);
            }

            return creado;
        }
        catch
        {
            return false;
        }
    }
    public UsuarioConsultaDto ConsultarUsuario(string id)
    {
        try
        {
            var usuario = _usuarioRepository.ConsultarUsuario(id);



            return new UsuarioConsultaDto
            {
                Id = usuario.Id,
                NombreUsuario = usuario.NombreUsuario,
                ApellidoUsuario = usuario.ApellidoUsuario,
                Correo = usuario.Correo,
                TipoUsuario = usuario.TipoUsuario,
                FechaNacimiento = usuario.FechaNacimiento
            };
        }
        catch (Exception)
        {
            throw;
        }
    }


    public bool ActualizarUsuario(ActulizarUsuarioDto dto)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto), "No puedes envias valore vacios.");

        if (string.IsNullOrWhiteSpace(dto.Id))
            throw new ArgumentException("El Id del usuario es obligatorio.");

        if (string.IsNullOrWhiteSpace(dto.NombreUsuario))
            throw new ArgumentException("El nombre del usuario es obligatorio.");

        if (string.IsNullOrWhiteSpace(dto.ApellidoUsuario))
            throw new ArgumentException("El apellido del usuario es obligatorio.");

        if (_usuarioRepository.CorreoEnUsoPorOtroUsuario(dto.Correo, dto.Id))
            throw new Exception("El correo ya está siendo utilizado por otro usuario.");

        if (string.IsNullOrWhiteSpace(dto.Correo) || !dto.Correo.Contains("@"))
            throw new ArgumentException("El correo del usuario no es válido.");

        var usuario = new Usuario
        {
            Id = dto.Id,
            NombreUsuario = dto.NombreUsuario,
            ApellidoUsuario = dto.ApellidoUsuario,
            Correo = dto.Correo
        };

        return _usuarioRepository.ActualizarUsuario(usuario);
    }
    public long ObtenerNumeroCuentaPorUsuarioId(string usuarioId)
    {
        var cuenta = _context.Cuentas.FirstOrDefault(c => c.UsuarioId == usuarioId);
        if (cuenta == null)
            throw new NegocioException("No se encontró la cuenta asociada al usuario.");

        return cuenta.Numero;
    }

    public UsuarioDto? ValidarUsuario(string correo, string contrasena)
    {
        var usuario = _usuarioRepository.ObtenerUsuarioPorCorreo(correo);
        if (usuario == null || !VerificarContraseña(contrasena, usuario.Contraseña))
            return null;

        return new UsuarioDto
        {
            Id = usuario.Id,
            NombreUsuario = usuario.NombreUsuario,
            Correo = usuario.Correo
        };
    }

    public bool VerificarCorreo(string token)
    {
        try
        {
            var usuario = _context.Usuarios.FirstOrDefault(u => u.TokenVerificacion == token);

            if (usuario == null)
                throw new NegocioException("Token inválido o expirado.");

            usuario.CorreoVerificado = true;
            usuario.TokenVerificacion = null;

            _context.SaveChanges();

            _correoService.EnviarCorreoConfirmacionAsync(usuario.Correo);

            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task SolicitarRestablecimientoContraseña(string correo)
    {
        var usuario = _context.Usuarios.FirstOrDefault(u => u.Correo == correo);

        if (usuario == null)
            throw new NegocioException("El correo no está registrado.");

        // Generar token único
        usuario.TokenVerificacion = Guid.NewGuid().ToString("N");
        _context.SaveChanges();

        var link = $"https://localhost:7297/api/Usuario/restablecer?token={usuario.TokenVerificacion}";
        string asunto = "Restablecimiento de contraseña";
        string cuerpo = $@"
        <h3>Solicitud de restablecimiento de contraseña</h3>
        <p>Haz clic en el siguiente enlace para restablecer tu contraseña:</p>
        <a href='{link}' style='color: #007bff;'>Restablecer contraseña</a>";

        await _correoService.EnviarCorreoAsync(correo, asunto, cuerpo);
    }
    public bool RestablecerContraseña(string token, string nuevaContraseña)
    {
        var usuario = _context.Usuarios.FirstOrDefault(u => u.TokenVerificacion == token);

        if (usuario == null)
            throw new NegocioException("Token inválido o expirado.");

        usuario.Contraseña = EncriptarContraseña(nuevaContraseña);
        usuario.TokenVerificacion = null;

        _context.SaveChanges();

        _correoService.EnviarCorreoAsync(usuario.Correo, "Contraseña restablecida",
            "<p>Tu contraseña se ha restablecido exitosamente.</p>");

        return true;
    }


    public string EncriptarContraseña(string contraseña)
    {

        byte[] salt = new byte[16];
        using (var rng = new RNGCryptoServiceProvider())
        {
            rng.GetBytes(salt);
        }


        using (var pbkdf2 = new Rfc2898DeriveBytes(contraseña, salt, 10000))
        {
            byte[] hash = pbkdf2.GetBytes(20);


            byte[] hashBytes = new byte[36];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);

            return Convert.ToBase64String(hashBytes);
        }
    }

    public bool VerificarContraseña(string contraseña, string hashAlmacenado)
    {
        byte[] hashBytes = Convert.FromBase64String(hashAlmacenado);

        byte[] salt = new byte[16];
        Array.Copy(hashBytes, 0, salt, 0, 16);

        using (var pbkdf2 = new Rfc2898DeriveBytes(contraseña, salt, 10000))
        {
            byte[] hash = pbkdf2.GetBytes(20);

            for (int i = 0; i < 20; i++)
            {
                if (hashBytes[i + 16] != hash[i])
                    return false;
            }

            return true;
        }
    }



}
