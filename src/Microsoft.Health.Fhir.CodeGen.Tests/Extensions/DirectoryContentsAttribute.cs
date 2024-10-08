﻿// <copyright file="DirectoryContentsAttribute.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>


using System.Reflection;
using Xunit.Sdk;

namespace Microsoft.Health.Fhir.CodeGen.Tests.Extensions;

/// <summary>Attribute for directory contents.</summary>
public class DirectoryContentsAttribute : DataAttribute
{
    private readonly string _path;
    private readonly string _searchPattern;
    private readonly SearchOption _searchOption;

    /// <summary>Load directory contents as the data source for a theory.</summary>
    /// <param name="path">The absolute or relative path to the directory to load.</param>
    public DirectoryContentsAttribute(string path)
        : this(path, "*.*", SearchOption.TopDirectoryOnly) { }

    public DirectoryContentsAttribute(string path, string searchPattern)
        : this(path, searchPattern, SearchOption.TopDirectoryOnly) { }

    /// <summary>Load directory contents as the data source for a theory.</summary>
    /// <param name="path">         The absolute or relative path to the directory to load.</param>
    /// <param name="searchPattern">A pattern specifying the search.</param>
    /// <param name="searchOption"> The search option.</param>
    public DirectoryContentsAttribute(string path, string searchPattern, SearchOption searchOption)
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
            : FindRelativeDir(string.Empty, _path);

        if (!Directory.Exists(path))
        {
            throw new ArgumentException($"Could not find directory: {path}");
        }

        List<object> data = [];

        foreach (string filename in Directory.EnumerateFiles(path, _searchPattern, _searchOption))
        {
            data.Add(File.ReadAllText(filename));
        }

        return new object[][] { [.. data] };
    }

    internal static string FindRelativeDir(
        string startDir,
        string dirName,
        bool throwIfNotFound = true)
    {
        string currentDir = string.IsNullOrEmpty(startDir) ? Path.GetDirectoryName(AppContext.BaseDirectory) ?? string.Empty : startDir;
        string testDir = Path.Combine(currentDir, dirName);

        while (!Directory.Exists(testDir))
        {
            currentDir = Path.GetFullPath(Path.Combine(currentDir, ".."));

            if (currentDir == Path.GetPathRoot(currentDir))
            {
                if (throwIfNotFound)
                {
                    throw new DirectoryNotFoundException($"Could not find directory {dirName}!");
                }

                return string.Empty;
            }

            testDir = Path.Combine(currentDir, dirName);
        }

        return testDir;
    }
}

