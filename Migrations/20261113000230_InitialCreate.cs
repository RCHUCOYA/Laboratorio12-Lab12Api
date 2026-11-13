using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lab12Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dbo");

            migrationBuilder.CreateTable(
                name: "Estudiante",
                schema: "dbo",
                columns: table => new
                {
                    IdEstudiante = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombres = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Apellidos = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Telefono = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Estudiante", x => x.IdEstudiante);
                });

            migrationBuilder.CreateTable(
                name: "Instructor",
                schema: "dbo",
                columns: table => new
                {
                    IdInstructor = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombres = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Apellidos = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Especialidad = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Instructor", x => x.IdInstructor);
                });

            migrationBuilder.CreateTable(
                name: "Curso",
                schema: "dbo",
                columns: table => new
                {
                    IdCurso = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Precio = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    DuracionHoras = table.Column<int>(type: "int", nullable: false),
                    IdInstructor = table.Column<int>(type: "int", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Curso", x => x.IdCurso);
                    table.ForeignKey(
                        name: "FK_Curso_Instructor_IdInstructor",
                        column: x => x.IdInstructor,
                        principalSchema: "dbo",
                        principalTable: "Instructor",
                        principalColumn: "IdInstructor",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Matricula",
                schema: "dbo",
                columns: table => new
                {
                    IdMatricula = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdEstudiante = table.Column<int>(type: "int", nullable: false),
                    IdCurso = table.Column<int>(type: "int", nullable: false),
                    FechaMatricula = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MontoTotal = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Matricula", x => x.IdMatricula);
                    table.ForeignKey(
                        name: "FK_Matricula_Curso_IdCurso",
                        column: x => x.IdCurso,
                        principalSchema: "dbo",
                        principalTable: "Curso",
                        principalColumn: "IdCurso",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Matricula_Estudiante_IdEstudiante",
                        column: x => x.IdEstudiante,
                        principalSchema: "dbo",
                        principalTable: "Estudiante",
                        principalColumn: "IdEstudiante",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Pago",
                schema: "dbo",
                columns: table => new
                {
                    IdPago = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdMatricula = table.Column<int>(type: "int", nullable: false),
                    Monto = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    FechaPago = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MetodoPago = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EstadoPago = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pago", x => x.IdPago);
                    table.ForeignKey(
                        name: "FK_Pago_Matricula_IdMatricula",
                        column: x => x.IdMatricula,
                        principalSchema: "dbo",
                        principalTable: "Matricula",
                        principalColumn: "IdMatricula",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Curso_IdInstructor",
                schema: "dbo",
                table: "Curso",
                column: "IdInstructor");

            migrationBuilder.CreateIndex(
                name: "IX_Matricula_IdCurso",
                schema: "dbo",
                table: "Matricula",
                column: "IdCurso");

            migrationBuilder.CreateIndex(
                name: "IX_Matricula_IdEstudiante",
                schema: "dbo",
                table: "Matricula",
                column: "IdEstudiante");

            migrationBuilder.CreateIndex(
                name: "IX_Pago_IdMatricula",
                schema: "dbo",
                table: "Pago",
                column: "IdMatricula");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Pago",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Matricula",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Curso",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Estudiante",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Instructor",
                schema: "dbo");
        }
    }
}
