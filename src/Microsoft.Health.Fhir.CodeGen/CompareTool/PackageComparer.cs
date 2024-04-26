// <copyright file="PackageComparer.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hl7.Fhir.Model;
using Microsoft.Health.Fhir.CodeGen.Configuration;
using Microsoft.Health.Fhir.CodeGen.Language;
using Microsoft.Health.Fhir.CodeGen.Models;
using Microsoft.Health.Fhir.CodeGenCommon.Extensions;

namespace Microsoft.Health.Fhir.CodeGen.CompareTool;

public class PackageComparer
{
    DefinitionCollection _left;
    DefinitionCollection _right;

    ConfigCompare _config;

    public PackageComparer(ConfigCompare config, DefinitionCollection left, DefinitionCollection right)
    {
        _config = config;
        _left = left;
        _right = right;
    }

    public void Compare()
    {
        Console.WriteLine(
            $"Comparing {_left.MainPackageId}#{_left.MainPackageVersion}" +
            $" and {_right.MainPackageId}#{_right.MainPackageVersion}");

        // build our filename
        string mdFilename = _left.MainPackageId.ToPascalCase() + "_" + SanitizeVersion(_left.MainPackageVersion) + "_" +
                            _right.MainPackageId.ToPascalCase() + "_" + SanitizeVersion(_right.MainPackageVersion) + ".md";

        string mdFullFilename = Path.Combine(_config.OutputDirectory, mdFilename);

        using ExportStreamWriter mdWriter = new(mdFullFilename);

        ComparePrimitiveTypes(mdWriter, _left.PrimitiveTypesByName, _right.PrimitiveTypesByName);

    }

    private void ComparePrimitiveTypes(
        ExportStreamWriter mdWriter,
        IReadOnlyDictionary<string, StructureDefinition> A,
        IReadOnlyDictionary<string, StructureDefinition> B)
    {
        HashSet<string> keysA = A?.Keys.ToHashSet() ?? [];
        HashSet<string> keysB = B?.Keys.ToHashSet() ?? [];

        HashSet<string> keyIntersection = A?.Keys.ToHashSet() ?? [];
        keyIntersection.IntersectWith(keysB);

        keysA.ExceptWith(keyIntersection);
        keysB.ExceptWith(keyIntersection);
    }

    private string SanitizeVersion(string version)
    {
        return version.Replace('.', '_').Replace('-', '_');
    }
}
