// <copyright file="BindStrengthComparer.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using Hl7.Fhir.Model;

namespace Microsoft.Health.Fhir.CodeGen.FhirExtensions;


/// <summary>A binding strength comparer.</summary>
public class BindingStrengthComparer : IComparer<BindingStrength?>
{
    /// <summary>(Immutable) A static instance for ease of use.</summary>
    public static readonly BindingStrengthComparer Instance = new();

    // <summary>
    // Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
    // </summary>
    // <param name="x">The first object to compare.</param>
    // <param name="y">The second object to compare.</param>
    // <returns>
    // A signed integer that indicates the relative values of <paramref name="x" /> and <paramref name="y" />,
    // as shown in the following table.  
    // 
    //  <list type="table"><listheader><term> Value</term><description> Meaning</description></listheader>
    //  <item><term> Less than zero</term><description><paramref name="x" /> is less than <paramref name="y" />.
    //  </description></item><item><term> Zero</term><description><paramref name="x" /> equals <paramref name="y" />.
    //  </description></item><item><term> Greater than zero</term><description><paramref name="x" />
    //  is greater than <paramref name="y" />.</description></item></list>
    // </returns>
    public int Compare(BindingStrength? x, BindingStrength? y) => x switch
    {
        null => y switch
        {
            null => 0,
            _ => -1
        },
        BindingStrength.Required => y switch
        {
            BindingStrength.Required => 0,
            BindingStrength.Extensible => 1,
            BindingStrength.Preferred => 1,
            BindingStrength.Example => 1,
            null => 1,
            _ => -1
        },
        BindingStrength.Extensible => y switch
        {
            BindingStrength.Required => -1,
            BindingStrength.Extensible => 0,
            BindingStrength.Preferred => 1,
            BindingStrength.Example => 1,
            null => 1,
            _ => -1
        },
        BindingStrength.Preferred => y switch
        {
            BindingStrength.Required => -1,
            BindingStrength.Extensible => -1,
            BindingStrength.Preferred => 0,
            BindingStrength.Example => 1,
            null => 1,
            _ => -1
        },
        BindingStrength.Example => y switch
        {
            BindingStrength.Required => -1,
            BindingStrength.Extensible => -1,
            BindingStrength.Preferred => -1,
            BindingStrength.Example => 0,
            null => 1,
            _ => -1
        },
        _ => -1
    };
}
