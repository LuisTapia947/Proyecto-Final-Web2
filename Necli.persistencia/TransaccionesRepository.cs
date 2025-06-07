using Necli.entities;
using Necli.persistencia.Interfaces;
namespace Necli.persistencia;

public class TransaccionesRepository:ITransaccionesRepository
{
    private readonly NecliDbContext _context;

    public TransaccionesRepository(NecliDbContext context)
    {
        _context = context;
    }

    public void RegistrarTransaccion(Transaccion transaccion)
    {
        _context.Transacciones.Add(transaccion); 
    }

    public Cuenta ObtenerCuentaPorNumero(long numero)
    {
        return _context.Cuentas.FirstOrDefault(c => c.Numero == numero);
    }

    public int GuardarCambios()
    {
        return _context.SaveChanges();
    }




}
