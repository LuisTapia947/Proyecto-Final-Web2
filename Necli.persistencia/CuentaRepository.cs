using Necli.entities;
using Necli.persistencia.Interfaces;
namespace Necli.persistencia;

public class CuentaRepository:ICuentaRepository
{
    private readonly NecliDbContext _context;

    public CuentaRepository(NecliDbContext context)
    {
        _context = context;
    }

    public Cuenta ConsultarCuenta(long numero)
    {
        return _context.Cuentas.FirstOrDefault(u => u.Numero == numero);
    }

    public bool EliminarCuenta(long numero)
    {
        var cuenta = _context.Cuentas.Find(numero);

        _context.Cuentas.Remove(cuenta);
        return _context.SaveChanges() > 0;
    }




}
