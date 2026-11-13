using Lab12Api.Data;
using Lab12Api.DTOs;
using Lab12Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lab12Api.Controllers;

[ApiController]
[Route("api/estudiantes")]
[Produces("application/json")]
public class EstudiantesController(ApplicationDbContext db) : ControllerBase
{
    /// <summary>Lista todos los estudiantes.</summary>
    [HttpGet]
    [ProducesResponseType<IEnumerable<EstudianteDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<EstudianteDto>>> GetAll()
    {
        var items = await db.Estudiantes.AsNoTracking()
            .OrderBy(e => e.IdEstudiante)
            .Select(e => ToDto(e))
            .ToListAsync();
        return Ok(items);
    }

    /// <summary>Obtiene un estudiante por su ID.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType<EstudianteDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EstudianteDto>> GetById(int id)
    {
        var estudiante = await db.Estudiantes.AsNoTracking().FirstOrDefaultAsync(e => e.IdEstudiante == id);
        return estudiante is null ? NotFoundProblem(id) : Ok(ToDto(estudiante));
    }

    /// <summary>Registra un nuevo estudiante. Si no se envía FechaRegistro, se usa la fecha actual (UTC).</summary>
    [HttpPost]
    [ProducesResponseType<EstudianteDto>(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<EstudianteDto>> Create(EstudianteCreateUpdateDto dto)
    {
        var estudiante = new Estudiante { FechaRegistro = dto.FechaRegistro ?? DateTime.UtcNow };
        Apply(estudiante, dto);
        db.Estudiantes.Add(estudiante);
        await db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = estudiante.IdEstudiante }, ToDto(estudiante));
    }

    /// <summary>Actualiza un estudiante existente. Si no se envía FechaRegistro, se conserva la original.</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, EstudianteCreateUpdateDto dto)
    {
        var estudiante = await db.Estudiantes.FindAsync(id);
        if (estudiante is null) return NotFoundProblem(id);

        Apply(estudiante, dto);
        if (dto.FechaRegistro.HasValue) estudiante.FechaRegistro = dto.FechaRegistro.Value;
        await db.SaveChangesAsync();
        return NoContent();
    }

    /// <summary>Elimina un estudiante. Devuelve 409 si tiene matrículas asociadas.</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Delete(int id)
    {
        var estudiante = await db.Estudiantes.FindAsync(id);
        if (estudiante is null) return NotFoundProblem(id);

        if (await db.Matriculas.AnyAsync(m => m.IdEstudiante == id))
            return ConflictProblem();

        db.Estudiantes.Remove(estudiante);
        try
        {
            await db.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return ConflictProblem();
        }
        return NoContent();
    }

    private static EstudianteDto ToDto(Estudiante e) =>
        new(e.IdEstudiante, e.Nombres, e.Apellidos, e.Email, e.Telefono, e.FechaRegistro, e.Activo);

    private static void Apply(Estudiante e, EstudianteCreateUpdateDto dto)
    {
        e.Nombres = dto.Nombres.Trim();
        e.Apellidos = dto.Apellidos.Trim();
        e.Email = dto.Email.Trim();
        e.Telefono = dto.Telefono?.Trim() ?? string.Empty;
        e.Activo = dto.Activo;
    }

    private ObjectResult NotFoundProblem(int id) =>
        Problem(statusCode: StatusCodes.Status404NotFound, title: "No encontrado",
            detail: $"No existe un estudiante con ID {id}.");

    private ObjectResult ConflictProblem() =>
        Problem(statusCode: StatusCodes.Status409Conflict, title: "Conflicto de integridad",
            detail: "No se puede eliminar el estudiante porque tiene matrículas asociadas.");
}
