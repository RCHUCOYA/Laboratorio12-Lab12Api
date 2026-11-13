namespace Lab12Api.Models;

public class Instructor
{
    public int IdInstructor { get; set; }
    public string Nombres { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;
    public string Especialidad { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool Activo { get; set; }

    public ICollection<Curso> Cursos { get; set; } = new List<Curso>();
}
