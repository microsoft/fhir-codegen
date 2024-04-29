// <copyright file="ComponentDefinition.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>


using System.Diagnostics.CodeAnalysis;
using Hl7.Fhir.Model;
using Microsoft.Health.Fhir.CodeGen.FhirExtensions;
using static Microsoft.Health.Fhir.CodeGenCommon.Extensions.FhirNameConventionExtensions;

namespace Microsoft.Health.Fhir.CodeGen.Models;

/// <summary>
/// A component definition.
/// </summary>
public record class ComponentDefinition
{
    /// <summary>
    /// Gets or sets the structure definition.
    /// </summary>
    public required StructureDefinition Structure { get; init; }

    /// <summary>
    /// Gets or sets the element definition.
    /// </summary>
    public required ElementDefinition Element { get; init; }

    /// <summary>
    /// Gets or sets a value indicating whether this object is the root of the structure.
    /// </summary>
    public required bool IsRootOfStructure { get; init; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ComponentDefinition"/> class.
    /// </summary>
    /// <remarks>
    /// This constructor is used to create a new instance of the ComponentDefinition class.
    /// </remarks>
    public ComponentDefinition() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ComponentDefinition"/> class.
    /// </summary>
    /// <param name="sd">The structure definition.</param>
    [SetsRequiredMembers]
    public ComponentDefinition(StructureDefinition sd)
    {
        Structure = sd;
        Element = sd.cgRootElement()
            ?? throw new InvalidOperationException($"StructureDefinition {sd.Url} does not have a root element.");
        IsRootOfStructure = true;
    }

    /// <summary>
    /// Gets the URL for code generation.
    /// </summary>
    /// <returns>A string representing the URL.</returns>
    public string cgUrl() => Element?.cgUrl(Structure) ?? Structure.Url;

    public string cgExplicitName() => Element?.cgExplicitName() ?? string.Empty;

    /// <summary>
    /// Gets the code generation name.
    /// </summary>
    /// <param name="convention">The naming convention.</param>
    /// <returns>A string representing the code generation name.</returns>
    /// <remarks>Firely uses this version</remarks>
    public string cgName(NamingConvention convention = NamingConvention.PascalCase) => Element.cgNameForExport(convention);

    /// <summary>Get a short description for a component.</summary>
    /// <returns>A string.</returns>
    public string cgShort() =>
        (!string.IsNullOrEmpty(Element?.Short) && !Element.cgHasCodes()) ? Element.Short
        : !string.IsNullOrEmpty(Element?.Definition) ? Element.Definition
        : !string.IsNullOrEmpty(Structure?.Description) ? Structure.Description
        : !string.IsNullOrEmpty(Structure?.Purpose) ? Structure.Purpose
        : string.Empty;

    /// <summary>Get a description for a component</summary>
    /// <returns>A string.</returns>
    public string cgDefinition() =>
        !string.IsNullOrEmpty(Element?.Definition) ? Element.Definition
        : !string.IsNullOrEmpty(Structure?.Description) ? Structure.Description
        : !string.IsNullOrEmpty(Structure?.Purpose) ? Structure.Purpose
        : string.Empty;

    /// <summary>Cg name rooted.</summary>
    /// <param name="convention">(Optional) The convention.</param>
    /// <returns>A string.</returns>
    /// <remarks>TypeScript uses this version</remarks>
    public string cgNameRooted(NamingConvention convention = NamingConvention.PascalCase) => IsRootOfStructure
        ? Structure.Name.ToConvention(convention)
        : Element.cgNameForExport(convention, true);

    /// <summary>
    /// Gets the base type name for code generation.
    /// </summary>
    /// <param name="dc">The device context.</param>
    /// <returns>A string representing the base type name.</returns>
    public string cgBaseTypeName(DefinitionCollection dc, bool usePathForParents)
    {
        string tn = Structure.cgBaseTypeName();
        if (IsRootOfStructure && !string.IsNullOrEmpty(tn))
        {
            return Structure.cgBaseTypeName();
        }

        return Element?.cgBaseTypeName(dc, usePathForParents) ?? tn;
    }

    /// <summary>
    /// Gets the child element definition for code generation.
    /// </summary>
    /// <param name="name">The name of the child element.</param>
    /// <returns>An ElementDefinition representing the child element.</returns>
    public ElementDefinition? cgGetChild(string name)
    {
        string path = Element.Path + (name.StartsWith('.') ? name : "." + name);

        if ((Structure.Snapshot != null) && (Structure.Snapshot.Element.Count != 0))
        {
            return Structure.Snapshot.Element.FirstOrDefault(e => e.Path == path);
        }

        if ((Structure.Differential != null) && (Structure.Differential.Element.Count != 0))
        {
            return Structure.Differential.Element.FirstOrDefault(e => e.Path == path);
        }

        return null;
    }

    /// <summary>
    /// Gets the child component definitions for code generation.
    /// </summary>
    /// <param name="dc">The definition collection.</param>
    /// <param name="includeDescendants">(Optional) True to include, false to exclude the descendants.</param>
    /// <returns>An enumerator that allows foreach to be used to process child component definitions in this collection.</returns>
    public IEnumerable<ComponentDefinition> cgChildComponents(DefinitionCollection dc, bool includeDescendants = false) =>
        Structure.cgComponents(dc, Element, false, !includeDescendants);

    /// <summary>
    /// Gets the validation regular expression for code generation.
    /// </summary>
    /// <returns>A string representing the validation regular expression.</returns>
    public string cgValidationRegEx() => Element?.cgValidationRegEx() ?? string.Empty;

    /// <summary>
    /// Enumerates the elements contained in this component.
    /// </summary>
    /// <param name="includeDescendants">(Optional) True to include, false to exclude the descendants.</param>
    /// <returns>An enumerator that allows foreach to be used to process cg get children in this collection.</returns>
    public IEnumerable<ElementDefinition> cgGetChildren(
        bool includeDescendants = false,
        bool skipSlices = true)
    {
        IEnumerable<ElementDefinition> source;

        int dotCount = Element.Path.Count(c => c == '.');
        int colonCount = Element.ElementId.Count(c => c == ':');

        // filter based on the backbone path we want
        // we want child elements of the requested path, so append an additional dot
        source = (Structure.Snapshot != null) && (Structure.Snapshot.Element.Count != 0)
            ? Structure.Snapshot.Element.Where(e => e.ElementId.StartsWith(Element.ElementId + ".", StringComparison.Ordinal))
            : Structure.Differential.Element.Where(e => e.ElementId.StartsWith(Element.ElementId + ".", StringComparison.Ordinal));

        // traverse our filtered elements
        foreach (ElementDefinition e in source)
        {
            // skip slices and their children
            if (skipSlices && (e.ElementId.Count(c => c == ':') > colonCount))
            {
                continue;
            }

            // if top level only, we need exactly one dot more than the forBackbonePath
            if (!includeDescendants)
            {
                if (e.Path.Count(c => c == '.') != dotCount + 1)
                {
                    continue;
                }
            }

            yield return e;
        }
    }

    public bool DerivesFromDataType(DefinitionCollection dc)
    {
        string btName = cgBaseTypeName(dc, false);

        if ((btName == "DataType") || (btName == "Hl7.Fhir.Model.DataType"))
        {
            return true;
        }

        return dc.PrimitiveTypesByName.ContainsKey(btName) || dc.ComplexTypesByName.ContainsKey(btName);
    }

    public bool DerivesFromElement(DefinitionCollection dc)
    {
        string btName = cgBaseTypeName(dc, false);

        if ((btName == "Element") || (btName == "Hl7.Fhir.Model.Element") ||
            (btName == "BackboneElement") || (btName == "Hl7.Fhir.Model.BackboneElement"))
        {
            return true;
        }

        return !dc.PrimitiveTypesByName.ContainsKey(btName) && !dc.ComplexTypesByName.ContainsKey(btName);

    }
}
