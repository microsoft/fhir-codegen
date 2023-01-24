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

    public string TokenEndpoint { get; }
    public string AuthorizeEndpoint { get; }
    public string IntrospectEndpoint { get; }
    public string RevokeEndpoint { get; }

    /// <summary>Makes a deep copy of this object.</summary>
    /// <returns>A copy of this object.</returns>
    public override object Clone()
    {
        return new FhirCapSmartOAuthScheme(this);
    }
}
