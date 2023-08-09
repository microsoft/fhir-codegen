using System.Reflection;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Microsoft.Health.Fhir.CodeGenCommon.Models;
using Microsoft.Health.Fhir.SpecManager.Converters;
using Microsoft.Health.Fhir.SpecManager.Language;
using Microsoft.Health.Fhir.SpecManager.Manager;

namespace Microsoft.Health.Fhir.SourceGenerator
{
    [Generator]
    public class FhirSourceGenerator : ISourceGenerator
    {
        private static readonly DiagnosticDescriptor FailedParsingStructureDef = new(
            "FHIRGEN001",
            "Invalid .StructureDefinition.json file",
            "Unable to parse StructureDefinition file '{0}'. It must be a valid StructureDefinition file with a snapshot.",
            "FhirSourceGenerator",
            DiagnosticSeverity.Warning,
            true);

        private static readonly DiagnosticDescriptor TypeLoaderException = new(
            "FHIRGEN002",
            "Fatal FhirCode Generation error",
            "Type loader exception '{0}'.",
            "FhirSourceGenerator",
            DiagnosticSeverity.Error,
            true);

        private static readonly DiagnosticDescriptor UnhandledException = new(
            "FHIRGEN003",
            "Fatal FhirCode Generation error",
            "Unhandled exception '{0}'.",
            "FhirSourceGenerator",
            DiagnosticSeverity.Error,
            true);

        private static readonly DiagnosticDescriptor FailedToGenerate = new(
            "FHIRGEN004",
            "Invalid .StructureDefinition.json file",
            "Failed to generate code for json file [{0}]{1}.",
            "FhirSourceGenerator",
            DiagnosticSeverity.Error,
            true);

        private static readonly DiagnosticDescriptor ProcessSuccess = new(
            "FHIRGEN005",
            "Parsing success",
            "Canonical: {0}, Artifact Type: {1}, ResourceCount: {2}",
            "FhirSourceGenerator",
            DiagnosticSeverity.Warning,
            true);

        public void Execute(GeneratorExecutionContext context)
        {
            try
            {
                var fhirConverter = ConverterHelper.ConverterForVersion(FhirPackageCommon.FhirSequenceEnum.R4B);

                var fhirInfo = new FhirVersionInfo(FhirPackageCommon.FhirSequenceEnum.R4B);
                ILanguage language = LanguageHelper.GetLanguages("CSharpFirely2")[0];
                foreach (var structureDef in context.AdditionalFiles.Where(f => f.Path.EndsWith(".StructureDefinition.json", StringComparison.InvariantCultureIgnoreCase)))
                {
                    if (structureDef is null)
                    {
                        continue;
                    }

                    var complex = ProcessFile(structureDef.Path, fhirInfo, fhirConverter, out var fileName, out var canonical, out var artifactClass);
                    if (complex == null)
                    {
                        context.ReportDiagnostic(Diagnostic.Create(FailedParsingStructureDef, Location.None, structureDef.Path));
                        continue;
                    }

                    context.ReportDiagnostic(Diagnostic.Create(ProcessSuccess, Location.None, canonical, artifactClass, fhirInfo.Resources.Count));

                    using var memoryStream = new MemoryStream();
                    language.Export(fhirInfo, complex, memoryStream);
                    memoryStream.Position = 0;

                    if (memoryStream.Length == 0)
                    {
                        context.ReportDiagnostic(Diagnostic.Create(FailedToGenerate, Location.None, structureDef.Path));
                        continue;
                    }

                    var code = Encoding.UTF8.GetString(memoryStream.ToArray());

                    context.AddSource($"{fileName}.gen.cs", SourceText.From(code, Encoding.UTF8));
                }

            }
            catch (ReflectionTypeLoadException rex)
            {
                context.ReportDiagnostic(Diagnostic.Create(TypeLoaderException, Location.None, rex.LoaderExceptions));
            }
            catch (Exception ex)
            {
                context.ReportDiagnostic(Diagnostic.Create(UnhandledException, Location.None, ex.StackTrace.Replace(Environment.NewLine, "")));
            }
        }

        public void Initialize(GeneratorInitializationContext context)
        {
        }

        internal FhirComplex? ProcessFile(string path, FhirVersionInfo fhirInfo, IFhirConverter fhirConverter, out string fileName, out string? canonical, out FhirArtifactClassEnum artifactClass)
        {
            fileName = Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(path));

            if (!File.Exists(path))
            {
                canonical = null;
                artifactClass = default;
                return null;
            }

            var json = File.ReadAllText(path);

            if (!fhirConverter.TryParseResource(json, out var resource, out var resourceType))
            {
                canonical = null;
                artifactClass = default;
                //context.ReportDiagnostic(Diagnostic.Create(FailedParsingStructureDef, Location.None, path, json));
                return null;
            }

            fhirInfo.ProcessResource(resource, out canonical, out artifactClass);

            fhirInfo.Resources.TryGetValue(fileName, out var complex);
            return complex;
        }
    }
}
