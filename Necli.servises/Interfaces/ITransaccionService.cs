using Necli.entities;
using Necli.servises.Dtos;

namespace Necli.servises.Interfaces;

public interface ITransaccionService
{
    bool RealizarTransaccionInterna(TransaccionDto dto);
    Task<bool> RealizarTransaccionInterbancaria(string usuarioId, TransaccionInterbancariaDto dto);
    Task<List<Transaccion>> ObtenerTransaccionesDelMesAnterior(string usuarioId);
    byte[] GenerarPdfProtegido(List<Transaccion> transacciones, string cedula);
   
}
