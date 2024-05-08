// <copyright file="FmlVisitor.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime.Misc;

using static FmlMappingParser;

namespace Microsoft.Health.Fhir.MappingLanguage;

public class FmlVisitor : FmlMappingBaseVisitor<object>
{
    public override object VisitStructureMap([NotNull] StructureMapContext context)
    {
        HeaderContext mapHeaderContext = context.header();

        Console.WriteLine($"   Map URL: {mapHeaderContext.mapUrl().stringValue().GetText()}");
        Console.WriteLine($"  Map Name: {mapHeaderContext.mapName().stringValue().GetText()}");
        Console.WriteLine($" Map Title: {mapHeaderContext.mapTitle().stringValue().GetText()}");
        Console.WriteLine($"Map Status: {mapHeaderContext.mapStatus().stringValue().GetText()}");

        // MapUrlContext mapUrlContext = context.mapUrl();
        // MapNameContext mapNameContext = context.mapName();
        // MapTitleContext mapTitleContext = context.mapTitle();
        // MapStatusContext mapStatusContext = context.mapStatus();

        // Console.WriteLine($"   Map URL: {mapUrlContext.stringValue().GetText()}");
        // Console.WriteLine($"  Map Name: {mapNameContext.stringValue().GetText()}");
        // Console.WriteLine($" Map Title: {mapTitleContext.stringValue().GetText()}");
        // Console.WriteLine($"Map Status: {mapStatusContext.stringValue().GetText()}");

        StructureContext[] structureContexts = context.structure();
        ImportsContext[] importsContexts = context.imports();
        ConstContext[] constantContexts = context.@const();
        GroupContext[] groupContexts = context.group();

        Console.WriteLine($"Structures: {structureContexts.Length}");
        foreach (StructureContext sc in structureContexts)
        {
            Console.WriteLine($"Structure: {sc.url().GetText()} {sc.structureAlias().identifier().GetText()} {sc.modelMode().GetText()}");
        }

        Console.WriteLine($"Imports: {importsContexts.Length}");
        foreach (ImportsContext importsContext in importsContexts)
        {
            Console.WriteLine($"Import: {importsContext.url().GetText()}");
        }

        Console.WriteLine($"Constants: {constantContexts.Length}");
        foreach (ConstContext constContext in constantContexts)
        {
            Console.WriteLine($"Constant: {constContext.GetText()}");
        }

        Console.WriteLine($"Groups: {groupContexts.Length}");
        foreach (GroupContext gc in groupContexts)
        {
            Console.WriteLine($"Group: {gc.ID()}");
            foreach (RuleContext rc in gc.rules().rule())
            {
                Console.WriteLine($"  Rule: {rc.GetText()}");
            }
        }

        return base.VisitStructureMap(context);
    }
}
