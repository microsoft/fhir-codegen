// <copyright file="Differ.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

namespace Microsoft.Health.Fhir.SpecManager.Manager;

/// <summary>Class that performs a diff between two FhirVersionInfo packages.</summary>
public class Differ
{
    private IPackageExportable _a;
    private IPackageExportable _b;
    private DifferOptions _options;
    private DiffResults _results;

    /// <summary>
    /// Initializes a new instance of the <see cref="Differ"/> class.
    /// </summary>
    /// <param name="A">      The 'A' IPackageExportable to process (typically older version).</param>
    /// <param name="B">      The 'B' IPackageExportable to process (typically newer version).</param>
    /// <param name="options">Options for controlling the operation.</param>
    public Differ(
        IPackageExportable A,
        IPackageExportable B,
        DifferOptions options)
    {
        _a = A;
        _b = B;
        _options = options;
        _results = new();
    }

    /// <summary>Generates a difference.</summary>
    /// <param name="A">      The 'A' IPackageExportable to process (typically older version).</param>
    /// <param name="B">      The 'B' IPackageExportable to process (typically newer version).</param>
    /// <param name="_options">Options for controlling the operation.</param>
    /// <returns>The difference.</returns>
    public DiffResults GenerateDiff()
    {
        Console.WriteLine($"Starting diff between {_a.PackageName}#{_a.VersionString} and {_b.PackageName}#{_b.VersionString}");

        ComparePrimitiveTypes(_a.PrimitiveTypes, _b.PrimitiveTypes);
        CompareComplexTypes(FhirArtifactClassEnum.ComplexType, _a.ComplexTypes, _b.ComplexTypes);
        CompareComplexTypes(FhirArtifactClassEnum.Resource, _a.Resources, _b.Resources);

        //if (_options.CompareValueSetExpansions)
        //{

        //}

        Console.WriteLine($"Diff completed!");

        return _results;
    }

    /// <summary>Compare primitive types.</summary>
    /// <param name="A">The 'A' Primitive Type Dictionary (typically older version).</param>
    /// <param name="B">The 'B' Primitive Type Dictionary (typically newer version).</param>
    private void ComparePrimitiveTypes(
        Dictionary<string, FhirPrimitive> A,
        Dictionary<string, FhirPrimitive> B)
    {
        HashSet<string> keysA = A?.Keys.ToHashSet() ?? new();
        HashSet<string> keysB = B?.Keys.ToHashSet() ?? new();

        HashSet<string> keyIntersection = A?.Keys.ToHashSet() ?? new();
        keyIntersection.IntersectWith(keysB);

        keysA.ExceptWith(keyIntersection);
        keysB.ExceptWith(keyIntersection);

        foreach (string key in keysA)
        {
            _results.AddDiff(
                FhirArtifactClassEnum.PrimitiveType,
                key,
                key,
                DiffResults.DiffTypeEnum.Removed,
                key,
                string.Empty);
        }

        foreach (string key in keysB)
        {
            _results.AddDiff(
                FhirArtifactClassEnum.PrimitiveType,
                key,
                key,
                DiffResults.DiffTypeEnum.Added,
                string.Empty,
                key);
        }

        foreach (string key in keyIntersection)
        {
            TestPrimitiveDiff(
                A[key].BaseTypeName,
                B[key].BaseTypeName,
                key,
                DiffResults.DiffTypeEnum.ChangedType);

            if (_options.CompareRegEx)
            {
                TestPrimitiveDiff(
                    A[key].ValidationRegEx,
                    B[key].ValidationRegEx,
                    key,
                    DiffResults.DiffTypeEnum.ChangedRegEx);
            }

            if (_options.CompareDescriptions)
            {
                TestPrimitiveDiff(
                    A[key].ShortDescription,
                    B[key].ShortDescription,
                    key,
                    DiffResults.DiffTypeEnum.ChangedDescription);

                TestPrimitiveDiff(
                    A[key].Purpose,
                    B[key].Purpose,
                    key,
                    DiffResults.DiffTypeEnum.ChangedDescription);

                TestPrimitiveDiff(
                    A[key].Comment,
                    B[key].Comment,
                    key,
                    DiffResults.DiffTypeEnum.ChangedDescription);
            }
        }
    }

    /// <summary>Compare complex types.</summary>
    /// <param name="artifactClass">The artifact class.</param>
    /// <param name="A">            The 'A' Primitive Type Dictionary (typically older version).</param>
    /// <param name="B">            The 'B' Primitive Type Dictionary (typically newer version).</param>
    private void CompareComplexTypes(
        FhirArtifactClassEnum artifactClass,
        Dictionary<string, FhirComplex> A,
        Dictionary<string, FhirComplex> B)
    {
        HashSet<string> keysA = A?.Keys.ToHashSet() ?? new();
        HashSet<string> keysB = B?.Keys.ToHashSet() ?? new();

        HashSet<string> keyIntersection = A?.Keys.ToHashSet() ?? new();
        keyIntersection.IntersectWith(keysB);

        keysA.ExceptWith(keyIntersection);
        keysB.ExceptWith(keyIntersection);

        foreach (string key in keysA)
        {
            _results.AddDiff(
                artifactClass,
                key,
                key,
                DiffResults.DiffTypeEnum.Removed,
                key,
                string.Empty);
        }

        foreach (string key in keysB)
        {
            _results.AddDiff(
                artifactClass,
                key,
                key,
                DiffResults.DiffTypeEnum.Added,
                string.Empty,
                key);
        }

        foreach (string key in keyIntersection)
        {
            CompareFhirComplex(
                artifactClass,
                key,
                A[key],
                B[key]);
        }
    }

    /// <summary>Compare FHIR complex.</summary>
    /// <param name="artifactClass">The artifact class.</param>
    /// <param name="rootKey">      The root key.</param>
    /// <param name="A">            The 'A' IPackageExportable to process (typically older version).</param>
    /// <param name="B">            The 'B' IPackageExportable to process (typically newer version).</param>
    private void CompareFhirComplex(
        FhirArtifactClassEnum artifactClass,
        string rootKey,
        FhirComplex A,
        FhirComplex B)
    {
        TestForDiff(
            A.BaseTypeName,
            B.BaseTypeName,
            artifactClass,
            rootKey,
            rootKey,
            DiffResults.DiffTypeEnum.ChangedType);

        TestForDiff(
            A.ExplicitName,
            B.ExplicitName,
            artifactClass,
            rootKey,
            A.Path,
            DiffResults.DiffTypeEnum.ChangedExplicitName);

        if (_options.CompareRegEx)
        {
            TestForDiff(
                A.ValidationRegEx,
                B.ValidationRegEx,
                artifactClass,
                rootKey,
                A.Path,
                DiffResults.DiffTypeEnum.ChangedRegEx);
        }

        if (_options.CompareDescriptions)
        {
            TestForDiff(
                A.ShortDescription,
                B.ShortDescription,
                artifactClass,
                rootKey,
                A.Path,
                DiffResults.DiffTypeEnum.ChangedDescription);

            TestForDiff(
                A.Purpose,
                B.Purpose,
                artifactClass,
                rootKey,
                A.Path,
                DiffResults.DiffTypeEnum.ChangedDescription);

            TestForDiff(
                A.Comment,
                B.Comment,
                artifactClass,
                rootKey,
                A.Path,
                DiffResults.DiffTypeEnum.ChangedDescription);
        }

        CompareComplexComponents(artifactClass, rootKey, A.Components, B.Components);
        CompareComplexElements(artifactClass, rootKey, A.Elements, B.Elements);
    }

    /// <summary>Compare complex components.</summary>
    /// <param name="artifactClass">The artifact class.</param>
    /// <param name="rootKey">      The root key.</param>
    /// <param name="A">            The 'A' Primitive Type Dictionary (typically older version).</param>
    /// <param name="B">            The 'B' Primitive Type Dictionary (typically newer version).</param>
    private void CompareComplexComponents(
        FhirArtifactClassEnum artifactClass,
        string rootKey,
        Dictionary<string, FhirComplex> A,
        Dictionary<string, FhirComplex> B)
    {
        HashSet<string> keysA = A?.Keys.ToHashSet() ?? new();
        HashSet<string> keysB = B?.Keys.ToHashSet() ?? new();

        HashSet<string> keyIntersection = A?.Keys.ToHashSet() ?? new();
        keyIntersection.IntersectWith(keysB);

        keysA.ExceptWith(keyIntersection);
        keysB.ExceptWith(keyIntersection);

        foreach (string key in keysA)
        {
            _results.AddDiff(
                artifactClass,
                rootKey,
                A[key].Path,
                DiffResults.DiffTypeEnum.Removed,
                key,
                string.Empty);
        }

        foreach (string key in keysB)
        {
            _results.AddDiff(
                artifactClass,
                rootKey,
                B[key].Path,
                DiffResults.DiffTypeEnum.Added,
                string.Empty,
                key);
        }

        foreach (string key in keyIntersection)
        {
            CompareFhirComplex(
                artifactClass,
                rootKey,
                A[key],
                B[key]);
        }
    }

    /// <summary>Compare complex elements.</summary>
    /// <param name="artifactClass">The artifact class.</param>
    /// <param name="rootKey">      The root key.</param>
    /// <param name="A">            The 'A' IPackageExportable to process (typically older version).</param>
    /// <param name="B">            The 'B' IPackageExportable to process (typically newer version).</param>
    private void CompareComplexElements(
        FhirArtifactClassEnum artifactClass,
        string rootKey,
        Dictionary<string, FhirElement> A,
        Dictionary<string, FhirElement> B)
    {
        HashSet<string> keysA = A?.Keys.ToHashSet() ?? new();
        HashSet<string> keysB = B?.Keys.ToHashSet() ?? new();

        HashSet<string> keyIntersection = A?.Keys.ToHashSet() ?? new();
        keyIntersection.IntersectWith(keysB);

        keysA.ExceptWith(keyIntersection);
        keysB.ExceptWith(keyIntersection);

        foreach (string key in keysA)
        {
            _results.AddDiff(
                artifactClass,
                rootKey,
                A[key].Path,
                DiffResults.DiffTypeEnum.Removed,
                key,
                string.Empty);
        }

        foreach (string key in keysB)
        {
            _results.AddDiff(
                artifactClass,
                rootKey,
                B[key].Path,
                DiffResults.DiffTypeEnum.Added,
                string.Empty,
                key);
        }

        foreach (string key in keyIntersection)
        {
            CompareComplexElement(artifactClass, rootKey, A[key], B[key]);
        }
    }

    /// <summary>Compare complex element.</summary>
    /// <param name="artifactClass">The artifact class.</param>
    /// <param name="rootKey">      The root key.</param>
    /// <param name="A">            The 'A' IPackageExportable to process (typically older version).</param>
    /// <param name="B">            The 'B' IPackageExportable to process (typically newer version).</param>
    private void CompareComplexElement(
        FhirArtifactClassEnum artifactClass,
        string rootKey,
        FhirElement A,
        FhirElement B)
    {
        TestForDiff(
            A.ExplicitName,
            B.ExplicitName,
            artifactClass,
            rootKey,
            A.Path,
            DiffResults.DiffTypeEnum.ChangedExplicitName);

        TestForDiff(
            A.BaseTypeName,
            B.BaseTypeName,
            artifactClass,
            rootKey,
            A.Path,
            DiffResults.DiffTypeEnum.ChangedType);

        CompareElementTypes(
            artifactClass,
            rootKey,
            A.Path,
            A.ElementTypes,
            B.ElementTypes);

        TestForDiff(
            A.FieldOrder,
            B.FieldOrder,
            artifactClass,
            rootKey,
            A.Path,
            DiffResults.DiffTypeEnum.ChangedOrder);

        if (A.CardinalityMin != B.CardinalityMin)
        {
            if (A.CardinalityMin == 0)
            {
                _results.AddDiff(
                    artifactClass,
                    rootKey,
                    A.Path,
                    DiffResults.DiffTypeEnum.MadeRequired,
                    A.CardinalityMin.ToString(),
                    B.CardinalityMin.ToString());
            }
            else if (B.CardinalityMin == 0)
            {
                _results.AddDiff(
                    artifactClass,
                    rootKey,
                    A.Path,
                    DiffResults.DiffTypeEnum.MadeOptional,
                    A.CardinalityMin.ToString(),
                    B.CardinalityMin.ToString());
            }
            else
            {
                _results.AddDiff(
                    artifactClass,
                    rootKey,
                    A.Path,
                    DiffResults.DiffTypeEnum.ChangedMinCardinality,
                    A.CardinalityMin.ToString(),
                    B.CardinalityMin.ToString());
            }
        }

        if (A.CardinalityMax != B.CardinalityMax)
        {
            if (A.CardinalityMin == 1)
            {
                _results.AddDiff(
                    artifactClass,
                    rootKey,
                    A.Path,
                    DiffResults.DiffTypeEnum.MadeArray,
                    A.CardinalityMin.ToString(),
                    B.CardinalityMin.ToString());
            }
            else if (B.CardinalityMin == 1)
            {
                _results.AddDiff(
                    artifactClass,
                    rootKey,
                    A.Path,
                    DiffResults.DiffTypeEnum.MadeScalar,
                    A.CardinalityMin.ToString(),
                    B.CardinalityMin.ToString());
            }
            else
            {
                _results.AddDiff(
                    artifactClass,
                    rootKey,
                    A.Path,
                    DiffResults.DiffTypeEnum.ChangedMaxCardinality,
                    A.CardinalityMin.ToString(),
                    B.CardinalityMin.ToString());
            }
        }

        if (A.IsModifier != B.IsModifier)
        {
            _results.AddDiff(
                artifactClass,
                rootKey,
                A.Path,
                DiffResults.DiffTypeEnum.ChangedModifierFlag,
                A.IsModifier.ToString(),
                B.IsModifier.ToString());
        }

        if (_options.CompareSummaryFlags && (A.IsSummary != B.IsSummary))
        {
            _results.AddDiff(
                artifactClass,
                rootKey,
                A.Path,
                DiffResults.DiffTypeEnum.ChangedSummaryFlag,
                A.IsModifier.ToString(),
                B.IsModifier.ToString());
        }

        if (_options.CompareMustSupportFlags && (A.IsMustSupport != B.IsMustSupport))
        {
            _results.AddDiff(
                artifactClass,
                rootKey,
                A.Path,
                DiffResults.DiffTypeEnum.ChangedMustSupportFlag,
                A.IsMustSupport.ToString(),
                B.IsMustSupport.ToString());
        }

        if (_options.CompareBindings)
        {
            TestForDiff(
                A.BindingStrength,
                B.BindingStrength,
                artifactClass,
                rootKey,
                A.Path,
                DiffResults.DiffTypeEnum.ChangedBindingStrength);

            TestForDiffIgnoreVersion(
                A.ValueSet,
                B.ValueSet,
                artifactClass,
                rootKey,
                A.Path,
                DiffResults.DiffTypeEnum.ChangedBindingTarget);
        }

        if (_options.CompareDescriptions)
        {
            TestForDiff(
                A.ShortDescription,
                B.ShortDescription,
                artifactClass,
                rootKey,
                A.Path,
                DiffResults.DiffTypeEnum.ChangedDescription);

            TestForDiff(
                A.Purpose,
                B.Purpose,
                artifactClass,
                rootKey,
                A.Path,
                DiffResults.DiffTypeEnum.ChangedDescription);

            TestForDiff(
                A.Comment,
                B.Comment,
                artifactClass,
                rootKey,
                A.Path,
                DiffResults.DiffTypeEnum.ChangedDescription);
        }
    }

    /// <summary>Compare element types.</summary>
    /// <param name="artifactClass">The artifact class.</param>
    /// <param name="rootKey">      The root key.</param>
    /// <param name="path">         Full pathname of the file.</param>
    /// <param name="A">            The 'A' IPackageExportable to process (typically older version).</param>
    /// <param name="B">            The 'B' IPackageExportable to process (typically newer version).</param>
    private void CompareElementTypes(
        FhirArtifactClassEnum artifactClass,
        string rootKey,
        string path,
        Dictionary<string, FhirElementType> A,
        Dictionary<string, FhirElementType> B)
    {
        HashSet<string> keysA = A?.Keys.ToHashSet() ?? new();
        HashSet<string> keysB = B?.Keys.ToHashSet() ?? new();

        HashSet<string> keyIntersection = A?.Keys.ToHashSet() ?? new();
        keyIntersection.IntersectWith(keysB);

        keysA.ExceptWith(keyIntersection);
        keysB.ExceptWith(keyIntersection);

        foreach (string key in keysA)
        {
            _results.AddDiff(
                artifactClass,
                rootKey,
                path,
                DiffResults.DiffTypeEnum.ReducedType,
                key,
                string.Empty);
        }

        foreach (string key in keysB)
        {
            _results.AddDiff(
                artifactClass,
                rootKey,
                path,
                DiffResults.DiffTypeEnum.ExpandedType,
                string.Empty,
                key);
        }

        foreach (string key in keyIntersection)
        {
            CompareElementTargetProfiles(
                artifactClass,
                rootKey,
                path,
                A[key].Profiles,
                B[key].Profiles);

            CompareElementTypeProfiles(
                artifactClass,
                rootKey,
                path,
                A[key].TypeProfiles,
                B[key].TypeProfiles);
        }
    }

    /// <summary>Compare element target profiles.</summary>
    /// <param name="artifactClass">The artifact class.</param>
    /// <param name="rootKey">      The root key.</param>
    /// <param name="path">         Full pathname of the file.</param>
    /// <param name="A">            The 'A' IPackageExportable to process (typically older version).</param>
    /// <param name="B">            The 'B' IPackageExportable to process (typically newer version).</param>
    private void CompareElementTargetProfiles(
        FhirArtifactClassEnum artifactClass,
        string rootKey,
        string path,
        Dictionary<string, FhirElementProfile> A,
        Dictionary<string, FhirElementProfile> B)
    {
        HashSet<string> keysA = A?.Keys.ToHashSet() ?? new();
        HashSet<string> keysB = B?.Keys.ToHashSet() ?? new();

        HashSet<string> keyIntersection = A?.Keys.ToHashSet() ?? new();
        keyIntersection.IntersectWith(keysB);

        keysA.ExceptWith(keyIntersection);
        keysB.ExceptWith(keyIntersection);

        foreach (string key in keysA)
        {
            _results.AddDiff(
                artifactClass,
                rootKey,
                path,
                DiffResults.DiffTypeEnum.ReducedTargetProfile,
                key,
                string.Empty);
        }

        foreach (string key in keysB)
        {
            _results.AddDiff(
                artifactClass,
                rootKey,
                path,
                DiffResults.DiffTypeEnum.ExpandedTargetProfile,
                string.Empty,
                key);
        }

        foreach (string key in keyIntersection)
        {
            TestForDiff(
                A[key].Name,
                B[key].Name,
                artifactClass,
                rootKey,
                path,
                DiffResults.DiffTypeEnum.ChangedTargetProfile);

            TestForDiff(
                A[key].URL.ToString(),
                B[key].URL.ToString(),
                artifactClass,
                rootKey,
                path,
                DiffResults.DiffTypeEnum.ChangedTargetProfile);
        }
    }

    /// <summary>Compare element type profiles.</summary>
    /// <param name="artifactClass">The artifact class.</param>
    /// <param name="rootKey">      The root key.</param>
    /// <param name="path">         Full pathname of the file.</param>
    /// <param name="A">            The 'A' IPackageExportable to process (typically older version).</param>
    /// <param name="B">            The 'B' IPackageExportable to process (typically newer version).</param>
    private void CompareElementTypeProfiles(
        FhirArtifactClassEnum artifactClass,
        string rootKey,
        string path,
        Dictionary<string, FhirElementProfile> A,
        Dictionary<string, FhirElementProfile> B)
    {
        HashSet<string> keysA = A?.Keys.ToHashSet() ?? new();
        HashSet<string> keysB = B?.Keys.ToHashSet() ?? new();

        HashSet<string> keyIntersection = A?.Keys.ToHashSet() ?? new();
        keyIntersection.IntersectWith(keysB);

        keysA.ExceptWith(keyIntersection);
        keysB.ExceptWith(keyIntersection);

        foreach (string key in keysA)
        {
            _results.AddDiff(
                artifactClass,
                rootKey,
                path,
                DiffResults.DiffTypeEnum.ReducedTypeProfile,
                key,
                string.Empty);
        }

        foreach (string key in keysB)
        {
            _results.AddDiff(
                artifactClass,
                rootKey,
                path,
                DiffResults.DiffTypeEnum.ExpandedTypeProfile,
                string.Empty,
                key);
        }

        foreach (string key in keyIntersection)
        {
            TestForDiff(
                A[key].Name,
                B[key].Name,
                artifactClass,
                rootKey,
                path,
                DiffResults.DiffTypeEnum.ChangedTypeProfile);

            TestForDiff(
                A[key].URL.ToString(),
                B[key].URL.ToString(),
                artifactClass,
                rootKey,
                path,
                DiffResults.DiffTypeEnum.ChangedTypeProfile);
        }
    }

    /// <summary>Tests for difference.</summary>
    /// <param name="valueA">       The value a.</param>
    /// <param name="valueB">       The value b.</param>
    /// <param name="artifactClass">The artifact class.</param>
    /// <param name="key">          The key.</param>
    /// <param name="path">         Full pathname of the file.</param>
    /// <param name="diffType">     Type of the difference.</param>
    private void TestForDiff(
        string valueA,
        string valueB,
        FhirArtifactClassEnum artifactClass,
        string key,
        string path,
        DiffResults.DiffTypeEnum diffType)
    {
        if (string.IsNullOrEmpty(valueA) && string.IsNullOrEmpty(valueB))
        {
            return;
        }

        if (string.IsNullOrEmpty(valueA) ||
            string.IsNullOrEmpty(valueB) ||
            (!valueA.Equals(valueB, StringComparison.Ordinal)))
        {
            _results.AddDiff(
                artifactClass,
                key,
                path,
                diffType,
                valueA,
                valueB);
        }
    }

    /// <summary>Tests for difference ignore version.</summary>
    /// <param name="valueA">       The value a.</param>
    /// <param name="valueB">       The value b.</param>
    /// <param name="artifactClass">The artifact class.</param>
    /// <param name="key">          The key.</param>
    /// <param name="path">         Full pathname of the file.</param>
    /// <param name="diffType">     Type of the difference.</param>
    private void TestForDiffIgnoreVersion(
        string valueA,
        string valueB,
        FhirArtifactClassEnum artifactClass,
        string key,
        string path,
        DiffResults.DiffTypeEnum diffType)
    {
        if (string.IsNullOrEmpty(valueA) && string.IsNullOrEmpty(valueB))
        {
            return;
        }

        if (string.IsNullOrEmpty(valueA) ||
            string.IsNullOrEmpty(valueB))
        {
            _results.AddDiff(
                artifactClass,
                key,
                path,
                diffType,
                valueA,
                valueB);
            return;
        }

        if (!valueA.Split('|')[0].Equals(valueB.Split('|')[0], StringComparison.Ordinal))
        {
            _results.AddDiff(
                artifactClass,
                key,
                path,
                diffType,
                valueA,
                valueB);
        }
    }

    /// <summary>Tests for difference.</summary>
    /// <param name="valueA">       The value a.</param>
    /// <param name="valueB">       The value b.</param>
    /// <param name="artifactClass">The artifact class.</param>
    /// <param name="key">          The key.</param>
    /// <param name="path">         Full pathname of the file.</param>
    /// <param name="diffType">     Type of the difference.</param>
    private void TestForDiff(
        int valueA,
        int valueB,
        FhirArtifactClassEnum artifactClass,
        string key,
        string path,
        DiffResults.DiffTypeEnum diffType)
    {
        if (valueA != valueB)
        {
            _results.AddDiff(
                artifactClass,
                key,
                path,
                diffType,
                valueA.ToString(),
                valueB.ToString());
        }
    }

    /// <summary>Tests for difference in a FHIR Primitive.</summary>
    /// <param name="valueA">  The value a.</param>
    /// <param name="valueB">  The value b.</param>
    /// <param name="key">     The key.</param>
    /// <param name="diffType">Type of the difference.</param>
    private void TestPrimitiveDiff(
        string valueA,
        string valueB,
        string key,
        DiffResults.DiffTypeEnum diffType)
    {
        if (string.IsNullOrEmpty(valueA) && string.IsNullOrEmpty(valueB))
        {
            return;
        }

        if (string.IsNullOrEmpty(valueA) ||
            string.IsNullOrEmpty(valueB) ||
            (!valueA.Equals(valueB, StringComparison.Ordinal)))
        {
            _results.AddDiff(
                FhirArtifactClassEnum.PrimitiveType,
                key,
                key,
                diffType,
                valueA,
                valueB);
        }
    }
}
