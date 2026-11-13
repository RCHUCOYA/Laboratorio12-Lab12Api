using System.ComponentModel.DataAnnotations;

namespace Lab12Api.DTOs;

public record EstudianteDto(
    int IdEstudiante,
    string Nombres,
    string Apellidos,
    string Email,
    string Telefono,
    DateTime FechaRegistro,
    bool Activo);

public class EstudianteCreateUpdateDto
{
    [Required(ErrorMessage = "Los nombres son obligatorios.")]
    [StringLength(100, ErrorMessage = "Máximo 100 caracteres.")]
    public string Nombres { get; set; } = string.Empty;

    [Required(ErrorMessage = "Los apellidos son obligatorios.")]
    [StringLength(100, ErrorMessage = "Máximo 100 caracteres.")]
    public string Apellidos { get; set; } = string.Empty;

    [Required(ErrorMessage = "El email es obligatorio.")]
    [EmailAddress(ErrorMessage = "El email no tiene un formato válido.")]
    [StringLength(150, ErrorMessage = "Máximo 150 caracteres.")]
    public string Email { get; set; } = string.Empty;

    [StringLength(20, ErrorMessage = "Máximo 20 caracteres.")]
    public string Telefono { get; set; } = string.Empty;

    /// <summary>Opcional. Si no se envía al crear, se asigna la fecha actual en UTC.</summary>
    public DateTime? FechaRegistro { get; set; }

    public bool Activo { get; set; } = true;
}
