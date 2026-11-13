namespace Lab12Api.Models;

public class Curso
{
    public int IdCurso { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public decimal Precio { get; set; }
    public int DuracionHoras { get; set; }
    public int IdInstructor { get; set; }
    public bool Activo { get; set; }

    public Instructor? Instructor { get; set; }
    public ICollection<Matricula> Matriculas { get; set; } = new List<Matricula>();
}
