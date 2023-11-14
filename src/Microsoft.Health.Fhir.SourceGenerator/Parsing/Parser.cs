using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Configuration;
using System.Diagnostics;
using System.Text;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Microsoft.Health.Fhir.SourceGenerator.Parsing;

internal class Parser
{
    internal const string GeneratedFhirAttributeName = "Synapxe.Fhir.CodeGeneration.GeneratedFhirAttribute";
    private readonly GeneratorExecutionContext _context;
    private readonly Action<Diagnostic> _reportDiagnostic;
    private readonly CancellationToken _cancellationToken;

    public Parser(GeneratorExecutionContext context, Action<Diagnostic> reportDiagnostic, CancellationToken cancellationToken)
    {
        _context = context;
        _reportDiagnostic = reportDiagnostic;
        _cancellationToken = cancellationToken;
    }

    internal IReadOnlyList<ResourcePartialClass> GetResourcePartialClasses(IEnumerable<ClassDeclarationSyntax> classes)
    {
        _context.AnalyzerConfigOptions.GlobalOptions.TryGetValue("build_property.projectdir", out var projectDir);

        INamedTypeSymbol? generateServiceRegistrationAttribute = _context.Compilation.GetBestTypeByMetadataName(GeneratedFhirAttributeName);
        if (generateServiceRegistrationAttribute == null)
        {
            // nothing to do if this type isn't available
            return Array.Empty<ResourcePartialClass>();
        }


        var result = new List<ResourcePartialClass>();

        foreach (IGrouping<SyntaxTree, ClassDeclarationSyntax> group in classes.GroupBy(x => x.SyntaxTree))
        {
            SyntaxTree syntaxTree = group.Key;
            SemanticModel sm = _context.Compilation.GetSemanticModel(syntaxTree);

            foreach (ClassDeclarationSyntax classDec in group)
            {
                // stop if we're asked to
                _cancellationToken.ThrowIfCancellationRequested();

                string nspace = string.Empty;
                var classDecSymbol = sm.GetDeclaredSymbol(classDec);

                if (classDecSymbol == null)
                {
                    continue;
                }

                string? structureDefPath = null;
                string[] terminologies = Array.Empty<string>();
                Location? location = null;

                foreach (AttributeListSyntax mal in classDec.AttributeLists)
                {
                    foreach (AttributeSyntax ma in mal.Attributes)
                    {
                        IMethodSymbol? attrCtorSymbol = sm.GetSymbolInfo(ma, _cancellationToken).Symbol as IMethodSymbol;
                        if (attrCtorSymbol == null || !generateServiceRegistrationAttribute.Equals(attrCtorSymbol.ContainingType, SymbolEqualityComparer.Default))
                        {
                            // badly formed attribute definition, or not the right attribute
                            continue;
                        }

                        bool hasMisconfiguredInput = false;
                        ImmutableArray<AttributeData> boundAttributes = classDecSymbol!.GetAttributes();

                        if (boundAttributes.Length == 0)
                        {
                            continue;
                        }

                        foreach (AttributeData attributeData in boundAttributes)
                        {
                            if (!SymbolEqualityComparer.Default.Equals(attributeData.AttributeClass, generateServiceRegistrationAttribute))
                            {
                                continue;
                            }

                            // supports: [GeneratedFhir("Path/To/Resource.json")]
                            // supports: [GeneratedFhir(structureDefinitionPath: "Path/To/Resource.json")]
                            if (attributeData.ConstructorArguments.Any())
                            {
                                foreach (TypedConstant typedConstant in attributeData.ConstructorArguments)
                                {
                                    if (typedConstant.Kind == TypedConstantKind.Error)
                                    {
                                        hasMisconfiguredInput = true;
                                        break; // if a compilation error was found, no need to keep evaluating other args
                                    }
                                }

                                ImmutableArray<TypedConstant> items = attributeData.ConstructorArguments;

                                switch (items.Length)
                                {
                                    case 1:
                                        var value = GetItem(items[0]) as string;
                                        structureDefPath = ResolvePath(value);
                                        break;

                                    default:
                                        Debug.Assert(false, "Unexpected number of arguments in attribute constructor.");
                                        break;
                                }
                            }

                            // argument syntax takes parameters. e.g. EventId = 0
                            // supports: e.g. [GeneratedFhir("Path/To/Resource.json", TerminologyResources = new []{ "Path/To/ValueSet.json" })]
                            if (attributeData.NamedArguments.Any())
                            {
                                foreach (KeyValuePair<string, TypedConstant> namedArgument in attributeData.NamedArguments)
                                {
                                    TypedConstant typedConstant = namedArgument.Value;
                                    if (typedConstant.Kind == TypedConstantKind.Error)
                                    {
                                        hasMisconfiguredInput = true;
                                        break; // if a compilation error was found, no need to keep evaluating other args
                                    }
                                    else
                                    {
                                        TypedConstant value = namedArgument.Value;
                                        switch (namedArgument.Key)
                                        {
                                            case "TerminologyResources":
                                                var values = (ImmutableArray<TypedConstant>)GetItem(value)!;
                                                var testValues = values.Select(x => x.Value?.ToString()).Where(x => !string.IsNullOrEmpty(x)).ToArray()!;
                                                terminologies = new string[testValues.Length];
                                                for (int i = 0; i < testValues.Length; i++)
                                                {
                                                    terminologies[i] = ResolvePath(testValues[i])!;
                                                }

                                                break;
                                        }
                                    }
                                }
                            }

                            if (hasMisconfiguredInput)
                            {
                                // skip further generator execution and let compiler generate the errors
                                break;
                            }

                            location = ma.GetLocation();
                            static object? GetItem(TypedConstant arg) => arg.Kind == TypedConstantKind.Array ? arg.Values : arg.Value;

                            string? ResolvePath(string? path)
                            {
                                if (path == null)
                                {
                                    return null;
                                }

                                path = Path.Combine(path.Split('/', '\\'));
                                string found = _context.AdditionalFiles.Where(x => x.Path.EndsWith(path)).Select(x => x.Path).FirstOrDefault();
                                if (found == null && projectDir != null)
                                {
                                    found = Path.Combine(projectDir, path);
                                }

                                if (!File.Exists(found))
                                {
                                    hasMisconfiguredInput = true;
                                    _reportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.UnableToResolveFilePath, ma.GetLocation(), found));
                                }

                                return found;
                            }
                        }
                    }
                }

                if (structureDefPath != null)
                {
                    // determine the namespace the class is declared in, if any
                    SyntaxNode? potentialNamespaceParent = classDec.Parent;
                    while (potentialNamespaceParent != null &&
                           potentialNamespaceParent is not NamespaceDeclarationSyntax &&
                           potentialNamespaceParent is not FileScopedNamespaceDeclarationSyntax)
                    {
                        potentialNamespaceParent = potentialNamespaceParent.Parent;
                    }

                    if (potentialNamespaceParent is FileScopedNamespaceDeclarationSyntax fileScopedNamespace)
                    {
                        nspace = fileScopedNamespace.Name.ToString();
                    }
                    else if (potentialNamespaceParent is NamespaceDeclarationSyntax namespaceParent)
                    {
                        nspace = namespaceParent.Name.ToString();
                        while (true)
                        {
                            if (namespaceParent.Parent is not NamespaceDeclarationSyntax namespaceDeclaration)
                            {
                                break;
                            }
                            else
                            {
                                namespaceParent = namespaceDeclaration;
                            }

                            nspace = $"{namespaceParent.Name}.{nspace}";
                        }
                    }

                    result.Add(new ResourcePartialClass(location, nspace, classDecSymbol.Name, structureDefPath, terminologies));
                }
            }
        }

        return result;
    }
}
