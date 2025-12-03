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

### 1. Start Infrastructure
`ash
cd docker
docker compose up -d
`

Services started:
- Postgres (main application DB)
- Postgres (Keycloak DB)
- Keycloak (Realm import enabled)
- pgAdmin

### 2. Run the API
`ash
nx serve dotnet-web
`
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
`
apps/dotnet/web
├── Endpoints/
├── Infrastructure/
├── Services/
├── appsettings.json
└── README.md
`

## 🗄 Environment Variables
See /docker/.env for required variables (database passwords, Keycloak admin credentials, client secrets, etc.).

## 🧪 API Testing
Swagger UI available at:
`
http://localhost:4200/api
`

---
