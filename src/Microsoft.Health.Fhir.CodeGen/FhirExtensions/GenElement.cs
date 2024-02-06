// <copyright file="GenElement.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using Hl7.Fhir.Model;

namespace Microsoft.Health.Fhir.CodeGen.FhirExtensions;

/// <summary>Interface for code-gen element definitions.</summary>
public interface IGenElement
{
}

/// <summary>Code-gen element definition extensions.</summary>
public static class IGenElementExtensions
{
    public static IGenElement AsElement(this ElementDefinition ed)
    {
        if (ed == null)
        {
            throw new ArgumentNullException(nameof(ed), "ElementDefinition cannot be null");
        }

        return (ed as IGenElement)!;
    }
}

/// <summary>A code-gen element definition.</summary>
public class GenElement : ElementDefinition, IGenElement
{
}
