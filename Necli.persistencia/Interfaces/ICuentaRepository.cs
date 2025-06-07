using Necli.entities;

namespace Necli.persistencia.Interfaces;

public interface ICuentaRepository
{
    Cuenta ConsultarCuenta(long numero);
    bool EliminarCuenta(long numero);
}
