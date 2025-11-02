# CheeseGrater

## Prerequisites

- Docker Desktop
- NxConsole for VSCode
- NodeJs
- Angular
- ASP.NET Core 9

## Installation

- `cd docker`
- `docker compose up -d`
  - This sets up all the critical infrastructure needed by backend apps

## Usage

> This is for default configuration. If you change configs like ports in the docker compose, you'd will need to use the ports you defined to access them

- Keycloak `http://localhost:8081`
- pgAdmin for main postgres instance (project has one that is used by all services) `localhost:5432`

- Run whichever API service or app you want from the NX Console (easiest method but harder to configure)

## Known issues

### Infinite nx graph calculation time when serving asp.net core app for the first time

Try running `nx reset` and then serving the asp.net core app again. This seemed to have solved the issue.

> **Source:** https://github.com/nx-dotnet/nx-dotnet/issues/924#issuecomment-2968430323

### Adding new .NET libs/apps don't get referenced in the root solution automatically

Simply use the dotnet cli or the `.NET: Add Existing Project...` vscode command to link the new projects to the root solution.

### Infinite nx graph calculation when running `nx migrate --run-migrations`

This is somehow due to the `@nx-dotnet/core` plugin in `nx.json`. By temporarily removing that plugin, you can run migrations successfully.

### Incorrect package versions pulled during `dotnet restore` in CI

The .NET solution uses Central Package Management to enforce consistent package versions, defined in `Directory.Packages.props`. The file must be named exactly `Directory.Packages.props` (with a capital "P" in "Packages") due to case-sensitivity on Linux-based CI environments like GitHub Actions. Incorrect casing (e.g., `Directory.packages.props`) causes NuGet to ignore the file, disabling central package management and pulling outdated, incompatible package versions (e.g., AutoMapper 1.1.0 instead of 13.0.1), resulting in build errors like NU1701 and NU1604. This issue does not occur on Windows due to its case-insensitive file system.

**Solution**: Ensure the file is named `Directory.Packages.props` in the repository. Verify the casing before committing changes.

> **Source:** https://learn.microsoft.com/en-us/nuget/consume-packages/central-package-management
