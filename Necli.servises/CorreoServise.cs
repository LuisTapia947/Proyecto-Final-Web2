using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using Necli.servises.Interfaces;

public class CorreoService : ICorreoServise
{
    private readonly IConfiguration _config;

    public CorreoService(IConfiguration config)
    {
        _config = config;
    }

    public async Task EnviarCorreoAsync(string destinatario, string asunto, string cuerpoHtml)
    {
        var remitente = _config["Correo:Remitente"];
        var contraseña = _config["Correo:Contraseña"];
        var host = _config["Correo:Host"];
        var puerto = int.Parse(_config["Correo:Puerto"]);

        using var cliente = new SmtpClient(host, puerto)
        {
            Credentials = new NetworkCredential(remitente, contraseña),
            EnableSsl = true
        };

        var mensaje = new MailMessage
        {
            From = new MailAddress(remitente),
            Subject = asunto,
            Body = cuerpoHtml,
            IsBodyHtml = true
        };

        mensaje.To.Add(destinatario);

        await cliente.SendMailAsync(mensaje);
    }

    public async Task EnviarCorreoVerificacionAsync(string correo, string link)
    {
        string asunto = "Verificación de cuenta";
        string cuerpo = $@"
            <h3>Bienvenido a Necli</h3>
            <p>Haz clic en el siguiente enlace para verificar tu cuenta:</p>
            <a href='{link}' style='color: #007bff;'>Confirmar cuenta</a>";

        await EnviarCorreoAsync(correo, asunto, cuerpo);
    }

    public async Task EnviarCorreoConfirmacionAsync(string correo)
    {
        string asunto = "Cuenta confirmada exitosamente";
        string cuerpo = $@"
            <h3>Cuenta activada</h3>
            <p>Tu cuenta ha sido verificada correctamente. Ya puedes iniciar sesión.</p>";

        await EnviarCorreoAsync(correo, asunto, cuerpo);
    }
    public async Task EnviarCorreoConAdjuntoAsync(string destinatario, string asunto, string cuerpoHtml, byte[] archivoAdjunto, string nombreArchivo)
    {
        var remitente = _config["Correo:Remitente"];
        var contraseña = _config["Correo:Contraseña"];
        var host = _config["Correo:Host"];
        var puerto = int.Parse(_config["Correo:Puerto"]);

        using var cliente = new SmtpClient(host, puerto)
        {
            Credentials = new NetworkCredential(remitente, contraseña),
            EnableSsl = true
        };

        using var mensaje = new MailMessage
        {
            From = new MailAddress(remitente),
            Subject = asunto,
            Body = cuerpoHtml,
            IsBodyHtml = true
        };

        mensaje.To.Add(destinatario);

        using var ms = new MemoryStream(archivoAdjunto);
        var adjunto = new Attachment(ms, nombreArchivo, "application/pdf");
        mensaje.Attachments.Add(adjunto);

        await cliente.SendMailAsync(mensaje);
    }

}
