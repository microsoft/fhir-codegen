// <copyright file="CqlCommon.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Health.Fhir.CodeGen.Language.Cql;

/// <summary>
/// Represents the parameters for CQL FHIR code generation.
/// </summary>
public class CqlFhirParameters
{
    /// <summary>
    /// Gets or sets the package ID.
    /// </summary>
    public string PackageId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the package version.
    /// </summary>
    public string? PackageVersion { get; set; } = null;

    /// <summary>
    /// Gets the set of supersedes.
    /// </summary>
    public HashSet<string> Supersedes { get; } = new();

    /// <summary>
    /// Gets the model properties specified in the parameters.
    /// </summary>
    public Dictionary<string, string> ModelProperties { get; } = new();

    /// <summary>
    /// Gets the structure properties, per structure, as specified in the parameters.
    /// </summary>
    public Dictionary<string, Dictionary<string, string>> StructureProperties { get; } = new();
}

public static class CqlCommon
{
}
