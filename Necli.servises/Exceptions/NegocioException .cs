using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Necli.servises.Exceptions;

public class NegocioException:Exception
{
    public NegocioException(string message) : base(message) { }
}

public class InfraestructuraException : Exception
{
    public InfraestructuraException(string message) : base(message) { }
}
