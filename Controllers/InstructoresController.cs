using Lab12Api.Data;
using Lab12Api.DTOs;
using Lab12Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lab12Api.Controllers;

[ApiController]
[Route("api/instructores")]
[Produces("application/json")]
public class InstructoresController(ApplicationDbContext db) : ControllerBase
{
    /// <summary>Lista todos los instructores.</summary>
    [HttpGet]
    [ProducesResponseType<IEnumerable<InstructorDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<InstructorDto>>> GetAll()
    {
        var items = await db.Instructores.AsNoTracking()
            .OrderBy(i => i.IdInstructor)
            .Select(i => ToDto(i))
            .ToListAsync();
        return Ok(items);
    }

    /// <summary>Obtiene un instructor por su ID.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType<InstructorDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<InstructorDto>> GetById(int id)
    {
        var instructor = await db.Instructores.AsNoTracking().FirstOrDefaultAsync(i => i.IdInstructor == id);
        return instructor is null ? NotFoundProblem(id) : Ok(ToDto(instructor));
    }

    /// <summary>Registra un nuevo instructor.</summary>
    [HttpPost]
    [ProducesResponseType<InstructorDto>(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<InstructorDto>> Create(InstructorCreateUpdateDto dto)
    {
        var instructor = new Instructor();
        Apply(instructor, dto);
        db.Instructores.Add(instructor);
        await db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = instructor.IdInstructor }, ToDto(instructor));
    }

    /// <summary>Actualiza un instructor existente.</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, InstructorCreateUpdateDto dto)
    {
        var instructor = await db.Instructores.FindAsync(id);
        if (instructor is null) return NotFoundProblem(id);

        Apply(instructor, dto);
        await db.SaveChangesAsync();
        return NoContent();
    }

    /// <summary>Elimina un instructor. Devuelve 409 si tiene cursos asociados.</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Delete(int id)
    {
        var instructor = await db.Instructores.FindAsync(id);
        if (instructor is null) return NotFoundProblem(id);

        if (await db.Cursos.AnyAsync(c => c.IdInstructor == id))
            return ConflictProblem();

        db.Instructores.Remove(instructor);
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

    private static InstructorDto ToDto(Instructor i) =>
        new(i.IdInstructor, i.Nombres, i.Apellidos, i.Especialidad, i.Email, i.Activo);

    private static void Apply(Instructor i, InstructorCreateUpdateDto dto)
    {
        i.Nombres = dto.Nombres.Trim();
        i.Apellidos = dto.Apellidos.Trim();
        i.Especialidad = dto.Especialidad.Trim();
        i.Email = dto.Email.Trim();
        i.Activo = dto.Activo;
    }

    private ObjectResult NotFoundProblem(int id) =>
        Problem(statusCode: StatusCodes.Status404NotFound, title: "No encontrado",
            detail: $"No existe un instructor con ID {id}.");

    private ObjectResult ConflictProblem() =>
        Problem(statusCode: StatusCodes.Status409Conflict, title: "Conflicto de integridad",
            detail: "No se puede eliminar el instructor porque tiene cursos asociados.");
}
