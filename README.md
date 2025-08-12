# CheeseGrater

## Known issues

### Infinite nx graph calculation time when serving asp.net core app for the first time

Try running `nx reset` and then serving the asp.net core app again. This seemed to have solved the issue.

> **Source:** https://github.com/nx-dotnet/nx-dotnet/issues/924#issuecomment-2968430323

### Adding new .NET libs/apps don't get referenced in the root solution automatically

Simply use the dotnet cli or the `.NET: Add Existing Project...` vscode command to link the new projects to the root solution.

### Infinite nx graph calculation when running `nx migrate --run-migrations`

This is somehow due to the `@nx-dotnet/core` plugin in `nx.json`. By temporarily removing that plugin, you can run migrations successfully.
