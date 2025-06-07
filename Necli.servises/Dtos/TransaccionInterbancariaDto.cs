namespace Necli.servises.Dtos;

public class TransaccionInterbancariaDto
{
    public long NumeroCuentaOrigen { get; set; }
    public long NumeroCuentaDestino { get; set; }
    public decimal Monto { get; set; }
    public string NumeroDocumento { get; set; }
    public int BancoDestino { get; set; }
    public string Moneda { get; set; }
    public string TipoDocumento { get; set; }
}

