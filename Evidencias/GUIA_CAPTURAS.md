# Guía de capturas — Laboratorio 12 (SQL Server + Docker + CloudBeaver)

Todas las capturas PNG de esta carpeta se generaron el 12/11/2026 con **resultados reales** de la API
conectada a **SQL Server 2022 en Docker** (`Lab12Db`).

## Capturas generadas

| Archivo | Contenido |
|---|---|
| `03-migracion-inicial.png` | `docker compose ps`, `dotnet ef migrations list`, `Migrations/`, `dotnet ef database update`, historial de migraciones, tablas y FKs en SQL Server (salida real de Terminal renderizada como imagen) |
| `04-base-datos-tablas.png` | CloudBeaver: esquema `dbo` de `Lab12Db` con las 5 tablas + `__EFMigrationsHistory` |
| `05-swagger-general.png` | Swagger UI con los 10 endpoints |
| `06` – `10c` | CRUD Instructor en Swagger (POST 201, GET 200, GET/{id} 200, PUT 204, GET tras PUT, DELETE 204, GET 404, POST inválido 400) |
| `11` – `15c` | CRUD Estudiante en Swagger (mismo flujo) |
| `16-compilacion-correcta.png` | `dotnet build`: 0 advertencias, 0 errores |
| `17-cloudbeaver-instructor-post.png` | `SELECT * FROM dbo.Instructor` tras el POST |
| `18-cloudbeaver-estudiante-post.png` | `SELECT * FROM dbo.Estudiante` tras el POST |
| `19-cloudbeaver-estudiante-put.png` | Estudiante modificado tras el PUT |
| `20-cloudbeaver-instructor-put.png` | Instructor modificado tras el PUT |
| `21-cloudbeaver-estudiante-antes-delete.png` | Estudiante temporal (Lucia) antes del DELETE |
| `22-cloudbeaver-estudiante-despues-delete.png` | El registro ya no existe tras el DELETE |

Respuestas JSON reales y verificación en SQL Server de cada paso (incluido el **409 Conflict**):
[respuestas/resumen.txt](./respuestas/resumen.txt).

## Código fuente

En lugar de capturas del proyecto en VS Code, el código completo (modelos, `ApplicationDbContext`, migraciones,
etc.) está en el repositorio público: <https://github.com/RCHUCOYA/Laboratorio12-Lab12Api>.

El informe con todas las capturas es [Informe_Laboratorio12_Evidencias.docx](./Informe_Laboratorio12_Evidencias.docx).

## Repetir capturas en vivo (opcional)

```zsh
cd ~/Projects/Laboratorio12/Lab12Api
docker compose up -d
dotnet run --launch-profile http
```

1. **Swagger** (<http://localhost:5080/swagger>): despliega el endpoint → *Try it out* → *Execute* → captura
   *Server response*.
2. **CloudBeaver** (<http://localhost:8978>): después de cada POST/PUT/DELETE ejecuta
   `SELECT * FROM dbo.Estudiante;` (o `dbo.Instructor`) y captura el resultado.
