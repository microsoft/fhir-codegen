// <copyright file="FhirActor.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System.Diagnostics.CodeAnalysis;
using Microsoft.Health.Fhir.CodeGenCommon.Extensions;

namespace Microsoft.Health.Fhir.CodeGenCommon.Resource;

/// <summary>A FHIR actor.</summary>
public record class FhirActor : FhirCanonicalBase, ICloneable
{
    private string _fhirActorType = string.Empty;
    private readonly ActorTypeCodes _actorType;

    /// <summary>Values that represent actor type codes.</summary>
    public enum ActorTypeCodes
    {
        /// <summary>A human actor.</summary>
        [FhirLiteral("person")]
        Person,

        /// <summary>A software application or another system.</summary>
        [FhirLiteral("system")]
        System,
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FhirActor"/> class.
    /// </summary>
    public FhirActor() { }

    /// <summary>Initializes a new instance of the <see cref="FhirActor"/> class.</summary>
    /// <param name="other">The other.</param>
    [SetsRequiredMembers]
    protected FhirActor(FhirActor other)
        : base(other)
    {
        FhirActorType = other.FhirActorType;
        Documentation = other.Documentation;
        InformationUrls = other.InformationUrls.Select(v => v);
        CapabilityCanonicalUrl = other.CapabilityCanonicalUrl;
        DerivedFromActorCanonicals = other.DerivedFromActorCanonicals.Select(v => v);
    }

    /// <summary>Gets the type of the actor.</summary>
    public ActorTypeCodes ActorType => _actorType;

    /// <summary>Gets or initializes the type of the FHIR actor.</summary>
    public required string FhirActorType
    {
        get => _fhirActorType;
        init
        {
            _fhirActorType = value;

            if (_fhirActorType.TryFhirEnum(out ActorTypeCodes v))
            {
                _actorType = v;
            }
        }
    }

    /// <summary>Gets or initializes the documentation (markdown).</summary>
    public string Documentation { get; init; } = string.Empty;

    /// <summary>Gets or initializes references to more information about the actor.</summary>
    public IEnumerable<string> InformationUrls { get; init; } = Enumerable.Empty<string>();

    /// <summary>Gets or initializes the CapabilityStatement for the actor (if applicable).</summary>
    public string CapabilityCanonicalUrl { get; init; } = string.Empty;

    /// <summary>Gets or initializes the definition of this actor in another context / IG.</summary>
    public IEnumerable<string> DerivedFromActorCanonicals { get; init; } = Enumerable.Empty<string>();

    /// <summary>Makes a deep copy of this object.</summary>
    /// <returns>A copy of this object.</returns>
    object ICloneable.Clone() => this with { };
}
