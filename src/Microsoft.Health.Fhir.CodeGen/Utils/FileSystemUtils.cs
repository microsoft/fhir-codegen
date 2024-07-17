// <copyright file="FileSystemUtils.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Microsoft.Health.Fhir.CodeGen.Utils;

public abstract class FileSystemUtils
{
    /// <summary>Searches for the FHIR specification directory.</summary>
    /// <exception cref="DirectoryNotFoundException">Thrown when the requested directory is not
    ///  present.</exception>
    /// <param name="startDir">       The start directory.</param>
    /// <param name="dirName">        The name of the directory we are searching for.</param>
    /// <param name="throwIfNotFound">(Optional) True to throw if not found.</param>
    /// <returns>The found FHIR directory.</returns>
    public static string FindRelativeDir(
        string startDir,
        string dirName,
        bool throwIfNotFound = true)
    {
        string currentDir = startDir switch
        {
            null => Path.GetDirectoryName(AppContext.BaseDirectory) ?? string.Empty,
            "" => Path.GetDirectoryName(AppContext.BaseDirectory) ?? string.Empty,
            "." => Path.GetDirectoryName(AppContext.BaseDirectory) ?? string.Empty,
            "./" => Path.GetDirectoryName(AppContext.BaseDirectory) ?? string.Empty,
            "~" => Path.GetDirectoryName(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)) ?? string.Empty,
            "~/" => Path.GetDirectoryName(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)) ?? string.Empty,
            _ => startDir,
        };
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
