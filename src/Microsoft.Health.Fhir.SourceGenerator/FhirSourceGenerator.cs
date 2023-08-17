using System.Reflection;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Microsoft.Health.Fhir.CodeGenCommon.Models;
using Microsoft.Health.Fhir.SpecManager.Converters;
using Microsoft.Health.Fhir.SpecManager.Language;
using Microsoft.Health.Fhir.SpecManager.Manager;
using Microsoft.Health.Fhir.SpecManager.Models;

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

        private static readonly ILanguage language = new CSharpFirely2();
        private static readonly IFhirConverter fhirConverter = new FromFhirExpando();

        public void Execute(GeneratorExecutionContext context)
        {
#if DEBUG
            // System.Diagnostics.Debugger.Launch();
#endif

            try
            {
                var fhirInfo = new FhirVersionInfo(FhirPackageCommon.FhirSequenceEnum.R4B);

                ProcessArtifacts(FhirArtifactClassEnum.CodeSystem, context, fhirInfo, (FhirVersionInfo m, string k, out FhirCodeSystem v) => m.CodeSystems.TryGetValue(k, out v));
                ProcessArtifacts(FhirArtifactClassEnum.ValueSet, context, fhirInfo, (FhirVersionInfo m, string k, out FhirValueSet v) => m.TryGetValueSet(k, out v));

                foreach (var item in fhirInfo.ValueSetsByUrl)
                {
                    var vs = item.Value;
                    foreach (var v in vs.ValueSetsByVersion)
                    {
                        v.Value.Resolve(fhirInfo.CodeSystems);
                    }
                }

                ProcessStructureDefinitions(context, fhirInfo);

            }
            catch (ReflectionTypeLoadException rex)
            {
                context.ReportDiagnostic(Diagnostic.Create(TypeLoaderException, Location.None, rex.LoaderExceptions));
            }
            catch (Exception ex)
            {
                context.ReportDiagnostic(Diagnostic.Create(UnhandledException, Location.None, ex));
            }
        }

        public void Initialize(GeneratorInitializationContext context)
        {
        }

        private void ProcessArtifacts<TModel>(
            FhirArtifactClassEnum expectedArtifactClass,
            GeneratorExecutionContext context,
            FhirVersionInfo fhirInfo,
            TryGetModel<TModel> modelCollectionLocator)
            where TModel : class
        {
            foreach (var valueSet in context.AdditionalFiles.Where(f => f.Path.EndsWith($".{expectedArtifactClass}.json", StringComparison.InvariantCultureIgnoreCase)))
            {
                if (valueSet is null)
                {
                    continue;
                }

                var namespaceName = context.Compilation.AssemblyName;

                var model = ProcessFile(valueSet.Path, fhirInfo, fhirConverter, modelCollectionLocator, out var fileName, out var canonical, out var artifactClass);
                if (model == null || artifactClass != expectedArtifactClass)
                {
                    context.ReportDiagnostic(Diagnostic.Create(FailedArtifactDef, Location.None, valueSet.Path, expectedArtifactClass));
                    continue;
                }

                context.ReportDiagnostic(Diagnostic.Create(ProcessSuccess, Location.None, valueSet.Path, canonical, artifactClass, fhirInfo.Resources.Count));
            }
        }

        private void ProcessStructureDefinitions(GeneratorExecutionContext context, FhirVersionInfo fhirInfo)
        {
            foreach (var structureDef in context.AdditionalFiles.Where(f => f.Path.EndsWith(".StructureDefinition.json", StringComparison.InvariantCultureIgnoreCase)))
            {
                if (structureDef is null)
                {
                    continue;
                }

                var complex = ProcessFile(structureDef.Path, fhirInfo, fhirConverter, m => m.Resources, out var id, out var canonical, out var artifactClass);
                if (complex == null || artifactClass != FhirArtifactClassEnum.Resource)
                {
                    context.ReportDiagnostic(Diagnostic.Create(FailedArtifactDef, Location.None, structureDef.Path, FhirArtifactClassEnum.Resource));
                    continue;
                }

                context.ReportDiagnostic(Diagnostic.Create(ProcessSuccess, Location.None, structureDef.Path, canonical, artifactClass, fhirInfo.Resources.Count));

                AddFhirResourceSource(context, fhirInfo, complex, structureDef.Path);
            }
        }

        private void AddFhirResourceSource(GeneratorExecutionContext context, FhirVersionInfo fhirInfo, FhirComplex complex, string originalFilePath)
        {
            var namespaceName = context.Compilation.AssemblyName;
            language.Namespace = GetNamespaceName(namespaceName, originalFilePath, context.AnalyzerConfigOptions.GlobalOptions.TryGetValue("build_property.projectdir", out var projectDir) ? projectDir : null);
            using var memoryStream = new MemoryStream();
            language.Export(fhirInfo, complex, memoryStream);
            memoryStream.Position = 0;

            if (memoryStream.Length == 0)
            {
                context.ReportDiagnostic(Diagnostic.Create(FailedToGenerate, Location.None, originalFilePath));
                return;
            }

            var code = Encoding.UTF8.GetString(memoryStream.ToArray());

            context.AddSource($"{complex.Id}.cs", SourceText.From(code, Encoding.UTF8));
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
            Func<FhirVersionInfo, Dictionary<string, TModel>> modelCollectionAccessor,
            out string fileName,
            out string? canonical,
            out FhirArtifactClassEnum artifactClass)
            where TModel : class
            => ProcessFile(path, fhirInfo, fhirConverter, (FhirVersionInfo m, string k, out TModel v) => modelCollectionAccessor(m).TryGetValue(k, out v), out fileName, out canonical, out artifactClass);

        private TModel? ProcessFile<TModel>(
            string path,
            FhirVersionInfo fhirInfo,
            IFhirConverter fhirConverter,
            TryGetModel<TModel> modelCollectionLocator,
            out string id,
            out string? canonical,
            out FhirArtifactClassEnum artifactClass)
            where TModel : class
        {
            id = Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(path));

            if (!File.Exists(path))
            {
                canonical = null;
                artifactClass = default;
                return null;
            }

            var json = File.ReadAllText(path);

            if (!fhirConverter.TryParseResource(json, out var resource, out _))
            {
                canonical = null;
                artifactClass = default;
                return null;
            }

            if (resource is FhirExpando expando)
            {
                if (expando.TryGetValue("type", out var typeVal) && typeVal is string typeName)
                {
                    id = typeName;
                }
                else if(expando.TryGetValue("url", out var urlVal) && urlVal is string url)
                {
                    id = url;
                }
                else if (expando.TryGetValue("id", out var idVal) && urlVal is string id2)
                {
                    id = id2;
                }
            }

            fhirInfo.ProcessResource(resource, out canonical, out artifactClass);

            modelCollectionLocator(fhirInfo, id, out var model);
            return model;
        }
    }
}
