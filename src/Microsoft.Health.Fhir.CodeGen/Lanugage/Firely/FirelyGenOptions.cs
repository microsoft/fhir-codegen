// <copyright file="FirelyOptions.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>


using System.ComponentModel;
using Microsoft.Health.Fhir.CodeGen.Configuration;
using Microsoft.Health.Fhir.CodeGen.Extensions;

namespace Microsoft.Health.Fhir.CodeGen.Lanugage.Firely;

/// <summary>Firely generation options.</summary>
public class FirelyGenOptions : ConfigGenerate
{
    /// <summary>
    /// Gets or sets the subset of language exports to make.
    /// </summary>
    [ConfigOption(
        ArgName = "--subset",
        Description = "Which subset of language exports to make.")]
    public CSharpFirelyCommon.GenSubset Subset { get; set; } = CSharpFirelyCommon.GenSubset.Satellite;

    /// <summary>
    /// Gets or sets a value indicating whether the output should include 5 W's mappings.
    /// </summary>
    [ConfigOption(
        ArgName = "--w5",
        Description = "If output should include 5W's mappings.")]
    public bool ExportFiveWs { get; set; } = false;

    /// <summary>Gets or sets the cql model.</summary>
    [ConfigOption(
        ArgName = "--cql-model",
        Description = "Name of the Cql model for which metadata attributes should be added to the pocos. 'Fhir401' is the only valid value at the moment.")]
    public string CqlModel { get; set; } = string.Empty;

}
