#!/bin/bash
set -euo pipefail

: "${KEYCLOAK_ADMIN:?Missing KEYCLOAK_ADMIN}"
: "${KEYCLOAK_ADMIN_PASSWORD:?Missing KEYCLOAK_ADMIN_PASSWORD}"
: "${KEYCLOAK_ADMIN_CLIENT_ID:=web-admin-serviceaccount}"

KC_BIN="/opt/keycloak/bin/kcadm.sh"
KC_HOST="${KC_HOST:-http://localhost:8080}"
REALM="master"

echo "Waiting for Keycloak to accept admin credentials..."
until $KC_BIN config credentials \
  --server "$KC_HOST" \
  --realm "$REALM" \
  --user "$KEYCLOAK_ADMIN" \
  --password "$KEYCLOAK_ADMIN_PASSWORD" >/dev/null 2>&1; do
  echo "Keycloak not ready yet, retrying in 3s..."
  sleep 3
done

# Create client if it does not exist
CLIENT_ID=$($KC_BIN get clients -r "$REALM" -q clientId="$KEYCLOAK_ADMIN_CLIENT_ID" --fields id --format csv | tail -n 1 | tr -d '\r')
if [[ "$CLIENT_ID" == "id" ]]; then
  CLIENT_ID=""
fi

if [[ -z "$CLIENT_ID" ]]; then
  $KC_BIN create clients -r "$REALM" \
    -s clientId="$KEYCLOAK_ADMIN_CLIENT_ID" \
    -s enabled=true \
    -s serviceAccountsEnabled=true \
    -s publicClient=false \
    -s standardFlowEnabled=false \
    -s directAccessGrantsEnabled=false \
    -s protocol="openid-connect" >/dev/null 2>&1 || true
  CLIENT_ID=$($KC_BIN get clients -r "$REALM" -q clientId="$KEYCLOAK_ADMIN_CLIENT_ID" --fields id --format csv | tail -n 1 | tr -d '\r')
fi

# Add audience mapper (idempotent)
AUDIENCE_MAPPER_NAME="aud-security-admin-console"
EXISTING_MAPPER=$($KC_BIN get clients/"$CLIENT_ID"/protocol-mappers/models -r "$REALM" --fields name --format csv | tail -n +2 | grep -F "$AUDIENCE_MAPPER_NAME" || true)
if [[ -z "${EXISTING_MAPPER:-}" ]]; then
  $KC_BIN create clients/"$CLIENT_ID"/protocol-mappers/models -r "$REALM" \
    -s name="$AUDIENCE_MAPPER_NAME" \
    -s protocol="openid-connect" \
    -s protocolMapper="oidc-audience-mapper" \
    -s 'config."included.client.audience"="security-admin-console"' \
    -s 'config."id.token.claim"="true"' \
    -s 'config."access.token.claim"="true"' >/dev/null 2>&1 || true
fi

# Grant admin realm role to the service account
SERVICE_ACCOUNT="service-account-$KEYCLOAK_ADMIN_CLIENT_ID"
$KC_BIN add-roles --uusername "$SERVICE_ACCOUNT" -r "$REALM" --rolename admin >/dev/null 2>&1 || true

