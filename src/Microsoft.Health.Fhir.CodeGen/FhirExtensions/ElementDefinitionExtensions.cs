// <copyright file="ElementDefinitionExtensions.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>


using System.Xml.Linq;
using Hl7.Fhir.Model;
using Microsoft.Health.Fhir.CodeGenCommon.Extensions;
using static Microsoft.Health.Fhir.CodeGenCommon.Extensions.FhirNameConventionExtensions;

namespace Microsoft.Health.Fhir.CodeGen.FhirExtensions;

/// <summary>An element definition extensions.</summary>
public static class ElementDefinitionExtensions
{
    /// <summary>Gets the field order.</summary>
    public static int cgFieldOrder(this ElementDefinition ed) => ed.GetExtensionValue<Hl7.Fhir.Model.Integer>(CommonDefinitions.ExtUrlFieldOrder)?.Value ?? -1;

    /// <summary>Gets the full path of the base definition.</summary>
    public static string cgBasePath(this ElementDefinition ed) => ed.Base?.Path ?? string.Empty;

    /// <summary>Gets the explicit name of this element if set, or returns an empty string.</summary>
    public static string cgExplicitName(this ElementDefinition ed) => ed.GetExtensionValue<FhirString>(CommonDefinitions.ExtUrlExplicitTypeName)?.ToString() ?? string.Empty;

    /// <summary>Gets the types.</summary>
    public static IReadOnlyDictionary<string, ElementDefinition.TypeRefComponent> cgTypes(this ElementDefinition ed) => ed.Type.ToDictionary(t => t.Code, t => t);

    /// <summary>An ElementDefinition extension method that cg cardinality.</summary>
    /// <param name="ed">The ed to act on.</param>
    /// <returns>A string.</returns>
    public static string cgCardinality(this ElementDefinition ed) => $"{ed.Min ?? 0}..{ed.Max ?? "*"}";

    /// <summary>Gets the cardinality minimum.</summary>
    public static int cgCardinalityMin(this ElementDefinition ed) => ed.Min ?? 0;

    /// <summary>Gets the cardinality maximum, -1 for *.</summary>
    public static int cgCardinalityMax(this ElementDefinition ed) => ed.Max == "*" ? -1 : int.Parse(ed.Max ?? "0");

    /// <summary>Gets if this element is defined as a simple type.</summary>
    /// <param name="ed">The ed to act on.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    public static bool cgIsSimple(this ElementDefinition ed) => ed.Type.Any(trc => trc.cgIsSimpleType());

    /// <summary>Gets the first validation regex defined for an element or an empty string</summary>
    /// <param name="ed">The ed to act on.</param>
    /// <returns>A string.</returns>
    public static string cgValidationRegEx(this ElementDefinition ed) =>
        ed.Type.Where(trc => !string.IsNullOrEmpty(trc.cgRegex()))
            .Select(trc => trc.cgRegex())
            .FirstOrDefault()
        ?? string.Empty;

    /// <summary>Gets the default value field name (if present) or an empty string.</summary>
    /// <param name="ed">The ed to act on.</param>
    /// <returns>A string.</returns>
    public static string cgDefaultFieldName(this ElementDefinition ed) =>
        ed.DefaultValue == null
        ? string.Empty
        : $"defaultValue{ed.DefaultValue.TypeName}";

    /// <summary>Gets the fixed field name (if present) or an empty string.</summary>
    /// <param name="ed">The ed to act on.</param>
    /// <returns>A string.</returns>
    public static string cgFixedFieldName(this ElementDefinition ed) =>
        ed.Fixed == null
        ? string.Empty
        : $"fixed{ed.Fixed.TypeName}";

    /// <summary>Gets the pattern field name (if present) or an empty string.</summary>
    /// <param name="ed">The ed to act on.</param>
    /// <returns>A string.</returns>
    public static string cgPatternFieldName(this ElementDefinition ed) =>
        ed.Pattern == null
        ? string.Empty
        : $"pattern{ed.Fixed.TypeName}";

    /// <summary>Get an explicit binding name.</summary>
    /// <param name="ed">The ed to act on.</param>
    /// <returns>A string.</returns>
    public static string cgBindingName(this ElementDefinition ed) =>
        ed.Binding?.GetExtensionValue<FhirString>(CommonDefinitions.ExtUrlBindingName)?.ToString()
        ?? string.Empty;

    /// <summary>Get a flag for if this binding is a "common" binding.</summary>
    /// <param name="ed">The ed to act on.</param>
    /// <returns>True if the binding is common, false if it is not.</returns>
    public static bool cgBindingIsCommon(this ElementDefinition ed) =>
        ed.Binding?.GetExtensionValue<FhirBoolean>(CommonDefinitions.ExtUrlIsCommonBinding)?.Value
        ?? false;

    /// <summary>An ElementDefinition extension method that cg name for export.</summary>
    /// <exception cref="ArgumentException">Thrown when one or more arguments have unsupported or
    ///  illegal values.</exception>
    /// <param name="ed">                    The ed to act on.</param>
    /// <param name="convention">            The convention.</param>
    /// <param name="concatenatePath">       (Optional) True to concatenate path.</param>
    /// <param name="concatenationDelimiter">(Optional) The concatenation delimiter.</param>
    /// <param name="reservedWords">         (Optional) The reserved words.</param>
    /// <returns>A string.</returns>
    public static string cgNameForExport(
        this ElementDefinition ed,
        NamingConvention convention,
        bool concatenatePath = false,
        string concatenationDelimiter = "",
        HashSet<string>? reservedWords = null)
    {
        switch (convention)
        {
            case NamingConvention.FhirDotNotation:
                {
                    if ((reservedWords != null) &&
                        reservedWords.Contains(ed.Path))
                    {
                        return "Fhir" + ed.Path;
                    }

                    return ed.Path;
                }

            case NamingConvention.PascalDotNotation:
                {
                    string value = ed.Path.ToPascalDotCase();

                    if ((reservedWords != null) &&
                        reservedWords.Contains(value))
                    {
                        return "Fhir" + value;
                    }

                    return value;
                }

            case NamingConvention.PascalCase:
                {
                    if (concatenatePath)
                    {
                        string value = ed.Path.ToPascalCase(true, concatenationDelimiter);

                        if ((reservedWords != null) &&
                            reservedWords.Contains(value))
                        {
                            return "Fhir" + value;
                        }

                        return value;
                    }

                    string nc = ed.cgNameForExport(NamingConvention.PascalCase, false);

                    if ((reservedWords != null) &&
                        reservedWords.Contains(nc))
                    {
                        return "Fhir" + nc;
                    }

                    return nc;
                }

            case NamingConvention.CamelCase:
                {
                    string value;

                    if (concatenatePath)
                    {
                        value = ed.Path.ToCamelCase(true, concatenationDelimiter);

                        if ((reservedWords != null) &&
                            reservedWords.Contains(value))
                        {
                            // change the main value to Pascal case since we are prefixing with lower case
                            return "fhir" + value.ToPascalCase(false);
                        }

                        return value;
                    }

                    value = ed.cgNameForExport(NamingConvention.CamelCase, false);

                    if ((reservedWords != null) &&
                        reservedWords.Contains(value))
                    {
                        // note we use capitialized (pascal) for appending here since the prefix is lower-cased
                        return "fhir" + ed.cgNameForExport(NamingConvention.PascalCase, false);
                    }

                    return value;
                }

            case NamingConvention.UpperCase:
                {
                    string value;

                    if (concatenatePath)
                    {
                        value = ed.Path.ToUpperCase(true, concatenationDelimiter);

                        if ((reservedWords != null) &&
                            reservedWords.Contains(value))
                        {
                            return "FHIR_" + value;
                        }

                        return value;
                    }

                    value = ed.cgNameForExport(NamingConvention.UpperCase, false);

                    if ((reservedWords != null) &&
                        reservedWords.Contains(value))
                    {
                        return "FHIR" + value;
                    }

                    return value;
                }

            case NamingConvention.LowerCase:
                {
                    string value;

                    if (concatenatePath)
                    {
                        value = ed.Path.ToLowerCase(true, concatenationDelimiter);

                        if ((reservedWords != null) &&
                            reservedWords.Contains(value))
                        {
                            return "fhir_" + value;
                        }
                    }

                    value = ed.cgNameForExport(NamingConvention.LowerCase, false);

                    if ((reservedWords != null) &&
                        reservedWords.Contains(value))
                    {
                        return "fhir" + value;
                    }

                    return value;
                }

            case NamingConvention.None:
            default:
                throw new ArgumentException($"Invalid Naming Convention: {convention}");
        }
    }
}
