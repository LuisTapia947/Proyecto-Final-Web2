using Necli.entities;

namespace Necli.servises.Dtos;  
 public class CuentaYUsuarioDto
    {
        
        public string Id { get; set; }
        public string NombreUsuario { get; set; }
        public string ApellidoUsuario { get; set; }
        public string Correo { get; set; }
        public string Contraseña { get; set; }
        public long Numero { get; set; }
        public TipoUsuario TipoUsuario { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public decimal SaldoInicial { get; set; }
}


