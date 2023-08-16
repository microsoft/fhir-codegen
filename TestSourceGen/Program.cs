// See https://aka.ms/new-console-template for more information

using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.Health.Fhir.SourceGenerator;
using Microsoft.Health.Fhir.SpecManager.Converters;
using Microsoft.Health.Fhir.SpecManager.Language;
using Microsoft.Health.Fhir.SpecManager.Manager;

var generator = new FhirSourceGenerator();

var fhirConverter = ConverterHelper.ConverterForVersion(FhirPackageCommon.FhirSequenceEnum.R4B);

var fhirInfo = new FhirVersionInfo(FhirPackageCommon.FhirSequenceEnum.R4B);

var complex = generator.ProcessFile("Patient.StructureDefinition.json", fhirInfo, fhirConverter, m => m.Resources, out var fileName, out var canonical, out var artifactClass);

ILanguage language = LanguageHelper.GetLanguages("CSharpFirely2")[0];
using var memoryStream = new MemoryStream(short.MaxValue);
language.Export(fhirInfo, complex, memoryStream);

memoryStream.Seek(0, SeekOrigin.Begin);
StreamReader reader = new StreamReader(memoryStream, Encoding.UTF8);
var code = await reader.ReadToEndAsync();

Console.WriteLine(code);
