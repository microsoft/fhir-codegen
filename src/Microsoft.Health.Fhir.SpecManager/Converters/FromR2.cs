// -------------------------------------------------------------------------------------------------
// <copyright file="FromR2.cs" company="Microsoft Corporation">
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
using fhir_2 = Microsoft.Health.Fhir.SpecManager.fhir.r2;

namespace Microsoft.Health.Fhir.SpecManager.Converters
{
    /// <summary>Convert FHIR R2 into local definitions.</summary>
    public sealed class FromR2 : IFhirConverter
    {
        /// <summary>The JSON converter for polymorphic deserialization of this version of FHIR.</summary>
        private readonly JsonConverter _jsonConverter;

        /// <summary>
        /// Initializes a new instance of the <see cref="FromR2"/> class.
        /// </summary>
        public FromR2() => _jsonConverter = new fhir_2.ResourceConverter();

        /// <summary>Process the operation.</summary>
        /// <param name="op">             The operation.</param>
        /// <param name="fhirVersionInfo">FHIR Version information.</param>
        private void ProcessOperation(
            fhir_2.OperationDefinition op,
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
                foreach (fhir_2.OperationDefinitionParameter opParam in op.Parameter)
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
                (op.Type == null) || (op.Type.Length == 0),
                op.Instance,
                op.Code,
                op.Comment,
                op.Type,
                parameters);

            // add our parameter
            fhirVersionInfo.AddOperation(operation);
        }

        /// <summary>Process the search parameter.</summary>
        /// <param name="sp">             The sp.</param>
        /// <param name="fhirVersionInfo">FHIR Version information.</param>
        private void ProcessSearchParam(
            fhir_2.SearchParameter sp,
            FhirVersionInfo fhirVersionInfo)
        {
            // ignore retired
            if (sp.Status.Equals("retired", StringComparison.Ordinal))
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

            // create the search parameter
            FhirSearchParam param = new FhirSearchParam(
                sp.Id,
                new Uri(sp.Url),
                fhirVersionInfo.VersionString,
                sp.Name,
                sp.Description,
                string.Empty,
                sp.Code,
                resourceList.ToArray(),
                sp.Type,
                sp.Status,
                sp.Experimental == true);

            // add our parameter
            fhirVersionInfo.AddSearchParameter(param);
        }

        /// <summary>Process the structure definition.</summary>
        /// <param name="sd">             The structure definition to parse.</param>
        /// <param name="fhirVersionInfo">FHIR Version information.</param>
        private void ProcessStructureDef(
            fhir_2.StructureDefinition sd,
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
                case "datatype":
                    // exclude extensions
                    if (sd.ConstrainedType != "Extension")
                    {
                        // leading lower case is primitive
                        if (char.IsLower(sd.Name[0]))
                        {
                            ProcessDataTypePrimitive(sd, fhirVersionInfo);
                        }
                        else
                        {
                            ProcessComplex(sd, fhirVersionInfo, false);
                        }
                    }

                    break;

                case "resource":
                    // exclude profiles for now
                    if (string.IsNullOrEmpty(sd.ConstrainedType))
                    {
                        ProcessComplex(sd, fhirVersionInfo, true);
                    }

                    break;
            }
        }

        /// <summary>Process a structure definition for a primitive data type.</summary>
        /// <param name="sd">             The structure definition to parse.</param>
        /// <param name="fhirVersionInfo">FHIR Version information.</param>
        private static void ProcessDataTypePrimitive(
            fhir_2.StructureDefinition sd,
            FhirVersionInfo fhirVersionInfo)
        {
            // create a new primitive type object
            FhirPrimitive primitive = new FhirPrimitive(
                sd.Name,
                sd.Status,
                sd.Description,
                sd.Requirements,
                string.Empty,
                null);

            // add to our dictionary of primitive types
            fhirVersionInfo.AddPrimitive(primitive);
        }

        /// <summary>Gets type from element.</summary>
        ///
        /// <param name="structureName">Name of the structure.</param>
        /// <param name="element">      The element.</param>
        /// <param name="elementType">  [out] Type of the element.</param>
        ///
        /// <returns>True if it succeeds, false if it fails.</returns>
        private static bool TryGetTypeFromElement(
            string structureName,
            fhir_2.ElementDefinition element,
            out string elementType,
            out string[] targetProfiles)
        {
            targetProfiles = null;
            elementType = null;

            List<string> profiles = new List<string>();

            // check for declared type
            if (element.Type != null)
            {
                foreach (fhir_2.ElementDefinitionType edType in element.Type)
                {
                    // check for a specified type
                    if (!string.IsNullOrEmpty(edType.Code))
                    {
                        // use this type
                        elementType = edType.Code;

                        // check for a target profile
                        if (edType.Profile != null)
                        {
                            profiles.AddRange(edType.Profile);
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
                    foreach (fhir_2.Extension ext in edType._Code.Extension)
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
            if (string.IsNullOrEmpty(element.Name) ||
                element.Name.Equals(structureName, StringComparison.Ordinal))
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
            fhir_2.ElementDefinition[] elements,
            out string typeName,
            out string[] targetProfiles)
        {
            targetProfiles = null;
            typeName = string.Empty;

            foreach (fhir_2.ElementDefinition element in elements)
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
        private static bool TryGetChoiceTypes(fhir_2.ElementDefinition element, out HashSet<string> types)
        {
            types = new HashSet<string>();

            // expanded types must be explicit
            if (element.Type == null)
            {
                return false;
            }

            foreach (fhir_2.ElementDefinitionType edType in element.Type)
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
                foreach (fhir_2.Extension ext in edType._Code.Extension)
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
            fhir_2.StructureDefinition sd,
            FhirVersionInfo fhirVersionInfo,
            bool isResource)
        {
            string[] targetProfiles = null;
            Dictionary<string, string> aliasTable = new Dictionary<string, string>();

            // create a new complex type object
            FhirComplex complex = new FhirComplex(
                sd.Name,
                sd.Status,
                sd.Description,
                sd.Requirements,
                string.Empty,
                null);

            // check for a base definition
            if (!string.IsNullOrEmpty(sd.Base))
            {
                complex.BaseTypeName = sd.Base.Substring(sd.Base.LastIndexOf('/') + 1);
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
            foreach (fhir_2.ElementDefinition element in sd.Snapshot.Element)
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
                else if (!string.IsNullOrEmpty(element.NameReference))
                {
                    // look up the named reference in the alias table
                    if (!aliasTable.ContainsKey(element.NameReference))
                    {
                        throw new InvalidDataException($"Could not resolve NameReference {element.NameReference} in {sd.Name} field {element.Path}");
                    }

                    // use the named type
                    elementType = aliasTable[element.NameReference];
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

                // check to see if we need to insert into our alias table
                if (!string.IsNullOrEmpty(element.Name))
                {
                    // add this record, with it's current path
                    aliasTable.Add(element.Name, element.Path);
                }
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
                return JsonConvert.DeserializeObject<fhir_2.Resource>(json, _jsonConverter);
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"FromR2.ParseResource <<< failed to parse:\n{ex}\n------------------------------------");
                throw;
            }
        }

        /// <summary>Attempts to process resource.</summary>
        /// <param name="resourceToParse">[out] The resource object.</param>
        /// <param name="fhirVersionInfo">FHIR Version information.</param>
        void IFhirConverter.ProcessResource(object resourceToParse, FhirVersionInfo fhirVersionInfo)
        {
            switch (resourceToParse)
            {
                // ignore
                // case fhir_2.Conformance conformance:
                // case fhir_2.NamingSystem namingSystem:
                // case fhir_2.ValueSet valueSet:

                // process
                case fhir_2.OperationDefinition operationDefinition:
                    ProcessOperation(operationDefinition, fhirVersionInfo);
                    break;

                case fhir_2.SearchParameter searchParameter:
                    ProcessSearchParam(searchParameter, fhirVersionInfo);
                    break;

                case fhir_2.StructureDefinition structureDefinition:
                    ProcessStructureDef(structureDefinition, fhirVersionInfo);
                    break;
            }
        }
    }
}
