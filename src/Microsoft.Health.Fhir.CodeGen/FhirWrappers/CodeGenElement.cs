// <copyright file="IGenElement.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>


using Hl7.Fhir.Model;

namespace Microsoft.Health.Fhir.CodeGen.FhirWrappers;

/// <summary>A code generate element.</summary>
public class CodeGenElement : ElementDefinition
{
    /// <summary>
    /// Initializes a new instance of the Microsoft.Health.Fhir.CodeGen.FhirWrappers.CodeGenElement
    /// class.
    /// </summary>
    /// <param name="ed">    The ed.</param>
    /// <param name="parent">Or initializes the parent.</param>
    public CodeGenElement(ElementDefinition ed, StructureDefinition parent)
    {
        Parent = parent;
        FieldOrder = (parent.Snapshot?.Element?.Any() ?? false)
            ? parent.Snapshot!.Element!.FindIndex((e) => e.Path.Equals(ed.Path, StringComparison.Ordinal))
            : (parent.Differential?.Element?.FindIndex((e) => e.Path.Equals(ed.Path, StringComparison.Ordinal)) ?? 0);

        // copy the contents
        ed.CopyTo(this);
    }

    /// <summary>Gets or initializes the parent.</summary>
    public StructureDefinition Parent { get; init; }

    /// <summary>Gets or sets the field order.</summary>
    public int FieldOrder { get; set; }

    /// <summary>Gets the full pathname of the base file.</summary>
    public string BasePath => Base?.Path ?? string.Empty;

    /// <summary>Gets the name of the explicit.</summary>
    public string ExplicitName => this.GetExtensionValue<FhirString>(CommonDefinitions.ExtUrlExplicitTypeName)?.ToString() ?? string.Empty;
}
