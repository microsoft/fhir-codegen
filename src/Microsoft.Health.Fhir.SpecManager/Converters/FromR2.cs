// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
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
    /// -------------------------------------------------------------------------------------------------
    /// <summary>Convert FHIR R2 into local definitions.</summary>
    /// -------------------------------------------------------------------------------------------------
    public sealed class FromR2 : IFhirConverter
    {
        /// <summary>The JSON converter for polymorphic deserialization of this version of FHIR.</summary>
        private readonly JsonConverter _jsonConverter;

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        /// Initializes a new instance of the <see cref="FromR2"/> class.
        /// </summary>
        /// -------------------------------------------------------------------------------------------------
        public FromR2() => _jsonConverter = new fhir_2.ResourceConverter();

        /// -------------------------------------------------------------------------------------------------
        /// <summary>Process the structure definition.</summary>
        ///
        /// <param name="sd">          The SD.</param>
        /// <param name="simpleTypes"> [in,out] List of types of the simples.</param>
        /// <param name="complexTypes">[in,out] List of types of the complexes.</param>
        /// <param name="resources">   [in,out] The resources.</param>
        ///
        /// <returns>True if it succeeds, false if it fails.</returns>
        /// -------------------------------------------------------------------------------------------------
        private bool ProcessStructureDef(
            fhir_2.StructureDefinition sd,
            ref Dictionary<string, FhirSimpleType> simpleTypes,
            ref Dictionary<string, FhirComplexType> complexTypes,
            ref Dictionary<string, FhirResource> resources)
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

                        // 4 elements is for a simple type, more is for a complex one
                        if (sd.Snapshot.Element.Length > 4)
                        {
                            return ProcessComplex<FhirComplexType>(sd, ref complexTypes);
                        }

                        return ProcessDataTypeSimple(sd, ref simpleTypes);

                    case "resource":
                        // exclude profiles for now
                        if (!string.IsNullOrEmpty(sd.ConstrainedType))
                        {
                            return true;
                        }

                        return ProcessComplex<FhirResource>(sd, ref resources);

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

        /// -------------------------------------------------------------------------------------------------
        /// <summary>Process a structure definition for a Simple data type.</summary>
        ///
        /// <param name="sd">         The SD.</param>
        /// <param name="simpleTypes">[in,out] List of types of the simples.</param>
        ///
        /// <returns>True if it succeeds, false if it fails.</returns>
        /// -------------------------------------------------------------------------------------------------
        private static bool ProcessDataTypeSimple(
            fhir_2.StructureDefinition sd,
            ref Dictionary<string, FhirSimpleType> simpleTypes)
        {
            try
            {
                // create a new Simple Type object
                FhirSimpleType simple = new FhirSimpleType()
                {
                    Name = sd.Name,
                    NameCapitalized = Utils.Capitalize(sd.Name),
                    StandardStatus = sd.Status,
                    ShortDescription = sd.Description,
                    Definition = sd.Requirements,
                };

                // grab possible types
                string valueType = null;
                string mainType = null;

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
                            mainType = elementType;
                            continue;
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

                // simple type: prefer the 'value' type, if not use main type
                simple.BaseTypeName = valueType ?? mainType;

                // make sure we have a type
                if (string.IsNullOrEmpty(simple.BaseTypeName))
                {
                    Console.WriteLine($"FromR2.ProcessDataTypeSimple <<<" +
                        $" Could not determine base type for {sd.Name}");
                    return false;
                }

                // add to our dictionary of simple types
                simpleTypes[sd.Name] = simple;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"FromR2.ProcessDataTypeSimple <<< failed to process {sd.Id}:\n{ex}\n--------------");
                return false;
            }

            // success
            return true;
        }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>Gets type from element.</summary>
        ///
        /// <param name="structureName">Name of the structure.</param>
        /// <param name="element">      The element.</param>
        /// <param name="elementType">  [out] Type of the element.</param>
        ///
        /// <returns>True if it succeeds, false if it fails.</returns>
        /// -------------------------------------------------------------------------------------------------
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

        /// -------------------------------------------------------------------------------------------------
        /// <summary>Attempts to get expanded types.</summary>
        ///
        /// <param name="element">      The element.</param>
        /// <param name="types">        [out] The types.</param>
        ///
        /// <returns>True if it succeeds, false if it fails.</returns>
        /// -------------------------------------------------------------------------------------------------
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

        /// -------------------------------------------------------------------------------------------------
        /// <summary>Process a complex structure (Complex type or Resource).</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="sd">                The SD.</param>
        /// <param name="complexDefinitions">[in,out] The complex definitions.</param>
        ///
        /// <returns>True if it succeeds, false if it fails.</returns>
        /// -------------------------------------------------------------------------------------------------
        private bool ProcessComplex<T>(
            fhir_2.StructureDefinition sd,
            ref Dictionary<string, T> complexDefinitions)
            where T : FhirTypeBase, new()
        {
            try
            {
                // create a new Complex Type object
                T definition = new T()
                {
                    Name = sd.Name,
                    NameCapitalized = Utils.Capitalize(sd.Name),
                    StandardStatus = sd.Status,
                    ShortDescription = sd.Description,
                    Definition = sd.Requirements,
                };

                // make a dictionary to track all the related resource definitions we create
                Dictionary<string, T> subDefs = new Dictionary<string, T>();

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
                            mainType = elementType;
                            continue;
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
                definition.BaseTypeName = mainType ?? valueType;

                // make sure we have a type
                if (string.IsNullOrEmpty(definition.BaseTypeName))
                {
                    Console.WriteLine($"FromR2.ProcessComplex <<<" +
                        $" Could not determine base type for {sd.Name}");
                    return false;
                }

                // add the current type definition to our internal dict for sanity
                subDefs.Add(sd.Name, definition);

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
                            T sub = new T()
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
                            T sub = new T()
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
                foreach (KeyValuePair<string, T> kvp in subDefs)
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
                Console.WriteLine($"FromR2.ProcessComplex <<< failed to process {sd.Id}:\n{ex}\n--------------");
                return false;
            }

            // success
            return true;
        }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>Attempts to parse resource an object from the given string.</summary>
        ///
        /// <param name="json">The JSON.</param>
        /// <param name="obj"> [out] The object.</param>
        ///
        /// <returns>True if it succeeds, false if it fails.</returns>
        /// -------------------------------------------------------------------------------------------------
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

        /// -------------------------------------------------------------------------------------------------
        /// <summary>Attempts to process resource.</summary>
        ///
        /// <param name="obj">         [out] The object.</param>
        /// <param name="simpleTypes"> [in,out] Simple types.</param>
        /// <param name="complexTypes">[in,out] Complex types.</param>
        /// <param name="resources">   [in,out] Resources.</param>
        ///
        /// <returns>True if it succeeds, false if it fails.</returns>
        /// -------------------------------------------------------------------------------------------------
        bool IFhirConverter.TryProcessResource(
            object obj,
            ref Dictionary<string, FhirSimpleType> simpleTypes,
            ref Dictionary<string, FhirComplexType> complexTypes,
            ref Dictionary<string, FhirResource> resources)
        {
            try
            {
                switch (obj)
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
                            ref simpleTypes,
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
