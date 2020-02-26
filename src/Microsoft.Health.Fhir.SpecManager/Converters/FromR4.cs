// -------------------------------------------------------------------------------------------------
// <copyright file="FromR4.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>
// -------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Health.Fhir.SpecManager.Manager;
using Microsoft.Health.Fhir.SpecManager.Models;
using Newtonsoft.Json;
using fhir_4 = Microsoft.Health.Fhir.SpecManager.fhir.r4;

namespace Microsoft.Health.Fhir.SpecManager.Converters
{
    /// <summary>Convert FHIR R4 into local definitions.</summary>
    public sealed class FromR4 : IFhirConverter
    {
        /// <summary>The JSON converter for polymorphic deserialization of this version of FHIR.</summary>
        private readonly JsonConverter _jsonConverter;

        /// <summary>
        /// Initializes a new instance of the <see cref="FromR4"/> class.
        /// </summary>
        public FromR4() => _jsonConverter = new fhir_4.ResourceConverter();

        /// <summary>Process the structure definition.</summary>
        /// <param name="sd">            The structure definition we are parsing.</param>
        /// <param name="primitiveTypes">[in,out] Primitive types.</param>
        /// <param name="complexTypes">  [in,out] Complex types.</param>
        /// <param name="resources">     [in,out] Resources.</param>
        /// <returns>True if it succeeds, false if it fails.</returns>
        private bool ProcessStructureDef(
            fhir_4.StructureDefinition sd,
            ref Dictionary<string, FhirPrimitive> primitiveTypes,
            ref Dictionary<string, FhirComplex> complexTypes,
            ref Dictionary<string, FhirComplex> resources)
        {
            try
            {
                // ignore retired
                if (sd.Status.Equals("retired", StringComparison.Ordinal))
                {
                    return true;
                }

                // act depending on kind
                switch (sd.Kind)
                {
                    case "primitive-type":
                        // exclude extensions
                        if (sd.Type == "Extension")
                        {
                            return true;
                        }

                        return ProcessDataTypePrimitive(sd, ref primitiveTypes);

                    case "complex-type":
                        // exclude extensions
                        if (sd.Type == "Extension")
                        {
                            return true;
                        }

                        // exclude profiles for now
                        if (sd.Derivation == "constraint")
                        {
                            return true;
                        }

                        return ProcessComplex(sd, ref complexTypes);

                    case "resource":

                        // exclude profiles for now
                        if (sd.Derivation == "constraint")
                        {
                            return true;
                        }

                        return ProcessComplex(sd, ref resources);

                    case "logical":
                        // ignore logical
                        return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"FromR4.ProcessStructureDef <<< failed to process {sd.Id}:\n{ex}\n--------------");
                return false;
            }

            // here means success
            return true;
        }

        /// <summary>Process a structure definition for a Primitive data type.</summary>
        /// <param name="sd">            The structure definition.</param>
        /// <param name="primitiveTypes">[in,out] Primitive types.</param>
        /// <returns>True if it succeeds, false if it fails.</returns>
        private static bool ProcessDataTypePrimitive(
            fhir_4.StructureDefinition sd,
            ref Dictionary<string, FhirPrimitive> primitiveTypes)
        {
            try
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
                primitiveTypes[sd.Name] = primitive;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"FromR4.ProcessDataTypePrimitive <<< failed to process {sd.Id}:\n{ex}\n--------------");
                return false;
            }

            // success
            return true;
        }

        /// <summary>Gets type from element.</summary>
        /// <param name="structureName">Name of the structure.</param>
        /// <param name="element">      The element.</param>
        /// <param name="elementType">  [out] Type of the element.</param>
        /// <returns>True if it succeeds, false if it fails.</returns>
        private static bool TryGetTypeFromElement(string structureName, fhir_4.ElementDefinition element, out string elementType)
        {
            elementType = null;

            // check for declared type
            if (element.Type != null)
            {
                foreach (fhir_4.ElementDefinitionType edType in element.Type)
                {
                    if ((edType._Code != null) && (edType._Code.Extension != null))
                    {
                        // use an extension-defined type
                        foreach (fhir_4.Extension ext in edType._Code.Extension)
                        {
                            switch (ext.Url)
                            {
                                case FhirVersionInfo.UrlFhirType:

                                    // use this type
                                    // elementType = Utils.TypeFromFhirType(ext.ValueString);
                                    elementType = ext.ValueUrl;

                                    // stop looking
                                    return true;

                                default:
                                    // ignore
                                    break;
                            }
                        }
                    }

                    // check for a specified type
                    if (!string.IsNullOrEmpty(edType.Code))
                    {
                        // use this type
                        elementType = Utils.TypeFromFhirType(edType.Code);

                        // done searching
                        return true;
                    }
                }
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
            elementType = null;
            return false;
        }

        /// <summary>Attempts to get type from elements.</summary>
        /// <param name="structureName">Name of the structure.</param>
        /// <param name="elements">     The elements.</param>
        /// <param name="typeName">     [out] Name of the type.</param>
        /// <returns>True if it succeeds, false if it fails.</returns>
        private static bool TryGetTypeFromElements(
            string structureName,
            fhir_4.ElementDefinition[] elements,
            out string typeName)
        {
            typeName = string.Empty;

            foreach (fhir_4.ElementDefinition element in elements)
            {
                // split the path
                string[] components = element.Path.Split('.');

                // check for base path having a type
                if (components.Length == 1)
                {
                    if (TryGetTypeFromElement(structureName, element, out string elementType))
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
                    if (TryGetTypeFromElement(structureName, element, out string elementType))
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

        /// <summary>Attempts to get expanded types.</summary>
        /// <param name="element">The element.</param>
        /// <param name="types">  [out] The types.</param>
        /// <returns>True if it succeeds, false if it fails.</returns>
        private static bool TryGetExpandedTypes(fhir_4.ElementDefinition element, out HashSet<string> types)
        {
            types = new HashSet<string>();

            // expanded types must be explicit
            if (element.Type == null)
            {
                return false;
            }

            foreach (fhir_4.ElementDefinitionType edType in element.Type)
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
                foreach (fhir_4.Extension ext in edType._Code.Extension)
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
        /// <param name="sd">         The structure definition to parse.</param>
        /// <param name="complexDict">[in,out] Dictionary with definitions known types.</param>
        /// <returns>True if it succeeds, false if it fails.</returns>
        private static bool ProcessComplex(
            fhir_4.StructureDefinition sd,
            ref Dictionary<string, FhirComplex> complexDict)
        {
            try
            {
                // create a new complex type object for this type or resource
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
                    if (!TryGetTypeFromElements(sd.Name, sd.Snapshot.Element, out string typeName))
                    {
                        Console.WriteLine($"FromR4.ProcessComplex <<< Could not determine base type for {sd.Name}");
                        return false;
                    }

                    complex.BaseTypeName = typeName;
                }

                // look for properties on this type
                foreach (fhir_4.ElementDefinition element in sd.Snapshot.Element)
                {
                    string path = element.Path;

                    // split the path into component parts
                    string[] components = path.Split('.');

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
                        Console.WriteLine($"FromR4.ProcessComplex <<<" +
                            $" Could not find parent for {element.Path}!");
                        return false;
                    }

                    string elementType;
                    HashSet<string> expandedTypes = null;

                    // determine if there is type expansion
                    if (field.Contains("[x]"))
                    {
                        // fix the field and path names
                        path = path.Replace("[x]", string.Empty);
                        field = field.Replace("[x]", string.Empty);

                        // no base type
                        elementType = string.Empty;

                        // get multiple types
                        if (!TryGetExpandedTypes(element, out expandedTypes))
                        {
                            Console.WriteLine($"FromR4.ProcessComplex <<<" +
                                $" Could not get expanded types for {sd.Name} field {element.Path}");
                            return false;
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
                                Console.WriteLine($"FromR4.ProcessComplex <<<" +
                                    $" Could not resolve content reference {element.ContentReference} in {sd.Name} field {element.Path}");
                                return false;
                        }
                    }
                    else
                    {
                        // if we can't find a type, assume Element
                        if (!TryGetTypeFromElement(parent.Name, element, out elementType))
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
                            expandedTypes,
                            (int)(element.Min ?? 0),
                            element.Max));
                }

                // add our type
                complexDict.Add(complex.Path, complex);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"FromR4.ProcessComplex <<< failed to process {sd.Id}:\n{ex}\n--------------");
                return false;
            }

            // success
            return true;
        }

        /// <summary>Attempts to parse resource an object from the given string.</summary>
        /// <param name="json">The JSON.</param>
        /// <param name="obj"> [out] The object.</param>
        /// <returns>True if it succeeds, false if it fails.</returns>
        bool IFhirConverter.TryParseResource(string json, out object obj)
        {
            try
            {
                // try to parse this JSON into a resource object
                obj = JsonConvert.DeserializeObject<fhir_4.Resource>(json, _jsonConverter);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"FromR4.TryParseResource <<< failed to parse:\n{ex}\n------------------------------------");
            }

            // failed to parse
            obj = null;
            return false;
        }

        /// <summary>Attempts to process resource.</summary>
        /// <param name="resourceToParse">[out] The resource object.</param>
        /// <param name="primitiveTypes"> [in,out] Primitive types.</param>
        /// <param name="complexTypes">   [in,out] Complex types.</param>
        /// <param name="resources">      [in,out] Resources.</param>
        /// <returns>True if it succeeds, false if it fails.</returns>
        bool IFhirConverter.TryProcessResource(
            object resourceToParse,
            ref Dictionary<string, FhirPrimitive> primitiveTypes,
            ref Dictionary<string, FhirComplex> complexTypes,
            ref Dictionary<string, FhirComplex> resources)
        {
            try
            {
                switch (resourceToParse)
                {
                    // ignore

                    // case fhir_4.CapabilityStatement capabilityStatement:
                    // case fhir_4.CodeSystem codeSystem:
                    // case fhir_4.CompartmentDefinition compartmentDefinition:
                    // case fhir_4.ConceptMap conceptMap:
                    // case fhir_4.NamingSystem namingSystem:
                    // case fhir_4.OperationDefinition operationDefinition:
                    // case fhir_4.SearchParameter searchParameter:
                    // case fhir_4.StructureMap structureMap:
                    // case fhir_4.ValueSet valueSet:

                    // process
                    case fhir_4.StructureDefinition structureDefinition:
                        return ProcessStructureDef(
                            structureDefinition,
                            ref primitiveTypes,
                            ref complexTypes,
                            ref resources);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"FromR4.TryProcessResource <<< Failed to process resource:\n{ex}\n--------------");
                return false;
            }

            // ignored
            return true;
        }
    }
}
