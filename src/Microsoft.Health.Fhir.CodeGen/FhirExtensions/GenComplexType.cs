// <copyright file="GenComplexType.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>


using Hl7.Fhir.Model;

namespace Microsoft.Health.Fhir.CodeGen.FhirExtensions;

/// <summary>Interface for a code-gen complex type.</summary>
public interface IGenComplexType
{
    /// <summary>Gets or sets the identifier.</summary>
    string Id { get; }

    /// <summary>Gets the name.</summary>
    string Name { get; }

    /// <summary>Gets the short.</summary>
    string Short { get; }

    /// <summary>Gets the definition.</summary>
    string Definition { get; }

    /// <summary>Gets the comment.</summary>
    string Comment { get; }
}

/// <summary>Code-gen complex type extensions.</summary>
public static class IComplexTypeExtensions
{
    public static IGenComplexType AsComplexType(this StructureDefinition sd)
    {
        if (sd == null)
        {
            throw new ArgumentNullException(nameof(sd), "StructureDefinition cannot be null");
        }

        if (sd.Derivation == StructureDefinition.TypeDerivationRule.Constraint)
        {
            throw new ArgumentException("Cannot view a constraining type as a complex type (derivation == constraint)", nameof(sd));
        }

        if (sd.Kind != StructureDefinition.StructureDefinitionKind.ComplexType)
        {
            throw new ArgumentException("StructureDefinition must be a complex type", nameof(sd));
        }

        return (sd as IGenComplexType)!;
    }

}

/// <summary>A code-gen complex type.</summary>
public class GenComplexType : Hl7.Fhir.Model.StructureDefinition, IGenComplexType
{
    /// <summary>Gets the short.</summary>
    /// <remarks>right now, differential is generally 'more correct' than snapshot for primitives, see FHIR-37465</remarks>
    public string Short => Differential.Element.Any() ? Differential.Element[0].Short : Description;

    /// <summary>Gets the definition.</summary>
    /// <remarks>right now, differential is generally 'more correct' than snapshot for primitives, see FHIR-37465</remarks>
    public string Definition => Differential.Element.Any() ? Differential.Element[0].Definition : Purpose;

    /// <summary>Gets the comment.</summary>
    /// <remarks>right now, differential is generally 'more correct' than snapshot for primitives, see FHIR-37465</remarks>
    public string Comment => Differential.Element.Any() ? Differential.Element[0].Comment : string.Empty;
}
