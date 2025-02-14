using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Shouldly;

namespace Microsoft.Health.Fhir.CodeGen.Tests.Extensions;

internal static class ShouldlyExtensions
{
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ShouldNotBeNullOrEmpty<TKey, TValue>([NotNull] this Dictionary<TKey, TValue>? actual, string? customMessage = null)
        where TKey : notnull
    {
        if ((actual == null) || (actual.Count == 0))
            throw new ShouldAssertException(new ActualShouldlyMessage(actual, customMessage).ToString());
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ShouldNotBeNullOrEmpty<T>([NotNull] this IEnumerable<T>? actual, string? customMessage = null)
    {
        if ((actual == null) || (!actual.Any()))
            throw new ShouldAssertException(new ActualShouldlyMessage(actual, customMessage).ToString());
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ShouldHaveCount<T>([NotNull] this IEnumerable<T>? actual, int count, string? customMessage = null)
    {
        if (actual == null || actual.Count() != count)
            throw new ShouldAssertException(new ExpectedShouldlyMessage(actual, customMessage).ToString());
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ShouldHaveCount<TKey, TValue>([NotNull] this Dictionary<TKey, TValue>? actual, int count, string? customMessage = null)
        where TKey : notnull
    {
        if (actual == null || actual.Count() != count)
            throw new ShouldAssertException(new ExpectedShouldlyMessage(actual, customMessage).ToString());
    }
}
