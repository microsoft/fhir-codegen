// <copyright file="FhirMappingLanguage.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime;

namespace Microsoft.Health.Fhir.MappingLanguage;

public class FhirMappingLanguage
{
    public void TryParse(string fml)
    {
        try
        {
            AntlrInputStream inputStream = new AntlrInputStream(fml);
            FmlMappingLexer fmlLexer = new FmlMappingLexer(inputStream);
            CommonTokenStream commonTokenStream = new CommonTokenStream(fmlLexer);
            FmlMappingParser fmlParser = new FmlMappingParser(commonTokenStream);

            FmlMappingParser.StructureMapContext structureMapContext = fmlParser.structureMap();

            FmlVisitor visitor = new FmlVisitor();
            visitor.Visit(structureMapContext);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}
