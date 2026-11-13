using Lab12Api.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Oculta los detalles internos del deserializador (tipos .NET) en los errores 400.
builder.Services.AddControllers()
    .AddJsonOptions(o => o.AllowInputFormatterExceptionMessages = false);
builder.Services.AddProblemDetails();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrWhiteSpace(connectionString) || connectionString.Contains("<definida en user-secrets>"))
    throw new InvalidOperationException(
        "Configura la cadena de conexión con: dotnet user-secrets set \"ConnectionStrings:DefaultConnection\" \"...\" (ver README).");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString, sql => sql.EnableRetryOnFailure()));

builder.Services.AddOpenApiDocument(config =>
{
    config.DocumentName = "v1";
    config.Title = "Laboratorio 12 - API de Gestión Académica";
    config.Version = "v1";
    config.Description = "API REST para gestionar instructores y estudiantes.";
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.UseSwaggerUi(config =>
    {
        config.DocumentTitle = "Laboratorio 12 - Swagger";
        config.Path = "/swagger";
        config.DocumentPath = "/swagger/{documentName}/swagger.json";
    });
}

app.UseExceptionHandler();
app.UseStatusCodePages();

app.MapControllers();

app.Run();
