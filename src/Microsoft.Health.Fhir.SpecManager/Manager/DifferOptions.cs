// <copyright file="DifferOptions.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

namespace Microsoft.Health.Fhir.SpecManager.Manager;

/// <summary>A differ options.</summary>
public class DifferOptions
{
    /// <summary>Initializes a new instance of the <see cref="DifferOptions"/> class.</summary>
    /// <param name="compareDescriptions">      A value indicating whether the text changes should be
    ///  included.</param>
    /// <param name="compareRegEx">             A value indicating whether the RegEx changes should
    ///  be included.</param>
    /// <param name="compareValueSetExpansions">A value indicating whether the compare value set
    ///  expansions.</param>
    /// <param name="compareSummaryFlags">      A value indicating whether the compare summary flags.</param>
    /// <param name="compareMustSupportFlags">  A value indicating whether the compare must support
    ///  flags.</param>
    /// <param name="compareBindings">          A value indicating whether the compare bindings.</param>
    public DifferOptions(
        bool compareDescriptions,
        bool compareRegEx,
        bool compareValueSetExpansions,
        bool compareSummaryFlags,
        bool compareMustSupportFlags,
        bool compareBindings)
    {
        CompareDescriptions = compareDescriptions;
        CompareRegEx = compareRegEx;
        CompareValueSetExpansions = compareValueSetExpansions;
        CompareSummaryFlags = compareSummaryFlags;
        CompareMustSupportFlags = compareMustSupportFlags;
        CompareBindings = compareBindings;
    }

    /// <summary>Gets a value indicating whether the text changes should be included.</summary>
    public bool CompareDescriptions { get; }

    /// <summary>Gets a value indicating whether the RegEx changes should be included.</summary>
    public bool CompareRegEx { get; }

    /// <summary>Gets a value indicating whether the compare value set expansions.</summary>
    public bool CompareValueSetExpansions { get; }

    /// <summary>Gets a value indicating whether the compare summary flags.</summary>
    public bool CompareSummaryFlags { get; }

    /// <summary>Gets a value indicating whether the compare must support flags.</summary>
    public bool CompareMustSupportFlags { get; }

    /// <summary>Gets a value indicating whether the compare bindings.</summary>
    public bool CompareBindings { get; }
}
