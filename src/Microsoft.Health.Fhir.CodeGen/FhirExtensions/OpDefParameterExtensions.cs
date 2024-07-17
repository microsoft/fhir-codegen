// <copyright file="OpDefParameterExtensions.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using Hl7.Fhir.Model;
using Hl7.Fhir.Utility;
using Microsoft.Health.Fhir.CodeGenCommon.FhirExtensions;

namespace Microsoft.Health.Fhir.CodeGen.FhirExtensions;

public static class OpDefParameterExtensions
{
    public record class ParameterFieldOrder
    {
        public required int FieldOrder { get; init; }
    }

    public static void cgSetFieldOrder(this OperationDefinition.ParameterComponent pc, int fieldOrder)
    {
        ParameterFieldOrder pfo = new()
        {
            FieldOrder = fieldOrder,
        };

        if (pc.HasAnnotation<ParameterFieldOrder>())
        {
            pc.RemoveAnnotations<ParameterFieldOrder>();
        }

        pc.AddAnnotation(pfo);
    }

    /// <summary>Gets the field order.</summary>
    public static int cgFieldOrder(this OperationDefinition.ParameterComponent pc)
    {
        if (pc.TryGetAnnotation(out ParameterFieldOrder? pfo))
        {
            return pfo!.FieldOrder;
        }

        return -1;
    }

    public static string cgCardinality(this OperationDefinition.ParameterComponent pc) => $"{pc.Min ?? 0}..{pc.Max ?? "*"}";

    /// <summary>Gets the cardinality minimum.</summary>
    public static int cgCardinalityMin(this OperationDefinition.ParameterComponent pc) => pc.Min ?? 0;

    /// <summary>Gets the cardinality maximum, -1 for *.</summary>
    public static int cgCardinalityMax(this OperationDefinition.ParameterComponent pc) => pc.Max == "*" ? -1 : int.Parse(pc.Max ?? "0");
}
