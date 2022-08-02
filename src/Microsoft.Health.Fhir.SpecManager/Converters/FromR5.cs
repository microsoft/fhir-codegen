// <copyright file="FromR5.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System.IO;
using Microsoft.Health.Fhir.SpecManager.Manager;
using Microsoft.Health.Fhir.SpecManager.Models;
using Newtonsoft.Json;
using fhirModels = fhirCsR5.Models;
using fhirSerialization = fhirCsR5.Serialization;

namespace Microsoft.Health.Fhir.SpecManager.Converters
{
    /// <summary>Convert FHIR R5 into local definitions.</summary>
    public sealed class FromR5 : IFhirConverter
    {
        private const string ExtensionComment = "There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.";
        private const string ExtensionDefinition = "May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.";
        private const string ExtensionShort = "Additional content defined by implementations";

        /// <summary>The errors.</summary>
        private static List<string> _errors;

        /// <summary>The warnings.</summary>
        private static List<string> _warnings;

        /// <summary>
        /// Initializes a new instance of the <see cref="FromR5"/> class.
        /// </summary>
        public FromR5()
        {
            // _jsonConverter = new fhirModels.ResourceConverter();
            _errors = new List<string>();
            _warnings = new List<string>();
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
        private static void ProcessValueSet(
            fhirModels.ValueSet vs,
            IPackageImportable fhirVersionInfo)
        {
            if (string.IsNullOrEmpty(vs.Status))
            {
                vs.Status = "unknown";
            }

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

                        if (param.ValueDateTime != null)
                        {
                            parameters.Add(param.Name, param.ValueDateTime);
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

            if (string.IsNullOrEmpty(vs.Url))
            {
                throw new Exception($"Cannot index ValueSet: {vs.Name} version: {vs.Version}");
            }

            if (string.IsNullOrEmpty(vs.Version))
            {
                throw new Exception($"Cannot index ValueSet: {vs.Url} version: {vs.Version}");
            }

            FhirValueSet valueSet = new FhirValueSet(
                vs.Name,
                vs.Id,
                vs.Version,
                vs.Title,
                vs.Url,
                vs.Status,
                vs.Description,
                includes,
                excludes,
                expansion);

            // add our code system
            fhirVersionInfo.AddValueSet(valueSet);
        }

        /// <summary>Query if 'includes' is expandable.</summary>
        /// <param name="includes">The includes.</param>
        /// <returns>True if expandable, false if not.</returns>
        private static bool IsExpandable(List<FhirValueSetComposition> includes)
        {
            if ((includes == null) || (includes.Count == 0))
            {
                return false;
            }

            foreach (FhirValueSetComposition comp in includes)
            {
                if (comp.System != null)
                {
                    if (comp.System.StartsWith("http://hl7.org/fhir/", StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }

                    if (comp.System.StartsWith("http://terminology.hl7.org/CodeSystem/", StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }

                if ((comp.LinkedValueSets != null) &&
                    (comp.LinkedValueSets.Count > 0))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>Adds a set of contains clauses to a value set expansion.</summary>
        /// <param name="contains">[in,out] The contains.</param>
        /// <param name="ec">      The ec.</param>
        private static void AddContains(ref List<FhirConcept> contains, fhirModels.ValueSetExpansionContains ec)
        {
            if (contains == null)
            {
                contains = new List<FhirConcept>();
            }

            FhirConcept fhirConcept = new FhirConcept(
                ec.System,
                ec.Code,
                ec.Display,
                ec.Version,
                string.Empty,
                string.Empty);

            if (ec.Property != null)
            {
                foreach (fhirModels.ValueSetExpansionContainsProperty prop in ec.Property)
                {
                    if (string.IsNullOrEmpty(prop.Code))
                    {
                        continue;
                    }

                    if (!string.IsNullOrEmpty(prop.ValueCode))
                    {
                        fhirConcept.AddProperty(prop.Code, prop.ValueCode, prop.ValueCode);
                        continue;
                    }

                    if (prop.ValueCoding != null)
                    {
                        fhirConcept.AddProperty(
                            prop.Code,
                            prop.ValueCoding,
                            FhirConcept.GetCanonical(
                                prop.ValueCoding.System,
                                prop.ValueCoding.Code,
                                prop.ValueCoding.Version));
                        continue;
                    }

                    if (!string.IsNullOrEmpty(prop.ValueString))
                    {
                        fhirConcept.AddProperty(prop.Code, prop.ValueString, prop.ValueString);
                        continue;
                    }

                    if (prop.ValueInteger != null)
                    {
                        fhirConcept.AddProperty(prop.Code, prop.ValueInteger, prop.ValueInteger?.ToString() ?? string.Empty);
                        continue;
                    }

                    if (prop.ValueBoolean != null)
                    {
                        fhirConcept.AddProperty(prop.Code, prop.ValueBoolean, prop.ValueBoolean?.ToString() ?? string.Empty);
                        continue;
                    }

                    if (!string.IsNullOrEmpty(prop.ValueDateTime))
                    {
                        fhirConcept.AddProperty(prop.Code, prop.ValueDateTime, prop.ValueDateTime);
                        continue;
                    }

                    if (prop.ValueDecimal != null)
                    {
                        fhirConcept.AddProperty(prop.Code, prop.ValueDecimal, prop.ValueDecimal?.ToString() ?? string.Empty);
                        continue;
                    }
                }
            }

            // TODO: Determine if the Inactive flag needs to be checked
            if ((!string.IsNullOrEmpty(ec.System)) ||
                (!string.IsNullOrEmpty(ec.Code)))
            {
                contains.Add(fhirConcept);
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

            if ((compose.ValueSet != null) && (compose.ValueSet.Count > 0))
            {
                linkedValueSets = new List<string>();

                foreach (string valueSet in compose.ValueSet)
                {
                    if (string.IsNullOrEmpty(valueSet))
                    {
                        continue;
                    }

                    linkedValueSets.Add(valueSet);
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
            fhirModels.CodeSystem cs,
            IPackageImportable fhirVersionInfo)
        {
            // TODO: Patch for 4.6.0
            if (string.IsNullOrEmpty(cs.Status))
            {
                cs.Status = "unknown";
                _errors.Add($"CodeSystem {cs.Name} ({cs.Id}): Status field missing");
            }

            // ignore retired
            if (cs.Status.Equals("retired", StringComparison.Ordinal))
            {
                return;
            }

            Dictionary<string, FhirCodeSystem.FilterDefinition> filters = new();

            if (cs.Filter != null)
            {
                foreach (fhirModels.CodeSystemFilter filter in cs.Filter)
                {
                    filters.Add(
                        filter.Code,
                        new(
                            filter.Code,
                            filter.Description,
                            filter.Operator,
                            filter.Value));
                }
            }

            Dictionary<string, FhirCodeSystem.PropertyDefinition> properties = new();

            if (cs.Property != null)
            {
                foreach (fhirModels.CodeSystemProperty prop in cs.Property)
                {
                    if (properties.ContainsKey(prop.Code))
                    {
                        _warnings.Add($"CodeSystem {cs.Name} ({cs.Id}): Duplicate proprety found: {prop.Code}");
                        continue;
                    }

                    properties.Add(
                        prop.Code,
                        new(
                            prop.Code,
                            prop.Uri,
                            prop.Description,
                            FhirCodeSystem.PropertyTypeFromValue(prop.Type)));
                }
            }

            Dictionary<string, FhirConceptTreeNode> nodeLookup = new Dictionary<string, FhirConceptTreeNode>();
            FhirConceptTreeNode root = new FhirConceptTreeNode(null, null);

            if (cs.Concept != null)
            {
                AddConceptTree(cs.Url, cs.Id, cs.Concept, root, nodeLookup, properties);
            }

            FhirCodeSystem codeSystem = new FhirCodeSystem(
                cs.Name,
                cs.Id,
                cs.Version,
                cs.Title,
                cs.Url,
                cs.Status,
                cs.Description,
                cs.Content,
                root,
                nodeLookup,
                filters,
                properties);

            // add our code system
            fhirVersionInfo.AddCodeSystem(codeSystem);
        }

        /// <summary>Attempts to build internal concept from FHIR.</summary>
        /// <param name="codeSystemUrl">      URL of the code system.</param>
        /// <param name="codeSystemId">       Id of the code system.</param>
        /// <param name="concept">            The concept.</param>
        /// <param name="propertyDefinitions">The property definitions.</param>
        /// <param name="fhirConcept">        [out] The FHIR concept.</param>
        /// <param name="nodeLookup">         (Optional) The node lookup.</param>
        /// <returns>True if it succeeds, false if it fails.</returns>
        private bool TryBuildInternalConceptFromFhir(
            string codeSystemUrl,
            string codeSystemId,
            fhirModels.CodeSystemConcept concept,
            Dictionary<string, FhirCodeSystem.PropertyDefinition> propertyDefinitions,
            out FhirConcept fhirConcept,
            Dictionary<string, FhirConceptTreeNode> nodeLookup = null)
        {
            if (string.IsNullOrEmpty(concept.Code))
            {
                fhirConcept = null;
                return false;
            }

            if ((nodeLookup != null) &&
                nodeLookup.ContainsKey(concept.Code))
            {
                fhirConcept = null;
                return false;
            }

            fhirConcept = new FhirConcept(
                codeSystemUrl,
                concept.Code,
                concept.Display,
                string.Empty,
                concept.Definition,
                codeSystemId);

            if (concept.Property != null)
            {
                foreach (fhirModels.CodeSystemConceptProperty prop in concept.Property)
                {
                    if (string.IsNullOrEmpty(prop.Code) ||
                        (!propertyDefinitions.ContainsKey(prop.Code)))
                    {
                        continue;
                    }

                    if ((prop.Code == "status") && (prop.ValueCode == "deprecated"))
                    {
                        fhirConcept = null;
                        return false;
                    }

                    switch (propertyDefinitions[prop.Code].PropType)
                    {
                        case FhirCodeSystem.PropertyTypeEnum.Code:
                            fhirConcept.AddProperty(prop.Code, prop.ValueCode, prop.ValueCode);
                            break;

                        case FhirCodeSystem.PropertyTypeEnum.Coding:
                            fhirConcept.AddProperty(
                                prop.Code,
                                prop.ValueCoding,
                                FhirConcept.GetCanonical(
                                    prop.ValueCoding.System,
                                    prop.ValueCoding.Code,
                                    prop.ValueCoding.Version));
                            break;

                        case FhirCodeSystem.PropertyTypeEnum.String:
                            fhirConcept.AddProperty(prop.Code, prop.ValueString, prop.ValueString);
                            break;

                        case FhirCodeSystem.PropertyTypeEnum.Integer:
                            fhirConcept.AddProperty(prop.Code, prop.ValueInteger, prop.ValueInteger?.ToString() ?? string.Empty);
                            break;

                        case FhirCodeSystem.PropertyTypeEnum.Boolean:
                            fhirConcept.AddProperty(prop.Code, prop.ValueBoolean, prop.ValueBoolean?.ToString() ?? string.Empty);
                            break;

                        case FhirCodeSystem.PropertyTypeEnum.DateTime:
                            fhirConcept.AddProperty(prop.Code, prop.ValueDateTime, prop.ValueDateTime);
                            break;

                        case FhirCodeSystem.PropertyTypeEnum.Decimal:
                            fhirConcept.AddProperty(prop.Code, prop.ValueDecimal, prop.ValueDecimal?.ToString() ?? string.Empty);
                            break;
                    }
                }
            }

            return true;
        }

        /// <summary>Adds a concept tree to 'concepts'.</summary>
        /// <param name="codeSystemUrl">      URL of the code system.</param>
        /// <param name="codeSystemId">       Id of the code system.</param>
        /// <param name="concepts">           The concept.</param>
        /// <param name="parent">             The parent.</param>
        /// <param name="nodeLookup">         The node lookup.</param>
        /// <param name="propertyDefinitions">The property definitions.</param>
        private void AddConceptTree(
            string codeSystemUrl,
            string codeSystemId,
            List<fhirModels.CodeSystemConcept> concepts,
            FhirConceptTreeNode parent,
            Dictionary<string, FhirConceptTreeNode> nodeLookup,
            Dictionary<string, FhirCodeSystem.PropertyDefinition> propertyDefinitions)
        {
            if ((concepts == null) ||
                (concepts.Count == 0) ||
                (parent == null))
            {
                return;
            }

            List<KeyValuePair<string, string>> properties = new List<KeyValuePair<string, string>>();

            foreach (fhirModels.CodeSystemConcept concept in concepts)
            {
                if (TryBuildInternalConceptFromFhir(
                        codeSystemUrl,
                        codeSystemId,
                        concept,
                        propertyDefinitions,
                        out FhirConcept fhirConcept,
                        nodeLookup))
                {
                    FhirConceptTreeNode node = parent.AddChild(fhirConcept);

                    if (concept.Concept != null)
                    {
                        AddConceptTree(codeSystemUrl, codeSystemId, concept.Concept, node, nodeLookup, propertyDefinitions);
                    }

                    nodeLookup.Add(concept.Code, node);
                }
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
                        opParam.Min,
                        opParam.Max,
                        opParam.Documentation,
                        opParam.Type,
                        parameters.Count));
                }
            }

            // create the operation
            FhirOperation operation = new FhirOperation(
                op.Id,
                new Uri(op.Url),
                op.Version,
                op.Name,
                op.Description,
                op.System,
                op.Type,
                op.Instance,
                op.Code,
                op.Comment,
                op.Resource,
                parameters,
                op.Experimental == true);

            // add our operation
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
            if (sp.Status.Equals("retired", StringComparison.Ordinal))
            {
                return;
            }

            List<string> resources = sp.Base;

            // check for parameters with no base resource
            if (sp.Base == null)
            {
                resources = new List<string>();

                // see if we can determine the resource based on id
                string[] components = sp.Id.Split('-');

                foreach (string component in components)
                {
                    if (fhirVersionInfo.Resources.ContainsKey(component))
                    {
                        resources.Add(component);
                    }
                }

                // don't know where to put this, could try parsing XPath in the future
                if (resources.Count == 0)
                {
                    return;
                }
            }

            // create the search parameter
            FhirSearchParam param = new FhirSearchParam(
                sp.Id,
                new Uri(sp.Url),
                sp.Version,
                sp.Name,
                sp.Description,
                sp.Purpose,
                sp.Code,
                resources,
                sp.Target,
                sp.Type,
                sp.Status,
                sp.Experimental == true,
                sp.Xpath,
                sp.XpathUsage,
                sp.Expression);

            // add our parameter
            fhirVersionInfo.AddSearchParameter(param);
        }

        /// <summary>Process the structure definition.</summary>
        /// <param name="sd">             The structure definition we are parsing.</param>
        /// <param name="fhirVersionInfo">FHIR Version information.</param>
        private void ProcessStructureDef(
            fhirModels.StructureDefinition sd,
            IPackageImportable fhirVersionInfo)
        {
            // ignore retired
            if (sd.Status == "retired")
            {
                return;
            }

            // act depending on kind
            switch (sd.Kind)
            {
                case "primitive-type":
                    ProcessDataTypePrimitive(sd, fhirVersionInfo);
                    break;

                case "logical":
                    ProcessComplex(sd, fhirVersionInfo, FhirArtifactClassEnum.LogicalModel);
                    break;

                case "resource":
                case "complex-type":
                    if (sd.Derivation == "constraint")
                    {
                        if (sd.Type == "Extension")
                        {
                            ProcessComplex(sd, fhirVersionInfo, FhirArtifactClassEnum.Extension);
                        }
                        else
                        {
                            ProcessComplex(sd, fhirVersionInfo, FhirArtifactClassEnum.Profile);
                        }
                    }
                    else
                    {
                        ProcessComplex(
                            sd,
                            fhirVersionInfo,
                            sd.Kind == "complex-type" ? FhirArtifactClassEnum.ComplexType : FhirArtifactClassEnum.Resource);
                    }

                    break;
            }
        }

        /// <summary>Process a structure definition for a Primitive data type.</summary>
        /// <param name="sd">             The structure definition.</param>
        /// <param name="fhirVersionInfo">FHIR Version information.</param>
        private static void ProcessDataTypePrimitive(
            fhirModels.StructureDefinition sd,
            IPackageImportable fhirVersionInfo)
        {
            string regex = string.Empty;
            string descriptionShort = sd.Description;
            string definition = sd.Purpose;
            string comment = string.Empty;
            string baseTypeName = string.Empty;

#if false   // right now, differential is generally 'more correct' than snapshot, see FHIR-37465
            if ((sd.Snapshot != null) &&
                (sd.Snapshot.Element != null) &&
                (sd.Snapshot.Element.Count > 0))
            {
                foreach (fhirModels.ElementDefinition element in sd.Snapshot.Element)
                {
                    if (element.Id == sd.Id)
                    {
                        descriptionShort = element.Short;
                        definition = element.Definition;
                        comment = element.Comment;
                        continue;
                    }

                    if (element.Id != $"{sd.Id}.value")
                    {
                        continue;
                    }

                    if (element.Type == null)
                    {
                        continue;
                    }

                    foreach (fhirModels.ElementDefinitionType type in element.Type)
                    {
                        if (!string.IsNullOrEmpty(type.Code))
                        {
                            if (FhirElementType.IsFhirPathType(type.Code, out string fhirType))
                            {
                                baseTypeName = fhirType;
                            }
                            else if (FhirElementType.IsXmlBaseType(type.Code, out string xmlFhirType))
                            {
                                baseTypeName = xmlFhirType;
                            }
                        }

                        if (type.Extension == null)
                        {
                            continue;
                        }

                        foreach (fhirModels.Extension ext in type.Extension)
                        {
                            if (ext.Url == "http://hl7.org/fhir/StructureDefinition/regex")
                            {
                                regex = ext.ValueString;
                                break;
                            }
                        }
                    }
                }
            }
#endif

            // right now, differential is generally 'more correct' than snapshot, see FHIR-37465
            if ((sd.Differential != null) &&
                (sd.Differential.Element != null) &&
                (sd.Differential.Element.Count > 0))
            {
                foreach (fhirModels.ElementDefinition element in sd.Differential.Element)
                {
                    if (element.Id == sd.Id)
                    {
                        descriptionShort = element.Short;
                        definition = element.Definition;
                        comment = element.Comment;
                        continue;
                    }

                    if (element.Id != $"{sd.Id}.value")
                    {
                        continue;
                    }

                    if (element.Type == null)
                    {
                        continue;
                    }

                    foreach (fhirModels.ElementDefinitionType type in element.Type)
                    {
                        if (!string.IsNullOrEmpty(type.Code))
                        {
                            if (FhirElementType.IsFhirPathType(type.Code, out string fhirType))
                            {
                                baseTypeName = fhirType;
                            }
                            else if (FhirElementType.IsXmlBaseType(type.Code, out string xmlFhirType))
                            {
                                baseTypeName = xmlFhirType;
                            }
                        }

                        if (type.Extension == null)
                        {
                            continue;
                        }

                        foreach (fhirModels.Extension ext in type.Extension)
                        {
                            if (ext.Url == "http://hl7.org/fhir/StructureDefinition/regex")
                            {
                                regex = ext.ValueString;
                                break;
                            }
                        }
                    }
                }
            }

            if (string.IsNullOrEmpty(baseTypeName))
            {
                baseTypeName = sd.Name;
            }

            // create a new primitive type object
            FhirPrimitive primitive = new FhirPrimitive(
                sd.Id,
                sd.Name,
                baseTypeName,
                new Uri(sd.Url),
                sd.Status,
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
        /// <param name="elementTypes"> [out] Type of the element.</param>
        /// <param name="regex">        [out] The RegEx.</param>
        /// <param name="isSimple">     [out] True if is simple, false if not.</param>
        /// <returns>True if it succeeds, false if it fails.</returns>
        private static bool TryGetTypeFromElement(
            string structureName,
            fhirModels.ElementDefinition element,
            out Dictionary<string, FhirElementType> elementTypes,
            out string regex,
            out bool isSimple)
        {
            elementTypes = new Dictionary<string, FhirElementType>();
            regex = string.Empty;
            isSimple = false;

            // TODO(ginoc): 5.0.0-snapshot1 needs these fixed
            switch (element.Path)
            {
                case "ArtifactAssessment.approvalDate":
                case "ArtifactAssessment.lastReviewDate":
                    elementTypes.Add("date", new FhirElementType("date"));
                    _warnings.Add($"StructureDefinition - {structureName} coerced {element.Id} to type 'date'");
                    return true;
            }

            // check for declared type
            if (element.Type != null)
            {
                string fType;

                foreach (fhirModels.ElementDefinitionType edType in element.Type)
                {
                    regex = edType.Extension?
                        .FirstOrDefault((ext) => ext.Url.Equals("http://hl7.org/fhir/StructureDefinition/regex", StringComparison.Ordinal), new())
                        .ValueString ?? string.Empty;

                    fType = edType.Extension?
                        .FirstOrDefault((ext) => ext.Url.Equals("http://hl7.org/fhir/StructureDefinition/structuredefinition-fhir-type", StringComparison.Ordinal), new())
                        .ValueString ?? string.Empty;

                    if (!string.IsNullOrEmpty(fType))
                    {
                        // create a type for this code
                        FhirElementType elementType = new FhirElementType(
                            fType,
                            edType.TargetProfile,
                            edType.Profile);

                        isSimple = true;

                        // add to our dictionary
                        elementTypes.Add(elementType.Name, elementType);
                    }
                    else if (!string.IsNullOrEmpty(edType.Code))
                    {
                        // create a type for this code
                        FhirElementType elementType = new FhirElementType(
                            edType.Code,
                            edType.TargetProfile,
                            edType.Profile);

                        // add to our dictionary
                        elementTypes.Add(elementType.Name, elementType);
                    }
                }
            }

            if (elementTypes.Count > 0)
            {
                return true;
            }

            // check for base derived type
            if (string.IsNullOrEmpty(element.Id) ||
                element.Id.Equals(structureName, StringComparison.Ordinal))
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
        /// <param name="elementTypes"> [out] Type of the element.</param>
        /// <param name="regex">        [out] The RegEx.</param>
        /// <param name="isSimple">     [out] True if is simple, false if not.</param>
        /// <returns>True if it succeeds, false if it fails.</returns>
        private static bool TryGetTypeFromElements(
            string structureName,
            List<fhirModels.ElementDefinition> elements,
            out Dictionary<string, FhirElementType> elementTypes,
            out string regex,
            out bool isSimple)
        {
            elementTypes = null;
            regex = string.Empty;
            isSimple = false;

            foreach (fhirModels.ElementDefinition element in elements)
            {
                // split the path
                string[] components = element.Path.Split('.');

                // check for base path having a type
                if (components.Length == 1)
                {
                    if (TryGetTypeFromElement(structureName, element, out elementTypes, out regex, out isSimple))
                    {
                        // done searching
                        return true;
                    }
                }

                // check for path {type}.value having a type
                if ((components.Length == 2) &&
                    components[1].Equals("value", StringComparison.Ordinal))
                {
                    if (TryGetTypeFromElement(structureName, element, out elementTypes, out regex, out isSimple))
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
        /// <param name="sd">             The structure definition to parse.</param>
        /// <param name="fhirVersionInfo">FHIR Version information.</param>
        /// <param name="artifactClass">  Type of structure definition we are parsing.</param>
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
            string definition = sd.Purpose;

            try
            {
                List<string> contextElements = new List<string>();
                if (sd.Context != null)
                {
                    foreach (fhirModels.StructureDefinitionContext context in sd.Context)
                    {
                        if (context.Type != "element")
                        {
                            // throw new ArgumentException($"Invalid extension context type: {context.Type}");
                            _errors.Add($"StructureDefinition {sd.Name} ({sd.Id}) unhandled context type: {context.Type}");
                            return;
                        }

                        contextElements.Add(context.Expression);
                    }
                }

                if (sd.Snapshot.Element.Count > 0)
                {
                    descriptionShort = sd.Snapshot.Element[0].Short;
                    definition = sd.Snapshot.Element[0].Definition;
                }

                // create a new complex type object for this type or resource
                FhirComplex complex = new FhirComplex(
                    sd.Id,
                    sd.Name,
                    string.Empty,
                    sd.Type,
                    new Uri(sd.Url),
                    sd.Status,
                    sd.Experimental == true,
                    descriptionShort,
                    definition,
                    string.Empty,
                    null,
                    contextElements,
                    sd.Abstract);

                // check for a base definition
                if (!string.IsNullOrEmpty(sd.BaseDefinition))
                {
                    complex.BaseTypeName = sd.BaseDefinition.Substring(sd.BaseDefinition.LastIndexOf('/') + 1);
                }
                else
                {
                    if (!TryGetTypeFromElements(
                            sd.Name,
                            sd.Snapshot.Element,
                            out Dictionary<string, FhirElementType> baseTypes,
                            out string _,
                            out bool _))
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

                // look for properties on this type
                foreach (fhirModels.ElementDefinition element in sd.Snapshot.Element)
                {
                    try
                    {
                        string id = element.Id ?? element.Path;
                        string path = element.Path ?? element.Id;
                        Dictionary<string, FhirElementType> elementTypes = null;
                        string elementType = string.Empty;
                        string regex = string.Empty;
                        bool isRootElement = false;
                        bool isSimple = false;

                        // split the id into component parts
                        string[] idComponents = id.Split('.');
                        string[] pathComponents = path.Split('.');

                        // base definition, already processed
                        if (pathComponents.Length < 2)
                        {
                            // check for this component being different from primar
                            if ((pathComponents[0] != sd.Name) && (contextElements.Count == 0))
                            {
                                // add to our context
                                complex.AddContextElement(pathComponents[0]);
                            }

                            // parse as root element
                            isRootElement = true;
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
                                        element.IsModifierReason,
                                        element.IsSummary,
                                        element.MustSupport,
                                        false,
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
                        if (!TryGetTypeFromElement(parent.Name, element, out elementTypes, out regex, out isSimple))
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
                        if (field.Contains("[x]", StringComparison.Ordinal))
                        {
                            // fix the field and path names
                            id = id.Replace("[x]", string.Empty, StringComparison.Ordinal);
                            field = field.Replace("[x]", string.Empty, StringComparison.Ordinal);

                            // force no base type
                            elementType = string.Empty;
                        }
                        else if (!string.IsNullOrEmpty(element.ContentReference))
                        {
                            if (element.ContentReference.StartsWith("http://hl7.org/fhir/StructureDefinition/", StringComparison.OrdinalIgnoreCase))
                            {
                                int loc = element.ContentReference.IndexOf('#', StringComparison.Ordinal);
                                elementType = element.ContentReference.Substring(loc + 1);
                            }
                            else if (element.ContentReference[0] == '#')
                            {
                                // use the local reference
                                elementType = element.ContentReference.Substring(1);
                            }
                            else
                            {
                                throw new InvalidDataException($"Could not resolve ContentReference {element.ContentReference} in {sd.Name} field {element.Path}");
                            }
                        }

                        // get default values (if present)
                        GetDefaultValueIfPresent(element, out string defaultName, out object defaultValue);

                        // get fixed values (if present)
                        GetFixedValueIfPresent(element, out string fixedName, out object fixedValue);

                        // determine if this element is inherited
                        bool isInherited = false;
                        bool modifiesParent = true;

                        if (!element.Path.StartsWith(complex.Name, StringComparison.Ordinal))
                        {
                            isInherited = true;
                        }

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

                        string bindingStrength = string.Empty;
                        string valueSet = string.Empty;

                        if (element.Binding != null)
                        {
                            bindingStrength = element.Binding.Strength;
                            valueSet = element.Binding.ValueSet;
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

                        if (parent.Elements.ContainsKey(path))
                        {
                            _errors.Add($"Complex {sd.Name} snapshot error ({path}): Repeated snapshot: {parent.Elements[path].Id} & {id}");
                            continue;
                        }

                        FhirElement fhirElement = new FhirElement(
                            id,
                            path,
                            explicitName,
                            null,
                            parent.Elements.Count,
                            element.Short,
                            element.Definition,
                            element.Comment,
                            regex,
                            elementType,
                            elementTypes,
                            (int)(element.Min ?? 0),
                            element.Max,
                            element.IsModifier,
                            element.IsModifierReason,
                            element.IsSummary,
                            element.MustSupport,
                            isSimple,
                            defaultName,
                            defaultValue,
                            fixedName,
                            fixedValue,
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
                        else
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

                            foreach (fhirModels.ElementDefinitionSlicingDiscriminator discriminator in element.Slicing.Discriminator)
                            {
                                discriminatorRules.Add(new FhirSliceDiscriminatorRule(
                                    discriminator.Type,
                                    discriminator.Path));
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
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(string.Empty);
                        Console.WriteLine($"FromR5.ProcessComplex <<< element: {element.Path} ({element.Id}) - exception: {ex.Message}");
                        throw;
                    }
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
                                con.Expression,
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

                            if ((!string.IsNullOrEmpty(dif.SliceName)) &&
                                (complex.Elements[dif.Path].Slicing != null) &&
                                complex.Elements[dif.Path].Slicing.ContainsKey(sd.Url) &&
                                complex.Elements[dif.Path].Slicing[sd.Url].HasSlice(dif.SliceName))
                            {
                                complex.Elements[dif.Path].Slicing[sd.Url].SetInDifferential(dif.SliceName);
                            }
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
                    case FhirArtifactClassEnum.LogicalModel:
                        fhirVersionInfo.AddLogicalModel(complex);
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Empty);
                Console.WriteLine($"FromR5.ProcessComplex <<< SD: {sd.Name} ({sd.Id}) - exception: {ex.Message}");
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
                return System.Text.Json.JsonSerializer.Deserialize<fhirModels.Resource>(
                    json,
                    fhirSerialization.FhirSerializerOptions.Compact);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"FromR5.ParseResource <<< failed to parse:\n{ex}\n------------------------------------");
                throw;
            }
        }

        /// <summary>Attempts to process resource.</summary>
        /// <param name="resourceToParse">[out] The resource object.</param>
        /// <param name="fhirVersionInfo">FHIR Version information.</param>
        void IFhirConverter.ProcessResource(
            object resourceToParse,
            IPackageImportable fhirVersionInfo)
        {
            try
            {
                switch (resourceToParse)
                {
                    // ignore

                    // case fhirModels.CapabilityStatement capabilityStatement:
                    // case fhirModels.CompartmentDefinition compartmentDefinition:
                    // case fhirModels.ConceptMap conceptMap:
                    // case fhirModels.NamingSystem namingSystem:
                    // case fhirModels.StructureMap structureMap:

                    // process
                    case fhirModels.CodeSystem codeSystem:
                        ProcessCodeSystem(codeSystem, fhirVersionInfo);
                        break;

                    case fhirModels.OperationDefinition operationDefinition:
                        ProcessOperation(operationDefinition, fhirVersionInfo);
                        break;

                    case fhirModels.SearchParameter searchParameter:
                        ProcessSearchParam(searchParameter, fhirVersionInfo);
                        break;

                    case fhirModels.StructureDefinition structureDefinition:
                        ProcessStructureDef(structureDefinition, fhirVersionInfo);
                        break;

                    case fhirModels.ValueSet valueSet:
                        ProcessValueSet(valueSet, fhirVersionInfo);
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"FromR5.TryProcessResource <<< Failed to process resource:\n{ex}\n--------------");
                throw;
            }
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

            fhirModels.CapabilityStatement caps = metadata as fhirModels.CapabilityStatement;

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
                fhirModels.CapabilityStatementRest rest = caps.Rest[0];

                if (rest.Interaction != null)
                {
                    foreach (fhirModels.CapabilityStatementRestInteraction interaction in rest.Interaction)
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
                    foreach (fhirModels.CapabilityStatementRestResource resource in rest.Resource)
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
                    foreach (fhirModels.CapabilityStatementRestResourceSearchParam sp in rest.SearchParam)
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
                    foreach (fhirModels.CapabilityStatementRestResourceOperation operation in rest.Operation)
                    {
                        if (serverOperations.ContainsKey(operation.Name))
                        {
                            serverOperations[operation.Name].AddDefinition(operation.Definition);
                            continue;
                        }

                        serverOperations.Add(
                            operation.Name,
                            new FhirServerOperation(
                                operation.Name,
                                operation.Definition,
                                operation.Documentation));
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
                resourceInteractions,
                serverSearchParams,
                serverOperations);
        }

        /// <summary>Parse server REST resource.</summary>
        /// <param name="resource">The resource.</param>
        /// <returns>A FhirServerResourceInfo.</returns>
        private static FhirServerResourceInfo ParseServerRestResource(
            fhirModels.CapabilityStatementRestResource resource)
        {
            List<string> interactions = new List<string>();
            Dictionary<string, FhirServerSearchParam> searchParams = new Dictionary<string, FhirServerSearchParam>();
            Dictionary<string, FhirServerOperation> operations = new Dictionary<string, FhirServerOperation>();

            if (resource.Interaction != null)
            {
                foreach (fhirModels.CapabilityStatementRestResourceInteraction interaction in resource.Interaction)
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
                foreach (fhirModels.CapabilityStatementRestResourceSearchParam sp in resource.SearchParam)
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

            if (resource.Operation != null)
            {
                foreach (fhirModels.CapabilityStatementRestResourceOperation operation in resource.Operation)
                {
                    if (operations.ContainsKey(operation.Name))
                    {
                        operations[operation.Name].AddDefinition(operation.Definition);
                        continue;
                    }

                    operations.Add(
                        operation.Name,
                        new FhirServerOperation(
                            operation.Name,
                            operation.Definition,
                            operation.Documentation));
                }
            }

            return new FhirServerResourceInfo(
                interactions,
                resource.Type,
                resource.SupportedProfile,
                resource.Versioning,
                resource.ReadHistory,
                resource.UpdateCreate,
                resource.ConditionalCreate,
                resource.ConditionalRead,
                resource.ConditionalUpdate,
                resource.ConditionalDelete,
                resource.ReferencePolicy,
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

            if (element.DefaultValueCanonical != null)
            {
                defaultName = "defaultValueCanonical";
                defaultValue = element.DefaultValueCanonical;
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

            if (element.DefaultValueInteger64 != null)
            {
                defaultName = "defaultValueInteger64";
                defaultValue = element.DefaultValueInteger64;
                return;
            }

            if (element.DefaultValueMarkdown != null)
            {
                defaultName = "defaultValueMarkdown";
                defaultValue = element.DefaultValueMarkdown;
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

            if (element.DefaultValueUrl != null)
            {
                defaultName = "defaultValueUrl";
                defaultValue = element.DefaultValueUrl;
                return;
            }

            if (element.DefaultValueUuid != null)
            {
                defaultName = "defaultValueUuid";
                defaultValue = element.DefaultValueUuid;
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

            if (element.FixedCanonical != null)
            {
                fixedName = "fixedValueCanonical";
                fixedValue = element.FixedCanonical;
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

            if (element.FixedInteger64 != null)
            {
                fixedName = "fixedValueInteger64";
                fixedValue = element.FixedInteger64;
                return;
            }

            if (element.FixedMarkdown != null)
            {
                fixedName = "fixedValueMarkdown";
                fixedValue = element.FixedMarkdown;
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

            if (element.FixedUrl != null)
            {
                fixedName = "fixedValueUrl";
                fixedValue = element.FixedUrl;
                return;
            }

            if (element.FixedUuid != null)
            {
                fixedName = "fixedValueUuid";
                fixedValue = element.FixedUuid;
                return;
            }

            fixedName = string.Empty;
            fixedValue = null;
        }
    }
}
