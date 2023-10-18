// <copyright file="DirectoryContentsDataAttribute.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System.Reflection;
using Xunit.Sdk;

namespace Microsoft.Health.Fhir.PackageManager.Tests.Extensions;

/// <summary>Attribute for directory contents data.</summary>
public class DirectoryContentsDataAttribute : DataAttribute
{
    private readonly string _path;
    private readonly string _searchPattern;
    private readonly SearchOption _searchOption;

    /// <summary>Load directory contents as the data source for a theory.</summary>
    /// <param name="path">The absolute or relative path to the directory to load.</param>
    public DirectoryContentsDataAttribute(string path)
        : this(path, "*.*", SearchOption.TopDirectoryOnly) { }

    public DirectoryContentsDataAttribute(string path, string searchPattern)
        : this(path, searchPattern, SearchOption.TopDirectoryOnly) { }

    /// <summary>Load directory contents as the data source for a theory.</summary>
    /// <param name="path">         The absolute or relative path to the directory to load.</param>
    /// <param name="searchPattern">A pattern specifying the search.</param>
    /// <param name="searchOption"> The search option.</param>
    public DirectoryContentsDataAttribute(string path, string searchPattern, SearchOption searchOption)
    {
        _path = path;
        _searchPattern = searchPattern;
        _searchOption = searchOption;
    }

    /// <inheritDoc />
    public override IEnumerable<object[]> GetData(MethodInfo testMethod)
    {
        if (testMethod == null) { throw new ArgumentNullException(nameof(testMethod)); }

        // Get the absolute path to the directory
        string path = Path.IsPathRooted(_path)
            ? _path
            : Path.GetRelativePath(Directory.GetCurrentDirectory(), _path);

        if (!Directory.Exists(path))
        {
            throw new ArgumentException($"Could not find directory: {path}");
        }

        List<object> data = new();

        foreach (string filename in Directory.EnumerateFiles(path, _searchPattern, _searchOption))
        {
            data.Add(File.ReadAllText(filename));
        }

        return new object[][] { data.ToArray() };
    }
}
