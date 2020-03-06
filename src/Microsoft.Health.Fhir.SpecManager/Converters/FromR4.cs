﻿// -------------------------------------------------------------------------------------------------
// <copyright file="FromR4.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>
// -------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Health.Fhir.SpecManager.fhir.r2;
using Microsoft.Health.Fhir.SpecManager.Manager;
using Microsoft.Health.Fhir.SpecManager.Models;
using Newtonsoft.Json;
using fhir_4 = Microsoft.Health.Fhir.SpecManager.fhir.r4;

namespace Microsoft.Health.Fhir.SpecManager.Converters
{
    /// <summary>Convert FHIR R4 into local definitions.</summary>
    public sealed class FromR4 : IFhirConverter
    {
        /// <summary>The path seperators.</summary>
        private static readonly char[] _pathSeperators = new char[] { '.', ':' };

        /// <summary>The JSON converter for polymorphic deserialization of this version of FHIR.</summary>
        private readonly JsonConverter _jsonConverter;

        /// <summary>
        /// Initializes a new instance of the <see cref="FromR4"/> class.
        /// </summary>
        public FromR4() => _jsonConverter = new fhir_4.ResourceConverter();

        /// <summary>Process the operation.</summary>
        /// <param name="op">             The operation.</param>
        /// <param name="fhirVersionInfo">FHIR Version information.</param>
        private void ProcessOperation(
            fhir_4.OperationDefinition op,
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
                foreach (fhir_4.OperationDefinitionParameter opParam in op.Parameter)
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
        /// <param name="sp">             The search parameter.</param>
        /// <param name="fhirVersionInfo">FHIR Version information.</param>
        private void ProcessSearchParam(
            fhir_4.SearchParameter sp,
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
        /// <param name="sd">             The structure definition we are parsing.</param>
        /// <param name="fhirVersionInfo">FHIR Version information.</param>
        private void ProcessStructureDef(
            fhir_4.StructureDefinition sd,
            FhirVersionInfo fhirVersionInfo)
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
                    // exclude extensions
                    if (sd.Type != "Extension")
                    {
                        ProcessDataTypePrimitive(sd, fhirVersionInfo);
                    }

                    break;

                case "complex-type":
                    // exclude extensions and profiles
                    if ((sd.Type != "Extension") &&
                        (sd.Derivation != "constraint"))
                    {
                        ProcessComplex(sd, fhirVersionInfo, FhirComplex.FhirComplexType.DataType);
                    }

                    break;

                case "resource":
                    // exclude profiles
                    if (sd.Derivation != "constraint")
                    {
                        ProcessComplex(sd, fhirVersionInfo, FhirComplex.FhirComplexType.Resource);
                    }

                    break;
            }
        }

        /// <summary>Process the structure definition of an extension.</summary>
        /// <param name="sd">             The structure definition we are parsing.</param>
        /// <param name="fhirVersionInfo">FHIR Version information.</param>
        private void ProcessStructureDefExtension(
            fhir_4.StructureDefinition sd,
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
                    Console.Write(string.Empty);

                    break;

                case "complex-type":
                    // include extensions and profiles
                    if ((sd.Type == "Extension") ||
                        (sd.Derivation == "constraint"))
                    {
                        ProcessComplex(sd, fhirVersionInfo, FhirComplex.FhirComplexType.Extension);
                    }

                    break;

                case "resource":
                    // include profiles
                    if (sd.Derivation == "constraint")
                    {
                        ProcessComplex(sd, fhirVersionInfo, FhirComplex.FhirComplexType.Extension);
                    }

                    break;
            }
        }

        /// <summary>Process a structure definition for a Primitive data type.</summary>
        /// <param name="sd">             The structure definition.</param>
        /// <param name="fhirVersionInfo">FHIR Version information.</param>
        private static void ProcessDataTypePrimitive(
            fhir_4.StructureDefinition sd,
            FhirVersionInfo fhirVersionInfo)
        {
            // create a new primitive type object
            FhirPrimitive primitive = new FhirPrimitive(
                sd.Id,
                sd.Name,
                new Uri(sd.Url),
                sd.Status,
                sd.Description,
                sd.Purpose,
                string.Empty,
                null);

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
            fhir_4.ElementDefinition element,
            out Dictionary<string, FhirElementType> elementTypes)
        {
            elementTypes = new Dictionary<string, FhirElementType>();

            // check for declared type
            if (element.Type != null)
            {
                foreach (fhir_4.ElementDefinitionType edType in element.Type)
                {
                    // check for a specified type
                    if (!string.IsNullOrEmpty(edType.Code))
                    {
                        // create a type for this code
                        FhirElementType elementType = new FhirElementType(edType.Code, edType.TargetProfile);

                        // add to our dictionary
                        elementTypes.Add(elementType.Code, elementType);
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
                elementTypes.Add(elementType.Code, elementType);

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
            fhir_4.ElementDefinition[] elements,
            out Dictionary<string, FhirElementType> elementTypes)
        {
            elementTypes = null;

            foreach (fhir_4.ElementDefinition element in elements)
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
        /// <param name="definitionComplexType">Type of strcuture definition we are parsing.</param>
        private static void ProcessComplex(
            fhir_4.StructureDefinition sd,
            FhirVersionInfo fhirVersionInfo,
            FhirComplex.FhirComplexType definitionComplexType)
        {
            try
            {
                if ((sd.Snapshot == null) || (sd.Snapshot.Element == null))
                {
                    return;
                }

                List<string> contextElements = new List<string>();

                // create a new complex type object for this type or resource
                FhirComplex complex = new FhirComplex(
                    sd.Id,
                    sd.Name,
                    new Uri(sd.Url),
                    sd.Status,
                    sd.Description,
                    sd.Purpose,
                    string.Empty,
                    null);

                if (sd.Context != null)
                {
                    foreach (fhir_4.StructureDefinitionContext context in sd.Context)
                    {
                        if (context.Type != "element")
                        {
                            throw new ArgumentException($"Invalid extension context type: {context.Type}");
                        }

                        contextElements.Add(context.Expression);
                    }
                }

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

                    complex.BaseTypeName = baseTypes.ElementAt(0).Value.Code;
                }

                // look for properties on this type
                foreach (fhir_4.ElementDefinition element in sd.Snapshot.Element)
                {
                    if (element.Id == "Observation.code.coding:BMICode")
                    {
                        Console.Write(string.Empty);
                    }

                    string id = element.Id;
                    string path = element.Path;
                    Dictionary<string, FhirElementType> elementTypes = null;
                    string elementType = string.Empty;

                    // split the id into component parts
                    string[] idComponents = id.Split('.');
                    string[] pathComponents = path.Split('.');

                    // base definition, already processed
                    if (pathComponents.Length < 2)
                    {
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
                        // add this slice to the field
                        parent.Elements[field].AddSlice(sd.Url, sliceName);

                        // only slice parent has slice name
                        continue;
                    }

                    // if we can't find a type, assume Element
                    if (!TryGetTypeFromElement(parent.Name, element, out elementTypes))
                    {
                        elementType = "Element";
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
                            fixedValue));

                    if (element.Slicing != null)
                    {
                        List<FhirSliceDiscriminatorRule> discriminatorRules = new List<FhirSliceDiscriminatorRule>();

                        if (element.Slicing.Discriminator == null)
                        {
                            throw new InvalidDataException($"Missing slicing discriminator: {sd.Name} - {element.Path}");
                        }

                        foreach (fhir_4.ElementDefinitionSlicingDiscriminator discriminator in element.Slicing.Discriminator)
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

                switch (definitionComplexType)
                {
                    case FhirComplex.FhirComplexType.DataType:
                        fhirVersionInfo.AddComplexType(complex);
                        break;
                    case FhirComplex.FhirComplexType.Resource:
                        fhirVersionInfo.AddResource(complex);
                        break;
                    case FhirComplex.FhirComplexType.Extension:
                        fhirVersionInfo.AddExtension(contextElements, complex);
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
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
                return JsonConvert.DeserializeObject<fhir_4.Resource>(json, _jsonConverter);
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"FromR4.ParseResource <<< failed to parse:\n{ex}\n------------------------------------");
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
                    // case fhir_4.StructureMap structureMap:
                    // case fhir_4.ValueSet valueSet:

                    // process
                    case fhir_4.OperationDefinition operationDefinition:
                        ProcessOperation(operationDefinition, fhirVersionInfo);
                        break;

                    case fhir_4.SearchParameter searchParameter:
                        ProcessSearchParam(searchParameter, fhirVersionInfo);
                        break;

                    case fhir_4.StructureDefinition structureDefinition:
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
            catch (Exception ex)
            {
                Console.WriteLine($"FromR4.TryProcessResource <<< Failed to process resource:\n{ex}\n--------------");
                throw;
            }
        }

        /// <summary>Gets default value if present.</summary>
        /// <param name="element">     The element.</param>
        /// <param name="defaultName"> [out] The default name.</param>
        /// <param name="defaultValue">[out] The default value.</param>
        private static void GetDefaultValueIfPresent(
            fhir_4.ElementDefinition element,
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
            fhir_4.ElementDefinition element,
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
