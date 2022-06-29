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
        CompareComplexTypes(FhirArtifactClassEnum.Extension, _a.ExtensionsByUrl, _b.ExtensionsByUrl);
        CompareComplexTypes(FhirArtifactClassEnum.Profile, _a.Profiles, _b.Profiles);

        CompareFhirOperations(_a.OperationsByUrl, _b.OperationsByUrl);

        CompareSearchParameters(_a.SearchParametersByUrl, _b.SearchParametersByUrl);

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

    /// <summary>Compare search parameters.</summary>
    /// <param name="A">The 'A' Search Parameter dictionary to process (typically older version).</param>
    /// <param name="B">The 'B' Search Paraemter dictionary to process (typically newer version).</param>
    private void CompareSearchParameters(
        Dictionary<string, FhirSearchParam> A,
        Dictionary<string, FhirSearchParam> B)
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
                FhirArtifactClassEnum.SearchParameter,
                key,
                key,
                DiffResults.DiffTypeEnum.Removed,
                key,
                string.Empty);
        }

        foreach (string key in keysB)
        {
            _results.AddDiff(
                FhirArtifactClassEnum.SearchParameter,
                key,
                key,
                DiffResults.DiffTypeEnum.Added,
                string.Empty,
                key);
        }

        foreach (string key in keyIntersection)
        {
            CompareSearchParameter(key, A[key], B[key]);
        }
    }

    /// <summary>Compare search parameter.</summary>
    /// <param name="rootKey">The root key.</param>
    /// <param name="A">      The 'A' search parameter to process (typically older version).</param>
    /// <param name="B">      The 'B' search parameter to process (typically newer version).</param>
    private void CompareSearchParameter(
        string rootKey,
        FhirSearchParam A,
        FhirSearchParam B)
    {
        TestForDiff(
            A.Id,
            B.Id,
            FhirArtifactClassEnum.SearchParameter,
            rootKey,
            rootKey,
            DiffResults.DiffTypeEnum.ChangedId);

        if (_options.CompareDescriptions)
        {
            TestForDiff(
                A.Description,
                B.Description,
                FhirArtifactClassEnum.SearchParameter,
                rootKey,
                rootKey,
                DiffResults.DiffTypeEnum.ChangedDescription);

            TestForDiff(
                A.Purpose,
                B.Purpose,
                FhirArtifactClassEnum.SearchParameter,
                rootKey,
                rootKey,
                DiffResults.DiffTypeEnum.ChangedDescription);
        }

        TestForDiff(
            A.Code,
            B.Code,
            FhirArtifactClassEnum.SearchParameter,
            rootKey,
            rootKey,
            DiffResults.DiffTypeEnum.ChangedCode);

        TestForDiff(
            A.ResourceTypes,
            B.ResourceTypes,
            FhirArtifactClassEnum.SearchParameter,
            rootKey,
            rootKey,
            DiffResults.DiffTypeEnum.ExpandedType,
            DiffResults.DiffTypeEnum.ReducedType);

        TestForDiff(
            A.Targets,
            B.Targets,
            FhirArtifactClassEnum.SearchParameter,
            rootKey,
            rootKey,
            DiffResults.DiffTypeEnum.ExpandedTargetProfile,
            DiffResults.DiffTypeEnum.ReducedTargetProfile);

        TestForDiff(
            A.ValueType,
            B.ValueType,
            FhirArtifactClassEnum.SearchParameter,
            rootKey,
            rootKey,
            DiffResults.DiffTypeEnum.ChangedSearchParameterType);

        TestForDiff(
            A.XPath,
            B.XPath,
            FhirArtifactClassEnum.SearchParameter,
            rootKey,
            rootKey,
            DiffResults.DiffTypeEnum.ChangedXPath);

        TestForDiff(
            A.XPathUsage,
            B.XPathUsage,
            FhirArtifactClassEnum.SearchParameter,
            rootKey,
            rootKey,
            DiffResults.DiffTypeEnum.ChangedXPath);

        TestForDiff(
            A.Expression,
            B.Expression,
            FhirArtifactClassEnum.SearchParameter,
            rootKey,
            rootKey,
            DiffResults.DiffTypeEnum.ChangedExpression);
    }

    /// <summary>Compare operations.</summary>
    /// <param name="A">The 'A' Operation Dictionary to process (typically older version).</param>
    /// <param name="B">The 'B' Operation Dictionary to process (typically newer version).</param>
    private void CompareFhirOperations(
        Dictionary<string, FhirOperation> A,
        Dictionary<string, FhirOperation> B)
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
                FhirArtifactClassEnum.Operation,
                key,
                key,
                DiffResults.DiffTypeEnum.Removed,
                key,
                string.Empty);
        }

        foreach (string key in keysB)
        {
            _results.AddDiff(
                FhirArtifactClassEnum.Operation,
                key,
                key,
                DiffResults.DiffTypeEnum.Added,
                string.Empty,
                key);
        }

        foreach (string key in keyIntersection)
        {
            CompareFhirOperation(key, A[key], B[key]);
        }
    }

    /// <summary>Compare FHIR operation.</summary>
    /// <param name="rootKey">The root key.</param>
    /// <param name="A">      The 'A' Operation to process (typically older version).</param>
    /// <param name="B">      The 'B' Operation to process (typically newer version).</param>
    private void CompareFhirOperation(
        string rootKey,
        FhirOperation A,
        FhirOperation B)
    {
        TestForDiff(
            A.Id,
            B.Id,
            FhirArtifactClassEnum.Operation,
            rootKey,
            rootKey,
            DiffResults.DiffTypeEnum.ChangedId);

        if (_options.CompareDescriptions)
        {
            TestForDiff(
                A.Description,
                B.Description,
                FhirArtifactClassEnum.Operation,
                rootKey,
                rootKey,
                DiffResults.DiffTypeEnum.ChangedDescription);

            TestForDiff(
                A.Comment,
                B.Comment,
                FhirArtifactClassEnum.Operation,
                rootKey,
                rootKey,
                DiffResults.DiffTypeEnum.ChangedDescription);
        }

        if ((A.DefinedOnSystem != B.DefinedOnSystem) ||
            (A.DefinedOnType != B.DefinedOnType) ||
            (A.DefinedOnInstance != B.DefinedOnInstance))
        {
            List<string> scopesA = new();
            List<string> scopesB = new();

            if (A.DefinedOnSystem)
            {
                scopesA.Add("System");
            }

            if (A.DefinedOnType)
            {
                scopesA.Add("Type");
            }

            if (A.DefinedOnInstance)
            {
                scopesA.Add("Instance");
            }

            if (B.DefinedOnSystem)
            {
                scopesB.Add("System");
            }

            if (B.DefinedOnType)
            {
                scopesB.Add("Type");
            }

            if (B.DefinedOnInstance)
            {
                scopesB.Add("Instance");
            }

            _results.AddDiff(
                FhirArtifactClassEnum.Operation,
                rootKey,
                rootKey,
                DiffResults.DiffTypeEnum.ChangedScope,
                string.Join(", ", scopesA),
                string.Join(", ", scopesB));
        }

        TestForDiff(
            A.Code,
            B.Code,
            FhirArtifactClassEnum.Operation,
            rootKey,
            rootKey,
            DiffResults.DiffTypeEnum.ChangedCode);

        TestForDiff(
            A.ResourceTypes,
            B.ResourceTypes,
            FhirArtifactClassEnum.Operation,
            rootKey,
            rootKey,
            DiffResults.DiffTypeEnum.ExpandedType,
            DiffResults.DiffTypeEnum.ReducedType);

        CompareParameters(
            FhirArtifactClassEnum.Operation,
            rootKey,
            A.Parameters,
            B.Parameters);
    }

    /// <summary>Compare parameters.</summary>
    /// <param name="artifactClass">The artifact class.</param>
    /// <param name="rootKey">      The root key.</param>
    /// <param name="A">            The 'A' IPackageExportable to process (typically older version).</param>
    /// <param name="B">            The 'B' IPackageExportable to process (typically newer version).</param>
    private void CompareParameters(
        FhirArtifactClassEnum artifactClass,
        string rootKey,
        List<FhirParameter> A,
        List<FhirParameter> B)
    {
        Dictionary<string, FhirParameter> dictA = new();
        Dictionary<string, FhirParameter> dictB = new();

        if (A != null)
        {
            foreach (FhirParameter p in A)
            {
                dictA.Add($"{p.Use}: {p.Name}", p);
            }
        }

        if (B != null)
        {
            foreach (FhirParameter p in B)
            {
                dictB.Add($"{p.Use}: {p.Name}", p);
            }
        }

        HashSet<string> keysA = dictA?.Keys.ToHashSet() ?? new();
        HashSet<string> keysB = dictB?.Keys.ToHashSet() ?? new();

        HashSet<string> keyIntersection = dictA?.Keys.ToHashSet() ?? new();
        keyIntersection.IntersectWith(keysB);

        keysA.ExceptWith(keyIntersection);
        keysB.ExceptWith(keyIntersection);

        foreach (string key in keysA)
        {
            _results.AddDiff(
                artifactClass,
                rootKey,
                key,
                DiffResults.DiffTypeEnum.Removed,
                key,
                string.Empty);
        }

        foreach (string key in keysB)
        {
            _results.AddDiff(
                artifactClass,
                rootKey,
                key,
                DiffResults.DiffTypeEnum.Added,
                string.Empty,
                key);
        }

        foreach (string key in keyIntersection)
        {
            CompareParameter(
                artifactClass,
                rootKey,
                key,
                dictA[key],
                dictB[key]);
        }
    }

    /// <summary>Compare parameter.</summary>
    /// <param name="artifactClass">The artifact class.</param>
    /// <param name="rootKey">      The root key.</param>
    /// <param name="key">          The key.</param>
    /// <param name="A">            The 'A' IPackageExportable to process (typically older version).</param>
    /// <param name="B">            The 'B' IPackageExportable to process (typically newer version).</param>
    private void CompareParameter(
        FhirArtifactClassEnum artifactClass,
        string rootKey,
        string key,
        FhirParameter A,
        FhirParameter B)
    {
        if (_options.CompareDescriptions)
        {
            TestForDiff(
                A.Documentation,
                B.Documentation,
                FhirArtifactClassEnum.Operation,
                rootKey,
                key,
                DiffResults.DiffTypeEnum.ChangedDescription);
        }

        if (A.Min != B.Min)
        {
            if (A.Min == 0)
            {
                _results.AddDiff(
                    artifactClass,
                    rootKey,
                    key,
                    DiffResults.DiffTypeEnum.MadeRequired,
                    A.FhirCardinality,
                    B.FhirCardinality);
            }
            else if (B.Min == 0)
            {
                _results.AddDiff(
                    artifactClass,
                    rootKey,
                    key,
                    DiffResults.DiffTypeEnum.MadeOptional,
                    A.FhirCardinality,
                    B.FhirCardinality);
            }
            else
            {
                _results.AddDiff(
                    artifactClass,
                    rootKey,
                    key,
                    DiffResults.DiffTypeEnum.ChangedMinCardinality,
                    A.FhirCardinality,
                    B.FhirCardinality);
            }
        }

        if (A.Max != B.Max)
        {
            if (A.Max == 1)
            {
                _results.AddDiff(
                    artifactClass,
                    rootKey,
                    key,
                    DiffResults.DiffTypeEnum.MadeArray,
                    A.FhirCardinality,
                    B.FhirCardinality);
            }
            else if (B.Max == 1)
            {
                _results.AddDiff(
                    artifactClass,
                    rootKey,
                    key,
                    DiffResults.DiffTypeEnum.MadeScalar,
                    A.FhirCardinality,
                    B.FhirCardinality);
            }
            else
            {
                _results.AddDiff(
                    artifactClass,
                    rootKey,
                    key,
                    DiffResults.DiffTypeEnum.ChangedMaxCardinality,
                    A.FhirCardinality,
                    B.FhirCardinality);
            }
        }

        TestForDiff(
            A.ValueType,
            B.ValueType,
            FhirArtifactClassEnum.Operation,
            rootKey,
            key,
            DiffResults.DiffTypeEnum.ChangedType);

        TestForDiff(
            A.FieldOrder,
            B.FieldOrder,
            FhirArtifactClassEnum.Operation,
            rootKey,
            key,
            DiffResults.DiffTypeEnum.ChangedOrder);
    }

    /// <summary>Compare complex types.</summary>
    /// <param name="artifactClass">The artifact class.</param>
    /// <param name="A">            The 'A' Complex Type Dictionary (typically older version).</param>
    /// <param name="B">            The 'B' Complex Type Dictionary (typically newer version).</param>
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
            RecursiveAddComplex(
                artifactClass,
                key,
                B[key]);

            //_results.AddDiff(
            //    artifactClass,
            //    key,
            //    key,
            //    DiffResults.DiffTypeEnum.Added,
            //    string.Empty,
            //    key);
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
            A.Id,
            B.Id,
            artifactClass,
            rootKey,
            rootKey,
            DiffResults.DiffTypeEnum.ChangedId);

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

    /// <summary>Recursive add complex.</summary>
    /// <param name="artifactClass">The artifact class.</param>
    /// <param name="rootKey">      The root key.</param>
    /// <param name="complex">      The complex.</param>
    /// <param name="rootComplex">  The root complex.</param>
    private void RecursiveAddComplex(
        FhirArtifactClassEnum artifactClass,
        string rootKey,
        FhirComplex complex,
        FhirComplex rootComplex = null)
    {
        _results.AddDiff(
            artifactClass,
            rootKey,
            complex.Path,
            DiffResults.DiffTypeEnum.Added,
            string.Empty,
            complex.Path);

        foreach (FhirElement element in complex.Elements.Values.OrderBy((e) => e.FieldOrder))
        {
            if (complex.Components.ContainsKey(element.BaseTypeName))
            {
                RecursiveAddComplex(artifactClass, rootKey, complex.Components[element.BaseTypeName], complex);
            }
            else if ((rootComplex != null) && (rootComplex.Components.ContainsKey(element.BaseTypeName)))
            {
                RecursiveAddComplex(artifactClass, rootKey, rootComplex.Components[element.BaseTypeName], complex);
            }
            else
            {
                _results.AddDiff(
                    artifactClass,
                    rootKey,
                    element.Path,
                    DiffResults.DiffTypeEnum.Added,
                    string.Empty,
                    element.Path);
            }
        }
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
                    A.FhirCardinality,
                    B.FhirCardinality);
            }
            else if (B.CardinalityMin == 0)
            {
                _results.AddDiff(
                    artifactClass,
                    rootKey,
                    A.Path,
                    DiffResults.DiffTypeEnum.MadeOptional,
                    A.FhirCardinality,
                    B.FhirCardinality);
            }
            else
            {
                _results.AddDiff(
                    artifactClass,
                    rootKey,
                    A.Path,
                    DiffResults.DiffTypeEnum.ChangedMinCardinality,
                    A.FhirCardinality,
                    B.FhirCardinality);
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
                    A.FhirCardinality,
                    B.FhirCardinality);
            }
            else if (B.CardinalityMin == 1)
            {
                _results.AddDiff(
                    artifactClass,
                    rootKey,
                    A.Path,
                    DiffResults.DiffTypeEnum.MadeScalar,
                    A.FhirCardinality,
                    B.FhirCardinality);
            }
            else
            {
                _results.AddDiff(
                    artifactClass,
                    rootKey,
                    A.Path,
                    DiffResults.DiffTypeEnum.ChangedMaxCardinality,
                    A.FhirCardinality,
                    B.FhirCardinality);
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

    /// <summary>Tests for difference.</summary>
    /// <param name="A">            The 'A' IPackageExportable to process (typically older version).</param>
    /// <param name="B">            The 'B' IPackageExportable to process (typically newer version).</param>
    /// <param name="artifactClass">The artifact class.</param>
    /// <param name="key">          The key.</param>
    /// <param name="path">         Full pathname of the file.</param>
    /// <param name="diffIfAdded">  The difference if added.</param>
    /// <param name="diffIfRemoved">The difference if removed.</param>
    private void TestForDiff(
        List<string> A,
        List<string> B,
        FhirArtifactClassEnum artifactClass,
        string key,
        string path,
        DiffResults.DiffTypeEnum diffIfAdded,
        DiffResults.DiffTypeEnum diffIfRemoved)
    {
        HashSet<string> valuesA = A?.ToHashSet() ?? new();
        HashSet<string> valuesB = B?.ToHashSet() ?? new();

        HashSet<string> valueIntersection = A?.ToHashSet() ?? new();
        valueIntersection.IntersectWith(valuesB);

        valuesA.ExceptWith(valueIntersection);
        valuesB.ExceptWith(valueIntersection);

        foreach (string val in valuesA)
        {
            _results.AddDiff(
                artifactClass,
                key,
                path,
                diffIfRemoved,
                val,
                string.Empty);
        }

        foreach (string val in valuesB)
        {
            _results.AddDiff(
                artifactClass,
                key,
                path,
                diffIfAdded,
                string.Empty,
                val);
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
