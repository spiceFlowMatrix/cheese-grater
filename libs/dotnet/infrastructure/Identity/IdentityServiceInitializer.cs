using System.Text.Json;
using CheeseGrater.Core.Domain.Constants;
using Keycloak.AuthServices.Sdk.Kiota;
using Keycloak.AuthServices.Sdk.Kiota.Admin;
using Keycloak.AuthServices.Sdk.Kiota.Admin.Models;
using Keycloak.AuthServices.Sdk.Protection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Kiota.Abstractions.Serialization;
using Microsoft.Kiota.Serialization;

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
                var clientId = await SeedClientAsync();
                if (clientId != null)
                    await SeedResourcesAsync("Test", clientId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding Keycloak.");
            throw;
        }
    }

    private async Task<string?> SeedClientAsync()
    {
        try
        {
            await _adminClient.Admin.Realms["Test"].Clients.PostAsync(new ClientRepresentation
            {
                ClientId = "TestClient",
                Name = "Test Client",
                Description = "A test client for Keycloak initialisation",
                Enabled = true,
                PublicClient = false,
                Protocol = "openid-connect",
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Client creation failed");
        }

        IReadOnlyList<ClientRepresentation>? clients = null;
        try
        {
            clients = await _adminClient.Admin.Realms["Test"].Clients.GetAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve clients from Keycloak.");
            throw;
        }

        if (clients == null || !clients.Any())
        {
            _logger.LogError("No clients found in realm 'Test'.");
            return null;
        }

        var selectClient = clients.FirstOrDefault(c => c.ClientId == "TestClient");
        if (selectClient == null)
        {
            _logger.LogError("TestClient was not found after creation attempt.");
            return null;
        }

        ClientRepresentation? client = null;
        try
        {
            client = await _adminClient.Admin.Realms["Test"].Clients[selectClient.Id].GetAsync();
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
            await _adminClient.Admin.Realms["Test"].Clients[client.Id].PutAsync(client);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update TestClient with service account and authorization settings.");
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
                    DisplayName = $"{scope} {Resources.TodoResource}"
                })
            ],
        };
        await _adminClient.Admin.Realms[realm].Clients[clientId].Authz.ResourceServer.Resource.PostAsync(authSettings);
    }

    // private async Task SeedRolesAsync(string accessToken)
    // {
    //     var roles = new[]
    //     {
    //             new { name = "user", description = "Standard user role" },
    //             new { name = "admin", description = "Administrator role" }
    //             // Add more roles as needed
    //         };

    //     using var client = _httpClientFactory.CreateClient();
    //     client.BaseAddress = new Uri(_configuration["Keycloak:ServerUrl"]);
    //     client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

    //     foreach (var role in roles)
    //     {
    //         var existingRoles = await GetRolesAsync(client, realm, role.name);
    //         if (existingRoles.Count == 0)
    //         {
    //             var json = JsonSerializer.Serialize(role);
    //             var content = new StringContent(json, Encoding.UTF8, "application/json");
    //             var response = await client.PostAsync($"/admin/realms/{realm}/roles", content);
    //             response.EnsureSuccessStatusCode();
    //             _logger.LogInformation($"Created role: {role.name}");
    //         }
    //         else
    //         {
    //             _logger.LogInformation($"Role {role.name} already exists.");
    //         }
    //     }
    // }


    private class TokenResponse
    {
        public string access_token { get; set; }
    }
}