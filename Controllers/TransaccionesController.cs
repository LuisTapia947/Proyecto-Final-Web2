using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Necli.servises.Dtos;
using Necli.servises.Exceptions;
using Necli.servises.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

[ApiController]
[Route("api/[controller]")]
public class TransaccionesController : ControllerBase
{
    private readonly ITransaccionService _transaccionService;
    private readonly IUsuarioService _userService;
    private readonly IConfiguration _config;
    public TransaccionesController(ITransaccionService transaccionService, IUsuarioService userService, IConfiguration config)
    {
        _transaccionService = transaccionService;
        _userService = userService;
        _config = config;
    }


    [HttpPost("realizar")]
    public ActionResult<bool> RealizarTransaccion([FromBody] TransaccionDto dto)
    {
        try
        {
            string usuarioId = User.FindFirst("UserId")?.Value;

            if (dto == null)
                return BadRequest("Datos inválidos.");

            long numeroCuentaOrigen;

            
            if (!string.IsNullOrEmpty(usuarioId))
            {
                var cuentaOrigen = _userService.ObtenerNumeroCuentaPorUsuarioId(usuarioId);
                if (cuentaOrigen == null)
                    return BadRequest("Cuenta origen no encontrada.");
                numeroCuentaOrigen = cuentaOrigen;
            }
            
            else 
            {
                numeroCuentaOrigen = dto.NumeroCuentaOrigen;
            }
            

            
            var dtoSeguro = new TransaccionDto
            {
                NumeroCuentaOrigen = numeroCuentaOrigen,
                NumeroCuentaDestino = dto.NumeroCuentaDestino,
                Monto = dto.Monto
            };

            var resultado = _transaccionService.RealizarTransaccionInterna(dtoSeguro);

            return resultado ? Ok(true) : BadRequest("No se pudo realizar la transacción.");
        }
        catch (NegocioException)
        {
            return BadRequest();
        }
        catch (Exception)
        {
            return StatusCode(500, "Error interno");
        }
    }

    [HttpPost("login")]
    public ActionResult<string> Login([FromBody] LoginDto loginDto)
    {
        var usuario = _userService.ValidarUsuario(loginDto.Correo, loginDto.Contrasena);
        if (usuario == null)
            return Unauthorized("Correo o contraseña incorrectos.");

        var claims = new[]
        {
        new Claim("UserId", usuario.Id),
        new Claim(ClaimTypes.Name, usuario.NombreUsuario)
    };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddHours(2),
            signingCredentials: creds);

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return Ok(tokenString);
    }

    [Authorize]
    [HttpPost("interbancaria")]
    public async Task<IActionResult> RealizarTransaccionInterbancaria([FromBody] TransaccionInterbancariaDto dto)
    {
        var userId = User.FindFirstValue("UserId");

        var resultado = await _transaccionService.RealizarTransaccionInterbancaria(userId, dto);
        return resultado ? Ok("Transacción programada exitosamente.") : BadRequest("Error en la transacción.");
    }


}
