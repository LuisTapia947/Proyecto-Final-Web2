using Necli.servises.Dtos;

namespace Necli.servises.Interfaces;

public interface ICorreoServise
{
    Task EnviarCorreoAsync(string destinatario, string asunto, string cuerpoHtml);
    Task EnviarCorreoVerificacionAsync(string correo, string link);
    Task EnviarCorreoConfirmacionAsync(string correo);
    Task EnviarCorreoConAdjuntoAsync(string destinatario, string asunto, string cuerpoHtml, byte[] archivoAdjunto, string nombreArchivo);

}
