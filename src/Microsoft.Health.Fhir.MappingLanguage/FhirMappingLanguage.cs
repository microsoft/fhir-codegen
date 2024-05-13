// <copyright file="FhirMappingLanguage.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime;

namespace Microsoft.Health.Fhir.MappingLanguage;

public class FhirMappingLanguage
{
    //public bool TryParse(string fml, [NotNullWhen(true)] out Hl7.Fhir.Model.StructureMap? structureMap)
    //{
    //    try
    //    {
    //        AntlrInputStream inputStream = new AntlrInputStream(fml);
    //        FmlMappingLexer fmlLexer = new FmlMappingLexer(inputStream);
    //        CommonTokenStream commonTokenStream = new CommonTokenStream(fmlLexer);
    //        FmlMappingParser fmlParser = new FmlMappingParser(commonTokenStream);

    //        FmlMappingParser.StructureMapContext structureMapContext = fmlParser.structureMap();

    //        FmlToStructureMapVisitor visitor = new();
    //        visitor.Visit(structureMapContext);

    //        structureMap = visitor.ParsedStructureMap;

    //        if (string.IsNullOrEmpty(structureMap?.Id) && (!string.IsNullOrEmpty(structureMap?.Name)))
    //        {
    //            structureMap.Id = structureMap.Name;
    //        }
    //        else if (string.IsNullOrEmpty(structureMap?.Name) && !string.IsNullOrEmpty(structureMap?.Id))
    //        {
    //            structureMap.Name = structureMap.Id;
    //        }

    //        return structureMap != null;
    //    }
    //    catch (Exception ex)
    //    {
    //        Console.WriteLine(ex.Message);
    //    }

    //    structureMap = null;
    //    return false;
    //}


    public bool TryParse(string fml, [NotNullWhen(true)] out FhirStructureMap? map)
    {
        try
        {
            AntlrInputStream inputStream = new AntlrInputStream(fml);
            FmlMappingLexer fmlLexer = new FmlMappingLexer(inputStream);
            CommonTokenStream commonTokenStream = new CommonTokenStream(fmlLexer);
            FmlMappingParser fmlParser = new FmlMappingParser(commonTokenStream);

            FmlMappingParser.StructureMapContext structureMapContext = fmlParser.structureMap();

            FmlParseVisitor visitor = new();
            visitor.Visit(structureMapContext);

            map = visitor.GetCurrentMap();

            return map != null;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        map = null;
        return false;
    }
}
