# CheeseGrater .NET Backend — Keycloak Integrated API

This is the primary ASP.NET Core 9 backend API inside the Nx monorepo.

## Purpose of This Monorepo

1. **Reusable Base Platform** – common identity, data, and tooling you can lift into new services.
2. **Core Service Library** – shared domain/application/infrastructure packages.
3. **Showcase & Demo Environment** – runnable stack to demonstrate Keycloak + UMA integration.

## Features

- Keycloak authentication via JWT (Keycloak.AuthServices)
- UMA resource-based authorization
- Custom owner-policy JS script in Keycloak
- Full Keycloak + Postgres dockerized environment
- Minimal API endpoint groups
- Clean Architecture with application, core, domain, and infrastructure layers

## Running the Backend

### 1) Start Infrastructure (from repo root)

```bash
cd ../../../   # must start at repository root
cd docker
docker compose up -d
```

Services started:

- Postgres (main application DB)
- Postgres (Keycloak DB)
- Keycloak (Realm import enabled)
- pgAdmin

Keycloak-related links:

- Keycloak container configuration: [docker/docker-compose.yaml](../../docker/docker-compose.yaml)
- Realm import + custom scripts: [docker/keycloak/imports/realm-export.json](../../docker/keycloak/imports/realm-export.json) and [docker/keycloak/providers/isOwnerPolicy.js](../../docker/keycloak/providers/isOwnerPolicy.js)
- Local Keycloak UI: [http://localhost:8081/admin/master/console](http://localhost:8081/admin/master/console)
- Backend Keycloak services:
  - Service registration & UMA wiring: [libs/dotnet/core/infrastructure/DependencyInjection.cs](../../../libs/dotnet/core/infrastructure/DependencyInjection.cs)
  - Keycloak seeding/initialization service: [libs/dotnet/infrastructure/Identity/IdentityServiceInitializer.cs](../../../libs/dotnet/infrastructure/Identity/IdentityServiceInitializer.cs)
  - Resource creation per Todo list: [libs/dotnet/application/TodoLists/Commands/CreateTodoList/CreateTodoList.cs](../../../libs/dotnet/application/TodoLists/Commands/CreateTodoList/CreateTodoList.cs)
  - Identity abstraction & policy enforcement helper: [libs/dotnet/core/infrastructure/Identity/IdentityService.cs](../../../libs/dotnet/core/infrastructure/Identity/IdentityService.cs)
  - Keycloak admin client request sanitizer: [libs/dotnet/core/infrastructure/Identity/RequestBodyFixHandler.cs](../../../libs/dotnet/core/infrastructure/Identity/RequestBodyFixHandler.cs)

## Dual IAM Initialization Modes

- **Declarative Mode (recommended)** – `KEYCLOAK_BOOTSTRAP_MODE=off` builds a config image that renders `docker/keycloak/config/master-realm.yaml.template` with env vars, then runs keycloak-config-cli against the generated YAML for reproducible admin client setup.
- **Bootstrap Mode (fast dev)** – `KEYCLOAK_BOOTSTRAP_MODE=on` runs `/opt/keycloak/bootstrap/create-admin-client.sh` inside the Keycloak container to create/update the admin service account client and secret.

### Architecture (ASCII)

```
Monorepo
 ├── Core Libraries
 ├── Sample Apps
 ├── Identity Infrastructure
 │     ├── Declarative Mode
 │     └── Bootstrap Mode
 └── Docker Stack
```

### Keycloak usage in this backend

- UMA protection and protected resource policies enforced in handlers and MediatR behaviors.
- Admin API access via the `web-admin-serviceaccount` service account client (deterministic secret).
- Admin client creation happens either declaratively (config file) or via bootstrap script to keep NSwag/seed flows consistent.

## Run the API (from repo root)

```bash
cd ../../../
nx serve dotnet-web
```

API will auto-initialize:

- Database
- Keycloak seeds (Development only)

## Authentication / Authorization

- JWT bearer authentication via Keycloak
- UMA resource protection using the KeycloakProtectionClient
- Per-request authorization via MediatR behaviors and UMA policies

## Folder Structure

```
apps/dotnet/web
├── Endpoints/
├── Infrastructure/
├── Services/
├── appsettings.json
└── README.md
```

## Environment Variables

See `/docker/.env.example` for required variables (database passwords, Keycloak admin credentials, client secrets, bootstrap toggle).

## API Testing

Swagger UI: `http://localhost:4200/api`
