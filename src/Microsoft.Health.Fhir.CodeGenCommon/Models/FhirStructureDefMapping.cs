// <copyright file="FhirStructureDefMapping.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

namespace Microsoft.Health.Fhir.CodeGenCommon.Models;

/// <summary>External specification that the content is mapped to - e.g., FiveWs, RIM, Workflow.</summary>
public class FhirStructureDefMapping : ICloneable
{
    /// <summary>Gets or sets the internal id when this mapping is used.</summary>
    public string Identity { get; set; } = "";

    /// <summary>Gets or sets canonical uri - identifies what this mapping refers to.</summary>
    public string CanonicalUri { get; set; } = "";

    /// <summary>Gets or sets the name of what this mapping refers to.</summary>
    public string Name { get; set; } = "";

    /// <summary>Gets or sets the comment - Versions, Issues, Scope limitations etc..</summary>
    public string Comment { get; set; } = "";

    /// <summary>Creates a new object that is a copy of the current instance.</summary>
    /// <returns>A new object that is a copy of this instance.</returns>
    public object Clone()
    {
        return new FhirStructureDefMapping()
        {
            Identity = Identity,
            CanonicalUri = CanonicalUri,
            Name = Name,
            Comment = Comment,
        };
    }
}
