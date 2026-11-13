#!/bin/zsh
# Ejecuta una consulta T-SQL en el contenedor lab12-sqlserver (Lab12Db). Uso: ./scripts/sql.sh "SELECT ..."
cd "$(dirname "$0")/.." && source .env
docker exec -e SQLCMDPASSWORD="$MSSQL_SA_PASSWORD" lab12-sqlserver \
  /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -C -d Lab12Db -W -s " | " -Q "SET NOCOUNT ON; $1"
