// <copyright file="FromR2.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System.IO;
using Microsoft.Health.Fhir.SpecManager.Manager;
using Microsoft.Health.Fhir.SpecManager.Models;
using fhirModels = fhirCsR2.Models;
using fhirSerialization = fhirCsR2.Serialization;

namespace Microsoft.Health.Fhir.SpecManager.Converters;

/// <summary>Convert FHIR R2 into local definitions.</summary>
public sealed class FromR2 : IFhirConverter
{
    private const string ExtensionComment = "There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.";
    private const string ExtensionDefinition = "May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.";
    private const string ExtensionShort = "Additional content defined by implementations";

    /// <summary>The maximum number of path components.</summary>
    private const int MaxPathComponents = 10;

    /// <summary>The named reference links.</summary>
    private static Dictionary<string, string> _namedReferenceLinks;

    /// <summary>The errors.</summary>
    private List<string> _errors;

    /// <summary>The warnings.</summary>
    private List<string> _warnings;

    /// <summary>
    /// Initializes a new instance of the <see cref="FromR2"/> class.
    /// </summary>
    public FromR2()
    {
        _errors = new List<string>();
        _warnings = new List<string>();
        _namedReferenceLinks = new Dictionary<string, string>()
        {
            { "Extension.extension", "Extension.extension" },
            { "Bundle.link", "Bundle.link" },
            { "Composition.section", "Composition.section" },
            { "ConceptMap.dependsOn", "ConceptMap.element.target.dependsOn" },
            { "Conformance.searchParam", "Conformance.rest.resource.searchParam" },
            { "ConsentDirective.identifier", "Contract.identifier" },
            { "ConsentDirective.issued", "Contract.issued" },
            { "ConsentDirective.applies", "Contract.applies" },
            { "ConsentDirective.subject", "Contract.subject" },
            { "ConsentDirective.authority", "Contract.authority" },
            { "ConsentDirective.domain", "Contract.domain" },
            { "ConsentDirective.type", "Contract.type" },
            { "ConsentDirective.subType", "Contract.subType" },
            { "ConsentDirective.action", "Contract.action" },
            { "ConsentDirective.actionReason", "Contract.actionReason" },
            { "ConsentDirective.actor", "Contract.actor" },
            { "ConsentDirective.actor.entity", "Contract.actor.entity" },
            { "ConsentDirective.actor.role", "Contract.actor.role" },
            { "ConsentDirective.valuedItem", "Contract.valuedItem" },
            { "ConsentDirective.valuedItem.entity[x]", "Contract.valuedItem.entity[x]" },
            { "ConsentDirective.valuedItem.identifier", "Contract.valuedItem.identifier" },
            { "ConsentDirective.valuedItem.effectiveTime", "Contract.valuedItem.effectiveTime" },
            { "ConsentDirective.valuedItem.quantity", "Contract.valuedItem.quantity" },
            { "ConsentDirective.valuedItem.unitprice", "Contract.valuedItem.unitPrice" },
            { "ConsentDirective.valuedItem.factor", "Contract.valuedItem.factor" },
            { "ConsentDirective.valuedItem.points", "Contract.valuedItem.points" },
            { "ConsentDirective.valuedItem.net", "Contract.valuedItem.net" },
            { "ConsentDirective.signer", "Contract.signer" },
            { "ConsentDirective.signer.type", "Contract.signer.type" },
            { "ConsentDirective.signer.party", "Contract.signer.party" },
            { "ConsentDirective.signer.signature", "Contract.signer.signature" },
            { "ConsentDirective.term", "Contract.term" },
            { "ConsentDirective.term.identifier", "Contract.term.identifier" },
            { "ConsentDirective.term.issued", "Contract.term.issued" },
            { "ConsentDirective.term.applies", "Contract.term.applies" },
            { "ConsentDirective.term.type", "Contract.term.type" },
            { "ConsentDirective.term.subType", "Contract.term.subType" },
            { "ConsentDirective.term.subject", "Contract.term.subject" },
            { "ConsentDirective.term.action", "Contract.term.action" },
            { "ConsentDirective.term.actionReason", "Contract.term.actionReason" },
            { "ConsentDirective.term.actor", "Contract.term.actor" },
            { "ConsentDirective.term.actor.entity", "Contract.term.actor.entity" },
            { "ConsentDirective.term.actor.role", "Contract.term.actor.role" },
            { "ConsentDirective.term.text", "Contract.term.text" },
            { "ConsentDirective.term.valuedItem", "Contract.term.valuedItem" },
            { "ConsentDirective.term.valuedItem.entity[x]", "Contract.term.valuedItem.entity[x]" },
            { "ConsentDirective.term.valuedItem.", "Contract.term.valuedItem.identifier" },
            { "ConsentDirective.term.valuedItem.effectiveTime", "Contract.term.valuedItem.effectiveTime" },
            { "ConsentDirective.term.valuedItem.quantity", "Contract.term.valuedItem.quantity" },
            { "ConsentDirective.term.valuedItem.unitPrice", "Contract.term.valuedItem.unitPrice" },
            { "ConsentDirective.term.valuedItem.factor", "Contract.term.valuedItem.factor" },
            { "ConsentDirective.term.valuedItem.points", "Contract.term.valuedItem.points" },
            { "ConsentDirective.term.valuedItem.net", "Contract.term.valuedItem.net" },
            { "Contract.term", "Contract.term" },
            { "Condition.onsetquantity", "Condition.onsetQuantity" },
            { "Condition.onsetdatetime", "Condition.onsetDateTime" },
            { "DiagnosticReport.USLabLOINCCoding", "DiagnosticReport.code.coding" },
            { "MedicationAdministration.medicationcodeableconcept", "MedicationAdministration.medicationCodeableConcept" },
            { "MedicationAdministration.medicationreference", "MedicationAdministration.medicationReference" },
            { "Observation.referenceRange", "Observation.referenceRange" },
            { "Specimen.USLabPlacerSID", "Specimen.identifier" },
            { "DiagnosticOrder.event", "DiagnosticOrder.event" },
            { "ImplementationGuide.page", "ImplementationGuide.page" },
            { "OperationDefinition.parameter", "OperationDefinition.parameter" },
            { "Parameters.parameter", "Parameters.parameter" },
            { "Provenance.agent", "Provenance.agent" },
            { "DiagnosticReport.locationPerformed.valueReference", "DiagnosticReport.extension.valueReference" },
            { "Questionnaire.group", "Questionnaire.group" },
            { "QuestionnaireResponse.group", "QuestionnaireResponse.group" },
            { "ValueSet.designation", "ValueSet.codeSystem.concept.designation" },
            { "DataElement.l", "DataElement.element.maxValue[x]" },
            { "DataElement.MappingEquivalence", "DataElement.element.mapping.extension" },
            { "ValueSet.concept", "ValueSet.codeSystem.concept" },
            { "ValueSet.include", "ValueSet.compose.include" },
            { "ValueSet.contains", "ValueSet.expansion.contains" },
            { "TestScript.metadata", "TestScript.metadata" },
            { "TestScript.operation", "TestScript.setup.action.operation" },
            { "TestScript.assert", "TestScript.setup.action.assert" },
            { "DiagnosticOrder.USLabDOPlacerID", "DiagnosticOrder.identifier" },
        };
    }

    /// <summary>Query if this object has issues.</summary>
    /// <param name="errorCount">  [out] Number of errors.</param>
    /// <param name="warningCount">[out] Number of warnings.</param>
    /// <returns>True if issues, false if not.</returns>
    public bool HasIssues(out int errorCount, out int warningCount)
    {
        errorCount = _errors.Count;
        warningCount = _warnings.Count;

        if ((errorCount > 0) || (warningCount > 0))
        {
            return true;
        }

        return false;
    }

    /// <summary>Displays the issues.</summary>
    public void DisplayIssues()
    {
#pragma warning disable CA1303 // Do not pass literals as localized parameters
        Console.WriteLine("Errors (only able to pass with manual code changes)");

        foreach (string value in _errors)
        {
            Console.WriteLine($" - {value}");
        }

        Console.WriteLine("Warnings (able to pass, but should be reviewed)");

        foreach (string value in _warnings)
        {
            Console.WriteLine($" - {value}");
        }
#pragma warning restore CA1303 // Do not pass literals as localized parameters
    }

    /// <summary>Process the value set.</summary>
    /// <param name="vs">             The vs.</param>
    /// <param name="fhirVersionInfo">FHIR Version information.</param>
    private void ProcessValueSet(
        fhirModels.ValueSet vs,
        IPackageImportable fhirVersionInfo)
    {
        // ignore retired
        if (vs.Status.Equals("retired", StringComparison.Ordinal))
        {
            return;
        }

        // do not process a value set if we have already loaded it
        if (fhirVersionInfo.HasValueSet(vs.Url))
        {
            return;
        }

        if (vs.CodeSystem != null)
        {
            ProcessCodeSystem(vs.CodeSystem, fhirVersionInfo, vs.Id);
        }

        List<FhirValueSetComposition> includes = null;
        List<FhirValueSetComposition> excludes = null;
        FhirValueSetExpansion expansion = null;

        if ((vs.Compose != null) &&
            (vs.Compose.Include != null) &&
            (vs.Compose.Include.Count > 0))
        {
            includes = new List<FhirValueSetComposition>();

            foreach (fhirModels.ValueSetComposeInclude compose in vs.Compose.Include)
            {
                includes.Add(BuildComposition(compose));
            }
        }

        if ((vs.Compose != null) &&
            (vs.Compose.Exclude != null) &&
            (vs.Compose.Exclude.Count > 0))
        {
            excludes = new List<FhirValueSetComposition>();

            foreach (fhirModels.ValueSetComposeInclude compose in vs.Compose.Exclude)
            {
                excludes.Add(BuildComposition(compose));
            }
        }

        if (vs.Expansion != null)
        {
            Dictionary<string, dynamic> parameters = null;

            if ((vs.Expansion.Parameter != null) && (vs.Expansion.Parameter.Count > 0))
            {
                parameters = new Dictionary<string, dynamic>();

                foreach (fhirModels.ValueSetExpansionParameter param in vs.Expansion.Parameter)
                {
                    if (parameters.ContainsKey(param.Name))
                    {
                        continue;
                    }

                    if (param.ValueBoolean != null)
                    {
                        parameters.Add(param.Name, param.ValueBoolean);
                        continue;
                    }

                    if (param.ValueCode != null)
                    {
                        parameters.Add(param.Name, param.ValueCode);
                        continue;
                    }

                    if (param.ValueDecimal != null)
                    {
                        parameters.Add(param.Name, param.ValueDecimal);
                        continue;
                    }

                    if (param.ValueInteger != null)
                    {
                        parameters.Add(param.Name, param.ValueInteger);
                        continue;
                    }

                    if (!string.IsNullOrEmpty(param.ValueString))
                    {
                        parameters.Add(param.Name, param.ValueString);
                        continue;
                    }

                    if (param.ValueUri != null)
                    {
                        parameters.Add(param.Name, param.ValueUri);
                        continue;
                    }
                }
            }

            List<FhirConcept> expansionContains = null;

            if ((vs.Expansion.Contains != null) && (vs.Expansion.Contains.Count > 0))
            {
                foreach (fhirModels.ValueSetExpansionContains contains in vs.Expansion.Contains)
                {
                    AddContains(ref expansionContains, contains);
                }
            }

            expansion = new FhirValueSetExpansion(
                vs.Expansion.Id,
                vs.Expansion.Timestamp,
                vs.Expansion.Total,
                vs.Expansion.Offset,
                parameters,
                expansionContains);
        }

        if ((includes == null) &&
            (expansion == null) &&
            (vs.CodeSystem != null) &&
            (vs.CodeSystem.Concept != null))
        {
            includes = new List<FhirValueSetComposition>();

            List<FhirConcept> concepts = new List<FhirConcept>();

            foreach (fhirModels.ValueSetCodeSystemConcept concept in vs.CodeSystem.Concept)
            {
                concepts.Add(new FhirConcept(
                    vs.CodeSystem.System,
                    concept.Code,
                    concept.Display,
                    vs.CodeSystem.Version,
                    concept.Definition));
            }

            FhirValueSetComposition comp = new FhirValueSetComposition(
                vs.CodeSystem.System,
                vs.CodeSystem.Version,
                concepts,
                null,
                null);

            includes.Add(comp);
        }

        if (string.IsNullOrEmpty(vs.Url))
        {
            return;
            throw new Exception($"Cannot index ValueSet: {vs.Name} version: {vs.Version}");
        }

        if (string.IsNullOrEmpty(vs.Version))
        {
            return;
            throw new Exception($"Cannot index ValueSet: {vs.Url} version: {vs.Version}");
        }

        string standardStatus =
            vs.Extension?.Where(e => e.Url == "http://hl7.org/fhir/StructureDefinition/structuredefinition-standards-status")
                ?.FirstOrDefault()?.ValueCode;

        int? fmmLevel =
            vs.Extension?.Where(e => e.Url == "http://hl7.org/fhir/StructureDefinition/structuredefinition-fmm")
                ?.FirstOrDefault()?.ValueInteger;

        FhirValueSet valueSet = new FhirValueSet(
            vs.Name,
            vs.Id,
            vs.Version,
            string.Empty,
            vs.Url,
            vs.Status,
            standardStatus,
            fmmLevel,
            vs.Description,
            includes,
            excludes,
            expansion);

        // add our code system
        fhirVersionInfo.AddValueSet(valueSet);
    }

    /// <summary>Adds the contains to 'ec'.</summary>
    /// <param name="contains">[in,out] The contains.</param>
    /// <param name="ec">      The ec.</param>
    private void AddContains(ref List<FhirConcept> contains, fhirModels.ValueSetExpansionContains ec)
    {
        if (contains == null)
        {
            contains = new List<FhirConcept>();
        }

        // TODO: Determine if the Inactive flag needs to be checked
        if ((!string.IsNullOrEmpty(ec.System)) ||
            (!string.IsNullOrEmpty(ec.Code)))
        {
            contains.Add(new FhirConcept(
                ec.System,
                ec.Code,
                ec.Display,
                ec.Version));
        }

        if ((ec.Contains != null) && (ec.Contains.Count > 0))
        {
            foreach (fhirModels.ValueSetExpansionContains subContains in ec.Contains)
            {
                AddContains(ref contains, subContains);
            }
        }
    }

    /// <summary>Builds a composition.</summary>
    /// <param name="compose">The compose.</param>
    /// <returns>A FhirValueSetComposition.</returns>
    private static FhirValueSetComposition BuildComposition(fhirModels.ValueSetComposeInclude compose)
    {
        if (compose == null)
        {
            return null;
        }

        List<FhirConcept> concepts = null;
        List<FhirValueSetFilter> filters = null;
        List<string> linkedValueSets = null;

        if ((compose.Concept != null) && (compose.Concept.Count > 0))
        {
            concepts = new List<FhirConcept>();

            foreach (fhirModels.ValueSetComposeIncludeConcept concept in compose.Concept)
            {
                concepts.Add(new FhirConcept(
                    compose.System,
                    concept.Code,
                    concept.Display));
            }
        }

        if ((compose.Filter != null) && (compose.Filter.Count > 0))
        {
            filters = new List<FhirValueSetFilter>();

            foreach (fhirModels.ValueSetComposeIncludeFilter filter in compose.Filter)
            {
                filters.Add(new FhirValueSetFilter(
                    filter.Property,
                    filter.Op,
                    filter.Value));
            }
        }

        return new FhirValueSetComposition(
            compose.System,
            compose.Version,
            concepts,
            filters,
            linkedValueSets);
    }

    /// <summary>Process the code system.</summary>
    /// <param name="cs">             The create struct.</param>
    /// <param name="fhirVersionInfo">FHIR Version information.</param>
    private void ProcessCodeSystem(
        fhirModels.ValueSetCodeSystem cs,
        IPackageImportable fhirVersionInfo,
        string valueSetId = "")
    {
        string id = cs.Id;

        if (string.IsNullOrEmpty(id))
        {
            id = valueSetId;
            id = id.Replace("-", "_");
        }

        if (string.IsNullOrEmpty(id))
        {
            id = cs.System;
            id = id.Replace("http://hl7.org/fhir/", string.Empty);
            id = id.Replace("/", "_");
            id = id.Replace("-", "_");
        }

        Dictionary<string, FhirConceptTreeNode> nodeLookup = new Dictionary<string, FhirConceptTreeNode>();
        FhirConceptTreeNode root = new FhirConceptTreeNode(null, null);

        if (cs.Concept != null)
        {
            AddConceptTree(cs.System, id, cs.Concept, ref root, ref nodeLookup);
        }

        Dictionary<string, FhirCodeSystem.FilterDefinition> filters = new();
        Dictionary<string, FhirCodeSystem.PropertyDefinition> properties = new();

        string standardStatus =
            cs.Extension?.Where(e => e.Url == "http://hl7.org/fhir/StructureDefinition/structuredefinition-standards-status")
                ?.FirstOrDefault()?.ValueCode;

        int? fmmLevel =
            cs.Extension?.Where(e => e.Url == "http://hl7.org/fhir/StructureDefinition/structuredefinition-fmm")
                ?.FirstOrDefault()?.ValueInteger;

        FhirCodeSystem codeSystem = new FhirCodeSystem(
            string.Empty,
            cs.Id,
            cs.Version,
            string.Empty,
            cs.System,
            string.Empty,
            standardStatus,
            fmmLevel,
            string.Empty,
            string.Empty,
            root,
            nodeLookup,
            filters,
            properties);

        // add our code system
        fhirVersionInfo.AddCodeSystem(codeSystem);
    }

    /// <summary>Adds a concept tree to 'concepts'.</summary>
    /// <param name="codeSystemUrl">URL of the code system.</param>
    /// <param name="codeSystemId"> Identifier for the code system.</param>
    /// <param name="concepts">     The concept.</param>
    /// <param name="parent">       [in,out] The parent.</param>
    /// <param name="nodeLookup">   [in,out] The node lookup.</param>
    private void AddConceptTree(
        string codeSystemUrl,
        string codeSystemId,
        List<fhirModels.ValueSetCodeSystemConcept> concepts,
        ref FhirConceptTreeNode parent,
        ref Dictionary<string, FhirConceptTreeNode> nodeLookup)
    {
        if ((concepts == null) ||
            (concepts.Count == 0) ||
            (parent == null))
        {
            return;
        }

        foreach (fhirModels.ValueSetCodeSystemConcept concept in concepts)
        {
            if (concept.Extension != null)
            {
                bool deprecated = false;
                foreach (fhirModels.Extension ext in concept.Extension)
                {
                    if ((ext.Url == "http://hl7.org/fhir/StructureDefinition/valueset-deprecated") &&
                        (ext.ValueBoolean == true))
                    {
                        deprecated = true;
                        break;
                    }
                }

                if (deprecated)
                {
                    continue;
                }
            }

            if (string.IsNullOrEmpty(concept.Code) || nodeLookup.ContainsKey(concept.Code))
            {
                continue;
            }

            FhirConceptTreeNode node = parent.AddChild(
                new FhirConcept(
                    codeSystemUrl,
                    concept.Code,
                    concept.Display,
                    string.Empty,
                    concept.Definition,
                    codeSystemId));

            if (concept.Concept != null)
            {
                AddConceptTree(codeSystemUrl, codeSystemId, concept.Concept, ref node, ref nodeLookup);
            }

            nodeLookup.Add(concept.Code, node);
        }
    }

    /// <summary>Process the operation.</summary>
    /// <param name="op">             The operation.</param>
    /// <param name="fhirVersionInfo">FHIR Version information.</param>
    private void ProcessOperation(
        fhirModels.OperationDefinition op,
        IPackageImportable fhirVersionInfo)
    {
        // ignore retired
        if (op.Status.Equals("retired", StringComparison.Ordinal))
        {
            return;
        }

        List<FhirParameter> parameters = new List<FhirParameter>();

        if (op.Parameter != null)
        {
            foreach (fhirModels.OperationDefinitionParameter opParam in op.Parameter)
            {
                parameters.Add(new FhirParameter(
                    opParam.Name,
                    opParam.Use,
                    Array.Empty<string>(),
                    opParam.Min,
                    opParam.Max,
                    opParam.Documentation,
                    opParam.Type,
                    Array.Empty<string>(),
                    opParam.Profile == null ? Array.Empty<string>() : new string[1] { opParam.Profile.ReferenceField },
                    string.Empty,
                    parameters.Count));
            }
        }

        string standardStatus =
            op.Extension?.Where(e => e.Url == "http://hl7.org/fhir/StructureDefinition/structuredefinition-standards-status")
                ?.FirstOrDefault()?.ValueCode;

        int? fmmLevel =
            op.Extension?.Where(e => e.Url == "http://hl7.org/fhir/StructureDefinition/structuredefinition-fmm")
                ?.FirstOrDefault()?.ValueInteger;

        // create the operation
        FhirOperation operation = new FhirOperation(
            op.Id,
            new Uri(op.Url),
            op.Version,
            op.Name,
            op.Description,
            op.Status,
            standardStatus,
            fmmLevel,
            op.Idempotent == null ? null : !op.Idempotent,
            op.System,
            (op.Type == null) || (op.Type.Count == 0),
            op.Instance,
            op.Code,
            op.Requirements,
            op.Base?.ReferenceField ?? string.Empty,
            op.Type,
            parameters,
            op.Experimental == true);

        // add our parameter
        fhirVersionInfo.AddOperation(operation);
    }

    /// <summary>Process the search parameter.</summary>
    /// <param name="sp">             The search parameter.</param>
    /// <param name="fhirVersionInfo">FHIR Version information.</param>
    private void ProcessSearchParam(
        fhirModels.SearchParameter sp,
        IPackageImportable fhirVersionInfo)
    {
        // ignore retired
        if (sp.Status == "retired")
        {
            return;
        }

        List<string> resourceList = new List<string>();

        if (!string.IsNullOrEmpty(sp.Base))
        {
            resourceList.Add(sp.Base);
        }

        // check for parameters with no base resource
        if (resourceList.Count == 0)
        {
            // see if we can determine the resource based on id
            string[] components = sp.Id.Split('-');

            foreach (string component in components)
            {
                if (fhirVersionInfo.Resources.ContainsKey(component))
                {
                    resourceList.Add(component);
                }
            }

            // don't know where to put this, could try parsing XPath in the future
            if (resourceList.Count == 0)
            {
                return;
            }
        }

        string standardStatus =
            sp.Extension?.Where(e => e.Url == "http://hl7.org/fhir/StructureDefinition/structuredefinition-standards-status")
                ?.FirstOrDefault()?.ValueCode;

        int? fmmLevel =
            sp.Extension?.Where(e => e.Url == "http://hl7.org/fhir/StructureDefinition/structuredefinition-fmm")
                ?.FirstOrDefault()?.ValueInteger;


        // create the search parameter
        FhirSearchParam param = new FhirSearchParam(
            sp.Id,
            new Uri(sp.Url),
            fhirVersionInfo.VersionString,
            sp.Name,
            sp.Description,
            string.Empty,
            sp.Code,
            resourceList,
            sp.Target,
            sp.Type,
            sp.Status,
            standardStatus,
            fmmLevel,
            sp.Experimental == true,
            sp.Xpath,
            sp.XpathUsage,
            string.Empty);

        // add our parameter
        fhirVersionInfo.AddSearchParameter(param);
    }

    /// <summary>Process the structure definition.</summary>
    /// <param name="sd">             The structure definition to parse.</param>
    /// <param name="fhirVersionInfo">FHIR Version information.</param>
    private void ProcessStructureDef(
        fhirModels.StructureDefinition sd,
        IPackageImportable fhirVersionInfo,
        out FhirArtifactClassEnum artifactClass)
    {
        // ignore retired
        if (sd.Status.Equals("retired", StringComparison.Ordinal))
        {
            artifactClass = FhirArtifactClassEnum.Unknown;
            return;
        }

        // act depending on kind
        switch (sd.Kind)
        {
            case "datatype":
                if (sd.ConstrainedType == "Extension")
                {
                    ProcessComplex(sd, fhirVersionInfo, FhirArtifactClassEnum.Extension);
                    artifactClass = FhirArtifactClassEnum.Extension;
                }
                else
                {
                    // leading lower case is primitive
                    if (char.IsLower(sd.Name[0]))
                    {
                        ProcessDataTypePrimitive(sd, fhirVersionInfo);
                        artifactClass = FhirArtifactClassEnum.PrimitiveType;
                    }
                    else
                    {
                        ProcessComplex(sd, fhirVersionInfo, FhirArtifactClassEnum.ComplexType);
                        artifactClass = FhirArtifactClassEnum.ComplexType;
                    }
                }

                break;

            case "resource":
                if (string.IsNullOrEmpty(sd.ConstrainedType) ||
                    (sd.ConstrainedType == "Quantity"))
                {
                    ProcessComplex(sd, fhirVersionInfo, FhirArtifactClassEnum.Resource);
                    artifactClass = FhirArtifactClassEnum.Resource;
                }
                else
                {
                    ProcessComplex(sd, fhirVersionInfo, FhirArtifactClassEnum.Extension);
                    artifactClass = FhirArtifactClassEnum.Extension;
                }

                break;

            default:
                artifactClass = FhirArtifactClassEnum.Unknown;
                break;
        }
    }

    /// <summary>Process a structure definition for a primitive data type.</summary>
    /// <param name="sd">             The structure definition to parse.</param>
    /// <param name="fhirVersionInfo">FHIR Version information.</param>
    private static void ProcessDataTypePrimitive(
        fhirModels.StructureDefinition sd,
        IPackageImportable fhirVersionInfo)
    {
        string regex = string.Empty;
        string descriptionShort = sd.Description;
        string definition = sd.Requirements;
        string comment = string.Empty;
        string baseTypeName = string.Empty;

        if ((sd.Snapshot != null) &&
            (sd.Snapshot.Element != null) &&
            (sd.Snapshot.Element.Count > 0))
        {
            foreach (fhirModels.ElementDefinition element in sd.Snapshot.Element)
            {
                if (!element.Path.Contains('.'))
                {
                    descriptionShort = element.Short;
                    definition = element.Definition;
                    comment = element.Requirements;
                    continue;
                }

                string[] components = element.Path.Split('.');

                if (components.Length != 2)
                {
                    continue;
                }

                if (components[1] != "value")
                {
                    continue;
                }

                if (element.Type == null)
                {
                    continue;
                }

                foreach (fhirModels.ElementDefinitionType type in element.Type)
                {
                    if (type.Extension != null)
                    {
                        foreach (fhirModels.Extension ext in type.Extension)
                        {
                            if (ext.Url == "http://hl7.org/fhir/StructureDefinition/structuredefinition-regex")
                            {
                                regex = ext.ValueString;
                                break;
                            }
                        }
                    }

                    if ((type._Code != null) &&
                        (type._Code.Extension != null))
                    {
                        foreach (fhirModels.Extension ext in type._Code.Extension)
                        {
                            if ((ext.Url == "http://hl7.org/fhir/StructureDefinition/structuredefinition-xml-type") &&
                                FhirElementType.IsXmlBaseType(ext.ValueString, out string fhirType))
                            {
                                baseTypeName = fhirType;
                            }
                        }
                    }
                }
            }
        }

        if (string.IsNullOrEmpty(baseTypeName))
        {
            baseTypeName = sd.Name;
        }

        string standardStatus =
            sd.Extension?.Where(e => e.Url == "http://hl7.org/fhir/StructureDefinition/structuredefinition-standards-status")
                ?.FirstOrDefault()?.ValueCode;

        int? fmmLevel =
            sd.Extension?.Where(e => e.Url == "http://hl7.org/fhir/StructureDefinition/structuredefinition-fmm")
                ?.FirstOrDefault()?.ValueInteger;

        // create a new primitive type object
        FhirPrimitive primitive = new FhirPrimitive(
            sd.Id,
            sd.Name,
            baseTypeName,
            new Uri(sd.Url),
            sd.Status,
            standardStatus,
            fmmLevel,
            sd.Experimental == true,
            descriptionShort,
            definition,
            comment,
            regex);

        // add to our dictionary of primitive types
        fhirVersionInfo.AddPrimitive(primitive);
    }

    /// <summary>Gets type from element.</summary>
    /// <param name="structureName">Name of the structure.</param>
    /// <param name="element">      The element.</param>
    /// <param name="elementTypes"> [out] Name of the type.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    private static bool TryGetTypeFromElement(
        string structureName,
        fhirModels.ElementDefinition element,
        out Dictionary<string, FhirElementType> elementTypes)
    {
        elementTypes = new Dictionary<string, FhirElementType>();

        // check for declared type
        if (element.Type != null)
        {
            foreach (fhirModels.ElementDefinitionType edType in element.Type)
            {
                // check for a specified type
                if (!string.IsNullOrEmpty(edType.Code))
                {
                    // create a type for this code
                    FhirElementType elementType = new FhirElementType(edType.Code);

                    if (!elementTypes.ContainsKey(elementType.Name))
                    {
                        elementTypes.Add(elementType.Name, elementType);
                    }

                    if (edType.Profile != null)
                    {
                        if (elementType.Name == "Reference")
                        {
                            foreach (string profile in edType.Profile)
                            {
                                elementTypes[elementType.Name].AddProfile(profile);
                            }
                        }
                        else
                        {
                            foreach (string profile in edType.Profile)
                            {
                                elementTypes[elementType.Name].AddTypeProfile(profile);
                            }
                        }
                    }

                    continue;
                }

                // use an extension-defined type
                foreach (fhirModels.Extension ext in edType._Code.Extension)
                {
                    switch (ext.Url)
                    {
                        case FhirPackageCommon.UrlXmlType:
                        case FhirPackageCommon.UrlFhirType:

                            // create a type for this code
                            FhirElementType elementType = new FhirElementType(edType.Code);

                            if (!elementTypes.ContainsKey(elementType.Name))
                            {
                                elementTypes.Add(elementType.Name, elementType);
                            }

                            if (edType.Profile != null)
                            {
                                if (elementType.Name == "Reference")
                                {
                                    foreach (string profile in edType.Profile)
                                    {
                                        elementTypes[elementType.Name].AddProfile(profile);
                                    }
                                }
                                else
                                {
                                    foreach (string profile in edType.Profile)
                                    {
                                        elementTypes[elementType.Name].AddTypeProfile(profile);
                                    }
                                }
                            }

                            break;

                        default:
                            // ignore
                            break;
                    }
                }
            }
        }

        if (elementTypes.Count > 0)
        {
            return true;
        }

        // check for base derived type
        if (string.IsNullOrEmpty(element.Name) ||
            element.Name.Equals(structureName, StringComparison.Ordinal))
        {
            // base type is here
            FhirElementType elementType = new FhirElementType(element.Path);

            // add to our dictionary
            elementTypes.Add(elementType.Name, elementType);

            // done searching
            return true;
        }

        // no discovered type
        elementTypes = null;
        return false;
    }

    /// <summary>Attempts to get type from elements.</summary>
    /// <param name="structureName">Name of the structure.</param>
    /// <param name="elements">     The elements.</param>
    /// <param name="elementTypes"> [out] Name of the type.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    private static bool TryGetTypeFromElements(
        string structureName,
        List<fhirModels.ElementDefinition> elements,
        out Dictionary<string, FhirElementType> elementTypes)
    {
        elementTypes = null;

        foreach (fhirModels.ElementDefinition element in elements)
        {
            // split the path
            string[] components = element.Path.Split('.');

            // check for base path having a type
            if (components.Length == 1)
            {
                if (TryGetTypeFromElement(structureName, element, out elementTypes))
                {
                    // done searching
                    return true;
                }
            }

            // check for path {type}.value having a type
            if ((components.Length == 2) &&
                components[1].Equals("value", StringComparison.Ordinal))
            {
                if (TryGetTypeFromElement(structureName, element, out elementTypes))
                {
                    // keep looking in case we find a better option
                    continue;
                }
            }
        }

        if (elementTypes != null)
        {
            return true;
        }

        return false;
    }

    /// <summary>Process a complex structure (Complex Type or Resource).</summary>
    /// <exception cref="InvalidDataException">Thrown when an Invalid Data error condition occurs.</exception>
    /// <param name="sd">                   The structure definition to parse.</param>
    /// <param name="fhirVersionInfo">      FHIR Version information.</param>
    /// <param name="artifactClass">        The artifact class.</param>
    private static void ProcessComplex(
        fhirModels.StructureDefinition sd,
        IPackageImportable fhirVersionInfo,
        FhirArtifactClassEnum artifactClass)
    {
        if ((sd.Snapshot == null) || (sd.Snapshot.Element == null))
        {
            return;
        }

        string descriptionShort = sd.Description;
        string definition = sd.Requirements;

        try
        {
            List<string> contextElements = new List<string>();
            if (sd.Context != null)
            {
                contextElements.AddRange(sd.Context);
            }

            if (sd.Snapshot.Element.Count > 0)
            {
                descriptionShort = sd.Snapshot.Element[0].Short;
                definition = sd.Snapshot.Element[0].Definition;
            }

            string standardStatus =
                sd.Extension?.Where(e => e.Url == "http://hl7.org/fhir/StructureDefinition/structuredefinition-standards-status")
                    ?.FirstOrDefault()?.ValueCode
                ?? sd.Status;

            int? fmmLevel =
                sd.Extension?.Where(e => e.Url == "http://hl7.org/fhir/StructureDefinition/structuredefinition-fmm")
                    ?.FirstOrDefault()?.ValueInteger;

            // create a new complex type object
            FhirComplex complex = new FhirComplex(
                sd.Id,
                sd.Name,
                string.Empty,
                string.Empty,
                sd.ConstrainedType,
                new Uri(sd.Url),
                sd.Status,
                standardStatus,
                fmmLevel,
                sd.Experimental == true,
                descriptionShort,
                definition,
                string.Empty,
                null,
                contextElements,
                sd.Abstract);

            // check for a base definition
            if (!string.IsNullOrEmpty(sd.Base))
            {
                complex.BaseTypeName = sd.Base.Substring(sd.Base.LastIndexOf('/') + 1);
                complex.BaseTypeCanonical = sd.Base;
            }
            else
            {
                if (!TryGetTypeFromElements(sd.Name, sd.Snapshot.Element, out Dictionary<string, FhirElementType> baseTypes))
                {
                    throw new InvalidDataException($"Could not determine base type for {sd.Name}");
                }

                if (baseTypes.Count == 0)
                {
                    throw new InvalidDataException($"Could not determine base type for {sd.Name}");
                }

                if (baseTypes.Count > 1)
                {
                    throw new InvalidDataException($"Too many types for {sd.Name}: {baseTypes.Count}");
                }

                complex.BaseTypeName = baseTypes.ElementAt(0).Value.Name;
            }

            HashSet<string> differentialPaths = new HashSet<string>();

            if ((sd.Differential != null) && (sd.Differential.Element != null))
            {
                foreach (fhirModels.ElementDefinition element in sd.Differential.Element)
                {
                    if (string.IsNullOrEmpty(element.Path))
                    {
                        continue;
                    }

                    differentialPaths.Add(element.Path);
                }
            }

            HashSet<int> slicingDepths = new HashSet<int>();
            string[] slicingPaths = new string[MaxPathComponents];
            string[] traversalSliceNames = new string[MaxPathComponents];

            for (int i = 0; i < MaxPathComponents; i++)
            {
                slicingPaths[i] = string.Empty;
                traversalSliceNames[i] = string.Empty;
            }

            // look for properties on this type
            foreach (fhirModels.ElementDefinition element in sd.Snapshot.Element)
            {
                try
                {
                    string id = element.Id ?? element.Path;
                    string path = element.Path ?? element.Id;
                    Dictionary<string, FhirElementType> elementTypes = null;
                    string elementType = string.Empty;
                    bool isRootElement = false;

                    // split the id into component parts
                    string[] idComponents = id.Split('.');
                    string[] pathComponents = path.Split('.');

                    // base definition, already processed
                    if (pathComponents.Length < 2)
                    {
                        // check for this component being different from primary
                        if ((pathComponents[0] != sd.Name) && (contextElements.Count == 0))
                        {
                            // add to our context
                            complex.AddContextElement(pathComponents[0]);
                        }

                        // parse as root element
                        isRootElement = true;
                    }

                    // check for having slicing
                    if (slicingDepths.Count > 0)
                    {
                        List<int> depthsToRemove = new List<int>();

                        int elementDepth = pathComponents.Length - 1;

                        // check according to depth
                        foreach (int depth in slicingDepths)
                        {
                            // check for earlier depths than current
                            if (elementDepth > depth)
                            {
                                // add slice name to this component
                                idComponents[depth] = $"{idComponents[depth]}:{traversalSliceNames[depth]}";

                                // done with this slicing
                                continue;
                            }

                            // check for matching slicing level
                            if (elementDepth == depth)
                            {
                                if ((path == slicingPaths[depth]) &&
                                    (!string.IsNullOrEmpty(element.Name)))
                                {
                                    // grab our new slice name
                                    traversalSliceNames[depth] = element.Name;

                                    // add slice name to this component
                                    idComponents[depth] = $"{idComponents[depth]}:{traversalSliceNames[depth]}";

                                    // done with this depth
                                    continue;
                                }
                            }

                            // we have iterated out of this slicing group
                            slicingPaths[depth] = string.Empty;
                            traversalSliceNames[depth] = string.Empty;
                            depthsToRemove.Add(depth);
                        }

                        // remove cleared depths
                        foreach (int depth in depthsToRemove)
                        {
                            slicingDepths.Remove(depth);
                        }
                    }

                    // check for needing to rebuild the id
                    if (slicingDepths.Count > 0)
                    {
                        // rebuild the id with our slicing information
                        id = string.Join(".", idComponents);
                    }

                    // get the parent container and our field name
                    if (!complex.GetParentAndFieldName(
                            sd.Url,
                            idComponents,
                            pathComponents,
                            out FhirComplex parent,
                            out string field,
                            out string sliceName))
                    {
                        if (isRootElement)
                        {
                            parent = complex;
                            field = string.Empty;
                            sliceName = string.Empty;
                        }
                        else
                        {
                            // throw new InvalidDataException($"Could not find parent for {element.Path}!");
                            // should load later
                            // TODO: figure out a way to verify all dependencies loaded
                            continue;
                        }
                    }

                    // check for needing to add a slice to an element
                    if (!string.IsNullOrEmpty(sliceName))
                    {
                        // check for extension (implicit slicing in differentials)
                        if ((!parent.Elements.ContainsKey(path)) && (field == "extension"))
                        {
                            // grab the extension definition
                            parent.Elements.Add(
                                path,
                                new FhirElement(
                                    path,
                                    path,
                                    string.Empty,
                                    null,
                                    parent.Elements.Count,
                                    ExtensionShort,
                                    ExtensionDefinition,
                                    ExtensionComment,
                                    string.Empty,
                                    "Extension",
                                    null,
                                    0,
                                    "*",
                                    element.IsModifier,
                                    string.Empty,
                                    element.IsSummary,
                                    element.MustSupport,
                                    false,
                                    string.Empty,
                                    null,
                                    string.Empty,
                                    null,
                                    string.Empty,
                                    null,
                                    true,
                                    true,
                                    string.Empty,
                                    string.Empty,
                                    null,
                                    null));
                        }

                        // check for implicit slicing definition
                        if (parent.Elements.ContainsKey(path) &&
                            (!parent.Elements[path].Slicing.ContainsKey(sd.Url)))
                        {
                            List<FhirSliceDiscriminatorRule> discriminatorRules = new List<FhirSliceDiscriminatorRule>()
                            {
                                new FhirSliceDiscriminatorRule(
                                    "value",
                                    "url"),
                            };

                            // create our slicing
                            parent.Elements[path].AddSlicing(
                                new FhirSlicing(
                                    sd.Id,
                                    new Uri(sd.Url),
                                    "Extensions are always sliced by (at least) url",
                                    null,
                                    "open",
                                    discriminatorRules));
                        }

                        // check for invalid slicing definition (composition-catalog)
                        if (parent.Elements.ContainsKey(path))
                        {
                            // add this slice to the field
                            parent.Elements[path].AddSlice(sd.Url, sliceName);
                        }

                        // only slice parent has slice name
                        continue;
                    }

                    // if we can't find a type, assume Element
                    if (!TryGetTypeFromElement(parent.Name, element, out elementTypes))
                    {
                        if ((field == "Extension") || (field == "extension"))
                        {
                            elementType = "Extension";
                        }
                        else
                        {
                            elementType = "Element";
                        }
                    }

                    // determine if there is type expansion
                    if (field.Contains("[x]", StringComparison.OrdinalIgnoreCase))
                    {
                        // fix the field and path names
                        id = id.Replace("[x]", string.Empty, StringComparison.OrdinalIgnoreCase);
                        field = field.Replace("[x]", string.Empty, StringComparison.OrdinalIgnoreCase);

                        // force no base type
                        elementType = string.Empty;
                    }
                    else if (!string.IsNullOrEmpty(element.NameReference))
                    {
                        string lookupName;

                        if (element.NameReference.Contains('.', StringComparison.Ordinal))
                        {
                            lookupName = element.NameReference;
                        }
                        else
                        {
                            lookupName = $"{element.Path.Split('.')[0]}.{element.NameReference}";
                        }

                        // look up the named reference in the alias table
                        if (!_namedReferenceLinks.ContainsKey(lookupName))
                        {
                            throw new InvalidDataException($"Could not resolve NameReference {element.NameReference} in {sd.Name} field {element.Path}");
                        }

                        // use the named type
                        elementType = _namedReferenceLinks[lookupName];
                        elementTypes = null;
                    }

                    // get default values (if present)
                    GetDefaultValueIfPresent(element, out string defaultName, out object defaultValue);

                    // get fixed values (if present)
                    GetFixedValueIfPresent(element, out string fixedName, out object fixedValue);

                    // determine if this element is inherited
                    bool isInherited = false;
                    bool modifiesParent = true;

                    if (element.Base != null)
                    {
                        if (element.Base.Path != element.Path)
                        {
                            isInherited = true;
                        }

                        if ((element.Base.Min == element.Min) &&
                            (element.Base.Max == element.Max) &&
                            (element.Slicing == null))
                        {
                            modifiesParent = false;
                        }
                    }

                    if (!differentialPaths.Contains(element.Path))
                    {
                        isInherited = true;
                        modifiesParent = false;
                    }

                    string bindingStrength = string.Empty;
                    string valueSet = string.Empty;

                    if (element.Binding != null)
                    {
                        bindingStrength = element.Binding.Strength;
                        valueSet = element.Binding.ValueSetUri;

                        if (string.IsNullOrEmpty(valueSet) &&
                            (element.Binding.ValueSetReference != null))
                        {
                            valueSet = element.Binding.ValueSetReference.ReferenceField;
                        }
                    }

                    string explicitName = string.Empty;
                    if (element.Extension != null)
                    {
                        foreach (fhirModels.Extension ext in element.Extension)
                        {
                            if (ext.Url == "http://hl7.org/fhir/StructureDefinition/structuredefinition-explicit-type-name")
                            {
                                explicitName = ext.ValueString;
                            }
                        }
                    }

                    List<string> fwMapping = element.Mapping?.Where(x =>
                        (x != null) &&
                        x.Identity.Equals("w5", StringComparison.OrdinalIgnoreCase) &&
                        x.Map.StartsWith("FiveWs", StringComparison.Ordinal) &&
                        (!x.Map.Equals("FiveWs.subject[x]", StringComparison.Ordinal)))?
                            .Select(x => x.Map).ToList();

                    string fiveWs = ((fwMapping != null) && fwMapping.Any()) ? fwMapping[0] : string.Empty;

                    FhirElement fhirElement = new FhirElement(
                        id,
                        path,
                        explicitName,
                        null,
                        parent.Elements.Count,
                        element.Short,
                        element.Definition,
                        element.Requirements,
                        string.Empty,
                        elementType,
                        elementTypes,
                        (int)(element.Min ?? 0),
                        element.Max,
                        element.IsModifier,
                        string.Empty,
                        element.IsSummary,
                        element.MustSupport,
                        false,
                        defaultName,
                        defaultValue,
                        fixedName,
                        fixedValue,
                        string.Empty,
                        null,
                        isInherited,
                        modifiesParent,
                        bindingStrength,
                        valueSet,
                        fiveWs,
                        FhirElement.ConvertFhirRepresentations(element.Representation));

                    if (isRootElement)
                    {
                        parent.AddRootElement(fhirElement);
                    }

                    // elements can repeat in R2 due to the way slicing was done
                    else if (!parent.Elements.ContainsKey(path))
                    {
                        // add this field to the parent type
                        parent.Elements.Add(path, fhirElement);
                    }

                    if (element.Slicing != null)
                    {
                        List<FhirSliceDiscriminatorRule> discriminatorRules = new List<FhirSliceDiscriminatorRule>();

                        if (element.Slicing.Discriminator == null)
                        {
                            throw new InvalidDataException($"Missing slicing discriminator: {sd.Name} - {element.Path}");
                        }

                        foreach (string discriminator in element.Slicing.Discriminator)
                        {
                            discriminatorRules.Add(new FhirSliceDiscriminatorRule(
                                "value",
                                discriminator));
                        }

                        // create our slicing
                        parent.Elements[path].AddSlicing(
                            new FhirSlicing(
                                sd.Id,
                                new Uri(sd.Url),
                                element.Slicing.Description,
                                element.Slicing.Ordered,
                                element.Slicing.Rules,
                                discriminatorRules));

                        // flag we are in a slicing
                        int slicingDepth = pathComponents.Length - 1;
                        slicingDepths.Add(slicingDepth);
                        slicingPaths[slicingDepth] = element.Path;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(string.Empty);
                    Console.WriteLine($"FromR4.ProcessComplex <<< element: {element.Path} ({element.Id}) - exception: {ex.Message}");
                    throw;
                }
            }

            if ((sd.Differential != null) &&
                (sd.Differential.Element != null) &&
                (sd.Differential.Element.Count > 0) &&
                (sd.Differential.Element[0].Constraint != null) &&
                (sd.Differential.Element[0].Constraint.Count > 0))
            {
                foreach (fhirModels.ElementDefinitionConstraint con in sd.Differential.Element[0].Constraint)
                {
                    bool isBestPractice = false;
                    string explanation = string.Empty;

                    if (con.Extension != null)
                    {
                        foreach (fhirModels.Extension ext in con.Extension)
                        {
                            switch (ext.Url)
                            {
                                case "http://hl7.org/fhir/StructureDefinition/elementdefinition-bestpractice":
                                    isBestPractice = ext.ValueBoolean == true;
                                    break;

                                case "http://hl7.org/fhir/StructureDefinition/elementdefinition-bestpractice-explanation":
                                    if (!string.IsNullOrEmpty(ext.ValueMarkdown))
                                    {
                                        explanation = ext.ValueMarkdown;
                                    }
                                    else
                                    {
                                        explanation = ext.ValueString;
                                    }

                                    break;
                            }
                        }
                    }

                    complex.AddConstraint(new FhirConstraint(
                        con.Key,
                        con.Severity,
                        con.Human,
                        string.Empty,
                        con.Xpath,
                        isBestPractice,
                        explanation));
                }
            }

            // TODO(ginoc): Review this hack to add fhir_comments to Element and persist it
            if (complex.Id.Equals("Element", StringComparison.Ordinal))
            {
                complex.Elements.Add(
                    "Element.fhir_comments",
                    new FhirElement(
                        "Element.fhir_comments",
                        "Element.fhir_comments",
                        string.Empty,
                        null,
                        int.MaxValue,
                        "JSON-Serialization Comments - not an actual element",
                        "JSON Serialization Comments - not an actual element",
                        string.Empty,
                        string.Empty,
                        string.Empty,
                        new Dictionary<string, FhirElementType>()
                        {
                            { "string", new FhirElementType("string") },
                        },
                        0,
                        "*",
                        false,
                        string.Empty,
                        false,
                        false,
                        true,
                        string.Empty,
                        null,
                        string.Empty,
                        null,
                        string.Empty,
                        null,
                        false,
                        false,
                        string.Empty,
                        string.Empty,
                        null,
                        null));
            }

            if ((sd.Differential != null) &&
                (sd.Differential.Element != null) &&
                (sd.Differential.Element.Count > 0))
            {
                // look for additional constraints
                if ((sd.Differential.Element[0].Constraint != null) &&
                    (sd.Differential.Element[0].Constraint.Count > 0))
                {
                    foreach (fhirModels.ElementDefinitionConstraint con in sd.Differential.Element[0].Constraint)
                    {
                        bool isBestPractice = false;
                        string explanation = string.Empty;

                        if (con.Extension != null)
                        {
                            foreach (fhirModels.Extension ext in con.Extension)
                            {
                                switch (ext.Url)
                                {
                                    case "http://hl7.org/fhir/StructureDefinition/elementdefinition-bestpractice":
                                        isBestPractice = ext.ValueBoolean == true;
                                        break;

                                    case "http://hl7.org/fhir/StructureDefinition/elementdefinition-bestpractice-explanation":
                                        if (!string.IsNullOrEmpty(ext.ValueMarkdown))
                                        {
                                            explanation = ext.ValueMarkdown;
                                        }
                                        else
                                        {
                                            explanation = ext.ValueString;
                                        }

                                        break;
                                }
                            }
                        }

                        complex.AddConstraint(new FhirConstraint(
                            con.Key,
                            con.Severity,
                            con.Human,
                            string.Empty,
                            con.Xpath,
                            isBestPractice,
                            explanation));
                    }
                }

                // traverse all elements to flag proper 'differential' tags on elements
                foreach (fhirModels.ElementDefinition dif in sd.Differential.Element)
                {
                    if (complex.Elements.ContainsKey(dif.Path))
                    {
                        complex.Elements[dif.Path].SetInDifferential();
                    }
                }
            }

            switch (artifactClass)
            {
                case FhirArtifactClassEnum.ComplexType:
                    fhirVersionInfo.AddComplexType(complex);
                    break;
                case FhirArtifactClassEnum.Resource:
                    fhirVersionInfo.AddResource(complex);
                    break;
                case FhirArtifactClassEnum.Extension:
                    fhirVersionInfo.AddExtension(complex);
                    break;
                case FhirArtifactClassEnum.Profile:
                    fhirVersionInfo.AddProfile(complex);
                    break;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(string.Empty);
            Console.WriteLine($"FromR2.ProcessComplex <<< SD: {sd.Name} ({sd.Id}) - exception: {ex.Message}");
            throw;
        }
    }

    /// <summary>Parses resource an object from the given string.</summary>
    /// <exception cref="JsonException">Thrown when a JSON error condition occurs.</exception>
    /// <param name="json">The JSON.</param>
    /// <returns>A typed Resource object.</returns>
    object IFhirConverter.ParseResource(string json)
    {
        try
        {
            // try to parse this JSON into a resource object
            // return JsonConvert.DeserializeObject<fhirModels.Resource>(json, _jsonConverter);
            // return JsonSerializer.Deserialize<fhirModels.Resource>(json);
            return System.Text.Json.JsonSerializer.Deserialize<fhirModels.Resource>(
                json,
                fhirSerialization.FhirSerializerOptions.Compact);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"FromR2.ParseResource <<< failed to parse:\n{ex}\n------------------------------------");
            throw;
        }
    }

    /// <summary>Attempts to process resource.</summary>
    /// <param name="resourceToParse">The resource object.</param>
    /// <param name="fhirVersionInfo">Information describing the FHIR version.</param>
    public void ProcessResource(object resourceToParse, IPackageImportable fhirVersionInfo)
    {
        ProcessResource(resourceToParse, fhirVersionInfo, out _, out _);
    }

    /// <summary>Attempts to process resource.</summary>
    /// <param name="resourceToParse">The resource object.</param>
    /// <param name="fhirVersionInfo">Information describing the FHIR version.</param>
    /// <param name="resourceCanonical">Canonical URL of the processed resource, or string.Empty if not processed.</param>
    /// <param name="artifactClass">  Class of the resource parsed</param>
    public void ProcessResource(
        object resourceToParse,
        IPackageImportable fhirVersionInfo,
        out string resourceCanonical,
        out FhirArtifactClassEnum artifactClass)
    {
        switch (resourceToParse)
        {
            // ignore
            // case fhirModels.Conformance conformance:
            // case fhirModels.NamingSystem namingSystem:

            // process
            case fhirModels.ValueSetCodeSystem codeSystem:
                ProcessCodeSystem(codeSystem, fhirVersionInfo);
                resourceCanonical = codeSystem.System;
                artifactClass = FhirArtifactClassEnum.CodeSystem;
                break;

            case fhirModels.OperationDefinition operationDefinition:
                ProcessOperation(operationDefinition, fhirVersionInfo);
                resourceCanonical = operationDefinition.Url;
                artifactClass = FhirArtifactClassEnum.Operation;
                break;

            case fhirModels.SearchParameter searchParameter:
                ProcessSearchParam(searchParameter, fhirVersionInfo);
                resourceCanonical = searchParameter.Url;
                artifactClass = FhirArtifactClassEnum.SearchParameter;
                break;

            case fhirModels.StructureDefinition structureDefinition:
                ProcessStructureDef(structureDefinition, fhirVersionInfo, out artifactClass);
                resourceCanonical = structureDefinition.Url;
                break;

            case fhirModels.ValueSet valueSet:
                ProcessValueSet(valueSet, fhirVersionInfo);
                resourceCanonical = valueSet.Url;
                artifactClass = FhirArtifactClassEnum.ValueSet;
                break;

            default:
                resourceCanonical = string.Empty;
                artifactClass = FhirArtifactClassEnum.Unknown;
                break;
        }
    }

    /// <summary>
    /// Replace a value in a parsed but not-yet processed resource
    /// </summary>
    /// <param name="resource"></param>
    /// <param name="path"></param>
    /// <param name="value"></param>
    public void ReplaceValue(
        object resource,
        string[] path,
        object value)
    {
        throw new NotImplementedException();
    }

    /// <summary>Process a FHIR metadata resource into Server Information.</summary>
    /// <param name="metadata">  The metadata resource object (e.g., r4.CapabilitiesStatement).</param>
    /// <param name="serverUrl"> URL of the server.</param>
    /// <param name="serverInfo">[out] Information describing the server.</param>
    void IFhirConverter.ProcessMetadata(
        object metadata,
        string serverUrl,
        out FhirServerInfo serverInfo)
    {
        if (metadata == null)
        {
            serverInfo = null;
            return;
        }

        fhirModels.Conformance caps = metadata as fhirModels.Conformance;

        string swName = string.Empty;
        string swVersion = string.Empty;
        string swReleaseDate = string.Empty;

        if (caps.Software != null)
        {
            swName = caps.Software.Name ?? string.Empty;
            swVersion = caps.Software.Version ?? string.Empty;
            swReleaseDate = caps.Software.ReleaseDate ?? string.Empty;
        }

        string impDescription = string.Empty;
        string impUrl = string.Empty;

        if (caps.Implementation != null)
        {
            impDescription = caps.Implementation.Description ?? string.Empty;
            impUrl = caps.Implementation.Url ?? string.Empty;
        }

        List<string> serverInteractions = new List<string>();
        Dictionary<string, FhirServerResourceInfo> resourceInteractions = new Dictionary<string, FhirServerResourceInfo>();
        Dictionary<string, FhirServerSearchParam> serverSearchParams = new Dictionary<string, FhirServerSearchParam>();
        Dictionary<string, FhirServerOperation> serverOperations = new Dictionary<string, FhirServerOperation>();

        if ((caps.Rest != null) && (caps.Rest.Count > 0))
        {
            fhirModels.ConformanceRest rest = caps.Rest[0];

            if (rest.Interaction != null)
            {
                foreach (fhirModels.ConformanceRestInteraction interaction in rest.Interaction)
                {
                    if (string.IsNullOrEmpty(interaction.Code))
                    {
                        continue;
                    }

                    serverInteractions.Add(interaction.Code);
                }
            }

            if (rest.Resource != null)
            {
                foreach (fhirModels.ConformanceRestResource resource in rest.Resource)
                {
                    FhirServerResourceInfo resourceInfo = ParseServerRestResource(resource);

                    if (resourceInteractions.ContainsKey(resourceInfo.ResourceType))
                    {
                        continue;
                    }

                    resourceInteractions.Add(
                        resourceInfo.ResourceType,
                        resourceInfo);
                }
            }

            if (rest.SearchParam != null)
            {
                foreach (fhirModels.ConformanceRestResourceSearchParam sp in rest.SearchParam)
                {
                    if (serverSearchParams.ContainsKey(sp.Name))
                    {
                        continue;
                    }

                    serverSearchParams.Add(
                        sp.Name,
                        new FhirServerSearchParam(
                            sp.Name,
                            sp.Definition,
                            sp.Type,
                            sp.Documentation));
                }
            }

            if (rest.Operation != null)
            {
                foreach (fhirModels.ConformanceRestOperation operation in rest.Operation)
                {
                    if (serverOperations.ContainsKey(operation.Name))
                    {
                        serverOperations[operation.Name].AddDefinition(operation.Definition.ReferenceField);
                        continue;
                    }

                    serverOperations.Add(
                        operation.Name,
                        new FhirServerOperation(
                            operation.Name,
                            operation.Definition.ReferenceField,
                            string.Empty));
                }
            }
        }

        serverInfo = new FhirServerInfo(
            serverInteractions,
            serverUrl,
            caps.FhirVersion,
            swName,
            swVersion,
            swReleaseDate,
            impDescription,
            impUrl,
            null,
            null,
            resourceInteractions,
            serverSearchParams,
            serverOperations);
    }

    /// <summary>Parse server REST resource.</summary>
    /// <param name="resource">The resource.</param>
    /// <returns>A FhirServerResourceInfo.</returns>
    private static FhirServerResourceInfo ParseServerRestResource(
        fhirModels.ConformanceRestResource resource)
    {
        List<string> interactions = new List<string>();
        Dictionary<string, FhirServerSearchParam> searchParams = new Dictionary<string, FhirServerSearchParam>();
        Dictionary<string, FhirServerOperation> operations = new Dictionary<string, FhirServerOperation>();

        if (resource.Interaction != null)
        {
            foreach (fhirModels.ConformanceRestResourceInteraction interaction in resource.Interaction)
            {
                if (string.IsNullOrEmpty(interaction.Code))
                {
                    continue;
                }

                interactions.Add(interaction.Code);
            }
        }

        if (resource.SearchParam != null)
        {
            foreach (fhirModels.ConformanceRestResourceSearchParam sp in resource.SearchParam)
            {
                if (searchParams.ContainsKey(sp.Name))
                {
                    continue;
                }

                searchParams.Add(
                    sp.Name,
                    new FhirServerSearchParam(
                        sp.Name,
                        sp.Definition,
                        sp.Type,
                        sp.Documentation));
            }
        }

        return new FhirServerResourceInfo(
            interactions,
            resource.Type,
            null,
            resource.Versioning,
            resource.ReadHistory,
            resource.UpdateCreate,
            resource.ConditionalCreate,
            null,
            resource.ConditionalUpdate,
            resource.ConditionalDelete,
            null,
            resource.SearchInclude,
            resource.SearchRevInclude,
            searchParams,
            operations);
    }

    /// <summary>Gets default value if present.</summary>
    /// <param name="element">     The element.</param>
    /// <param name="defaultName"> [out] The default name.</param>
    /// <param name="defaultValue">[out] The default value.</param>
    private static void GetDefaultValueIfPresent(
        fhirModels.ElementDefinition element,
        out string defaultName,
        out object defaultValue)
    {
        if (element.DefaultValueBase64Binary != null)
        {
            defaultName = "defaultValueBase64Binary";
            defaultValue = element.DefaultValueBase64Binary;
            return;
        }

        if (element.DefaultValueCode != null)
        {
            defaultName = "defaultValueCode";
            defaultValue = element.DefaultValueCode;
            return;
        }

        if (element.DefaultValueDate != null)
        {
            defaultName = "defaultValueDate";
            defaultValue = element.DefaultValueDate;
            return;
        }

        if (element.DefaultValueDateTime != null)
        {
            defaultName = "defaultValueDateTime";
            defaultValue = element.DefaultValueDateTime;
            return;
        }

        if (element.DefaultValueDecimal != null)
        {
            defaultName = "defaultValueDecimal";
            defaultValue = element.DefaultValueDecimal;
            return;
        }

        if (element.DefaultValueId != null)
        {
            defaultName = "defaultValueId";
            defaultValue = element.DefaultValueId;
            return;
        }

        if (element.DefaultValueInstant != null)
        {
            defaultName = "defaultValueInstant";
            defaultValue = element.DefaultValueInstant;
            return;
        }

        if (element.DefaultValueInteger != null)
        {
            defaultName = "defaultValueInteger";
            defaultValue = element.DefaultValueInteger;
            return;
        }

        if (element.DefaultValueOid != null)
        {
            defaultName = "defaultValueOid";
            defaultValue = element.DefaultValueOid;
            return;
        }

        if (element.DefaultValuePositiveInt != null)
        {
            defaultName = "defaultValuePositiveInt";
            defaultValue = element.DefaultValuePositiveInt;
            return;
        }

        if (element.DefaultValueString != null)
        {
            defaultName = "defaultValueString";
            defaultValue = element.DefaultValueString;
            return;
        }

        if (element.DefaultValueTime != null)
        {
            defaultName = "defaultValueTime";
            defaultValue = element.DefaultValueTime;
            return;
        }

        if (element.DefaultValueUnsignedInt != null)
        {
            defaultName = "defaultValueUnsignedInt";
            defaultValue = element.DefaultValueUnsignedInt;
            return;
        }

        if (element.DefaultValueUri != null)
        {
            defaultName = "defaultValueUri";
            defaultValue = element.DefaultValueUri;
            return;
        }

        defaultName = string.Empty;
        defaultValue = null;
    }

    /// <summary>Gets fixed value if present.</summary>
    /// <param name="element">   The element.</param>
    /// <param name="fixedName"> [out] Name of the fixed.</param>
    /// <param name="fixedValue">[out] The fixed value.</param>
    private static void GetFixedValueIfPresent(
        fhirModels.ElementDefinition element,
        out string fixedName,
        out object fixedValue)
    {
        if (element.FixedBase64Binary != null)
        {
            fixedName = "fixedValueBase64Binary";
            fixedValue = element.FixedBase64Binary;
            return;
        }

        if (element.FixedCode != null)
        {
            fixedName = "fixedValueCode";
            fixedValue = element.FixedCode;
            return;
        }

        if (element.FixedDate != null)
        {
            fixedName = "fixedValueDate";
            fixedValue = element.FixedDate;
            return;
        }

        if (element.FixedDateTime != null)
        {
            fixedName = "fixedValueDateTime";
            fixedValue = element.FixedDateTime;
            return;
        }

        if (element.FixedDecimal != null)
        {
            fixedName = "fixedValueDecimal";
            fixedValue = element.FixedDecimal;
            return;
        }

        if (element.FixedId != null)
        {
            fixedName = "fixedValueId";
            fixedValue = element.FixedId;
            return;
        }

        if (element.FixedInstant != null)
        {
            fixedName = "fixedValueInstant";
            fixedValue = element.FixedInstant;
            return;
        }

        if (element.FixedInteger != null)
        {
            fixedName = "fixedValueInteger";
            fixedValue = element.FixedInteger;
            return;
        }

        if (element.FixedOid != null)
        {
            fixedName = "fixedValueOid";
            fixedValue = element.FixedOid;
            return;
        }

        if (element.FixedPositiveInt != null)
        {
            fixedName = "fixedValuePositiveInt";
            fixedValue = element.FixedPositiveInt;
            return;
        }

        if (element.FixedString != null)
        {
            fixedName = "fixedValueString";
            fixedValue = element.FixedString;
            return;
        }

        if (element.FixedTime != null)
        {
            fixedName = "fixedValueTime";
            fixedValue = element.FixedTime;
            return;
        }

        if (element.FixedUnsignedInt != null)
        {
            fixedName = "fixedValueUnsignedInt";
            fixedValue = element.FixedUnsignedInt;
            return;
        }

        if (element.FixedUri != null)
        {
            fixedName = "fixedValueUri";
            fixedValue = element.FixedUri;
            return;
        }

        fixedName = string.Empty;
        fixedValue = null;
    }
}
