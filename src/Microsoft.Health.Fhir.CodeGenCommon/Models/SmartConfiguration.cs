// <copyright file="SmartConfiguration.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System.Text.Json.Serialization;

namespace Microsoft.Health.Fhir.CodeGenCommon.Models;

/// <summary>SMART configuration information, i.e. from /.well-known/smart-configuration.</summary>
public record class SmartConfiguration
{
    /// <summary>
    /// Gets or sets the string conveying this system’s OpenID Connect Issuer URL. Required
    /// if the server’s capabilities include sso-openid-connect; otherwise, omitted.
    /// </summary>
    [JsonPropertyName("issuer")]
    public string Issuer { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the string conveying this system’s JSON Web Key Set URL. Required if the server’s
    /// capabilities include sso-openid-connect; otherwise, optional.
    /// </summary>
    [JsonPropertyName("jwks_uri")]
    public string JwksUri { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the array of grant types supported at the token endpoint. The options are
    /// “authorization_code” (when SMART App Launch is supported) and “client_credentials” (when
    /// SMART Backend Services is supported).
    /// </summary>
    [JsonPropertyName("grant_types_supported")]
    public IEnumerable<string> GrantTypes { get; set; } = Enumerable.Empty<string>();

    /// <summary>Gets or sets the URL to the OAuth2 authorization endpoint.</summary>
    [JsonPropertyName("authorization_endpoint")]
    public string AuthorizationEndpoint { get; set; } = string.Empty;

    /// <summary>Gets or sets the URL to the OAuth2 token endpoint.</summary>
    [JsonPropertyName("token_endpoint")]
    public string TokenEndpoint { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the array of client authentication methods supported by the token endpoint. The
    /// options are “client_secret_post”, “client_secret_basic”, and “private_key_jwt”.
    /// </summary>
    [JsonPropertyName("token_endpoint_auth_methods_supported")]
    public IEnumerable<string> TokenEndpointAuthMethods { get; set; } = Enumerable.Empty<string>();

    /// <summary>
    /// Gets or sets the URL to the OAuth2 dynamic registration endpoint for this FHIR server, if
    /// available.
    /// </summary>
    [JsonPropertyName("registration_endpoint")]
    public string RegistrationEndpoint { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets URL the to the EHR’s app state endpoint. SHALL be present when the EHR supports the
    /// smart-app-state capability and the endpoint is distinct from the EHR’s primary endpoint.
    /// </summary>
    [JsonPropertyName("smart_app_state_endpoint")]
    public string AppStateEndpoint { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the array of scopes a client may request. See scopes and launch context. The
    /// server SHALL support all scopes listed here; additional scopes MAY be supported (so clients
    /// should not consider this an exhaustive list).
    /// </summary>
    [JsonPropertyName("scopes_supported")]
    public IEnumerable<string> SupportedScopes { get; set; } = Enumerable.Empty<string>();

    /// <summary>
    /// Gets or sets the array of OAuth2 response_type values that are supported. Implementers can refer
    /// to response_types defined in OAuth 2.0 (RFC 6749) and in OIDC Core.
    /// </summary>
    [JsonPropertyName("response_types_supported")]
    public IEnumerable<string> SupportedResponseTypes { get; set; } = Enumerable.Empty<string>();

    /// <summary>
    /// Gets or sets the URL where an end-user can view which applications currently have access to
    /// data and can make adjustments to these access rights.
    /// </summary>
    [JsonPropertyName("management_endpoint")]
    public string ManagementEndpoint { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the URL to a server’s introspection endpoint that can be used to validate a
    /// token.
    /// </summary>
    [JsonPropertyName("introspection_endpoint")]
    public string IntrospectionEndpoint { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the URL to a server’s revoke endpoint that can be used to revoke a token.
    /// </summary>
    [JsonPropertyName("revocation_endpoint")]
    public string RecovationEndpoint { get; set; } = string.Empty;

    /// <summary>Gets or sets the array of strings representing SMART capabilities (e.g., sso-openid-connect or launch-standalone) that the server supports..</summary>
    [JsonPropertyName("capabilities")]
    public IEnumerable<string> Capabilities { get; set; } = Enumerable.Empty<string>();

    /// <summary>
    /// Gets or sets the array of PKCE code challenge methods supported. The S256 method SHALL be
    /// included in this list, and the plain method SHALL NOT be included in this list.
    /// </summary>
    [JsonPropertyName("challenge_methods_supported")]
    public IEnumerable<string> SupportedChallengeMethods { get; set; } = Enumerable.Empty<string>();
}
