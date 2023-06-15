// <copyright file="FhirCapSecurity.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

namespace Microsoft.Health.Fhir.CodeGenCommon.Models;

/// <summary>Information about a security scheme, taken from the CapabilityStatement.</summary>
abstract public class FhirCapSecurityScheme : ICloneable
{
    public abstract object Clone();
}

/// <summary>Information about a security scheme, taken from the CapabilityStatement.</summary>
public class FhirCapSmartOAuthScheme : FhirCapSecurityScheme
{
    /// <summary>Initializes a new instance of the <see cref="FhirCapSmartOAuthSecurity"/> class.</summary>
    public FhirCapSmartOAuthScheme(
        string tokenEndpoint,
        string authorizeEndpoint,
        string introspectEndpoint,
        string revokeEndpoint)
    {
        TokenEndpoint = tokenEndpoint;
        AuthorizeEndpoint = authorizeEndpoint;
        IntrospectEndpoint = introspectEndpoint;
        RevokeEndpoint = revokeEndpoint;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FhirCapOperation"/> class.
    /// </summary>
    /// <param name="source">Source for the.</param>
    public FhirCapSmartOAuthScheme(FhirCapSmartOAuthScheme source)
    {
        TokenEndpoint = source.TokenEndpoint;
        AuthorizeEndpoint = source.AuthorizeEndpoint;
        IntrospectEndpoint = source.IntrospectEndpoint;
        RevokeEndpoint = source.RevokeEndpoint;
    }

    /// <summary>Gets the token endpoint.</summary>
    public string TokenEndpoint { get; }

    /// <summary>Gets the authorize endpoint.</summary>
    public string AuthorizeEndpoint { get; }

    /// <summary>Gets the introspect endpoint.</summary>
    public string IntrospectEndpoint { get; }

    /// <summary>Gets the revoke endpoint.</summary>
    public string RevokeEndpoint { get; }

    /// <summary>Makes a deep copy of this object.</summary>
    /// <returns>A copy of this object.</returns>
    public override object Clone()
    {
        return new FhirCapSmartOAuthScheme(this);
    }
}
