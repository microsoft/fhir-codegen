using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Microsoft.Health.Fhir.SourceGenerator.Parsing;

namespace Microsoft.Health.Fhir.SourceGenerator
{
    [Generator]
    public class FhirSourceGenerator : ISourceGenerator
    {
        /// <inheritdoc/>
        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(SyntaxContextReceiver.Create);
        }

        public void Execute(GeneratorExecutionContext context)
        {
            if (context.SyntaxContextReceiver is not SyntaxContextReceiver receiver || receiver.ClassDeclarations.Count == 0)
            {
                // nothing to do yet
                return;
            }

#if DEBUG
            System.Diagnostics.Debugger.Launch();
#endif

            var parser = new Parser(context, context.ReportDiagnostic, context.CancellationToken);
            var resourcePartialClasses = parser.GetResourcePartialClasses(receiver.ClassDeclarations);

            foreach (var resourceClass in resourcePartialClasses)
            {
                var emitter = new Emitter(resourceClass, context.ReportDiagnostic);
                var code = emitter.Emit();
                if (!string.IsNullOrEmpty(code))
                {
                    context.AddSource($"{resourceClass.Name}.cs", SourceText.From(code!, Encoding.UTF8));
                }
            }
        }
    }
}
