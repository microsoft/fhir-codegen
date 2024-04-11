// <copyright file="DefinitionCollection.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>


using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Xml.Linq;
using Hl7.Fhir.Model;
using Hl7.Fhir.Specification.Terminology;
using Microsoft.Health.Fhir.CodeGen.FhirExtensions;
using Microsoft.Health.Fhir.CodeGenCommon.Extensions;
using Microsoft.Health.Fhir.CodeGenCommon.FhirExtensions;
using Microsoft.Health.Fhir.CodeGenCommon.Models;
using Microsoft.Health.Fhir.CodeGenCommon.Packaging;

namespace Microsoft.Health.Fhir.CodeGen.Models;

/// <summary>A FHIR package and its contents.</summary>
public partial class DefinitionCollection
{
    /// <summary>Gets or sets the name.</summary>
    public required string Name { get; set; }

    ///// <summary>Gets or sets the transmit servers.</summary>
    //public string[] TxServers { get; set; } = Array.Empty<string>();

    /// <summary>Gets or sets the FHIR version.</summary>
    public FHIRVersion? FhirVersion { get; set; } = null;

    /// <summary>Gets or sets the FHIR version literal.</summary>
    public string FhirVersionLiteral { get; set; } = string.Empty;

    public FhirReleases.FhirSequenceCodes FhirSequence { get; set; } = FhirReleases.FhirSequenceCodes.Unknown;

    /// <summary>Gets or sets the identifier of the main package.</summary>
    public string MainPackageId { get; set; } = string.Empty;

    /// <summary>Gets or sets the main package canonical.</summary>
    public string MainPackageCanonical { get; set; } = string.Empty;

    /// <summary>Gets or sets the main package version.</summary>
    public string MainPackageVersion { get; set; } = string.Empty;

    /// <summary>Gets or sets the manifest.</summary>
    public Dictionary<string, CachePackageManifest> Manifests { get; set; } = [];

    /// <summary>Gets or sets the contents.</summary>
    public Dictionary<string, PackageContents> ContentListings { get; set; } = [];

    //private readonly Dictionary<ElementDefinition, StructureDefinition> _elementSdLookup = new();

    private readonly Dictionary<string, StructureDefinition> _primitiveTypesByName = [];
    private readonly Dictionary<string, StructureDefinition> _complexTypesByName = [];
    private readonly Dictionary<string, StructureDefinition> _resourcesByName = [];
    private readonly Dictionary<string, StructureDefinition> _logicalModelsByName = [];
    private readonly Dictionary<string, StructureDefinition> _extensionsByUrl = [];
    private readonly Dictionary<string, Dictionary<string, StructureDefinition>> _extensionsByPath = [];
    private readonly Dictionary<string, StructureDefinition> _profilesByUrl = [];
    private readonly Dictionary<string, Dictionary<string, StructureDefinition>> _profilesByBaseType = [];

    private readonly Dictionary<string, OperationDefinition> _systemOperations = [];
    private readonly Dictionary<string, OperationDefinition> _operationsByUrl = [];
    private readonly Dictionary<string, Dictionary<string, OperationDefinition>> _typeOperationsByType = [];
    private readonly Dictionary<string, Dictionary<string, OperationDefinition>> _instanceOperationsByType = [];

    private readonly Dictionary<string, SearchParameter> _globalSearchParameters = [];
    private readonly Dictionary<string, FhirQueryParameter> _searchResultParameters = [];
    private readonly Dictionary<string, FhirQueryParameter> _allInteractionParameters = [];
    private readonly Dictionary<string, SearchParameter> _searchParamsByUrl = [];
    private readonly Dictionary<string, Dictionary<string, SearchParameter>> _searchParamsByBase = [];

    private readonly Dictionary<string, CodeSystem> _codeSystemsByUrl = [];
    private readonly Dictionary<string, ValueSet> _valueSetsByVersionedUrl = [];
    private readonly Dictionary<string, string[]> _valueSetVersions = [];
    private readonly Dictionary<string, string> _valueSetUrlsById = [];

    private readonly Dictionary<string, List<StructureElementCollection>> _coreBindingEdsByPathByValueSet = [];
    private readonly Dictionary<string, List<StructureElementCollection>> _extendedBindingEdsByPathByValueSet = [];

    private readonly Dictionary<string, ImplementationGuide> _implementationGuidesByUrl = [];
    private readonly Dictionary<string, CapabilityStatement> _capabilityStatementsByUrl = [];
    private readonly Dictionary<string, CompartmentDefinition> _compartmentsByUrl = [];

    internal readonly Dictionary<string, string> _parentElementsAndType = [];

    private readonly List<string> _errors = [];

    /// <summary>(Immutable) all resources.</summary>
    private readonly Dictionary<string, Resource> _allResources = [];

    /// <summary>(Immutable) The canonical resources.</summary>
    private readonly Dictionary<string, Dictionary<string, IConformanceResource>> _canonicalResources = [];

    /// <summary>(Immutable) The local transmit.</summary>
    private readonly LocalTerminologyService _localTx;

    /// <summary>
    /// Initializes a new instance of the <see cref="DefinitionCollection"/> class.
    /// </summary>
    public DefinitionCollection()
    {
        _localTx = new LocalTerminologyService(this);
    }

    /// <summary>Query if 'path' has child elements.</summary>
    /// <param name="path">Dot-notation path to the element.</param>
    /// <returns>True if the path contains child elements, false if not.</returns>
    public bool HasChildElements(string path) => _parentElementsAndType.ContainsKey(path);

    /// <summary>Query if 'path' is backbone element.</summary>
    /// <param name="path">Dot-notation path to the element.</param>
    /// <returns>True if backbone element, false if not.</returns>
    public bool IsBackboneElement(string path) =>
        _parentElementsAndType.TryGetValue(path, out string? type) &&
        ((type == "BackboneElement") || (type == "Element"));

    internal bool TryUpdateElement(StructureDefinition destinationSd, ElementDefinition ed)
    {
        // lookup the structure locally because the parameter may be a copy or read only
        if ((!_complexTypesByName.TryGetValue(destinationSd.Name, out StructureDefinition? sd)) &&
            (!_resourcesByName.TryGetValue(destinationSd.Name, out sd)) &&
            (!_profilesByUrl.TryGetValue(destinationSd.Url, out sd)))
        {
            throw new Exception($"Failed to find StructureDefinition id: {destinationSd.Id}, name: {destinationSd.Name}, url: {destinationSd.Url}");
        }

        int sdFieldOrder = ed.GetIntegerExtension(CommonDefinitions.ExtUrlEdFieldOrder) ?? -1;
        int componentFieldOrder = ed.GetIntegerExtension(CommonDefinitions.ExtUrlEdComponentFieldOrder) ?? -1;

        if ((sdFieldOrder == -1) || (componentFieldOrder == -1))
        {
            throw new Exception(
                $"ElementDefinitions must be annotated with extensions: " +
                $"{CommonDefinitions.ExtUrlEdFieldOrder} and " +
                $"{CommonDefinitions.ExtUrlEdComponentFieldOrder}");
        }

        // check for adding to snapshot
        if ((sd.Snapshot != null) && (sd.Snapshot.Element.Count != 0))
        {
            // find the element to insert after
            int matchIndex = sd.Snapshot.Element.FindIndex(
                e => (e.cgFieldOrder() == sdFieldOrder) && (e.cgComponentFieldOrder() == componentFieldOrder));
            sd.Snapshot.Element[matchIndex] = ed;
        }

        // check for adding to differential
        if ((sd.Differential != null) && (sd.Differential.Element.Count != 0))
        {
            // find the element to insert after
            int matchIndex = sd.Differential.Element.FindIndex(
                e => (e.cgFieldOrder() == sdFieldOrder) && (e.cgComponentFieldOrder() == componentFieldOrder));
            sd.Differential.Element[matchIndex] = ed;
        }

        return true;
    }

    /// <summary>Attempts to insert element, must be annotated with field orders.</summary>
    /// <param name="destinationSd">           Destination SD.</param>
    /// <param name="ed">                      The <see cref="ElementDefinition"/> to add the
    ///  missing ID to.</param>
    /// <param name="increaseSubsequentOrders">True to increase subsequent orders.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    internal bool TryInsertElement(
        StructureDefinition destinationSd,
        ElementDefinition ed,
        bool increaseSubsequentOrders)
    {
        // lookup the structure locally because the parameter may be a copy or read only
        if ((!_complexTypesByName.TryGetValue(destinationSd.Name, out StructureDefinition? sd)) &&
            (!_resourcesByName.TryGetValue(destinationSd.Name, out sd)) &&
            (!_profilesByUrl.TryGetValue(destinationSd.Url, out sd)))
        {
            throw new Exception($"Failed to find StructureDefinition id: {destinationSd.Id}, name: {destinationSd.Name}, url: {destinationSd.Url}");
        }

        int sdFieldOrder = ed.GetIntegerExtension(CommonDefinitions.ExtUrlEdFieldOrder) ?? -1;
        int componentFieldOrder = ed.GetIntegerExtension(CommonDefinitions.ExtUrlEdComponentFieldOrder) ?? -1;

        if ((sdFieldOrder == -1) || (componentFieldOrder == -1))
        {
            throw new Exception(
                $"ElementDefinitions to insert must be annotated with extensions: " +
                $"{CommonDefinitions.ExtUrlEdFieldOrder} and " +
                $"{CommonDefinitions.ExtUrlEdComponentFieldOrder}");
        }

        int lastDot = ed.Path.LastIndexOf('.');
        string parentPath = lastDot == -1 ? ed.Path : ed.Path.Substring(0, lastDot);

        //// add to lookup dict
        //_elementSdLookup.Add($"{ed.Path}|{ed.ElementId}", sd);

        // check for a value set binding
        CheckElementBindings(sd.cgArtifactClass(), sd, ed);

        // check for adding to snapshot
        if ((sd.Snapshot != null) && (sd.Snapshot.Element.Count != 0))
        {
            // find the element to insert after
            int matchIndex = sd.Snapshot.Element.FindIndex(e => e.cgFieldOrder() == sdFieldOrder);

            if (matchIndex == -1)
            {
                // add to the end
                sd.Snapshot.Element.Add(ed);
            }
            else
            {
                sd.Snapshot.Element.Insert(matchIndex, ed);

                if (increaseSubsequentOrders)
                {
                    // increase the order of all subsequent elements
                    foreach (ElementDefinition e in sd.Snapshot.Element.Skip(matchIndex + 1))
                    {
                        int currentOrder = e.cgFieldOrder();
                        e.SetIntegerExtension(CommonDefinitions.ExtUrlEdFieldOrder, currentOrder + 1);

                        int currentLastDot = e.Path.LastIndexOf('.');
                        string currentParentPath = currentLastDot == -1 ? e.Path : e.Path.Substring(0, currentLastDot);

                        if (currentParentPath == parentPath)
                        {
                            int currentComponentOrder = e.GetIntegerExtension(CommonDefinitions.ExtUrlEdComponentFieldOrder) ?? -1;
                            e.SetIntegerExtension(CommonDefinitions.ExtUrlEdComponentFieldOrder, currentComponentOrder + 1);
                        }
                    }
                }
            }
        }

        // check for adding to differential
        if ((sd.Differential != null) && (sd.Differential.Element.Count != 0))
        {
            // find the element to insert after
            int matchIndex = sd.Differential.Element.FindIndex(e => e.cgFieldOrder() == sdFieldOrder);

            if (matchIndex == -1)
            {
                // add to the end
                sd.Differential.Element.Add(ed);
            }
            else
            {
                sd.Differential.Element.Insert(matchIndex, ed);

                if (increaseSubsequentOrders)
                {
                    // increase the order of all subsequent elements
                    foreach (ElementDefinition e in sd.Differential.Element.Skip(matchIndex + 1))
                    {
                        int currentOrder = e.cgFieldOrder();
                        e.SetIntegerExtension(CommonDefinitions.ExtUrlEdFieldOrder, currentOrder + 1);

                        int currentLastDot = e.Path.LastIndexOf('.');
                        string currentParentPath = currentLastDot == -1 ? e.Path : e.Path.Substring(0, currentLastDot);

                        if (currentParentPath == parentPath)
                        {
                            int currentComponentOrder = e.GetIntegerExtension(CommonDefinitions.ExtUrlEdComponentFieldOrder) ?? -1;
                            e.SetIntegerExtension(CommonDefinitions.ExtUrlEdComponentFieldOrder, currentComponentOrder + 1);
                        }
                    }
                }
            }
        }

        return true;
    }

    /// <summary>Processes elements in a structure definition.</summary>
    /// <remarks>Adds field orders, indexes paths that contain child elements, etc.</remarks>
    /// <param name="artifactClass">The artifact class.</param>
    /// <param name="sd">           The structure definition.</param>
    /// <param name="fhirVersion">  The FHIR version.</param>
    private void ProcessElements(FhirArtifactClassEnum artifactClass, StructureDefinition sd, FhirReleases.FhirSequenceCodes fhirVersion)
    {
        Dictionary<string, int> allFieldOrders = [];
        Dictionary<string, Dictionary<string, int>> componentOrdersByIdByParent = [];

        List<string> idByDepth = [];

        Dictionary<string, string> pathTypes = [];

        // process each element in the snapshot
        foreach (ElementDefinition ed in sd.Snapshot?.Element ?? Enumerable.Empty<ElementDefinition>())
        {
            // DSTU2 did not include element id's on ElementDefinitions, so we need to construct them
            if (string.IsNullOrEmpty(ed.ElementId))
            {
                AddMissingElementId(ed, idByDepth);

                // DSTU2 allowed repetitions of elements that we want to ignore (slicing definition before every slice)
                if (allFieldOrders.ContainsKey(ed.ElementId))
                {
                    continue;
                }
            }

            int lastDot = ed.Path.LastIndexOf('.');
            string parentPath = lastDot == -1 ? ed.Path : ed.Path.Substring(0, lastDot);
            int componentFieldOrder = 0;

            if (lastDot != -1)
            {
                if (!componentOrdersByIdByParent.TryGetValue(parentPath, out Dictionary<string, int>? coById))
                {
                    coById = [];
                    componentOrdersByIdByParent.Add(parentPath, coById);
                }

                if (!coById.TryGetValue(ed.ElementId, out componentFieldOrder))
                {
                    componentFieldOrder = coById.Count;
                    coById.Add(ed.ElementId, componentFieldOrder);
                }
            }

            int fo = allFieldOrders.Count;
            allFieldOrders.Add(ed.ElementId, fo);
            ed.AddExtension(CommonDefinitions.ExtUrlEdFieldOrder, new Integer(fo));
            ed.AddExtension(CommonDefinitions.ExtUrlEdComponentFieldOrder, new Integer(componentFieldOrder));

            //// add to lookup dict
            //_elementSdLookup.Add($"{ed.Path}|{ed.ElementId}", sd);

            if (lastDot != -1)
            {
                if (parentPath.Contains('.') &&
                    !_parentElementsAndType.ContainsKey(parentPath))
                {
                    if (pathTypes.TryGetValue(parentPath, out string? parentType))
                    {
                        _parentElementsAndType.Add(parentPath, parentType);
                    }
                    else
                    {
                        _parentElementsAndType.Add(parentPath, string.Empty);
                    }
                }
            }

            // check for being a slice
            if (!string.IsNullOrEmpty(ed.SliceName))
            {
                if (!_pathsWithSlices.TryGetValue(ed.Path, out KeyValuePair<string, StructureDefinition>[]? slices))
                {
                    slices = [new(ed.SliceName, sd)];
                    _pathsWithSlices[ed.Path] = slices;
                }
                else
                {
                    if (!slices.Any(sliceDef => sliceDef.Key == ed.SliceName))
                    {
                        _pathsWithSlices[ed.Path] = [.. slices, new(ed.SliceName, sd)];
                    }
                }
            }

            // check for a value set binding
            CheckElementBindings(artifactClass, sd, ed);

            // DSTU2 and STU3 need type consolidation
            if ((fhirVersion == FhirReleases.FhirSequenceCodes.DSTU2) || (fhirVersion == FhirReleases.FhirSequenceCodes.STU3))
            {
                ConsolidateTypes(sd, ed);
            }

            // check for a single type and add to the path types dictionary
            if (ed.Type.Count == 1)
            {
                pathTypes[ed.Path] = ed.Type.First().Code;
            }
        }

        // process each element in the differential
        foreach (ElementDefinition ed in sd.Differential?.Element ?? Enumerable.Empty<ElementDefinition>())
        {
            // DSTU2 did not include element id's on ElementDefinitions, so we need to construct them
            if (string.IsNullOrEmpty(ed.ElementId))
            {
                AddMissingElementId(ed, idByDepth);

                // DSTU2 allowed repetitions of elements that we want to ignore (slicing definition before every slice)
                if (allFieldOrders.ContainsKey(ed.ElementId))
                {
                    continue;
                }
            }

            int lastDot = ed.Path.LastIndexOf('.');
            string parentPath = lastDot == -1 ? ed.Path : ed.Path.Substring(0, lastDot);
            int componentFieldOrder = 0;

            if (lastDot != -1)
            {
                if (!componentOrdersByIdByParent.TryGetValue(parentPath, out Dictionary<string, int>? coById))
                {
                    coById = [];
                    componentOrdersByIdByParent.Add(parentPath, coById);
                }

                if (!coById.TryGetValue(ed.ElementId, out componentFieldOrder))
                {
                    componentFieldOrder = coById.Count;
                    coById.Add(ed.ElementId, componentFieldOrder);
                }
            }

            // check if this element has already been processed
            if (!allFieldOrders.TryGetValue(ed.ElementId, out int fo))
            {
                fo = allFieldOrders.Count;
                allFieldOrders.Add(ed.ElementId, fo);

                //// add to lookup dict
                //_elementSdLookup.Add($"{ed.Path}|{ed.ElementId}", sd);

                // check for being a child element to promote a parent to backbone
                if (lastDot != -1)
                {
                    if (parentPath.Contains('.') &&
                        !_parentElementsAndType.ContainsKey(parentPath))
                    {
                        if (pathTypes.TryGetValue(parentPath, out string? parentType))
                        {
                            _parentElementsAndType.Add(parentPath, parentType);
                        }
                        else
                        {
                            _parentElementsAndType.Add(parentPath, string.Empty);
                        }
                    }
                }

                // check for being a slice - only need to test if this element has not been processed already
                if (!string.IsNullOrEmpty(ed.SliceName))
                {
                    if (!_pathsWithSlices.TryGetValue(ed.Path, out KeyValuePair<string, StructureDefinition>[]? slices))
                    {
                        slices = [new(ed.SliceName, sd)];
                        _pathsWithSlices[ed.Path] = slices;
                    }
                    else
                    {
                        if (!slices.Any(sliceDef => sliceDef.Key == ed.SliceName))
                        {
                            _pathsWithSlices[ed.Path] = [.. slices, new(ed.SliceName, sd)];
                        }
                    }
                }

                // check for a value set binding
                CheckElementBindings(artifactClass, sd, ed);

                // DSTU2 and STU3 need type consolidation
                if ((fhirVersion == FhirReleases.FhirSequenceCodes.DSTU2) || (fhirVersion == FhirReleases.FhirSequenceCodes.STU3))
                {
                    ConsolidateTypes(sd, ed);
                }

                // check for a single type and add to the path types dictionary
                if (ed.Type.Count == 1)
                {
                    pathTypes[ed.Path] = ed.Type.First().Code;
                }
            }

            ed.AddExtension(CommonDefinitions.ExtUrlEdFieldOrder, new Integer(fo));
            ed.AddExtension(CommonDefinitions.ExtUrlEdComponentFieldOrder, new Integer(componentFieldOrder));
        }

        // DSTU2 and STU3 do not always declare types on root elements
        if ((fhirVersion == FhirReleases.FhirSequenceCodes.DSTU2) || (fhirVersion == FhirReleases.FhirSequenceCodes.STU3))
        {
            ElementDefinition? re = sd.cgRootElement();
            if (re != null)
            {
                re.Base ??= new ElementDefinition.BaseComponent();

                if (string.IsNullOrEmpty(re.Base.Path))
                {
                    re.Base.Path = re.Path;
                }

                re.Min ??= 0;

                if (string.IsNullOrEmpty(re.Max))
                {
                    re.Max = "*";
                }
            }
        }
    }

    /// <summary>
    /// Adds the missing element ID to the given <see cref="ElementDefinition"/>.
    /// </summary>
    /// <param name="ed">The <see cref="ElementDefinition"/> to add the missing ID to.</param>
    /// <param name="idByDepth">The list of IDs by depth.</param>
    private void AddMissingElementId(ElementDefinition ed, List<string> idByDepth)
    {
        string[] components = ed.Path.Split('.');
        int depth = components.Length;

        if (depth == 1)
        {
            ed.ElementId = ed.Path;
            idByDepth.Clear();
            idByDepth.Add(ed.Path);
            return;
        }

        // remove keys with the same length or deeper
        if (idByDepth.Count >= depth)
        {
            idByDepth.RemoveRange(depth - 1, (idByDepth.Count - depth) + 1);
        }

        // append the slice name if present
        if (!string.IsNullOrEmpty(ed.SliceName))
        {
            // check for a dot-notation name
            if (ed.SliceName.Contains('.'))
            {
                // check for resource name prefix
                if (ed.SliceName.StartsWith(idByDepth[0]))
                {
                    // append just the last dot component as a slice name
                    components[depth - 1] += string.Concat(":", ed.SliceName.AsSpan(ed.SliceName.LastIndexOf('.') + 1));
                }
                else
                {
                    // convert to pascal case
                    components[depth - 1] += ":" + ed.SliceName.ToPascalCase();
                }
            }
            else
            {
                components[depth - 1] += ":" + ed.SliceName;
            }
        }

        // add our path components
        idByDepth.AddRange(components.Skip(idByDepth.Count));

        ed.ElementId = string.Join('.', idByDepth);
    }

    /// <summary>Consolidate types.</summary>
    /// <param name="sd">The structure definition.</param>
    /// <param name="ed">The ed.</param>
    private void ConsolidateTypes(StructureDefinition sd, ElementDefinition ed)
    {
        // only need to consolidate if there are 2 or more
        if (ed.Type.Count < 2)
        {
            return;
        }

        // consolidate types
        Dictionary<string, ElementDefinition.TypeRefComponent> consolidatedTypes = [];

        foreach (ElementDefinition.TypeRefComponent tr in ed.Type)
        {
            if (!consolidatedTypes.TryGetValue(tr.Code, out ElementDefinition.TypeRefComponent? existing))
            {
                consolidatedTypes[tr.Code] = tr;
                continue;
            }

            // add any missing profile references
            if (tr.ProfileElement.Count != 0)
            {
                existing.ProfileElement.AddRange(tr.ProfileElement);
            }

            if (tr.TargetProfileElement.Count != 0)
            {
                existing.TargetProfileElement.AddRange(tr.TargetProfileElement);
            }
        }

        // update our types
        ed.Type = [.. consolidatedTypes.Values];
    }

    /// <summary>
    /// Retrieves the concept definition for a given code in a specified code system.
    /// </summary>
    /// <param name="system">The URL of the code system.</param>
    /// <param name="code">The code to retrieve the definition for.</param>
    /// <returns>The concept definition for the specified code, or an empty string if not found.</returns>
    public string ConceptDefinition(string system, string code, string defaultValue = "")
    {
        // check to see if we have the code system
        if (!_codeSystemsByUrl.TryGetValue(system, out CodeSystem? cs))
        {
            return defaultValue;
        }

        if (cs.Concept is null)
        {
            return defaultValue;
        }

        return searchConceptRecursive(cs.Concept!) ?? defaultValue;

        string? searchConceptRecursive(List<CodeSystem.ConceptDefinitionComponent> defs)
        {
            string? val = null;

            foreach (CodeSystem.ConceptDefinitionComponent c in defs)
            {
                if (c.Code == code)
                {
                    return c.Definition;
                }

                if (c.Concept is null)
                {
                    continue;
                }

                val = searchConceptRecursive(c.Concept);
                if (val is not null)
                {
                    return val;
                }
            }

            return null;
        }
    }

    /// <summary>Attempts to get value set.</summary>
    /// <param name="unversionedUrl">URL of the unversioned.</param>
    /// <param name="version">       The version.</param>
    /// <param name="vs">            [out] The vs.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    internal bool TryGetValueSet(string unversionedUrl, string version, [NotNullWhen(true)] out ValueSet? vs)
    {
        return _valueSetsByVersionedUrl.TryGetValue(unversionedUrl + "|" + version, out vs);
    }

    /// <summary>
    /// Returns the versioned URL for a given value set URL.
    /// </summary>
    /// <param name="vsUrl">The URL of the value set.</param>
    /// <returns>The versioned URL of the value set.</returns>
    internal string VersionedUrlForVs(string vsUrl)
    {
        int lastPipe = vsUrl.LastIndexOf('|');

        if ((lastPipe != -1) ||
            (!_valueSetVersions.TryGetValue(vsUrl, out string[]? vsVersions)) ||
            (vsVersions == null) ||
            (vsVersions.Length == 0))
        {
            return vsUrl;
        }

        return vsUrl + "|" + vsVersions!.Max();
    }

    /// <summary>Unversioned URL for vs.</summary>
    /// <param name="vsUrl">The URL of the value set.</param>
    /// <returns>A string.</returns>
    internal string UnversionedUrlForVs(string vsUrl)
    {
        int lastPipe = vsUrl.LastIndexOf('|');

        if (lastPipe == -1)
        {
            return vsUrl;
        }

        // strip the pipe and version
        return vsUrl.Substring(0, lastPipe - 1);
    }

    /// <summary>Check element bindings.</summary>
    /// <param name="artifactClass">The artifact class.</param>
    /// <param name="ed">           The ed.</param>
    private void CheckElementBindings(FhirArtifactClassEnum artifactClass, StructureDefinition sd, ElementDefinition ed)
    {
        // check for a value set binding
        if ((ed.Binding != null) && (!string.IsNullOrEmpty(ed.Binding.ValueSet)))
        {
            string url = VersionedUrlForVs(ed.Binding.ValueSet);

            // need to pick the right dictionary based on artifact class
            switch (artifactClass)
            {
                // these artifacts are always core
                case FhirArtifactClassEnum.PrimitiveType:
                case FhirArtifactClassEnum.ComplexType:
                case FhirArtifactClassEnum.Resource:
                case FhirArtifactClassEnum.Compartment:
                    {
                        if (!_coreBindingEdsByPathByValueSet.TryGetValue(url, out List<StructureElementCollection>? bindings))
                        {
                            bindings = [];
                            _coreBindingEdsByPathByValueSet[url] = bindings;
                        }

                        // search for the structure element collection that contains the structure definition
                        StructureElementCollection? matchingElementCollection = bindings.FirstOrDefault(e => e.Structure.Url == sd.Url);
                        if (matchingElementCollection is null)
                        {
                            matchingElementCollection = new StructureElementCollection { Structure = sd, Elements = [] };
                            bindings.Add(matchingElementCollection);
                        }

                        matchingElementCollection.Elements.Add(ed);

                        //pathElementCollections = pathElementCollections.Append(matchingElementCollection).ToArray();
                    }
                    break;

                // extensions need to be processed based on their contexts, even though the element does not have them
                case FhirArtifactClassEnum.Extension:
                    {
                        NestIntoExtension(url, sd);
                    }
                    break;

                // these artifacts are always extended
                case FhirArtifactClassEnum.Operation:
                case FhirArtifactClassEnum.SearchParameter:
                case FhirArtifactClassEnum.Profile:
                case FhirArtifactClassEnum.LogicalModel:
                case FhirArtifactClassEnum.ConceptMap:
                case FhirArtifactClassEnum.NamingSystem:
                case FhirArtifactClassEnum.StructureMap:
                    {
                        if (!_extendedBindingEdsByPathByValueSet.TryGetValue(url, out List<StructureElementCollection>? bindings))
                        {
                            bindings = [];
                            _extendedBindingEdsByPathByValueSet[url] = bindings;
                        }

                        // search for the structure element collection that contains the structure definition
                        StructureElementCollection? matchingElementCollection = bindings.FirstOrDefault(e => e.Structure.Url == sd.Url);
                        if (matchingElementCollection is null)
                        {
                            matchingElementCollection = new StructureElementCollection { Structure = sd, Elements = [] };
                            bindings.Add(matchingElementCollection);
                        }

                        matchingElementCollection.Elements.Add(ed);

                    }
                    break;

                // these are untracked because they should never happen
                case FhirArtifactClassEnum.CodeSystem:
                case FhirArtifactClassEnum.ValueSet:
                case FhirArtifactClassEnum.ImplementationGuide:
                case FhirArtifactClassEnum.CapabilityStatement:
                case FhirArtifactClassEnum.Unknown:
                    break;
                default:
                    break;
            }
        }

        return;

        /// <summary>
        /// Nest the current structure definition into an extension with the specified URL.
        /// </summary>
        /// <param name="url">The URL of the extension.</param>
        /// <param name="currentSd">The current structure definition to nest.</param>
        void NestIntoExtension(string url, StructureDefinition currentSd)
        {
            foreach (StructureDefinition.ContextComponent cc in currentSd.Context ?? Enumerable.Empty<StructureDefinition.ContextComponent>())
            {
                switch (cc.Type)
                {
                    case StructureDefinition.ExtensionContextType.Element:
                        {
                            if (!_extendedBindingEdsByPathByValueSet.TryGetValue(url, out List<StructureElementCollection>? bindings))
                            {
                                bindings = [];
                                _extendedBindingEdsByPathByValueSet[url] = bindings;
                            }

                            //bindings[cc.Expression] = bindings.TryGetValue(cc.Expression, out ElementDefinition[]? ar) ? ar.Append(ed).ToArray() : new[] { ed };

                            //if (!bindings.TryGetValue(cc.Expression, out List<StructureElementCollection>? pathElementCollections))
                            //{
                            //    pathElementCollections = new();
                            //    bindings.Add(cc.Expression, pathElementCollections);
                            //}

                            // search for the structure element collection that contains the structure definition
                            StructureElementCollection? matchingElementCollection = bindings.FirstOrDefault(e => e.Structure.Url == currentSd.Url);
                            if (matchingElementCollection is null)
                            {
                                matchingElementCollection = new StructureElementCollection { Structure = currentSd, Elements = [] };
                                bindings.Add(matchingElementCollection);
                            }

                            matchingElementCollection.Elements.Add(ed);

                        }
                        break;

                    case StructureDefinition.ExtensionContextType.Extension:
                        {
                            if (_extensionsByUrl.TryGetValue(cc.Expression, out StructureDefinition? extSd))
                            {
                                NestIntoExtension(url, extSd);
                            }
                        }
                        break;

                    case StructureDefinition.ExtensionContextType.Fhirpath:
                    default:
                        {
                            Console.WriteLine($"CheckElementBindings.NestIntoExtension <<< {currentSd.Id}: resolving bindings in extension for {url} encountered an unhandled definition: {cc.Type}:{cc.Expression}");
                        }
                        continue;
                }

            }
        }
    }

    /// <summary>Processes parameters in an operation definition.</summary>
    /// <remarks>Adds field orders, etc.</remarks>
    /// <param name="op">The operation.</param>
    private void ProcessParameters(OperationDefinition op)
    {
        Dictionary<string, int> inFieldOrder = [];
        Dictionary<string, int> outFieldOrder = [];

        // annotate each parameter with a field order extension
        foreach (OperationDefinition.ParameterComponent pc in op.Parameter ?? Enumerable.Empty<OperationDefinition.ParameterComponent>())
        {
            int fo;
            if (pc.Use == OperationParameterUse.Out)
            {
                fo = outFieldOrder.Count + 1;
                outFieldOrder.Add(pc.Name, fo);
            }
            else 
            {
                fo = inFieldOrder.Count + 1;
                inFieldOrder.Add(pc.Name, fo);
            }

            pc.AddExtension(CommonDefinitions.ExtUrlEdFieldOrder, new Integer(fo));
        }
    }

    /// <summary>Track resource.</summary>
    /// <param name="r">A Resource to process.</param>
    private void TrackResource(Resource r)
    {
        if (r is IConformanceResource canonical)
        {
            string fullUrl = canonical.Url;

            string canonicalUrl;
            string version;

            if (fullUrl.Contains('|'))
            {
                string[] parts = fullUrl.Split('|');
                canonicalUrl = parts[0];
                version = parts[1];
            }
            else
            {
                canonicalUrl = fullUrl;
                IEnumerable<ElementValue> vElement = r.NamedChildren.Where(e => e.ElementName == "version");

                if (vElement.Any())
                {
                    version = vElement.First().Value.ToString() ?? FhirSequence.ToLongVersion();
                }
                else
                {
                    version = FhirSequence.ToLongVersion();
                }
            }

            if (!_canonicalResources.TryGetValue(canonicalUrl, out Dictionary<string, IConformanceResource>? versions))
            {
                versions = [];
                _canonicalResources.Add(canonicalUrl, versions);
            }

            versions[version] = canonical;
        }

        string url;

        IEnumerable<ElementValue> uElement = r.NamedChildren.Where(e => e.ElementName == "url");

        if (uElement.Any())
        {
            url = uElement.First().Value.ToString() ?? r.Id;
        }
        else
        {
            url = r.Id;
        }

        // there is nothing we can really do about collisions, so just use the most recent
        _allResources[url] = r;
    }

    /// <summary>Gets URL of the code systems by.</summary>
    public IReadOnlyDictionary<string, CodeSystem> CodeSystemsByUrl => _codeSystemsByUrl;

    /// <summary>Adds a code system.</summary>
    /// <param name="codeSystem">The code system.</param>
    public void AddCodeSystem(CodeSystem codeSystem)
    {
        _codeSystemsByUrl[codeSystem.Url] = codeSystem;
        TrackResource(codeSystem);
    }

    /// <summary>Gets the value set versions.</summary>
    public IReadOnlyDictionary<string, string[]> ValueSetVersions => _valueSetVersions;

    /// <summary>Gets URL of the value sets by.</summary>
    public IReadOnlyDictionary<string, ValueSet> ValueSetsByVersionedUrl => _valueSetsByVersionedUrl;

    /// <summary>Versions for value set.</summary>
    /// <param name="valueSetUrl">URL of the value set.</param>
    /// <returns>A string[].</returns>
    public string[] VersionsForValueSet(string valueSetUrl)
    {
        string url = UnversionedUrlForVs(valueSetUrl);

        _valueSetVersions.TryGetValue(url, out string[]? versions);

        return versions ?? [];
    }

    /// <summary>
    /// Returns the external value sets that are bound in the DefinitionCollection.
    /// </summary>
    /// <returns>An enumerable collection of the external value sets.</returns>
    public IEnumerable<string> BoundExternalValueSets()
    {
        IEnumerable<string> core = _coreBindingEdsByPathByValueSet.Keys.Except(_valueSetsByVersionedUrl.Keys);
        core = core.Except(_valueSetVersions.Keys);

        IEnumerable<string> ext = _extendedBindingEdsByPathByValueSet.Keys.Except(_valueSetsByVersionedUrl.Keys);
        ext = ext.Except(_valueSetVersions.Keys);

        return core.Union(ext);
    }

    /// <summary>Structures and elements that contain bindings to the specified value set URL.</summary>
    /// <param name="valueSetUrl">URL of the value set.</param>
    public IEnumerable<StructureElementCollection> CoreBindingsForVs(string valueSetUrl)
    {
        string url = VersionedUrlForVs(valueSetUrl);

        if (_coreBindingEdsByPathByValueSet.TryGetValue(url, out List<StructureElementCollection>? bindings))
        {
            return bindings;
        }

        return [];
    }

    /// <summary>Strongest core binding.</summary>
    /// <param name="valueSetUrl">URL of the value set.</param>
    /// <returns>A BindingStrength?</returns>
    public BindingStrength? StrongestCoreBinding(string valueSetUrl)
    {
        string url = VersionedUrlForVs(valueSetUrl);

        if (_coreBindingEdsByPathByValueSet.TryGetValue(url, out List<StructureElementCollection>? bindings) &&
            (bindings != null))
        {
            return bindings
                .SelectMany(ec => ec.Elements.Select(ed => ed.Binding.Strength))
                .OrderBy(s => s, BindingStrengthComparer.Instance)
                .FirstOrDefault();
        }

        return null;
    }

    /// <summary>Core binding strength by type.</summary>
    /// <param name="valueSetUrl">URL of the value set.</param>
    /// <returns>An IReadOnlyDictionary&lt;string,BindingStrength&gt;</returns>
    public IReadOnlyDictionary<string, BindingStrength> CoreBindingStrengthByType(string valueSetUrl)
    {
        string url = VersionedUrlForVs(valueSetUrl);

        Dictionary<string, BindingStrength> bindingStrengthByType = [];

        if (!_coreBindingEdsByPathByValueSet.TryGetValue(url, out List<StructureElementCollection>? bindings))
        {
            return bindingStrengthByType;
        }

        // traverse existing bindings to find the strongest for each type
        foreach (StructureElementCollection ec in bindings)
        {
            foreach (ElementDefinition ed in ec.Elements)
            {
                foreach (ElementDefinition.TypeRefComponent tr in ed.Type)
                {
                    if (bindingStrengthByType.TryGetValue(tr.Code, out BindingStrength bs) &&
                        BindingStrengthComparer.Instance.Compare(bs, ed.Binding.Strength!) <= 0)
                    {
                        continue;
                    }

                    bindingStrengthByType[tr.Code] = (BindingStrength)ed.Binding.Strength!;
                }
            }
        }

        return bindingStrengthByType;
    }

    /// <summary>Extended bindings for vs.</summary>
    /// <param name="valueSetUrl">URL of the value set.</param>
    /// <returns>
    /// An enumerator that allows foreach to be used to process extended bindings for vs in this
    /// collection.
    /// </returns>
    public IEnumerable<StructureElementCollection> ExtendedBindingsForVs(string valueSetUrl)
    {
        string url = VersionedUrlForVs(valueSetUrl);

        if (_extendedBindingEdsByPathByValueSet.TryGetValue(url, out List<StructureElementCollection>? bindings))
        {
            return bindings;

            //List<StructureElementCollection> filtered = new();

            //foreach (StructureElementCollection ec in bindings)
            //{
            //    List<ElementDefinition> elements = ec.Elements.Where(ed => !ed.cgIsInherited(ec.Structure)).ToList();

            //    if (elements.Any())
            //    {
            //        filtered.Add(new StructureElementCollection { Structure = ec.Structure, Elements = elements });
            //    }
            //}

            //return filtered;
        }

        return [];
    }

    /// <summary>Strongest extended binding.</summary>
    /// <param name="valueSetUrl">URL of the value set.</param>
    /// <returns>A BindingStrength?</returns>
    public BindingStrength? StrongestExtendedBinding(string valueSetUrl)
    {
        string url = VersionedUrlForVs(valueSetUrl);

        if (_extendedBindingEdsByPathByValueSet.TryGetValue(url, out List<StructureElementCollection>? bindings) &&
            (bindings != null))
        {
            return bindings
                .SelectMany(ec => ec.Elements.Select(ed => ed.Binding.Strength))
                .OrderBy(s => s, BindingStrengthComparer.Instance)
                .FirstOrDefault();
        }

        return null;
    }

    /// <summary>Extended binding strength by type.</summary>
    /// <param name="valueSetUrl">URL of the value set.</param>
    /// <returns>An IReadOnlyDictionary&lt;string,BindingStrength&gt;</returns>
    public IReadOnlyDictionary<string, BindingStrength> ExtendedBindingStrengthByType(string valueSetUrl)
    {
        string url = VersionedUrlForVs(valueSetUrl);

        Dictionary<string, BindingStrength> bindingStrengthByType = [];

        if (!_extendedBindingEdsByPathByValueSet.TryGetValue(url, out List<StructureElementCollection>? bindings))
        {
            return bindingStrengthByType;
        }

        // traverse existing bindings to find the strongest for each type
        foreach (StructureElementCollection ec in bindings)
        {
            foreach (ElementDefinition ed in ec.Elements)
            {
                foreach (ElementDefinition.TypeRefComponent tr in ed.Type)
                {
                    if (bindingStrengthByType.TryGetValue(tr.Code, out BindingStrength bs) &&
                        BindingStrengthComparer.Instance.Compare(bs, ed.Binding.Strength!) <= 0)
                    {
                        continue;
                    }

                    bindingStrengthByType[tr.Code] = (BindingStrength)ed.Binding.Strength!;
                }
            }
        }

        return bindingStrengthByType;
    }

    /// <summary>Bindings for vs.</summary>
    /// <param name="valueSetUrl">URL of the value set.</param>
    /// <returns>An IReadOnlyDictionary&lt;string,ElementDefinition&gt;</returns>
    public IEnumerable<StructureElementCollection> BindingsForVs(string valueSetUrl)
    {
        string url = VersionedUrlForVs(valueSetUrl);

        if (!_coreBindingEdsByPathByValueSet.TryGetValue(url, out List<StructureElementCollection>? core))
        {
            core = [];
        }

        if (!_extendedBindingEdsByPathByValueSet.TryGetValue(url, out List<StructureElementCollection>? extended))
        {
            extended = [];
        }

        return core.Union(extended);
    }

    /// <summary>Strongest binding.</summary>
    /// <param name="valueSetUrl">URL of the value set.</param>
    /// <returns>A BindingStrength?</returns>
    public BindingStrength? StrongestBinding(string valueSetUrl)
    {
        string url = VersionedUrlForVs(valueSetUrl);

        BindingStrength? cbs = null;
        BindingStrength? ebs = null;

        if (_coreBindingEdsByPathByValueSet.TryGetValue(url, out List<StructureElementCollection>? core))
        {
            cbs = core
                .SelectMany(ec => ec.Elements.Select(ed => ed.Binding.Strength))
                .OrderBy(s => s, BindingStrengthComparer.Instance)
                .FirstOrDefault();
        }

        if (_extendedBindingEdsByPathByValueSet.TryGetValue(url, out List<StructureElementCollection>? extended))
        {
            ebs = extended
                .SelectMany(ec => ec.Elements.Select(ed => ed.Binding.Strength))
                .OrderBy(s => s, BindingStrengthComparer.Instance)
                .FirstOrDefault();
        }

        return BindingStrengthComparer.Instance.Compare(cbs, ebs) > 0 ? cbs : ebs;
    }

    /// <summary>Strongest binding.</summary>
    /// <param name="bindings">The bindings.</param>
    /// <returns>A BindingStrength?</returns>
    public BindingStrength? StrongestBinding(IEnumerable<StructureElementCollection> bindings)
    {
        // we want descending order so that strongest is first
        return bindings
            .SelectMany(ec => ec.Elements.Select(ed => ed.Binding.Strength))
            .OrderByDescending(s => s, BindingStrengthComparer.Instance)
            .FirstOrDefault();
    }

    /// <summary>Binding strength by type.</summary>
    /// <param name="valueSetUrl">URL of the value set.</param>
    /// <returns>An IReadOnlyDictionary&lt;string,BindingStrength&gt;</returns>
    public IReadOnlyDictionary<string, BindingStrength> BindingStrengthByType(string valueSetUrl)
    {
        string url = VersionedUrlForVs(valueSetUrl);

        Dictionary<string, BindingStrength> bindingStrengthByType = [];

        if (!_coreBindingEdsByPathByValueSet.TryGetValue(url, out List<StructureElementCollection>? core))
        {
            core = [];
        }

        if (!_extendedBindingEdsByPathByValueSet.TryGetValue(url, out List<StructureElementCollection>? extended))
        {
            extended = [];
        }

        // note that we do not have to worry about structure collisions between the two since no structure can be in both
        IEnumerable<StructureElementCollection> all = core.Union(extended);

        if (!all.Any())
        {
            return bindingStrengthByType;
        }

        // traverse existing bindings to find the strongest for each type
        foreach (ElementDefinition ed in all.SelectMany(ec => ec.Elements))
        {
            foreach (ElementDefinition.TypeRefComponent tr in ed.Type)
            {
                if (bindingStrengthByType.TryGetValue(tr.Code, out BindingStrength bs) &&
                    BindingStrengthComparer.Instance.Compare(bs, ed.Binding.Strength!) <= 0)
                {
                    continue;
                }

                bindingStrengthByType[tr.Code] = (BindingStrength)ed.Binding.Strength!;
            }
        }

        return bindingStrengthByType;
    }

    /// <summary>Binding strength by type.</summary>
    /// <param name="bindings">The bindings.</param>
    /// <returns>An IReadOnlyDictionary&lt;string,BindingStrength&gt;</returns>
    public IReadOnlyDictionary<string, BindingStrength> BindingStrengthByType(IEnumerable<StructureElementCollection> bindings)
    {
        Dictionary<string, BindingStrength> bindingStrengthByType = [];

        if (!bindings.Any())
        {
            return bindingStrengthByType;
        }

        // traverse existing bindings to find the strongest for each type
        foreach (ElementDefinition ed in bindings.SelectMany(ec => ec.Elements))
        {
            foreach (ElementDefinition.TypeRefComponent tr in ed.Type)
            {
                if (bindingStrengthByType.TryGetValue(tr.Code, out BindingStrength bs) &&
                    BindingStrengthComparer.Instance.Compare(bs, ed.Binding.Strength!) <= 0)
                {
                    continue;
                }

                bindingStrengthByType[tr.Code] = (BindingStrength)ed.Binding.Strength!;
            }
        }

        return bindingStrengthByType;
    }


    /// <summary>
    /// Adds a value set to the definition collection.
    /// </summary>
    /// <param name="valueSet">The value set to be added.</param>
    public void AddValueSet(ValueSet valueSet)
    {
        string vsUrl = valueSet.Url;

        if (!vsUrl.Contains('|'))
        {
            vsUrl = $"{vsUrl}|{valueSet.Version}";
        }

        string unversioned = UnversionedUrlForVs(valueSet.Url);

        if (_valueSetVersions.TryGetValue(unversioned, out string[]? versions))
        {
            if (!versions.Contains(valueSet.Version))
            {
                _valueSetVersions[unversioned] = [.. versions, valueSet.Version];
            }
        }
        else
        {
            _valueSetVersions[unversioned] = [valueSet.Version];
        }

        if (!_valueSetUrlsById.ContainsKey(valueSet.Id))
        {
            _valueSetUrlsById[valueSet.Id] = vsUrl;
        }

        if (_valueSetsByVersionedUrl.TryGetValue(vsUrl, out ValueSet? existing) && (existing != null))
        {
            // sort out unexpanded vs expanded vs multiple expansions
            if ((valueSet.Expansion != null) && (existing.Expansion == null))
            {
                existing.Expansion = new();

                // copy the expansion into the existing
                valueSet.Expansion.CopyTo(existing.Expansion);
            }
            else if ((valueSet.Expansion == null) && (existing.Expansion != null))
            {
                valueSet.Expansion = new();

                // copy the existing expansion into the new record
                existing.Expansion.CopyTo(valueSet.Expansion);
            }
            else if ((valueSet.Expansion != null) && (existing.Expansion != null))
            {
                // merge the expansion values
                existing.Expansion.Contains = existing.Expansion.Contains.Union(valueSet.Expansion.Contains).ToList();

                // merge the parameters
                existing.Expansion.Parameter = existing.Expansion.Parameter.Union(valueSet.Expansion.Parameter).ToList();

                // update the total
                existing.Expansion.Total = existing.Expansion.Contains.Count;
            }
            else
            {
                // keep the more recent if possible
                DateTimeOffset existingUpdated = existing.Meta?.LastUpdated?.UtcDateTime ?? DateTimeOffset.MinValue;
                DateTimeOffset incomingUpdated = valueSet.Meta?.LastUpdated?.UtcDateTime ?? DateTimeOffset.MinValue;

                if (existingUpdated < incomingUpdated)
                {
                    // keep the incoming
                    _valueSetsByVersionedUrl[vsUrl] = valueSet;
                }
            }

            // done
            return;
        }

        _valueSetsByVersionedUrl[vsUrl] = valueSet;
        TrackResource(valueSet);
    }

    /// <summary>Gets the name of the primitive types by.</summary>
    public IReadOnlyDictionary<string, StructureDefinition> PrimitiveTypesByName => _primitiveTypesByName;

    /// <summary>Adds a primitive type.</summary>
    /// <param name="sd">The structure definition.</param>
    public void AddPrimitiveType(StructureDefinition sd, FhirReleases.FhirSequenceCodes fhirVersion)
    {
        // DSTU2 did not include the publication status extension, add it here for consistency
        if (fhirVersion == FhirReleases.FhirSequenceCodes.DSTU2)
        {
            sd.AddExtension(CommonDefinitions.ExtUrlStandardStatus, new Code("draft"));
        }

        // add field orders to elements
        ProcessElements(FhirArtifactClassEnum.PrimitiveType, sd, fhirVersion);

        // TODO(ginoc): Consider if we want to make this explicit on any definitions that do not have it
        //if (sd.FhirVersion == null)
        //{
        //    sd.FhirVersion = FhirVersion;
        //}

        _primitiveTypesByName[sd.Name] = sd;
        TrackResource(sd);
    }

    /// <summary>Gets the name of the complex types by.</summary>
    public IReadOnlyDictionary<string, StructureDefinition> ComplexTypesByName => _complexTypesByName;

    /// <summary>Adds a complex type.</summary>
    /// <param name="sd">The structure definition.</param>
    public void AddComplexType(StructureDefinition sd, FhirReleases.FhirSequenceCodes fhirVersion)
    {
        if (fhirVersion == FhirReleases.FhirSequenceCodes.DSTU2)
        {
            // DSTU2 did not include the publication status extension, add it here for consistency
            sd.AddExtension(CommonDefinitions.ExtUrlStandardStatus, new Code("draft"));
        }

        // add field orders to elements
        ProcessElements(FhirArtifactClassEnum.ComplexType, sd, fhirVersion);

        _complexTypesByName[sd.Name] = sd;
        TrackResource(sd);
    }

    /// <summary>Gets the name of the resources by.</summary>
    public IReadOnlyDictionary<string, StructureDefinition> ResourcesByName => _resourcesByName;

    /// <summary>Adds a resource.</summary>
    /// <param name="sd">The structure definition.</param>
    public void AddResource(StructureDefinition sd, FhirReleases.FhirSequenceCodes fhirVersion)
    {
        // DSTU2 did not include the publication status extension, add it here for consistency
        if (fhirVersion == FhirReleases.FhirSequenceCodes.DSTU2)
        {
            sd.AddExtension(CommonDefinitions.ExtUrlStandardStatus, new Code("draft"));
        }

        // add field orders to elements
        ProcessElements(FhirArtifactClassEnum.Resource, sd, fhirVersion);

        _resourcesByName[sd.Name] = sd;
        TrackResource(sd);
    }

    /// <summary>Gets the name of the logical models by.</summary>
    public IReadOnlyDictionary<string, StructureDefinition> LogicalModelsByName => _logicalModelsByName;

    /// <summary>Adds a logical model.</summary>
    /// <param name="sd">The structure definition.</param>
    public void AddLogicalModel(StructureDefinition sd, FhirReleases.FhirSequenceCodes fhirVersion)
    {
        // add field orders to elements
        ProcessElements(FhirArtifactClassEnum.LogicalModel, sd, fhirVersion);

        _logicalModelsByName[sd.Url] = sd;
        TrackResource(sd);
    }

    /// <summary>Gets extensions, keyed by URL.</summary>
    public IReadOnlyDictionary<string, StructureDefinition> ExtensionsByUrl => _extensionsByUrl;

    /// <summary>Gets extensions, keyed by URL, grouped by Path</summary>
    public IReadOnlyDictionary<string, Dictionary<string, StructureDefinition>> ExtensionsByPath => _extensionsByPath;

    /// <summary>Adds an extension.</summary>
    /// <param name="sd">The structure definition.</param>
    public void AddExtension(StructureDefinition sd, FhirReleases.FhirSequenceCodes fhirVersion)
    {
        // add field orders to elements
        ProcessElements(FhirArtifactClassEnum.Extension, sd, fhirVersion);

        string url = sd.Url;

        // add to main tracking dictionary
        _extensionsByUrl[sd.Url] = sd;
        TrackResource(sd);

        // traverse context to add to path tracking dictionary
        foreach (StructureDefinition.ContextComponent ctx in sd.Context)
        {
            if (ctx.Type != StructureDefinition.ExtensionContextType.Element)
            {
                // throw new ArgumentException($"Invalid extension context type: {context.Type}");
                _errors.Add($"AddExtension <<< StructureDefinition {sd.Name} ({sd.Id}) unhandled context type: {ctx.Type}");
                continue;
            }

            if (string.IsNullOrEmpty(ctx.Expression))
            {
                _errors.Add($"AddExtension <<< StructureDefinition {sd.Name} ({sd.Id}) missing context expression");
                continue;
            }

            if (!_extensionsByPath.ContainsKey(ctx.Expression))
            {
                _extensionsByPath[ctx.Expression] = [];
            }

            _extensionsByPath[ctx.Expression][url] = sd;
        }
    }

    /// <summary>Gets URL of the profiles by.</summary>
    public IReadOnlyDictionary<string, StructureDefinition> ProfilesByUrl => _profilesByUrl;

    /// <summary>Profiles for base.</summary>
    /// <param name="resourceType">Type of the resource.</param>
    /// <returns>An IReadOnlyDictionary&lt;string,StructureDefinition&gt;</returns>
    public IReadOnlyDictionary<string, StructureDefinition> ProfilesForBase(string resourceType)
    {
        if (_profilesByBaseType.TryGetValue(resourceType, out Dictionary<string, StructureDefinition>? sdDict))
        {
            return sdDict;
        }

        return new Dictionary<string, StructureDefinition>();
    }

    /// <summary>Adds a profile.</summary>
    /// <param name="sd">The structure definition.</param>
    public void AddProfile(StructureDefinition sd, FhirReleases.FhirSequenceCodes fhirVersion)
    {
        // add field orders to elements
        ProcessElements(FhirArtifactClassEnum.Profile, sd, fhirVersion);

        _profilesByUrl[sd.Url] = sd;
        TrackResource(sd);

        if (!string.IsNullOrEmpty(sd.Type))
        {
            if (!_profilesByBaseType.TryGetValue(sd.Type, out Dictionary<string, StructureDefinition>? sdDict))
            {
                sdDict = [];
                _profilesByBaseType.Add(sd.Type, sdDict);
            }

            sdDict[sd.Url] = sd;
        }
    }

    /// <summary>Gets the global search parameters, by URL.</summary>
    public IReadOnlyDictionary<string, SearchParameter> GlobalSearchParameters => _globalSearchParameters;

    /// <summary>Gets URL of the search parameters by.</summary>
    public IReadOnlyDictionary<string, SearchParameter> SearchParametersByUrl => _searchParamsByUrl;

    /// <summary>Searches for the first parameters for base.</summary>
    /// <param name="resourceType">Type of the resource.</param>
    /// <returns>The found parameters for base.</returns>
    public IReadOnlyDictionary<string, SearchParameter> SearchParametersForBase(string resourceType)
    {
        if (_searchParamsByBase.TryGetValue(resourceType, out Dictionary<string, SearchParameter>? spDict))
        {
            return spDict;
        }

        return new Dictionary<string, SearchParameter>();
    }

    /// <summary>Adds a search parameter.</summary>
    /// <exception cref="Exception">Thrown when an exception error condition occurs.</exception>
    /// <param name="sp">            The search parameter.</param>
    /// <param name="doNotOverwrite">(Optional) True to do not overwrite.</param>
    public void AddSearchParameter(SearchParameter sp, bool doNotOverwrite = false)
    {
        if (string.IsNullOrEmpty(sp.Url))
        {
            // best guess at a canonical URL for this
            sp.Url = string.Join('/', MainPackageCanonical, "SearchParameter", sp.Id).Replace("//", "/");
        }

        if (doNotOverwrite && _searchParamsByUrl.ContainsKey(sp.Url))
        {
            return;
        }

        _searchParamsByUrl[sp.Url] = sp;
        TrackResource(sp);

        foreach (VersionIndependentResourceTypesAll? rt in sp.Base)
        {
            if (rt == null)
            {
                // TODO(ginoc): Check to see if this is actually possible
                throw new Exception("SearchParameter.Base is null");
                //continue;
            }

            string spBase = Hl7.Fhir.Utility.EnumUtility.GetLiteral(rt) ?? string.Empty;

            if (string.IsNullOrEmpty(spBase))
            {
                // TODO(ginoc): Check to see if this is actually possible
                throw new Exception("SearchParameter.Base is null");
            }

            // check for a base of "Resource" and add to the global list
            if (spBase == "Resource")
            {
                _globalSearchParameters[sp.Url] = sp;
                continue;
            }

            if ((!_searchParamsByBase.TryGetValue(spBase, out Dictionary<string, SearchParameter>? spDict)) ||
                (spDict == null))
            {
                spDict = [];
                _searchParamsByBase.Add(spBase, spDict);
            }

            _ = spDict.TryAdd(sp.Url, sp);
        }
    }

    /// <summary>Gets the search result parameter.</summary>
    public IReadOnlyDictionary<string, FhirQueryParameter> SearchResultParameters => _searchResultParameters;

    /// <summary>Adds a search result parameter.</summary>
    /// <param name="sp">The search parameter.</param>
    public void AddSearchResultParameter(FhirQueryParameter sp)
    {
        _searchResultParameters[sp.Url] = sp;
    }

    /// <summary>Gets options for controlling the HTTP.</summary>
    public IReadOnlyDictionary<string, FhirQueryParameter> HttpParameters => _allInteractionParameters;

    /// <summary>Adds a HTTP query parameter.</summary>
    /// <param name="sp">The search parameter.</param>
    public void AddHttpQueryParameter(FhirQueryParameter sp)
    {
        _allInteractionParameters[sp.Url] = sp;
    }

    /// <summary>Gets URL of the operations by.</summary>
    public IReadOnlyDictionary<string, OperationDefinition> OperationsByUrl => _operationsByUrl;

    /// <summary>Type-level Operations for a resource type.</summary>
    /// <param name="resourceType">Type of the resource.</param>
    /// <returns>An IReadOnlyDictionary&lt;string,OperationDefinition&gt;</returns>
    public IReadOnlyDictionary<string, OperationDefinition> TypeOperationsForResource(string resourceType)
    {
        if (_typeOperationsByType.TryGetValue(resourceType, out Dictionary<string, OperationDefinition>? opDict))
        {
            return opDict;
        }

        return new Dictionary<string, OperationDefinition>();
    }

    /// <summary>Type-level Operations for a resource type.</summary>
    /// <param name="resourceType">Type of the resource.</param>
    /// <returns>An IReadOnlyDictionary&lt;string,OperationDefinition&gt;</returns>
    public IReadOnlyDictionary<string, OperationDefinition> TypeOperationsForResource(VersionIndependentResourceTypesAll resourceType) =>
        TypeOperationsForResource(Hl7.Fhir.Utility.EnumUtility.GetLiteral(resourceType) ?? string.Empty);

    /// <summary>Instance-level Operations for a resource type.</summary>
    /// <param name="resourceType">Type of the resource.</param>
    /// <returns>An IReadOnlyDictionary&lt;string,OperationDefinition&gt;</returns>
    public IReadOnlyDictionary<string, OperationDefinition> InstanceOperationsForResource(string resourceType)
    {
        if (_instanceOperationsByType.TryGetValue(resourceType, out Dictionary<string, OperationDefinition>? opDict))
        {
            return opDict;
        }

        return new Dictionary<string, OperationDefinition>();
    }

    /// <summary>Instance-level Operations for a resource type.</summary>
    /// <param name="resourceType">Type of the resource.</param>
    /// <returns>An IReadOnlyDictionary&lt;string,OperationDefinition&gt;</returns>
    public IReadOnlyDictionary<string, OperationDefinition> InstanceOperationsForResource(VersionIndependentResourceTypesAll resourceType) =>
        InstanceOperationsForResource(Hl7.Fhir.Utility.EnumUtility.GetLiteral(resourceType) ?? string.Empty);

    /// <summary>Gets the system-level operations.</summary>
    public IReadOnlyDictionary<string, OperationDefinition> SystemOperations => _systemOperations;

    /// <summary>Adds an operation.</summary>
    /// <param name="op">The operation.</param>
    public void AddOperation(OperationDefinition op)
    {
        // add field orders to parameters
        ProcessParameters(op);

        _operationsByUrl[op.Url] = op;
        TrackResource(op);

        // check to see if this is a system level operation
        if (op.System == true)
        {
            _systemOperations[op.Url] = op;
        }

        // add to the correct resource dictionary
        foreach (VersionIndependentResourceTypesAll? t in op.Resource)
        {
            if (t == null)
            {
                continue;
            }

            string rt = Hl7.Fhir.Utility.EnumUtility.GetLiteral(t) ?? string.Empty;

            if ((!_typeOperationsByType.TryGetValue(rt, out Dictionary<string, OperationDefinition>? typeDict)) ||
                (typeDict == null))
            {
                typeDict = [];
                _typeOperationsByType.Add(rt, typeDict);
            }

            if (op.Type == true)
            {
                typeDict[op.Url] = op;
            }

            if ((!_instanceOperationsByType.TryGetValue(rt, out Dictionary<string, OperationDefinition>? instanceDict)) ||
                (instanceDict == null))
            {
                instanceDict = [];
                _instanceOperationsByType.Add(rt, instanceDict);
            }

            if (op.Instance == true)
            {
                instanceDict[op.Url] = op;
            }
        }
    }

    public IReadOnlyDictionary<string, CapabilityStatement> CapabilityStatementsByUrl => _capabilityStatementsByUrl;

    public void AddCapabilityStatement(CapabilityStatement cs, CachePackageManifest source)
    {
        if (string.IsNullOrEmpty(cs.Url))
        {
            cs.Url = source.CanonicalUrl.EndsWith('/') ? source.CanonicalUrl + cs.Id : source.CanonicalUrl + "/" + cs.Id;
        }

        _capabilityStatementsByUrl[cs.Url] = cs;
        TrackResource(cs);
    }

    public IReadOnlyDictionary<string, ImplementationGuide> ImplementationGuidesByUrl => _implementationGuidesByUrl;

    public void AddImplementationGuide(ImplementationGuide ig)
    {
        _implementationGuidesByUrl[ig.Url] = ig;
        TrackResource(ig);
    }

    public IReadOnlyDictionary<string, CompartmentDefinition> CompartmentsByUrl => _compartmentsByUrl;

    public void AddCompartment(CompartmentDefinition compartmentDefinition)
    {
        _compartmentsByUrl[compartmentDefinition.Url] = compartmentDefinition;
        TrackResource(compartmentDefinition);
    }
}
