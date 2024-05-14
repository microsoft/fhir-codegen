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
using static Microsoft.Health.Fhir.MappingLanguage.VisitorUtilities;

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

            // create our lexer
            FmlMappingLexer fmlLexer = new(inputStream);

            // create a token stream for extracting comments
            CommonTokenStream hiddenTokenStream = new(fmlLexer, 1);

            Dictionary<int, ParsedCommentNode> comments = [];

            int lastCommentTokenIndex = -1;
            int lastCommentWsStartIndex = -1;

            int lastWsTokenIndex = -1;
            int lastWsStart = -1;
            int lastWsStop = -1;
            bool lastWsHasNewline = false;

            // parse all the comment tokens
            while (hiddenTokenStream.LT(1).Type != FmlMappingLexer.Eof)
            {
                IToken t = hiddenTokenStream.LT(1);

                if (t.Type == FmlMappingLexer.WS)
                {
                    bool currentHasNewline = t.Text.Any(c => c == '\r' || c == '\n');

                    // if we are continuing from a comment, we want to make sure the lookup has additional indexes
                    if ((lastCommentTokenIndex == t.TokenIndex - 1) &&
                        (comments[lastCommentWsStartIndex].LastWsHasNewline == true))
                    {
                        // continue the last used comment so we can backtrack to it
                        lastCommentTokenIndex = t.TokenIndex;
                        comments.Add(t.StopIndex + 1, comments[lastCommentWsStartIndex]);
                    }
                    else
                    {
                        // do not track from last comment any more
                        lastCommentTokenIndex = -1;
                        lastCommentWsStartIndex = -1;
                    }

                    // we only want contiguous whitespace
                    if (lastWsTokenIndex == t.TokenIndex - 1)
                    {
                        lastWsTokenIndex = t.TokenIndex;
                        lastWsStop = t.StopIndex;
                        lastWsHasNewline = lastWsHasNewline || currentHasNewline;
                    }
                    else
                    {
                        lastWsTokenIndex = t.TokenIndex;
                        lastWsStart = t.StartIndex;
                        lastWsStop = t.StopIndex;
                        lastWsHasNewline = currentHasNewline;
                        lastCommentTokenIndex = -1;
                    }

                    hiddenTokenStream.Consume();
                    continue;
                }

                ParsedCommentNode c = new()
                {
                    NodeText = GetString(t),

                    Line = t.Line,
                    Column = t.Column,
                    TokenIndex = t.TokenIndex,
                    StartIndex = t.StartIndex,
                    StopIndex = t.StopIndex,

                    LastWsStartIndex = lastWsStart,
                    LastWsStopIndex = lastWsStop,
                    LastWsHasNewline = lastWsHasNewline,
                };

                // add from first whitespace
                if (lastWsStart != -1)
                {
                    comments[lastWsStart] = c;
                }

                // add the end of the comment so we can backtrack
                comments[t.StopIndex] = c;

                lastCommentTokenIndex = t.TokenIndex;
                lastCommentWsStartIndex = lastWsStart;

                lastWsTokenIndex = -1;
                lastWsStart = -1;
                lastWsStop = -1;
                lastWsHasNewline = false;

                hiddenTokenStream.Consume();
            }

            // reset for parsing main stream tokens
            fmlLexer.Reset();

            // create a token stream for standard parsing
            CommonTokenStream commonTokenStream = new(fmlLexer, 0);

            FmlMappingParser fmlParser = new FmlMappingParser(commonTokenStream);

            FmlMappingParser.StructureMapContext structureMapContext = fmlParser.structureMap();

            FmlParseVisitor visitor = new(comments);
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
