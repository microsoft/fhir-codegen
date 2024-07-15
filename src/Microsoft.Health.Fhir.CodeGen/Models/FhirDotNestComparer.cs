// <copyright file="FhirDotNestComparer.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

namespace Microsoft.Health.Fhir.CodeGen.Models;

public class FhirDotNestComparer : IComparer<string>
{
    public static readonly FhirDotNestComparer Instance = new();

    /// <summary>
    /// Compares two objects and returns a value indicating whether one is less than, equal to, or
    /// greater than the other.
    /// </summary>
    /// <param name="x">The first object to compare.</param>
    /// <param name="y">The second object to compare.</param>
    /// <returns>
    /// A signed integer that indicates the relative values of <paramref name="x" /> and <paramref name="y" />,
    /// as shown in the following table.  
    /// 
    ///  <list type="table"><listheader><term> Value</term><description> Meaning</description></listheader>
    ///  <item><term> Less than zero</term><description><paramref name="x" /> is less than <paramref name="y" />.
    ///  </description></item><item><term> Zero</term><description><paramref name="x" /> equals <paramref name="y" />.
    ///  </description></item><item><term> Greater than zero</term><description><paramref name="x" />
    ///  is greater than <paramref name="y" />.</description></item></list>
    /// </returns>
    public int Compare(string? x, string? y)
    {
        if (x == null)
        {
            return y == null ? 0 : -1;
        }

        if (y == null)
        {
            return 1;
        }

#if NET8_0_OR_GREATER
        string[] xComponents = x.Split('.', StringSplitOptions.RemoveEmptyEntries);
        string[] yComponents = y.Split('.', StringSplitOptions.RemoveEmptyEntries);
#else
        string[] xComponents = x.Split('.');
        string[] yComponents = y.Split('.');
#endif

        // get enumerators for each of the arrays
        using IEnumerator<string> xE = ((IEnumerable<string>)xComponents).GetEnumerator();
        using IEnumerator<string> yE = ((IEnumerable<string>)yComponents).GetEnumerator();
        {
            while (true)
            {
                bool hasX = xE.MoveNext();
                bool hasY = yE.MoveNext();

                if (!(hasX || hasY)) return 0;

                // we want longer paths before shorter ones for nesting
                if (!hasX) return 1;
                if (!hasY) return -1;

                int itemResult = string.Compare(xE.Current, yE.Current);
                if (itemResult != 0) return itemResult;
            }
        }
    }
}
