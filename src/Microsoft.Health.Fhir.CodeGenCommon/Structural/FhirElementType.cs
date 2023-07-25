// <copyright file="FhirElementType.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using static Microsoft.Health.Fhir.CodeGenCommon.Structural.FhirTypeUtils;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Health.Fhir.CodeGenCommon.Structural;

/// <summary>A FHIR element type.</summary>
public record class FhirElementType : ICloneable
{
    /// <summary>(Immutable) URL of the base element type.</summary>
    private const string _baseElementTypeUrl = "http://hl7.org/fhir/StructureDefinition/";

    private readonly Dictionary<string, FhirElementProfile> _targetProfiles = new();
    private readonly Dictionary<string, FhirElementProfile> _typeProfiles = new();

    private readonly HashSet<string> _aggregation = new();

    /// <summary>Initializes a new instance of the FhirElementType class.</summary>
    public FhirElementType() { }

    /// <summary>Initializes a new instance of the FhirElementType class.</summary>
    /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
    /// <param name="code">The code.</param>
    [SetsRequiredMembers]
    public FhirElementType(string code)
    {
        if (string.IsNullOrEmpty(code))
        {
            throw new ArgumentNullException(nameof(code));
        }

        if (IsXmlType(code, out string xmlFhirType))
        {
            code = xmlFhirType;
        }

        if (IsFhirPathType(code, out string fhirType))
        {
            code = fhirType;
        }

        // check for no slashes - implied relative StructureDefinition url
        int lastSlash = code.LastIndexOf('/');
        if (lastSlash == -1)
        {
            Name = code;
            Url = _baseElementTypeUrl + code;
        }
        else
        {
            Name = code.Substring(lastSlash + 1);
            Url = code;
        }

        TypeName = Name;
    }

    /// <summary>Initializes a new instance of the FhirElementType class.</summary>
    /// <param name="other">The other.</param>
    [SetsRequiredMembers]
    protected FhirElementType(FhirElementType other)
    {
        Name = other.Name;
        TypeName = other.TypeName;
        Url = other.Url;
        _aggregation = other._aggregation.ToHashSet();
        _targetProfiles = other._targetProfiles.ToDictionary(kvp => kvp.Key, kvp => kvp.Value with { });
        _typeProfiles = other._typeProfiles.ToDictionary(kvp => kvp.Key, kvp => kvp.Value with { });
    }

    /// <summary>Gets the name. Data type or Resource (reference to definition)</summary>
    public required string Name { get; init; }

    /// <summary>Gets the type. Data type or Resource (reference to definition)</summary>
    public required string TypeName { get; init; }

    /// <summary>Gets URL of the document. Data type or Resource (reference to definition)</summary>
    public required string Url { get; init; }

    /// <summary>
    /// Gets the aggregation modes. If the type is a reference to another resource, how the resource
    /// is or can be aggregated - is it a contained resource, or a reference, and if the context is a
    /// bundle, is it included in the bundle.
    /// </summary>
    public HashSet<string> AggregationModes { get => _aggregation; init => _aggregation = value; }

    /// <summary>Gets or initializes the FHIR aggregations.</summary>
    public IEnumerable<string> FhirAggregations
    {
        init => _aggregation.UnionWith(value ?? Array.Empty<string>());
    }

    /// <summary>
    /// Gets the target profiles. Used when the type is "Reference" or "canonical", and identifies a
    /// profile structure or implementation Guide that applies to the target of the reference this
    /// element refers to.If any profiles are specified, then the content must conform to at least
    /// one of them.The URL can be a local reference - to a contained StructureDefinition, or a
    /// reference to another StructureDefinition or Implementation Guide by a canonical URL.When an
    /// implementation guide is specified, the target resource SHALL conform to at least one profile
    /// defined in the implementation guide.
    /// </summary>
    public Dictionary<string, FhirElementProfile> TargetProfiles { get => _targetProfiles; init => _targetProfiles = value; }

    /// <summary>Initializes the FHIR target profiles.</summary>
    public IEnumerable<string> FhirTargetProfiles
    {
        get => _targetProfiles.Values.Select(x => x.Url);
        init => _targetProfiles = FhirElementProfile.ParseProfiles(value);
    }

    /// <summary>
    /// Gets the type profiles. Identifies a profile structure or implementation Guide that applies
    /// to the datatype this element refers to. If any profiles are specified, then the content must
    /// conform to at least one of them. The URL can be a local reference - to a contained
    /// StructureDefinition, or a reference to another StructureDefinition or Implementation Guide by
    /// a canonical URL. When an implementation guide is specified, the type SHALL conform to at
    /// least one profile defined in the implementation guide.
    /// </summary>
    public Dictionary<string, FhirElementProfile> TypeProfiles { get => _typeProfiles; init => _typeProfiles = value; }

    /// <summary>Initializes the FHIR type profiles.</summary>
    public IEnumerable<string> FhirTypeProfiles
    {
        get => _typeProfiles.Values.Select(x => x.Url);
        init => _typeProfiles = FhirElementProfile.ParseProfiles(value);
    }

    /// <summary>Makes a deep copy of this object.</summary>
    /// <returns>A copy of this object.</returns>
    object ICloneable.Clone() => this with { };
}
