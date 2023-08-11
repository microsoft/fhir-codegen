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
            "JsonPath: {0} Canonical: {1}, Artifact Type: {2}, ResourceCount: {3}",
            "FhirSourceGenerator",
            DiagnosticSeverity.Info,
            true);

        public void Execute(GeneratorExecutionContext context)
        {
#if DEBUG
            // System.Diagnostics.Debugger.Launch();
#endif

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

                    var namespaceName = context.Compilation.AssemblyName;

                    var complex = ProcessFile(structureDef.Path, fhirInfo, fhirConverter, out var fileName, out var canonical, out var artifactClass);
                    if (complex == null)
                    {
                        context.ReportDiagnostic(Diagnostic.Create(FailedParsingStructureDef, Location.None, structureDef.Path));
                        continue;
                    }

                    context.ReportDiagnostic(Diagnostic.Create(ProcessSuccess, Location.None, structureDef.Path, canonical, artifactClass, fhirInfo.Resources.Count));

                    using var memoryStream = new MemoryStream();

                    language.Namespace = GetNamespaceName(namespaceName, structureDef.Path, context.AnalyzerConfigOptions.GlobalOptions.TryGetValue("build_property.projectdir", out var projectDir) ? projectDir : null);

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

        private string? GetNamespaceName(string? namespaceName, string path, string? projectPath)
        {
            string folderName;
            if (projectPath != null)
            {
                folderName = (Path.GetDirectoryName(path) + Path.DirectorySeparatorChar).Replace(projectPath, string.Empty);
                if (folderName.EndsWith($"{Path.DirectorySeparatorChar}"))
                {
                    folderName = folderName.Substring(0, folderName.Length - 1);
                }
            }
            else
            {
                folderName = Path.GetDirectoryName(path).Split(Path.DirectorySeparatorChar).Last();
            }

            if (!string.IsNullOrEmpty(folderName) && folderName != namespaceName)
            {
                return $"{namespaceName}.{folderName.Replace(Path.DirectorySeparatorChar, '.')}";
            }
            else
            {
                return namespaceName;
            }
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
                return null;
            }

            fhirInfo.ProcessResource(resource, out canonical, out artifactClass);

            fhirInfo.Resources.TryGetValue(fileName, out var complex);
            return complex;
        }
    }
}
