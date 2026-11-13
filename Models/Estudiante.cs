namespace Lab12Api.Models;

public class Estudiante
{
    public int IdEstudiante { get; set; }
    public string Nombres { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public DateTime FechaRegistro { get; set; }
    public bool Activo { get; set; }

    public ICollection<Matricula> Matriculas { get; set; } = new List<Matricula>();
}
