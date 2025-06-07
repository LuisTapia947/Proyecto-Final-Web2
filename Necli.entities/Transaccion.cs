using System.ComponentModel.DataAnnotations;

namespace Necli.entities;

public class Transaccion
{
    [Key]
    public int NumeroTransaccion { get; set; }

    public DateTime FechaTransaccion { get; set; } = DateTime.Now;

    public long NumeroCuentaOrigen { get; set; }

    public long NumeroCuentaDestino { get; set; }

    public decimal Monto { get; set; }

    public TipoTransaccion TipoTransaccion { get; set; }

    public string Banco { get; set; }
    public virtual Cuenta CuentaOrigen { get; set; }

    public virtual Cuenta CuentaDestino { get; set; }
}
