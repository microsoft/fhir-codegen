// <copyright file="FhirElementProfile.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Health.Fhir.CodeGenCommon.Models;

/// <summary>A fhir element profile.</summary>
public class FhirElementProfile : ICloneable
{
    /// <summary>Initializes a new instance of the <see cref="FhirElementProfile"/> class.</summary>
    /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
    /// <param name="url">The URL.</param>
    public FhirElementProfile(Uri url)
    {
        if (url == null)
        {
            throw new ArgumentNullException(nameof(url));
        }

        URL = url;
        Name = url.Segments[url.Segments.Length - 1];
    }

    /// <summary>
    /// Initializes a new instance of the
    /// Microsoft.Health.Fhir.CodeGenCommon.Models.FhirElementProfile class.s
    /// </summary>
    /// <param name="source">Source for the.</param>
    public FhirElementProfile(FhirElementProfile source)
    {
        Name = source.Name;
        URL = source.URL;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FhirElementProfile"/> class.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="url"> The URL.</param>
    [System.Text.Json.Serialization.JsonConstructor]
    public FhirElementProfile(
        string name,
        Uri url)
    {
        Name = name;
        URL = url;
    }

    /// <summary>Gets the name.</summary>
    /// <value>The name.</value>
    public string Name { get; }

    /// <summary>Gets URL of the document.</summary>
    /// <value>The URL.</value>
    public Uri URL { get; }

    /// <summary>Returns a string that represents the current object.</summary>
    /// <returns>A string that represents the current object.</returns>
    public override string ToString() => Name;

    /// <summary>Creates a new object that is a copy of the current instance.</summary>
    /// <returns>A new object that is a copy of this instance.</returns>
    public object Clone()
    {
        return new FhirElementProfile(this);
    }

    /// <summary>Deep copy.</summary>
    /// <returns>A FhirElementProfile.</returns>
    public FhirElementProfile DeepCopy()
    {
        return new FhirElementProfile(Name, URL);
    }
}
