// -------------------------------------------------------------------------------------------------
// <copyright file="FromR3.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>
// -------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Health.Fhir.SpecManager.Manager;
using Microsoft.Health.Fhir.SpecManager.Models;
using Newtonsoft.Json;
using fhir_3 = Microsoft.Health.Fhir.SpecManager.fhir.r3;

namespace Microsoft.Health.Fhir.SpecManager.Converters
{
    /// <summary>Convert FHIR R3 into local definitions.</summary>
    public sealed class FromR3 : IFhirConverter
    {
        private const string ExtensionComment = "There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.";
        private const string ExtensionDefinition = "May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.";
        private const string ExtensionShort = "Additional content defined by implementations";

        /// <summary>The JSON converter for polymorphic deserialization of this version of FHIR.</summary>
        private JsonConverter _jsonConverter;

        /// <summary>
        /// Initializes a new instance of the <see cref="FromR3"/> class.
        /// </summary>
        public FromR3() => _jsonConverter = new fhir_3.ResourceConverter();

        /// <summary>Process the value set.</summary>
        /// <param name="vs">             The vs.</param>
        /// <param name="fhirVersionInfo">FHIR Version information.</param>
        private void ProcessValueSet(
            fhir_3.ValueSet vs,
            FhirVersionInfo fhirVersionInfo)
        {
            // ignore retired
            if (vs.Status.Equals("retired", StringComparison.Ordinal))
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

                foreach (fhir_3.ValueSetComposeInclude compose in vs.Compose.Include)
                {
                    includes.Add(BuildComposition(compose));
                }
            }

            if ((vs.Compose != null) &&
                (vs.Compose.Exclude != null) &&
                (vs.Compose.Exclude.Count > 0))
            {
                excludes = new List<FhirValueSetComposition>();

                foreach (fhir_3.ValueSetComposeInclude compose in vs.Compose.Exclude)
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

                    foreach (fhir_3.ValueSetExpansionParameter param in vs.Expansion.Parameter)
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
                    foreach (fhir_3.ValueSetExpansionContains contains in vs.Expansion.Contains)
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
                return;
                throw new Exception($"Cannot index ValueSet: {vs.Name} version: {vs.Version}");
            }

            if (string.IsNullOrEmpty(vs.Version))
            {
                return;
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

        /// <summary>Adds the contains to 'ec'.</summary>
        /// <param name="contains">[in,out] The contains.</param>
        /// <param name="ec">      The ec.</param>
        private void AddContains(ref List<FhirConcept> contains, fhir_3.ValueSetExpansionContains ec)
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
                foreach (fhir_3.ValueSetExpansionContains subContains in ec.Contains)
                {
                    AddContains(ref contains, subContains);
                }
            }
        }

        /// <summary>Builds a composition.</summary>
        /// <param name="compose">The compose.</param>
        /// <returns>A FhirValueSetComposition.</returns>
        private static FhirValueSetComposition BuildComposition(fhir_3.ValueSetComposeInclude compose)
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

                foreach (fhir_3.ValueSetComposeIncludeConcept concept in compose.Concept)
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

                foreach (fhir_3.ValueSetComposeIncludeFilter filter in compose.Filter)
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
            fhir_3.CodeSystem cs,
            FhirVersionInfo fhirVersionInfo)
        {
            // ignore retired
            if (cs.Status.Equals("retired", StringComparison.Ordinal))
            {
                return;
            }

            Dictionary<string, FhirConceptTreeNode> nodeLookup = new Dictionary<string, FhirConceptTreeNode>();
            FhirConceptTreeNode root = new FhirConceptTreeNode(null, null);

            if (cs.Concept != null)
            {
                AddConceptTree(cs.Url, cs.Id, cs.Concept, ref root, ref nodeLookup);
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
                nodeLookup);

            // add our code system
            fhirVersionInfo.AddCodeSystem(codeSystem);
        }

        /// <summary>Adds a concept tree to 'concepts'.</summary>
        /// <param name="codeSystemUrl">URL of the code system.</param>
        /// <param name="codeSystemId"> Id of the code system.</param>
        /// <param name="concepts">     The concept.</param>
        /// <param name="parent">       [in,out] The parent.</param>
        /// <param name="nodeLookup">   [in,out] The node lookup.</param>
        private void AddConceptTree(
            string codeSystemUrl,
            string codeSystemId,
            List<fhir_3.CodeSystemConcept> concepts,
            ref FhirConceptTreeNode parent,
            ref Dictionary<string, FhirConceptTreeNode> nodeLookup)
        {
            if ((concepts == null) ||
                (concepts.Count == 0) ||
                (parent == null))
            {
                return;
            }

            foreach (fhir_3.CodeSystemConcept concept in concepts)
            {
                if (concept.Property != null)
                {
                    bool deprecated = false;
                    foreach (fhir_3.CodeSystemConceptProperty prop in concept.Property)
                    {
                        if ((prop.Code == "status") && (prop.ValueCode == "deprecated"))
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
            fhir_3.OperationDefinition op,
            FhirVersionInfo fhirVersionInfo)
        {
            // ignore retired
            if (op.Status.Equals("retired", StringComparison.Ordinal))
            {
                return;
            }

            List<FhirParameter> parameters = new List<FhirParameter>();

            if (op.Parameter != null)
            {
                foreach (fhir_3.OperationDefinitionParameter opParam in op.Parameter)
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
                parameters);

            // add our parameter
            fhirVersionInfo.AddOperation(operation);
        }

        /// <summary>Process the search parameter.</summary>
        /// <param name="sp">             The sp.</param>
        /// <param name="fhirVersionInfo">FHIR Version information.</param>
        private void ProcessSearchParam(
            fhir_3.SearchParameter sp,
            FhirVersionInfo fhirVersionInfo)
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
        /// <param name="sd">             The structure definition to parse.</param>
        /// <param name="fhirVersionInfo">FHIR Version information.</param>
        private static void ProcessStructureDef(
            fhir_3.StructureDefinition sd,
            FhirVersionInfo fhirVersionInfo)
        {
            // ignore retired
            if (sd.Status.Equals("retired", StringComparison.Ordinal))
            {
                return;
            }

            // act depending on kind
            switch (sd.Kind)
            {
                case "primitive-type":
                    ProcessDataTypePrimitive(sd, fhirVersionInfo);
                    break;

                case "complex-type":
                    if ((sd.Derivation == "constraint") &&
                        (sd.Type != "Quantity"))
                    {
                        ProcessComplex(sd, fhirVersionInfo, FhirComplex.FhirComplexType.Extension);
                    }
                    else
                    {
                        ProcessComplex(sd, fhirVersionInfo, FhirComplex.FhirComplexType.DataType);
                    }

                    break;

                case "resource":
                    if ((sd.Derivation == "constraint") &&
                        (sd.Type != "Quantity"))
                    {
                        ProcessComplex(sd, fhirVersionInfo, FhirComplex.FhirComplexType.Extension);
                    }
                    else
                    {
                        ProcessComplex(sd, fhirVersionInfo, FhirComplex.FhirComplexType.Resource);
                    }

                    break;
            }
        }

        /// <summary>Process a structure definition for a Primitve data type.</summary>
        /// <param name="sd">             The structure definition.</param>
        /// <param name="fhirVersionInfo">FHIR Version information.</param>
        private static void ProcessDataTypePrimitive(
            fhir_3.StructureDefinition sd,
            FhirVersionInfo fhirVersionInfo)
        {
            string regex = string.Empty;
            string descriptionShort = sd.Description;
            string definition = sd.Purpose;
            string comment = string.Empty;
            string baseTypeName = string.Empty;

            if ((sd.Snapshot != null) &&
                (sd.Snapshot.Element != null) &&
                (sd.Snapshot.Element.Count > 0))
            {
                foreach (fhir_3.ElementDefinition element in sd.Snapshot.Element)
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

                    foreach (fhir_3.ElementDefinitionType type in element.Type)
                    {
                        if ((type._Code == null) ||
                            (type._Code.Extension == null))
                        {
                            continue;
                        }

                        foreach (fhir_3.Extension ext in type._Code.Extension)
                        {
                            if ((ext.Url == "http://hl7.org/fhir/StructureDefinition/structuredefinition-xml-type") &&
                                FhirElementType.IsXmlType(ext.ValueString, out string fhirType))
                            {
                                baseTypeName = fhirType;
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
        /// <returns>True if it succeeds, false if it fails.</returns>
        private static bool TryGetTypeFromElement(
            string structureName,
            fhir_3.ElementDefinition element,
            out Dictionary<string, FhirElementType> elementTypes)
        {
            elementTypes = new Dictionary<string, FhirElementType>();

            // check for declared type
            if (element.Type != null)
            {
                foreach (fhir_3.ElementDefinitionType edType in element.Type)
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

                        if (!string.IsNullOrEmpty(edType.TargetProfile))
                        {
                            elementTypes[elementType.Name].AddProfile(edType.TargetProfile);
                        }

                        continue;
                    }

                    // use an extension-defined type
                    foreach (fhir_3.Extension ext in edType._Code.Extension)
                    {
                        switch (ext.Url)
                        {
                            case FhirVersionInfo.UrlXmlType:
                            case FhirVersionInfo.UrlFhirType:

                                // create a type for this code
                                FhirElementType elementType = new FhirElementType(edType.Code);

                                if (!elementTypes.ContainsKey(elementType.Name))
                                {
                                    elementTypes.Add(elementType.Name, elementType);
                                }

                                if (!string.IsNullOrEmpty(edType.TargetProfile))
                                {
                                    elementTypes[elementType.Name].AddProfile(edType.TargetProfile);
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
            if (string.IsNullOrEmpty(element.Id) ||
                element.Id.Equals(structureName, StringComparison.Ordinal))
            {
                // base type is here
                FhirElementType elementType = new FhirElementType(element.Path, null);

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
        /// <returns>True if it succeeds, false if it fails.</returns>
        private static bool TryGetTypeFromElements(
            string structureName,
            List<fhir_3.ElementDefinition> elements,
            out Dictionary<string, FhirElementType> elementTypes)
        {
            elementTypes = null;

            foreach (fhir_3.ElementDefinition element in elements)
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
        /// <param name="definitionComplexType">Type of structure definition we are parsing.</param>
        private static void ProcessComplex(
            fhir_3.StructureDefinition sd,
            FhirVersionInfo fhirVersionInfo,
            FhirComplex.FhirComplexType definitionComplexType)
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
                    contextElements.AddRange(sd.Context);
                }

                if (sd.Snapshot.Element.Count > 0)
                {
                    descriptionShort = sd.Snapshot.Element[0].Short;
                    definition = sd.Snapshot.Element[0].Definition;
                }

                // create a new complex type object
                FhirComplex complex = new FhirComplex(
                    sd.Id,
                    sd.Name,
                    new Uri(sd.Url),
                    sd.Status,
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

                // look for properties on this type
                foreach (fhir_3.ElementDefinition element in sd.Snapshot.Element)
                {
                    try
                    {
                        string id = element.Id ?? element.Path;
                        string path = element.Path ?? element.Id;
                        Dictionary<string, FhirElementType> elementTypes = null;
                        string elementType = string.Empty;

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

                            continue;
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
                            // throw new InvalidDataException($"Could not find parent for {element.Path}!");
                            // should load later
                            // TODO: figure out a way to verify all dependencies loaded
                            continue;
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
                                        false,
                                        false,
                                        string.Empty,
                                        null,
                                        string.Empty,
                                        null,
                                        true,
                                        true,
                                        string.Empty,
                                        string.Empty));
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
                        if (field.Contains("[x]"))
                        {
                            // fix the field and path names
                            id = id.Replace("[x]", string.Empty);
                            field = field.Replace("[x]", string.Empty);

                            // force no base type
                            elementType = string.Empty;
                        }
                        else if (!string.IsNullOrEmpty(element.ContentReference))
                        {
                            // check for local definition
                            switch (element.ContentReference[0])
                            {
                                case '#':
                                    // use the local reference
                                    elementType = element.ContentReference.Substring(1);
                                    break;

                                default:
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
                            valueSet = element.Binding.ValueSetUri;

                            if (string.IsNullOrEmpty(valueSet) &&
                                (element.Binding.ValueSetReference != null))
                            {
                                valueSet = element.Binding.ValueSetReference.ReferenceField;
                            }
                        }

                        // add this field to the parent type
                        parent.Elements.Add(
                            path,
                            new FhirElement(
                                id,
                                path,
                                null,
                                parent.Elements.Count,
                                element.Short,
                                element.Definition,
                                element.Comment,
                                string.Empty,
                                elementType,
                                elementTypes,
                                (int)(element.Min ?? 0),
                                element.Max,
                                element.IsModifier,
                                element.IsSummary,
                                defaultName,
                                defaultValue,
                                fixedName,
                                fixedValue,
                                isInherited,
                                modifiesParent,
                                bindingStrength,
                                valueSet));

                        if (element.Slicing != null)
                        {
                            List<FhirSliceDiscriminatorRule> discriminatorRules = new List<FhirSliceDiscriminatorRule>();

                            if (element.Slicing.Discriminator == null)
                            {
                                throw new InvalidDataException($"Missing slicing discriminator: {sd.Name} - {element.Path}");
                            }

                            foreach (fhir_3.ElementDefinitionSlicingDiscriminator discriminator in element.Slicing.Discriminator)
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
                    foreach (fhir_3.ElementDefinitionConstraint con in sd.Differential.Element[0].Constraint)
                    {
                        complex.AddConstraint(new FhirConstraint(
                            con.Key,
                            con.Severity,
                            con.Human,
                            con.Expression,
                            con.Xpath));
                    }
                }

                switch (definitionComplexType)
                {
                    case FhirComplex.FhirComplexType.DataType:
                        fhirVersionInfo.AddComplexType(complex);
                        break;
                    case FhirComplex.FhirComplexType.Resource:
                        fhirVersionInfo.AddResource(complex);
                        break;
                    case FhirComplex.FhirComplexType.Extension:
                        fhirVersionInfo.AddExtension(complex);
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Empty);
                Console.WriteLine($"FromR4.ProcessComplex <<< SD: {sd.Name} ({sd.Id}) - exception: {ex.Message}");
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
                return JsonConvert.DeserializeObject<fhir_3.Resource>(json, _jsonConverter);
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"FromR3.ParseResource <<< failed to parse:\n{ex}\n------------------------------------");
                throw;
            }
        }

        /// <summary>Attempts to process resource.</summary>
        /// <param name="resourceToParse">[out] The resource object.</param>
        /// <param name="fhirVersionInfo">FHIR Version information.</param>
        void IFhirConverter.ProcessResource(
            object resourceToParse,
            FhirVersionInfo fhirVersionInfo)
        {
            switch (resourceToParse)
            {
                // ignore
                /*
                //case fhir_3.CapabilityStatement capabilityStatement:
                //case fhir_3.CompartmentDefinition compartmentDefinition:
                //case fhir_3.ConceptMap conceptMap:
                //case fhir_3.NamingSystem namingSystem:
                //case fhir_3.StructureMap structureMap:
                */
                // process
                case fhir_3.CodeSystem codeSystem:
                    ProcessCodeSystem(codeSystem, fhirVersionInfo);
                    break;

                case fhir_3.OperationDefinition operationDefinition:
                    ProcessOperation(operationDefinition, fhirVersionInfo);
                    break;

                case fhir_3.SearchParameter searchParameter:
                    ProcessSearchParam(searchParameter, fhirVersionInfo);
                    break;

                case fhir_3.StructureDefinition structureDefinition:
                    ProcessStructureDef(structureDefinition, fhirVersionInfo);
                    break;

                case fhir_3.ValueSet valueSet:
                    ProcessValueSet(valueSet, fhirVersionInfo);
                    break;
            }
        }

        /// <summary>Gets default value if present.</summary>
        /// <param name="element">     The element.</param>
        /// <param name="defaultName"> [out] The default name.</param>
        /// <param name="defaultValue">[out] The default value.</param>
        private static void GetDefaultValueIfPresent(
            fhir_3.ElementDefinition element,
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

            defaultName = string.Empty;
            defaultValue = null;
        }

        /// <summary>Gets fixed value if present.</summary>
        /// <param name="element">   The element.</param>
        /// <param name="fixedName"> [out] Name of the fixed.</param>
        /// <param name="fixedValue">[out] The fixed value.</param>
        private static void GetFixedValueIfPresent(
            fhir_3.ElementDefinition element,
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

            fixedName = string.Empty;
            fixedValue = null;
        }
    }
}
