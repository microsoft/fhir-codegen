using Microsoft.Health.Fhir.SpecManager.Manager;
using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using fhir_2 = Microsoft.Health.Fhir.SpecManager.fhir.v2;
using Microsoft.Health.Fhir.SpecManager.Models;

namespace Microsoft.Health.Fhir.SpecManager.Converters
{
    ///-------------------------------------------------------------------------------------------------
    /// <summary>from v 2.</summary>
    ///
    /// <remarks>Gino Canessa, 2/19/2020.</remarks>
    ///-------------------------------------------------------------------------------------------------

    public class FromV2 : IFhirConverter
    {
        #region Class Variables . . .

        #endregion Class Variables . . .

        #region Instance Variables . . .

        /// <summary>The JSON converter for polymorphic deserialization of this version of FHIR.</summary>
        private JsonConverter _jsonConverter;

        #endregion Instance Variables . . .

        #region Constructors . . .

        public FromV2()
        {
            _jsonConverter = new fhir_2.ResourceConverter();
        }

        #endregion Constructors . . .

        #region Class Interface . . .

        #endregion Class Interface . . .

        #region Instance Interface . . .

        #endregion Instance Interface . . .

        #region Internal Functions . . .

        ///-------------------------------------------------------------------------------------------------
        /// <summary>Process the structure definition.</summary>
        ///
        /// <remarks>Gino Canessa, 2/19/2020.</remarks>
        ///
        /// <param name="sd">          The SD.</param>
        /// <param name="simpleTypes"> [in,out] List of types of the simples.</param>
        /// <param name="complexTypes">[in,out] List of types of the complexes.</param>
        /// <param name="resources">   [in,out] The resources.</param>
        ///
        /// <returns>True if it succeeds, false if it fails.</returns>
        ///-------------------------------------------------------------------------------------------------

        private bool ProcessStructureDef(
                                        fhir_2.StructureDefinition sd,
                                        ref Dictionary<string, FhirSimpleType> simpleTypes,
                                        ref Dictionary<string, FhirComplexType> complexTypes,
                                        ref Dictionary<string, FhirResource> resources
                                        )
        {
            try
            {
                // **** ignore retired ****

                if (sd.Status.Equals("retired", StringComparison.Ordinal))
                {
                    return true;
                }

                // **** act depending on kind ****

                switch (sd.Kind)
                {
                    case "datatype":
                        // **** exclude extensions ****

                        if (sd.ConstrainedType == "Extension")
                        {
                            return true;
                        }

                        // **** 4 elements is for a simple type, more is for a complex one ****

                        if (sd.Snapshot.Element.Length > 4)
                        {
                            return ProcessDataTypeComplex(sd, ref complexTypes);
                        }

                        return ProcessDataTypeSimple(sd, ref simpleTypes);
                    //break;

                    case "resource":

                        // **** exclude profiles for now ****

                        if (!string.IsNullOrEmpty(sd.ConstrainedType))
                        {
                            return true;
                        }

                        return ProcessResource(sd, ref resources);

                        //break;

                    case "logical":
                        // **** ignore logical ****

                        return true;
                        //break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"FromV2.ProcessStructureDef <<< failed to process {sd.Id}:\n{ex}\n--------------");
                return false;
            }

            // **** here means success ****

            return true;
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>Process a structure definition for a Simple data type.</summary>
        ///
        /// <remarks>Gino Canessa, 2/19/2020.</remarks>
        ///
        /// <param name="sd">         The SD.</param>
        /// <param name="simpleTypes">[in,out] List of types of the simples.</param>
        ///
        /// <returns>True if it succeeds, false if it fails.</returns>
        ///-------------------------------------------------------------------------------------------------

        private bool ProcessDataTypeSimple(
                                            fhir_2.StructureDefinition sd, 
                                            ref Dictionary<string, FhirSimpleType> simpleTypes
                                            )
        {
            try
            {
                // **** create a new Simple Type object ****

                FhirSimpleType simple = new FhirSimpleType()
                {
                    Name = sd.Name,
                    NameCapitalized = Utils.Capitalize(sd.Name),
                    StandardStatus = sd.Status,
                    ShortDescription = sd.Description,
                    Definition = sd.Requirements,
                };

                // **** grab possible types ****

                string valueType = null;
                string mainType = null;

                foreach (fhir_2.ElementDefinition element in sd.Snapshot.Element)
                {
                    // **** split the path ****

                    string[] components = element.Path.Split('.');

                    // **** check for base path having a type ****

                    if (components.Length == 1)
                    {
                        if (TryGetTypeFromElement(sd.Name, element, out string elementType))
                        {
                            // **** set our type ****

                            mainType = elementType;
                            continue;
                        }
                    }

                    // **** check for path {type}.value having a type ****

                    if ((components.Length == 2) &&
                        (components[1].Equals("value", StringComparison.Ordinal)))
                    {
                        if (TryGetTypeFromElement(sd.Name, element, out string elementType))
                        {
                            // **** set our type ****

                            valueType = elementType;
                            continue;
                        }
                    }
                }

                // **** simple type: prefer the 'value' type, if not use main type ****

                simple.BaseTypeName = valueType ?? mainType;

                // **** make sure we have a type ****

                if (string.IsNullOrEmpty(simple.BaseTypeName))
                {
                    Console.WriteLine($"FromV2.ProcessDataTypeSimple <<<" +
                        $" Could not determine base type for {sd.Name}");
                    return false;
                }

                // **** add to our dictionary of simple types ****

                simpleTypes[sd.Name] = simple;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"FromV2.ProcessDataTypeSimple <<< failed to process {sd.Id}:\n{ex}\n--------------");
                return false;
            }

            // **** success ****

            return true;
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>Gets type from element.</summary>
        ///
        /// <remarks>Gino Canessa, 2/20/2020.</remarks>
        ///
        /// <param name="structureName">Name of the structure.</param>
        /// <param name="element">      The element.</param>
        /// <param name="elementType">  [out] Type of the element.</param>
        ///
        /// <returns>True if it succeeds, false if it fails.</returns>
        ///-------------------------------------------------------------------------------------------------

        private bool TryGetTypeFromElement(string structureName, fhir_2.ElementDefinition element, out string elementType)
        {
            elementType = null;

            // **** check for declared type ****

            if (element.Type != null)
            {
                foreach (fhir_2.ElementDefinitionType edType in element.Type)
                {
                    // **** check for a specified type ****

                    if (!string.IsNullOrEmpty(edType.Code))
                    {
                        // **** use this type ****

                        elementType = edType.Code;

                        // **** done searching ****

                        return true;
                    }

                    // **** use an extension-defined type ****

                    foreach (fhir_2.Extension ext in edType._Code.Extension)
                    {
                        switch (ext.Url)
                        {
                            case FhirVersionInfo.UrlFhirType:

                                // **** use this type ****

                                elementType = ext.ValueString;

                                // **** stop looking ****

                                return true;
                                //break;

                            case FhirVersionInfo.UrlXmlType:

                                // **** use this type ****

                                elementType = Utils.TypeFromXmlType(ext.ValueString);

                                // **** stop looking ****

                                return true;
                                //break;

                            default:
                                // **** ignore ****
                                break;
                        }
                    }
                }
            }

            // **** check for base derived type ****

            if ((string.IsNullOrEmpty(element.Name)) ||
                (element.Name.Equals(structureName, StringComparison.Ordinal)))
            {
                // **** base type is here ****

                elementType = element.Path;

                // **** done searching ****

                return true;
            }

            // **** no discovered type ****

            elementType = null;
            return false;
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>Attempts to get expanded types.</summary>
        ///
        /// <remarks>Gino Canessa, 2/21/2020.</remarks>
        ///
        /// <param name="element">      The element.</param>
        /// <param name="types">        [out] The types.</param>
        ///
        /// <returns>True if it succeeds, false if it fails.</returns>
        ///-------------------------------------------------------------------------------------------------

        private bool TryGetExpandedTypes(fhir_2.ElementDefinition element, out HashSet<string> types)
        {
            types = new HashSet<string>();

            // **** expanded types must be explicit ****

            if (element.Type == null)
            {
                return false;
            }

            foreach (fhir_2.ElementDefinitionType edType in element.Type)
            {

                // **** check for a specified type ****

                if (!string.IsNullOrEmpty(edType.Code))
                {
                    // **** use this type ****

                    types.Add(edType.Code);

                    // **** check next type ****

                    continue;
                }

                // **** use an extension-defined type ****

                foreach (fhir_2.Extension ext in edType._Code.Extension)
                {
                    switch (ext.Url)
                    {
                        case FhirVersionInfo.UrlFhirType:

                            // **** use this type ****

                            types.Add(ext.ValueString);

                            // **** check next type ****

                            continue;
                        //break;

                        case FhirVersionInfo.UrlXmlType:

                            // **** use this type ****

                            types.Add(Utils.TypeFromXmlType(ext.ValueString));

                            // **** check next type ****

                            continue;
                        //break;

                        default:
                            // **** ignore ****
                            break;
                    }
                }
            }

            // **** check for no discovered types ****

            if (types.Count == 0)
            {
                return false;
            }

            // **** success ****

            return true;
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>Process a structure definition for a complex data type.</summary>
        ///
        /// <remarks>Gino Canessa, 2/19/2020.</remarks>
        ///
        /// <param name="sd">          The SD.</param>
        /// <param name="complexTypes">[in,out] List of types of the complexes.</param>
        ///
        /// <returns>True if it succeeds, false if it fails.</returns>
        ///-------------------------------------------------------------------------------------------------

        private bool ProcessDataTypeComplex(
                                            fhir_2.StructureDefinition sd, 
                                            ref Dictionary<string, FhirComplexType> complexTypes
                                            )
        {
            try
            {
                // **** create a new Complex Type object ****

                FhirComplexType complex = new FhirComplexType()
                {
                    Name = sd.Name,
                    NameCapitalized = Utils.Capitalize(sd.Name),
                    StandardStatus = sd.Status,
                    ShortDescription = sd.Description,
                    Definition = sd.Requirements,
                };

                Dictionary<string, FhirComplexType> subTypes = new Dictionary<string, FhirComplexType>();

                // **** figure out the base type ****

                string mainType = null;
                string valueType = null;

                foreach (fhir_2.ElementDefinition element in sd.Snapshot.Element)
                {
                    // **** split the path ****

                    string[] components = element.Path.Split('.');

                    // **** check for base path having a type ****

                    if (components.Length == 1)
                    {
                        if (TryGetTypeFromElement(sd.Name, element, out string elementType))
                        {
                            // **** set our type ****

                            mainType = elementType;
                            continue;
                        }
                    }

                    // **** check for path {type}.value having a type ****

                    if ((components.Length == 2) &&
                        (components[1].Equals("value", StringComparison.Ordinal)))
                    {
                        if (TryGetTypeFromElement(sd.Name, element, out string elementType))
                        {
                            // **** set our type ****

                            valueType = elementType;
                            continue;
                        }
                    }
                }

                // **** complex type: prefer main type, use 'value' if it isn't present ****

                complex.BaseTypeName = mainType ?? valueType;

                // **** make sure we have a type ****

                if (string.IsNullOrEmpty(complex.BaseTypeName))
                {
                    Console.WriteLine($"FromV2.ProcessDataTypeComplex <<<" +
                        $" Could not determine base type for {sd.Name}");
                    return false;
                }

                // **** add the current type definition to our internal dict for sanity ****

                subTypes.Add(sd.Name, complex);

                // **** look for properties on this type ****

                foreach (fhir_2.ElementDefinition element in sd.Snapshot.Element)
                {
                    // **** split the path into component parts ****

                    string[] components = element.Path.Split('.');

                    // **** base definition, already processed ****

                    if (components.Length < 2)
                    {
                        continue;
                    }

                    // **** grab field info we need ****

                    Utils.GetParentAndField(components, out string field, out string parent);

                    string elementType;

                    // **** if we can't find a type, assume Element ****

                    if (!TryGetTypeFromElement(parent, element, out elementType))
                    {
                        elementType = "Element";
                    }

                    // **** check to see if we do NOT have this parent, but do have a definition ****

                    if (!subTypes.ContainsKey(parent))
                    {
                        // **** figure out if we have this parent as a field ****

                        string[] parentComponents = new string[components.Length - 1];
                        Array.Copy(components, 0, parentComponents, 0, components.Length - 1);

                        // **** get parent info ****

                        Utils.GetParentAndField(parentComponents, out string pField, out string pParent);

                        if ((subTypes.ContainsKey(pParent)) &&
                            (subTypes[pParent].Properties.ContainsKey(pField)))
                        {
                            // **** use this type ****

                            FhirComplexType sub = new FhirComplexType()
                            {
                                Name = subTypes[pParent].Properties[pField].Name,
                                NameCapitalized = subTypes[pParent].Properties[pField].NameCapitalized,
                                ShortDescription = subTypes[pParent].Properties[pField].ShortDescription,
                                Definition = subTypes[pParent].Properties[pField].Definition,
                                Comment = subTypes[pParent].Properties[pField].Comment,
                                BaseTypeName = subTypes[pParent].Properties[pField].BaseTypeName,
                            };

                            // **** add this parent to our local dictionary ****

                            subTypes.Add(parent, sub);
                        }
                        else
                        {
                            // **** add a placeholder type ****

                            FhirComplexType sub = new FhirComplexType()
                            {
                                Name = parent,
                                NameCapitalized = Utils.Capitalize(parent),
                                ShortDescription = "",
                                Definition = "",
                                Comment = $"Placeholder for {parent}",
                                BaseTypeName = parent,
                                IsPlaceholder = true,
                            };

                            // **** add this parent to our local dictionary ****

                            subTypes.Add(parent, sub);
                        }
                    }

                    // **** add this field to the parent type ****

                    subTypes[parent].Properties.Add(
                        field,
                        new FhirProperty()
                        {
                            Name = field,
                            NameCapitalized = Utils.Capitalize(field),
                            ShortDescription = element.Short,
                            Definition = element.Definition,
                            Comment = element.Comment,
                            BaseTypeName = elementType,
                            CardinalityMin = element.Min ?? 0,
                            CardinaltiyMax = Utils.MaxCardinality(element.Max),
                        });

                }

                // **** copy over our definitions ****

                foreach (KeyValuePair<string, FhirComplexType> kvp in subTypes)
                {
                    // **** check for removing a placeholder ****

                    if ((complexTypes.ContainsKey(kvp.Key)) &&
                        (complexTypes[kvp.Key].IsPlaceholder == true))
                    {
                        complexTypes.Remove(kvp.Key);
                    }

                    // **** check for not being present ****

                    if (!complexTypes.ContainsKey(kvp.Key))
                    {
                        complexTypes.Add(kvp.Key, kvp.Value);
                        continue;
                    }

                    // **** check fields ****

                    foreach (KeyValuePair<string, FhirProperty> propKvp in kvp.Value.Properties)
                    {
                        if (!complexTypes[kvp.Key].Properties.ContainsKey(propKvp.Key))
                        {
                            complexTypes[kvp.Key].Properties.Add(propKvp.Key, propKvp.Value);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"FromV2.ProcessDataTypeComplex <<< failed to process {sd.Id}:\n{ex}\n--------------");
                return false;
            }
            // **** success ****

            return true;
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>Process the resource.</summary>
        ///
        /// <remarks>Gino Canessa, 2/21/2020.</remarks>
        ///
        /// <param name="sd">       The SD.</param>
        /// <param name="resources">[in,out] The resources.</param>
        ///
        /// <returns>True if it succeeds, false if it fails.</returns>
        ///-------------------------------------------------------------------------------------------------

        private bool ProcessResource(
                                    fhir_2.StructureDefinition sd,
                                    ref Dictionary<string, FhirResource> resources
                                    )
        {
            try
            {
                // **** create a new Complex Type object ****

                FhirResource resource = new FhirResource()
                {
                    Name = sd.Name,
                    NameCapitalized = Utils.Capitalize(sd.Name),
                    StandardStatus = sd.Status,
                    ShortDescription = sd.Description,
                    Definition = sd.Requirements,
                };

                // **** make a dictionary to track all the related resource definitions we create ****

                Dictionary<string, FhirResource> subResources = new Dictionary<string, FhirResource>();

                Dictionary<string, string> aliasTable = new Dictionary<string, string>();

                // **** figure out the base type ****

                string mainType = null;
                string valueType = null;

                foreach (fhir_2.ElementDefinition element in sd.Snapshot.Element)
                {
                    // **** split the path ****

                    string[] components = element.Path.Split('.');

                    // **** check for base path having a type ****

                    if (components.Length == 1)
                    {
                        if (TryGetTypeFromElement(sd.Name, element, out string elementType))
                        {
                            // **** set our type ****

                            mainType = elementType;
                            continue;
                        }
                    }

                    // **** check for path {type}.value having a type ****

                    if ((components.Length == 2) &&
                        (components[1].Equals("value", StringComparison.Ordinal)))
                    {
                        if (TryGetTypeFromElement(sd.Name, element, out string elementType))
                        {
                            // **** set our type ****

                            valueType = elementType;
                            continue;
                        }
                    }
                }

                // **** resource: prefer main type, use 'value' if it isn't present ****

                resource.BaseTypeName = mainType ?? valueType;

                // **** make sure we have a type ****

                if (string.IsNullOrEmpty(resource.BaseTypeName))
                {
                    Console.WriteLine($"FromV2.ProcessResource <<<" +
                        $" Could not determine base type for {sd.Name}");
                    return false;
                }

                // **** add the current type definition to our internal dict for sanity ****

                subResources.Add(sd.Name, resource);

                // **** look for properties on this type ****

                foreach (fhir_2.ElementDefinition element in sd.Snapshot.Element)
                {
                    // **** split the path into component parts ****

                    string[] components = element.Path.Split('.');

                    // **** base definition, already processed ****

                    if (components.Length < 2)
                    {
                        continue;
                    }

                    // **** grab field info we need ****

                    Utils.GetParentAndField(components, out string field, out string parent);

                    string elementType;
                    HashSet<string> expandedTypes = null;

                    // **** determine if there is type expansion ****

                    if (field.Contains("[x]"))
                    {
                        // **** fix the field name ****

                        field = field.Replace("[x]", "");

                        // **** no base type ****

                        elementType = "";

                        // **** get multiple types ****

                        if (!TryGetExpandedTypes(element, out expandedTypes))
                        {
                            Console.WriteLine($"FromV2.ProcessResource <<<" +
                                $" Could not get expanded types for {sd.Name} field {element.Path}");
                            return false;
                        }
                    }
                    else if (!string.IsNullOrEmpty(element.NameReference))
                    {
                        // **** look up the named reference in the alias table ****

                        if (!aliasTable.ContainsKey(element.NameReference))
                        {
                            Console.WriteLine($"FromV2.ProcessResource <<<" +
                                $" Could not find named reference {element.NameReference} in {sd.Name} field {element.Path}");
                            return false;
                        }

                        // **** use the named type ****

                        elementType = aliasTable[element.NameReference];
                    }
                    else
                    {
                        // **** if we can't find a type, assume Element ****

                        if (!TryGetTypeFromElement(parent, element, out elementType))
                        {
                            elementType = "Element";
                        }
                    }

                    // **** check to see if we do NOT have this parent, but do have a definition ****

                    if (!subResources.ContainsKey(parent))
                    {
                        // **** figure out if we have this parent as a field ****

                        string[] parentComponents = new string[components.Length - 1];
                        Array.Copy(components, 0, parentComponents, 0, components.Length - 1);

                        // **** get parent info ****

                        Utils.GetParentAndField(parentComponents, out string pField, out string pParent);

                        if ((subResources.ContainsKey(pParent)) &&
                            (subResources[pParent].Properties.ContainsKey(pField)))
                        {
                            // **** use this type ****

                            FhirResource sub = new FhirResource()
                            {
                                Name = Utils.Capitalize(parent),
                                NameCapitalized = Utils.Capitalize(parent),
                                ShortDescription = subResources[pParent].Properties[pField].ShortDescription,
                                Definition = subResources[pParent].Properties[pField].Definition,
                                Comment = subResources[pParent].Properties[pField].Comment,
                                BaseTypeName = subResources[pParent].Properties[pField].BaseTypeName,
                            };

                            // **** add this parent to our local dictionary ****

                            subResources.Add(parent, sub);

                            // **** change our element type to point at the parent ****

                            subResources[pParent].Properties[pField].BaseTypeName = Utils.Capitalize(parent);
                        }
                        else
                        {
                            // **** add a placeholder type ****

                            FhirResource sub = new FhirResource()
                            {
                                Name = Utils.Capitalize(parent),
                                NameCapitalized = Utils.Capitalize(parent),
                                ShortDescription = "",
                                Definition = "",
                                Comment = $"Placeholder for {parent}",
                                BaseTypeName = parent,
                                IsPlaceholder = true,
                            };

                            // **** add this parent to our local dictionary ****

                            subResources.Add(parent, sub);
                        }
                    }

                    // **** add this field to the parent type ****

                    subResources[parent].Properties.Add(
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

                    // **** check to see if we need to insert into our alias table ****

                    if (!string.IsNullOrEmpty(element.Name))
                    {
                        // **** add this record, with it's current path ****

                        aliasTable.Add(element.Name, $"{parent}{subResources[parent].Properties[field].NameCapitalized}");
                    }
                }

                // **** copy over our definitions ****

                foreach (KeyValuePair<string, FhirResource> kvp in subResources)
                {
                    // **** check for removing a placeholder ****

                    if ((resources.ContainsKey(kvp.Key)) &&
                        (resources[kvp.Key].IsPlaceholder == true))
                    {
                        resources.Remove(kvp.Key);
                    }

                    // **** check for not being present ****

                    if (!resources.ContainsKey(kvp.Key))
                    {
                        resources.Add(kvp.Key, kvp.Value);
                        continue;
                    }

                    // **** check fields ****

                    foreach (KeyValuePair<string, FhirProperty> propKvp in kvp.Value.Properties)
                    {
                        if (!resources[kvp.Key].Properties.ContainsKey(propKvp.Key))
                        {
                            resources[kvp.Key].Properties.Add(propKvp.Key, propKvp.Value);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"FromV2.ProcessResource <<< failed to process {sd.Id}:\n{ex}\n--------------");
                return false;
            }
            // **** success ****

            return true;
        }

        #endregion Internal Functions . . .

        #region IFhirConverter . . .

        ///-------------------------------------------------------------------------------------------------
        /// <summary>Attempts to parse resource an object from the given string.</summary>
        ///
        /// <remarks>Gino Canessa, 2/19/2020.</remarks>
        ///
        /// <param name="json">The JSON.</param>
        /// <param name="obj"> [out] The object.</param>
        ///
        /// <returns>True if it succeeds, false if it fails.</returns>
        ///-------------------------------------------------------------------------------------------------

        bool IFhirConverter.TryParseResource(string json, out object obj)
        {
            try
            {
                // **** try to parse this JSON into a resource object ****

                obj = JsonConvert.DeserializeObject<fhir_2.Resource>(json, _jsonConverter);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"FromV2.TryParseResource <<< failed to parse:\n{ex}\n------------------------------------");
            }

            // **** failed to parse ****

            obj = null;
            return false;
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>Attempts to process resource.</summary>
        ///
        /// <remarks>Gino Canessa, 2/19/2020.</remarks>
        ///
        /// <param name="obj">            [out] The object.</param>
        /// <param name="fhirVersionInfo">[in,out] Information describing the fhir version.</param>
        ///
        /// <returns>True if it succeeds, false if it fails.</returns>
        ///-------------------------------------------------------------------------------------------------

        bool IFhirConverter.TryProcessResource(
                                                object obj,
                                                ref Dictionary<string, FhirSimpleType> simpleTypes,
                                                ref Dictionary<string, FhirComplexType> complexTypes,
                                                ref Dictionary<string, FhirResource> resources
                                                )
        {
            try
            {

            switch (obj)
            {
                case fhir_2.Conformance conformance:
                    // **** ignore for now ****

                    return true;
                //break;

                case fhir_2.NamingSystem namingSystem:
                    // **** ignore for now ****

                    return true;
                //break;

                case fhir_2.OperationDefinition operationDefinition:
                    // **** ignore for now ****

                    return true;
                //break;

                case fhir_2.SearchParameter searchParameter:
                    // **** ignore for now ****

                    return true;
                //break;

                case fhir_2.StructureDefinition structureDefinition:
                    return ProcessStructureDef(
                        structureDefinition,
                        ref simpleTypes,
                        ref complexTypes,
                        ref resources
                        );
                //break;

                case fhir_2.ValueSet valueSet:
                    // **** ignore for now ****

                    return true;
                    //break;
            }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"FromV2.TryProcessResource <<< Failed to process resource:\n{ex}\n--------------");
                return false;
            }

            // **** ignored ****

            return true;
        }

        #endregion IFhirConverter . . .


    }
}
