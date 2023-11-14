﻿// <copyright file="Info.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System.IO;
using System.Text.Json;
using Microsoft.Health.Fhir.CodeGenCommon.Extensions;
using Microsoft.Health.Fhir.SpecManager.Manager;

namespace Microsoft.Health.Fhir.SpecManager.Language
{
    /// <summary>Export to an Information format - used to check parsing and dump FHIR version info.</summary>
    public sealed class Info : ILanguage
    {
        /// <summary>FHIR information we are exporting.</summary>
        private FhirVersionInfo _info;

        /// <summary>Options for controlling the export.</summary>
        private ExporterOptions _options;

        /// <summary>The currently in-use text writer.</summary>
        private ExportStreamWriter _writer;

        /// <summary>Name of the language.</summary>
        private const string LanguageName = "Info";

        /// <summary>The single file export extension.</summary>
        private const string SingleFileExportExtension = ".txt";

        /// <summary>Dictionary mapping FHIR primitive types to language equivalents.</summary>
        private static readonly Dictionary<string, string> _primitiveTypeMap = new()
        {
            { "base", "base" },
            { "base64Binary", "base64Binary" },
            { "boolean", "boolean" },
            { "canonical", "canonical" },
            { "code", "code" },
            { "date", "date" },
            { "dateTime", "dateTime" },
            { "decimal", "decimal" },
            { "id", "id" },
            { "instant", "instant" },
            { "integer", "integer" },
            { "integer64", "integer64" },
            { "markdown", "markdown" },
            { "oid", "oid" },
            { "positiveInt", "positiveInt" },
            { "string", "string" },
            { "time", "time" },
            { "unsignedInt", "unsignedInt" },
            { "uri", "uri" },
            { "url", "url" },
            { "uuid", "uuid" },
            { "xhtml", "xhtml" },
        };

        /// <summary>Gets the reserved words.</summary>
        /// <value>The reserved words.</value>
        private static readonly HashSet<string> _reservedWords = new();

        /// <summary>Gets the name of the language.</summary>
        /// <value>The name of the language.</value>
        string ILanguage.LanguageName => LanguageName;

        string ILanguage.Namespace
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the single file extension for this language - null or empty indicates a multi-file
        /// export (exporter should copy the contents of the directory).
        /// </summary>
        string ILanguage.SingleFileExportExtension => SingleFileExportExtension;

        /// <summary>Gets the FHIR primitive type map.</summary>
        /// <value>The FHIR primitive type map.</value>
        Dictionary<string, string> ILanguage.FhirPrimitiveTypeMap => _primitiveTypeMap;

        /// <summary>Gets the reserved words.</summary>
        /// <value>The reserved words.</value>
        HashSet<string> ILanguage.ReservedWords => _reservedWords;

        /// <summary>
        /// Gets a list of FHIR class types that the language WILL export, regardless of user choices.
        /// Used to provide information to users.
        /// </summary>
        List<ExporterOptions.FhirExportClassType> ILanguage.RequiredExportClassTypes => new()
        {
            ExporterOptions.FhirExportClassType.PrimitiveType,
            ExporterOptions.FhirExportClassType.ComplexType,
            ExporterOptions.FhirExportClassType.Resource,
            ExporterOptions.FhirExportClassType.Interaction,
            ExporterOptions.FhirExportClassType.Enum,
            ExporterOptions.FhirExportClassType.Profile,
        };

        /// <summary>
        /// Gets a list of FHIR class types that the language CAN export, depending on user choices.
        /// </summary>
        List<ExporterOptions.FhirExportClassType> ILanguage.OptionalExportClassTypes => new();

        /// <summary>Gets language-specific options and their descriptions.</summary>
        Dictionary<string, string> ILanguage.LanguageOptions => new();


        void ILanguage.Export(
            FhirVersionInfo info,
            FhirComplex complex,
            Stream outputStream)
            => throw new NotImplementedException();

        /// <summary>Export the passed FHIR version into the specified directory.</summary>
        /// <param name="info">           The information.</param>
        /// <param name="serverInfo">     Information describing the server.</param>
        /// <param name="options">        Options for controlling the operation.</param>
        /// <param name="exportDirectory">Directory to write files.</param>
        void ILanguage.Export(
            FhirVersionInfo info,
            FhirCapabiltyStatement serverInfo,
            ExporterOptions options,
            string exportDirectory)
        {
            // set internal vars so we don't pass them to every function
            // this is ugly, but the interface patterns get bad quickly because we need the type map to copy the FHIR info
            _info = info;
            _options = options;

            // create a filename for writing (single file for now)
            string filename = Path.Combine(exportDirectory, $"Info_{info.FhirSequence}.txt");

            using (FileStream stream = new(filename, FileMode.Create))
            using (ExportStreamWriter writer = new(stream))
            {
                _writer = writer;

                WriteHeader();

                WritePrimitiveTypes(_info.PrimitiveTypes.Values);
                WriteComplexes(_info.ComplexTypes.Values, "Complex Types");
                WriteComplexes(_info.Resources.Values, "Resources");

                WriteOperations(_info.SystemOperations.Values, true, "System Operations");
                WriteSearchParameters(_info.AllResourceParameters.Values, "All Resource Parameters");
                WriteSearchParameters(_info.SearchResultParameters.Values, "Search Result Parameters");
                WriteSearchParameters(_info.AllInteractionParameters.Values, "All Interaction Parameters");

                WriteValueSets(_info.ValueSetsByUrl.Values, "Value Sets");

                WriteFooter();
            }
        }

        /// <summary>Builds standard snippet.</summary>
        /// <param name="standardStatus">The standard status.</param>
        /// <param name="fmmLevel">      The fmm level.</param>
        /// <returns>A string.</returns>
        private static string BuildStandardSnippet(string standardStatus, int? fmmLevel, bool? isExperimental)
        {
            string val = standardStatus;

            if (fmmLevel != null)
            {
                if (string.IsNullOrEmpty(val))
                {
                    val = "FMM: " + fmmLevel.ToString();
                }
                else
                {
                    val = val + " FMM: " + fmmLevel.ToString();
                }
            }

            if (isExperimental == true)
            {
                if (string.IsNullOrEmpty(val))
                {
                    val = "experimental";
                }
                else
                {
                    val += " experimental";
                }
            }

            if (string.IsNullOrEmpty(val))
            {
                return string.Empty;
            }

            return " (" + val + ")";
        }

        /// <summary>Writes a value sets.</summary>
        /// <param name="valueSets"> Sets the value belongs to.</param>
        /// <param name="headerHint">(Optional) The header hint.</param>
        private void WriteValueSets(
            IEnumerable<FhirValueSetCollection> valueSets,
            string headerHint = null)
        {
            if (!string.IsNullOrEmpty(headerHint))
            {
                _writer.WriteLineIndented($"{headerHint}: {valueSets.Count()} (unversioned)");
            }

            foreach (FhirValueSetCollection collection in valueSets.OrderBy(c => c.URL))
            {
                foreach (FhirValueSet vs in collection.ValueSetsByVersion.Values.OrderBy(v => v.Version))
                {
                    string snip = BuildStandardSnippet(vs.StandardStatus, vs.FhirMaturityLevel, null);

                    _writer.WriteLineIndented($"- ValueSet: {vs.URL}|{vs.Version}{snip} ({vs.Name})");

                    _writer.IncreaseIndent();

                    if (vs.StrongestBindingByType?.Any() ?? false)
                    {
                        string vsReferences =
                            $"references ({vs.ReferencingElementsByPath.Count}): " + string.Join(", ", vs.ReferencingElementsByPath.Keys) +
                            ", strongest binding: " + vs.StrongestBinding.ToString() +
                            ", by type: " + string.Join(", ", vs.StrongestBindingByType.Select(bt => $"{bt.Key}:{bt.Value}"));

                        _writer.WriteLineIndented(vsReferences);
                    }

                    if (vs.StrongestExternalBindingByType?.Any() ?? false)
                    {
                        string vsReferences =
                            $"extensions/profiles ({vs.ReferencingExternalElementsByUrl.Count}):" +
                            " strongest binding: " + vs.StrongestExternalBinding.ToString() +
                            ", refs: " + string.Join(", ", vs.ReferencingExternalElementsByUrl.Select(bt => $"{bt.Value.RootArtifact.ArtifactClass}:{bt.Value.RootArtifact.Id}"));

                        _writer.WriteLineIndented(vsReferences);
                    }

                    if (vs.Expansion != null)
                    {
                        if (vs.Expansion.IsLimitedExpansion)
                        {
                            _writer.WriteLineIndented($"! Partial expansion, not displayed");
                        }
                        else
                        {
                            foreach (FhirConcept value in vs.Expansion.Contains.OrderBy(c => c.Code))
                            {
                                _writer.WriteLineIndented($"- #{value.Code}: {value.Display}");
                            }
                        }
                    }
                    else
                    {
                        foreach (FhirConcept value in vs.Concepts.OrderBy(c => c.Code))
                        {
                            _writer.WriteLineIndented($"- #{value.Code}: {value.Display}");
                        }
                    }

                    _writer.DecreaseIndent();
                }
            }
        }

        /// <summary>Writes the complexes.</summary>
        /// <param name="complexes"> The complexes.</param>
        /// <param name="headerHint">(Optional) The header hint.</param>
        private void WriteComplexes(
            IEnumerable<FhirComplex> complexes,
            string headerHint = null)
        {
            if (!string.IsNullOrEmpty(headerHint))
            {
                _writer.WriteLineIndented($"{headerHint}: {complexes.Count()}");
            }

            foreach (FhirComplex complex in complexes.OrderBy(c => c.Id))
            {
                WriteComplex(complex);
            }
        }

        /// <summary>Writes a primitive types.</summary>
        /// <param name="primitives">The primitives.</param>
        private void WritePrimitiveTypes(
            IEnumerable<FhirPrimitive> primitives)
        {
            _writer.WriteLineIndented($"Primitive Types: {primitives.Count()}");

            foreach (FhirPrimitive primitive in primitives.OrderBy(p => p.Id))
            {
                WritePrimitiveType(primitive);
            }
        }

        /// <summary>Writes a primitive type.</summary>
        /// <param name="primitive">The primitive.</param>
        private void WritePrimitiveType(
            FhirPrimitive primitive)
        {
            string snip = BuildStandardSnippet(primitive.StandardStatus, primitive.FhirMaturityLevel, primitive.IsExperimental);

            _writer.WriteLineIndented(
                $"- {primitive.Name}:" +
                    $" {primitive.NameForExport(FhirTypeBase.NamingConvention.CamelCase)}" +
                    $"::{primitive.TypeForExport(FhirTypeBase.NamingConvention.CamelCase, _primitiveTypeMap)}" +
                    $"{snip}");

            _writer.IncreaseIndent();

            // check for regex
            if (!string.IsNullOrEmpty(primitive.ValidationRegEx))
            {
                _writer.WriteLineIndented($"[{primitive.ValidationRegEx}]");
            }

            if (_info.ExtensionsByPath.ContainsKey(primitive.Path))
            {
                WriteExtensions(_info.ExtensionsByPath[primitive.Path].Values);
            }

            if (_info.ProfilesByBaseType.ContainsKey(primitive.Path))
            {
                WriteProfiles(_info.ProfilesByBaseType[primitive.Path].Values);
            }

            _writer.DecreaseIndent();
        }

        /// <summary>Writes the profiles.</summary>
        /// <param name="profiles">The profiles.</param>
        private void WriteProfiles(
            IEnumerable<FhirComplex> profiles)
        {
            _writer.WriteLineIndented($"Profiles: {profiles.Count()}");

            foreach (FhirComplex profile in profiles.OrderBy(e => e.Id))
            {
                _writer.WriteLineIndented($"- {profile.Name} ({profile.Id})");
                _writer.IncreaseIndent();
                WriteComplex(profile);
                _writer.DecreaseIndent();
            }
        }

        /// <summary>Writes the extensions.</summary>
        /// <param name="extensions">The extensions.</param>
        private void WriteExtensions(
            IEnumerable<FhirComplex> extensions)
        {
            _writer.WriteLineIndented($"Extensions: {extensions.Count()}");

            foreach (FhirComplex extension in extensions.OrderBy(e => e.Id))
            {
                WriteExtension(extension);
            }
        }

        /// <summary>Writes an extension.</summary>
        /// <param name="extension">The extension.</param>
        private void WriteExtension(
            FhirComplex extension)
        {
            _writer.WriteLineIndented($"+{extension.URL}");

            if (extension.Elements.Count > 0)
            {
                WriteComplex(extension);
            }
        }

        /// <summary>Writes a complex.</summary>
        /// <param name="complex">The complex.</param>
        private void WriteComplex(
            FhirComplex complex)
        {
            bool indented = false;

            // write this type's line, if it's a root element
            // (sub-properties are written with cardinality in the prior loop)
            if (_writer.Indentation == 0)
            {
                string snip = BuildStandardSnippet(complex.StandardStatus, complex.FhirMaturityLevel, complex.IsExperimental);

                _writer.WriteLine($"- {complex.Name}: {complex.BaseTypeName}{snip}");

                if (complex.RootElement != null)
                {
                    WriteElement(complex, complex.RootElement, true);
                }

                _writer.IncreaseIndent();
                indented = true;

                if (complex.Constraints?.Any() ?? false == true)
                {
                    WriteConstraints(complex.ConstraintsByKey.OrderBy(kvp => kvp.Key).Select(kvp => kvp.Value));
                }
            }
            else if (complex.RootElement != null)
            {
                WriteElement(complex, complex.RootElement, true);
            }

            // write elements
            WriteElements(complex);

            // check for extensions
            if (_info.ExtensionsByPath.ContainsKey(complex.Path))
            {
                WriteExtensions(_info.ExtensionsByPath[complex.Path].Values);
            }

            // check for search parameters on this object
            if (complex.SearchParameters != null)
            {
                WriteSearchParameters(complex.SearchParameters.Values);
            }

            // check for type operations
            if (complex.TypeOperations != null)
            {
                WriteOperations(complex.TypeOperations.Values, true);
            }

            // check for instance operations
            if (complex.InstanceOperations != null)
            {
                WriteOperations(complex.TypeOperations.Values, false);
            }

            if (_info.ProfilesByBaseType.ContainsKey(complex.Path))
            {
                WriteProfiles(_info.ProfilesByBaseType[complex.Path].Values);
            }

            if (indented)
            {
                _writer.DecreaseIndent();
            }
        }

        /// <summary>Writes the constraints.</summary>
        /// <param name="constraints">The constraints.</param>
        private void WriteConstraints(IEnumerable<FhirConstraint> constraints)
        {
            foreach (FhirConstraint constraint in constraints)
            {
                string inherited = constraint.IsInherited ? "inherited" : "local";
                _writer.WriteLineIndented($"!{constraint.Key}: {inherited} {constraint.Severity}: {constraint.Expression}");
            }
        }

        /// <summary>Writes the operations.</summary>
        /// <param name="operations"> The operations.</param>
        /// <param name="isTypeLevel">True if is type level, false if not.</param>
        /// <param name="headerHint"> (Optional) The header hint.</param>
        private void WriteOperations(
            IEnumerable<FhirOperation> operations,
            bool isTypeLevel,
            string headerHint = null)
        {
            bool indented = false;

            if (!string.IsNullOrEmpty(headerHint))
            {
                _writer.WriteLineIndented($"{headerHint}: {operations.Count()}");
                _writer.IncreaseIndent();
                indented = true;
            }

            foreach (FhirOperation operation in operations.OrderBy(o => o.Code))
            {
                string snip = BuildStandardSnippet(operation.StandardStatus, operation.FhirMaturityLevel, operation.IsExperimental);

                if (isTypeLevel)
                {
                    _writer.WriteLineIndented($"${operation.Code}{snip}");
                }
                else
                {
                    _writer.WriteLineIndented($"/{{id}}/${operation.Code}{snip}");
                }

                if (operation.Parameters != null)
                {
                    _writer.IncreaseIndent();

                    // write operation parameters inline
                    foreach (FhirParameter parameter in operation.Parameters.OrderBy(p => p.FieldOrder))
                    {
                        string st = string.IsNullOrEmpty(parameter.SearchType) ? string.Empty : "<" + parameter.SearchType + ">";
                        _writer.WriteLineIndented(
                            $"{parameter.Use}:" +
                            $" {parameter.Name}:" +
                            $" {parameter.ValueType}{st}[{parameter.FhirCardinality}]");
                    }

                    _writer.DecreaseIndent();
                }
            }

            if (indented)
            {
                _writer.DecreaseIndent();
            }
        }

        /// <summary>Writes search parameters.</summary>
        /// <param name="searchParameters">Options for controlling the search.</param>
        /// <param name="headerHint">      (Optional) The header hint.</param>
        private void WriteSearchParameters(
            IEnumerable<FhirSearchParam> searchParameters,
            string headerHint = null)
        {
            bool indented = false;

            if (!string.IsNullOrEmpty(headerHint))
            {
                _writer.WriteLineIndented($"{headerHint}: {searchParameters.Count()}");
                _writer.IncreaseIndent();
                indented = true;
            }

            foreach (FhirSearchParam searchParam in searchParameters.OrderBy(s => s.Code))
            {
                if (searchParam.Components?.Any() ?? false)
                {
                    _writer.WriteLineIndented($"?{searchParam.Name}: {searchParam.Code} is composite (resolves: {searchParam.CompositeResolvesCorrectly})");

                    _writer.IncreaseIndent();

                    foreach (FhirSearchParamComponent c in searchParam.Components)
                    {
                        _writer.WriteLineIndented($"$({c.Definition}):{c.DefinitionParam?.ValueType ?? "unresolved"}");
                    }

                    _writer.DecreaseIndent();
                }
                else
                {
                    string snip = BuildStandardSnippet(searchParam.StandardStatus, searchParam.FhirMaturityLevel, searchParam.IsExperimental);
                    _writer.WriteLineIndented($"?{searchParam.Name}: {searchParam.Code}={searchParam.ValueType}{snip}");
                }
            }

            if (indented)
            {
                _writer.DecreaseIndent();
            }
        }

        /// <summary>Writes the elements.</summary>
        /// <param name="complex">    The complex.</param>
        private void WriteElements(
            FhirComplex complex)
        {
            foreach (FhirElement element in complex.Elements.Values.OrderBy(s => s.FieldOrder))
            {
                WriteElement(complex, element);
            }
        }

        /// <summary>Writes an element.</summary>
        /// <param name="complex">      The complex.</param>
        /// <param name="element">      The element.</param>
        /// <param name="writeAsRootElementInfo">(Optional) True if is root element, false if not.</param>
        private void WriteElement(
            FhirComplex complex,
            FhirElement element,
            bool writeAsRootElementInfo = false)
        {
            string propertyType = string.Empty;

            if (element.ElementTypes != null)
            {
                foreach (FhirElementType elementType in element.ElementTypes.Values.OrderBy(e => e.Name))
                {
                    string joiner = string.IsNullOrEmpty(propertyType) ? string.Empty : "|";

                    string profiles = string.Empty;
                    if ((elementType.Profiles != null) && (elementType.Profiles.Count > 0))
                    {
                        profiles = "(" + string.Join("|", elementType.Profiles.Values) + ")";
                    }

                    string targets = string.Empty;
                    if ((elementType.TypeProfiles != null) && (elementType.TypeProfiles.Count > 0))
                    {
                        targets = "(" + string.Join("|", elementType.TypeProfiles.Values) + ")";
                    }

                    propertyType = $"{propertyType}{joiner}{elementType.Name}{profiles}{targets}";
                }
            }

            if (string.IsNullOrEmpty(propertyType))
            {
                propertyType = element.BaseTypeName;
            }

            if (element.IsSimple)
            {
                propertyType += " *simple*";
            }

            if (element.ConstraintsByKey?.Where(kvp => !kvp.Value.IsInherited).Any() ?? false)
            {
                propertyType = propertyType +
                    " constraints: " +
                    string.Join(", ", element.ConstraintsByKey.Values.Where(c => !c.IsInherited).Select(c => c.Key).OrderBy(v => v)) +
                    "";
            }

            if (element.Conditions?.Any() ?? false)
            {
                propertyType = propertyType +
                    " conditions: " +
                    string.Join(", ", element.Conditions.OrderBy(v => v)) +
                    "";
            }

            if (!string.IsNullOrEmpty(element.BindingName))
            {
                propertyType = propertyType + " binding name: " + element.BindingName;
            }

            string fiveWs = string.Empty;

            if (!string.IsNullOrEmpty(element.FiveWs))
            {
                fiveWs = " (W5: " + element.FiveWs + ")";
            }

            if (writeAsRootElementInfo)
            {
                _writer.WriteLineIndented(
                    $"  ^{element.NameForExport(FhirTypeBase.NamingConvention.CamelCase)}[{element.FhirCardinality}]:" +
                    $" {propertyType}" +
                    $"{fiveWs}");
            }
            else
            {
                _writer.WriteLineIndented(
                    $"-" +
                    $" {element.NameForExport(FhirTypeBase.NamingConvention.CamelCase)}[{element.FhirCardinality}]:" +
                    $" {propertyType}" +
                    $"{fiveWs}");
            }

            _writer.IncreaseIndent();

            // check for regex
            if (!string.IsNullOrEmpty(element.ValidationRegEx))
            {
                _writer.WriteLineIndented($"[{element.ValidationRegEx}]");
            }

            // check for default value
            if (!string.IsNullOrEmpty(element.DefaultFieldName))
            {
                _writer.WriteLineIndented($".{element.DefaultFieldName} = {JsonSerializer.Serialize(element.DefaultFieldValue)}");
            }

            // check for fixed value
            if (!string.IsNullOrEmpty(element.FixedFieldName))
            {
                _writer.WriteLineIndented($".{element.FixedFieldName} = {JsonSerializer.Serialize(element.FixedFieldValue)}");
            }

            // check for pattern value
            if (!string.IsNullOrEmpty(element.PatternFieldName))
            {
                _writer.WriteLineIndented($".{element.PatternFieldName} = {JsonSerializer.Serialize(element.PatternFieldValue)}");
            }

            if ((element.Codes != null) && (element.Codes.Count > 0))
            {
                string codes = string.Join("|", element.Codes);
                _writer.WriteLineIndented($"{{{codes}}}");
            }

            if (!writeAsRootElementInfo)
            {
                // either step into backbone definition OR extensions, don't write both
                if (complex.Components.ContainsKey(element.Path))
                {
                    WriteComplex(complex.Components[element.Path]);
                }
                else if (_info.ExtensionsByPath.ContainsKey(element.Path))
                {
                    WriteExtensions(_info.ExtensionsByPath[element.Path].Values);
                }

                // check for slicing information
                if (element.Slicing != null)
                {
                    WriteSlicings(element.Slicing.Values);
                }
            }

            _writer.DecreaseIndent();
        }

        /// <summary>Writes the slicings.</summary>
        /// <param name="slicings">The slicings.</param>
        private void WriteSlicings(
            IEnumerable<FhirSlicing> slicings)
        {
            foreach (FhirSlicing slicing in slicings.OrderBy(s => s.FieldOrder))
            {
                if (slicing.Slices.Count == 0)
                {
                    continue;
                }

                WriteSlicing(slicing);
            }
        }

        /// <summary>Writes a slicing.</summary>
        /// <param name="slicing">The slicing.</param>
        private void WriteSlicing(
            FhirSlicing slicing)
        {
            string rules = string.Empty;

            foreach (FhirSliceDiscriminatorRule rule in slicing.DiscriminatorRules.Values)
            {
                if (!string.IsNullOrEmpty(rules))
                {
                    rules += ", ";
                }

                rules += $"{rule.DiscriminatorTypeName}@{rule.Path}";
            }

            _writer.WriteLineIndented($": {slicing.DefinedByUrl} - {slicing.SlicingRules} ({rules})");

            _writer.IncreaseIndent();

            // write slices inline
            int sliceNumber = 0;
            foreach (FhirComplex slice in slicing.Slices)
            {
                _writer.WriteLineIndented($": Slice {sliceNumber++}:{slice.SliceName} - on {slice.Name}");

                _writer.IncreaseIndent();

                // recurse into this slice
                WriteComplex(slice);

                _writer.DecreaseIndent();
            }

            _writer.DecreaseIndent();
        }

        /// <summary>Writes a header.</summary>
        private void WriteHeader()
        {
            _writer.WriteLine($"Contents of: {_info.PackageName} version: {_info.VersionString}");
            _writer.WriteLine($"  Primitive Naming Style: {FhirTypeBase.NamingConvention.CamelCase}");
            _writer.WriteLine($"  Element Naming Style: {FhirTypeBase.NamingConvention.CamelCase}");
            _writer.WriteLine($"  Complex Type / Resource Naming Style: {FhirTypeBase.NamingConvention.PascalCase}");
            _writer.WriteLine($"  Enum Naming Style: {FhirTypeBase.NamingConvention.FhirDotNotation}");
            _writer.WriteLine($"  Interaction Naming Style: {FhirTypeBase.NamingConvention.PascalCase}");
            _writer.WriteLine($"  Extension Support: {_options.ExtensionSupport}");

            if ((_options.ExportList != null) && _options.ExportList.Any())
            {
                string restrictions = string.Join("|", _options.ExportList);
                _writer.WriteLine($"  Restricted to: {restrictions}");
            }

            if ((_options.LanguageOptions != null) && (_options.LanguageOptions.Count > 0))
            {
                foreach (KeyValuePair<string, string> kvp in _options.LanguageOptions)
                {
                    _writer.WriteLine($"  Language option: \"{kvp.Key}\" = \"{kvp.Value}\"");
                }
            }
        }

        /// <summary>Writes a footer.</summary>
        private static void WriteFooter()
        {
            return;
        }
    }
}
