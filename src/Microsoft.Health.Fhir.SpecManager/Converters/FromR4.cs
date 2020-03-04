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
            if (sd.Status.Equals("retired", StringComparison.Ordinal))
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
                        ProcessComplex(sd, fhirVersionInfo, false);
                    }

                    break;

                case "resource":
                    // exclude profiles
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
        /// <param name="sd">             The structure definition we are parsing.</param>
        /// <param name="fhirVersionInfo">FHIR Version information.</param>
        private static void ProcessExtension(
            fhir_4.StructureDefinition sd,
            FhirVersionInfo fhirVersionInfo)
        {
            return;
            /*
            List<string> elementPaths = new List<string>();
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
            foreach (fhir_4.StructureDefinitionContext context in sd.Context)
            {
                if (context.Type != "element")
                {
                    throw new ArgumentException($"Invalid extension context type: {context.Type}");
                }

                elementPaths.Add(context.Expression);
            }

            if (sd.Snapshot.Element.Length > 5)
            {
                Console.Write(string.Empty);
            }

            string description = string.Empty;
            string definition = string.Empty;
            string comment = string.Empty;

            Dictionary<string, FhirElement> properties = new Dictionary<string, FhirElement>();

            // traverse elements looking for data we need
            foreach (fhir_4.ElementDefinition element in sd.Snapshot.Element)
            {
                string path = element.Path;
                string[] components = element.Path.Split(_pathSeperators);
                string field = string.Empty;
                List<string> targetProfiles = new List<string>();

                HashSet<string> choiceTypes = null;

                switch (path)
                {
                    case "Extension.value[x]":
                        // grab types
                        if (element.Type != null)
                        {
                            field = field.Replace("[x]", string.Empty);
                            path = path.Replace("[x]", string.Empty);

                            // traverse allowed types
                            foreach (fhir_4.ElementDefinitionType type in element.Type)
                            {
                                if (!choiceTypes.Contains(type.Code))
                                {
                                    choiceTypes.Add(type.Code);
                                }

                                if (type.TargetProfile != null)
                                {
                                    foreach (string profile in type.TargetProfile)
                                    {
                                        allowedTypesAndProfiles[type.Code].Add(
                                            profile.Substring(profile.LastIndexOf('/') + 1));
                                    }
                                }
                            }

                            // create a new property for this value
                            FhirElement property = new FhirElement(
                                path,
                                null,
                                properties.Count,
                                element.Short,
                                element.Definition,
                                element.Comment,
                                null,
                                string.Empty,
                                choiceTypes,
                                (int)(element.Min ?? 0),
                                element.Max,
                                targetProfiles);
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

                        description = element.Short;
                        definition = element.Definition;
                        comment = element.Comment;

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
                sd.Status,
                description,
                definition,
                comment,
                elementPaths,
                isModifier,
                isSummary);

            // add our property extension
            fhirVersionInfo.AddExtension(extension);
            */
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
        /// <param name="sd">             The structure definition to parse.</param>
        /// <param name="fhirVersionInfo">FHIR Version information.</param>
        /// <param name="isResource">     True if is resource, false if not.</param>
        private static void ProcessComplex(
            fhir_4.StructureDefinition sd,
            FhirVersionInfo fhirVersionInfo,
            bool isResource)
        {
            // create a new complex type object for this type or resource
            FhirComplex complex = new FhirComplex(
                sd.Name,
                new Uri(sd.Url),
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
                string path = element.Path;
                Dictionary<string, FhirElementType> elementTypes = null;
                string elementType = string.Empty;

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
                    // throw new InvalidDataException($"Could not find parent for {element.Path}!");
                    // should load later
                    // TODO: figure out a way to verify all dependencies loaded
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
                    path = path.Replace("[x]", string.Empty);
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

                // add this field to the parent type
                parent.Elements.Add(
                    path,
                    new FhirElement(
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
                        element.Max));
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
    }
}
