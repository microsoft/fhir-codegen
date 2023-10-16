using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Microsoft.Health.Fhir.SourceGenerator.Parsing
{
    internal sealed class SyntaxContextReceiver : ISyntaxContextReceiver
    {
        public HashSet<ClassDeclarationSyntax> ClassDeclarations { get; } = new();

        internal static SyntaxContextReceiver Create()
        {
            return new SyntaxContextReceiver();
        }

        public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
        {
            if (IsSyntaxTargetForGeneration(context.Node))
            {
                var classSyntax = GetSemanticTargetForGeneration(context);
                if (classSyntax != null)
                {
                    ClassDeclarations.Add(classSyntax);
                }
            }
        }

        private static bool IsSyntaxTargetForGeneration(SyntaxNode node) =>
            node is ClassDeclarationSyntax cls && cls.AttributeLists.Count > 0;

        private static ClassDeclarationSyntax? GetSemanticTargetForGeneration(GeneratorSyntaxContext context)
        {
            var classDeclarationSyntax = (ClassDeclarationSyntax)context.Node;

            foreach (AttributeListSyntax attributeListSyntax in classDeclarationSyntax.AttributeLists)
            {
                foreach (AttributeSyntax attributeSyntax in attributeListSyntax.Attributes)
                {
                    var attributeSymbol = context.SemanticModel.GetSymbolInfo(attributeSyntax).Symbol;
                    if (attributeSymbol == null)
                    {
                        continue;
                    }

                    INamedTypeSymbol attributeContainingTypeSymbol = attributeSymbol.ContainingType;
                    string fullName = attributeContainingTypeSymbol.ToDisplayString();

                    if (fullName == Parser.GeneratedFhirAttributeName)
                    {
                        return classDeclarationSyntax;
                    }
                }
            }

            return null;
        }
    }
}
