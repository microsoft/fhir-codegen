// -------------------------------------------------------------------------------------------------
// <copyright file="FromR2.cs" company="Microsoft Corporation">
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

        /// <summary>Process the structure definition.</summary>
        /// <param name="sd">            The structure definition to parse.</param>
        /// <param name="primitiveTypes">[in,out] Primitive types.</param>
        /// <param name="complexTypes">  [in,out] Complex types.</param>
        /// <param name="resources">     [in,out] Resources.</param>
        /// <returns>True if it succeeds, false if it fails.</returns>
        private bool ProcessStructureDef(
            fhir_2.StructureDefinition sd,
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
                    case "datatype":
                        // exclude extensions
                        if (sd.ConstrainedType == "Extension")
                        {
                            return true;
                        }

                        // leading lower case is primitive
                        if (char.IsLower(sd.Name[0]))
                        {
                            return ProcessDataTypePrimitive(sd, ref primitiveTypes);
                        }

                        return ProcessComplex(sd, ref complexTypes);

                    case "resource":
                        // exclude profiles for now
                        if (!string.IsNullOrEmpty(sd.ConstrainedType))
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
                Console.WriteLine($"FromR2.ProcessStructureDef <<< failed to process {sd.Id}:\n{ex}\n--------------");
                return false;
            }

            // here means success
            return true;
        }

        /// <summary>Process a structure definition for a primitive data type.</summary>
        /// <param name="sd">            The structure definition to parse.</param>
        /// <param name="primitiveTypes">[in,out] Primitive types.</param>
        /// <returns>True if it succeeds, false if it fails.</returns>
        private static bool ProcessDataTypePrimitive(
            fhir_2.StructureDefinition sd,
            ref Dictionary<string, FhirPrimitiveType> primitiveTypes)
        {
            FhirPrimitiveType primitive = new FhirPrimitiveType()
            {
                Name = sd.Name,
                NameCapitalized = Utils.Capitalize(sd.Name),
                StandardStatus = sd.Status,
                ShortDescription = sd.Description,
                Definition = sd.Requirements,
                BaseTypeName = sd.Id,
            };

            // add to our dictionary of primitive types
            primitiveTypes[sd.Name] = primitive;

            // success
            return true;
        }

        /// <summary>Gets type from element.</summary>
        ///
        /// <param name="structureName">Name of the structure.</param>
        /// <param name="element">      The element.</param>
        /// <param name="elementType">  [out] Type of the element.</param>
        ///
        /// <returns>True if it succeeds, false if it fails.</returns>
        private static bool TryGetTypeFromElement(string structureName, fhir_2.ElementDefinition element, out string elementType)
        {
            elementType = null;

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
            elementType = null;
            return false;
        }

        /// <summary>Attempts to get expanded types.</summary>
        ///
        /// <param name="element">      The element.</param>
        /// <param name="types">        [out] The types.</param>
        ///
        /// <returns>True if it succeeds, false if it fails.</returns>
        private static bool TryGetExpandedTypes(fhir_2.ElementDefinition element, out HashSet<string> types)
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
        /// <param name="sd">         The structure definition to parse.</param>
        /// <param name="complexDict">[in,out] Dictionary with definitions of this complex type.</param>
        /// <returns>True if it succeeds, false if it fails.</returns>
        private bool ProcessComplex(
            fhir_2.StructureDefinition sd,
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
                    Definition = sd.Requirements,
                };

                // make a dictionary to track all the related resource definitions we create
                Dictionary<string, FhirComplexType> subDefs = new Dictionary<string, FhirComplexType>();

                // create an alias table for named reference linking within the type
                Dictionary<string, string> aliasTable = new Dictionary<string, string>();

                // figure out the base type
                string mainType = null;
                string valueType = null;

                foreach (fhir_2.ElementDefinition element in sd.Snapshot.Element)
                {
                    // split the path
                    string[] components = element.Path.Split('.');

                    // check for base path having a type
                    if (components.Length == 1)
                    {
                        if (TryGetTypeFromElement(sd.Name, element, out string elementType))
                        {
                            // set our type
                            complex.BaseTypeName = elementType;

                            // done searching
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
                            complex.BaseTypeName = elementType;

                            // keep looking in case we find a better option
                            continue;
                        }
                    }
                }

                // complex: prefer main type, use 'value' if it isn't present
                complex.BaseTypeName = mainType ?? valueType;

                // make sure we have a type
                if (string.IsNullOrEmpty(complex.BaseTypeName))
                {
                    Console.WriteLine($"FromR2.ProcessComplex <<<" +
                        $" Could not determine base type for {sd.Name}");
                    return false;
                }

                // add the current type definition to our internal dict for sanity
                subDefs.Add(sd.Name, complex);

                // look for properties on this type
                foreach (fhir_2.ElementDefinition element in sd.Snapshot.Element)
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
                            Console.WriteLine($"FromR2.ProcessComplex <<<" +
                                $" Could not get expanded types for {sd.Name} field {element.Path}");
                            return false;
                        }
                    }
                    else if (!string.IsNullOrEmpty(element.NameReference))
                    {
                        // look up the named reference in the alias table
                        if (!aliasTable.ContainsKey(element.NameReference))
                        {
                            Console.WriteLine($"FromR2.ProcessComplex <<<" +
                                $" Could not find named reference {element.NameReference} in {sd.Name} field {element.Path}");
                            return false;
                        }

                        // use the named type
                        elementType = aliasTable[element.NameReference];
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
                            CardinalityMin = element.Min ?? 0,
                            CardinaltiyMax = Utils.MaxCardinality(element.Max),
                        });

                    // check to see if we need to insert into our alias table
                    if (!string.IsNullOrEmpty(element.Name))
                    {
                        // add this record, with it's current path
                        aliasTable.Add(element.Name, $"{parent}{subDefs[parent].Properties[field].NameCapitalized}");
                    }
                }

                // copy over our definitions
                foreach (KeyValuePair<string, FhirComplexType> kvp in subDefs)
                {
                    // check for removing a placeholder
                    if (complexDict.ContainsKey(kvp.Key) &&
                        (complexDict[kvp.Key].IsPlaceholder == true))
                    {
                        complexDict.Remove(kvp.Key);
                    }

                    // check for not being present
                    if (!complexDict.ContainsKey(kvp.Key))
                    {
                        complexDict.Add(kvp.Key, kvp.Value);
                        continue;
                    }

                    // check fields
                    foreach (KeyValuePair<string, FhirProperty> propKvp in kvp.Value.Properties)
                    {
                        if (!complexDict[kvp.Key].Properties.ContainsKey(propKvp.Key))
                        {
                            complexDict[kvp.Key].Properties.Add(propKvp.Key, propKvp.Value);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"FromR2.ProcessComplex <<< failed to process {sd.Id}:\n{ex}\n--------------");
                return false;
            }

            // success
            return true;
        }

        /// <summary>Attempts to parse resource an object from the given string.</summary>
        ///
        /// <param name="json">The JSON.</param>
        /// <param name="obj"> [out] The object.</param>
        ///
        /// <returns>True if it succeeds, false if it fails.</returns>
        bool IFhirConverter.TryParseResource(string json, out object obj)
        {
            try
            {
                // try to parse this JSON into a resource object
                obj = JsonConvert.DeserializeObject<fhir_2.Resource>(json, _jsonConverter);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"FromR2.TryParseResource <<< failed to parse:\n{ex}\n------------------------------------");
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
                    // case fhir_2.Conformance conformance:
                    // case fhir_2.NamingSystem namingSystem:
                    // case fhir_2.OperationDefinition operationDefinition:
                    // case fhir_2.SearchParameter searchParameter:
                    // case fhir_2.ValueSet valueSet:

                    // process
                    case fhir_2.StructureDefinition structureDefinition:
                        return ProcessStructureDef(
                            structureDefinition,
                            ref primitiveTypes,
                            ref complexTypes,
                            ref resources);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"FromR2.TryProcessResource <<< Failed to process resource:\n{ex}\n--------------");
                return false;
            }

            // ignored
            return true;
        }
    }
}
