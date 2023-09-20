// <copyright file="FhirDefinitionBase.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System.Diagnostics.CodeAnalysis;
using Microsoft.Health.Fhir.CodeGenCommon.BaseModels;
using Microsoft.Health.Fhir.CodeGenCommon.Extensions;
using static Microsoft.Health.Fhir.CodeGenCommon.Extensions.FhirNameConventionExtensions;

namespace Microsoft.Health.Fhir.CodeGenCommon.Structure;

/// <summary>A FHIR definition base.</summary>
public abstract record class FhirDefinitionBase : FhirArtifactBase, ICloneable
{
    internal string _path = string.Empty;

    /// <summary>Initializes a new instance of the FhirDefinitionBase class.</summary>
    public FhirDefinitionBase() : base() { }

    /// <summary>Initializes a new instance of the FhirDefinitionBase class.</summary>
    /// <param name="other">The other.</param>
    [SetsRequiredMembers]
    protected FhirDefinitionBase(FhirDefinitionBase other)
        : base(other)
    {
        Path = other.Path;
    }

    /// <summary>Gets the dot-notation path to this element/resource/datatype.</summary>
    /// <value>The dot-notation path to this element/resource/datatype.</value>
    public required string Path { get => _path; init => _path = value; }

    /// <summary>Type for export.</summary>
    /// <param name="convention">            The convention.</param>
    /// <param name="primitiveTypeMap">      The base type map.</param>
    /// <param name="concatenatePath">       (Optional) True to concatenate path.</param>
    /// <param name="concatenationDelimiter">(Optional) The concatenation delimiter.</param>
    /// <param name="reservedWords">         (Optional) The reserved words.</param>
    /// <returns>A string.</returns>
    public string TypeForExport(
        NamingConvention convention,
        Dictionary<string, string> primitiveTypeMap,
        bool concatenatePath = false,
        string concatenationDelimiter = "",
        HashSet<string> reservedWords = null!)
    {
        if (primitiveTypeMap?.ContainsKey(_baseTypeName) ?? false)
        {
            return primitiveTypeMap[_baseTypeName];
        }

        // Resources cannot inherit patterns or interfaces, but they are listed that way today
        // see https://chat.fhir.org/#narrow/stream/179177-conformance/topic/Inheritance.20and.20Cardinality.20Changes
        string baseType = _baseTypeName switch
        {
            "CanonicalResource" or "MetadataResource" => "DomainResource",
            _ => _baseTypeName,
        };

        string type = Models.FhirUtils.ToConvention(
            baseType,
            _path,
            convention,
            concatenatePath,
            concatenationDelimiter,
            reservedWords);

        return type;
    }

    /// <summary>Converts this object to a requested naming convention.</summary>
    /// <exception cref="ArgumentException">Thrown when one or more arguments have unsupported or
    ///  illegal values.</exception>
    /// <param name="convention">            The convention.</param>
    /// <param name="concatenatePath">       (Optional) True to concatenate path.</param>
    /// <param name="delimiter">(Optional) The concatenation delimiter.</param>
    /// <param name="reservedWords">         (Optional) The reserved words.</param>
    /// <returns>A string.</returns>
    public new string NameForExport(
        NamingConvention convention,
        bool concatenatePath = false,
        string delimiter = "",
        HashSet<string> reservedWords = null!)
    {
        if (string.IsNullOrEmpty(_name))
        {
            throw new ArgumentException($"Invalid Name: {_name}");
        }

        if (string.IsNullOrEmpty(_namePascal))
        {
            throw new ArgumentException($"Invalid Name: {_namePascal}");
        }

        if (string.IsNullOrEmpty(_path))
        {
            throw new ArgumentException($"Invalid Path: {_path}");
        }

        string value = convention switch
        {
            NamingConvention.FhirDotNotation => _path,
            NamingConvention.PascalDotNotation => _path.ToPascalDotCase(),
            NamingConvention.PascalCase => concatenatePath ? _path.ToPascalCase(true, delimiter) : _namePascal,
            NamingConvention.CamelCase => concatenatePath ? _path.ToCamelCase(true, delimiter) : _name.ToCamelCase(false),
            NamingConvention.UpperCase => concatenatePath ? _path.ToUpperCase(true, delimiter) : _name.ToUpperInvariant(),
            NamingConvention.LowerCase => concatenatePath ? _path.ToLowerCase(true, delimiter) : _name.ToLowerInvariant(),
            NamingConvention.LowerKebab => concatenatePath ? _path.ToLowerKebabCase(true) : _name.ToLowerKebabCase(),
            //NamingConvention.None => throw new NotImplementedException(),
            //NamingConvention.LanguageControlled => throw new NotImplementedException(),
            _ => throw new ArgumentException($"Cannot convert to Naming Convention: {convention}"),
        };

        if (reservedWords?.Contains(value) ?? false)
        {
            value = convention switch
            {
                NamingConvention.FhirDotNotation => "Fhir" + value,
                NamingConvention.PascalDotNotation => "Fhir" + value,
                NamingConvention.PascalCase => "Fhir" + value,
                NamingConvention.CamelCase => "fhir" + value.ToPascalCase(false),
                NamingConvention.UpperCase => concatenatePath ? "FHIR_" + value : "FHIR" + value,
                NamingConvention.LowerCase => concatenatePath ? "fhir_" + value : "fhir" + value,
                NamingConvention.LowerKebab => concatenatePath ? "fhir_" + value : "fhir" + value,
                _ => throw new ArgumentException($"Cannot convert to Naming Convention: {convention}"),
            };
        }

        return value;
    }

    /// <summary>Makes a deep copy of this object.</summary>
    /// <returns>A copy of this object.</returns>
    object ICloneable.Clone() => this with { };
}
