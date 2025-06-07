using Necli.entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Necli.servises.Dtos
{
    public class UsuarioConsultaDto
    {
        public string Id { get; set; }
        public string NombreUsuario { get; set; }
        public string ApellidoUsuario { get; set; }
        public string Correo { get; set; }
        public TipoUsuario TipoUsuario { get; set; }
        public DateTime FechaNacimiento { get; set; }
    }
}
