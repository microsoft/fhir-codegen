// -------------------------------------------------------------------------------------------------
// <copyright file="FromR3.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>
// -------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
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
        /// <summary>The JSON converter for polymorphic deserialization of this version of FHIR.</summary>
        private JsonConverter _jsonConverter;

        /// <summary>
        /// Initializes a new instance of the <see cref="FromR3"/> class.
        /// </summary>
        public FromR3() => _jsonConverter = new fhir_3.ResourceConverter();

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

            string[] resources = sp.Base;

            // check for parameters with no base resource
            if (sp.Base == null)
            {
                List<string> resourceList = new List<string>();

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

                resources = resourceList.ToArray();
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
                sp.Type,
                sp.Status,
                sp.Experimental == true);

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
                    // exclude extensions
                    if (sd.Type == "Extension")
                    {
                        ProcessDataTypePrimitive(sd, fhirVersionInfo);
                    }

                    break;

                case "complex-type":
                    // exclude extensions and profiles
                    if ((sd.Type != "Extension") &&
                        (sd.Derivation != "constraint"))
                    {
                        ProcessComplex(sd, fhirVersionInfo, false);
                    }

                    break;

                case "resource":
                    // exclude profiles for now
                    if (sd.Derivation != "constraint")
                    {
                        ProcessComplex(sd, fhirVersionInfo, true);
                    }

                    break;
            }
        }

        /// <summary>Process the structure definition of an extension.</summary>
        /// <param name="sd">             The structure definition we are parsing.</param>
        /// <param name="fhirVersionInfo">FHIR Version information.</param>
        private void ProcessStructureDefExtension(
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
                    // include extensions
                    if (sd.Type == "Extension")
                    {
                        ProcessExtension(sd, fhirVersionInfo);
                    }

                    break;

                case "complex-type":
                    // include extensions and profiles
                    if ((sd.Type == "Extension") ||
                        (sd.Derivation == "constraint"))
                    {
                        ProcessExtension(sd, fhirVersionInfo);
                    }

                    break;

                case "resource":
                    // include profiles
                    if (sd.Derivation == "constraint")
                    {
                        ProcessExtension(sd, fhirVersionInfo);
                    }

                    break;
            }
        }

        /// <summary>Process the extension.</summary>
        /// <exception cref="ArgumentException">Thrown when one or more arguments have unsupported or
        ///  illegal values.</exception>
        /// <param name="sd">             The structure definition to parse.</param>
        /// <param name="fhirVersionInfo">FHIR Version information.</param>
        private static void ProcessExtension(
            fhir_3.StructureDefinition sd,
            FhirVersionInfo fhirVersionInfo)
        {
            List<string> elementPaths = new List<string>();
            Dictionary<string, List<string>> allowedTypesAndProfiles = new Dictionary<string, List<string>>();
            bool isModifier = false;
            bool isSummary = false;

            // check for nothing to process
            if ((sd == null) ||
                (sd.Context == null) ||
                (sd.Snapshot == null) ||
                (sd.Snapshot.Element == null))
            {
                return;
            }

            // look for context information
            foreach (string context in sd.Context)
            {
                elementPaths.Add(context);
            }

            // traverse elements looking for data we need
            foreach (fhir_3.ElementDefinition element in sd.Snapshot.Element)
            {
                switch (element.Id)
                {
                    case "Extension.value[x]":
                        // grab types
                        if (element.Type != null)
                        {
                            foreach (fhir_3.ElementDefinitionType type in element.Type)
                            {
                                if (!allowedTypesAndProfiles.ContainsKey(type.Code))
                                {
                                    allowedTypesAndProfiles.Add(type.Code, new List<string>());
                                }

                                if (type.TargetProfile != null)
                                {
                                    allowedTypesAndProfiles[type.Code].Add(
                                        type.TargetProfile.Substring(type.TargetProfile.LastIndexOf('/') + 1));
                                }
                            }
                        }

                        if (element.IsModifier == true)
                        {
                            isModifier = true;
                        }

                        if (element.IsSummary == true)
                        {
                            isSummary = true;
                        }

                        break;

                    case "Extension":

                        if (element.IsModifier == true)
                        {
                            isModifier = true;
                        }

                        if (element.IsSummary == true)
                        {
                            isSummary = true;
                        }

                        break;
                }
            }

            // check internal constraints for adding
            if ((elementPaths.Count == 0) ||
                (allowedTypesAndProfiles.Count == 0))
            {
                return;
            }

            // create a new extension object
            FhirExtension extension = new FhirExtension(
                sd.Name,
                sd.Id,
                new Uri(sd.Url),
                elementPaths,
                allowedTypesAndProfiles,
                isModifier,
                isSummary);

            // add our property extension
            fhirVersionInfo.AddExtension(extension);
        }

        /// <summary>Process a structure definition for a Primitve data type.</summary>
        /// <param name="sd">             The structure definition.</param>
        /// <param name="fhirVersionInfo">FHIR Version information.</param>
        private static void ProcessDataTypePrimitive(
            fhir_3.StructureDefinition sd,
            FhirVersionInfo fhirVersionInfo)
        {
            // create a new primitive type object
            FhirPrimitive primitive = new FhirPrimitive(
                sd.Name,
                sd.Status,
                sd.Description,
                sd.Purpose,
                string.Empty,
                null);

            // add to our dictionary of primitive types
            fhirVersionInfo.AddPrimitive(primitive);
        }

        /// <summary>Gets type from element.</summary>
        /// <param name="structureName"> Name of the structure.</param>
        /// <param name="element">       The element.</param>
        /// <param name="elementType">   [out] Type of the element.</param>
        /// <param name="targetProfiles">[out] Target profiles.</param>
        /// <returns>True if it succeeds, false if it fails.</returns>
        private static bool TryGetTypeFromElement(
            string structureName,
            fhir_3.ElementDefinition element,
            out string elementType,
            out string[] targetProfiles)
        {
            targetProfiles = null;
            elementType = null;

            List<string> profiles = new List<string>();

            // check for declared type
            if (element.Type != null)
            {
                foreach (fhir_3.ElementDefinitionType edType in element.Type)
                {
                    // check for a specified type
                    if (!string.IsNullOrEmpty(edType.Code))
                    {
                        // use this type
                        elementType = edType.Code;

                        // check for a target profile
                        if (!string.IsNullOrEmpty(edType.TargetProfile))
                        {
                            profiles.Add(edType.TargetProfile);
                        }

                        // reference will have multiple types here - need to grab each profile
                        if (elementType.Equals("Reference", StringComparison.Ordinal))
                        {
                            continue;
                        }

                        // done searching
                        return true;
                    }

                    // use an extension-defined type
                    foreach (fhir_3.Extension ext in edType._Code.Extension)
                    {
                        switch (ext.Url)
                        {
                            case FhirVersionInfo.UrlFhirType:

                                // use this type
                                elementType = ext.ValueString;

                                // stop looking
                                return true;

                            case FhirVersionInfo.UrlXmlType:

                                // use this type
                                elementType = Utils.TypeFromXmlType(ext.ValueString);

                                // stop looking
                                return true;

                            default:
                                // ignore
                                break;
                        }
                    }
                }
            }

            // check for target profiles
            if (profiles.Count > 0)
            {
                targetProfiles = profiles.ToArray();
                return true;
            }

            // check for base derived type
            if (string.IsNullOrEmpty(element.Id) ||
                element.Id.Equals(structureName, StringComparison.Ordinal))
            {
                // base type is here
                elementType = element.Path;

                // done searching
                return true;
            }

            // no discovered type
            return false;
        }

        /// <summary>Attempts to get type from elements.</summary>
        /// <param name="structureName">Name of the structure.</param>
        /// <param name="elements">     The elements.</param>
        /// <param name="typeName">     [out] Name of the type.</param>
        /// <returns>True if it succeeds, false if it fails.</returns>
        private static bool TryGetTypeFromElements(
            string structureName,
            fhir_3.ElementDefinition[] elements,
            out string typeName,
            out string[] targetProfiles)
        {
            targetProfiles = null;
            typeName = string.Empty;

            foreach (fhir_3.ElementDefinition element in elements)
            {
                // split the path
                string[] components = element.Path.Split('.');

                // check for base path having a type
                if (components.Length == 1)
                {
                    if (TryGetTypeFromElement(structureName, element, out string elementType, out targetProfiles))
                    {
                        // set our type
                        typeName = elementType;

                        // done searching
                        return true;
                    }
                }

                // check for path {type}.value having a type
                if ((components.Length == 2) &&
                    components[1].Equals("value", StringComparison.Ordinal))
                {
                    if (TryGetTypeFromElement(structureName, element, out string elementType, out targetProfiles))
                    {
                        // set our type
                        typeName = elementType;

                        // keep looking in case we find a better option
                        continue;
                    }
                }
            }

            if (!string.IsNullOrEmpty(typeName))
            {
                return true;
            }

            return false;
        }

        /// <summary>Attempts to get choice types.</summary>
        /// <param name="element">The element.</param>
        /// <param name="types">  [out] The types.</param>
        /// <returns>True if it succeeds, false if it fails.</returns>
        private static bool TryGetChoiceTypes(fhir_3.ElementDefinition element, out HashSet<string> types)
        {
            types = new HashSet<string>();

            // expanded types must be explicit
            if (element.Type == null)
            {
                return false;
            }

            foreach (fhir_3.ElementDefinitionType edType in element.Type)
            {
                // check for a specified type
                if (!string.IsNullOrEmpty(edType.Code))
                {
                    // use this type
                    types.Add(edType.Code);

                    // check next type
                    continue;
                }

                // use an extension-defined type
                foreach (fhir_3.Extension ext in edType._Code.Extension)
                {
                    switch (ext.Url)
                    {
                        case FhirVersionInfo.UrlFhirType:

                            // use this type
                            types.Add(ext.ValueString);

                            // check next type
                            continue;

                        case FhirVersionInfo.UrlXmlType:

                            // use this type
                            types.Add(Utils.TypeFromXmlType(ext.ValueString));

                            // check next type
                            continue;

                        default:
                            // ignore
                            break;
                    }
                }
            }

            // check for no discovered types
            if (types.Count == 0)
            {
                return false;
            }

            // success
            return true;
        }

        /// <summary>Process a complex structure (Complex Type or Resource).</summary>
        /// <param name="sd">             The structure definition to parse.</param>
        /// <param name="fhirVersionInfo">FHIR Version information.</param>
        /// <param name="isResource">     True if is resource, false if not.</param>
        private static void ProcessComplex(
            fhir_3.StructureDefinition sd,
            FhirVersionInfo fhirVersionInfo,
            bool isResource)
        {
            string[] targetProfiles = null;

            // create a new complex type object
            FhirComplex complex = new FhirComplex(
                sd.Name,
                sd.Status,
                sd.Description,
                sd.Purpose,
                string.Empty,
                null);

            // check for a base definition
            if (!string.IsNullOrEmpty(sd.BaseDefinition))
            {
                complex.BaseTypeName = sd.BaseDefinition.Substring(sd.BaseDefinition.LastIndexOf('/') + 1);
            }
            else
            {
                if (!TryGetTypeFromElements(sd.Name, sd.Snapshot.Element, out string typeName, out targetProfiles))
                {
                    throw new InvalidDataException($"Could not determine base type for {sd.Name}");
                }

                complex.BaseTypeName = typeName;
            }

            // look for properties on this type
            foreach (fhir_3.ElementDefinition element in sd.Snapshot.Element)
            {
                string path = element.Path;

                // split the path into component parts
                string[] components = element.Path.Split('.');

                // base definition, already processed
                if (components.Length < 2)
                {
                    continue;
                }

                // get the parent container and our field name
                if (!complex.GetParentAndFieldName(
                        components,
                        out FhirComplex parent,
                        out string field))
                {
                    throw new InvalidDataException($"Could not find parent for {element.Path}!");
                }

                string elementType;
                HashSet<string> choiceTypes = null;

                // determine if there is type expansion
                if (field.Contains("[x]"))
                {
                    // fix the field and path names
                    path = path.Replace("[x]", string.Empty);
                    field = field.Replace("[x]", string.Empty);

                    // no base type
                    elementType = string.Empty;

                    // get multiple types
                    if (!TryGetChoiceTypes(element, out choiceTypes))
                    {
                        throw new InvalidDataException($"Could not get choice types for {sd.Name} field {element.Path}");
                    }
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
                else
                {
                    // if we can't find a type, assume Element
                    if (!TryGetTypeFromElement(parent.Name, element, out elementType, out targetProfiles))
                    {
                        elementType = "Element";
                    }
                }

                // add this field to the parent type
                parent.Properties.Add(
                    path,
                    new FhirProperty(
                        path,
                        parent.Properties.Count,
                        element.Short,
                        element.Definition,
                        element.Comment,
                        string.Empty,
                        elementType,
                        choiceTypes,
                        (int)(element.Min ?? 0),
                        element.Max,
                        targetProfiles));
            }

            // add our type
            if (isResource)
            {
                fhirVersionInfo.AddResource(complex);
            }
            else
            {
                fhirVersionInfo.AddComplexType(complex);
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
        /// <param name="processHint">    Process hints related to load operation.</param>
        void IFhirConverter.ProcessResource(
            object resourceToParse,
            FhirVersionInfo fhirVersionInfo,
            string processHint)
        {
            switch (resourceToParse)
            {
                // ignore
                /*
                //case fhir_3.CapabilityStatement capabilityStatement:
                //case fhir_3.CodeSystem codeSystem:
                //case fhir_3.CompartmentDefinition compartmentDefinition:
                //case fhir_3.ConceptMap conceptMap:
                //case fhir_3.NamingSystem namingSystem:
                //case fhir_3.StructureMap structureMap:
                //case fhir_3.ValueSet valueSet:
                */
                // process
                case fhir_3.OperationDefinition operationDefinition:
                    ProcessOperation(operationDefinition, fhirVersionInfo);
                    break;

                case fhir_3.SearchParameter searchParameter:
                    ProcessSearchParam(searchParameter, fhirVersionInfo);
                    break;

                case fhir_3.StructureDefinition structureDefinition:
                    if (processHint.Equals("Extension", StringComparison.Ordinal))
                    {
                        ProcessStructureDefExtension(structureDefinition, fhirVersionInfo);
                    }
                    else
                    {
                        ProcessStructureDef(structureDefinition, fhirVersionInfo);
                    }
                    break;
            }
        }
    }
}
