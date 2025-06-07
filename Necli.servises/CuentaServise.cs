using Necli.persistencia.Interfaces;
using Necli.servises.Dtos;
using Necli.servises.Interfaces;
namespace Necli.servises;

public class CuentaServise:ICuentaServise
{
    
    private readonly NecliDbContext _context;
    private readonly ICuentaRepository _cuentaRepository;

    public CuentaServise(NecliDbContext context, ICuentaRepository cuentaRepository)
    {
        _context = context;
        _cuentaRepository = cuentaRepository;
    }

    public ConsultarCuentaDto consultarCuenta(long numero)
    {
        try
        {
            var cuenta = _cuentaRepository.ConsultarCuenta(numero);

            return new ConsultarCuentaDto
            {
                Numero = cuenta.Numero,
                UsuarioId = cuenta.UsuarioId,
                Saldo = cuenta.Saldo,
                FechaCreacion = cuenta.FechaCreacion

            };
        }
        catch (Exception)
        {
            throw;
        }
        
    }
    public bool EliminarCuenta(long numeroCuenta)
    {
        try
        {
            return _cuentaRepository.EliminarCuenta(numeroCuenta);
        }
        catch (Exception ex)
        {
           throw new Exception("No se pudo eliminar la cuenta." + ex.Message, ex);
        }
    }

}
