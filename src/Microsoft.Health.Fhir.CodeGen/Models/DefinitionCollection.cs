// <copyright file="DefinitionCollection.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>


using System.Xml.Linq;
using Hl7.Fhir.Model;
using Hl7.Fhir.Specification.Terminology;
using Microsoft.Health.Fhir.CodeGen.FhirExtensions;
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

    public FhirReleases.FhirSequenceCodes FhirSequence { get; set; } = FhirReleases.FhirSequenceCodes.Unknown;

    /// <summary>Gets or sets the manifest.</summary>
    public Dictionary<string, CachePackageManifest> Manifests { get; set; } = new();

    /// <summary>Gets or sets the contents.</summary>
    public Dictionary<string, PackageContents> ContentListings { get; set; } = new();
    
    private readonly Dictionary<string, StructureDefinition> _primitiveTypesByName = new();
    private readonly Dictionary<string, StructureDefinition> _complexTypesByName = new();
    private readonly Dictionary<string, StructureDefinition> _resourcesByName = new();
    private readonly Dictionary<string, StructureDefinition> _logicalModelsByName = new();
    private readonly Dictionary<string, StructureDefinition> _extensionsByUrl = new();
    private readonly Dictionary<string, Dictionary<string, StructureDefinition>> _extensionsByPath = new();
    private readonly Dictionary<string, StructureDefinition> _profilesByUrl = new();
    private readonly Dictionary<string, Dictionary<string, StructureDefinition>> _profilesByBaseType = new();

    private readonly Dictionary<string, OperationDefinition> _systemOperations = new();
    private readonly Dictionary<string, OperationDefinition> _operationsByUrl = new();

    private readonly Dictionary<string, SearchParameter> _globalSearchParameters = new();
    private readonly Dictionary<string, SearchParameter> _searchResultParameters = new();
    private readonly Dictionary<string, SearchParameter> _allInteractionParameters = new();
    private readonly Dictionary<string, SearchParameter> _searchParamsByUrl = new();
    private readonly Dictionary<string, Dictionary<string, SearchParameter>> _searchParamsByBase = new();

    private readonly Dictionary<string, CodeSystem> _codeSystemsByUrl = new();
    private readonly Dictionary<string, ValueSet> _valueSetsByVersionedUrl = new();
    private readonly Dictionary<string, string[]> _valueSetVersions = new();
    private readonly Dictionary<string, string> _valueSetUrlsById = new();

    private readonly Dictionary<string, ImplementationGuide> _implementationGuidesByUrl = new();
    private readonly Dictionary<string, CapabilityStatement> _capabilityStatementsByUrl = new();
    private readonly Dictionary<string, CompartmentDefinition> _compartmentsByUrl = new();

    private readonly HashSet<string> _backbonePaths = new();
    private readonly Dictionary<string, string[]> _pathsWithSlices = new();

    private readonly List<string> _errors = new();

    /// <summary>(Immutable) all resources.</summary>
    private readonly Dictionary<string, Resource> _allResources = new();

    /// <summary>(Immutable) The canonical resources.</summary>
    private readonly Dictionary<string, Dictionary<string, IConformanceResource>> _canonicalResources = new();

    /// <summary>(Immutable) The local transmit.</summary>
    private readonly LocalTerminologyService _localTx;

    /// <summary>
    /// Initializes a new instance of the <see cref="DefinitionCollection"/> class.
    /// </summary>
    public DefinitionCollection()
    {
        _localTx = new LocalTerminologyService(this);
    }

    /// <summary>Query if 'path' is backbone path.</summary>
    /// <param name="path">Full pathname of the file.</param>
    /// <returns>True if backbone path, false if not.</returns>
    public bool IsBackbonePath(string path) => _backbonePaths.Contains(path);

    /// <summary>Processes elements in a structure definition.</summary>
    /// <remarks>Addes field orders, indexes paths that contain child elements, etc.</remarks>
    /// <param name="sd">The structure definition.</param>
    private void ProcessElements(StructureDefinition sd)
    {
        Dictionary<string, int> allFieldOrders = new();

        // process each element in the snapshot
        foreach (ElementDefinition ed in sd.Snapshot?.Element ?? Enumerable.Empty<ElementDefinition>())
        {
            int fo = allFieldOrders.Count();
            allFieldOrders.Add(ed.ElementId, fo);
            ed.AddExtension(CommonDefinitions.ExtUrlFieldOrder, new Integer(fo));

            // check for being a child element
            //if (ed.Type.Any(t => t.Code.Equals("BackboneElement", StringComparison.Ordinal)))
            //{
            //    _backbonePaths.Add(ed.Path);
            //}
            if (ed.Path.Contains('.'))
            {
                string parentPath = ed.Path.Substring(0, ed.Path.LastIndexOf('.'));
                if (parentPath.Contains('.') &&
                    !_backbonePaths.Contains(parentPath))
                {
                    _backbonePaths.Add(parentPath);
                }
            }

            // check for being a slice
            if (!string.IsNullOrEmpty(ed.SliceName))
            {
                if (!_pathsWithSlices.TryGetValue(ed.Path, out string[]? slices))
                {
                    slices = new string[] { ed.SliceName };
                    _pathsWithSlices[ed.Path] = slices;
                }
                else
                {
                    if (!slices.Contains(ed.SliceName))
                    {
                        _pathsWithSlices[ed.Path] = slices.Append(ed.SliceName).ToArray();
                    }
                }
            }
        }

        // process each element in the differential
        foreach (ElementDefinition ed in sd.Differential?.Element ?? Enumerable.Empty<ElementDefinition>())
        {
            // use the field order from the snapshot if it exists
            if (!allFieldOrders.TryGetValue(ed.ElementId, out int fo))
            {
                fo = allFieldOrders.Count();
                allFieldOrders.Add(ed.ElementId, fo);

                // check for being a child element - only need to test if this element has not been processed already
                //if (ed.Type.Any(t => t.Code.Equals("BackboneElement", StringComparison.Ordinal)))
                //{
                //    _backbonePaths.Add(ed.Path);
                //}
                if (ed.Path.Contains('.'))
                {
                    string parentPath = ed.Path.Substring(0, ed.Path.LastIndexOf('.'));
                    if (!_backbonePaths.Contains(parentPath))
                    {
                        _backbonePaths.Add(parentPath);
                    }
                }

                // check for being a slice - only need to test if this element has not been processed already
                if (!string.IsNullOrEmpty(ed.SliceName))
                {
                    if (!_pathsWithSlices.TryGetValue(ed.Path, out string[]? slices))
                    {
                        slices = new string[] { ed.SliceName };
                        _pathsWithSlices[ed.Path] = slices;
                    }
                    else
                    {
                        if (!slices.Contains(ed.SliceName))
                        {
                            _pathsWithSlices[ed.Path] = slices.Append(ed.SliceName).ToArray();
                        }
                    }
                }
            }

            ed.AddExtension(CommonDefinitions.ExtUrlFieldOrder, new Integer(fo));
        }
    }

    /// <summary>Processes parameters in an operation definition.</summary>
    /// <remarks>Adds field orders, etc.</remarks>
    /// <param name="op">The operation.</param>
    private void ProcessParameters(OperationDefinition op)
    {
        Dictionary<string, int> inFieldOrder = new();
        Dictionary<string, int> outFieldOrder = new();

        // annotate each parameter with a field order extension
        foreach (OperationDefinition.ParameterComponent pc in op.Parameter ?? Enumerable.Empty<OperationDefinition.ParameterComponent>())
        {
            int fo;
            if (pc.Use == OperationParameterUse.Out)
            {
                fo = outFieldOrder.Count() + 1;
                outFieldOrder.Add(pc.Name, fo);
            }
            else 
            {
                fo = inFieldOrder.Count() + 1;
                inFieldOrder.Add(pc.Name, fo);
            }

            pc.AddExtension(CommonDefinitions.ExtUrlFieldOrder, new Integer(fo));
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
                IEnumerable<ElementValue> vElement = r.NamedChildren.Where(e => e.ElementName.Equals("version", StringComparison.Ordinal));

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
                versions = new();
                _canonicalResources.Add(canonicalUrl, versions);
            }

            versions[version] = canonical;
        }

        string url;

        IEnumerable<ElementValue> uElement = r.NamedChildren.Where(e => e.ElementName.Equals("url", StringComparison.Ordinal));

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
    }

    /// <summary>Gets URL of the value sets by.</summary>
    public IReadOnlyDictionary<string, ValueSet> ValueSetsByUrl => _valueSetsByVersionedUrl;

    /// <summary>Adds a value set.</summary>
    /// <param name="valueSet">Set the value belongs to.</param>
    public void AddValueSet(ValueSet valueSet)
    {
        string vsUrl = valueSet.Url;

        if (!vsUrl.Contains('|'))
        {
            vsUrl = $"{vsUrl}|{valueSet.Version}";
        }

        string unversioned = valueSet.Url.Substring(0, vsUrl.LastIndexOf('|'));

        if (_valueSetVersions.TryGetValue(unversioned, out string[]? versions))
        {
            if (!versions.Contains(valueSet.Version))
            {
                _valueSetVersions[unversioned] = versions.Append(valueSet.Version).ToArray();
            }
        }
        else
        {
            _valueSetVersions[unversioned] = new[] { valueSet.Version };
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
    public void AddPrimitiveType(StructureDefinition sd)
    {
        // add field orders to elements
        ProcessElements(sd);

        // TODO(ginoc): Consider if we want to make this explicit on any definitions that do not have it
        //if (sd.FhirVersion == null)
        //{
        //    sd.FhirVersion = FhirVersion;
        //}

        _primitiveTypesByName[sd.Name] = sd;
        TrackResource(sd);
    }

    public IReadOnlyDictionary<string, StructureDefinition> ComplexTypesByName => _complexTypesByName;

    public void AddComplexType(StructureDefinition sd)
    {
        // add field orders to elements
        ProcessElements(sd);

        _complexTypesByName[sd.Name] = sd;
        TrackResource(sd);
    }

    public IReadOnlyDictionary<string, StructureDefinition> ResourcesByName => _resourcesByName;

    public void AddResource(StructureDefinition sd)
    {
        // add field orders to elements
        ProcessElements(sd);

        _resourcesByName[sd.Name] = sd;
        TrackResource(sd);
    }

    public IReadOnlyDictionary<string, StructureDefinition> LogicalModelsByName => _logicalModelsByName;

    public void AddLogicalModel(StructureDefinition sd)
    {
        // add field orders to elements
        ProcessElements(sd);

        _logicalModelsByName[sd.Url] = sd;
        TrackResource(sd);
    }

    /// <summary>Gets extensions, keyed by URL.</summary>
    public IReadOnlyDictionary<string, StructureDefinition> ExtensionsByUrl => _extensionsByUrl;

    /// <summary>Gets extensions, keyed by URL, grouped by Path</summary>
    public IReadOnlyDictionary<string, Dictionary<string, StructureDefinition>> ExtensionsByPath => _extensionsByPath;

    /// <summary>Adds an extension.</summary>
    /// <param name="sd">The structure definition.</param>
    public void AddExtension(StructureDefinition sd)
    {
        // add field orders to elements
        ProcessElements(sd);

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
                _extensionsByPath[ctx.Expression] = new();
            }

            _extensionsByPath[ctx.Expression][url] = sd;
        }
    }

    public IReadOnlyDictionary<string, StructureDefinition> ProfilesByUrl => _profilesByUrl;

    public void AddProfile(StructureDefinition sd)
    {
        // add field orders to elements
        ProcessElements(sd);

        _profilesByUrl[sd.Url] = sd;
        TrackResource(sd);
    }

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
    /// <param name="sp">The search parameter.</param>
    public void AddSearchParameter(SearchParameter sp)
    {
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

            if ((!_searchParamsByBase.TryGetValue(spBase, out Dictionary<string, SearchParameter>? spDict)) ||
                (spDict == null))
            {
                spDict = new();
                _searchParamsByBase.Add(spBase, spDict);
            }

            if (!spDict.ContainsKey(sp.Url))
            {
                spDict.Add(sp.Url, sp);
            }
        }
    }

    public IReadOnlyDictionary<string, OperationDefinition> OperationsByUrl => _operationsByUrl;

    public void AddOperation(OperationDefinition op)
    {
        // add field orders to parameters
        ProcessParameters(op);

        _operationsByUrl[op.Url] = op;
        TrackResource(op);
    }

    public IReadOnlyDictionary<string, CapabilityStatement> CapabilityStatementsByUrl => _capabilityStatementsByUrl;

    public void AddCapabilityStatement(CapabilityStatement cs)
    {
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
