// <copyright file="NaturalComparer.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>


using System.Text.RegularExpressions;

namespace Microsoft.Health.Fhir.CodeGen.Models;

public class NaturalComparer : IComparer<string>
{
    public static readonly NaturalComparer Instance = new NaturalComparer();

    /// <summary>(Immutable) The tokenizer.</summary>
    private static readonly Regex _tokenizer = new Regex(@"(\D+)|(\d+)", RegexOptions.Compiled);

    private const int _matchGroupAlphabetic = 1;
    private const int _matchGroupNumeric = 2;

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

        IEnumerator<Match> xE = (IEnumerator<Match>)_tokenizer.Matches(x).GetEnumerator();
        IEnumerator<Match> yE = (IEnumerator<Match>)_tokenizer.Matches(y).GetEnumerator();

        while (true)
        {
            bool hasX = xE.MoveNext();
            bool hasY = yE.MoveNext();

            if (!(hasX || hasY)) return 0;

            if (!hasX) return -1;
            if (!hasY) return 1;

            bool isXNumeric = xE.Current.Groups[_matchGroupNumeric].Success;
            bool isYNumeric = yE.Current.Groups[_matchGroupNumeric].Success;

            if (isXNumeric != isYNumeric)
            {
                return isXNumeric ? -1 : 1;
            }

            if (isXNumeric)
            {
                int xNum = int.Parse(xE.Current.Value);
                int yNum = int.Parse(yE.Current.Value);

                int numResult = xNum - yNum;
                if (numResult != 0) return numResult;
            }
            else
            {
                int itemResult = string.Compare(xE.Current.Value, yE.Current.Value, StringComparison.Ordinal);
                if (itemResult != 0) return itemResult;
            }
        }
    }
}
