// <copyright file="FhirValueSetExpansionContains.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Health.Fhir.CodeGenCommon.Models;

/// <summary>A fhir value set expansion contains.</summary>
public class FhirValueSetExpansionContains
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FhirValueSetExpansionContains"/> class.
    /// </summary>
    /// <param name="system">    The system.</param>
    /// <param name="isAbstract">The abstract.</param>
    /// <param name="isInactive">The inactive.</param>
    /// <param name="version">   The version.</param>
    /// <param name="code">      The code.</param>
    /// <param name="display">   The display.</param>
    /// <param name="contains">  The contains.</param>
    public FhirValueSetExpansionContains(
        Uri system,
        bool? isAbstract,
        bool? isInactive,
        string version,
        string code,
        string display,
        List<FhirValueSetExpansionContains> contains)
    {
        System = system;
        IsAbstract = isAbstract;
        IsInactive = isInactive;
        Version = version;
        Code = code;
        Display = display;
        Contains = contains;
    }

    /// <summary>Gets the system.</summary>
    /// <value>The system.</value>
    public Uri System { get; }

    /// <summary>Gets the abstract.</summary>
    /// <value>The abstract.</value>
    public bool? IsAbstract { get; }

    /// <summary>Gets the inactive.</summary>
    /// <value>The inactive.</value>
    public bool? IsInactive { get; }

    /// <summary>Gets the version.</summary>
    /// <value>The version.</value>
    public string Version { get; }

    /// <summary>Gets the code.</summary>
    /// <value>The code.</value>
    public string Code { get; }

    /// <summary>Gets the display.</summary>
    /// <value>The display.</value>
    public string Display { get; }

    /// <summary>Gets the contains.</summary>
    /// <value>The contains.</value>
    public List<FhirValueSetExpansionContains> Contains { get; }
}
