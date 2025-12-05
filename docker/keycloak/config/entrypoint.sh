#!/bin/bash
set -euo pipefail

TEMPLATE=/opt/keycloak-config/templates/master-realm.yaml.template
OUTPUT=/opt/keycloak-config/generated/master-realm.yaml

echo "Rendering Keycloak config template -> $OUTPUT"
envsubst < "$TEMPLATE" > "$OUTPUT"

echo "Rendered config:"
cat "$OUTPUT"

echo "Waiting for Keycloak health endpoint..."
until curl -sf "${KEYCLOAK_URL}/realms/master" > /dev/null; do
  echo "Keycloak not ready, retrying..."
  sleep 3
done

echo "Applying declarative configuration..."
exec java -jar /app/keycloak-config-cli.jar --import.files.locations="$OUTPUT"
