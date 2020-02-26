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
            ref Dictionary<string, FhirPrimitiveType> primitiveTypes,
            ref Dictionary<string, FhirComplexType> complexTypes,
            ref Dictionary<string, FhirComplexType> resources)
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
            ref Dictionary<string, FhirPrimitiveType> primitiveTypes)
        {
            try
            {
                // create a new primitive type object
                FhirPrimitiveType primitive = new FhirPrimitiveType()
                {
                    Name = sd.Name,
                    NameCapitalized = Utils.Capitalize(sd.Name),
                    StandardStatus = sd.Status,
                    ShortDescription = sd.Description,
                    Definition = sd.Purpose,
                    BaseTypeName = sd.Id,
                };

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
        /// <param name="complexDict">[in,out] Dictionary with definitions of this complex type.</param>
        /// <returns>True if it succeeds, false if it fails.</returns>
        private bool ProcessComplex(
            fhir_4.StructureDefinition sd,
            ref Dictionary<string, FhirComplexType> complexDict)
        {
            try
            {
                // create a new Complex Type object
                FhirComplexType complex = new FhirComplexType()
                {
                    Name = sd.Name,
                    NameCapitalized = Utils.Capitalize(sd.Name),
                    StandardStatus = sd.Status,
                    ShortDescription = sd.Description,
                    Definition = sd.Purpose,
                };

                // make a dictionary to track all the related resource definitions we create
                Dictionary<string, FhirComplexType> subDefs = new Dictionary<string, FhirComplexType>();

                // figure out the base type
                string mainType = null;
                string valueType = null;

                foreach (fhir_4.ElementDefinition element in sd.Snapshot.Element)
                {
                    // split the path
                    string[] components = element.Path.Split('.');

                    // check for base path having a type
                    if (components.Length == 1)
                    {
                        if (TryGetTypeFromElement(sd.Name, element, out string elementType))
                        {
                            // set our type
                            mainType = elementType;
                            break;
                        }
                    }

                    // check for path {type}.value having a type
                    if ((components.Length == 2) &&
                        components[1].Equals("value", StringComparison.Ordinal))
                    {
                        if (TryGetTypeFromElement(sd.Name, element, out string elementType))
                        {
                            // set our type
                            valueType = elementType;
                            continue;
                        }
                    }
                }

                // complex: prefer main type, use 'value' if it isn't present
                complex.BaseTypeName = mainType ?? valueType;

                // make sure we have a type
                if (string.IsNullOrEmpty(complex.BaseTypeName))
                {
                    Console.WriteLine($"FromR4.ProcessComplex <<<" +
                        $" Could not determine base type for {sd.Name}");
                    return false;
                }

                // add the current type definition to our internal dict for sanity
                subDefs.Add(sd.Name, complex);

                // look for properties on this type
                foreach (fhir_4.ElementDefinition element in sd.Snapshot.Element)
                {
                    // split the path into component parts
                    string[] components = element.Path.Split('.');

                    // base definition, already processed
                    if (components.Length < 2)
                    {
                        continue;
                    }

                    // grab field info we need
                    Utils.GetParentAndField(components, out string field, out string parent);

                    string elementType;
                    HashSet<string> expandedTypes = null;

                    // determine if there is type expansion
                    if (field.Contains("[x]"))
                    {
                        // fix the field name
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
                                // search for referenced element
                                string localRef = Utils.PascalFromDot(element.ContentReference.Substring(1));

                                // look up the named reference in the alias table
                                if (!subDefs.ContainsKey(localRef))
                                {
                                    Console.WriteLine($"FromR4.ProcessComplex <<<" +
                                        $" Could not find content reference {element.ContentReference} in {sd.Name} field {element.Path}");
                                    return false;
                                }

                                // use the local reference
                                elementType = localRef;

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
                        if (!TryGetTypeFromElement(parent, element, out elementType))
                        {
                            elementType = "Element";
                        }
                    }

                    // check to see if we do NOT have this parent, but do have a definition
                    if (!subDefs.ContainsKey(parent))
                    {
                        // figure out if we have this parent as a field
                        string[] parentComponents = new string[components.Length - 1];
                        Array.Copy(components, 0, parentComponents, 0, components.Length - 1);

                        // get parent info
                        Utils.GetParentAndField(parentComponents, out string pField, out string pParent);

                        if (subDefs.ContainsKey(pParent) &&
                            subDefs[pParent].Properties.ContainsKey(pField))
                        {
                            // use this type
                            FhirComplexType sub = new FhirComplexType()
                            {
                                Name = Utils.Capitalize(parent),
                                NameCapitalized = Utils.Capitalize(parent),
                                ShortDescription = subDefs[pParent].Properties[pField].ShortDescription,
                                Definition = subDefs[pParent].Properties[pField].Definition,
                                Comment = subDefs[pParent].Properties[pField].Comment,
                                BaseTypeName = subDefs[pParent].Properties[pField].BaseTypeName,
                            };

                            // add this parent to our local dictionary
                            subDefs.Add(parent, sub);

                            // change our element type to point at the parent
                            subDefs[pParent].Properties[pField].BaseTypeName = Utils.Capitalize(parent);
                        }
                        else
                        {
                            // add a placeholder type
                            FhirComplexType sub = new FhirComplexType()
                            {
                                Name = Utils.Capitalize(parent),
                                NameCapitalized = Utils.Capitalize(parent),
                                ShortDescription = string.Empty,
                                Definition = string.Empty,
                                Comment = $"Placeholder for {parent}",
                                BaseTypeName = parent,
                                IsPlaceholder = true,
                            };

                            // add this parent to our local dictionary
                            subDefs.Add(parent, sub);
                        }
                    }

                    // add this field to the parent type
                    subDefs[parent].Properties.Add(
                        field,
                        new FhirProperty()
                        {
                            Name = field,
                            NameCapitalized = Utils.Capitalize(field),
                            ShortDescription = element.Short,
                            Definition = element.Definition,
                            Comment = element.Comment,
                            BaseTypeName = elementType,
                            ExpandedTypes = expandedTypes,
                            CardinalityMin = (int)(element.Min ?? 0),
                            CardinaltiyMax = Utils.MaxCardinality(element.Max),
                        });
                }

                // copy over our definitions
                foreach (KeyValuePair<string, FhirComplexType> kvp in subDefs)
                {
                    // check for removing a placeholder
                    if (complexDefinitions.ContainsKey(kvp.Key) &&
                        (complexDefinitions[kvp.Key].IsPlaceholder == true))
                    {
                        complexDefinitions.Remove(kvp.Key);
                    }

                    // check for not being present
                    if (!complexDefinitions.ContainsKey(kvp.Key))
                    {
                        complexDefinitions.Add(kvp.Key, kvp.Value);
                        continue;
                    }

                    // check fields
                    foreach (KeyValuePair<string, FhirProperty> propKvp in kvp.Value.Properties)
                    {
                        if (!complexDefinitions[kvp.Key].Properties.ContainsKey(propKvp.Key))
                        {
                            complexDefinitions[kvp.Key].Properties.Add(propKvp.Key, propKvp.Value);
                        }
                    }
                }
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
            ref Dictionary<string, FhirPrimitiveType> primitiveTypes,
            ref Dictionary<string, FhirComplexType> complexTypes,
            ref Dictionary<string, FhirComplexType> resources)
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
