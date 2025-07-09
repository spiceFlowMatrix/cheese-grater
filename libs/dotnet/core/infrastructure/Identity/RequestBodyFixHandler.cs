using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CheeseGrater.Core.Infrastructure.Identity;

/// <summary>
/// A custom DelegatingHandler that intercepts HTTP requests to fix double-escaped JSON bodies
/// for specific Keycloak API endpoints and logs request/response details securely.
/// </summary>
/// <remarks>
/// This handler addresses a mismatch in the Kiota-generated Keycloak admin API client where
/// the POST body for the /authz/resource-server/policy endpoint is serialized as a double-escaped
/// JSON string, while the server expects a JSON object. It unescapes the body for the specific
/// endpoint and logs details only in development environments to prevent sensitive data exposure.
/// Sensitive headers, such as Authorization, are redacted in logs for security.
/// </remarks>
public class RequestBodyFixHandler : DelegatingHandler
{
  private readonly ILogger<RequestBodyFixHandler> _logger;
  private readonly IHostEnvironment _env;

  /// <summary>
  /// Initializes a new instance of the <see cref="RequestBodyFixHandler"/> class.
  /// </summary>
  /// <param name="logger">The logger for recording request and response details.</param>
  /// <param name="env">The hosting environment to determine if the application is in development.</param>
  public RequestBodyFixHandler(ILogger<RequestBodyFixHandler> logger, IHostEnvironment env)
  {
    _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    _env = env ?? throw new ArgumentNullException(nameof(env));
  }

  /// <summary>
  /// Processes an HTTP request, fixing double-escaped JSON bodies for specific endpoints
  /// and logging details securely based on the environment.
  /// </summary>
  /// <param name="request">The HTTP request message to process.</param>
  /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
  /// <returns>A task representing the asynchronous operation, returning the HTTP response.</returns>
  protected override async Task<HttpResponseMessage> SendAsync(
    HttpRequestMessage request,
    CancellationToken cancellationToken
  )
  {
    // Log request method and URI (safe to log in all environments)
    _logger.LogInformation("Request: {Method} {Uri}", request.Method, request.RequestUri);

    // Preparing to handle the request content
    string content = null;
    if (request.Content != null)
    {
      content = await request.Content.ReadAsStringAsync(cancellationToken);
    }

    // Fixing double-escaped JSON body for the specific Keycloak endpoint
    if (
      request.Method == HttpMethod.Post
      && request.RequestUri?.AbsolutePath.Contains("/authz/resource-server/policy") == true
      && !string.IsNullOrEmpty(content)
      && content.StartsWith("\"")
      && content.EndsWith("\"")
    )
    {
      try
      {
        // Unescaping the double-escaped JSON string
        var unescapedContent = JsonSerializer.Deserialize<string>(content);
        if (!string.IsNullOrEmpty(unescapedContent))
        {
          // Replacing the request content with the corrected JSON payload
          request.Content = new StringContent(unescapedContent, Encoding.UTF8, "application/json");
          if (_env.IsDevelopment())
          {
            _logger.LogDebug("Modified Request Body: {Body}", unescapedContent);
          }
          else
          {
            _logger.LogInformation("Modified Request Body: [Redacted for production]");
          }
        }
      }
      catch (JsonException ex)
      {
        // Logging the error but proceeding with the original content to avoid breaking the request
        _logger.LogError(
          ex,
          "Failed to unescape JSON body for request to {Uri}",
          request.RequestUri
        );
      }
    }
    else if (_env.IsDevelopment() && !string.IsNullOrEmpty(content))
    {
      // Logging the request body only in development for other endpoints
      _logger.LogDebug("Request Body: {Body}", content);
    }

    // Logging headers securely with redaction for sensitive fields
    if (_env.IsDevelopment() && request.Headers.Any())
    {
      var headers = string.Join(
        "; ",
        request.Headers.Select(h =>
          h.Key.ToLowerInvariant() == "authorization"
            ? $"{h.Key}: [Redacted]"
            : $"{h.Key}: {string.Join(", ", h.Value)}"
        )
      );
      _logger.LogDebug("Request Headers: {Headers}", headers);
    }

    // Sending the request to the next handler or server
    var response = await base.SendAsync(request, cancellationToken);

    // Logging response status code (safe to log in all environments)
    _logger.LogInformation("Response: {StatusCode}", response.StatusCode);

    // Logging response body only in development
    if (_env.IsDevelopment() && response.Content != null)
    {
      var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
      _logger.LogDebug("Response Body: {Body}", responseContent);
    }

    return response;
  }
}
