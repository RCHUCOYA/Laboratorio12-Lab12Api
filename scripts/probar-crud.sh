#!/bin/zsh
# Ejecuta el CRUD completo contra la API (http://localhost:5080) y verifica cada paso en SQL Server (Lab12Db).
# Guarda las respuestas reales en Evidencias/respuestas/.
set -u
ROOT="$(cd "$(dirname "$0")/.." && pwd)"
BASE="${BASE_URL:-http://localhost:5080}"
OUT="$ROOT/Evidencias/respuestas"
SQL="$ROOT/scripts/sql.sh"
mkdir -p "$OUT"
: > "$OUT/resumen.txt"

log() { echo "$@" | tee -a "$OUT/resumen.txt"; }
call() { # nombre metodo url [json]
  local name=$1 method=$2 url=$3 body=${4:-}
  local args=(-s -o "$OUT/$name.json" -w "%{http_code}" -X "$method" "$BASE$url")
  [[ -n "$body" ]] && args+=(-H "Content-Type: application/json" -d "$body")
  local code=$(curl "${args[@]}")
  log "$(printf "%-36s %-6s %-26s -> HTTP %s" "$name" "$method" "$url" "$code")"
}
db() { log "   [SQL Server] $1"; "$SQL" "$1" | sed 's/^/     /' | tee -a "$OUT/resumen.txt"; }

log "Pruebas ejecutadas: $(date)"
log "== INSTRUCTOR =="
call 06-instructor-post POST /api/instructores '{"nombres":"Carlos","apellidos":"Ramirez","especialidad":"Desarrollo de Software","email":"carlos@example.com","activo":true}'
ID=$(sed -E 's/.*"idInstructor":([0-9]+).*/\1/' "$OUT/06-instructor-post.json")
db "SELECT IdInstructor, Nombres, Apellidos, Especialidad, Email, Activo FROM dbo.Instructor WHERE IdInstructor = $ID;"
call 07-instructor-get GET /api/instructores
call 08-instructor-get-id GET /api/instructores/$ID
call 09-instructor-put PUT /api/instructores/$ID '{"nombres":"Carlos Alberto","apellidos":"Ramirez","especialidad":"Arquitectura de Software","email":"carlos.ramirez@example.com","activo":true}'
call 09b-instructor-get-id-tras-put GET /api/instructores/$ID
db "SELECT IdInstructor, Nombres, Especialidad, Email FROM dbo.Instructor WHERE IdInstructor = $ID;"
call 10-instructor-delete DELETE /api/instructores/$ID
call 10b-instructor-get-id-tras-delete GET /api/instructores/$ID
db "SELECT COUNT(*) AS Existe FROM dbo.Instructor WHERE IdInstructor = $ID;"
call instructor-post-invalido POST /api/instructores '{"nombres":"","apellidos":"","especialidad":"","email":"correo-invalido","activo":true}'

log "== ESTUDIANTE =="
call 11-estudiante-post POST /api/estudiantes '{"nombres":"Rafael","apellidos":"Chuco","email":"rafael@example.com","telefono":"987654321","activo":true}'
ID=$(sed -E 's/.*"idEstudiante":([0-9]+).*/\1/' "$OUT/11-estudiante-post.json")
db "SELECT IdEstudiante, Nombres, Apellidos, Email, Telefono, FechaRegistro, Activo FROM dbo.Estudiante WHERE IdEstudiante = $ID;"
call 12-estudiante-get GET /api/estudiantes
call 13-estudiante-get-id GET /api/estudiantes/$ID
call 14-estudiante-put PUT /api/estudiantes/$ID '{"nombres":"Rafael","apellidos":"Chuco Yarasca","email":"rafael.chuco@example.com","telefono":"912345678","activo":false}'
call 14b-estudiante-get-id-tras-put GET /api/estudiantes/$ID
db "SELECT IdEstudiante, Apellidos, Email, Telefono, Activo FROM dbo.Estudiante WHERE IdEstudiante = $ID;"
call 15-estudiante-delete DELETE /api/estudiantes/$ID
call 15b-estudiante-get-id-tras-delete GET /api/estudiantes/$ID
db "SELECT COUNT(*) AS Existe FROM dbo.Estudiante WHERE IdEstudiante = $ID;"
call estudiante-post-invalido POST /api/estudiantes '{"nombres":"","apellidos":"","email":"no-es-email","telefono":"987654321","activo":true}'
call estudiante-post-fecha-invalida POST /api/estudiantes '{"nombres":"Ana","apellidos":"Diaz","email":"ana@example.com","telefono":"900000000","fechaRegistro":"fecha-mala","activo":true}'

log "== INTEGRIDAD (409) =="
call conflicto-instructor-post POST /api/instructores '{"nombres":"Ana","apellidos":"Lopez","especialidad":"Base de Datos","email":"ana@example.com","activo":true}'
ID=$(sed -E 's/.*"idInstructor":([0-9]+).*/\1/' "$OUT/conflicto-instructor-post.json")
"$SQL" "INSERT INTO dbo.Curso (Nombre, Descripcion, Precio, DuracionHoras, IdInstructor, Activo) VALUES ('SQL Server', 'Curso de prueba', 150.00, 40, $ID, 1);" >/dev/null
call instructor-delete-conflicto-409 DELETE /api/instructores/$ID
"$SQL" "DELETE FROM dbo.Curso WHERE IdInstructor = $ID;" >/dev/null
call conflicto-limpieza DELETE /api/instructores/$ID
