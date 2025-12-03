# CheeseGrater .NET Backend — Keycloak Integrated API

This is the primary ASP.NET Core 9 backend API inside the Nx monorepo.

## 🚀 Features

- Keycloak authentication via JWT (Keycloak.AuthServices)
- UMA resource-based authorization
- Custom owner-policy JS script in Keycloak
- Full Keycloak + Postgres dockerized environment
- Minimal API endpoint groups
- Clean Architecture with application, core, domain, and infrastructure layers

## 📦 Running the Backend

### 1. Start Infrastructure (run from repo root)

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

### 2. Run the API (run from repo root)

```bash
cd ../../../
nx serve dotnet-web
```

(or run through Nx Console)

API will auto-initialize:

- Database
- Keycloak seeds (Development only)

## 🔐 Authentication / Authorization

- JWT bearer authentication via Keycloak
- UMA resource protection using:
  - KeycloakProtectionClient
  - isOwnerPolicy.js custom policy
- Resource creation on TodoList creation
- Per-request authorization via MediatR behaviors

## 📁 Folder Structure

```
apps/dotnet/web
├── Endpoints/
├── Infrastructure/
├── Services/
├── appsettings.json
└── README.md
```

## 🗄 Environment Variables

See /docker/.env for required variables (database passwords, Keycloak admin credentials, client secrets, etc.).

## 🧪 API Testing

Swagger UI available at:
`http://localhost:4200/api`
