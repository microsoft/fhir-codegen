// <copyright file="FileDataAttribute.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System.Reflection;
using Xunit.Sdk;

namespace Microsoft.Health.Fhir.PackageManager.Tests.Extensions;


/// <summary>Attribute for file data.</summary>
public class FileDataAttribute : DataAttribute
{
    private readonly List<string> _filePaths = [];

    /// <summary>Load file contents as the data source for a theory.</summary>
    /// <param name="filePath">The absolute or relative path to the file to load.</param>
    public FileDataAttribute(string filePath)
    {
        _filePaths.Add(filePath);
    }

    /// <summary>Load file contents as the data source for a theory.</summary>
    /// <param name="filePath1">The first file path.</param>
    /// <param name="filePath2">The second file path.</param>
    public FileDataAttribute(string filePath1, string filePath2)
    {
        _filePaths.Add(filePath1);
        _filePaths.Add(filePath2);
    }

    /// <summary>Load file contents as the data source for a theory.</summary>
    /// <param name="filePath1">The first file path.</param>
    /// <param name="filePath2">The second file path.</param>
    /// <param name="filePath3">The third file path.</param>
    public FileDataAttribute(string filePath1, string filePath2, string filePath3)
    {
        _filePaths.Add(filePath1);
        _filePaths.Add(filePath2);
        _filePaths.Add(filePath3);
    }

    /// <summary>Load file contents as the data source for a theory.</summary>
    /// <param name="filePath1">The first file path.</param>
    /// <param name="filePath2">The second file path.</param>
    /// <param name="filePath3">The third file path.</param>
    /// <param name="filePath4">The fourth file path.</param>
    public FileDataAttribute(string filePath1, string filePath2, string filePath3, string filePath4)
    {
        _filePaths.Add(filePath1);
        _filePaths.Add(filePath2);
        _filePaths.Add(filePath3);
        _filePaths.Add(filePath4);
    }

    /// <inheritDoc />
    public override IEnumerable<object[]> GetData(MethodInfo testMethod)
    {
        if (testMethod == null) { throw new ArgumentNullException(nameof(testMethod)); }

        List<object> contents = [];

        foreach (string filePath in _filePaths)
        {
            // Get the absolute path to the file
            string path = Path.IsPathRooted(filePath)
                ? filePath
                : Path.GetRelativePath(Directory.GetCurrentDirectory(), filePath);

            if (!File.Exists(path))
            {
                throw new ArgumentException($"Could not find file at path: {path}");
            }

            string data = File.ReadAllText(path);
            contents.Add(data);
        }

        return new object[][] { [.. contents] };
    }
}
