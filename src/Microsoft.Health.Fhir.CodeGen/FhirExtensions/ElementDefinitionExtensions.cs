// <copyright file="ElementDefinitionExtensions.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>


using System.Xml.Linq;
using Hl7.Fhir.Model;
using Hl7.Fhir.Utility;
using Microsoft.Health.Fhir.CodeGen.Models;
using Microsoft.Health.Fhir.CodeGen.Utils;
using Microsoft.Health.Fhir.CodeGenCommon.Extensions;
using Microsoft.Health.Fhir.CodeGenCommon.FhirExtensions;
using static Microsoft.Health.Fhir.CodeGenCommon.Extensions.FhirNameConventionExtensions;

namespace Microsoft.Health.Fhir.CodeGen.FhirExtensions;

/// <summary>An element definition extensions.</summary>
public static class ElementDefinitionExtensions
{
    public record class ElementFieldOrder
    {
        public required int FieldOrder { get; init; }
        public required int ComponentFieldOrder { get; init; }
    }

    /// <summary>An ElementDefinition extension method that sets the field orders for an element.</summary>
    /// <param name="ed">                 The ElementDefinition to act on.</param>
    /// <param name="fieldOrder">         The field order.</param>
    /// <param name="componentFieldOrder">The component field order.</param>
    public static void cgSetFieldOrder(this ElementDefinition ed, int fieldOrder, int componentFieldOrder)
    {
        ElementFieldOrder efo = new()
        {
            FieldOrder = fieldOrder,
            ComponentFieldOrder = componentFieldOrder,
        };

        if (ed.HasAnnotation<ElementFieldOrder>())
        {
            ed.RemoveAnnotations<ElementFieldOrder>();
        }

        ed.AddAnnotation(efo);
    }

    /// <summary>Gets the field order (order within the structure it is defined by).</summary>
    public static int cgFieldOrder(this ElementDefinition ed)
    {
        if (ed.TryGetAnnotation(out ElementFieldOrder? efo))
        {
            return efo!.FieldOrder;
        }

        return -1;
    }
    //public static int cgFieldOrder(this ElementDefinition ed) => ed.GetExtensionValue<Hl7.Fhir.Model.Integer>(CommonDefinitions.ExtUrlEdFieldOrder)?.Value ?? -1;

    /// <summary>Gets the component field order for an element (order within its parent path).</summary>
    /// <param name="ed">The ElementDefinition to act on.</param>
    /// <returns>An int.</returns>
    public static int cgComponentFieldOrder(this ElementDefinition ed)
    {
        if (ed.TryGetAnnotation(out ElementFieldOrder? efo))
        {
            return efo!.ComponentFieldOrder;
        }

        return -1;
    }
    //public static int cgComponentFieldOrder(this ElementDefinition ed) => ed.GetExtensionValue<Hl7.Fhir.Model.Integer>(CommonDefinitions.ExtUrlEdComponentFieldOrder)?.Value ?? -1;

    /// <summary>Gets the full path of the base definition.</summary>
    public static string cgBasePath(this ElementDefinition ed) => ed.Base?.Path ?? string.Empty;

    /// <summary>Gets the explicit name of this element if set, or returns an empty string.</summary>
    public static string cgExplicitName(this ElementDefinition ed) => ed.GetExtensionValue<FhirString>(CommonDefinitions.ExtUrlExplicitTypeName)?.ToString() ?? string.Empty;

    /// <summary>Gets the short name of this element, or explicit name if there is one.</summary>
    /// <param name="ed">                The ed to act on.</param>
    /// <param name="allowExplicitName"> (Optional) True to allow, false to suppress the explicit name.</param>
    /// <param name="removeChoiceMarker">(Optional) True to remove choice marker (strip [x]).</param>
    /// <returns>A string.</returns>
    public static string cgName(this ElementDefinition ed, bool allowExplicitName = false, bool removeChoiceMarker = false)
    {
        if (allowExplicitName)
        {
            string en = ed.cgExplicitName();

            if (!string.IsNullOrEmpty(en))
            {
                return en;
            }
        }

        if (removeChoiceMarker && ed.Path.EndsWith("[x]", StringComparison.Ordinal))
        {
            return ed.Path.Split('.').Last()[0..^3];
        }

        return ed.Path.Split('.').Last();
    }

    /// <summary>Get the path of the element, but strip off any choice-type markers ([x]).</summary>
    /// <param name="ed">The ed to act on.</param>
    /// <returns>A string.</returns>
    public static string cgPath(this ElementDefinition ed) => ed.Path.EndsWith("[x]", StringComparison.Ordinal)
        ? ed.Path[0..^3]
        : ed.Path;

    /// <summary>Get a short description for a component.</summary>
    /// <returns>A string.</returns>
    public static string cgShort(this ElementDefinition ed) =>
        !string.IsNullOrEmpty(ed.Short) ? ed.Short
        : !string.IsNullOrEmpty(ed.Definition) ? ed.Definition
        : string.Empty;

    /// <summary>Get the URL for a root element or a relative URL for components.</summary>
    /// <param name="ed">        The ed to act on.</param>
    /// <param name="sd">        The SD.</param>
    /// <param name="convention">(Optional) The convention.</param>
    /// <returns>A string.</returns>
    public static string cgUrl(this ElementDefinition ed, StructureDefinition sd, NamingConvention convention = NamingConvention.PascalCase)
    {
        // check for root element and use the SD url
        if (!ed.Path.Contains('.'))
        {
            return sd.Url;
        }

        // join the sd name and the element name
        return sd.Id.ToPascalCase() + "#" + ed.cgName().ToPascalCase();
    }

    /// <summary>Gets the types.</summary>
    public static IReadOnlyDictionary<string, ElementDefinition.TypeRefComponent> cgTypes(this ElementDefinition ed) => ed.Type.ToDictionary(t => t.cgName(), t => t);

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

    /// <summary>Gets if this element is optional.</summary>
    /// <param name="ed">The ed to act on.</param>
    /// <returns>True if it is optional, false if it is not.</returns>
    public static bool cgIsOptional(this ElementDefinition ed) => ed.Min == 0;

    /// <summary>Gets if this element represents an array of values.</summary>
    /// <param name="ed">The ed to act on.</param>
    /// <returns>True if it is an array, false if it is scalar.</returns>
    public static bool cgIsArray(this ElementDefinition ed) => ed.Max != "1";

    /// <summary>An ElementDefinition extension method that cg is inherited.</summary>
    /// <param name="ed">The ed to act on.</param>
    /// <param name="sd">The SD.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    public static bool cgIsInherited(this ElementDefinition ed, StructureDefinition sd) =>
        ed.Path.StartsWith(sd.Id, StringComparison.Ordinal)
        ? (ed.Base != null)
            ? (ed.Base.Path != ed.Path)
            : !(sd.Differential?.Element?.Any(e => e.Path == ed.Path)) ?? false
        : true;

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
        : $"pattern{ed.Pattern.TypeName}";

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

    /// <summary>Gets the base type name for the given ElementDefinition.</summary>
    /// <param name="ed">                            The ElementDefinition.</param>
    /// <param name="dc">                            The device-context.</param>
    /// <param name="usePathForElementsWithChildren">True to use path for elements with children.
    ///  False will return either Element or BackboneElement for any element that has children.</param>
    /// <param name="typeMap">                       (Optional) The dictionary containing type
    ///  mappings.</param>
    /// <param name="namingConvention">              (Optional) The naming convention.</param>
    /// <returns>The base type name.</returns>
    public static string cgBaseTypeName(
        this ElementDefinition ed,
        DefinitionCollection dc,
        bool usePathForElementsWithChildren,
        Dictionary<string, string>? typeMap = null,
        NamingConvention namingConvention = NamingConvention.PascalCase)
    {
        if (ed.Path.Contains("[x]", StringComparison.Ordinal))
        {
            return string.Empty;
        }

        string value;
        string? mapped;

        if (ed.ContentReference != null)
        {
            if (ed.ContentReference.StartsWith(CommonDefinitions.FhirStructureUrlPrefix, StringComparison.Ordinal))
            {
                value = ed.ContentReference.Split('#').Last();
                return (typeMap?.TryGetValue(value, out mapped) ?? false)
                    ? mapped : value;
            }

            if (ed.ContentReference.StartsWith('#'))
            {
                value = ed.ContentReference.Substring(1);
                return (typeMap?.TryGetValue(value, out mapped) ?? false)
                    ? mapped : value;
            }

            value = ed.ContentReference;
            return (typeMap?.TryGetValue(value, out mapped) ?? false)
                ? mapped : value;
        }

        // check to see if we are in an extension
        string lastSegment = ed.Path.Split('.').Last();
        switch (lastSegment)
        {
            case "extension":
            case "Extension":
                return "Extension";

            case "modifierExtension":
            case "ModifierExtension":
                return "Extension";
        }

        // check for having child elements
        if (dc.IsBackboneElement(ed.Path))       // (dc.HasChildElements(ed.Path))
        {
            if (usePathForElementsWithChildren)
            {
                return ed.Path;
            }

            if (ed.Type.Any(et => et.Code == "Element"))
            {
                return "Element";
            }

            return "BackboneElement";
        }

        // check for no types
        if (ed.Type.Count == 0)
        {
            return "Element";
        }

        // check for single type
        if (ed.Type.Count == 1)
        {
            value = ed.Type.First().cgName();
            return (typeMap?.TryGetValue(value, out mapped) ?? false)
                ? mapped : value;
        }

        // no derivable base type name
        return string.Empty;
    }

    /// <summary>Get a flag for if this element should have codes.</summary>
    /// <param name="ed">The ed to act on.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    public static bool cgHasCodes(this ElementDefinition ed) =>
        (ed.Binding?.Strength == BindingStrength.Required) && (ed.Type?.Any(tr => tr.Code == "code") ?? false);

    /// <summary>Gets the required codes for this element.</summary>
    /// <param name="ed">       The ed to act on.</param>
    /// <param name="valueSets">Sets the value belongs to.</param>
    /// <returns>
    /// An enumerator that allows foreach to be used to process cg codes in this collection.
    /// </returns>
    public static IEnumerable<string> cgCodes(this ElementDefinition ed, DefinitionCollection definitions)
    {
        // if the binding is not required, we don't need to generate the codes
        if ((ed.Binding == null) || (ed.Binding.Strength != BindingStrength.Required))
        {
            return [];
        }

        // only generate codes for elements of type code, string, uri, url, or canonical
        if (!ed.Type.Any(tr =>
                (tr.Code == "code") ||
                (tr.Code == "string") ||
                (tr.Code == "uri") ||
                (tr.Code == "url") ||
                (tr.Code == "canonical")))
        {
            return [];
        }

        ValueSet? vs = null;

        try
        {
            // TODO(ginoc): backwards-compatibility says we should use 'starter' in place of actual binding for CS.format
            // On the other hand, Attachment.language looks like we should ignore.  What do we want?
            // check for additional bindings
            if ((ed.Path == "CapabilityStatement.format") &&
                ed.Binding.Additional.Any(a => a.Purpose == ElementDefinition.AdditionalBindingPurposeVS.Starter))
            {
                ElementDefinition.AdditionalComponent add = ed.Binding.Additional.First(a => a.Purpose == ElementDefinition.AdditionalBindingPurposeVS.Starter);

                vs = definitions.ExpandVs(add.ValueSet).Result;
                //vs = definitions.ResolveVs(add.ValueSet);
            }

            vs ??= definitions.ExpandVs(ed.Binding.ValueSet).Result;

            return vs?.Expansion?.Contains?.Select(c => c.Code) ?? [];
        }
        catch (Exception ex)
        {
            if (ex.InnerException == null)
            {
                Console.WriteLine($"Error resolving value set {ed.Binding.ValueSet}: {ex.Message}");
            }
            else
            {
                Console.WriteLine($"Error resolving value set {ed.Binding.ValueSet}: {ex.Message}: {ex.InnerException}");
            }
            return [];
        }
    }

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
        string name = ed.Path.Split('.').Last();

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

                    string nc = name.ToPascalCase();

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

                    value = name.ToCamelCase();

                    if ((reservedWords != null) &&
                        reservedWords.Contains(value))
                    {
                        // note we use capitalized (pascal) for appending here since the prefix is lower-cased
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

                    value = name.ToUpperCase();

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

                    value = name.ToLowerCase();

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

    /// <summary>Enumerates cg extract base types in this collection.</summary>
    /// <param name="elementCollections">The elements to act on.</param>
    /// <returns>
    /// An enumerator that allows foreach to be used to process cg extract base types in this
    /// collection.
    /// </returns>
    public static IEnumerable<string> cgExtractBaseTypes(this IEnumerable<StructureElementCollection> elementCollections, DefinitionCollection dc)
    {
        HashSet<string> seen = [];

        foreach (StructureElementCollection elementCollection in elementCollections)
        {
            foreach (ElementDefinition ed in elementCollection.Elements)
            {
                string bt;

                switch (elementCollection.Structure.cgArtifactClass())
                {
                    case CodeGenCommon.Models.FhirArtifactClassEnum.PrimitiveType:
                        bt = elementCollection.Structure.Id;
                        break;

                    case CodeGenCommon.Models.FhirArtifactClassEnum.ComplexType:
                        {
                            bt = elementCollection.Structure.cgBaseTypeName();
                            if (bt != "Quantity")
                            {
                                bt = elementCollection.Structure.Id;
                            }
                        }
                        break;

                    case CodeGenCommon.Models.FhirArtifactClassEnum.Resource:
                    case CodeGenCommon.Models.FhirArtifactClassEnum.LogicalModel:
                        bt = elementCollection.Structure.Id;
                        break;

                    case CodeGenCommon.Models.FhirArtifactClassEnum.Profile:
                    default:
                        bt = elementCollection.Structure.cgBaseTypeName();
                        break;
                }

                //// get the base type out of the structure
                //bt = sd.cgBaseTypeName();

                //// for first-order types, we want the actual type name
                //switch (bt)
                //{
                //    case "DataType":
                //    case "DomainResource":
                //    case "Resource":
                //        bt = sd.Id;
                //        break;
                //}

                if (seen.Contains(bt))
                {
                    continue;
                }

                seen.Add(bt);

                yield return bt;
            }
        }
    }
}
