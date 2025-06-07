using Necli.entities;
namespace Necli.persistencia.Interfaces;

public interface ITransaccionesRepository
{
    void RegistrarTransaccion(Transaccion transaccion);
    Cuenta ObtenerCuentaPorNumero(long numero);
    int GuardarCambios();
}
