// <copyright file="DefinitionCollection.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>


using System;
using System.Diagnostics.CodeAnalysis;
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

    public FhirReleases.FhirSequenceCodes FhirSequence { get; set; } = FhirReleases.FhirSequenceCodes.Unknown;

    /// <summary>Gets or sets the identifier of the main package.</summary>
    public string MainPackageId { get; set; } = string.Empty;

    /// <summary>Gets or sets the main package canonical.</summary>
    public string MainPackageCanonical { get; set; } = string.Empty;

    /// <summary>Gets or sets the main package version.</summary>
    public string MainPackageVersion { get; set; } = string.Empty;

    /// <summary>Gets or sets the manifest.</summary>
    public Dictionary<string, CachePackageManifest> Manifests { get; set; } = new();

    /// <summary>Gets or sets the contents.</summary>
    public Dictionary<string, PackageContents> ContentListings { get; set; } = new();

    private readonly Dictionary<ElementDefinition, StructureDefinition> _elementSdLookup = new();

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
    private readonly Dictionary<string, Dictionary<string, OperationDefinition>> _typeOperationsByType = new();
    private readonly Dictionary<string, Dictionary<string, OperationDefinition>> _instanceOperationsByType = new();

    private readonly Dictionary<string, SearchParameter> _globalSearchParameters = new();
    private readonly Dictionary<string, FhirQueryParameter> _searchResultParameters = new();
    private readonly Dictionary<string, FhirQueryParameter> _allInteractionParameters = new();
    private readonly Dictionary<string, SearchParameter> _searchParamsByUrl = new();
    private readonly Dictionary<string, Dictionary<string, SearchParameter>> _searchParamsByBase = new();

    private readonly Dictionary<string, CodeSystem> _codeSystemsByUrl = new();
    private readonly Dictionary<string, ValueSet> _valueSetsByVersionedUrl = new();
    private readonly Dictionary<string, string[]> _valueSetVersions = new();
    private readonly Dictionary<string, string> _valueSetUrlsById = new();

    private readonly Dictionary<string, Dictionary<string, ElementDefinition[]>> _coreBindingEdsByPathByValueSet = new();
    private readonly Dictionary<string, Dictionary<string, ElementDefinition[]>> _extendedBindingEdsByPathByValueSet = new();

    private readonly Dictionary<string, ImplementationGuide> _implementationGuidesByUrl = new();
    private readonly Dictionary<string, CapabilityStatement> _capabilityStatementsByUrl = new();
    private readonly Dictionary<string, CompartmentDefinition> _compartmentsByUrl = new();

    private readonly HashSet<string> _backbonePaths = new();

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
    private void ProcessElements(FhirArtifactClassEnum artifactClass, StructureDefinition sd, FhirReleases.FhirSequenceCodes fhirVersion)
    {
        Dictionary<string, int> allFieldOrders = new();

        List<string> idByDepth = new();

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

            int fo = allFieldOrders.Count();
            allFieldOrders.Add(ed.ElementId, fo);
            ed.AddExtension(CommonDefinitions.ExtUrlFieldOrder, new Integer(fo));

            // add to lookup dict
            _elementSdLookup.Add(ed, sd);

            // check for being a child element
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
                if (!_pathsWithSlices.TryGetValue(ed.Path, out KeyValuePair<string, StructureDefinition>[]? slices))
                {
                    slices = new KeyValuePair<string, StructureDefinition>[] { new(ed.SliceName, sd) };
                    _pathsWithSlices[ed.Path] = slices;
                }
                else
                {
                    if (!slices.Any(sliceDef => sliceDef.Key.Equals(ed.SliceName, StringComparison.Ordinal)))
                    {
                        _pathsWithSlices[ed.Path] = slices.Append(new(ed.SliceName, sd)).ToArray();
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

            // use the field order from the snapshot if it exists
            if (!allFieldOrders.TryGetValue(ed.ElementId, out int fo))
            {
                fo = allFieldOrders.Count();
                allFieldOrders.Add(ed.ElementId, fo);

                // add to lookup dict
                _elementSdLookup.Add(ed, sd);

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
                    if (!_pathsWithSlices.TryGetValue(ed.Path, out KeyValuePair<string, StructureDefinition>[]? slices))
                    {
                        slices = new KeyValuePair<string, StructureDefinition>[] { new(ed.SliceName, sd) };
                        _pathsWithSlices[ed.Path] = slices;
                    }
                    else
                    {
                        if (!slices.Any(sliceDef => sliceDef.Key.Equals(ed.SliceName, StringComparison.Ordinal)))
                        {
                            _pathsWithSlices[ed.Path] = slices.Append(new(ed.SliceName, sd)).ToArray();
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
            }

            ed.AddExtension(CommonDefinitions.ExtUrlFieldOrder, new Integer(fo));
        }

        // DSTU2 and STU3 do not always declare types on root elements
        if ((fhirVersion == FhirReleases.FhirSequenceCodes.DSTU2) || (fhirVersion == FhirReleases.FhirSequenceCodes.STU3))
        {
            ElementDefinition? re = sd.cgRootElement();
            if (re != null)
            {
                if (re.Base == null)
                {
                    re.Base = new ElementDefinition.BaseComponent();
                }

                if (string.IsNullOrEmpty(re.Base.Path))
                {
                    re.Base.Path = re.Path;
                }

                if (re.Min == null)
                {
                    re.Min = 0;
                }

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
                    components[depth - 1] += ":" + ed.SliceName.Substring(ed.SliceName.LastIndexOf('.') + 1);
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
        if (ed.Type.Count() < 2)
        {
            return;
        }

        // consolidate types
        Dictionary<string, ElementDefinition.TypeRefComponent> consolidatedTypes = new();

        foreach (ElementDefinition.TypeRefComponent tr in ed.Type)
        {
            if (!consolidatedTypes.TryGetValue(tr.Code, out ElementDefinition.TypeRefComponent? existing))
            {
                consolidatedTypes[tr.Code] = tr;
                continue;
            }

            // add any missing profile references
            if (tr.ProfileElement.Any())
            {
                existing.ProfileElement.AddRange(tr.ProfileElement);
            }

            if (tr.TargetProfileElement.Any())
            {
                existing.TargetProfileElement.AddRange(tr.TargetProfileElement);
            }
        }

        // update our types
        ed.Type = consolidatedTypes.Values.ToList();
    }

    /// <summary>
    /// Returns the versioned URL for a given value set URL.
    /// </summary>
    /// <param name="vsUrl">The URL of the value set.</param>
    /// <returns>The versioned URL of the value set.</returns>
    private string VersionedUrlForVs(string vsUrl)
    {
        int lastPipe = vsUrl.LastIndexOf('|');

        if ((lastPipe != -1) ||
            (!_valueSetVersions.TryGetValue(vsUrl, out string[]? vsVersions)) ||
            (!(vsVersions?.Any() ?? false)))
        {
            return vsUrl;
        }

        return vsUrl + "|" + vsVersions!.Max();
    }

    private string UnversionedUrlForVs(string vsUrl)
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
                        if (!_coreBindingEdsByPathByValueSet.TryGetValue(url, out Dictionary<string, ElementDefinition[]>? bindings))
                        {
                            bindings = new();
                            _coreBindingEdsByPathByValueSet[url] = bindings;
                        }

                        bindings[ed.Path] = bindings.TryGetValue(ed.Path, out ElementDefinition[]? ar) ? ar.Append(ed).ToArray() : new[] { ed };
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
                        if (!_extendedBindingEdsByPathByValueSet.TryGetValue(url, out Dictionary<string, ElementDefinition[]>? bindings))
                        {
                            bindings = new();
                            _extendedBindingEdsByPathByValueSet[url] = bindings;
                        }

                        bindings[ed.Path] = bindings.TryGetValue(ed.Path, out ElementDefinition[]? ar) ? ar.Append(ed).ToArray() : new[] { ed };
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
                            if (!_extendedBindingEdsByPathByValueSet.TryGetValue(url, out Dictionary<string, ElementDefinition[]>? bindings))
                            {
                                bindings = new();
                                _extendedBindingEdsByPathByValueSet[url] = bindings;
                            }

                            bindings[cc.Expression] = bindings.TryGetValue(cc.Expression, out ElementDefinition[]? ar) ? ar.Append(ed).ToArray() : new[] { ed };
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

    /// <summary>Structure for element.</summary>
    /// <param name="ed">The ed.</param>
    /// <returns>A StructureDefinition.</returns>
    public StructureDefinition StructureForElement(ElementDefinition ed) => _elementSdLookup[ed];

    /// <summary>Gets URL of the code systems by.</summary>
    public IReadOnlyDictionary<string, CodeSystem> CodeSystemsByUrl => _codeSystemsByUrl;

    /// <summary>Adds a code system.</summary>
    /// <param name="codeSystem">The code system.</param>
    public void AddCodeSystem(CodeSystem codeSystem)
    {
        _codeSystemsByUrl[codeSystem.Url] = codeSystem;
        TrackResource(codeSystem);
    }

    /// <summary>Gets URL of the value sets by.</summary>
    public IReadOnlyDictionary<string, ValueSet> ValueSetsByVersionedUrl => _valueSetsByVersionedUrl;

    /// <summary>Versions for value set.</summary>
    /// <param name="valueSetUrl">URL of the value set.</param>
    /// <returns>A string[].</returns>
    public string[] VersionsForValueSet(string valueSetUrl)
    {
        string url = UnversionedUrlForVs(valueSetUrl);

        _valueSetVersions.TryGetValue(url, out string[]? versions);

        return versions ?? Array.Empty<string>();
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

    /// <summary>Value set bind strength by path.</summary>
    /// <param name="valueSetUrl">URL of the value set.</param>
    /// <returns>An IReadOnlyDictionary&lt;string,BindingStrength&gt;</returns>
    public IReadOnlyDictionary<string, ElementDefinition[]> CoreBindingsForVs(string valueSetUrl)
    {
        string url = VersionedUrlForVs(valueSetUrl);

        if (_coreBindingEdsByPathByValueSet.TryGetValue(url, out Dictionary<string, ElementDefinition[]>? bindings))
        {
            return bindings;
        }

        return new Dictionary<string, ElementDefinition[]>();
    }

    /// <summary>Strongest core binding.</summary>
    /// <param name="valueSetUrl">URL of the value set.</param>
    /// <returns>A BindingStrength?</returns>
    public BindingStrength? StrongestCoreBinding(string valueSetUrl)
    {
        string url = VersionedUrlForVs(valueSetUrl);

        if (_coreBindingEdsByPathByValueSet.TryGetValue(url, out Dictionary<string, ElementDefinition[]>? bindings) &&
            (bindings != null))
        {
            return bindings.SelectMany(kvp => kvp.Value.Select(ed => ed.Binding.Strength)).OrderBy(s => s, BindingStrengthComparer.Instance).FirstOrDefault(); ;
        }

        return null;
    }

    /// <summary>Core binding strength by type.</summary>
    /// <param name="valueSetUrl">URL of the value set.</param>
    /// <returns>An IReadOnlyDictionary&lt;string,BindingStrength&gt;</returns>
    public IReadOnlyDictionary<string, BindingStrength> CoreBindingStrengthByType(string valueSetUrl)
    {
        string url = VersionedUrlForVs(valueSetUrl);

        Dictionary<string, BindingStrength> bindingStrengthByType = new();

        if (!_coreBindingEdsByPathByValueSet.TryGetValue(url, out Dictionary<string, ElementDefinition[]>? bindings))
        {
            return bindingStrengthByType;
        }

        // traverse existing bindings to find the strongest for each type
        foreach (ElementDefinition ed in bindings.Values.SelectMany(a => a))
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

    /// <summary>Extended bindings for vs.</summary>
    /// <param name="valueSetUrl">URL of the value set.</param>
    /// <returns>An IReadOnlyDictionary&lt;string,ElementDefinition&gt;</returns>
    public IReadOnlyDictionary<string, ElementDefinition[]> ExtendedBindingsForVs(string valueSetUrl)
    {
        string url = VersionedUrlForVs(valueSetUrl);

        if (_extendedBindingEdsByPathByValueSet.TryGetValue(url, out Dictionary<string, ElementDefinition[]>? bindings))
        {
            return bindings;
        }

        return new Dictionary<string, ElementDefinition[]>();
    }

    /// <summary>Strongest extended binding.</summary>
    /// <param name="valueSetUrl">URL of the value set.</param>
    /// <returns>A BindingStrength?</returns>
    public BindingStrength? StrongestExtendedBinding(string valueSetUrl)
    {
        string url = VersionedUrlForVs(valueSetUrl);

        if (_extendedBindingEdsByPathByValueSet.TryGetValue(url, out Dictionary<string, ElementDefinition[]>? bindings) &&
            (bindings != null))
        {
            return bindings.SelectMany(kvp => kvp.Value.Select(ed => ed.Binding.Strength)).OrderBy(s => s, BindingStrengthComparer.Instance).FirstOrDefault(); ;
        }

        return null;
    }

    /// <summary>Extended binding strength by type.</summary>
    /// <param name="valueSetUrl">URL of the value set.</param>
    /// <returns>An IReadOnlyDictionary&lt;string,BindingStrength&gt;</returns>
    public IReadOnlyDictionary<string, BindingStrength> ExtendedBindingStrengthByType(string valueSetUrl)
    {
        string url = VersionedUrlForVs(valueSetUrl);

        Dictionary<string, BindingStrength> bindingStrengthByType = new();

        if (!_extendedBindingEdsByPathByValueSet.TryGetValue(url, out Dictionary<string, ElementDefinition[]>? bindings))
        {
            return bindingStrengthByType;
        }

        // traverse existing bindings to find the strongest for each type
        foreach (ElementDefinition ed in bindings.Values.SelectMany(a => a))
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

    /// <summary>Bindings for vs.</summary>
    /// <param name="valueSetUrl">URL of the value set.</param>
    /// <returns>An IReadOnlyDictionary&lt;string,ElementDefinition&gt;</returns>
    public IReadOnlyDictionary<string, ElementDefinition[]> BindingsForVs(string valueSetUrl)
    {
        string url = VersionedUrlForVs(valueSetUrl);

        if (!_coreBindingEdsByPathByValueSet.TryGetValue(url, out Dictionary<string, ElementDefinition[]>? core))
        {
            core = new();
        }

        if (!_extendedBindingEdsByPathByValueSet.TryGetValue(url, out Dictionary<string, ElementDefinition[]>? extended))
        {
            extended = new();
        }

        return core.Union(extended).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
    }

    /// <summary>Strongest binding.</summary>
    /// <param name="valueSetUrl">URL of the value set.</param>
    /// <returns>A BindingStrength?</returns>
    public BindingStrength? StrongestBinding(string valueSetUrl)
    {
        string url = VersionedUrlForVs(valueSetUrl);

        BindingStrength? cbs = null;
        BindingStrength? ebs = null;

        if (_coreBindingEdsByPathByValueSet.TryGetValue(url, out Dictionary<string, ElementDefinition[]>? core))
        {
            cbs = core.SelectMany(kvp => kvp.Value.Select(ed => ed.Binding.Strength)).OrderBy(s => s, BindingStrengthComparer.Instance).FirstOrDefault();
        }

        if (_extendedBindingEdsByPathByValueSet.TryGetValue(url, out Dictionary<string, ElementDefinition[]>? extended))
        {
            ebs = extended.SelectMany(kvp => kvp.Value.Select(ed => ed.Binding.Strength)).OrderBy(s => s, BindingStrengthComparer.Instance).FirstOrDefault();
        }

        return BindingStrengthComparer.Instance.Compare(cbs, ebs) > 0 ? cbs : ebs;
    }

    /// <summary>Strongest binding.</summary>
    /// <param name="bindings">The bindings.</param>
    /// <returns>A BindingStrength?</returns>
    public BindingStrength? StrongestBinding(IReadOnlyDictionary<string, ElementDefinition[]> bindings)
    {
        return bindings.SelectMany(kvp => kvp.Value.Select(ed => ed.Binding.Strength)).OrderBy(s => s, BindingStrengthComparer.Instance).FirstOrDefault();
    }

    /// <summary>Binding strength by type.</summary>
    /// <param name="valueSetUrl">URL of the value set.</param>
    /// <returns>An IReadOnlyDictionary&lt;string,BindingStrength&gt;</returns>
    public IReadOnlyDictionary<string, BindingStrength> BindingStrengthByType(string valueSetUrl)
    {
        string url = VersionedUrlForVs(valueSetUrl);

        Dictionary<string, BindingStrength> bindingStrengthByType = new();

        if (!_coreBindingEdsByPathByValueSet.TryGetValue(url, out Dictionary<string, ElementDefinition[]>? core))
        {
            core = new();
        }

        if (!_extendedBindingEdsByPathByValueSet.TryGetValue(url, out Dictionary<string, ElementDefinition[]>? extended))
        {
            extended = new();
        }

        Dictionary<string, ElementDefinition[]> all = core.Union(extended).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        if (!all.Any())
        {
            return bindingStrengthByType;
        }

        // traverse existing bindings to find the strongest for each type
        foreach (ElementDefinition ed in all.Values.SelectMany(a => a))
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
    public IReadOnlyDictionary<string, BindingStrength> BindingStrengthByType(IReadOnlyDictionary<string, ElementDefinition[]> bindings)
    {
        Dictionary<string, BindingStrength> bindingStrengthByType = new();

        if (!bindings.Any())
        {
            return bindingStrengthByType;
        }

        // traverse existing bindings to find the strongest for each type
        foreach (ElementDefinition ed in bindings.Values.SelectMany(a => a))
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
                _extensionsByPath[ctx.Expression] = new();
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
                sdDict = new();
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
            if (spBase.Equals("Resource", StringComparison.Ordinal))
            {
                _globalSearchParameters[sp.Url] = sp;
                continue;
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

        // add to the correct resoure dictionary
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
                typeDict = new();
                _typeOperationsByType.Add(rt, typeDict);
            }

            if (op.Type == true)
            {
                typeDict[op.Url] = op;
            }

            if ((!_instanceOperationsByType.TryGetValue(rt, out Dictionary<string, OperationDefinition>? instanceDict)) ||
                (instanceDict == null))
            {
                instanceDict = new();
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
