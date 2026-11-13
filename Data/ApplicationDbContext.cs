using Lab12Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Lab12Api.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Instructor> Instructores => Set<Instructor>();
    public DbSet<Estudiante> Estudiantes => Set<Estudiante>();
    public DbSet<Curso> Cursos => Set<Curso>();
    public DbSet<Matricula> Matriculas => Set<Matricula>();
    public DbSet<Pago> Pagos => Set<Pago>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Instructor>(e =>
        {
            e.ToTable("Instructor", "dbo");
            e.HasKey(x => x.IdInstructor);
            e.Property(x => x.Nombres).IsRequired().HasMaxLength(100);
            e.Property(x => x.Apellidos).IsRequired().HasMaxLength(100);
            e.Property(x => x.Especialidad).IsRequired().HasMaxLength(150);
            e.Property(x => x.Email).IsRequired().HasMaxLength(150);
        });

        modelBuilder.Entity<Estudiante>(e =>
        {
            e.ToTable("Estudiante", "dbo");
            e.HasKey(x => x.IdEstudiante);
            e.Property(x => x.Nombres).IsRequired().HasMaxLength(100);
            e.Property(x => x.Apellidos).IsRequired().HasMaxLength(100);
            e.Property(x => x.Email).IsRequired().HasMaxLength(150);
            e.Property(x => x.Telefono).HasMaxLength(20);
            // SQL Server (datetime2) no guarda DateTimeKind; se marca como UTC al leer.
            e.Property(x => x.FechaRegistro)
                .HasConversion(v => v, v => DateTime.SpecifyKind(v, DateTimeKind.Utc));
        });

        modelBuilder.Entity<Curso>(e =>
        {
            e.ToTable("Curso", "dbo");
            e.HasKey(x => x.IdCurso);
            e.Property(x => x.Nombre).IsRequired().HasMaxLength(150);
            e.Property(x => x.Descripcion).HasMaxLength(500);
            e.Property(x => x.Precio).HasPrecision(10, 2);

            // Instructor 1:N Curso
            e.HasOne(x => x.Instructor)
                .WithMany(i => i.Cursos)
                .HasForeignKey(x => x.IdInstructor)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Matricula>(e =>
        {
            e.ToTable("Matricula", "dbo");
            e.HasKey(x => x.IdMatricula);
            e.Property(x => x.Estado).IsRequired().HasMaxLength(50);
            e.Property(x => x.MontoTotal).HasPrecision(10, 2);

            // Estudiante 1:N Matricula
            e.HasOne(x => x.Estudiante)
                .WithMany(s => s.Matriculas)
                .HasForeignKey(x => x.IdEstudiante)
                .OnDelete(DeleteBehavior.Restrict);

            // Curso 1:N Matricula
            e.HasOne(x => x.Curso)
                .WithMany(c => c.Matriculas)
                .HasForeignKey(x => x.IdCurso)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Pago>(e =>
        {
            e.ToTable("Pago", "dbo");
            e.HasKey(x => x.IdPago);
            e.Property(x => x.Monto).HasPrecision(10, 2);
            e.Property(x => x.MetodoPago).IsRequired().HasMaxLength(50);
            e.Property(x => x.EstadoPago).IsRequired().HasMaxLength(50);

            // Matricula 1:N Pago
            e.HasOne(x => x.Matricula)
                .WithMany(m => m.Pagos)
                .HasForeignKey(x => x.IdMatricula)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
