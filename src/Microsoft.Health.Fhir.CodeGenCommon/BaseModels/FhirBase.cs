using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.Health.Fhir.CodeGenCommon.Extensions;
using static Microsoft.Health.Fhir.CodeGenCommon.Extensions.FhirNameConventionExtensions;

namespace Microsoft.Health.Fhir.CodeGenCommon.BaseModels;

public abstract record class FhirBase :  ICloneable
{
    public FhirBase() { }

    [SetsRequiredMembers]
    protected FhirBase(FhirBase other)
    {
        Id = other.Id;
        Extensions = other.Extensions.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
    }

    /// <summary>Gets or initializes the logical id of this artifact.</summary>
    public required string Id { get; init; }

    /// <summary>Gets or initializes the extensions.</summary>
    public Dictionary<string, object> Extensions { get; init; } = new();

    /// <summary>Makes a deep copy of this object.</summary>
    /// <returns>A copy of this object.</returns>
    object ICloneable.Clone() => this with { };
}
