using Microsoft.AspNetCore.Mvc;
using Necli.servises.Dtos;
using Necli.servises.Exceptions;
using Necli.servises.Interfaces;

namespace ProyectoFinal.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsuarioController : ControllerBase
{
    private readonly IUsuarioService _usuarioService;
    

    public UsuarioController(IUsuarioService usuarioCuentaService, ILogger<UsuarioController> logger)
    {
        _usuarioService = usuarioCuentaService;
        
    }


    [HttpPost("crear")]
    public ActionResult CrearCuentaYUsuario([FromBody] CuentaYUsuarioDto dto)
    {
        try
        {
            var resultado = _usuarioService.CrearCuentaYUsuario(dto);
            
            return Ok("Usuario y cuenta creados correctamente");
        }
        catch (Exception)
        {
            return BadRequest();
        }
    }
       
    [HttpGet("consultar")]
    public ActionResult<UsuarioConsultaDto> ConsultarUsuario(string id)
    {
        
            var usuario = _usuarioService.ConsultarUsuario(id);
            return Ok(usuario);
        
        
    }

 
    [HttpPut("Actualizar Usuario")]
    public ActionResult<bool> ActualizarUsuario([FromBody] ActulizarUsuarioDto usuario)
    {
        if (usuario == null)
            return BadRequest("El objeto usuario está vacío.");

        if (string.IsNullOrWhiteSpace(usuario.Id))
            return BadRequest("El Id del usuario es obligatorio.");

        if (string.IsNullOrWhiteSpace(usuario.NombreUsuario) || string.IsNullOrWhiteSpace(usuario.ApellidoUsuario))
            return BadRequest("El nombre y apellido del usuario son obligatorios.");

        if (string.IsNullOrWhiteSpace(usuario.Correo) || !usuario.Correo.Contains("@"))
            return BadRequest("El correo es inválido.");

        try
        {
            var resultado = _usuarioService.ActualizarUsuario(usuario);

            if (!resultado)
                return StatusCode(500, "Error al actualizar el usuario.");

            return Ok(true);
        }
        catch (Exception)
        {
            return NotFound();
        }
    }

    [HttpGet("confirmar")]
    public IActionResult ConfirmarCorreo([FromQuery] string token)
    {
        var resultado = _usuarioService.VerificarCorreo(token);

        if (!resultado)
            return BadRequest("Verificación fallida. Token inválido o usuario ya verificado.");

        return Ok("Correo verificado correctamente.");
    }

    [HttpPost("solicitar-restablecimiento")]
    public async Task<IActionResult> SolicitarRestablecimiento([FromBody] SolicitudRestablecimientoDto solicitud)
    {
        await _usuarioService.SolicitarRestablecimientoContraseña(solicitud.Correo);
        return Ok("Correo de restablecimiento enviado.");
    }

    [HttpGet("restablecer")]
    public IActionResult MostrarFormulario([FromQuery] string token)
    {
        if (string.IsNullOrEmpty(token))
            return BadRequest("Token no válido.");

        var html = $@"
    <html>
    <head>
        <title>Restablecer Contraseña</title>
    </head>
    <body style='font-family: sans-serif;'>
        <h2>Restablecer tu contraseña</h2>
        <form method='post' action='/api/Usuario/restablecer?token={token}'>
            <label>Nueva contraseña:</label><br/>
            <input type='password' name='nuevaContraseña' required /><br/><br/>
            <button type='submit'>Cambiar contraseña</button>
        </form>
    </body>
    </html>";

        return Content(html, "text/html");
    }


    [HttpPost("restablecer")]
    public IActionResult RestablecerContraseña([FromQuery] string token, [FromForm] string nuevaContraseña)
    {
        var result = _usuarioService.RestablecerContraseña(token, nuevaContraseña);
        return result ? Ok("Contraseña actualizada.") : BadRequest("Token inválido.");
    }

    [HttpPost("generar-reporte-prueba/{usuarioId}")]
    public async Task<IActionResult> GenerarReportePrueba(string usuarioId)
    {
        try
        {
            var pdf = await _usuarioService.GenerarReportePruebaAsync(usuarioId);
            return File(pdf, "application/pdf", $"{usuarioId}_prueba.pdf");
        }
        catch (NegocioException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { mensaje = "Error interno al generar el reporte." });
        }
    }



    private string ObtenerMensajeCompleto(Exception ex)
    {
        if (ex == null) return string.Empty;
        return ex.InnerException == null ? ex.Message : $"{ex.Message} -> {ObtenerMensajeCompleto(ex.InnerException)}";
    }

}
