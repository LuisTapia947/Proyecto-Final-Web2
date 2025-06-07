using Necli.servises.Dtos;

namespace Necli.servises.Interfaces;

public interface ICuentaServise
{
    ConsultarCuentaDto consultarCuenta(long numero);
    bool EliminarCuenta(long numeroCuenta);
}
