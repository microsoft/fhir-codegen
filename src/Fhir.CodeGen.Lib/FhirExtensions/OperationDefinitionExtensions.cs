// <copyright file="OperationDefinitionExtensions.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using Hl7.Fhir.Model;
using Fhir.CodeGen.Common.FhirExtensions;
using Fhir.CodeGen.Common.Models;

namespace Fhir.CodeGen.Lib.FhirExtensions;

public static class OperationDefinitionExtensions
{
    /// <summary>An OperationDefinition extension method that cg artifact class.</summary>
    /// <param name="op">The op to act on.</param>
    /// <returns>A FhirArtifactClassEnum.</returns>
    public static FhirArtifactClassEnum cgArtifactClass(this OperationDefinition op) => FhirArtifactClassEnum.Operation;

    /// <summary>Gets a flag indicating if this definition is experimental.</summary>
    /// <param name="sd">The SD to act on.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    public static bool cgIsExperimental(this OperationDefinition sd) => sd.Experimental ?? false;

}
