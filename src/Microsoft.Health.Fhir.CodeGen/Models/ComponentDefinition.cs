// <copyright file="ComponentDefinition.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>


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
    public string cgName(NamingConvention convention = NamingConvention.PascalCase) => IsRootOfStructure
        ? Element.cgNameForExport(convention)
        : Element.cgNameForExport(convention, true);

    /// <summary>
    /// Gets the base type name for code generation.
    /// </summary>
    /// <param name="dc">The device context.</param>
    /// <returns>A string representing the base type name.</returns>
    public string cgBaseTypeName(DefinitionCollection dc) => Element?.cgBaseTypeName(dc) ?? Structure.cgBaseTypeName();

    /// <summary>
    /// Gets the child element definition for code generation.
    /// </summary>
    /// <param name="name">The name of the child element.</param>
    /// <returns>An ElementDefinition representing the child element.</returns>
    public ElementDefinition? cgGetChild(string name)
    {
        string path = Element.Path + (name.StartsWith('.') ? name : "." + name);

        if (Structure.Snapshot?.Element.Any() ?? false)
        {
            return Structure.Snapshot.Element.FirstOrDefault(e => e.Path.Equals(path, StringComparison.Ordinal));
        }

        if (Structure.Differential?.Element.Any() ?? false)
        {
            return Structure.Differential.Element.FirstOrDefault(e => e.Path.Equals(path, StringComparison.Ordinal));
        }

        return null;
    }

    /// <summary>
    /// Enumerates cg get children in this collection.
    /// </summary>
    /// <param name="includeDescendants">(Optional) True to include, false to exclude the descendants.</param>
    /// <returns>An enumerator that allows foreach to be used to process cg get children in this collection.</returns>
    public IEnumerable<ElementDefinition> cgGetChildren(bool includeDescendants = false) => Structure.cgElements(Element.Path, !includeDescendants, false);

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
}
