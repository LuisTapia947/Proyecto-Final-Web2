using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Necli.entities;
using Necli.persistencia.Interfaces;
using Necli.servises.Dtos;
using Necli.servises.Exceptions;
using Necli.servises.Interfaces;
using System.Reflection.Metadata;
using System.Text;
using Document = iTextSharp.text.Document;

namespace Necli.servises;

public class TransaccionService : ITransaccionService
{
    private readonly ITransaccionesRepository _TransaccionRepository;
    private readonly HttpClient _httpClient;
    private readonly NecliDbContext _context;
    private readonly ConfiguracionTransacciones _config;

    public TransaccionService(
        ITransaccionesRepository repo,
        IHttpClientFactory httpClientFactory,
        NecliDbContext context,
        IOptions<ConfiguracionTransacciones> configOptions)
    {
        _TransaccionRepository = repo;
        _httpClient = httpClientFactory.CreateClient();
        _context = context;
        _config = configOptions.Value;
    }

    public async Task<bool> ValidarCuentaDestino(string numeroCuenta, string documento, int banco)
    {
        var url = $"https://apis.fluxis.com.co/apis/interbanks/accounts/?numeroCuenta={numeroCuenta}&documentoUsuario={documento}&banco={banco}";
        var response = await _httpClient.GetAsync(url);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> RealizarTransaccionInterbancaria(string usuarioId, TransaccionInterbancariaDto dto)
    {

        var cuentaOrigen = _context.Cuentas.FirstOrDefault(c => c.UsuarioId == usuarioId);
        if (cuentaOrigen == null)
            throw new NegocioException("Cuenta origen no encontrada.");

        if (cuentaOrigen.Numero == dto.NumeroCuentaDestino)
            throw new NegocioException("No puedes transferir a tu propia cuenta.");

        if (dto.Monto < _config.MontoMinimo || dto.Monto > _config.MontoMaximo)
            throw new NegocioException("El monto está fuera de los límites permitidos.");

        if (cuentaOrigen.Saldo < dto.Monto)
            throw new NegocioException("Saldo insuficiente.");

        var cuentaValida = await ValidarCuentaDestino(dto.NumeroCuentaDestino.ToString(), dto.NumeroDocumento, dto.BancoDestino);
        if (!cuentaValida)
            throw new NegocioException("Cuenta destino inválida.");

        cuentaOrigen.Saldo -= dto.Monto;

        var fechaTransaccion = DateTime.Now;
        if (dto.BancoDestino != 1558)
            fechaTransaccion = fechaTransaccion.AddHours(_config.TiempoDiferidoHoras);

        var transaccion = new Transaccion
        {
            NumeroCuentaOrigen = cuentaOrigen.Numero,
            NumeroCuentaDestino = dto.NumeroCuentaDestino,
            Monto = dto.Monto,
            FechaTransaccion = fechaTransaccion
        };

        _context.Transacciones.Add(transaccion);
        await _context.SaveChangesAsync();

        return true;
    }

    public bool RealizarTransaccionInterna(TransaccionDto dto)
    {
        var cuentaOrigen = _TransaccionRepository.ObtenerCuentaPorNumero(dto.NumeroCuentaOrigen);
        if (cuentaOrigen == null)
            throw new NegocioException("La cuenta origen no existe.");

        var cuentaDestino = _TransaccionRepository.ObtenerCuentaPorNumero(dto.NumeroCuentaDestino);
        if (cuentaDestino == null)
            throw new NegocioException("La cuenta destino no existe.");

        if (cuentaOrigen.Numero == cuentaDestino.Numero)
            throw new NegocioException("No puedes transferir a la misma cuenta.");

        if (dto.Monto < _config.MontoMinimo || dto.Monto > _config.MontoMaximo)
            throw new NegocioException("El monto está fuera de los límites permitidos.");

        if (cuentaOrigen.Saldo < dto.Monto)
            throw new NegocioException("Saldo insuficiente.");

        cuentaOrigen.Saldo -= dto.Monto;
        cuentaDestino.Saldo += dto.Monto;

        var ahora = DateTime.Now;

        var transaccionSalida = new Transaccion
        {
            NumeroCuentaOrigen = cuentaOrigen.Numero,
            NumeroCuentaDestino = cuentaDestino.Numero,
            Monto = dto.Monto,
            TipoTransaccion = TipoTransaccion.Salida,
            FechaTransaccion = ahora
        };

        var transaccionEntrada = new Transaccion
        {
            NumeroCuentaOrigen = cuentaOrigen.Numero,
            NumeroCuentaDestino = cuentaDestino.Numero,
            Monto = dto.Monto,
            TipoTransaccion = TipoTransaccion.Entrada,
            FechaTransaccion = ahora
        };

        _TransaccionRepository.RegistrarTransaccion(transaccionSalida);
        _TransaccionRepository.RegistrarTransaccion(transaccionEntrada);

        return _TransaccionRepository.GuardarCambios() > 0;
    }

    public async Task<List<Transaccion>> ObtenerTransaccionesDelMesAnterior(string usuarioId)
    {
        var primerDiaMesAnterior = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(-1);
        var ultimoDiaMesAnterior = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddDays(-1);

        return await _context.Transacciones
            .Include(t => t.CuentaOrigen)
            .Where(t => t.CuentaOrigen.UsuarioId == usuarioId &&
                        t.FechaTransaccion >= primerDiaMesAnterior &&
                        t.FechaTransaccion <= ultimoDiaMesAnterior)
            .ToListAsync();
    }

    public byte[] GenerarPdfProtegido(List<Transaccion> transacciones, string cedula)
    {
        using (var ms = new MemoryStream())
        {
            var doc = new Document();
            var writer = PdfWriter.GetInstance(doc, ms);
            writer.SetEncryption(
                Encoding.UTF8.GetBytes(cedula),
                null,
                PdfWriter.ALLOW_PRINTING,
                PdfWriter.ENCRYPTION_AES_128);

            doc.Open();
            doc.Add(new Paragraph("Reporte de movimientos del mes anterior"));

            foreach (var t in transacciones)
            {
                doc.Add(new Paragraph($"Fecha: {t.FechaTransaccion.ToShortDateString()}, Monto: {t.Monto}, Origen: {t.NumeroCuentaOrigen}, Destino: {t.NumeroCuentaDestino}"));
            }

            doc.Close();
            return ms.ToArray();
        }
    }

}
