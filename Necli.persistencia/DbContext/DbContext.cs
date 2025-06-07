using Microsoft.EntityFrameworkCore;
using Necli.entities;

public class NecliDbContext : DbContext
{
    public NecliDbContext(DbContextOptions<NecliDbContext> options)
        : base(options)
    {
    }

    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Cuenta> Cuentas { get; set; }
    public DbSet<Transaccion> Transacciones { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Usuario
        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(u => u.Id);

            entity.Property(u => u.NombreUsuario)
                  .IsRequired()
                  .HasMaxLength(100);

            entity.Property(u => u.ApellidoUsuario)
                  .IsRequired()
                  .HasMaxLength(100);

            entity.Property(u => u.Correo)
                  .IsRequired()
                  .HasMaxLength(100);

            entity.Property(u => u.Contraseña)
                  .HasColumnName("Contrasena")
                  .IsRequired();

            entity.Property(u => u.TipoUsuario)
                  .IsRequired();

            entity.Property(u => u.FechaNacimiento)
                  .IsRequired();

            entity.Property(u => u.FechaCreacion)
                  .HasDefaultValueSql("GETDATE()");

            // Relación 1:N con Cuentas
            entity.HasMany(u => u.Cuentas)
                  .WithOne(c => c.Usuario)
                  .HasForeignKey(c => c.UsuarioId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Cuenta
        modelBuilder.Entity<Cuenta>(entity =>
        {
            entity.HasKey(c => c.Numero);

            // Valor asignado manualmente 
            entity.Property(c => c.Numero)
                  .ValueGeneratedNever();

            entity.Property(c => c.Saldo);

            entity.Property(c => c.FechaCreacion)
                  .HasDefaultValueSql("GETDATE()");
        });

        // Transaccion
        modelBuilder.Entity<Transaccion>(entity =>
        {
            entity.HasKey(t => t.NumeroTransaccion);

            entity.Property(t => t.FechaTransaccion)
                  .HasDefaultValueSql("GETDATE()");

            // Relación con CuentaOrigen
            entity.HasOne(t => t.CuentaOrigen)
                  .WithMany()
                  .HasForeignKey(t => t.NumeroCuentaOrigen)
                  .OnDelete(DeleteBehavior.NoAction);

            // Relación con CuentaDestino
            entity.HasOne(t => t.CuentaDestino)
                  .WithMany()
                  .HasForeignKey(t => t.NumeroCuentaDestino)
                  .OnDelete(DeleteBehavior.NoAction);
        });
    }
}
