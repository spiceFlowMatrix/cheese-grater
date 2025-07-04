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
                var authSettings = new ResourceRepresentation
                {
                    Name = Resources.TodoResource,
                    Type = $"urn:{Resources.TodoResource.ToLower()}",
                    // Scopes =
                    // [
                    //     new ScopeRepresentation
                    //     {
                    //         Name = "view",
                    //         DisplayName = "View Todo Resource"
                    //     },
                    //     new ScopeRepresentation
                    //     {
                    //         Name = "edit",
                    //         DisplayName = "Edit Edit Resource"
                    //     }
                    // ]
                };
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
                    _logger.LogWarning("ClientCreation: {Message}", ex.Message);
                }
                var client = await _adminClient.Admin.Realms["Test"].Clients["TestClient"].GetAsync();
                client.AuthorizationServicesEnabled = true;
                await _adminClient.Admin.Realms["Test"].Clients["TestClient"].PutAsync(client);

                // await _adminClient.Admin.Realms["Test"].Clients["TestClient"].Authz.ResourceServer.Resource.PostAsync(authSettings);
            }

            // realm.Clients.Where
            // await SeedClientsAsync(_accessToken, realm);
            // await SeedRolesAsync(_accessToken, realm);
            // Add methods for resources and permissions as needed
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding Keycloak.");
            throw;
        }
    }

    private async Task SeedClientsAsync(string accessToken)
    {
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

    // private async Task<List<object>> GetClientsAsync(HttpClient client, string realm, string clientId)
    // {
    //     var response = await client.GetAsync($"/admin/realms/{realm}/clients?clientId={clientId}");
    //     response.EnsureSuccessStatusCode();
    //     var content = await response.Content.ReadAsStringAsync();
    //     return JsonSerializer.Deserialize<List<object>>(content);
    // }

    // private async Task<List<object>> GetRolesAsync(HttpClient client, string realm, string roleName)
    // {
    //     var response = await client.GetAsync($"/admin/realms/{realm}/roles/{roleName}");
    //     if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
    //         return new List<object>();
    //     response.EnsureSuccessStatusCode();
    //     var content = await response.Content.ReadAsStringAsync();
    //     return new List<object> { JsonSerializer.Deserialize<object>(content) };
    // }

    private class TokenResponse
    {
        public string access_token { get; set; }
    }
}