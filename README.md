# Laboratorio 12 — API de Gestión Académica

API REST con **ASP.NET Core Web API (.NET 10)**, **Entity Framework Core 10** con **migraciones**,
**Microsoft SQL Server 2022** en **Docker**, administración visual con **CloudBeaver** y documentación
**Swagger UI (NSwag)**. Desarrollada y probada en **macOS** con VS Code.

## 1. Objetivo del laboratorio

1. Crear una API usando migraciones.
2. Implementar Swagger.
3. Crear los cinco modelos del diagrama (Instructor, Estudiante, Curso, Matricula, Pago).
4. Implementar el CRUD completo de Instructor y Estudiante, con evidencias.

## 2. Arquitectura

```
macOS + VS Code
 └─ ASP.NET Core Web API + Swagger  ──►  http://localhost:5080/swagger
        │  EF Core (SqlServer) · puerto 1433
        ▼
 Docker Compose (proyecto "lab12")
 ├─ lab12-sqlserver   SQL Server 2022 Developer · BD Lab12Db  (127.0.0.1:1433)
 └─ lab12-cloudbeaver CloudBeaver Community                    (http://localhost:8978)
```

| Componente | Tecnología |
|---|---|
| Backend | C# / ASP.NET Core Web API con controladores (.NET 10) |
| ORM y migraciones | EF Core 10.0.15 (`Microsoft.EntityFrameworkCore.SqlServer`, `.Design`) |
| Base de datos | SQL Server 2022 Developer (imagen `mcr.microsoft.com/mssql/server:2022-latest`) |
| Infraestructura | Docker Desktop + Docker Compose |
| Administración BD | CloudBeaver Community (`dbeaver/cloudbeaver`) |
| Documentación | NSwag.AspNetCore 14.8 (OpenAPI 3 + Swagger UI) |

### ⚠️ Nota sobre Apple Silicon

Este Mac es **Apple M4 Pro (`arm64`)**. SQL Server solo publica imágenes **x86-64**, por eso el
`docker-compose.yml` usa `platform: linux/amd64` y Docker Desktop lo ejecuta con **emulación Rosetta**.
Microsoft **no soporta oficialmente** esta configuración. En este equipo funcionó correctamente
(SQL Server 2022 CU28 arranca, la migración se aplica y todo el CRUD pasa), pero si en otro Mac falla, la
alternativa es usar un SQL Server remoto (Azure SQL u otro servidor x86-64) cambiando solo la cadena de
conexión; CloudBeaver y la API siguen igual.

## 3. Requisitos (macOS)

```zsh
uname -m                                  # arm64 (Apple Silicon) o x86_64 (Intel)
dotnet --version                          # 10.x
dotnet tool install --global dotnet-ef --version "10.*"
export PATH="$PATH:$HOME/.dotnet/tools"   # añádelo a ~/.zshrc
docker --version && docker compose version
```

Docker Desktop debe estar abierto. Si `docker` no se encuentra en la Terminal:
`export PATH="$PATH:/Applications/Docker.app/Contents/Resources/bin"`.

## 4. Puesta en marcha (paso a paso)

```zsh
cd ~/Projects/Laboratorio12/Lab12Api

# 1) Credenciales locales (excluidas de Git). Solo la primera vez:
cp .env.example .env          # edita .env y define contraseñas fuertes
./scripts/configurar-secretos.sh   # guarda la cadena de conexión en dotnet user-secrets

# 2) Contenedores
docker compose up -d
docker compose ps             # lab12-sqlserver debe aparecer "(healthy)"

# 3) Base de datos (crea Lab12Db y sus tablas mediante la migración)
dotnet restore
dotnet build
dotnet ef database update
dotnet ef migrations list

# 4) Conexión de CloudBeaver (solo la primera vez; no sobrescribe datos)
./scripts/configurar-cloudbeaver.sh

# 5) API
dotnet run --launch-profile http
```

| Servicio | URL |
|---|---|
| Swagger UI | <http://localhost:5080/swagger> |
| OpenAPI JSON | <http://localhost:5080/swagger/v1/swagger.json> |
| CloudBeaver | <http://localhost:8978> |

### Gestión de credenciales

- `.env` (no versionado): `MSSQL_SA_PASSWORD`, `CB_ADMIN_NAME`, `CB_ADMIN_PASSWORD`.
- La API lee la cadena de conexión desde **dotnet user-secrets** (fuera del proyecto).
  `appsettings.json` solo contiene una plantilla sin contraseña; si no se configuró el secreto, la API
  muestra un mensaje indicando cómo hacerlo.
- Los puertos se publican solo en `127.0.0.1` (no accesibles desde la red).

### Persistencia de datos

Los datos viven en los volúmenes `lab12_sqlserver_data` y `lab12_cloudbeaver_data`.
`docker compose down` / `up -d` conserva la información. **No ejecutes `docker compose down -v`** si quieres
conservarla (borra los volúmenes).

## 5. Estructura del proyecto

```
Lab12Api/
├── Controllers/                    # InstructoresController, EstudiantesController
├── Models/                         # Instructor, Estudiante, Curso, Matricula, Pago
├── Data/ApplicationDbContext.cs    # DbSets + Fluent API (tablas dbo, PK, FK, Restrict)
├── DTOs/                           # DTOs de lectura y de creación/actualización con validaciones
├── Migrations/                     # InitialCreate (SQL Server)
├── cloudbeaver/data-sources.json   # Conexión preconfigurada para CloudBeaver
├── scripts/
│   ├── configurar-secretos.sh      # .env → user-secrets
│   ├── configurar-cloudbeaver.sh   # registra la conexión en CloudBeaver
│   ├── probar-crud.sh              # CRUD completo + verificación en SQL Server
│   └── sql.sh                      # ejecuta T-SQL en el contenedor (sqlcmd)
├── Evidencias/                     # capturas PNG, respuestas JSON, guía
├── docker-compose.yml
├── .env.example                    # plantilla de credenciales (.env real no se versiona)
├── appsettings.json
├── Program.cs
└── README.md
```

## 6. Modelos

| Entidad | Propiedades |
|---|---|
| **Instructor** | IdInstructor (PK), Nombres, Apellidos, Especialidad, Email, Activo |
| **Estudiante** | IdEstudiante (PK), Nombres, Apellidos, Email, Telefono, FechaRegistro, Activo |
| **Curso** | IdCurso (PK), Nombre, Descripcion, Precio, DuracionHoras, IdInstructor (FK), Activo |
| **Matricula** | IdMatricula (PK), IdEstudiante (FK), IdCurso (FK), FechaMatricula, Estado, MontoTotal |
| **Pago** | IdPago (PK), IdMatricula (FK), Monto, FechaPago, MetodoPago, EstadoPago |

Tipos en SQL Server: `int IDENTITY` para PK, `nvarchar(n)` para textos, `decimal(10,2)` para montos,
`datetime2` para fechas y `bit` para `Activo`. Se añadieron propiedades de navegación para EF Core.

## 7. Relaciones (Fluent API)

| Relación | Clave foránea | Al eliminar |
|---|---|---|
| Instructor **1:N** Curso | `Curso.IdInstructor` | Restrict |
| Estudiante **1:N** Matricula | `Matricula.IdEstudiante` | Restrict |
| Curso **1:N** Matricula | `Matricula.IdCurso` | Restrict |
| Matricula **1:N** Pago | `Pago.IdMatricula` | Restrict |

`DeleteBehavior.Restrict` se traduce en SQL Server como `ON DELETE NO ACTION`: no se puede borrar un
registro padre que tenga hijos. La API responde **409 Conflict** en ese caso.

## 8. Migraciones

```zsh
dotnet ef migrations add InitialCreate   # ya generada en Migrations/
dotnet ef database update                # crea Lab12Db y aplica la migración
dotnet ef migrations list                # 20261113000230_InitialCreate
./scripts/sql.sh "SELECT * FROM __EFMigrationsHistory;"
```

No se usa `EnsureCreated()` ni scripts SQL manuales.

## 9. Endpoints

| Método | Ruta | Descripción | Respuestas |
|---|---|---|---|
| GET | `/api/instructores` | Lista instructores | 200 |
| GET | `/api/instructores/{id}` | Busca instructor por ID | 200, 404 |
| POST | `/api/instructores` | Registra instructor | 201, 400 |
| PUT | `/api/instructores/{id}` | Actualiza instructor | 204, 400, 404 |
| DELETE | `/api/instructores/{id}` | Elimina instructor | 204, 404, 409 |
| GET | `/api/estudiantes` | Lista estudiantes | 200 |
| GET | `/api/estudiantes/{id}` | Busca estudiante por ID | 200, 404 |
| POST | `/api/estudiantes` | Registra estudiante | 201, 400 |
| PUT | `/api/estudiantes/{id}` | Actualiza estudiante | 204, 400, 404 |
| DELETE | `/api/estudiantes/{id}` | Elimina estudiante | 204, 404, 409 |

**Validaciones:** nombres, apellidos (y especialidad en Instructor) obligatorios; email con formato válido;
teléfono texto (máx. 20); `fechaRegistro` debe ser una fecha válida. Si `fechaRegistro` no se envía al
**crear**, se asigna la fecha actual en **UTC**; si no se envía al **actualizar**, se conserva la original.
Los errores se devuelven como *ProblemDetails* sin exponer excepciones internas.

### Ejemplos

`POST /api/instructores`

```json
{ "nombres": "Carlos", "apellidos": "Ramirez", "especialidad": "Desarrollo de Software", "email": "carlos@example.com", "activo": true }
```

`POST /api/estudiantes`

```json
{ "nombres": "Rafael", "apellidos": "Chuco", "email": "rafael@example.com", "telefono": "987654321", "activo": true }
```

Respuestas reales: ver [Evidencias/respuestas/](Evidencias/respuestas/) (201, 200, 204, 404, 400 y 409).

## 10. Verificación en CloudBeaver

1. Abre <http://localhost:8978> → **Settings (⚙) → Login** con `CB_ADMIN_NAME` / `CB_ADMIN_PASSWORD` de `.env`.
2. En el panel izquierdo aparece **Shared → Lab12 - SQL Server (Lab12Db)**. Expándelo e ingresa usuario
   `sa` y la contraseña `MSSQL_SA_PASSWORD` (no se guarda en disco).
3. Abre el editor SQL (botón **SQL**) y ejecuta:

```sql
SELECT * FROM dbo.Instructor;
SELECT * FROM dbo.Estudiante;
```

Si necesitas crear la conexión a mano: *New Connection → SQL Server*, Host `sqlserver`, Puerto `1433`,
Base de datos `Lab12Db`, Usuario `sa`, y en *Driver properties* `trustServerCertificate=true`.

## 11. Pruebas automáticas

Con los contenedores y la API en ejecución:

```zsh
./scripts/probar-crud.sh
```

Ejecuta POST → GET → GET/{id} → PUT → GET/{id} → DELETE → GET/{id} (404) → POST inválido (400) para cada
entidad, consulta SQL Server después de cada cambio y prueba el **409** (instructor con curso).
Resultado: [Evidencias/respuestas/resumen.txt](Evidencias/respuestas/resumen.txt).

## 12. Detener / reanudar

```zsh
# API: Ctrl+C en su terminal
docker compose stop        # detiene contenedores conservando datos
docker compose start       # los vuelve a iniciar
```

## 13. Evidencias

Ver [Evidencias/GUIA_CAPTURAS.md](Evidencias/GUIA_CAPTURAS.md).
