#!/bin/zsh
# Registra la conexión "Lab12 - SQL Server" en CloudBeaver solo si aún no hay conexiones (no sobrescribe datos).
set -e
cd "$(dirname "$0")/.."
DEST=/opt/cloudbeaver/workspace/GlobalConfiguration/.dbeaver/data-sources.json
if docker exec lab12-cloudbeaver grep -q '"lab12-sqlserver"' "$DEST" 2>/dev/null; then
  echo "La conexión ya existe en CloudBeaver. No se modifica nada."; exit 0
fi
if docker exec lab12-cloudbeaver sh -c "grep -q '\"connections\": {}' $DEST"; then
  docker cp cloudbeaver/data-sources.json "lab12-cloudbeaver:$DEST"
  docker restart lab12-cloudbeaver >/dev/null
  echo "Conexión registrada. CloudBeaver reiniciado: http://localhost:8978"
else
  echo "CloudBeaver ya tiene otras conexiones; créala manualmente (ver README)."; exit 1
fi
