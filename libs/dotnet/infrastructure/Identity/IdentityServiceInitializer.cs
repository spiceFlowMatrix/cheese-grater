using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
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
using Microsoft.Extensions.Options;
using Microsoft.Kiota.Abstractions;
using Microsoft.Kiota.Serialization;
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
  private readonly KeycloakAdminClientOptions _options;

  private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
  {
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    WriteIndented = true,
  };

  public KeycloakInitialiser(
    ILogger<KeycloakInitialiser> logger,
    IKeycloakProtectionClient protectionClient,
    KeycloakAdminApiClient adminClient,
    IOptions<KeycloakAdminClientOptions> options
  )
  {
    _protectionClient = protectionClient;
    _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    _adminClient = adminClient ?? throw new ArgumentNullException(nameof(adminClient));
    _options = options.Value ?? throw new ArgumentNullException(nameof(options.Value));
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
          await SeedPoliciesAsync("Test", clientId);
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

  /// <summary>
  /// Seeds policies into a Keycloak realm for a client, adding missing policies.
  /// </summary>
  /// <param name="realm">The target realm. Must not be null or empty.</param>
  /// <param name="clientId">The target client ID. Must not be null or empty.</param>
  /// <exception cref="ArgumentException">Thrown if realm or clientId is null/empty.</exception>
  /// <remarks>
  /// Custom URL is used due to auto-generated client based on an API spec not matching the server.
  /// JSON body is serialized manually as the server expects JSON, unlike the spec's string body.
  /// </remarks>
  private async Task SeedPoliciesAsync(string realm, string clientId)
  {
    if (string.IsNullOrWhiteSpace(realm))
      throw new ArgumentException("Realm cannot be null or empty.", nameof(realm));
    if (string.IsNullOrWhiteSpace(clientId))
      throw new ArgumentException("Client ID cannot be null or empty.", nameof(clientId));

    _logger.LogInformation(
      "Starting policy seeding for client {ClientId} in realm {Realm}",
      clientId,
      realm
    );

    // Fetch existing resources, policies, and roles
    var existingResources = await _adminClient
      .Admin.Realms[realm]
      .Clients[clientId]
      .Authz.ResourceServer.Resource.GetAsync();
    var existingPolicies = await _adminClient
      .Admin.Realms[realm]
      .Clients[clientId]
      .Authz.ResourceServer.Policy.GetAsync();
    var existingRoles = await _adminClient.Admin.Realms[realm].Clients[clientId].Roles.GetAsync();

    if (existingResources == null || existingPolicies == null || existingRoles == null)
    {
      _logger.LogWarning(
        "Existing resources, policies, or roles are null. Skipping policy seeding for client {ClientId} in realm {Realm}",
        clientId,
        realm
      );
      return;
    }

    // Filter policies that do not exist
    var policiesToAdd = PolicyConstants.All.Where(policy =>
      !existingPolicies.Select(e => e.Name).Contains(policy.PolicyName)
    );

    foreach (var policy in policiesToAdd)
    {
      try
      {
        _logger.LogInformation(
          "Seeding policy {PolicyName} for client {ClientId} in realm {Realm}",
          policy.PolicyName,
          clientId,
          realm
        );

        var serializedPolicy = GenerateKeycloakPolicy(policy, existingRoles);
        var baseUrl =
          $"{_options.AuthServerUrl}admin/realms/{realm}/clients/{clientId}/authz/resource-server/policy";
        var policyUrl = policy.Type == EPolicyType.Role ? $"{baseUrl}/role" : baseUrl;

        await _adminClient
          .Admin.Realms[realm]
          .Clients[clientId]
          .Authz.ResourceServer.Policy.WithUrl(policyUrl)
          .PostAsync(
            serializedPolicy,
            config => config.Headers = new RequestHeaders { { "Content-Type", "application/json" } }
          );

        _logger.LogInformation("Successfully seeded policy {PolicyName}", policy.PolicyName);
      }
      catch (Exception ex)
      {
        _logger.LogError(
          ex,
          "Error seeding policy {PolicyName} for client {ClientId} in realm {Realm}",
          policy.PolicyName,
          clientId,
          realm
        );
      }
    }

    _logger.LogInformation(
      "Completed policy seeding for client {ClientId} in realm {Realm}",
      clientId,
      realm
    );
  }

  /// <summary>
  /// Generates a JSON string for a Keycloak policy from an application policy and roles.
  /// </summary>
  /// <param name="appPolicy">The application policy. Must not be null.</param>
  /// <param name="existingRoles">Existing roles in Keycloak. Must not be null.</param>
  /// <returns>JSON string of the policy.</returns>
  /// <exception cref="ArgumentNullException">Thrown if appPolicy or existingRoles is null.</exception>
  /// <remarks>
  /// Manual JSON serialization is used as the server expects a JSON body, unlike the spec's string body.
  /// </remarks>
  private string GenerateKeycloakPolicy(
    ApplicationPolicy appPolicy,
    List<RoleRepresentation> existingRoles
  )
  {
    if (appPolicy == null)
      throw new ArgumentNullException(nameof(appPolicy));
    if (existingRoles == null)
      throw new ArgumentNullException(nameof(existingRoles));

    string policyType = appPolicy.Type switch
    {
      EPolicyType.Role => "role",
      EPolicyType.Owner => "script-isOwnerPolicy.js",
      _ => "role",
    };

    var policyDict = new Dictionary<string, object>
    {
      ["name"] = appPolicy.PolicyName,
      ["decisionStrategy"] = "AFFIRMATIVE",
      ["type"] = policyType,
      ["logic"] = "POSITIVE",
    };

    if (appPolicy.Type == EPolicyType.Role && appPolicy.Roles != null)
    {
      var selectedRoles = existingRoles
        .Where(r => appPolicy.Roles.Contains(r.Name ?? "") && !string.IsNullOrEmpty(r.Id))
        .Select(r => new Dictionary<string, object> { ["id"] = r.Id!, ["required"] = false })
        .ToList();

      policyDict["roles"] = selectedRoles;
    }

    return JsonSerializer.Serialize(policyDict, _jsonOptions);
  }
}
