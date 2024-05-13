// <copyright file="FmlParseVisitor.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using CA = System.Diagnostics.CodeAnalysis;

using static FmlMappingParser;
using static Microsoft.Health.Fhir.MappingLanguage.VisitorUtilities;
using Antlr4.Runtime;
using Microsoft.Health.Fhir.CodeGenCommon.FhirExtensions;
using Hl7.Fhir.Model;
using System.Diagnostics.Metrics;
using Newtonsoft.Json.Linq;
using System.Reflection;
using Microsoft.Health.Fhir.CodeGenCommon.Extensions;
using System.Globalization;

namespace Microsoft.Health.Fhir.MappingLanguage;

public class FmlParseVisitor : FmlMappingBaseVisitor<object>
{
    private Dictionary<string, MetadataDeclaration> _metaByPath = [];
    private MapDeclaration? _mapDirective = null;
    private Dictionary<string, StructureDeclaration> _structuresByUrl = [];
    private Dictionary<string, ImportDeclaration> _importsByUrl = [];
    private Dictionary<string, ConstantDeclaration> _constantsByName = [];
    private Dictionary<string, GroupDeclaration> _groupsByName = [];

    public FhirStructureMap GetCurrentMap()
    {
        return new FhirStructureMap()
        {
            MetadataByPath = _metaByPath,
            MapDirective = _mapDirective,
            StructuresByUrl = _structuresByUrl,
            ImportsByUrl = _importsByUrl,
            ConstantsByName = _constantsByName,
            GroupsByName = _groupsByName,
        };
    }
    
    public override object VisitStructureMap([NotNull] StructureMapContext ctx)
    {
        // reset top level data structures
        _metaByPath = [];
        _mapDirective = null;
        _structuresByUrl = [];
        _importsByUrl = [];
        _constantsByName = [];
        _groupsByName = [];

        return base.VisitStructureMap(ctx);
    }

    public override object VisitMetadataDeclaration([NotNull] MetadataDeclarationContext ctx)
    {
        MetadataDeclaration value = new()
        {
            ElementPath = ctx.qualifiedIdentifier().GetText(),
            Literal = GetLiteral(ctx.literal()),
            MarkdownValue = GetString(ctx.markdownLiteral()),
            InlineComment = GetString(ctx.INLINE_COMMENT()) ?? string.Empty,
        };

        if (!string.IsNullOrEmpty(value.ElementPath))
        {
            _metaByPath[value.ElementPath] = value;
        }

        return base.VisitMetadataDeclaration(ctx);
    }

    public override object VisitMapDeclaration([NotNull] MapDeclarationContext context)
    {
        _mapDirective = new()
        {
            LineComments = context.LINE_COMMENT()?.Select(x => GetString(x)).ToList() ?? [],
            Url = context.url().GetText(),
            Identifier = context.identifier().GetText(),
            IdentifierTokenType = (FmlTokenTypeCodes)context.identifier().Stop.Type,
            InlineComment = GetString(context.INLINE_COMMENT()) ?? string.Empty,
        };

        return base.VisitMapDeclaration(context);
    }

    public override object VisitStructureDeclaration([NotNull] StructureDeclarationContext context)
    {
        StructureDeclaration value = new()
        {
            LineComments = context.LINE_COMMENT()?.Select(x => GetString(x)).ToList() ?? [],
            Url = context.url().GetText(),
            Alias = context.structureAlias()?.GetText(),
            ModelModeLiteral = context.modelMode().GetText(),
            ModelMode = GetEnum<StructureMap.StructureMapModelMode>(context.modelMode().GetText()),
            InlineComment = GetString(context.INLINE_COMMENT()) ?? string.Empty,
        };

        _structuresByUrl.Add(value.Url, value);

        return base.VisitStructureDeclaration(context);
    }

    public override object VisitImportDeclaration([NotNull] ImportDeclarationContext context)
    {
        ImportDeclaration value = new()
        {
            LineComments = context.LINE_COMMENT()?.Select(x => GetString(x)).ToList() ?? [],
            Url = context.url().GetText(),
            InlineComment = GetString(context.INLINE_COMMENT()) ?? string.Empty,
        };

        _importsByUrl.Add(value.Url, value);

        return base.VisitImportDeclaration(context);
    }

    public override object VisitConstantDeclaration([NotNull] ConstantDeclarationContext context)
    {
        ConstantDeclaration value = new()
        {
            LineComments = context.LINE_COMMENT()?.Select(x => GetString(x)).ToList() ?? [],
            Name = context.ID().GetText(),
            Value = ExtractFpExpression(context.fpExpression())!,
            InlineComment = GetString(context.INLINE_COMMENT()) ?? string.Empty,
        };

        _constantsByName.Add(value.Name, value);

        return base.VisitConstantDeclaration(context);
    }

    public override object VisitGroupDeclaration([NotNull] GroupDeclarationContext context)
    {
        List<GroupParameter> parameters = [];

        foreach (ParameterContext parameterContext in context.parameters().parameter())
        {
            parameters.Add(new GroupParameter
            {
                InputModeLiteral = GetString(parameterContext.inputMode())!,
                InputMode = GetEnum<StructureMap.StructureMapInputMode>(GetString(parameterContext.inputMode())!),
                Identifier = GetString(parameterContext.ID())!,
                TypeIdentifier = GetString(parameterContext.typeIdentifier()?.identifier())!,
            });
        }

        List<GroupExpression> expressions = [];

        foreach (ExpressionContext expressionContext in context.groupExpressions().expression())
        {
            expressions.Add(ExtractGroupExpression(expressionContext) ?? throw new Exception("Failed to parse expression!"));
        }

        GroupDeclaration value = new()
        {
            LineComments = context.LINE_COMMENT()?.Select(x => GetString(x)).ToList() ?? [],
            Name = context.ID().GetText(),
            Parameters = parameters,
            ExtendsIdentifier = GetString(context.extends()),
            TypeModeLiteral = GetString(context.typeMode()),
            TypeMode = GetTypeMode(GetString(context.typeMode())),              // note this is handled differently because the FML values are different than the FHIR values
            InlineComment = GetString(context.INLINE_COMMENT()) ?? string.Empty,
            Expressions = expressions,
        };

        _groupsByName.Add(value.Name, value);

        return base.VisitGroupDeclaration(context);

        StructureMap.StructureMapGroupTypeMode? GetTypeMode(string? value) => value switch
        {
            "type+" => StructureMap.StructureMapGroupTypeMode.TypeAndTypes,
            "<<type+>>" => StructureMap.StructureMapGroupTypeMode.TypeAndTypes,
            "type-and-types" => StructureMap.StructureMapGroupTypeMode.TypeAndTypes,
            "<<type-and-types>>" => StructureMap.StructureMapGroupTypeMode.TypeAndTypes,
            "type" => StructureMap.StructureMapGroupTypeMode.Types,
            "<<type>>" => StructureMap.StructureMapGroupTypeMode.Types,
            _ => null,
        };

    }

    private GroupExpression? ExtractGroupExpression(ExpressionContext? expressionContext)
    {
        if (expressionContext == null)
        {
            return null;
        }

        switch (expressionContext)
        {
            case MapSimpleCopyContext mapSimpleCopy:
                {
                    if (mapSimpleCopy.qualifiedIdentifier() == null || mapSimpleCopy.qualifiedIdentifier().Length != 2)
                    {
                        throw new Exception("MapSimpleCopyContext missing required source and target");
                    }

                    return new GroupExpression
                    {
                        LineComments = mapSimpleCopy.LINE_COMMENT()?.Select(x => GetString(x)).ToList() ?? [],
                        SimpleCopyExpression = new()
                        {
                            Source = GetString(mapSimpleCopy.qualifiedIdentifier()[0])!,
                            Target = GetString(mapSimpleCopy.qualifiedIdentifier()[1])!,
                        },
                        InlineComment = GetString(mapSimpleCopy.INLINE_COMMENT()) ?? string.Empty,
                    };
                }

            case MapFhirPathContext mapFhirPath:
                return new GroupExpression
                {
                    LineComments = mapFhirPath.LINE_COMMENT()?.Select(x => GetString(x)).ToList() ?? [],
                    FhirPathExpression = ExtractFpExpression(mapFhirPath.fpExpression()),
                    InlineComment = GetString(mapFhirPath.INLINE_COMMENT()) ?? string.Empty,
                };

            case MapFhirMarkupContext mapFhirMarkup:
                return new GroupExpression
                {
                    LineComments = mapFhirMarkup.LINE_COMMENT()?.Select(x => GetString(x)).ToList() ?? [],
                    MappingExpression = ExtractFmlMappingExpression(mapFhirMarkup.mapExpression()),
                    InlineComment = GetString(mapFhirMarkup.INLINE_COMMENT()) ?? string.Empty,
                };
        }

        throw new Exception("Unhandled GroupExpression type");
    }

    private FmlGroupExpression? ExtractFmlMappingExpression(MapExpressionContext? ctx)
    {
        if (ctx == null)
        {
            return null;
        }

        List<FmlExpressionSource> sources = [];
        foreach (MapExpressionSourceContext sourceContext in ctx.mapExpressionSource())
        {
            sources.Add(new()
            {
                Identifier = GetString(sourceContext.qualifiedIdentifier())!,
                TypeIdentifier = GetString(sourceContext.typeIdentifier()?.identifier())!,
                Cardinality = GetString(sourceContext.sourceCardinality())!,
                DefaultExpression = ExtractFpExpression(sourceContext.sourceDefault()?.fpExpression()),
                ListModeLiteral = GetString(sourceContext.sourceListMode()),
                ListMode = GetEnum<StructureMap.StructureMapSourceListMode>(GetString(sourceContext.sourceListMode())),
                Alias = GetString(sourceContext.alias()),
                WhereClause = ExtractFpExpression(sourceContext.whereClause()?.fpExpression()),
                CheckClause = ExtractFpExpression(sourceContext.checkClause()?.fpExpression()),
                LogExpression = ExtractFpExpression(sourceContext.log()?.fpExpression()),
            });
        }

        List<FmlExpressionTarget> targets = [];
        foreach (MapLineTargetContext targetContext in ctx.mapExpressionTarget()?.mapLineTarget() ?? [])
        {
            targets.Add(new()
            {
                Identifier = GetString(targetContext.qualifiedIdentifier())!,
                Transform = targetContext.transform() == null ? null :new FmlTargetTransform()
                {
                    Literal = GetLiteral(targetContext.transform()?.literal()),
                    Identifier = GetString(targetContext.transform()?.qualifiedIdentifier()),
                    Invocation = ExtractInvocation(targetContext.transform()?.invocation()),
                },
                Invocation = ExtractInvocation(targetContext.invocation()),
                Alias = GetString(targetContext.alias()),
                TargetListModeLiteral = GetString(targetContext.targetListMode()),
                TargetListMode = GetEnum<StructureMap.StructureMapTargetListMode>(GetString(targetContext.targetListMode())),
            });
        }

        return new FmlGroupExpression()
        {
            Sources = sources,
            Targets = targets,
            DependentExpression = ExtractDependentExpression(ctx.dependentExpression()),
            Name = GetString(ctx.mapExpressionName()),
        };

        FmlDependentExpression? ExtractDependentExpression(DependentExpressionContext? ctx)
        {
            if (ctx == null)
            {
                return null;
            }

            List<FmlInvocation> invocations = [];
            foreach (InvocationContext invocationContext in ctx.invocation())
            {
                invocations.Add(ExtractInvocation(invocationContext)!);
            }

            List<GroupExpression> expressions = [];
            foreach (ExpressionContext expressionContext in ctx.groupExpressions()?.expression() ?? Array.Empty<ExpressionContext>())
            {
                expressions.Add(ExtractGroupExpression(expressionContext) ?? throw new Exception("Failed to parse expression!"));
            }

            return new FmlDependentExpression
            {
                Invocations = invocations,
                Expressions = expressions,
            };
        }

        FmlInvocation? ExtractInvocation(InvocationContext? ctx)
        {
            if (ctx == null)
            {
                return null;
            }

            List<FmlInvocationParam> p = [];
            foreach (ParamContext param in ctx.paramList().param())
            {
                p.Add(new()
                {
                    Identifier = GetString(param.ID())!,
                    Literal = GetLiteral(param.literal()),
                });
            }

            return new FmlInvocation
            {
                Identifier = GetString(ctx.identifier())!,
                Parameters = p,
            };
        }
    }

    private FpExpression? ExtractFpExpression(FpExpressionContext? ctx)
    {
        if (ctx == null)
        {
            return null;
        }

        switch (ctx)
        {
            case TermExpressionContext termContext:
                return new FpExpression
                {
                    Expression = ctx.GetText(),
                    ExpressionRule = (FmlRuleCodes)ctx.RuleIndex,
                    TermExpression = extractFpTerm(termContext.fpTerm()),
                };

            case InvocationExpressionContext invocationContext:
                return new FpExpression
                {
                    Expression = ctx.GetText(),
                    ExpressionRule = (FmlRuleCodes)ctx.RuleIndex,
                    InvocationExpression = new()
                    {
                        Expression = ExtractFpExpression(invocationContext.fpExpression()) ?? throw new Exception("InvocationExpression missing required Expression"),
                        Invocation = extractFpInvocation(invocationContext.fpInvocation()) ?? throw new Exception("InvocationExpression missing required Invocation"),
                    },
                };

            case IndexerExpressionContext indexerContext:
                {
                    if (indexerContext.fpExpression() == null || indexerContext.fpExpression().Length != 2)
                    {
                        throw new Exception("IndexerExpression missing required Expression and Indexer");

                    }

                    return new FpExpression
                    {
                        Expression = ctx.GetText(),
                        ExpressionRule = (FmlRuleCodes)ctx.RuleIndex,
                        IndexerExpression = new()
                        {
                            Expression = ExtractFpExpression(indexerContext.fpExpression()[0])!,
                            Index = ExtractFpExpression(indexerContext.fpExpression()[1])!,
                        },
                    };
                }

            case PolarityExpressionContext polarityContext:
                return new FpExpression
                {
                    Expression = ctx.GetText(),
                    ExpressionRule = (FmlRuleCodes)ctx.RuleIndex,
                    PolarityExpression = new()
                    {
                        Expression = ExtractFpExpression(polarityContext.fpExpression())!,
                        Polarity = (FmlPolarityCodes)polarityContext.fpPolarityLiteral().Stop.Type,
                        Literal = polarityContext.fpPolarityLiteral().GetText(),
                        IsPositive = polarityContext.Start.TokenIndex == (int)FmlPolarityCodes.Positive,
                    },
                };

            case MultiplicativeExpressionContext multiplicativeContext:
                {
                    if (multiplicativeContext.fpExpression() == null || multiplicativeContext.fpExpression().Length != 2)
                    {
                        throw new Exception("MultiplicativeExpression missing required Left and Right");
                    }

                    return new FpExpression
                    {
                        Expression = ctx.GetText(),
                        ExpressionRule = (FmlRuleCodes)ctx.RuleIndex,
                        MultiplicativeExpression = new()
                        {
                            Left = ExtractFpExpression(multiplicativeContext.fpExpression()[0])!,
                            OperatorLiteral = multiplicativeContext.fpMultiplicativeLiteral().GetText(),
                            Operator = (FmlMultiplicativeOpCodes)multiplicativeContext.fpMultiplicativeLiteral().Stop.Type,
                            Right = ExtractFpExpression(multiplicativeContext.fpExpression()[1])!,
                        },
                    };
                }

            case AdditiveExpressionContext additiveContext:
                {
                    if (additiveContext.fpExpression() == null || additiveContext.fpExpression().Length != 2)
                    {
                        throw new Exception("AdditiveExpression missing required Left and Right");
                    }

                    return new FpExpression
                    {
                        Expression = ctx.GetText(),
                        ExpressionRule = (FmlRuleCodes)ctx.RuleIndex,
                        AdditiveExpression = new()
                        {
                            Left = ExtractFpExpression(additiveContext.fpExpression()[0])!,
                            OperatorLiteral = additiveContext.fpAdditiveLiteral().GetText(),
                            Operator = (FmlAdditiveOpCodes)additiveContext.fpAdditiveLiteral().Stop.Type,
                            Right = ExtractFpExpression(additiveContext.fpExpression()[1])!,
                        },
                    };
                }

            case TypeExpressionContext typeContext:
                return new FpExpression
                {
                    Expression = ctx.GetText(),
                    ExpressionRule = (FmlRuleCodes)ctx.RuleIndex,
                    TypeExpression = new()
                    {
                        Expression = ExtractFpExpression(typeContext.fpExpression())!,
                        TypeAssignmentLiteral = typeContext.fpTypeAssertionLiteral().GetText(),
                        TypeIdentifier = GetString(typeContext.fpTypeSpecifier()) ?? throw new Exception("TypeExpression missing required Type Identifier"),
                    },
                };

            case UnionExpressionContext unionContext:
                {
                    if (unionContext.fpExpression() == null || unionContext.fpExpression().Length != 2)
                    {
                        throw new Exception("UnionExpression missing required Left and Right");
                    }

                    return new FpExpression
                    {
                        Expression = ctx.GetText(),
                        ExpressionRule = (FmlRuleCodes)ctx.RuleIndex,
                        UnionExpression = new()
                        {
                            Left = ExtractFpExpression(unionContext.fpExpression()[0])!,
                            OperatorLiteral = unionContext.fpUnionLiteral().GetText(),
                            Operator = (FmlUnionOpCodes)unionContext.fpUnionLiteral().Stop.Type,
                            Right = ExtractFpExpression(unionContext.fpExpression()[1])!,
                        },
                    };
                }

            case InequalityExpressionContext inequalityContext:
                {
                    if (inequalityContext.fpExpression() == null || inequalityContext.fpExpression().Length != 2)
                    {
                        throw new Exception("InequalityExpression missing required Left and Right");
                    }

                    return new FpExpression
                    {
                        Expression = ctx.GetText(),
                        ExpressionRule = (FmlRuleCodes)ctx.RuleIndex,
                        InequalityExpression = new()
                        {
                            Left = ExtractFpExpression(inequalityContext.fpExpression()[0])!,
                            OperatorLiteral = inequalityContext.fpInequalityLiteral().GetText(),
                            Operator = (FmlInequalityOpCodes)inequalityContext.fpInequalityLiteral().Stop.Type,
                            Right = ExtractFpExpression(inequalityContext.fpExpression()[1])!,
                        },
                    };
                }

            case EqualityExpressionContext equalityContext:
                {
                    if (equalityContext.fpExpression() == null || equalityContext.fpExpression().Length != 2)
                    {
                        throw new Exception("EqualityExpression missing required Left and Right");
                    }

                    return new FpExpression
                    {
                        Expression = ctx.GetText(),
                        ExpressionRule = (FmlRuleCodes)ctx.RuleIndex,
                        EqualityExpression = new()
                        {
                            Left = ExtractFpExpression(equalityContext.fpExpression()[0])!,
                            OperatorLiteral = equalityContext.fpEqualityLiteral().GetText(),
                            Operator = (FmlEqualityOpCodes)equalityContext.fpEqualityLiteral().Stop.Type,
                            Right = ExtractFpExpression(equalityContext.fpExpression()[1])!,
                        },
                    };
                }

            case MembershipExpressionContext membershipContext:
                {
                    if (membershipContext.fpExpression() == null || membershipContext.fpExpression().Length != 2)
                    {
                        throw new Exception("MembershipExpression missing required Left and Right");
                    }

                    return new FpExpression
                    {
                        Expression = ctx.GetText(),
                        ExpressionRule = (FmlRuleCodes)ctx.RuleIndex,
                        MembershipExpression = new()
                        {
                            Left = ExtractFpExpression(membershipContext.fpExpression()[0])!,
                            OperatorLiteral = membershipContext.fpMembershipLiteral().GetText(),
                            Operator = (FmlMembershipOpCodes)membershipContext.fpMembershipLiteral().Stop.Type,
                            Right = ExtractFpExpression(membershipContext.fpExpression()[1])!,
                        },
                    };
                }

            case AndExpressionContext andContext:
                {
                    if (andContext.fpExpression() == null || andContext.fpExpression().Length != 2)
                    {
                        throw new Exception("AndExpression missing required Left and Right");
                    }

                    return new FpExpression
                    {
                        Expression = ctx.GetText(),
                        ExpressionRule = (FmlRuleCodes)ctx.RuleIndex,
                        AndExpression = new()
                        {
                            Left = ExtractFpExpression(andContext.fpExpression()[0])!,
                            OperatorLiteral = andContext.fpAndLiteral().GetText(),
                            Operator = (FmlAndOpCodes)andContext.fpAndLiteral().Stop.Type,
                            Right = ExtractFpExpression(andContext.fpExpression()[1])!,
                        },
                    };
                }

            case OrExpressionContext orContext:
                {
                    if (orContext.fpExpression() == null || orContext.fpExpression().Length != 2)
                    {
                        throw new Exception("OrExpression missing required Left and Right");
                    }

                    return new FpExpression
                    {
                        Expression = ctx.GetText(),
                        ExpressionRule = (FmlRuleCodes)ctx.RuleIndex,
                        OrExpression = new()
                        {
                            Left = ExtractFpExpression(orContext.fpExpression()[0])!,
                            OperatorLiteral = orContext.fpOrLiteral().GetText(),
                            Operator = (FmlOrOpCodes)orContext.fpOrLiteral().Stop.Type,
                            Right = ExtractFpExpression(orContext.fpExpression()[1])!,
                        },
                    };
                }

            case ImpliesExpressionContext impliesContext:
                {
                    if (impliesContext.fpExpression() == null || impliesContext.fpExpression().Length != 2)
                    {
                        throw new Exception("ImpliesExpression missing required Left and Right");
                    }

                    return new FpExpression
                    {
                        Expression = ctx.GetText(),
                        ExpressionRule = (FmlRuleCodes)ctx.RuleIndex,
                        ImpliesExpression = new()
                        {
                            Left = ExtractFpExpression(impliesContext.fpExpression()[0])!,
                            OperatorLiteral = impliesContext.fpImpliesLiteral().GetText(),
                            Operator = (FmlImpliesOpCodes)impliesContext.fpImpliesLiteral().Stop.Type,
                            Right = ExtractFpExpression(impliesContext.fpExpression()[1])!,
                        },
                    };
                }
        }

        throw new Exception("Unhandled FHIRPath expression type");


        FpTerm? extractFpTerm(FpTermContext? ctx)
        {
            if (ctx == null)
            {
                return null;
            }

            switch (ctx)
            {
                case InvocationTermContext invocation:
                    return new FpTerm
                    {
                        InvocationTerm = extractFpInvocation(invocation.fpInvocation()),
                    };

                case LiteralTermContext literal:
                    return new FpTerm
                    {
                        LiteralTerm = GetLiteral(literal.literal()),
                    };

                case ExternalConstantTermContext external:
                    return new FpTerm
                    {
                        ExternalConstantTerm = external.fpExternalConstant().identifier().GetText(),
                        ExternalConstantTokenType = (FmlTokenTypeCodes)external.fpExternalConstant().identifier().Stop.Type,
                    };

                case ParenthesizedTermContext parenthesized:
                    return new FpTerm
                    {
                        ParenthesizedTerm = ExtractFpExpression(parenthesized.fpExpression()),
                    };
            }

            throw new InvalidOperationException($"Unexpected term type: {ctx.GetType()}");
        }

        FpInvocation? extractFpInvocation(FpInvocationContext? ctx)
        {
            if (ctx == null)
            {
                return null;
            }

            switch (ctx)
            {
                case FunctionInvocationContext functionInvocation:
                    return new FpInvocation
                    {
                        FunctionInvocation = extractFpFunction(functionInvocation.fpFunction()),
                    };

                case MemberInvocationContext memberInvocation:
                    return new FpInvocation
                    {
                        MemberInvocation = memberInvocation.identifier().GetText(),
                        MemberInvocationTokenType = (FmlTokenTypeCodes?)memberInvocation.identifier()?.Stop.Type,
                    };

                case ThisInvocationContext thisInvocation:
                    return new FpInvocation
                    {
                        ThisInvocation = true,
                    };

                case IndexInvocationContext indexInvocation:
                    return new FpInvocation
                    {
                        IndexInvocation = true,
                    };

                case TotalInvocationContext totalInvocation:
                    return new FpInvocation
                    {
                        TotalInvocation = true,
                    };
            }

            throw new InvalidOperationException($"Unexpected invocation type: {ctx.GetType()}");
        }

        FpFunction? extractFpFunction(FpFunctionContext? ctx)
        {
            if (ctx == null)
            {
                return null;
            }

            return new FpFunction
            {
                Identifier = ctx.identifier().GetText(),
                IdentifierTokenType = (FmlTokenTypeCodes)ctx.identifier().Stop.Type,
                Parameters = ctx.fpParamList()?.fpExpression()?.Select(ExtractFpExpression).ToList() ?? [],
            };
        }
    }
}
