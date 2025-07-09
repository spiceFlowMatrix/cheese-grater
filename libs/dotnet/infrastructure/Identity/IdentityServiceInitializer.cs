using CheeseGrater.Application.Common.Security;
using CheeseGrater.Core.Application.Common.Security;
using CheeseGrater.Core.Domain.Constants;
using Keycloak.AuthServices.Sdk.Kiota;
using Keycloak.AuthServices.Sdk.Kiota.Admin;
using Keycloak.AuthServices.Sdk.Kiota.Admin.Models;
using Keycloak.AuthServices.Sdk.Protection;
using Microsoft.AspNetCore.Builder;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using static Keycloak.AuthServices.Sdk.Kiota.Admin.Admin.Realms.Item.Clients.ClientsRequestBuilder;

namespace CheeseGrater.Infrastructure.Identity;

public static class KeycloakInitialiserExtensions
{
  public static async Task InitialiseKeycloakAsync(this WebApplication app)
  {
    using var scope = app.Services.CreateScope();
    var initialiser = scope.ServiceProvider.GetRequiredService<KeycloakInitialiser>();
    await initialiser.InitialiseAsync();
  }
}

public class KeycloakInitialiser
{
  private readonly ILogger<KeycloakInitialiser> _logger;
  private readonly IKeycloakProtectionClient _protectionClient;
  private readonly KeycloakAdminApiClient _adminClient;

  public KeycloakInitialiser(
    ILogger<KeycloakInitialiser> logger,
    IKeycloakProtectionClient protectionClient,
    KeycloakAdminApiClient adminClient
  )
  {
    _logger = logger;
    _protectionClient = protectionClient;
    _adminClient = adminClient;
  }

  public async Task InitialiseAsync()
  {
    try
    {
      await SeedAsync();
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "An error occurred while initialising Keycloak.");
      throw;
    }
  }

  private async Task SeedAsync()
  {
    try
    {
      var realm = await _adminClient.Admin.Realms["Test"].GetAsync();

      if (realm != null)
      {
        var clientId = await SeedClientAsync("Test", "test-client");
        if (clientId != null)
        {
          await SeedResourcesAsync("Test", clientId);
          await SeedRolesAsync("Test", clientId);
        }
      }
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "An error occurred while seeding Keycloak.");
      throw;
    }
  }

  private async Task<string?> SeedClientAsync(string realm, string clientNameId)
  {
    try
    {
      var existingClients =
        await _adminClient
          .Admin.Realms[realm]
          .Clients.GetAsync(config =>
          {
            config.QueryParameters = new ClientsRequestBuilderGetQueryParameters
            {
              ClientId = clientNameId,
            };
          }) ?? [];

      var existingClientId = existingClients.Find((e) => e.ClientId == clientNameId);

      if (existingClientId != null)
        return existingClientId.Id;
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Failed to retrieve TestClient details.");
      return null;
    }

    try
    {
      await _adminClient
        .Admin.Realms[realm]
        .Clients.PostAsync(
          new ClientRepresentation
          {
            ClientId = clientNameId,
            Name = clientNameId,
            Enabled = true,
            PublicClient = false,
            Protocol = "openid-connect",
          }
        );
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Client creation failed");
    }

    IReadOnlyList<ClientRepresentation>? clients = null;
    try
    {
      clients = await _adminClient.Admin.Realms[realm].Clients.GetAsync();
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Failed to retrieve clients from Keycloak.");
    }

    if (clients == null || !clients.Any())
    {
      _logger.LogError("No clients found in realm 'Test'.");
      return null;
    }

    var selectClient = clients.FirstOrDefault(c => c.ClientId == clientNameId);
    if (selectClient == null)
    {
      _logger.LogError("TestClient was not found after creation attempt.");
      return null;
    }

    ClientRepresentation? client = null;
    try
    {
      client = await _adminClient.Admin.Realms[realm].Clients[selectClient.Id].GetAsync();
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Failed to retrieve TestClient details.");
      return null;
    }

    if (client == null)
    {
      _logger.LogError("TestClient details could not be loaded.");
      return null;
    }

    client.ServiceAccountsEnabled = true;
    client.AuthorizationServicesEnabled = true;

    try
    {
      await _adminClient.Admin.Realms[clientNameId].Clients[client.Id].PutAsync(client);
    }
    catch (Exception ex)
    {
      _logger.LogError(
        ex,
        "Failed to update TestClient with service account and authorization settings."
      );
    }

    return client.Id;
  }

  private async Task SeedResourcesAsync(string realm, string clientId)
  {
    var authSettings = new ResourceRepresentation
    {
      Name = Resources.TodoResource,
      Type = $"urn:{Resources.TodoResource.ToLower()}",
      Scopes =
      [
        .. Scopes.All.Select(scope => new ScopeRepresentation
        {
          Name = $"{Resources.TodoResource}:{scope}",
          DisplayName = $"{scope} {Resources.TodoResource}",
        }),
      ],
    };
    try
    {
      await _adminClient
        .Admin.Realms[realm]
        .Clients[clientId]
        .Authz.ResourceServer.Resource.PostAsync(authSettings);
      _logger.LogInformation(
        "Resource '{ResourceName}' seeded successfully for client '{ClientId}' in realm '{Realm}'.",
        Resources.TodoResource,
        clientId,
        realm
      );
    }
    catch (Exception ex)
    {
      _logger.LogError(
        ex,
        "Failed to seed resource '{ResourceName}' for client '{ClientId}' in realm '{Realm}'.",
        Resources.TodoResource,
        clientId,
        realm
      );
    }
  }

  private async Task SeedRolesAsync(string realm, string clientId)
  {
    // Get existing roles
    var existingRoles = await _adminClient.Admin.Realms[realm].Clients[clientId].Roles.GetAsync();

    if (existingRoles == null)
      return;

    // Filter roles that do not exist
    var rolesToAdd = Roles
      .All.Where(role => !existingRoles.Select((e) => e.Name).Contains(role))
      .Select(role => new RoleRepresentation { Name = role });

    // Add new roles
    foreach (var role in rolesToAdd)
    {
      try
      {
        await _adminClient.Admin.Realms[realm].Clients[clientId].Roles.PostAsync(role);
        _logger.LogInformation(
          "Role '{RoleName}' added successfully to client '{ClientId}' in realm '{Realm}'.",
          role.Name,
          clientId,
          realm
        );
      }
      catch (Exception ex)
      {
        _logger.LogError(
          ex,
          "Failed to add role '{RoleName}' to client '{ClientId}' in realm '{Realm}'.",
          role.Name,
          clientId,
          realm
        );
      }
    }
  }

  private async Task SeedPoliciesAsync(string realm, string clientId)
  {
    // Get existing roles
    var existingResources = await _adminClient
      .Admin.Realms[realm]
      .Clients[clientId]
      .Authz.ResourceServer.Resource.GetAsync();
    var existingPolicies = await _adminClient
      .Admin.Realms[realm]
      .Clients[clientId]
      .Authz.ResourceServer.Policy.GetAsync();
    var existingRoles = await _adminClient.Admin.Realms[realm].Clients[clientId].Roles.GetAsync();
    var resourceServer = await _adminClient
      .Admin.Realms[realm]
      .Clients[clientId]
      .Authz.ResourceServer.GetAsync();

    if (existingResources == null || existingPolicies == null || existingRoles == null)
      return;

    // Filter roles that do not exist
    var policiesToAdd = PolicyConstants
      .All.Where(policy => !existingPolicies.Select((e) => e.Name).Contains(policy.PolicyName))
      .Select(GenerateKeycloakPolicy);

    resourceServer.Policies = policiesToAdd.ToList();

    try
    {
      await _adminClient
        .Admin.Realms[realm]
        .Clients[clientId]
        .Authz.ResourceServer.Import.PostAsync(resourceServer);
    }
    catch (Exception ex) { }
  }

  private PolicyRepresentation GenerateKeycloakPolicy(ApplicationPolicy appPolicy)
  {
    string policyType;
    switch (appPolicy.Type)
    {
      case EPolicyType.Role:
        policyType = "role";
        break;
      case EPolicyType.Owner:
        policyType = "script-isOwnerPolicy.js";
        break;
      default:
        policyType = "role";
        break;
    }
    return new PolicyRepresentation
    {
      Name = appPolicy.PolicyName,
      DecisionStrategy = DecisionStrategy.AFFIRMATIVE,
      Type = policyType,
      Logic = Logic.POSITIVE,
      // Resources = policy.Roles != null && policy.Type == EPolicyType.Role
      //         ? existingRoles
      //             .Where((r) => policy.Roles.Contains(r.Name ?? ""))
      //             .Select((r) => r.Id ?? "")
      //             .ToList()
      //         : [],
      // Config = new PolicyRepresentation_config
      // {
      //     AdditionalData = new Dictionary<string, object>
      //     {
      //         ["roles"] = policy.Roles != null && policy.Type == EPolicyType.Role
      //         ? existingRoles
      //             .Where((r) => policy.Roles.Contains(r.Name ?? ""))
      //             .Select((r) => new
      //             {
      //                 id = r.Id,
      //                 required = false
      //             })
      //             .ToArray()
      //         : Array.Empty<string>()
      //     },
      // }
    };
  }

  private class TokenResponse
  {
    public string access_token { get; set; }
  }
}
