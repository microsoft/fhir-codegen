// <copyright file="FhirCode.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Health.Fhir.CodeGenCommon.Models;

/// <summary>A FHIR Code value object.</summary>
public static class FhirCode
{
    /// <summary>The codes.</summary>
    private static Dictionary<string, string[]> _codes = new Dictionary<string, string[]>();

    /// <summary>Adds the codes to 'values'.</summary>
    ///
    /// <param name="name">  The name.</param>
    /// <param name="values">The values.</param>
    ///
    /// <returns>True if it succeeds, false if it fails.</returns>
    public static bool AddCodes(string name, string[] values)
    {
        if (_codes.ContainsKey(name))
        {
            // should be same
            if ((values == null) ||
                (_codes[name].Length != values.Length))
            {
                Console.WriteLine($"Duplicate Code {name} with different values!");
                return false;
            }

            return true;
        }

        // add this code
        _codes.Add(name, values);

        // success
        return true;
    }

    /// <summary>Attempts to get codes a string[] from the given string.</summary>
    ///
    /// <param name="name">  The name.</param>
    /// <param name="values">[out] The values.</param>
    ///
    /// <returns>True if it succeeds, false if it fails.</returns>
    public static bool TryGetCodes(string name, out string[] values)
    {
        if (!_codes.ContainsKey(name))
        {
            values = null;
            return false;
        }

        values = _codes[name];
        return true;
    }
}
