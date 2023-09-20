// <copyright file="WithObsVal.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System.Diagnostics.CodeAnalysis;
using Microsoft.Health.Fhir.CodeGenCommon.Extensions;

namespace Microsoft.Health.Fhir.CodeGenCommon.Expectations;

/// <summary>A value with obligations.</summary>
public record class ConformanceVal<T> : ConformanceAnnotatedBase, ICloneable, IComparable<ConformanceVal<T>>, IComparable<T>, IEqualityComparer<ConformanceVal<T>>
    where T : IComparable, IConvertible
{
    /// <summary>Initializes a new instance of the <see cref="ConformanceVal{T}"/>class.</summary>
    public ConformanceVal() : base() { }

    /// <summary>Initializes a new instance of the <see cref="ConformanceVal{T}"/>class.</summary>
    /// <param name="other">The other.</param>
    [SetsRequiredMembers]
    protected ConformanceVal(ConformanceVal<T> other)
        : base(other)
    {
        Value = other.Value;
    }

    /// <summary>Gets or initializes the value.</summary>
    public required T Value { get; init; }

    /// <summary>Convert this object into a string representation.</summary>
    /// <returns>A string that represents this object.</returns>
    public override string ToString() => $"{Value}";

    /// <summary>Makes a deep copy of this object.</summary>
    /// <returns>A copy of this object.</returns>
    object ICloneable.Clone() => this with { };

    /// <summary>
    /// Compares this WithObsVal&lt;T&gt;? object to another to determine their relative ordering.
    /// </summary>
    /// <typeparam name="T>">Type of the t></typeparam>
    /// <param name="other">Another instance to compare.</param>
    /// <returns>
    /// Negative if this object is less than the other, 0 if they are equal, or positive if this is
    /// greater.
    /// </returns>
    int IComparable<ConformanceVal<T>>.CompareTo(ConformanceVal<T>? other) => other == null ? -1 : Value.CompareTo(other.Value);

    /// <summary>
    /// Compares this T? object to another to determine their relative ordering.
    /// </summary>
    /// <typeparam name="T">Generic type parameter.</typeparam>
    /// <param name="other">Another instance to compare.</param>
    /// <returns>
    /// Negative if this object is less than the other, 0 if they are equal, or positive if this is
    /// greater.
    /// </returns>
    int IComparable<T>.CompareTo(T? other) => other == null ? -1 : Value.CompareTo(other);

    /// <summary>Tests if two WithObsVal&lt;T&gt;? objects are considered equal.</summary>
    /// <typeparam name="T>">Type of the t></typeparam>
    /// <param name="x">With obs val&lt; t&gt;? to be compared.</param>
    /// <param name="y">With obs val&lt; t&gt;? to be compared.</param>
    /// <returns>True if the objects are considered equal, false if they are not.</returns>
    bool IEqualityComparer<ConformanceVal<T>>.Equals(ConformanceVal<T>? x, ConformanceVal<T>? y) => x == null || y == null ? false : x.Value.Equals(y.Value);

    /// <summary>Returns a hash code for this object.</summary>
    /// <typeparam name="T>">Type of the t></typeparam>
    /// <param name="obj">The object.</param>
    /// <returns>A hash code for this object.</returns>
    int IEqualityComparer<ConformanceVal<T>>.GetHashCode(ConformanceVal<T> obj) => obj?.Value.GetHashCode() ?? obj?.ObligationsByActor.GetHashCode() ?? 0;
}
