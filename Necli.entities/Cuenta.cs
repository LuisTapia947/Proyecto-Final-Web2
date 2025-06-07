namespace Necli.entities;

public class Cuenta
{
    public long Numero { get; set; }
    public string UsuarioId { get; set; }  
    public decimal Saldo { get; set; }
    public DateTime FechaCreacion { get; set; }
    public Usuario Usuario { get; set; }
}

