// <copyright file="ModelBuilder.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using Microsoft.Health.Fhir.SpecManager.Manager;
using static Microsoft.Health.Fhir.SpecManager.Language.TypeScriptSdk.TypeScriptSdkCommon;

namespace Microsoft.Health.Fhir.SpecManager.Language.TypeScriptSdk;

/// <summary>The TypeScript-SDK model builder - converts internal models into TS-specific structures.</summary>
public class ModelBuilder
{
    /// <summary>An export element choice type.</summary>
    public readonly record struct ExportElementChoiceType(
        string ExportName,
        string ExportType,
        string ExportInterfaceType,
        string ExportJsonType,
        bool IsPrimitive,
        bool HasValueSetEnum);

    /// <summary>An export element.</summary>
    public readonly record struct ExportElement(
        string FhirName,
        string FhirPath,
        string FhirType,
        string ExportName,
        string ExportComment,
        string ExportType,
        string ExportInterfaceType,
        string ExportJsonType,
        string ValidationRegEx,
        bool IsOptional,
        bool IsArray,
        bool IsPrimitive,
        bool IsJsonArtefact,
        bool HasReferencedValueSet,
        string ValueSetExportName,
        FhirElement.ElementDefinitionBindingStrength? BoundValueSetStrength,
        bool IsChoice,
        List<ExportElementChoiceType> ChoiceTypes);

    /// <summary>An export complex.</summary>
    public readonly record struct ExportComplex(
        string FhirName,
        string FhirPath,
        string ExportInterfaceName,
        string ExportClassName,
        string ExportType,
        string ExportInterfaceType,
        string ExportComment,
        FhirArtifactClassEnum ArtifactClass,
        List<ExportComplex> Backbones,
        List<ExportElement> Elements,
        List<string> ReferencedValueSetExportNames);

    /// <summary>An export primitive.</summary>
    public readonly record struct ExportPrimitive(
        string FhirName,
        string ExportClassName,
        string ExportClassType,
        string ExportInterfaceName,
        string ExportInterfaceType,
        string ExportComment,
        string JsonExportType,
        string ValidationRegEx);

    /// <summary>An export value set coding.</summary>
    public readonly record struct ExportValueSetCoding(
        string FhirName,
        string ExportName,
        string Code,
        string System,
        string Display,
        string Comment);

    /// <summary>An export value set.</summary>
    public readonly record struct ExportValueSet(
        string FhirName,
        string FhirUrl,
        string FhirVersion,
        string ExportName,
        string ExportComment,
        Dictionary<string, ExportValueSetCoding> CodingsByExportName);

    /// <summary>Information about the complex token.</summary>
    public readonly record struct ExportTokenInfo(
        string Token,
        bool requiresTypeLiteral);

    /// <summary>A sorted export key.</summary>
    public readonly record struct SortedExportKey(
        string ExportName,
        List<ExportTokenInfo> Tokens);

    /// <summary>The processed set of export models.</summary>
    public readonly record struct ExportModels(
        Dictionary<string, ExportPrimitive> PrimitiveTypesByExportName,
        Dictionary<string, ExportComplex> ComplexDataTypesByExportName,
        Dictionary<string, ExportComplex> ResourcesByExportName,
        Dictionary<string, ExportValueSet> ValueSetsByExportName,
        List<SortedExportKey> SortedPrimitives,
        List<SortedExportKey> SortedDataTypes,
        List<SortedExportKey> SortedResources);

    /// <summary>The information.</summary>
    private FhirVersionInfo _info;

    /// <summary>
    /// Initializes a new instance of the <see cref="ModelBuilder"/> class.
    /// </summary>
    /// <param name="info">The information.</param>
    public ModelBuilder(FhirVersionInfo info)
    {
        _info = info;
    }

    /// <summary>Builds the given information.</summary>
    /// <param name="supportPrimitves">(Optional) The support primitves.</param>
    /// <returns>The ExportModels.</returns>
    public ExportModels Build()
    {
        Dictionary<string, ExportPrimitive> primitiveTypes = new();
        List<SortedExportKey> sortedPrimitives;
        Dictionary<string, ExportComplex> complexDataTypes = new();
        List<SortedExportKey> sortedDataTypes;
        Dictionary<string, ExportComplex> resources = new();
        List<SortedExportKey> sortedResources;
        Dictionary<string, ExportValueSet> valueSets = new();

        foreach (FhirPrimitive fhirPrimitive in _info.PrimitiveTypes.Values)
        {
            string exportName = "Fhir" + fhirPrimitive.NameForExport(FhirTypeBase.NamingConvention.PascalCase, false, string.Empty, ReservedWords);

            string exportClassType;
            string exportInterfaceType;

            switch (exportName)
            {
                case "FhirBase":
                    exportClassType = string.Empty;
                    exportInterfaceType = string.Empty;
                    break;

                case "FhirPrimitive":
                    exportClassType = "fhir.FhirBase";
                    exportInterfaceType = "fhir.IFhirBase";
                    break;

                default:

                    if (fhirPrimitive.Id == fhirPrimitive.BaseTypeName)
                    {
                        exportClassType = "fhir.FhirPrimitive";
                        exportInterfaceType = "fhir.IFhirPrimitive";
                    }
                    else if (_info.PrimitiveTypes.ContainsKey(fhirPrimitive.BaseTypeName))
                    {
                        string baseName = FhirUtils.ToConvention(
                            fhirPrimitive.BaseTypeName,
                            string.Empty,
                            FhirTypeBase.NamingConvention.PascalCase,
                            false,
                            string.Empty,
                            ReservedWords);

                        exportClassType = "fhir.Fhir" + baseName;
                        exportInterfaceType = "fhir.IFhir" + baseName;
                    }
                    else
                    {
                        exportClassType = "fhir.FhirPrimitive";
                        exportInterfaceType = "fhir.IFhirPrimitive";
                    }

                    break;
            }

            primitiveTypes.Add(
                exportName,
                new ExportPrimitive(
                    fhirPrimitive.Name,
                    exportName,
                    exportClassType,
                    "I" + exportName,
                    exportInterfaceType,
                    fhirPrimitive.Comment,
                    PrimitiveTypeMap.ContainsKey(fhirPrimitive.Name) ? PrimitiveTypeMap[fhirPrimitive.Name] : exportName,
                    fhirPrimitive.ValidationRegEx));
        }

        sortedPrimitives = SortPrimitives(primitiveTypes);

        foreach (FhirComplex fhirComplex in _info.ComplexTypes.Values)
        {
            ExportComplex exportComplex = ProcessFhirComplex(fhirComplex, FhirArtifactClassEnum.ComplexType);

            complexDataTypes.Add(exportComplex.ExportClassName, exportComplex);
        }

        sortedDataTypes = SortComplexes(complexDataTypes);

        foreach (FhirComplex fhirComplex in _info.Resources.Values)
        {
            ExportComplex exportComplex = ProcessFhirComplex(fhirComplex, FhirArtifactClassEnum.Resource);

            resources.Add(exportComplex.ExportClassName, exportComplex);
        }

        sortedResources = SortComplexes(resources);

        foreach (FhirValueSetCollection collection in _info.ValueSetsByUrl.Values)
        {
            foreach (FhirValueSet fhirValueSet in collection.ValueSetsByVersion.Values)
            {
                ExportValueSet exportValueSet = ProcessFhirValueSet(fhirValueSet);

                valueSets.Add(exportValueSet.ExportName, exportValueSet);
            }
        }

        return new ExportModels(primitiveTypes, complexDataTypes, resources, valueSets, sortedPrimitives, sortedDataTypes, sortedResources);
    }

    /// <summary>Sort primitives.</summary>
    /// <param name="primitives">The primitives.</param>
    /// <returns>The sorted primitives.</returns>
    private List<SortedExportKey> SortPrimitives(Dictionary<string, ExportPrimitive> primitives)
    {
        List<SortedExportKey> sorted = new();
        HashSet<string> usedKeys = new();

        sorted.Add(new SortedExportKey("FhirBase", TokensFromPrimitive("FhirBase")));
        usedKeys.Add("FhirBase");

        sorted.Add(new SortedExportKey("FhirPrimitive", TokensFromPrimitive("FhirPrimitive")));
        usedKeys.Add("FhirPrimitive");

        foreach (ExportPrimitive primitive in primitives.Values)
        {
            if (usedKeys.Contains(primitive.ExportClassName))
            {
                continue;
            }

            sorted.Add(new SortedExportKey(primitive.ExportClassName, TokensFromPrimitive(primitive.ExportClassName)));
            usedKeys.Add(primitive.ExportClassName);
        }

        return sorted;
    }

    /// <summary>Tokens from primitive.</summary>
    /// <param name="primitiveName">The primitive name.</param>
    /// <returns>A List&lt;ExportTokenInfo&gt;</returns>
    private List<ExportTokenInfo> TokensFromPrimitive(string primitiveName)
    {
        List<ExportTokenInfo> tokens = new();

        if (primitiveName.Equals("FhirBase", StringComparison.Ordinal))
        {
            tokens.Add(new ExportTokenInfo("FhirConstructorOptions", true));
            tokens.Add(new ExportTokenInfo("FtsElement", true));
            tokens.Add(new ExportTokenInfo("FtsCoding", true));
            tokens.Add(new ExportTokenInfo("FtsCodeableConcept", true));
            tokens.Add(new ExportTokenInfo("FtsIssue", true));
        }

        //tokens.Add(new ExportTokenInfo("I" + primitiveName, true));
        tokens.Add(new ExportTokenInfo(primitiveName + "Args", true));
        tokens.Add(new ExportTokenInfo(primitiveName, false));

        return tokens;
    }

    /// <summary>Sort complexes.</summary>
    /// <param name="complexes">The complexes.</param>
    /// <returns>The sorted complexes.</returns>
    private List<SortedExportKey> SortComplexes(Dictionary<string, ExportComplex> complexes)
    {
        List<SortedExportKey> sorted = new();
        HashSet<string> usedKeys = new();

        // check for Coding first due to ValueSet mappings
        if (complexes.ContainsKey("Coding"))
        {
            AddComplexToSort(complexes, complexes["Coding"], sorted, usedKeys);
        }

        foreach ((string exportName, ExportComplex complex) in complexes)
        {
            if (usedKeys.Contains(exportName))
            {
                continue;
            }

            AddComplexToSort(complexes, complex, sorted, usedKeys);
        }

        return sorted;
    }

    /// <summary>Adds a complex for export.</summary>
    /// <param name="complexes">  The complexes.</param>
    /// <param name="complex">    The item.</param>
    /// <param name="sortedItems">The sorted items.</param>
    /// <param name="usedKeys">   The used keys.</param>
    private void AddComplexToSort(
        Dictionary<string, ExportComplex> complexes,
        ExportComplex complex,
        List<SortedExportKey> sortedItems,
        HashSet<string> usedKeys)
    {
        string exportType = complex.ExportType.StartsWith("fhir.", StringComparison.Ordinal)
            ? complex.ExportType.Substring(5)
            : complex.ExportType;

        if ((!string.IsNullOrEmpty(exportType)) &&
            (!usedKeys.Contains(exportType)) &&
            complexes.ContainsKey(exportType))
        {
            AddComplexToSort(
                complexes,
                complexes[exportType],
                sortedItems,
                usedKeys);
        }

        usedKeys.Add(complex.ExportClassName);

        List<ExportTokenInfo> tokens = TokensFromComplex(complex);

        sortedItems.Add(new SortedExportKey(complex.ExportClassName, tokens));
    }

    /// <summary>Tokens from complex.</summary>
    /// <param name="complex">The item.</param>
    /// <returns>A List&lt;ExportTokenInfo&gt;</returns>
    private List<ExportTokenInfo> TokensFromComplex(ExportComplex complex)
    {
        List<ExportTokenInfo> tokens = new();

        //tokens.Add(new ExportTokenInfo(complex.ExportInterfaceName, true));
        tokens.Add(new ExportTokenInfo(complex.ExportClassName + "Args", true));
        tokens.Add(new ExportTokenInfo(complex.ExportClassName, false));

        foreach (ExportComplex backbone in complex.Backbones)
        {
            tokens.AddRange(TokensFromComplex(backbone));
        }

        //tokens.AddRange(complex.CodesByExportName.Keys.Select((code) => new ExportTokenInfo(code, false)));

        return tokens;
    }

    /// <summary>Creates export complex from a FHIR complex.</summary>
    /// <param name="fhirComplex">                  The FHIR complex.</param>
    /// <param name="fhirArtifactClass">            The FHIR artifact class.</param>
    /// <param name="backbones">                    The backbones.</param>
    /// <param name="elements">                     The elements.</param>
    /// <param name="referencedValueSetExportNames">[out] List of names of referenced value sets
    ///  exports.</param>
    /// <returns>The new export complex.</returns>
    private ExportComplex ExportForFhirComplex(
        FhirComplex fhirComplex,
        FhirArtifactClassEnum fhirArtifactClass,
        List<ExportComplex> backbones,
        List<ExportElement> elements,
        List<string> referencedValueSetExportNames)
    {
        string exportName;
        string exportType;
        string exportInterfaceType;

        if (string.IsNullOrEmpty(fhirComplex.BaseTypeName) ||
            fhirComplex.Name.Equals("Element", StringComparison.Ordinal))
        {
            exportName = fhirComplex.NameForExport(FhirTypeBase.NamingConvention.PascalCase, false, string.Empty, ReservedWords);
            exportType = "fhir.FhirBase";
            exportInterfaceType = "fhir.IFhirBase";
        }
        else if (fhirComplex.Name.Equals(fhirComplex.BaseTypeName, StringComparison.Ordinal))
        {
            exportName = fhirComplex.NameForExport(FhirTypeBase.NamingConvention.PascalCase, true, string.Empty, ReservedWords);
            exportType = "fhir.FhirBase";
            exportInterfaceType = "fhir.IFhirBase";
        }
        else if ((fhirComplex.Components != null) && fhirComplex.Components.ContainsKey(fhirComplex.Path))
        {
            exportName = fhirComplex.NameForExport(FhirTypeBase.NamingConvention.PascalCase, true, string.Empty, ReservedWords);
            exportType = fhirComplex.TypeForExport(FhirTypeBase.NamingConvention.PascalCase, PrimitiveTypeMap, false, string.Empty, ReservedWords);
            exportInterfaceType = "I" + exportType;
        }
        else
        {
            exportName = fhirComplex.NameForExport(FhirTypeBase.NamingConvention.PascalCase, true, string.Empty, ReservedWords);
            exportType = "fhir." + fhirComplex.TypeForExport(FhirTypeBase.NamingConvention.PascalCase, PrimitiveTypeMap, false, string.Empty, ReservedWords);
            exportInterfaceType = exportType.Insert(5, "I");
        }

        return new ExportComplex(
            fhirComplex.Id,
            fhirComplex.Path,
            "I" + exportName,
            exportName,
            exportType,
            exportInterfaceType,
            fhirComplex.Comment,
            fhirArtifactClass,
            backbones,
            elements,
            referencedValueSetExportNames);
    }

    /// <summary>Element requires code enum.</summary>
    /// <param name="fhirElement">The FHIR element.</param>
    /// <param name="vs">         The vs.</param>
    /// <param name="codeName">   [out] Name of the code.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    private bool ElementRequiresCodeEnum(FhirElement fhirElement, FhirValueSet vs, out string codeName)
    {
        // Use generated enum for codes when required strength
        // EXCLUDE the MIME type value set - those should be bound to strings
        if ((fhirElement.Codes != null) &&
            fhirElement.Codes.Any() &&
            (!string.IsNullOrEmpty(fhirElement.ValueSet)) &&
            (!string.IsNullOrEmpty(fhirElement.BindingStrength)) &&
            string.Equals(fhirElement.BindingStrength, "required", StringComparison.Ordinal) &&
            (fhirElement.ValueSet != "http://www.rfc-editor.org/bcp/bcp13.txt") &&
            (!fhirElement.ValueSet.StartsWith("http://hl7.org/fhir/ValueSet/mimetypes", StringComparison.Ordinal)))
        {
            if (vs != null)
            {
                codeName = GetValueSetExportName(vs) + CodeTypeSuffix;
            }
            else
            {
                codeName = FhirUtils.ToConvention(
                    $"{fhirElement.Path}.{CodeTypeSuffix}",
                    string.Empty,
                    FhirTypeBase.NamingConvention.PascalCase);
            }

            return true;
        }

        codeName = string.Empty;
        return false;
    }

    /// <summary>Expand export type.</summary>
    /// <param name="fhirPath">     Path to the FHIR element.</param>
    /// <param name="fhirType"> Name of the FHIR type.</param>
    /// <param name="exportType">   [out] Type of the export.</param>
    /// <param name="interfaceType">[out] Type of the interface.</param>
    /// <param name="jsonType">     [out] Type of the JSON.</param>
    private void ExpandExportType(
        string fhirPath,
        string fhirType,
        string exportTypeName,
        Dictionary<string, FhirComplex> components,
        out string exportType,
        out string interfaceType,
        out string jsonType)
    {
        if (ComplexTypeSubstitutions.ContainsKey(fhirType))
        {
            exportType = "fhir." + ComplexTypeSubstitutions[fhirType];
            interfaceType = "fhir.I" + ComplexTypeSubstitutions[fhirType];
            jsonType = "fhir." + ComplexTypeSubstitutions[fhirType];

            return;
        }

        if (components.ContainsKey(fhirPath))
        {
            exportType = "fhir." + exportTypeName;
            interfaceType = "fhir.I" + exportTypeName;
            jsonType = "fhir." + exportTypeName;

            return;
        }

        if (PrimitiveTypeMap.ContainsKey(fhirType))
        {
            exportType = "fhir.Fhir" + FhirUtils.ToConvention(
                fhirType, // exportTypeName,
                string.Empty,
                FhirTypeBase.NamingConvention.PascalCase,
                false,
                string.Empty,
                ReservedWords);
            interfaceType = exportType;
            jsonType = PrimitiveTypeMap[fhirType];

            return;
        }

        if (_info.ExcludedKeys.Contains(fhirType))
        {
            exportType = "any";
            interfaceType = exportType;
            jsonType = "any";

            return;
        }

        exportType = "fhir." + exportTypeName;
        interfaceType = "fhir.I" + exportTypeName;
        jsonType = "fhir." + exportTypeName;
    }

    /// <summary>
    /// Creates export elements for a FHIR element.  Choice-types and primitives will expand the
    /// element definition to individual records.
    /// </summary>
    /// <param name="fhirComplex">                 The FHIR complex.</param>
    /// <param name="fhirElement">                 The FHIR element.</param>
    /// <param name="exportElement">               [out] The export element.</param>
    /// <param name="referencedValueSetExportName">[out] Name of the referenced value set export.</param>
    private void ProcessFhirElement(
        FhirComplex fhirComplex,
        FhirElement fhirElement,
        out ExportElement exportElement,
        out string referencedValueSetExportName)
    {
        string shortId = fhirElement.Id.Split('.').Last();
        string fhirElementType = string.Empty;
        string elementExportName;
        string elementExportType;
        string elementInterfaceType;
        string elementJsonType;

        bool hasReferencedValueSet = false;
        referencedValueSetExportName = string.Empty;
        FhirElement.ElementDefinitionBindingStrength? vsBindStrength = null;
        FhirValueSet vs = null;

        List<ExportElementChoiceType> choiceTypes = new();

        List<FhirElement.ExpandedElementRec> values = fhirElement.ExpandNamesAndTypes(
            out elementExportName,
            FhirTypeBase.NamingConvention.CamelCase,
            FhirTypeBase.NamingConvention.PascalCase,
            false,
            string.Empty,
            fhirComplex.Components.ContainsKey(fhirElement.Path));

        if (values.Count == 1)
        {
            fhirElementType = values[0].BaseFhirType;
            elementExportType = values[0].ExportFhirType;

            if ((!string.IsNullOrEmpty(fhirElement.ValueSet)) &&
                 _info.TryGetValueSet(fhirElement.ValueSet, out vs))
            {
                hasReferencedValueSet = true;
                referencedValueSetExportName = GetValueSetExportName(vs);
                vsBindStrength = fhirElement.ValueSetBindingStrength;
            }

            // Use generated enum for codes when required strength
            // EXCLUDE the MIME type value set - those should be bound to strings
            if (ElementRequiresCodeEnum(fhirElement, vs, out string codeName))
            {
                elementExportType = "fhir.FhirCode<" + codeName + ">";
                elementInterfaceType = elementExportType + "|" + codeName;
                elementJsonType = "string";
            }
            else
            {
                ExpandExportType(
                    fhirElement.Path,
                    fhirElementType,
                    elementExportType,
                    fhirComplex.Components,
                    out elementExportType,
                    out elementInterfaceType,
                    out elementJsonType);
            }
        }
        else
        {
            elementExportType = string.Empty;
            elementInterfaceType = string.Empty;
            elementJsonType = string.Empty;

            foreach (FhirElement.ExpandedElementRec rec in values)
            {
                bool hasVs = false;

                ExpandExportType(
                    fhirElement.Path,
                    rec.BaseFhirType,
                    rec.ExportFhirType,
                    fhirComplex.Components,
                    out string exportType,
                    out string exportInterfaceType,
                    out string exportJsonType);

                // Use generated enum for codes when required strength
                // EXCLUDE the MIME type value set - those should be bound to strings
                if ((exportType == "fhir.FhirCode") &&
                    ElementRequiresCodeEnum(fhirElement, vs, out string codeName))
                {
                    elementExportType = "fhir.FhirCode<" + codeName + ">";
                    elementInterfaceType = elementExportType + "|" + codeName;
                    elementJsonType = "string";

                    //exportType = codeName;
                    //exportInterfaceType = codeName;
                    //exportJsonType = codeName;
                }

                ExportElementChoiceType ct = new ExportElementChoiceType(
                    rec.ProperyName,
                    exportType,
                    exportInterfaceType,
                    exportJsonType,
                    RequiresExtension(exportJsonType),
                    hasVs);

                choiceTypes.Add(ct);
            }
        }

        exportElement = new ExportElement(
            shortId,
            fhirElement.Path,
            fhirElementType,
            elementExportName,
            fhirElement.Comment,
            elementExportType,
            elementInterfaceType,
            elementJsonType,
            fhirElement.ValidationRegEx,
            fhirElement.IsOptional,
            fhirElement.IsArray,
            RequiresExtension(elementJsonType),
            false,
            hasReferencedValueSet,
            referencedValueSetExportName,
            vsBindStrength,
            values.Count > 1,
            choiceTypes);
    }

    /// <summary>Requires extension.</summary>
    /// <param name="typeName">Name of the type.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    private static bool RequiresExtension(string typeName)
    {
        if (string.IsNullOrEmpty(typeName))
        {
            return false;
        }

        if (PrimitiveTypeMap.ContainsKey(typeName))
        {
            return true;
        }

        return false;
    }

    /// <summary>Process the backbone described by fhirComplex.</summary>
    /// <param name="fhirComplex">      The FHIR complex.</param>
    /// <param name="fhirArtifactClass">The FHIR artifact class.</param>
    /// <returns>An ExportComplex.</returns>
    private ExportComplex ProcessFhirComplex(
        FhirComplex fhirComplex,
        FhirArtifactClassEnum fhirArtifactClass)
    {
        HashSet<string> vsExportNames = new();
        List<ExportComplex> backbones = new();

        if (fhirComplex.Components != null)
        {
            foreach (FhirComplex component in fhirComplex.Components.Values)
            {
                // use unknown class for backbones to prevent adding DT or Resource specific elements
                ExportComplex backbone = ProcessFhirComplex(component, FhirArtifactClassEnum.Unknown);
                backbones.Add(backbone);

                // promote exported Value Set names so that parents always know about child dependencies
                foreach (string vsName in backbone.ReferencedValueSetExportNames)
                {
                    if (!vsExportNames.Contains(vsName))
                    {
                        vsExportNames.Add(vsName);
                    }
                }
            }
        }

        List<ExportElement> elements = new();

        if (fhirArtifactClass == FhirArtifactClassEnum.Resource)
        {
            if (fhirComplex.Id == "Resource")
            {
                elements.Add(new ExportElement(
                    "resourceType",
                    fhirComplex.Path + ".resourceType",
                    "string",
                    "resourceType",
                    "Resource Type Name",
                    "string",
                    "string",
                    "string",
                    string.Empty,
                    false,
                    false,
                    false,
                    true,
                    false,
                    "ResourceTypesValueSet",
                    FhirElement.ElementDefinitionBindingStrength.Extensible,
                    false,
                    null));

                vsExportNames.Add("ResourceTypesValueSet");
            }
            else if (fhirComplex.IsAbstract == false)
            {
                elements.Add(new ExportElement(
                    "resourceType",
                    fhirComplex.Path + ".resourceType",
                    $"\"{fhirComplex.Id}\"",
                    "resourceType",
                    "Resource Type Name",
                    $"\"{fhirComplex.Id}\"",
                    $"\"{fhirComplex.Id}\"",
                    $"\"{fhirComplex.Id}\"",
                    string.Empty,
                    false,
                    false,
                    false,
                    true,
                    false,
                    string.Empty,
                    null,
                    false,
                    null));
            }
        }

        if (fhirComplex.Elements != null)
        {
            foreach (FhirElement element in fhirComplex.Elements.Values)
            {
                if (element.IsInherited)
                {
                    continue;
                }

                ProcessFhirElement(
                    fhirComplex,
                    element,
                    out ExportElement elementExport,
                    out string referencedValueSetExportName);

                elements.Add(elementExport);

                if (!string.IsNullOrEmpty(referencedValueSetExportName))
                {
                    vsExportNames.ConditionalAdd(referencedValueSetExportName);
                }
            }
        }

        ExportComplex export = ExportForFhirComplex(
            fhirComplex,
            fhirArtifactClass,
            backbones,
            elements,
            vsExportNames.ToList());

        return export;
    }

    /// <summary>Gets value set export name.</summary>
    /// <param name="fhirValueSet">Set the FHIR value belongs to.</param>
    /// <returns>The value set export name.</returns>
    private string GetValueSetExportName(FhirValueSet fhirValueSet)
    {
        string vsName = FhirUtils.SanitizeForProperty(fhirValueSet.Id ?? fhirValueSet.Name, ReservedWords);
        vsName = FhirUtils.SanitizedToConvention(vsName, FhirTypeBase.NamingConvention.PascalCase);

        if (vsName.EndsWith("Codes", StringComparison.OrdinalIgnoreCase))
        {
            vsName = vsName.Substring(0, vsName.Length - 5);
        }

        //return vsName + "ValueSet";
        return vsName;
    }

    /// <summary>Process the FHIR value set described by fhirValueSet.</summary>
    /// <param name="fhirValueSet">Set the FHIR value belongs to.</param>
    /// <returns>An ExportValueSet.</returns>
    private ExportValueSet ProcessFhirValueSet(FhirValueSet fhirValueSet)
    {
        string vsName = GetValueSetExportName(fhirValueSet);

        Dictionary<string, ExportValueSetCoding> codingsByExportName = new();

        bool prefixWithSystem = fhirValueSet.ReferencedCodeSystems.Count > 1;
        HashSet<string> processedKeys = new();
        HashSet<string> usedConceptLiterals = new();

        foreach (FhirConcept concept in fhirValueSet.Concepts.OrderBy(c => c.Code))
        {
            if (processedKeys.Contains(concept.Key()))
            {
                continue;
            }

            processedKeys.Add(concept.Key());

            string fhirCodeName;
            string additionalData;

            if (SystemsNamedByDisplay.Contains(concept.System))
            {
                fhirCodeName = concept.Display;
                additionalData = concept.Code;
            }
            else if (SystemsNamedByCode.Contains(concept.System))
            {
                fhirCodeName = concept.Code;
                additionalData = concept.System;
            }
            else if (string.IsNullOrEmpty(concept.Display))
            {
                fhirCodeName = concept.Code;
                additionalData = concept.System;
            }
            else
            {
                fhirCodeName = concept.Display;
                additionalData = concept.Code;
            }

            string codeName = FhirUtils.SanitizeForProperty(fhirCodeName, ReservedWords);
            string codeValue = FhirUtils.SanitizeForValue(concept.Code);

            codeName = FhirUtils.SanitizedToConvention(codeName, FhirTypeBase.NamingConvention.PascalCase);

            if (usedConceptLiterals.Contains(codeName))
            {
                additionalData = FhirUtils.SanitizeForProperty(additionalData, ReservedWords);

                if (additionalData.StartsWith("VAL", StringComparison.Ordinal))
                {
                    additionalData = additionalData.Substring(3);
                    additionalData = "_" + FhirUtils.SanitizedToConvention(additionalData, FhirTypeBase.NamingConvention.PascalCase);
                }
                else
                {
                    additionalData = FhirUtils.SanitizedToConvention(additionalData, FhirTypeBase.NamingConvention.PascalCase);
                }

                codeName = codeName + additionalData;
            }

            usedConceptLiterals.Add(codeName);

            ExportValueSetCoding exportCoding = new ExportValueSetCoding(
                fhirCodeName,
                codeName,
                codeValue,
                concept.System,
                concept.Display,
                concept.Definition);

            codingsByExportName.Add(codeName, exportCoding);
        }

        ExportValueSet export = new ExportValueSet(
            fhirValueSet.Id ?? fhirValueSet.Name,
            fhirValueSet.URL,
            fhirValueSet.Version,
            vsName,
            fhirValueSet.Description,
            codingsByExportName);

        return export;
    }
}
