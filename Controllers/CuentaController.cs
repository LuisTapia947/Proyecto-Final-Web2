using Microsoft.AspNetCore.Mvc;
using Necli.servises.Dtos;
using Necli.servises;
using Necli.servises.Interfaces;

namespace ProyectoFinal.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CuentaController : ControllerBase
{
    private readonly ICuentaServise _cuentaService;


    public CuentaController(ICuentaServise CuentaService)
    {
        _cuentaService = CuentaService;

    }

    [HttpGet("consultar")]
    public ActionResult<ConsultarCuentaDto> ConsultarCuenta(long numero)
    {
        var usuario = _cuentaService.consultarCuenta(numero);
        return Ok(usuario);
    }

    [HttpDelete("eliminar")]
    public ActionResult<bool> EliminarCuenta(long numero)
    {
        try
        {
            var resultado = _cuentaService.EliminarCuenta(numero);
            return Ok(resultado);
        }
        catch (Exception ex)
        {
            return BadRequest(ObtenerMensajeCompleto(ex));
        }
    }

    private string ObtenerMensajeCompleto(Exception ex)
    {
        if (ex == null) return string.Empty;
        return ex.InnerException == null ? ex.Message : $"{ex.Message} -> {ObtenerMensajeCompleto(ex.InnerException)}";
    }




}
