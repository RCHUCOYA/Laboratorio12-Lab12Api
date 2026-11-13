using System.ComponentModel.DataAnnotations;

namespace Lab12Api.DTOs;

public record InstructorDto(
    int IdInstructor,
    string Nombres,
    string Apellidos,
    string Especialidad,
    string Email,
    bool Activo);

public class InstructorCreateUpdateDto
{
    [Required(ErrorMessage = "Los nombres son obligatorios.")]
    [StringLength(100, ErrorMessage = "Máximo 100 caracteres.")]
    public string Nombres { get; set; } = string.Empty;

    [Required(ErrorMessage = "Los apellidos son obligatorios.")]
    [StringLength(100, ErrorMessage = "Máximo 100 caracteres.")]
    public string Apellidos { get; set; } = string.Empty;

    [Required(ErrorMessage = "La especialidad es obligatoria.")]
    [StringLength(150, ErrorMessage = "Máximo 150 caracteres.")]
    public string Especialidad { get; set; } = string.Empty;

    [Required(ErrorMessage = "El email es obligatorio.")]
    [EmailAddress(ErrorMessage = "El email no tiene un formato válido.")]
    [StringLength(150, ErrorMessage = "Máximo 150 caracteres.")]
    public string Email { get; set; } = string.Empty;

    public bool Activo { get; set; } = true;
}
