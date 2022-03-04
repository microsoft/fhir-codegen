// <copyright file="FhirValueSetExpansion.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Health.Fhir.CodeGenCommon.Models;

/// <summary>A fhir value set expansion.</summary>
public class FhirValueSetExpansion : ICloneable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FhirValueSetExpansion"/> class.
    /// </summary>
    /// <param name="identifier">The identifier.</param>
    /// <param name="timestamp"> The timestamp.</param>
    /// <param name="total">     The total.</param>
    /// <param name="offset">    The offset.</param>
    /// <param name="parameters">The parameters.</param>
    /// <param name="contains">  The contains.</param>
    public FhirValueSetExpansion(
        string identifier,
        string timestamp,
        int? total,
        int? offset,
        Dictionary<string, dynamic> parameters,
        List<FhirConcept> contains)
    {
        Identifier = identifier;
        Timestamp = timestamp;
        Total = total;
        Offset = offset;
        Parameters = parameters;
        Contains = contains;
    }

    /// <summary>Gets the identifier.</summary>
    /// <value>The identifier.</value>
    public string Identifier { get; }

    /// <summary>Gets the Date/Time of the timestamp.</summary>
    /// <value>The timestamp.</value>
    public string Timestamp { get; }

    /// <summary>Gets the number of. </summary>
    /// <value>The total.</value>
    public int? Total { get; }

    /// <summary>Gets the offset.</summary>
    /// <value>The offset.</value>
    public int? Offset { get; }

    /// <summary>Gets options for controlling the operation.</summary>
    /// <value>The parameters.</value>
    public Dictionary<string, dynamic> Parameters { get; }

    /// <summary>Gets the contains.</summary>
    /// <value>The contains.</value>
    public List<FhirConcept> Contains { get; }

    /// <summary>Makes a deep copy of this object.</summary>
    /// <returns>A copy of this object.</returns>
    public object Clone()
    {
        Dictionary<string, dynamic> parameters = null;

        if (Parameters != null)
        {
            parameters = new Dictionary<string, dynamic>();
            foreach (KeyValuePair<string, dynamic> kvp in Parameters)
            {
                Type type = kvp.Value.GetType();

                if (type.IsValueType)
                {
                    parameters.Add(kvp.Key, kvp.Value);
                    continue;
                }

                parameters.Add(kvp.Key, kvp.Value.Clone());
            }
        }

        List<FhirConcept> contains = null;

        if (Contains != null)
        {
            contains = Contains.Select(c => (FhirConcept)c.Clone()).ToList();
        }

        return new FhirValueSetExpansion(
            Identifier,
            Timestamp,
            Total,
            Offset,
            parameters,
            contains);
    }
}
