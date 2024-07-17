// <copyright file="DefinitionCollectionSlicing.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>


using System.Diagnostics.CodeAnalysis;
using Hl7.Fhir.Model;

namespace Microsoft.Health.Fhir.CodeGen.Models;

/// <content>Collection of definitions.</content>
public partial class DefinitionCollection
{
    private readonly Dictionary<string, KeyValuePair<string, StructureDefinition>[]> _pathsWithSlices = [];

    /// <summary>Query if 'path' has slicing.</summary>
    /// <param name="path">Full pathname of the file.</param>
    /// <returns>True if slicing, false if not.</returns>
    public bool HasSlicing(string path, StructureDefinition? definedBy = null) =>
        (definedBy == null)
        ? _pathsWithSlices.ContainsKey(path)
        : _pathsWithSlices.ContainsKey(path) && _pathsWithSlices[path].Any(kvp => kvp.Value.Id == definedBy.Id);

    /// <summary>Attempts to get slicing a string[] from the given string.</summary>
    /// <param name="path">  Full pathname of the file.</param>
    /// <param name="slices">[out] The slices.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    public bool TryGetSliceNames(string path, [NotNullWhen(true)] out string[]? slices, StructureDefinition? definedBy = null)
    {
        bool success;
        KeyValuePair<string, StructureDefinition>[]? sliceDefs;

        if (definedBy == null)
        {
            success = _pathsWithSlices.TryGetValue(path, out sliceDefs);

            if ((!success) || (sliceDefs == null))
            {
                slices = null;
                return false;
            }

            slices = sliceDefs.Select(kvp => kvp.Key).ToArray();
            return true;
        }

        success = _pathsWithSlices.TryGetValue(path, out sliceDefs);

        if ((!success) || (sliceDefs == null))
        {
            slices = null;
            return false;
        }

        slices = sliceDefs.Where(kvp => kvp.Value.Id == definedBy.Id).Select(kvp => kvp.Key).ToArray();
        return true;
    }

}
