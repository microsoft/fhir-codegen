// <copyright file="FhirElementDefMapping.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

namespace Microsoft.Health.Fhir.CodeGenCommon.Models;

/// <summary>A FHIR element definition mapping - map an element to another set of definitions.</summary>
public class FhirElementDefMapping : ICloneable
{
    /// <summary>Gets or sets the reference to mapping declaration.</summary>
    public string Identity { get; set; } = "";

    /// <summary>Gets or sets the computable language of mapping.</summary>
    public string Language { get; set; } = "";
    
    /// <summary>Gets or sets the details of the mapping.</summary>
    public string Map { get; set; } = "";

    /// <summary>Gets or sets comments about the mapping or its use.</summary>
    public string Comment { get; set; } = "";

    /// <summary>Creates a new object that is a copy of the current instance.</summary>
    /// <returns>A new object that is a copy of this instance.</returns>
    public object Clone()
    {
        return new FhirElementDefMapping()
        {
            Identity = Identity,
            Language = Language,
            Map = Map,
            Comment = Comment,
        };
    }
}
