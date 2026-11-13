#!/bin/zsh
# Guarda la cadena de conexión de SQL Server en dotnet user-secrets usando la contraseña de .env.
set -e
cd "$(dirname "$0")/.."
[[ -f .env ]] || { echo "Falta .env (copia .env.example a .env y define las contraseñas)."; exit 1; }
source .env
dotnet user-secrets set "ConnectionStrings:DefaultConnection" \
  "Server=localhost,1433;Database=Lab12Db;User Id=sa;Password=$MSSQL_SA_PASSWORD;TrustServerCertificate=True;Encrypt=True" >/dev/null
echo "Cadena de conexión guardada en user-secrets."
