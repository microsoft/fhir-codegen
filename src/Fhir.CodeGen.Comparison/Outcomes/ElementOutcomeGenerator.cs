using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Fhir.CodeGen.Common.Models;
using Fhir.CodeGen.Common.Packaging;
using Fhir.CodeGen.Common.Utils;
using Fhir.CodeGen.Comparison.CompareTool;
using Fhir.CodeGen.Comparison.Extensions;
using Fhir.CodeGen.Comparison.Models;
using Fhir.CodeGen.Comparison.XVer;
using Hl7.Fhir.Model;
using Microsoft.Extensions.Logging;
using Octokit;
using static Hl7.Fhir.Model.PaymentReconciliation;
using CMR = Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship;

namespace Fhir.CodeGen.Comparison.Outcomes;

public class ElementOutcomeGenerator
{
    private class ElementOutcomeTrackingRecord
    {
        public required DbElement SourceElement { get; set; }
        public List<DbElementOutcome> ElementOutcomes { get; set; } = [];
        public List<DbElementComparison> ElementComparisons { get; set; } = [];
        public Dictionary<int, DbElement> TargetElements { get; set; } = [];
        public List<string> Messages { get; set; } = [];
        public bool IsFullyMappedAcrossAllTargets { get; set; } = false;
        public List<DbElementComparison> MapsToIndividualTargets { get; set; } = [];
        public List<List<DbElementComparison>> MapsToCombinationOfTargets { get; set; } = [];
        public int DiscreteTargetCount { get; set; } = 0;
        public CMR? QuantityBasedRelationship { get; set; } = null;
        public CMR? BoundValueSetRelationship { get; set; } = null;
        public List<DbElementType> MappedTypes { get; set; } = [];
        public List<DbElementType> UnmappedTypes { get; set; } = [];
        public List<ChildTypeMappingResult> ChildTypeMappingResults { get; set; } = [];
    }

    private class ChildTypeMappingResult
    {
        /// <summary>The parent source type being evaluated.</summary>
        public required DbElementType SourceParentType { get; set; }

        /// <summary>True if all significant children have valid type mappings.</summary>
        public required bool AllChildrenMapped { get; set; }

        /// <summary>Mapping information for each child that has mappings.</summary>
        public required Dictionary<int, ChildMappingInfo> ChildMappings { get; set; } = [];

        /// <summary>Child elements that have valid type mappings.</summary>
        public required List<DbElement> MappedTypeChildren { get; set; } = [];

        /// <summary>Child elements that do not have valid type mappings.</summary>
        public required List<DbElement> UnmappedTypeChildren { get; set; } = [];
    }

    private class ChildMappingInfo
    {
        /// <summary>The child element being mapped.</summary>
        public required DbElement ChildElement { get; set; }

        /// <summary>Valid type comparisons for this child.</summary>
        public required List<DbElementTypeComparison> TypeComparisons { get; set; } = [];

        /// <summary>Child element mappings relevant to this mapping.</summary>
        public required List<DbElementMapping> TypeChildMappings { get; set; } = [];

        /// <summary>Target element keys this child maps to.</summary>
        public required HashSet<int> TargetElementKeys { get; set; } = [];
    }

    private readonly IDbConnection _db;

    private ILoggerFactory _loggerFactory;
    private ILogger _logger;

    private readonly FhirPackageComparisonPair _packagePair;
    private readonly Dictionary<string, string?> _targetBasicElementPathLookup;
    private readonly Dictionary<string, DbElement> _targetBasicElementsById;

    private readonly Dictionary<int, DbElement> _allSourceElements;
    private readonly ILookup<int, DbElement> _allSourceElementsBySdKey;

    private readonly Dictionary<int, DbElement> _allTargetElements;
    private readonly ILookup<int, DbElement> _allTargetElementsBySdKey;

    private readonly List<DbElementComparison> _edComparisons;
    private readonly ILookup<int, DbElementComparison> _edComparsionsBySourceElementKey;
    private readonly ILookup<int, DbElementComparison> _edComparsionsBySourceStructureKey;

    private readonly Dictionary<int, DbElementType> _allSourceElementTypes;
    private readonly ILookup<int, DbElementType> _sourceElementTypesByElementKey;

    private readonly Dictionary<int, DbElementType> _allTargetElementTypes;
    private readonly ILookup<int, DbElementType> _targetElementTypesByElementKey;

    private readonly List<DbElementTypeComparison> _etcComparisons;
    private readonly ILookup<int, DbElementTypeComparison> _etcComparisonsBySourceElementKey;

    private readonly Dictionary<string, DbExtensionSubstitution> _extensionSubstitutionsByElementId;

    private DbRecordCache<DbElementOutcome> _edOutcomeCache;

    public ElementOutcomeGenerator(
        IDbConnection db,
        ILoggerFactory loggerFactory,
        FhirPackageComparisonPair packagePair,
        DbRecordCache<DbElementOutcome> edOutcomeCache)
    {
        _db = db;
        _loggerFactory = loggerFactory;
        _logger = loggerFactory.CreateLogger<ElementOutcomeGenerator>();
        _packagePair = packagePair;
        _edOutcomeCache = edOutcomeCache;

        _allSourceElements = DbElement.SelectDict(_db, FhirPackageKey: _packagePair.SourcePackageKey);
        _allTargetElements = DbElement.SelectDict(_db, FhirPackageKey: _packagePair.TargetPackageKey);

        _edComparisons = DbElementComparison.SelectList(
            _db,
            SourceFhirPackageKey: packagePair.SourcePackageKey,
            TargetFhirPackageKey: packagePair.TargetPackageKey);

        _logger.LogInformation(
            $"Element comparisons: {_edComparisons.Count}, from" +
            $" {_allSourceElements.Count} Elements" +
            $" to {_allTargetElements.Count} Elements");

        _allSourceElementTypes = DbElementType.SelectDict(_db, FhirPackageKey: packagePair.SourcePackageKey);
        _allTargetElementTypes = DbElementType.SelectDict(_db, FhirPackageKey: packagePair.TargetPackageKey);

        _etcComparisons = DbElementTypeComparison.SelectList(
            _db,
            SourceFhirPackageKey: packagePair.SourcePackageKey,
            TargetFhirPackageKey: packagePair.TargetPackageKey);

        _logger.LogInformation(
            $"Element Type comparisons: {_etcComparisons.Count}, from" +
            $" {_allSourceElementTypes.Count} Element Types" +
            $" to {_allTargetElementTypes.Count} Element Types");

        _allSourceElementsBySdKey = _allSourceElements.Values.ToLookup(c => c.StructureKey);
        _allTargetElementsBySdKey = _allTargetElements.Values.ToLookup(c => c.StructureKey);

        _sourceElementTypesByElementKey = _allSourceElementTypes.Values.ToLookup(c => c.ElementKey);
        _targetElementTypesByElementKey = _allTargetElementTypes.Values.ToLookup(c => c.ElementKey);

        _edComparsionsBySourceElementKey = _edComparisons.ToLookup(c => c.SourceContentKey);
        _edComparsionsBySourceStructureKey = _edComparisons.ToLookup(c => c.SourceStructureKey);

        _etcComparisonsBySourceElementKey = _etcComparisons.ToLookup(c => c.SourceElementKey);

        _targetBasicElementPathLookup = [];
        _targetBasicElementsById = [];

        // check for a basic structure
        DbStructureDefinition? targetBasicResource = DbStructureDefinition.SelectSingle(
            _db,
            FhirPackageKey: _packagePair.TargetPackageKey,
            Name: "Basic",
            ArtifactClass: FhirArtifactClassEnum.Resource);
        if (targetBasicResource != null)
        {
            // get the elements for this structure
            List<DbElement> targetBasicElements = DbElement.SelectList(
                _db,
                StructureKey: targetBasicResource.Key,
                orderByProperties: [nameof(DbElement.ResourceFieldOrder)]);

            // iterate over the elements
            foreach (DbElement element in targetBasicElements)
            {
                // skip root, elements with empty paths, and `code` element
                if ((element.ResourceFieldOrder == 0) ||
                    string.IsNullOrEmpty(element.Path) ||
                    element.Path.Equals("Basic.code", StringComparison.Ordinal))
                {
                    continue;
                }

                // add the path to the dictionary, but strip "Basic" from the front
                _targetBasicElementPathLookup.Add(element.Path.Substring(5), element.BasePath);
                _targetBasicElementsById.Add(element.Id, element);
                if (element.BasePath is not null)
                {
                    _targetBasicElementsById[element.BasePath] = element;
                }
            }
        }

        // build our extension substitution lookup
        List<DbExtensionSubstitution> extensionSubstitutions = DbExtensionSubstitution.SelectList(
            _db,
            SourceVersion: _packagePair.SourceFhirSequence);
        _extensionSubstitutionsByElementId = extensionSubstitutions
            .ToDictionary(es => es.SourceElementId);
    }

    private bool skipElement(
        DbElement ed,
        bool skipFirstElement = true,
        bool skipIds = true,
        bool skipExtensions = true,
        bool skipModifierExtenions = true) =>
            (skipFirstElement && (ed.ResourceFieldOrder == 0)) ||
            (skipIds && ((ed.BasePath == "id") || (ed.BasePath == "Element.id") || (ed.BasePath == "Resource.id"))) ||
            (skipExtensions && ((ed.Name == "extension") || (ed.FullCollatedTypeLiteral == "Extension"))) ||
            (skipModifierExtenions &&
                ((ed.Name == "modifierExtension") || (ed.BasePath == "DomainResource.modifierExtension") || (ed.BasePath == "BackboneElement.modifierExtension")));

    private static readonly char[] _literalSplitChars = [',', '(', ')', '[', ']'];

    private bool canSourceMapToBasicElementType(
        DbElement sourceEd,
        DbElement targetEd)
    {
        string[] sourceBaseTypes = sourceEd.FullCollatedTypeLiteral
            .Split(_literalSplitChars, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        HashSet<string> targetBaseTypes = targetEd.FullCollatedTypeLiteral
            .Split(_literalSplitChars, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .ToHashSet();

        foreach (string sbt in sourceBaseTypes)
        {
            if (sbt.Contains('/') || sbt.Contains(':'))
            {
                continue;
            }

            if (!targetBaseTypes.Contains(sbt))
            {
                return false;
            }
        }

        return true;
    }

    public void ProcessNoMapStructure(
        DbStructureDefinition sourceSd,
        DbStructureOutcome sdOutcome)
    {
        if (sourceSd.ArtifactClass == Common.Models.FhirArtifactClassEnum.PrimitiveType)
        {
            return;
        }

        // get the elements of this structure, but filter out elements we should ignore
        List<DbElement> sourceElements = _allSourceElementsBySdKey[sourceSd.Key]
            .Where(ed => !skipElement(ed, skipFirstElement: false))
            .ToList();

        if (sourceElements.Count == 0)
        {
            // this is a special type like 'datatype', just itnore it
            return;
        }

        DbElement rootEd = sourceElements[0];
        DbElementOutcome? rootEdOutcome = null;

        Dictionary<int, DbElementOutcome> edKeyOutcomeLookup = [];
        //HashSet<int> outcomesRequiringXver = [];

        // iterate over the source elements for this structure to create no-map outcomes
        foreach (DbElement sourceEd in sourceElements.OrderBy(ed => ed.ResourceFieldOrder))
        {
            List<DbElementType> sourceEts = _sourceElementTypesByElementKey[sourceEd.Key]
                .OrderBy(et => et.Literal)
                .ToList();

            (string idLong, string idShort) = XVerProcessor.GenerateExtensionId(
                _packagePair.SourcePackageShortName,
                sourceEd.Id);

            string extUrl = $"http://hl7.org/fhir/{_packagePair.SourceFhirVersionShort}/StructureDefinition/{idLong}";

            string? componentIdLong = null;
            string? componentIdShort = null;
            string? componentExtUrl = null;

            if (sourceEd.UsedAsContentReference == true)
            {
                componentIdLong = idLong;
                componentIdShort = idShort;
                componentExtUrl = extUrl;
            }

            string comments =
                $"Element `{sourceEd.Id}` is not mapped to FHIR {_packagePair.TargetFhirSequence}," +
                $" since FHIR {_packagePair.SourceFhirSequence} `{sourceSd.Name}` is not mapped.";

            bool requiresXVerDefinition = true;

            DbElementOutcome? parentOutcome = null;

            if ((sourceEd.ParentElementKey is not null) &&
                edKeyOutcomeLookup.TryGetValue(sourceEd.ParentElementKey!.Value, out DbElementOutcome? parentOutcomeValue))
            {
                parentOutcome = parentOutcomeValue;
                requiresXVerDefinition = requiresXVerDefinition || parentOutcome.RequiresXVerDefinition;
            }

            List<string> contexts = [];

            bool defineAsModifier = sourceEd.IsModifier;
            if (sourceEd.ResourceFieldOrder == 0)
            {
                contexts.Add("Basic");
            }
            else if (parentOutcome is not null)
            {
                contexts.Add(parentOutcome.GenLongId ?? parentOutcome.SourceName);
            }

            // check to see if we are trying to define an extension onto basic that has a matching basic path and compatible type
            string? basicBasePath = null;
            if (_targetBasicElementPathLookup.TryGetValue(sourceEd.Path.Substring(sourceSd.Name.Length), out basicBasePath) &&
                _targetBasicElementsById.TryGetValue(basicBasePath!, out DbElement? basicEd))
            {
                if (canSourceMapToBasicElementType(sourceEd, basicEd) &&
                    (sourceEd.MinCardinality >= basicEd.MinCardinality) &&
                    ((basicEd.MaxCardinality == -1) || (basicEd.MaxCardinality >= sourceEd.MaxCardinality)) &&
                    (   ((sourceEd.ChildElementCount == 0) && (basicEd.ChildElementCount == 0)) ||
                        ((sourceEd.ChildElementCount > 0) && (basicEd.ChildElementCount > 0))))
                {
                    requiresXVerDefinition = false;
                    comments +=
                        $"\nElement matches Basic element path `{basicBasePath}` and is compatible," +
                        $" use that element instead.";
                    contexts = [];
                    parentOutcome = null;
                }
                else
                {
                    comments +=
                        $"\nNote that the source element matches Basic element path `{basicBasePath}`," +
                        $" but the definitions are not compatible" +
                        $" (source: `{sourceEd.FullCollatedTypeLiteral}`:{sourceEd.FhirCardinalityString}" +
                        $" -> basic: `{basicEd.FullCollatedTypeLiteral}`:{basicEd.FhirCardinalityString}).";
                    basicBasePath = null;
                }
            }

            if (_extensionSubstitutionsByElementId.TryGetValue(sourceEd.Id, out DbExtensionSubstitution? extSubstitute))
            {
                comments +=
                    $"\nNote that there is an externally-defined extension that has been flagged as the" +
                    $" representation of FHIR {_packagePair.SourceFhirSequence} element `{sourceEd.Id}`:" +
                    $" `{extSubstitute.ReplacementUrl}`.";
            }

            //if (((parentOutcome is not null) && outcomesRequiringXver.Contains(parentOutcome.Key)) ||
            if ((parentOutcome?.RequiresXVerDefinition == true) ||
                (basicBasePath is not null))
            {
                idLong = sourceEd.NameClean();
                idShort = idLong;
                extUrl = idLong;
            }

            // create the non-mapped element outcome
            DbElementOutcome elementOutcome = new()
            {
                Key = DbElementOutcome.GetIndex(),
                StructureOutcomeKey = sdOutcome.Key,
                ElementComparisonKey = null,

                SourceFhirPackageKey = _packagePair.SourcePackageKey,
                SourceFhirSequence = _packagePair.SourceFhirSequence,
                SourceStructureKey = sourceSd.Key,
                SourceElementKey = sourceEd.Key,
                SourceCanonicalUnversioned = sourceSd.UnversionedUrl,
                SourceCanonicalVersioned = sourceSd.VersionedUrl,
                SourceVersion = sourceSd.Version,
                SourceId = sourceEd.Id,
                SourceName = sourceEd.Name,
                SourceResourceOrder = sourceEd.ResourceFieldOrder,
                SourceComponentOrder = sourceEd.ComponentFieldOrder,
                SourceMinCardinality = sourceEd.MinCardinality,
                SourceMaxCardinalityString = sourceEd.MaxCardinalityString,
                SourceChildElementCount = sourceEd.ChildElementCount,
                SourceUsedAsContentReference = sourceEd.UsedAsContentReference == true,
                TotalSourceCount = -1,

                TargetFhirPackageKey = _packagePair.TargetPackageKey,
                TargetFhirSequence = _packagePair.TargetFhirSequence,
                TargetStructureKey = null,
                TargetElementKey = null,
                TargetCanonicalUnversioned = null,
                TargetCanonicalVersioned = null,
                TargetVersion = null,
                TargetId = null,
                TargetName = null,
                TargetResourceOrder = null,
                TargetComponentOrder = null,
                TotalTargetCount = 0,

                RequiresXVerDefinition = requiresXVerDefinition,
                GenLongId = idLong,
                GenShortId = idShort,
                GenUrl = extUrl,
                RequiresComponentDefinition = sourceEd.UsedAsContentReference == true,
                ComponentGenLongId = componentIdLong,
                ComponentGenShortId = componentIdShort,
                ComponentGenUrl = componentExtUrl,

                AncestorElementOutcomeKey = requiresXVerDefinition ? rootEdOutcome?.Key : null,
                ParentElementOutcomeKey = parentOutcome?.Key,
                ParentRequiresXverDefinition = parentOutcome?.RequiresXVerDefinition ?? false,
                ParentRequiresComponentDefinition = parentOutcome?.RequiresComponentDefinition ?? false,
                SourceIsModifier = sourceEd.IsModifier,
                DefineAsModifier = defineAsModifier,
                ExtensionContexts = contexts,
                BasicElementEquivalent = basicBasePath,
                ExtensionSubstitutionKey = extSubstitute?.Key,
                ExtensionSubstitutionUrl = extSubstitute?.ReplacementUrl,

                IsRenamed = false,
                IsUnmapped = true,
                IsIdentical = false,
                IsEquivalent = false,
                IsBroaderThanTarget = false,
                IsNarrowerThanTarget = false,

                FullyMapsToThisTarget = false,
                FullyMapsAcrossAllTargets = false,
                UnmappedTypeKeys = sourceEts.Select(et => et.Key).ToList(),
                UnmappedTypeNames = sourceEts.Select(et => et.Literal).ToList(),

                Comments = comments,
            };

            //if (requiresXVerDefinition)
            //{
            //    outcomesRequiringXver.Add(elementOutcome.Key);
            //}

            if (sourceEd.ResourceFieldOrder == 0)
            {
                rootEdOutcome = elementOutcome;
            }

            edKeyOutcomeLookup[sourceEd.Key] = elementOutcome;

            _edOutcomeCache.CacheAdd(elementOutcome);
        }

        if (rootEdOutcome is not null)
        {
            edOutcomePostProcessing(
                edKeyOutcomeLookup.Values,
                rootEdOutcome);
        }
    }

    private void edOutcomePostProcessing(
        IEnumerable<DbElementOutcome> edOutcomes,
        DbElementOutcome edOutcome)
    {
        // create a lookup based on parent element key
        ILookup<int?, DbElementOutcome>? outcomesByParentKey = edOutcomes
            .ToLookup(eo => eo.ParentElementOutcomeKey);

        updateOutcomesWithChildInfoRecurse(
            edOutcome,
            outcomesByParentKey);
    }

    private (bool requiresXver, bool requiresComponent) updateOutcomesWithChildInfoRecurse(
        DbElementOutcome edOutcome,
        ILookup<int?, DbElementOutcome> outcomesByParentKey)
    {
        // get the child outcomes for this outcome
        List<DbElementOutcome> childOutcomes = outcomesByParentKey[edOutcome.Key]
            .OrderBy(eo => eo.SourceResourceOrder)
            .ToList();

        if (childOutcomes.Count == 0)
        {
            return (edOutcome.RequiresXVerDefinition, edOutcome.RequiresComponentDefinition);
        }

        bool childrenRequireXver = true;
        bool childrenRequireComponent = true;

        // iterate over each child outcome to bubble up requirements
        foreach (DbElementOutcome childOutcome in childOutcomes)
        {
            (bool childRequiresXver, bool childRequiresComponent) = updateOutcomesWithChildInfoRecurse(
                childOutcome,
                outcomesByParentKey);

            childrenRequireXver = childrenRequireXver && childRequiresXver;
            childrenRequireComponent = childrenRequireComponent && childRequiresComponent;
        }

        bool originalParentRequiresXver = edOutcome.ParentRequiresXverDefinition;
        bool originalParentRequiresComponent = edOutcome.ParentRequiresComponentDefinition;

        // update children if necessary
        if ((childrenRequireXver && !edOutcome.RequiresXVerDefinition) ||
            (childrenRequireComponent && !edOutcome.RequiresComponentDefinition))
        {
            setOutcomeRequiresRecursive(
                edOutcome,
                childrenRequireXver,
                childrenRequireComponent,
                outcomesByParentKey);
        }

        // reset to the value at this level - processing a parent will reset if necessary
        edOutcome.ParentRequiresXverDefinition = originalParentRequiresXver;
        edOutcome.ParentRequiresComponentDefinition = originalParentRequiresComponent;

        // update the generation info on the way out
        if (childrenRequireXver)
        {
            (string idLong, string idShort) = XVerProcessor.GenerateExtensionId(
                _packagePair.SourcePackageShortName,
                edOutcome.SourceId);

            edOutcome.GenLongId = idLong;
            edOutcome.GenShortId = idShort;
            edOutcome.GenUrl = $"http://hl7.org/fhir/{_packagePair.SourceFhirVersionShort}/StructureDefinition/{idLong}";
            edOutcome.RequiresXVerDefinition = true;
        }

        // update the component generation info on the way out
        if (childrenRequireComponent)
        {
            (string componentIdLong, string componentIdShort) = XVerProcessor.GenerateExtensionId(
                _packagePair.SourcePackageShortName,
                edOutcome.SourceId);

            edOutcome.ComponentGenLongId = componentIdLong;
            edOutcome.ComponentGenShortId = componentIdShort;
            edOutcome.ComponentGenUrl = $"http://hl7.org/fhir/{_packagePair.SourceFhirVersionShort}/StructureDefinition/{componentIdLong}";
            edOutcome.RequiresComponentDefinition = true;
        }

        // return for parent
        return (childrenRequireXver, childrenRequireComponent);
    }

    private void setOutcomeRequiresRecursive(
        DbElementOutcome edOutcome,
        bool requiresXver,
        bool requiresComponent,
        ILookup<int?, DbElementOutcome> outcomesByParentKey)
    {
        // get the child outcomes for this outcome
        List<DbElementOutcome> childOutcomes = outcomesByParentKey[edOutcome.Key]
            .OrderBy(eo => eo.SourceResourceOrder)
            .ToList();

        foreach (DbElementOutcome childOutcome in childOutcomes)
        {
            setOutcomeRequiresRecursive(
                childOutcome,
                requiresXver,
                requiresComponent,
                outcomesByParentKey);
        }

        if (requiresXver)
        {
            edOutcome.RequiresXVerDefinition = true;

            string nameClean = edOutcome.SourceNameClean();
            edOutcome.GenLongId = nameClean;
            edOutcome.GenShortId = nameClean;
            edOutcome.GenUrl = nameClean;

            edOutcome.ParentRequiresXverDefinition = true;
        }

        if (requiresComponent)
        {
            // clear the component gen info, since we are bubbling up
            edOutcome.ComponentGenLongId = null;
            edOutcome.ComponentGenShortId = null;
            edOutcome.ComponentGenUrl = null;

            edOutcome.RequiresComponentDefinition = true;

            edOutcome.ParentRequiresComponentDefinition = true;
        }
    }

    private bool relationshipMaps(CMR? relationship) =>
        (relationship is null) ||
        (relationship == CMR.Equivalent) ||
        (relationship == CMR.SourceIsNarrowerThanTarget);

    private bool skipType(string typeName) => typeName switch
    {
        "BackboneElement" => true,
        "Base" => true,
        "DataType" => true,
        "Element" => true,
        "Resource" => true,
        "DomainResource" => true,
        _ => false,
    };

    public void ProcessSourceStructure(
        DbStructureDefinition sourceSd,
        Dictionary<int, StructureOutcomeGenerator.StructureOutcomeTrackingRecord> structureTrackingRecords)
    {
        if (sourceSd.ArtifactClass == Common.Models.FhirArtifactClassEnum.PrimitiveType)
        {
            return;
        }

        Dictionary<int, DbElement> sourceElements = _allSourceElementsBySdKey[sourceSd.Key]
            .Where(ed => !skipElement(ed, skipFirstElement: false))
            .ToDictionary(c => c.Key);

        HashSet<int> fullyMappedElementsAllTargets = [];
        Dictionary<int, ElementOutcomeTrackingRecord> elementTrackingRecords = [];

        // iterate over the source elements for this structure to determine mapping completeness
        foreach (DbElement sourceEd in sourceElements.Values.OrderBy(ed => ed.ResourceFieldOrder))
        {
            // get all the source types for this element
            Dictionary<int, DbElementType> sourceEts = _sourceElementTypesByElementKey[sourceEd.Key]
                .ToDictionary(et => et.Key);

            if ((sourceEts.Count == 0) &&
                (sourceEd.BaseElementKey is not null))
            {
                sourceEts = _sourceElementTypesByElementKey[sourceEd.BaseElementKey!.Value]
                    .ToDictionary(et => et.Key);
            }

            // filter out base element types that make no sense to deal with (e.g., any target will cover)
            if (sourceEts.Count != 0)
            {
                List<int> typesToFilter = [];
                foreach ((int key, DbElementType et) in sourceEts)
                {
                    if (skipType(et.TypeName ?? et.Literal))
                    {
                        typesToFilter.Add(key);
                    }
                }

                foreach (int ttf in typesToFilter)
                {
                    sourceEts.Remove(ttf);
                }
            }

            // get the all comparisons for this element to the target FHIR package
            List<DbElementComparison> elementComparisons = _edComparsionsBySourceElementKey[sourceEd.Key]
                .ToList();

            HashSet<int> unrelatedEdComparisons = elementComparisons
                //.Where(ec => ec.NotMapped || (ec.Relationship == CMR.NotRelatedTo))
                .Where(ec => ec.TargetStructureKey is null)
                .Select(ec => ec.Key)
                .ToHashSet();

            // if we have at least one good comparsion, remove anything unmapped or not related
            if ((unrelatedEdComparisons.Count > 0) &&
                (unrelatedEdComparisons.Count < elementComparisons.Count))
            {
                elementComparisons = elementComparisons
                    .Where(ec => !unrelatedEdComparisons.Contains(ec.Key))
                    .ToList();
            }

            if (!elementTrackingRecords.TryGetValue(sourceEd.Key, out ElementOutcomeTrackingRecord? elementTrackingRec))
            {
                elementTrackingRec = new()
                {
                    SourceElement = sourceEd,
                    ElementComparisons = elementComparisons,
                    DiscreteTargetCount = elementComparisons
                        .Select(ec => ec.TargetElementKey)
                        .Where(tk => tk is not null)
                        .Distinct()
                        .Count(),
                    TargetElements = elementComparisons
                        .Where(ec => ec.TargetElementKey is not null)
                        .Select(ec => ec.TargetElementKey!.Value)
                        .Distinct()
                        .Select(key => _allTargetElements[key])
                        .ToDictionary(te => te.Key),
                };
                elementTrackingRecords[sourceEd.Key] = elementTrackingRec;
            }

            // easy check for any single comparison that fully maps
            List<DbElementComparison> fullyMappedComparisons = elementComparisons
                .Where(ec =>
                    (ec.TargetElementId is not null) &&
                    (ec.Relationship is not null) &&
                    relationshipMaps(ec.Relationship) &&
                    relationshipMaps(ec.BoundValueSetRelationship) &&
                    relationshipMaps(ec.TypeRelationship) &&
                    relationshipMaps(ec.TypeProfileRelationship) &&
                    relationshipMaps(ec.TargetProfileRelationship))
                .ToList();

            if (fullyMappedComparisons.Count != 0)
            {
                fullyMappedElementsAllTargets.Add(sourceEd.Key);

                elementTrackingRec.IsFullyMappedAcrossAllTargets = true;
                elementTrackingRec.MapsToIndividualTargets = fullyMappedComparisons;
                elementTrackingRec.Messages.Add(
                    $"Element `{sourceEd.Id}` has fully-mapped types to individual targets:" +
                    $" {string.Join(", ", fullyMappedComparisons.Select(fmc => $"`{fmc.TargetElementId}`"))}");

                elementTrackingRec.MappedTypes.AddRange(sourceEts.Values.OrderBy(et => et.Literal));

                continue;
            }

            // resolve the target elements involved in the comparisons
            Dictionary<int, DbElement> currentTargetElements = [];
            foreach (DbElementComparison ec in elementComparisons)
            {
                if ((ec.TargetStructureKey is null) ||
                    (ec.TargetContentKey is null))
                {
                    continue;
                }

                DbElement targetEd = _allTargetElements[ec.TargetContentKey!.Value];
                currentTargetElements[targetEd.Key] = targetEd;
            }

            elementTrackingRec.TargetElements = currentTargetElements;

            Dictionary<int, DbElementType> mappedTypes = [];
            Dictionary<int, DbElementType> unmappedTypes = sourceEts
                .Select(kvp => kvp)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            // check for no targets
            if (currentTargetElements.Count == 0)
            {
                elementTrackingRec.UnmappedTypes.AddRange(unmappedTypes.Values);

                elementTrackingRec.Messages.Add(
                    $"Element `{sourceEd.Id}` does not have any mapping targets." +
                    $" All source types are unmapped: `{sourceEd.FullCollatedTypeLiteral}`");

                // with no target, this is not mapped
                continue;
            }

            List<DbElementTypeComparison> etComparisons = _etcComparisonsBySourceElementKey[sourceEd.Key]
                .ToList();

            foreach (DbElementTypeComparison etc in etComparisons)
            {
                if (etc.TargetElementId is null)
                {
                    continue;
                }

                if ((etc.IsIdentical == true) ||
                    relationshipMaps(etc.Relationship))
                {
                    mappedTypes[etc.SourceElementTypeKey] = sourceEts[etc.SourceElementTypeKey];
                    unmappedTypes.Remove(etc.SourceElementTypeKey);

                    elementTrackingRec.Messages.Add(
                        $"Element `{sourceEd.Id}` type `{etc.SourceTypeLiteral}`" +
                        $" maps to target element `{etc.TargetElementId}` type `{etc.TargetTypeLiteral}`" +
                        $" with relationship {etc.Relationship}.");

                    if (unmappedTypes.Count == 0)
                    {
                        fullyMappedElementsAllTargets.Add(sourceEd.Key);

                        elementTrackingRec.MappedTypes.AddRange(mappedTypes.Values.OrderBy(et => et.Literal));
                        elementTrackingRec.IsFullyMappedAcrossAllTargets = true;
                        elementTrackingRec.MapsToIndividualTargets = fullyMappedComparisons;
                        elementTrackingRec.Messages.Add(
                            $"Element `{sourceEd.Id}` has mapped all types across all target elements:" +
                            $" {string.Join(", ", currentTargetElements.Values.Select(targetEd => $"`{targetEd.Id}`"))}");

                        break;
                    }
                }
            }

            // check if unmapped types are quantity types
            if ((unmappedTypes.Count > 0) &&
                (currentTargetElements.Count > 0))
            {
                DbStructureComparison? pairQuantityComparison = DbStructureComparison.SelectSingle(
                    _db,
                    SourceFhirPackageKey: _packagePair.SourcePackageKey,
                    SourceId: "Quantity",
                    TargetFhirPackageKey: _packagePair.TargetPackageKey,
                    TargetId: "Quantity");

                CMR qRelationship = pairQuantityComparison?.Relationship ?? CMR.Equivalent;

                // iterate over each unmapped type to see if it is a quantity type
                foreach (DbElementType unmappedType in unmappedTypes.Values)
                {
                    if (!unmappedType.IsQuantityType())
                    {
                        continue;
                    }

                    string sourceNormalizedTypeName = unmappedType.GetNormalizedName();

                    // build a list of target types and their elements (elements will be duplicated)
                    List<(DbElement, DbElementType)> targetQuantityTypes = [];
                    foreach (DbElement te in currentTargetElements.Values)
                    {
                        if (!_targetElementTypesByElementKey.Contains(te.Key))
                        {
                            continue;
                        }

                        IEnumerable<DbElementType> targetTypes = _targetElementTypesByElementKey[te.Key];

                        foreach (DbElementType tet in targetTypes)
                        {
                            if (tet.IsQuantityType())
                            {
                                targetQuantityTypes.Add((te, tet));
                            }
                        }
                    }

                    // resolve a source structure for the normalized type name
                    DbStructureDefinition? sourceQSd = DbStructureDefinition.SelectSingle(
                        _db,
                        FhirPackageKey: _packagePair.SourcePackageKey,
                        Name: sourceNormalizedTypeName);
                    
                    if ((sourceQSd is null) &&
                        (sourceNormalizedTypeName != "Quantity"))
                    {
                        // use the base Quantity type
                        sourceQSd = DbStructureDefinition.SelectSingle(
                            _db,
                            FhirPackageKey: _packagePair.SourcePackageKey,
                            Name: "Quantity");
                    }

                    if (sourceQSd is null)
                    {
                        continue;
                    }

                    // iterate over any matches
                    foreach ((DbElement targetEd, DbElementType targetType) in targetQuantityTypes)
                    {
                        string targetNormalizedTypeName = targetType.GetNormalizedName();

                        // if the source and target normalized names are the same, we can assume equivalence anyway
                        if (sourceNormalizedTypeName == targetNormalizedTypeName)
                        {
                            elementTrackingRec.Messages.Add(
                                $"Element `{sourceEd.Id}` normalized quantity type `{sourceNormalizedTypeName}`" +
                                $" maps to target element `{targetEd.Id}` normalized quantity type `{targetNormalizedTypeName}`" +
                                $" with assumed equivalence based on profile and type matching.");

                            mappedTypes[unmappedType.Key] = unmappedType;
                            unmappedTypes.Remove(unmappedType.Key);
                            if (unmappedTypes.Count == 0)
                            {
                                fullyMappedElementsAllTargets.Add(sourceEd.Key);

                                elementTrackingRec.MappedTypes.Add(unmappedType);
                                elementTrackingRec.IsFullyMappedAcrossAllTargets = true;
                                elementTrackingRec.MapsToIndividualTargets = fullyMappedComparisons;
                                elementTrackingRec.Messages.Add(
                                    $"Element `{sourceEd.Id}` has mapped all types across all target elements:" +
                                    $" {string.Join(", ", currentTargetElements.Values.Select(targetEd => $"`{targetEd.Id}`"))}");

                                elementTrackingRec.QuantityBasedRelationship = qRelationship;

                                break;
                            }

                            continue;
                        }

                        // resolve a target structure for the normalized type name
                        DbStructureDefinition? targetQSd = DbStructureDefinition.SelectSingle(
                            _db,
                            FhirPackageKey: _packagePair.TargetPackageKey,
                            Name: targetNormalizedTypeName);

                        if ((targetQSd is null) &&
                            (targetNormalizedTypeName != "Quantity"))
                        {
                            // use the base Quantity type
                            targetQSd = DbStructureDefinition.SelectSingle(
                                _db,
                                FhirPackageKey: _packagePair.TargetPackageKey,
                                Name: "Quantity");
                        }

                        if (targetQSd is null)
                        {
                            continue;
                        }

                        // check for comparison between the structures
                        DbStructureComparison? qComparison = DbStructureComparison.SelectSingle(
                            _db,
                            SourceFhirPackageKey: _packagePair.SourcePackageKey,
                            SourceStructureKey: sourceQSd.Key,
                            TargetFhirPackageKey: _packagePair.TargetPackageKey,
                            TargetStructureKey: targetQSd.Key);

                        if ((qComparison is not null) &&
                            relationshipMaps(qComparison.Relationship))
                        {
                            elementTrackingRec.Messages.Add(
                                $"Element `{sourceEd.Id}` normalized quantity type `{sourceNormalizedTypeName}`" +
                                $" maps to target element `{targetEd.Id}` normalized quantity type `{targetNormalizedTypeName}`" +
                                $" with relationship {qComparison.Relationship}.");

                            mappedTypes[unmappedType.Key] = unmappedType;
                            unmappedTypes.Remove(unmappedType.Key);
                            if (unmappedTypes.Count == 0)
                            {
                                fullyMappedElementsAllTargets.Add(sourceEd.Key);

                                elementTrackingRec.MappedTypes.Add(unmappedType);
                                elementTrackingRec.IsFullyMappedAcrossAllTargets = true;
                                elementTrackingRec.MapsToIndividualTargets = fullyMappedComparisons;
                                elementTrackingRec.Messages.Add(
                                    $"Element `{sourceEd.Id}` has mapped all types across all target elements:" +
                                    $" {string.Join(", ", currentTargetElements.Values.Select(targetEd => $"`{targetEd.Id}`"))}");

                                elementTrackingRec.QuantityBasedRelationship = qComparison.Relationship ?? qRelationship;

                                break;
                            }

                            continue;
                        }

                        // if there is no comparison, check to see if there is a mapping (primary first, then we'll check fallback)
                        DbStructureMapping? qMapping = DbStructureMapping.SelectSingle(
                            _db,
                            IsFallback: false,
                            SourceFhirPackageKey: _packagePair.SourcePackageKey,
                            SourceStructureKey: sourceQSd.Key,
                            TargetFhirPackageKey: _packagePair.TargetPackageKey,
                            TargetStructureKey: targetQSd.Key);

                        qMapping ??= DbStructureMapping.SelectSingle(
                            _db,
                            IsFallback: true,
                            SourceFhirPackageKey: _packagePair.SourcePackageKey,
                            SourceStructureKey: sourceQSd.Key,
                            TargetFhirPackageKey: _packagePair.TargetPackageKey,
                            TargetStructureKey: targetQSd.Key);

                        if ((qMapping is not null) &&
                            relationshipMaps(qMapping.Relationship))
                        {
                            elementTrackingRec.Messages.Add(
                                $"Element `{sourceEd.Id}` normalized quantity type `{sourceNormalizedTypeName}`" +
                                $" maps to target element `{targetEd.Id}` normalized quantity type `{targetNormalizedTypeName}`" +
                                $" with relationship {qMapping.Relationship}" +
                                $" (source ConceptMap: `{(qMapping.ConceptMapFilename ?? "-")}`, source FML: `{(qMapping.FmlFilename ?? "-")}`).");

                            mappedTypes[unmappedType.Key] = unmappedType;
                            unmappedTypes.Remove(unmappedType.Key);
                            if (unmappedTypes.Count == 0)
                            {
                                fullyMappedElementsAllTargets.Add(sourceEd.Key);

                                elementTrackingRec.MappedTypes.Add(unmappedType);
                                elementTrackingRec.IsFullyMappedAcrossAllTargets = true;
                                elementTrackingRec.MapsToIndividualTargets = fullyMappedComparisons;
                                elementTrackingRec.Messages.Add(
                                    $"Element `{sourceEd.Id}` has mapped all types across all target elements:" +
                                    $" {string.Join(", ", currentTargetElements.Values.Select(targetEd => $"`{targetEd.Id}`"))}");

                                elementTrackingRec.QuantityBasedRelationship = qMapping.Relationship ?? qRelationship;

                                break;
                            }

                            continue;
                        }
                    }

                    if (unmappedTypes.Count == 0)
                    {
                        break;
                    }
                }
            }

            // check if unmapped types can be handled via child element mappings (distributed mapping)
            if ((unmappedTypes.Count > 0) &&
                (sourceEd.ChildElementCount == 0) &&
                (currentTargetElements.Count > 0))
            {
                List<ChildTypeMappingResult> childMappingResults = [];

                // iterate over each unmapped type to check if its children provide mappings
                foreach (DbElementType unmappedType in unmappedTypes.Values)
                {
                    ChildTypeMappingResult cmr = checkChildElementMappings(
                        unmappedType,
                        sourceEd,
                        currentTargetElements);
                    childMappingResults.Add(cmr);
                }

                // process the results and update unmappedTypes accordingly
                HashSet<int> resolvedViaChildren = processChildMappingResults(
                    unmappedTypes,
                    childMappingResults,
                    elementTrackingRec,
                    sourceEd);

                elementTrackingRec.ChildTypeMappingResults.AddRange(childMappingResults);

                // if we resolved any types via children, log summary
                if (resolvedViaChildren.Count > 0)
                {
                    elementTrackingRec.Messages.Add(
                        $"Element `{sourceEd.Id}` resolved {resolvedViaChildren.Count} unmapped type(s)" +
                        $" via distributed child element mappings");
                }
            }

            // if we still have unmapped types, then this element is not fully mapped across all targets
            if (unmappedTypes.Count != 0)
            {
                elementTrackingRec.UnmappedTypes.AddRange(unmappedTypes.Values);

                elementTrackingRec.Messages.Add(
                    $"Element `{sourceEd.Id}` does not fully map to targets:" +
                    $" {string.Join(", ", currentTargetElements.Values.Select(te => $"`{te.Id}`"))}," +
                    $" because it does not account for source types:" +
                    $" {string.Join(", ", unmappedTypes.Values.Select(et => $"`{et.Literal}`"))}");

                // with unmapped types, there is no full mapping
                continue;
            }

            elementTrackingRec.Messages.Add(
                $"Element `{sourceEd.Id}` has fully-mapped types across: " +
                $" {string.Join(", ", currentTargetElements.Values.Select(te => $"`{te.Id}`"))}");

            elementTrackingRec.IsFullyMappedAcrossAllTargets = true;
            if (elementComparisons.Count == 1)
            {
                elementTrackingRec.MapsToIndividualTargets = elementComparisons;
            }
            else
            {
                elementTrackingRec.MapsToCombinationOfTargets.Add(elementComparisons);
            }
        }

        // iterate over our tracking records to check valueset mapping (if necessary)
        foreach (ElementOutcomeTrackingRecord elementTrackingRec in elementTrackingRecords.Values)
        {
            DbElement sourceEd = elementTrackingRec.SourceElement;

            if (sourceEd.ValueSetBindingStrength != Hl7.Fhir.Model.BindingStrength.Required)
            {
                if (sourceEd.ValueSetBindingStrength is not null)
                {
                    elementTrackingRec.Messages.Add(
                        $"Element `{sourceEd.Id}` has a" +
                        $" {sourceEd.ValueSetBindingStrength} binding to" +
                        $" `{sourceEd.BindingValueSet}`," +
                        $" which allows for any value and is not considered for full-mapping.");
                }

                continue;
            }

            bool vsMappingHasValidTarget = false;
            DbElement? mappingTargetEd = null;
            CMR? boundVsRelationship = CMR.Equivalent;

            foreach (DbElementComparison ec in elementTrackingRec.ElementComparisons)
            {
                if ((ec.TargetStructureKey is null) ||
                    (ec.TargetContentKey is null))
                {
                    continue;
                }

                boundVsRelationship = FhirDbComparer.ApplyRelationship(boundVsRelationship, ec.BoundValueSetRelationship);

                DbElement targetEd = elementTrackingRec.TargetElements[ec.TargetContentKey!.Value];

                // if the source and target are required, we need to consider the vs relationship
                if (targetEd.ValueSetBindingStrength == Hl7.Fhir.Model.BindingStrength.Required)
                {
                    // if the vs relationship is equivalent or source narrower, then we are good
                    if (relationshipMaps(ec.BoundValueSetRelationship))
                    {
                        vsMappingHasValidTarget = true;
                        mappingTargetEd = targetEd;

                        elementTrackingRec.Messages.Add(
                            $"Element `{sourceEd.Id}` has a required binding to `{sourceEd.BindingValueSet}`" +
                            $" and maps to `{targetEd.Id}` that has a required binding to `{targetEd.BindingValueSet}`" +
                            $" - the value set relationship is {ec.BoundValueSetRelationship}, which is considered fully-mapped.");

                        break;
                    }

                    elementTrackingRec.Messages.Add(
                        $"Element `{sourceEd.Id}` has a required binding to `{sourceEd.BindingValueSet}`" +
                        $" and maps to `{targetEd.Id}` that has a required binding to `{targetEd.BindingValueSet}`" +
                        $" - the value set relationship is {ec.BoundValueSetRelationship}, which is NOT considered fully-mapped.");

                    continue;
                }
            }

            elementTrackingRec.BoundValueSetRelationship = boundVsRelationship;
            if (!vsMappingHasValidTarget)
            {
                elementTrackingRec.IsFullyMappedAcrossAllTargets = false;
            }
        }

        Dictionary<(int sourceElementKey, int? targetStructureKey), (DbElementOutcome outcome, DbElementOutcome? rootOutcome)> edKeyOutcomeLookup = [];
        Dictionary<int, DbElementOutcome> rootEdOutcomesBySdOutcomeKey = [];

        //HashSet<int> outcomesRequiringXver = [];

        // iterate over our element tracking records to create outcomes
        foreach (ElementOutcomeTrackingRecord edTr in elementTrackingRecords.Values.OrderBy(etr => etr.SourceElement.ResourceFieldOrder))
        {
            DbElement sourceEd = edTr.SourceElement;
            List<DbElementComparison> elementComparisons = edTr.ElementComparisons;

            bool isRootElement = sourceEd.ResourceFieldOrder == 0;

            // iterate over the structure tracking records to create outcomes
            foreach (StructureOutcomeGenerator.StructureOutcomeTrackingRecord sdTr in structureTrackingRecords.Values)
            {
                (string idLong, string idShort) = XVerProcessor.GenerateExtensionId(
                    _packagePair.SourcePackageShortName,
                    sourceEd.Id);

                string extUrl = $"http://hl7.org/fhir/{_packagePair.SourceFhirVersionShort}/StructureDefinition/{idLong}";

                string? componentIdLong = null;
                string? componentIdShort = null;
                string? componentExtUrl = null;
                string? basicBasePath = null;
                DbElement? basicEd = null;
                DbExtensionSubstitution? extSubstitute = null;

                if (sourceEd.UsedAsContentReference == true)
                {
                    componentIdLong = idLong;
                    componentIdShort = idShort;
                    componentExtUrl = extUrl;
                }

                if (isRootElement)
                {
                    sdTr.SourceRootElement = sourceEd;
                }

                sdTr.IsFullyMappedAcrossAllTargets = sdTr.IsFullyMappedAcrossAllTargets &&
                    edTr.IsFullyMappedAcrossAllTargets;

                if (sdTr.TargetStructure is null)
                {
                    // don't create element outcomes for unmapped primitive types - they are handled at the structure level
                    if (sourceSd.ArtifactClass == Common.Models.FhirArtifactClassEnum.PrimitiveType)
                    {
                        continue;
                    }

                    string noMapEdComments =
                        $"Element `{sourceEd.Id}` is not mapped to FHIR {_packagePair.TargetFhirSequence}," +
                        $" since FHIR {_packagePair.SourceFhirSequence} `{sourceSd.Name}` is not mapped.";

                    List<string> noMapContexts = [];

                    int? noMapParentOutcomeKey = null;

                    bool defineNoMapAsModifier = sourceEd.IsModifier;
                    DbElementOutcome? rootEdOutcome = null;
                    if (isRootElement)
                    {
                        // no-map root elements can only go on `Basic`
                        noMapContexts = ["Basic"];
                        noMapParentOutcomeKey = null;
                    }
                    else
                    {
                        // no-map non-root elements are always part of the same full-resource structure
                        rootEdOutcomesBySdOutcomeKey.TryGetValue(sdTr.StructureOutcomeKey, out rootEdOutcome);

                        if ((sourceEd.ParentElementKey is not null) &&
                            edKeyOutcomeLookup.TryGetValue((sourceEd.ParentElementKey!.Value, sdTr.TargetStructure?.Key), out (DbElementOutcome outcome, DbElementOutcome? rootOutcome) noMapPO) &&
                            noMapPO.outcome.RequiresXVerDefinition)
                        {
                            noMapParentOutcomeKey = noMapPO.outcome.Key;
                            noMapContexts.Add(noMapPO.outcome.GenLongId ?? noMapPO.outcome.SourceName);
                        }
                    }

                    bool requiresXVerDefinition = !edTr.IsFullyMappedAcrossAllTargets;

                    // check to see if we are trying to define an extension onto basic that has a matching basic path
                    if (requiresXVerDefinition &&
                        _targetBasicElementPathLookup.TryGetValue(sourceEd.Path.Substring(sourceSd.Name.Length), out basicBasePath) &&
                        _targetBasicElementsById.TryGetValue(basicBasePath!, out basicEd))
                    {
                        if (canSourceMapToBasicElementType(sourceEd, basicEd) &&
                            (sourceEd.MinCardinality >= basicEd.MinCardinality) &&
                            ((basicEd.MaxCardinality == -1) || (basicEd.MaxCardinality >= sourceEd.MaxCardinality)) &&
                            (((sourceEd.ChildElementCount == 0) && (basicEd.ChildElementCount == 0)) ||
                                ((sourceEd.ChildElementCount > 0) && (basicEd.ChildElementCount > 0))))
                        {
                            requiresXVerDefinition = false;
                            noMapEdComments +=
                                $"\nElement matches Basic element path `{basicBasePath}`," +
                                $" use that element instead.";
                            noMapContexts = [];
                            noMapParentOutcomeKey = null;
                        }
                        else
                        {
                            basicBasePath = null;
                            noMapEdComments +=
                                $"\nNote that the source element matches Basic element path `{basicBasePath}`," +
                                $" but the definitions are not compatible" +
                                $" (source: `{sourceEd.FullCollatedTypeLiteral}`:{sourceEd.FhirCardinalityString}" +
                                $" -> basic: `{basicEd.FullCollatedTypeLiteral}`:{basicEd.FhirCardinalityString}).";
                        }
                    }

                    if (_extensionSubstitutionsByElementId.TryGetValue(sourceEd.Id, out extSubstitute))
                    {
                        noMapEdComments +=
                            $"\nNote that there is an externally-defined extension that has been flagged as the" +
                            $" representation of FHIR {_packagePair.SourceFhirSequence} element `{sourceEd.Id}`:" +
                            $" `{extSubstitute.ReplacementUrl}`.";
                    }

                    DbElementComparison? noMapElementComparison = elementComparisons
                        .FirstOrDefault(ec => ec.TargetStructureKey is null);
                    //?? throw new Exception($"Non-mapped {sdTr.SourceStructure.Name} element {sourceEd.Id} has no non-mapped comparison!");

                    //if (((rootEdOutcome is not null) && outcomesRequiringXver.Contains(rootEdOutcome.Key)) ||
                    if ((rootEdOutcome?.RequiresXVerDefinition == true) ||
                        (basicBasePath is not null))
                    {
                        idLong = sourceEd.NameClean();
                        idShort = idLong;
                        extUrl = idLong;
                    }

                    // create the no-map element outcome
                    DbElementOutcome noMapEdOutcome = new()
                    {
                        Key = DbElementOutcome.GetIndex(),
                        StructureOutcomeKey = sdTr.StructureOutcomeKey,

                        SourceFhirPackageKey = _packagePair.SourcePackageKey,
                        SourceFhirSequence = _packagePair.SourceFhirSequence,
                        SourceStructureKey = sourceSd.Key,
                        SourceElementKey = sourceEd.Key,
                        SourceCanonicalUnversioned = sourceSd.UnversionedUrl,
                        SourceCanonicalVersioned = sourceSd.VersionedUrl,
                        SourceVersion = sourceSd.Version,
                        SourceId = sourceEd.Id,
                        SourceName = sourceEd.Name,
                        SourceResourceOrder = sourceEd.ResourceFieldOrder,
                        SourceComponentOrder = sourceEd.ComponentFieldOrder,
                        SourceMinCardinality = sourceEd.MinCardinality,
                        SourceMaxCardinalityString = sourceEd.MaxCardinalityString,
                        SourceChildElementCount = sourceEd.ChildElementCount,
                        SourceUsedAsContentReference = sourceEd.UsedAsContentReference == true,
                        TotalSourceCount = 1,

                        TargetFhirPackageKey = _packagePair.TargetPackageKey,
                        TargetFhirSequence = _packagePair.TargetFhirSequence,
                        TargetStructureKey = null,
                        TargetElementKey = null,
                        TargetCanonicalUnversioned = null,
                        TargetCanonicalVersioned = null,
                        TargetVersion = null,
                        TargetId = null,
                        TargetName = null,
                        TargetResourceOrder = null,
                        TargetComponentOrder = null,
                        TotalTargetCount = edTr.DiscreteTargetCount,

                        ElementComparisonKey = noMapElementComparison?.Key ?? -1,

                        RequiresXVerDefinition = requiresXVerDefinition,
                        GenLongId = rootEdOutcome is null ? idLong : sourceEd.NameClean(),
                        GenShortId = rootEdOutcome is null ? idShort : sourceEd.NameClean(),
                        GenUrl = rootEdOutcome is null ? extUrl : sourceEd.NameClean(),

                        RequiresComponentDefinition = sourceEd.UsedAsContentReference == true,
                        ComponentGenLongId = componentIdLong,
                        ComponentGenShortId = componentIdShort,
                        ComponentGenUrl = componentExtUrl,

                        AncestorElementOutcomeKey = requiresXVerDefinition ? rootEdOutcome?.Key : null,
                        ParentElementOutcomeKey = noMapParentOutcomeKey,
                        ParentRequiresXverDefinition = noMapParentOutcomeKey is not null,
                        ParentRequiresComponentDefinition = false,
                        SourceIsModifier = sourceEd.IsModifier,
                        DefineAsModifier = defineNoMapAsModifier,
                        ExtensionContexts = noMapContexts,
                        BasicElementEquivalent = basicBasePath,
                        ExtensionSubstitutionKey = extSubstitute?.Key,
                        ExtensionSubstitutionUrl = extSubstitute?.ReplacementUrl,

                        IsRenamed = false,
                        IsUnmapped = false,
                        IsIdentical = false,
                        IsEquivalent = false,
                        IsBroaderThanTarget = false,
                        IsNarrowerThanTarget = false,

                        FullyMapsToThisTarget = false,
                        FullyMapsAcrossAllTargets = edTr.IsFullyMappedAcrossAllTargets,

                        MappedTypeKeys = edTr.MappedTypes.Select(mt => mt.Key).ToList(),
                        MappedTypeNames = edTr.MappedTypes.Select(mt => mt.Literal).ToList(),
                        UnmappedTypeKeys = edTr.UnmappedTypes.Select(ut => ut.Key).ToList(),
                        UnmappedTypeNames = edTr.UnmappedTypes.Select(ut => ut.Literal).ToList(),

                        MappedTypeChildElementKeys = edTr.ChildTypeMappingResults
                            .SelectMany(ctr => ctr.MappedTypeChildren)
                            .Select(med => med.Key)
                            .Distinct()
                            .ToList(),
                        MappedChildTypeElementNames = edTr.ChildTypeMappingResults
                            .SelectMany(ctr => ctr.MappedTypeChildren)
                            .Select(med => med.Name)
                            .Distinct()
                            .ToList(),
                        UnmappedTypeChildKeys = edTr.ChildTypeMappingResults
                            .SelectMany(ctr => ctr.UnmappedTypeChildren)
                            .Select(ued => ued.Key)
                            .Distinct()
                            .ToList(),
                        UnmappedChildTypeNames = edTr.ChildTypeMappingResults
                            .SelectMany(ctr => ctr.UnmappedTypeChildren)
                            .Select(ued => ued.Name)
                            .Distinct()
                            .ToList(),

                        Comments = noMapEdComments,
                    };

                    //if (requiresXVerDefinition)
                    //{
                    //    outcomesRequiringXver.Add(noMapEdOutcome.Key);
                    //}

                    // if this is the root element, being a non-mapped structure includes changes
                    if (isRootElement)
                    {
                        noMapEdOutcome.Comments =
                            $"FHIR {_packagePair.SourceFhirSequence} {sdTr.SourceStructure.ArtifactClass} `{sourceSd.Name}`" +
                            $" has no mapping to FHIR {_packagePair.TargetFhirSequence}.";

                        rootEdOutcomesBySdOutcomeKey[sdTr.StructureOutcomeKey] = noMapEdOutcome;
                    }

                    _edOutcomeCache.CacheAdd(noMapEdOutcome);
                    edTr.ElementOutcomes.Add(noMapEdOutcome);
                    sdTr.ElementOutcomes.Add(noMapEdOutcome);
                    edKeyOutcomeLookup.Add((sourceEd.Key, null), (noMapEdOutcome, rootEdOutcome));
                    continue;
                }

                // skip primitive targets
                if (sdTr.TargetStructure.ArtifactClass == Common.Models.FhirArtifactClassEnum.PrimitiveType)
                {
                    continue;
                }

                // if we target multiple elements, we need to consolidate to a single outcome that can target a common parent element (if one is necessary)
                List<DbElementComparison> targetSdEdComparisons = elementComparisons
                    .Where(ec => ec.TargetStructureKey == sdTr.TargetStructure.Key)
                    .ToList();

                // if this structure does not have any comparisons for this element, skip it
                if (targetSdEdComparisons.Count == 0)
                {
                    // this should never be possible - non-mapping elements have already been handled
                    continue;
                }

                DbElementComparison? singleEC = targetSdEdComparisons.Count == 1
                    ? targetSdEdComparisons[0]
                    : null;

                List<DbElement> targetEds = targetSdEdComparisons
                    .Where(ec => ec.TargetElementKey is not null)
                    .Distinct()
                    .Select(ec => _allTargetElements[ec.TargetElementKey!.Value])
                    .ToList();

                DbElement? targetEd = targetEds.Count == 1
                    ? targetEds[0]
                    : findCommonAncestor(sdTr.TargetStructure.FhirPackageKey, targetEds);

                //// traverse the comparisons for this element (can map to multiple elments in target structure)
                //foreach (DbElementComparison elementComparison in elementComparisons.Where(ec => ec.TargetStructureKey == sdTr.TargetStructure.Key))
                //{
                //    DbElement? targetEd = elementComparison.TargetElementKey is null
                //        ? null
                //        : _allTargetElements[elementComparison.TargetElementKey.Value];

                    DbElementOutcome? ancestorOutcome = null;
                    DbElementOutcome? parentOutcome = null;

                    if (!edTr.IsFullyMappedAcrossAllTargets)
                    {
                        sdTr.IsFullyMappedAcrossAllTargets = false;
                        if ((singleEC is not null) &&
                            !edTr.MapsToIndividualTargets.Any(ec => ec.TargetElementKey == singleEC.TargetElementKey))
                        {
                            sdTr.IsFullyMappedToThisTarget = false;
                        }
                    }

                    string comments = edTr.Messages.Count > 0
                        ? string.Join('\n', edTr.Messages)
                        : string.Join('\n', targetSdEdComparisons.Select(ec => ec.TechnicalMessage ?? ec.UserMessage));

                    bool elementRequiresXVer = !edTr.IsFullyMappedAcrossAllTargets;
                    if ((sourceEd.ParentElementKey is not null) &&
                        edKeyOutcomeLookup.TryGetValue((sourceEd.ParentElementKey!.Value, sdTr.TargetStructure.Key), out (DbElementOutcome outcome, DbElementOutcome? ancestor) po))
                    {
                        //parentOutcome = po.outcome.RequiresXVerDefinition ? po.outcome : null;
                        parentOutcome = po.outcome;
                        ancestorOutcome = po.ancestor;
                    }

                    // check for the current element not thinking it needs a definition, but the parent does
                    if (!elementRequiresXVer && (parentOutcome?.RequiresXVerDefinition == true))
                    {
                        elementRequiresXVer = true;
                    }

                    if (parentOutcome?.RequiresXVerDefinition == true)
                    {
                        comments = $"Element `{sourceEd.Id}` is part of a definition because parent element `{parentOutcome.SourceId}` requires an extension.";
                    }

                    bool fullyMapsToAllTargets = edTr.IsFullyMappedAcrossAllTargets;
                    bool fullyMapsToThisTarget = fullyMapsToAllTargets &&
                        (targetEd is not null) &&
                        edTr.MapsToIndividualTargets.Any(ec => ec.TargetElementKey == targetEd?.Key);

                    List<string> contexts = [];
                    List<DbElement> contextTargetElements = [];

                    bool defineAsModifier = sourceEd.IsModifier;

                    // if this is the root element, force some values
                    if (isRootElement)
                    {
                        elementRequiresXVer = false;
                        ancestorOutcome = null;
                        parentOutcome = null;
                        comments =
                            $"FHIR {_packagePair.SourceFhirSequence} {sourceSd.ArtifactClass} `{sourceSd.Name}`" +
                            $" is representable via" +
                            $" FHIR {_packagePair.TargetFhirSequence} {sdTr.TargetStructure.ArtifactClass} `{sdTr.TargetStructure.Name}`.";

                        fullyMapsToThisTarget = (sdTr.StructureComparison.IsIdentical == true) ||
                            (sdTr.StructureComparison.Relationship == CMR.Equivalent) ||
                            (sdTr.StructureComparison.Relationship == CMR.SourceIsNarrowerThanTarget);
                    }
                    else if (elementRequiresXVer && (parentOutcome?.RequiresXVerDefinition == true))
                    {
                        contexts.Add(parentOutcome.GenLongId ?? parentOutcome.SourceName);
                        DbElement? parentTargetElement = parentOutcome.TargetElementKey is null
                            ? null
                            : _allTargetElements[parentOutcome.TargetElementKey.Value];
                        if (parentTargetElement is not null)
                        {
                            contextTargetElements.Add(parentTargetElement);
                        }
                    }
                    else
                    {
                        // if we have a target element, that is the context we want
                        if (targetEd is not null)
                        {
                            contexts.Add(targetEd.Id);
                            contextTargetElements.Add(targetEd);
                        }
                        // if the source element is in the top level of the resource, we use the target resource as context
                        else if (sourceEd.ParentElementKey == sdTr.SourceRootElement?.Key)
                        {
                            contexts.Add(sdTr.TargetStructure.Name);
                            if (sdTr.SourceRootElement is not null)
                            {
                                contextTargetElements.Add(sdTr.SourceRootElement);
                            }
                        }
                        else
                        {
                            DbElement currentSourceEd = sourceEd;

                            // need to walk up parent elements until we find something that can be used
                            while (contexts.Count == 0)
                            {
                                if (currentSourceEd.ParentElementKey is null)
                                {
                                    // we reached the top without finding anything
                                    contexts.Add(sdTr.TargetStructure.Name);
                                    break;
                                }

                                currentSourceEd = _allSourceElements[currentSourceEd.ParentElementKey.Value];

                                // check for any comparisons
                                List<DbElementComparison> contextEdComparisons = _edComparsionsBySourceElementKey[currentSourceEd.Key]
                                    .Where(ec => ec.TargetStructureKey == sdTr.TargetStructure.Key)
                                    .ToList();

                                if (contextEdComparisons.Count == 0)
                                {
                                    continue;
                                }

                                foreach (DbElementComparison contextEdComparison in contextEdComparisons)
                                {
                                    if (contextEdComparison.TargetElementId is not null)
                                    {
                                        contexts.Add(contextEdComparison.TargetElementId);
                                        DbElement? contextTargetElement = contextEdComparison.TargetElementKey is null
                                            ? null
                                            : _allTargetElements[contextEdComparison.TargetElementKey.Value];
                                        if (contextTargetElement is not null)
                                        {
                                            contextTargetElements.Add(contextTargetElement);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    // if we need a context and failed to find one, add 'Element'
                    if ((contexts.Count == 0) &&
                        elementRequiresXVer &&
                        (ancestorOutcome is null))
                    {
                        contexts.Add("Element");
                    }

                    // check to see if we are trying to define an extension onto basic that has a matching basic path
                    if (elementRequiresXVer &&
                        _targetBasicElementPathLookup.TryGetValue(sourceEd.Path.Substring(sourceSd.Name.Length), out basicBasePath) &&
                        _targetBasicElementsById.TryGetValue(basicBasePath!, out basicEd))
                    {
                        if (canSourceMapToBasicElementType(sourceEd, basicEd) &&
                            (sourceEd.MinCardinality >= basicEd.MinCardinality) &&
                            ((basicEd.MaxCardinality == -1) || (basicEd.MaxCardinality >= sourceEd.MaxCardinality)) &&
                            (((sourceEd.ChildElementCount == 0) && (basicEd.ChildElementCount == 0)) ||
                                ((sourceEd.ChildElementCount > 0) && (basicEd.ChildElementCount > 0))))
                        {
                            elementRequiresXVer = false;
                            comments +=
                                $"\nElement matches Basic element path `{basicBasePath}`," +
                                $" use that element instead.";
                            elementRequiresXVer = false;
                            contexts = [];
                            parentOutcome = null;
                        }
                        else
                        {
                            basicBasePath = null;
                            comments +=
                                $"\nNote that the source element matches Basic element path `{basicBasePath}`," +
                                $" but the definitions are not compatible" +
                                $" (source: `{sourceEd.FullCollatedTypeLiteral}`:{sourceEd.FhirCardinalityString}" +
                                $" -> basic: `{basicEd.FullCollatedTypeLiteral}`:{basicEd.FhirCardinalityString}).";
                        }
                    }

                    List<DbElement> ctxToRemove = [];
                    List<DbElement> ctxToAdd = [];

                    // if the source is a modifier, figure out if we can actually do that
                    if (elementRequiresXVer && defineAsModifier)
                    {
                        /*
                         * Determine if this extension should really be a modifier based on context:
                         *  * Modifier element -> Modifier element : extension
                         *  * Modifier element -> Backbone element (not modifier) : modifier extension
                         *  * Modifier element -> Primitive-type element (not modifier) : modifier extension, context moves up a level
                         *  * Modifier element -> Primitive-type element (array, not modifier) : currently unresolvable, but also has not happened yet
                         */

                        if (contextTargetElements.Count == 0)
                        {
                            //bool promoted = false;
                            if (parentOutcome?.RequiresXVerDefinition == true)
                            {
                                //promoted = true;
                                // need to promote the modifier to the parent context
                                parentOutcome.DefineAsModifier = true;
                                parentOutcome.Comments +=
                                    $"\nNote that the child extension for element `{sourceEd.Name}`" +
                                    $" is a modifier, so this extension needs to be defined as a modifier.";
                            }

                            if ((ancestorOutcome is not null) &&
                                (ancestorOutcome.Key != parentOutcome?.Key))
                            {
                                //promoted = true;
                                // need to promote the modifier to the ancestor context
                                ancestorOutcome.DefineAsModifier = true;
                                ancestorOutcome.Comments +=
                                    $"\nNote that the child extension for element `{sourceEd.Id}`" +
                                    $" is a modifier, so this extension needs to be defined as a modifier.";
                            }

                            //if (!promoted)
                            //{
                            //    Console.Write("");
                            //}
                        }
                        else
                        {
                            // iterate over the context target elements for initial checks
                            foreach (DbElement ctxTargetEd in contextTargetElements)
                            {
                                // if this is a modifier element, we do not need to define as a modifier
                                if (ctxTargetEd.IsModifier)
                                {
                                    defineAsModifier = false;
                                    comments +=
                                        $"\nNote that the target element context `{ctxTargetEd.Id}` is a modifier element," +
                                        $" so this extension does not need to be defined as a modifier.";
                                    break;
                                }

                                if (ctxTargetEd.ChildElementCount > 0)
                                {
                                    // this is okay - elements with children can have modifier extensions
                                    continue;
                                }

                                // check to see if the element is or only has primitive types
                                List<DbElementType> ctxTargetEdTypes = _targetElementTypesByElementKey.Contains(ctxTargetEd.Key)
                                    ? _targetElementTypesByElementKey[ctxTargetEd.Key].ToList()
                                    : [];

                                if (ctxTargetEdTypes.All(ctxEt => FhirTypeUtils.IsPrimitiveType(ctxEt.TypeName ?? ctxEt.Literal)))
                                {
                                    // need to move up to a higher level
                                    if (ctxTargetEd.ParentElementKey is null)
                                    {
                                        throw new Exception(
                                            $"Cannot determine modifier extension context for source element `{sourceEd.Id}`" +
                                            $" mapping to target primitive-type element `{ctxTargetEd.Id}`" +
                                            $" because the target element has no parent to move up to.");
                                    }

                                    DbElement ctxTargetParentEd = _allTargetElements[ctxTargetEd.ParentElementKey.Value];

                                    contexts.Remove(ctxTargetEd.Id);
                                    ctxToRemove.Add(ctxTargetEd);

                                    contexts.Add(ctxTargetParentEd.Id);
                                    ctxToAdd.Add(ctxTargetParentEd);

                                    comments +=
                                        $"\nNote that the target element context `{ctxTargetEd.Id}` is a primitive-type element" +
                                        $" and this extension needs to be defined as a modifier. The context is moved up to parent element `{ctxTargetParentEd.Id}`.";
                                }
                            }

                            foreach (DbElement e in ctxToRemove)
                            {
                                contextTargetElements.Remove(e);
                            }
                            ctxToRemove.Clear();

                            foreach (DbElement e in ctxToAdd)
                            {
                                contextTargetElements.Add(e);
                            }
                            ctxToAdd.Clear();
                        }
                    }

                    // iterate over the context target elements to see if we have targets that are choice types
                    foreach (DbElement ctxTargetEd in contextTargetElements)
                    {
                        if (!ctxTargetEd.IsChoiceType)
                        {
                            continue;
                        }

                        // need to move up to a higher level
                        if (ctxTargetEd.ParentElementKey is null)
                        {
                            throw new Exception(
                                $"Cannot determine modifier extension context for source element `{sourceEd.Id}`" +
                                $" mapping to target choice-type element `{ctxTargetEd.Id}`" +
                                $" because the target element has no parent to move up to.");
                        }

                        DbElement ctxTargetParentEd = _allTargetElements[ctxTargetEd.ParentElementKey.Value];

                        contexts.Remove(ctxTargetEd.Id);
                        ctxToRemove.Add(ctxTargetEd);

                        contexts.Add(ctxTargetParentEd.Id);
                        ctxToAdd.Add(ctxTargetParentEd);

                        comments +=
                            $"\nNote that the target element context `{ctxTargetEd.Id}` is a choice-type element" +
                            $" and cannot directly hold extensions. The context is moved up to parent element `{ctxTargetParentEd.Id}`.";
                    }

                    foreach (DbElement e in ctxToRemove)
                    {
                        contextTargetElements.Remove(e);
                    }
                    ctxToRemove.Clear();

                    foreach (DbElement e in ctxToAdd)
                    {
                        contextTargetElements.Add(e);
                    }
                    ctxToAdd.Clear();

                    // build target canonical urls
                    string? targetCanonicalUnversioned = targetEd is null
                        ? null
                        : $"{sdTr.TargetStructure.UnversionedUrl}#{targetEd.Id}";
                    string? targetCanonicalVersioned = targetCanonicalUnversioned is null
                        ? null
                        : $"{targetCanonicalUnversioned}|{sdTr.TargetStructure.Version ?? _packagePair.TargetPackage.PackageVersion}";

                    if (_extensionSubstitutionsByElementId.TryGetValue(sourceEd.Id, out extSubstitute))
                    {
                        comments +=
                            $"\nNote that there is an externally-defined extension that has been flagged as the" +
                            $" representation of FHIR {_packagePair.SourceFhirSequence} element `{sourceEd.Id}`:" +
                            $" `{extSubstitute.ReplacementUrl}`.";
                    }

                    //if (((parentOutcome is not null) && outcomesRequiringXver.Contains(parentOutcome.Key)) ||
                    if ((parentOutcome?.RequiresXVerDefinition == true) ||
                        (basicBasePath is not null))
                    {
                        idLong = sourceEd.NameClean();
                        idShort = idLong;
                        extUrl = idLong;
                    }

                    //if ((parentOutcome is not null) && (basicBasePath is not null))
                    //{
                    //    idLong = sourceEd.NameClean();
                    //    idShort = idLong;
                    //    extUrl = idLong;
                    //}

                    // create the mapped element outcome
                    DbElementOutcome elementOutcome = new()
                    {
                        Key = DbElementOutcome.GetIndex(),
                        StructureOutcomeKey = sdTr.StructureOutcomeKey,
                        ElementComparisonKey = singleEC?.Key,

                        SourceFhirPackageKey = _packagePair.SourcePackageKey,
                        SourceFhirSequence = _packagePair.SourceFhirSequence,
                        SourceStructureKey = sourceSd.Key,
                        SourceElementKey = sourceEd.Key,
                        SourceCanonicalUnversioned = sourceSd.UnversionedUrl,
                        SourceCanonicalVersioned = sourceSd.VersionedUrl,
                        SourceVersion = sourceSd.Version,
                        SourceId = sourceEd.Id,
                        SourceName = sourceEd.Name,
                        SourceResourceOrder = sourceEd.ResourceFieldOrder,
                        SourceComponentOrder = sourceEd.ComponentFieldOrder,
                        SourceMinCardinality = sourceEd.MinCardinality,
                        SourceMaxCardinalityString = sourceEd.MaxCardinalityString,
                        SourceChildElementCount = sourceEd.ChildElementCount,
                        SourceUsedAsContentReference = sourceEd.UsedAsContentReference == true,
                        TotalSourceCount = -1,

                        TargetFhirPackageKey = _packagePair.TargetPackageKey,
                        TargetFhirSequence = _packagePair.TargetFhirSequence,
                        TargetStructureKey = sdTr.TargetStructure.Key,
                        TargetElementKey = targetEd?.Key,
                        TargetCanonicalUnversioned = targetCanonicalUnversioned,
                        TargetCanonicalVersioned = targetCanonicalVersioned,
                        TargetVersion = sdTr.TargetStructure.Version ?? _packagePair.TargetPackage.PackageVersion,
                        TargetId = targetEd?.Id,
                        TargetName = targetEd?.Name,
                        TargetResourceOrder = targetEd?.ResourceFieldOrder,
                        TargetComponentOrder = targetEd?.ComponentFieldOrder,
                        TotalTargetCount = edTr.DiscreteTargetCount,

                        RequiresXVerDefinition = elementRequiresXVer,
                        GenLongId = idLong,
                        GenShortId = idShort,
                        GenUrl = extUrl,

                        RequiresComponentDefinition = sourceEd.UsedAsContentReference == true,
                        ComponentGenLongId = componentIdLong,
                        ComponentGenShortId = componentIdShort,
                        ComponentGenUrl = componentExtUrl,

                        AncestorElementOutcomeKey = elementRequiresXVer ? ancestorOutcome?.Key : null,
                        ParentElementOutcomeKey = parentOutcome?.Key,
                        ParentRequiresXverDefinition = parentOutcome?.RequiresXVerDefinition ?? false,
                        ParentRequiresComponentDefinition = parentOutcome?.RequiresComponentDefinition ?? false,
                        SourceIsModifier = sourceEd.IsModifier,
                        DefineAsModifier = defineAsModifier,
                        ExtensionContexts = contexts,
                        BasicElementEquivalent = basicBasePath,
                        ExtensionSubstitutionKey = extSubstitute?.Key,
                        ExtensionSubstitutionUrl = extSubstitute?.ReplacementUrl,

                        IsRenamed = targetEd is null ? false : (sourceEd.Name != targetEd.Name),
                        IsUnmapped = targetEd is null || (singleEC?.NotMapped == true),
                        IsIdentical = singleEC?.IsIdentical == true,
                        IsEquivalent = singleEC?.Relationship == CMR.Equivalent,
                        IsBroaderThanTarget = singleEC?.Relationship == CMR.SourceIsBroaderThanTarget,
                        IsNarrowerThanTarget = singleEC?.Relationship == CMR.SourceIsNarrowerThanTarget,

                        FullyMapsAcrossAllTargets = fullyMapsToAllTargets,
                        FullyMapsToThisTarget = fullyMapsToThisTarget,

                        MappedTypeKeys = edTr.MappedTypes.Select(mt => mt.Key).ToList(),
                        MappedTypeNames = edTr.MappedTypes.Select(mt => mt.Literal).ToList(),
                        UnmappedTypeKeys = edTr.UnmappedTypes.Select(ut => ut.Key).ToList(),
                        UnmappedTypeNames = edTr.UnmappedTypes.Select(ut => ut.Literal).ToList(),

                        MappedTypeChildElementKeys = edTr.ChildTypeMappingResults
                            .SelectMany(ctr => ctr.MappedTypeChildren)
                            .Select(med => med.Key)
                            .Distinct()
                            .ToList(),
                        MappedChildTypeElementNames = edTr.ChildTypeMappingResults
                            .SelectMany(ctr => ctr.MappedTypeChildren)
                            .Select(med => med.Name)
                            .Distinct()
                            .ToList(),
                        UnmappedTypeChildKeys = edTr.ChildTypeMappingResults
                            .SelectMany(ctr => ctr.UnmappedTypeChildren)
                            .Select(ued => ued.Key)
                            .Distinct()
                            .ToList(),
                        UnmappedChildTypeNames = edTr.ChildTypeMappingResults
                            .SelectMany(ctr => ctr.UnmappedTypeChildren)
                            .Select(ued => ued.Name)
                            .Distinct()
                            .ToList(),

                        Comments = comments,
                    };

                    if (isRootElement)
                    {
                        rootEdOutcomesBySdOutcomeKey[sdTr.StructureOutcomeKey] = elementOutcome;
                    }

                    _edOutcomeCache.CacheAdd(elementOutcome);
                    edTr.ElementOutcomes.Add(elementOutcome);
                    sdTr.ElementOutcomes.Add(elementOutcome);
                    if (!edKeyOutcomeLookup.ContainsKey((sourceEd.Key, sdTr.TargetStructure.Key)))
                    {
                        edKeyOutcomeLookup.Add(
                            (sourceEd.Key, sdTr.TargetStructure.Key),
                            (elementOutcome, ancestorOutcome));
                    }
                //}
            }
        }

        foreach (DbElementOutcome edRootOutcome in rootEdOutcomesBySdOutcomeKey.Values)
        {
            edOutcomePostProcessing(
                edKeyOutcomeLookup
                    .Where(kvp => kvp.Key.targetStructureKey == edRootOutcome.TargetStructureKey)
                    .Select(kvp => kvp.Value.outcome),
                edRootOutcome);
        }
    }

    private DbElement? findCommonAncestor(int targetFhirPackageKey, List<DbElement> eds)
    {
        if (eds.Count == 0)
        {
            return null;
        }

        if (eds.Count == 1)
        {
            return eds[0];
        }

        List<HashSet<string>> components = [];

        // build the list of path components that appear in each element
        foreach (DbElement ed in eds)
        {
            string[] idComponents = ed.Id.Split('.');
            for (int i = 0; i < idComponents.Length; i++)
            {
                if (components.Count <= i)
                {
                    components.Add([]);
                }
                components[i].Add(idComponents[i]);
            }
        }

        // work backwards to find the last common component
        for (int i = components.Count - 1; i >= 0; i--)
        {
            if (components[i].Count > 1)
            {
                // still multiple element paths
                continue;
            }

            string commonId = string.Join('.', components[0..(i + 1)].Select(c => c.First()));

            DbElement? commonEd = DbElement.SelectSingle(_db, FhirPackageKey: targetFhirPackageKey, Id: commonId);
            return commonEd;
        }

        return null;
    }

    /// <summary>
    /// Determines if an unmapped source type can be fully represented through mappings of its child elements to target elements.
    /// </summary>
    /// <param name="sourceEt">The source type that lacks a direct mapping</param>
    /// <param name="sourceEd">The source element containing this type</param>
    /// <returns>Result indicating if all children are mapped and how</returns>
    private ChildTypeMappingResult checkChildElementMappings(
        DbElementType sourceEt,
        DbElement sourceEd,
        Dictionary<int, DbElement> currentTargetElements)
    {
        ChildTypeMappingResult result = new()
        {
            SourceParentType = sourceEt,
            AllChildrenMapped = false,
            ChildMappings = [],
            MappedTypeChildren = [],
            UnmappedTypeChildren = []
        };

        // type must have a resolved structure to check children
        if (sourceEt.TypeStructureKey is null)
        {
            return result;
        }

        // get the child elements of this type structure, but filter out elements we should ignore (root, id, extension, modifierExtension)
        List<DbElement> typeChildren = _allSourceElementsBySdKey[sourceEt.TypeStructureKey.Value]
            .Where(ed => !skipElement(ed))
            .ToList();

        // if there are no meaningful child elements, this type can't be distributed
        if (typeChildren.Count == 0)
        {
            return result;
        }

        HashSet<int> targetStructureKeysHash = [];
        HashSet<string> targetTypeLiterals = [];

        // build a list of target types and their elements (elements will be duplicated)
        List<(DbElement, DbElementType)> targetTypes = [];
        foreach (DbElement te in currentTargetElements.Values)
        {
            targetStructureKeysHash.Add(te.StructureKey);

            if (!_targetElementTypesByElementKey.Contains(te.Key))
            {
                continue;
            }

            IEnumerable<DbElementType> currentTargetTypes = _targetElementTypesByElementKey[te.Key];

            foreach (DbElementType tet in currentTargetTypes)
            {
                targetTypes.Add((te, tet));
                targetTypeLiterals.Add(tet.Literal);
            }
        }

        List<int> targetStructureKeys = targetStructureKeysHash.ToList();

        // iterate over the child elements to try and map the types
        foreach (DbElement childEd in typeChildren)
        {
            // check to see if there is a source mapping that drilled into this type from the original source element
            string typeChildId = $"{sourceEd.Id}.{childEd.Name}";
            List<DbElementMapping> typeChildMappings = DbElementMapping.SelectList(
                _db,
                SourceFhirPackageKey: _packagePair.SourcePackageKey,
                SourceElementId: typeChildId,
                TargetFhirPackageKey: _packagePair.TargetPackageKey,
                TargetStructureKeyValues: targetStructureKeys);

            List<DbElementMapping> validTypeChildMappings = typeChildMappings
                .Where(em => (em.Relationship == CMR.Equivalent) || (em.Relationship == CMR.SourceIsNarrowerThanTarget))
                .ToList();

            // check for existing comparisons
            List<DbElementTypeComparison> childTypeComparisons = _etcComparisonsBySourceElementKey[childEd.Key].ToList();

            // find valid mappings (identical, equivalent, or source is narrower)
            List<DbElementTypeComparison> validComparisons = childTypeComparisons
                .Where(etc =>
                    (etc.IsIdentical == true) ||
                    (etc.Relationship == CMR.Equivalent) ||
                    (etc.Relationship == CMR.SourceIsNarrowerThanTarget))
                .Where(etc => (etc.TargetTypeLiteral is not null) && targetTypeLiterals.Contains(etc.TargetTypeLiteral))
                .ToList();

            // if we have valid mappings or comparisons, use them
            if ((validTypeChildMappings.Count > 0) ||
                (validComparisons.Count > 0))
            {
                HashSet<int> targetElementKeys = [];

                foreach (DbElementMapping tcm in validTypeChildMappings)
                {
                    if (tcm.TargetElementKey.HasValue)
                    {
                        targetElementKeys.Add(tcm.TargetElementKey.Value);
                    }
                }

                foreach (DbElementTypeComparison vm in validComparisons)
                {
                    if (vm.TargetElementKey.HasValue)
                    {
                        targetElementKeys.Add(vm.TargetElementKey.Value);
                    }
                }

                result.MappedTypeChildren.Add(childEd);
                result.ChildMappings[childEd.Key] = new ChildMappingInfo
                {
                    ChildElement = childEd,
                    TypeComparisons = validComparisons,
                    TypeChildMappings = validTypeChildMappings,
                    TargetElementKeys = targetElementKeys
                };

                continue;
            }

            bool childElementIsMapped = false;
            List<DbElementType> sourceChildEts = _sourceElementTypesByElementKey[childEd.Key].ToList();

            // iterate over the types in this source child element to try and find type-based mappings
            foreach (DbElementType sourceChildElementType in sourceChildEts)
            {
                if (sourceChildElementType.TypeStructureKey is null)
                {
                    continue;
                }

                // resolve a source structure for current type
                DbStructureDefinition? sourceCETSd = DbStructureDefinition.SelectSingle(
                    _db,
                    FhirPackageKey: _packagePair.SourcePackageKey,
                    Key: sourceChildElementType.TypeStructureKey);

                if (sourceCETSd is null)
                {
                    throw new Exception(
                        $"Cannot find structure definition for source child element type `{sourceChildElementType.Literal}`" +
                        $" (key {sourceChildElementType.Key}) in package key {_packagePair.SourcePackageKey}");
                }

                // iterate over any potential matches
                foreach ((DbElement targetEd, DbElementType targetType) in targetTypes)
                {
                    if (targetType.TypeStructureKey is null)
                    {
                        continue;
                    }

                    // resolve a target structure for the target type
                    DbStructureDefinition? targetCETSd = DbStructureDefinition.SelectSingle(
                        _db,
                        FhirPackageKey: _packagePair.TargetPackageKey,
                        Key: targetType.TypeStructureKey);

                    if (targetCETSd is null)
                    {
                        throw new Exception(
                            $"Cannot find structure definition for target child element type `{targetType.Literal}`" +
                            $" (key {targetType.Key}) in package key {_packagePair.TargetPackageKey}");
                    }

                    // check for comparison between the structures
                    DbStructureComparison? cetComparison = DbStructureComparison.SelectSingle(
                        _db,
                        SourceFhirPackageKey: _packagePair.SourcePackageKey,
                        SourceStructureKey: sourceCETSd.Key,
                        TargetFhirPackageKey: _packagePair.TargetPackageKey,
                        TargetStructureKey: targetCETSd.Key);

                    if ((cetComparison is not null) &&
                        ((cetComparison.Relationship == CMR.Equivalent) || (cetComparison.Relationship == CMR.SourceIsNarrowerThanTarget)))
                    {
                        childElementIsMapped = true;
                        result.MappedTypeChildren.Add(childEd);
                        result.ChildMappings[childEd.Key] = new ChildMappingInfo
                        {
                            ChildElement = childEd,
                            TypeComparisons = validComparisons,
                            TypeChildMappings = validTypeChildMappings,
                            TargetElementKeys = [targetEd.Key],
                        };

                        continue;
                    }

                    // if there is no comparison, check to see if there is a mapping (primary first, then we'll check fallback)
                    DbStructureMapping? cetMapping = DbStructureMapping.SelectSingle(
                        _db,
                        IsFallback: false,
                        SourceFhirPackageKey: _packagePair.SourcePackageKey,
                        SourceStructureKey: sourceCETSd.Key,
                        TargetFhirPackageKey: _packagePair.TargetPackageKey,
                        TargetStructureKey: targetCETSd.Key);

                    cetMapping ??= DbStructureMapping.SelectSingle(
                        _db,
                        IsFallback: true,
                        SourceFhirPackageKey: _packagePair.SourcePackageKey,
                        SourceStructureKey: sourceCETSd.Key,
                        TargetFhirPackageKey: _packagePair.TargetPackageKey,
                        TargetStructureKey: targetCETSd.Key);

                    if ((cetMapping is not null) &&
                        ((cetMapping.Relationship == CMR.Equivalent) || (cetMapping.Relationship == CMR.SourceIsNarrowerThanTarget)))
                    {
                        childElementIsMapped = true;
                        result.MappedTypeChildren.Add(childEd);
                        result.ChildMappings[childEd.Key] = new ChildMappingInfo
                        {
                            ChildElement = childEd,
                            TypeComparisons = validComparisons,
                            TypeChildMappings = validTypeChildMappings,
                            TargetElementKeys = [targetEd.Key],
                        };

                        continue;
                    }
                }
            }

            if (childElementIsMapped)
            {
                result.MappedTypeChildren.Add(childEd);
            }
            else
            {
                result.UnmappedTypeChildren.Add(childEd);
                continue;
            }
        }

        // All children must be mapped for distributed mapping to work
        result.AllChildrenMapped = (result.UnmappedTypeChildren.Count == 0);

        return result;
    }

    /// <summary>
    /// Processes the results of child mapping checks and updates tracking accordingly.
    /// </summary>
    /// <param name="unmappedTypes">Dictionary of unmapped types (will be modified)</param>
    /// <param name="childMappingResults">Results from checking child mappings</param>
    /// <param name="elementTrackingRec">The tracking record to update with messages</param>
    /// <param name="sourceElement">The source element being processed</param>
    /// <returns>Set of unmapped type keys that were successfully handled via children</returns>
    private HashSet<int> processChildMappingResults(
        Dictionary<int, DbElementType> unmappedTypes,
        List<ChildTypeMappingResult> childMappingResults,
        ElementOutcomeTrackingRecord elementTrackingRec,
        DbElement sourceElement)
    {
        HashSet<int> resolvedTypeKeys = [];

        foreach (ChildTypeMappingResult cmr in childMappingResults)
        {
            if (!cmr.AllChildrenMapped)
            {
                // still have unmapped
                if (cmr.UnmappedTypeChildren.Count > 0)
                {
                    elementTrackingRec.Messages.Add(
                        $"Element `{sourceElement.Id}` has unmapped type `{cmr.SourceParentType.Literal}` -" +
                        $" cannot distribute via children because the following children lack mappings:" +
                        $" {string.Join(", ", cmr.UnmappedTypeChildren.Select(uc => $"`{uc.Id}`"))}");
                }
                continue;
            }

            // all children are mapped - this is a successful distributed mapping for this type build a nice message
            List<string> childMappingDescriptions = cmr.ChildMappings.Values
                .Select(cmi =>
                {
                    // get target element names
                    List<string> targetNames = cmi.TargetElementKeys
                        .Select(tek => _allTargetElements.TryGetValue(tek, out DbElement? te)
                            ? te.Id
                            : $"Unknown({tek})")
                        .ToList();

                    return $"`{cmi.ChildElement.Name}` -> {string.Join(", ", targetNames.Select(tn => $"`{tn}`"))}";
                })
                .ToList();

            elementTrackingRec.Messages.Add(
                $"Element `{sourceElement.Id}` has unmapped source type `{cmr.SourceParentType.Literal}`" +
                $" that fully maps when distributed across target elements via child mappings:" +
                $" {string.Join("; ", childMappingDescriptions)}");

            // remove from unmapped types using the CORRECT KEY (parent type key, not child type key)
            if (unmappedTypes.Remove(cmr.SourceParentType.Key))
            {
                resolvedTypeKeys.Add(cmr.SourceParentType.Key);
            }
        }

        return resolvedTypeKeys;
    }

}
