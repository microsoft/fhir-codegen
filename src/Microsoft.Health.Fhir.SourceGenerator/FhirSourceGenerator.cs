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
        private delegate bool TryGetModel<TModel>(FhirVersionInfo versionInfo, string key, out TModel model) where TModel : class;

        private static readonly DiagnosticDescriptor FailedArtifactDef = new(
            "FHIRGEN001",
            "Invalid artifact file",
            "Unable to parse {1} file '{0}'. StructureDefinition files must have a snapshot.",
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

                ProcessArtifacts(FhirArtifactClassEnum.CodeSystem, context, fhirInfo, fhirConverter, (FhirVersionInfo m, string k, out FhirCodeSystem v) => m.CodeSystems.TryGetValue(k, out v));
                ProcessArtifacts(FhirArtifactClassEnum.ValueSet, context, fhirInfo, fhirConverter, (FhirVersionInfo m, string k, out FhirValueSet v) => m.TryGetValueSet(k, out v));

                foreach (var item in fhirInfo.ValueSetsByUrl)
                {
                    var vs = item.Value;
                    foreach (var v in vs.ValueSetsByVersion)
                    {
                        v.Value.Resolve(fhirInfo.CodeSystems);
                    }
                }

                ProcessStructureDefinitions(context, fhirInfo, fhirConverter, language);

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

        private List<TModel> ProcessArtifacts<TModel>(
            FhirArtifactClassEnum expectedArtifactClass,
            GeneratorExecutionContext context,
            FhirVersionInfo fhirInfo,
            IFhirConverter fhirConverter,
            TryGetModel<TModel> modelCollectionLocator)
            where TModel : class
        {
            var result = new List<TModel>();
            foreach (var valueSet in context.AdditionalFiles.Where(f => f.Path.EndsWith($".{expectedArtifactClass}.json", StringComparison.InvariantCultureIgnoreCase)))
            {
                if (valueSet is null)
                {
                    continue;
                }

                var namespaceName = context.Compilation.AssemblyName;

                var model = ProcessFile<TModel>(valueSet.Path, fhirInfo, fhirConverter, modelCollectionLocator, out var fileName, out var canonical, out var artifactClass);
                if (model == null || artifactClass != expectedArtifactClass)
                {
                    context.ReportDiagnostic(Diagnostic.Create(FailedArtifactDef, Location.None, valueSet.Path, expectedArtifactClass));
                    continue;
                }

                result.Add(model);

                context.ReportDiagnostic(Diagnostic.Create(ProcessSuccess, Location.None, valueSet.Path, canonical, artifactClass, fhirInfo.Resources.Count));
            }

            return result;
        }

        private void ProcessStructureDefinitions(GeneratorExecutionContext context, FhirVersionInfo fhirInfo, IFhirConverter fhirConverter, ILanguage language)
        {
            foreach (var structureDef in context.AdditionalFiles.Where(f => f.Path.EndsWith(".StructureDefinition.json", StringComparison.InvariantCultureIgnoreCase)))
            {
                if (structureDef is null)
                {
                    continue;
                }

                var namespaceName = context.Compilation.AssemblyName;

                var complex = ProcessFile(structureDef.Path, fhirInfo, fhirConverter, m => m.Resources, out var fileName, out var canonical, out var artifactClass);
                if (complex == null && artifactClass != FhirArtifactClassEnum.Resource)
                {
                    context.ReportDiagnostic(Diagnostic.Create(FailedArtifactDef, Location.None, structureDef.Path, FhirArtifactClassEnum.Resource));
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

        internal TModel? ProcessFile<TModel>(
            string path,
            FhirVersionInfo fhirInfo,
            IFhirConverter fhirConverter,
            Func<FhirVersionInfo, Dictionary<string, TModel>> modelCollectionAccessor, out string fileName,
            out string? canonical,
            out FhirArtifactClassEnum artifactClass)
            where TModel : class
            => ProcessFile<TModel>(path, fhirInfo, fhirConverter, (FhirVersionInfo m, string k, out TModel v) => modelCollectionAccessor(m).TryGetValue(k, out v), out fileName, out canonical, out artifactClass);

        private TModel? ProcessFile<TModel>(
            string path,
            FhirVersionInfo fhirInfo,
            IFhirConverter fhirConverter,
            TryGetModel<TModel> modelCollectionLocator,
            out string fileName,
            out string? canonical,
            out FhirArtifactClassEnum artifactClass)
            where TModel : class
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

            modelCollectionLocator(fhirInfo, fileName, out var model);
            return model;
        }
    }
}
