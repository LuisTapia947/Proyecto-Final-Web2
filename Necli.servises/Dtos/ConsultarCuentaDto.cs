using Necli.entities;
namespace Necli.servises.Dtos;

public class ConsultarCuentaDto
{
    public long Numero { get; set; }
    public string UsuarioId { get; set; }
    public decimal Saldo { get; set; }
    public DateTime FechaCreacion { get; set; }
}
