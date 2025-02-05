// <copyright file="FhirCoreComparer.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Hl7.Fhir.Model;
using Hl7.Fhir.Utility;
using Microsoft.Extensions.Logging;
using Microsoft.Health.Fhir.Comparison.CompareTool;
using Microsoft.Health.Fhir.CodeGen.Configuration;
using Microsoft.Health.Fhir.CodeGen.FhirExtensions;
using Microsoft.Health.Fhir.CodeGen.Language;
using Microsoft.Health.Fhir.CodeGen.Models;
using Microsoft.Health.Fhir.CodeGenCommon.Extensions;
using Microsoft.Health.Fhir.CodeGenCommon.Packaging;
using Microsoft.Health.Fhir.CodeGenCommon.Utils;
using CMR = Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship;
using Microsoft.Health.Fhir.CodeGenCommon.FhirExtensions;
using System.Text.RegularExpressions;
using System.Collections;


namespace Microsoft.Health.Fhir.Comparison.CompareTool;


internal static partial class FhirCoreComparerLogMessages
{
    [LoggerMessage(Level = LogLevel.Information, Message = "Comparing {leftKey} and {rightKey}.")]
    internal static partial void LogComparisonStart(this ILogger logger, string leftKey, string rightKey);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Failed to find maps for {cvMapKey}! Processing will be only algorithmic!")]
    internal static partial void LogMapsNotFound(this ILogger logger, string cvMapKey);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Failed to load maps for {aKey} and {bKey}, processing will be only algorithmic!")]
    internal static partial void LogMapsNotLoaded(this ILogger logger, string aKey, string bKey);

    [LoggerMessage(Level = LogLevel.Warning, Message = "ValueSet {url} not compared because it is in the manual exclusion list.")]
    internal static partial void LogValueSetExcluded(this ILogger logger, string url);

    [LoggerMessage(Level = LogLevel.Warning, Message = "{url} not compared because it has no maps and does not exist in the target.")]
    internal static partial void LogNoTarget(this ILogger logger, string url);

    [LoggerMessage(Level = LogLevel.Warning, Message = "{url} not compared because the map {mapUrl} does not have a valid target.")]
    internal static partial void LogInvalidMapTarget(this ILogger logger, string url, string mapUrl);

    [LoggerMessage(Level = LogLevel.Warning, Message = "{url} requested but not compared because the map target {mapUrl} did not resolve.")]
    internal static partial void LogMapTargetNotFound(this ILogger logger, string url, string mapUrl);

    [LoggerMessage(Level = LogLevel.Warning, Message = "ValueSet {url} not compared because this ValueSet has no discovered required bindings.")]
    internal static partial void LogValueSetNoRequiredBindings(this ILogger logger, string url);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Failed to expand ValueSet {url} for comparison: {details}")]
    internal static partial void LogValueSetNotExpanded(this ILogger logger, string url, string? details);

}

public partial class FhirCoreComparer
{
    internal static readonly HashSet<string> _exclusionSet = [
        /* UCUM is used as a required binding in a codeable concept. Since we do not
         * use enums in this situation, it is not useful to generate this valueset
         */
        "http://hl7.org/fhir/ValueSet/ucum-units",

        /* R5 made Resource.language a required binding to all-languages, which contains
         * all of bcp:47 and is listed as infinite. This is not useful to generate.
         * Note that in R5, many elements that are required to all-languages also have bound
         * starter value sets.  TODO: consider if we want to generate constants for those.
         */
        "http://hl7.org/fhir/ValueSet/all-languages",

        /* MIME types are infinite, so we do not want to generate these.
         * Note that in R5, many elements that are required to MIME type also have bound
         * starter value sets.  TODO: consider if we want to generate constants for those.
         */
        "http://hl7.org/fhir/ValueSet/mimetypes",

        /* ISO 3166 is large and has not changed since used in this context - do not map it
         * This should not need to be in the list - it is a system not a value set
         */
        //"urn:iso:std:iso:3166",
        //"urn:iso:std:iso:3166:-2",
    ];



    private ILoggerFactory _loggerFactory;
    private ILogger _logger;
    private string _mapSourcePath;
    private string _dbPath;

    private DefinitionCollection _leftDc;
    private string _leftShortVersion;
    private string _leftRLiteral;
    private string _leftKey;

    private DefinitionCollection _rightDc;
    private string _rightShortVersion;
    private string _rightRLiteral;
    private string _rightKey;

    private DifferenceTracker? _diffsLeftToRight = null;
    private DifferenceTracker? _diffsRightToLeft = null;

    private CrossVersionMapCollection? _cvLeftToRight = null;
    private CrossVersionMapCollection? _cvRightToLeft = null;

    private Dictionary<string, List<ValueSetPairComparison>> _valueSetComparisons = [];

    private Dictionary<string, List<PairComparison<StructureDefinition>>> _primitiveComparisons = [];

    public FhirCoreComparer(
        DefinitionCollection left,
        DefinitionCollection right,
        ILoggerFactory loggerFactory,
        string mapSourcePath)
    {
        _loggerFactory = loggerFactory;
        _logger = loggerFactory.CreateLogger<FhirCoreComparer>();
        _mapSourcePath = mapSourcePath;
        _dbPath = Path.Combine(mapSourcePath, "db");

        _leftDc = left;
        _leftShortVersion = left.FhirSequence.ToShortVersion();
        _leftRLiteral = left.FhirSequence.ToRLiteral();
        _leftKey = left.Key;

        _rightDc = right;
        _rightShortVersion = right.FhirSequence.ToShortVersion();
        _rightRLiteral = right.FhirSequence.ToRLiteral();
        _rightKey = right.Key;
    }

    public DefinitionCollection LeftDC => _leftDc;
    public DefinitionCollection RightDC => _rightDc;

    public CrossVersionMapCollection? LeftToRight => _cvLeftToRight;
    public CrossVersionMapCollection? RightToLeft => _cvRightToLeft;

    public void Compare(CodeGenCommon.Models.FhirArtifactClassEnum? artifactFilter = null)
    {
        _logger.LogComparisonStart(_leftKey, _rightKey);

        if ((_cvLeftToRight == null) ||
            (_cvRightToLeft == null))
        {
            // load cross-version maps in both directions
            (_cvLeftToRight, _cvRightToLeft) = getInitialMaps();
        }

        switch (artifactFilter)
        {
            case CodeGenCommon.Models.FhirArtifactClassEnum.ValueSet:
                compareAllValueSets();
                break;

            // types are grouped together in maps, so always update both
            case CodeGenCommon.Models.FhirArtifactClassEnum.PrimitiveType:
            case CodeGenCommon.Models.FhirArtifactClassEnum.ComplexType:
                compareAllPrimitiveTypes();
                compareAllComplexTypes();
                break;

            // resources need all 'lower' types updated
            case CodeGenCommon.Models.FhirArtifactClassEnum.Resource:
            default:
                compareAllValueSets();
                compareAllPrimitiveTypes();
                compareAllComplexTypes();
                checkOverviewMaps(CodeGenCommon.Models.FhirArtifactClassEnum.Resource);
                break;
        }
    }

    public void Init(string cvMapSourcePath)
    {
        _diffsLeftToRight = new(_leftDc, _rightDc, _dbPath);
        _diffsLeftToRight.InitDb(_exclusionSet, _escapeValveCodes, out bool createdLR);

        _diffsRightToLeft = new(_rightDc, _leftDc, _dbPath);
        _diffsRightToLeft.InitDb(_exclusionSet, _escapeValveCodes, out bool createdRL);

        if (createdLR || createdRL)
        {
            (CrossVersionMapCollection cvLR, CrossVersionMapCollection cvRL) = getInitialMaps(true);

            if (createdLR && !string.IsNullOrEmpty(cvMapSourcePath))
            {
                _diffsLeftToRight.LoadFromCrossVersionMaps(cvLR);
            }

            if (createdRL && !string.IsNullOrEmpty(cvMapSourcePath))
            {
                _diffsRightToLeft.LoadFromCrossVersionMaps(cvRL);
            }
        }
    }

    /// <summary>
    /// Gets the initial cross-version maps between two definition collections.
    /// </summary>
    /// <param name="preferV1Maps"></param>
    /// <returns></returns>
    public (CrossVersionMapCollection leftToRight, CrossVersionMapCollection rightToLeft) GetInitialCrossVersionMaps(bool preferV1Maps)
    {
        if ((_cvLeftToRight == null) ||
            (_cvRightToLeft == null))
        {
            // load cross-version maps in both directions
            (_cvLeftToRight, _cvRightToLeft) = getInitialMaps(preferV1Maps);
        }

        return (_cvLeftToRight, _cvRightToLeft);
    }


    public void Save(CodeGenCommon.Models.FhirArtifactClassEnum? artifactFilter = null)
    {
        if ((artifactFilter == null) ||
            (artifactFilter == CodeGenCommon.Models.FhirArtifactClassEnum.ValueSet) ||
            (artifactFilter == CodeGenCommon.Models.FhirArtifactClassEnum.Resource))
        {
            saveValueSetMaps();
        }

        if ((artifactFilter == null) ||
            (artifactFilter == CodeGenCommon.Models.FhirArtifactClassEnum.PrimitiveType) ||
            (artifactFilter == CodeGenCommon.Models.FhirArtifactClassEnum.ComplexType))
        {
            saveTypeMaps();
        }
    }

    private void saveValueSetMaps()
    {
        string dir = Path.Combine(_mapSourcePath, "input", "codes_v2");

        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        _cvLeftToRight?.SaveValueSetConceptMaps(dir);
        _cvRightToLeft?.SaveValueSetConceptMaps(dir);
    }

    private void saveTypeMaps()
    {
        string dir = Path.Combine(_mapSourcePath, "input", "types_v2");

        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        _cvLeftToRight?.SaveDataTypeConceptMaps(dir);
        _cvRightToLeft?.SaveDataTypeConceptMaps(dir);
    }

    private void setUseContext(ConceptMap cm, string ctxType)
    {
        if (cm.UseContext.Any(uc => uc.Code.System == CommonDefinitions.ConceptMapUsageContextSystem && uc.Code.Code == ctxType))
        {
            return;
        }

        cm.UseContext.Add(new()
        {
            Code = new(CommonDefinitions.ConceptMapUsageContextSystem, CommonDefinitions.ConceptMapUsageContextTarget),
            Value = new CodeableConcept(CommonDefinitions.ConceptMapUsageContextSystem, ctxType),
        });
    }

    private List<ConceptMap.MappingPropertyComponent> getInvertedProperties() =>
        [
            new()
            {
                Code = CommonDefinitions.ConceptMapPropertyGenerated,
                Value = new FhirBoolean(true),
            },
            new()
            {
                Code = CommonDefinitions.ConceptMapPropertyNeedsReview,
                Value = new FhirBoolean(true),
            },
        ];

    private List<ConceptMap.MappingPropertyComponent> getMappingProperties(ValueSetCodeComparisonRec rec)
    {
        List<ConceptMap.MappingPropertyComponent> properties = [];

        if (rec.IsGenerated != null)
        {
            properties.Add(new()
            {
                Code = CommonDefinitions.ConceptMapPropertyGenerated,
                Value = new FhirBoolean(rec.IsGenerated),
            });
        }

        if (rec.NeedsReview != null)
        {
            properties.Add(new()
            {
                Code = CommonDefinitions.ConceptMapPropertyNeedsReview,
                Value = new FhirBoolean(rec.NeedsReview),
            });
        }

        return properties;

    }

    private List<ConceptMap.MappingPropertyComponent> getMappingProperties(
        bool? generated = null,
        bool? needsReview = null,
        CMR? conceptRelationship = null,
        CMR? valueRelationship = null)
    {
        List<ConceptMap.MappingPropertyComponent> properties = [];

        if (generated != null)
        {
            properties.Add(new()
            {
                Code = CommonDefinitions.ConceptMapPropertyGenerated,
                Value = new FhirBoolean(generated),
            });
        }

        if (needsReview != null)
        {
            properties.Add(new()
            {
                Code = CommonDefinitions.ConceptMapPropertyNeedsReview,
                Value = new FhirBoolean(needsReview),
            });
        }

        if (conceptRelationship != null)
        {
            properties.Add(new()
            {
                Code = CommonDefinitions.ConceptMapPropertyConceptDomainRelationship,
                Value = new Code<ConceptMap.ConceptMapRelationship>(conceptRelationship),
            });
        }

        if (valueRelationship != null)
        {
            properties.Add(new()
            {
                Code = CommonDefinitions.ConceptMapPropertyValueDomainRelationship,
                Value = new Code<ConceptMap.ConceptMapRelationship>(valueRelationship),
            });
        }

        return properties;
    }


    private CMR? invert(CMR? existing) => existing switch
    {
        CMR.RelatedTo => CMR.RelatedTo,
        CMR.Equivalent => CMR.Equivalent,
        CMR.SourceIsNarrowerThanTarget => CMR.SourceIsBroaderThanTarget,
        CMR.SourceIsBroaderThanTarget => CMR.SourceIsNarrowerThanTarget,
        CMR.NotRelatedTo => CMR.NotRelatedTo,
        _ => null,
    };


    private void addConceptMapPropertyDefinitions(ConceptMap cm, bool includeDomainProps = false)
    {
        List<ConceptMap.PropertyComponent> properties =
        [
            new()
            {
                Uri = CommonDefinitions.ConceptMapPropertiesSystem + "/" + CommonDefinitions.ConceptMapPropertyGenerated,
                Code = CommonDefinitions.ConceptMapPropertyGenerated,
                Description = "Generated by the FHIR Cross-Version Mapping Tool",
                Type = ConceptMap.ConceptMapPropertyType.Boolean,
            },
            new()
            {
                Uri = CommonDefinitions.ConceptMapPropertiesSystem + "/" + CommonDefinitions.ConceptMapPropertyNeedsReview,
                Code = CommonDefinitions.ConceptMapPropertyNeedsReview,
                Description = "This mapping needs review",
                Type = ConceptMap.ConceptMapPropertyType.Boolean,
            },
        ];

        if (includeDomainProps)
        {
            properties.Add(new()
            {
                Uri = CommonDefinitions.ConceptMapPropertiesSystem + "/" + CommonDefinitions.ConceptMapPropertyConceptDomainRelationship,
                Code = CommonDefinitions.ConceptMapPropertyConceptDomainRelationship,
                Description = "Explicit tracking of the concept domain for this mapping",
                Type = ConceptMap.ConceptMapPropertyType.Code,
                System = "http://hl7.org/fhir/concept-map-relationship",
            });
            properties.Add(new()
            {
                Uri = CommonDefinitions.ConceptMapPropertiesSystem + "/" + CommonDefinitions.ConceptMapPropertyValueDomainRelationship,
                Code = CommonDefinitions.ConceptMapPropertyValueDomainRelationship,
                Description = "Explicit tracking of the value domain for this mapping",
                Type = ConceptMap.ConceptMapPropertyType.Code,
                System = "http://hl7.org/fhir/concept-map-relationship",
            });
        }

        foreach (ConceptMap.PropertyComponent prop in properties)
        {
            if (!cm.Property.Any(p => p.Uri == prop.Uri && p.Code == prop.Code))
            {
                cm.Property.Add(prop);
            }
        }
    }


    /// <summary>
    /// Aggregates the relationships of a given ConceptMap by iterating over its groups and elements.
    /// </summary>
    /// <param name="cm">The ConceptMap to aggregate relationships for.</param>
    /// <returns>The aggregated ConceptMapRelationship for the entire ConceptMap.</returns>
    /// <remarks>
    /// This method starts with an optimistic assumption that the relationship is equivalent.
    /// It iterates over each group and each element within the group to apply the relationships.
    /// The aggregated relationship is stored as an extension in both the group and the ConceptMap.
    /// </remarks>
    private CMR aggregateValueSetRelationships(ConceptMap cm)
    {
        // start optimistic
        CMR vsRelationship = CMR.Equivalent;

        // iterate over groups
        foreach (ConceptMap.GroupComponent group in cm.Group)
        {
            // start optimistic
            CMR groupRelationship = CMR.Equivalent;

            // iterate over the elements (individual concept maps)
            foreach (ConceptMap.SourceElementComponent sourceElement in group.Element)
            {
                // check for no map
                if (sourceElement.NoMap == true)
                {
                    // unmapped element means the group is broader than the target
                    groupRelationship = applyRelationship(groupRelationship, CMR.SourceIsBroaderThanTarget);
                }

                // iterate over the targets
                foreach (ConceptMap.TargetElementComponent targetElement in sourceElement.Target)
                {
                    // apply the current relationship
                    groupRelationship = applyRelationship(groupRelationship, targetElement.Relationship);
                }
            }

            // add an extension to the group to store the relationship
            group.SetExtension(CommonDefinitions.ExtUrlConceptMapAggregateRelationship, new Code<ConceptMap.ConceptMapRelationship>(groupRelationship));

            // apply the group relationship to the value set relationship
            vsRelationship = applyRelationship(vsRelationship, groupRelationship);
        }

        // add an extension to the concept map to store the relationship
        cm.SetExtension(CommonDefinitions.ExtUrlConceptMapAggregateRelationship, new Code<ConceptMap.ConceptMapRelationship>(vsRelationship));

        // return the relationship
        return vsRelationship;
    }

    private CMR applyRelationship(CMR? existing, CMR? change) => existing switch
    {
        CMR.Equivalent => change ?? CMR.Equivalent,
        CMR.RelatedTo => (change == CMR.NotRelatedTo) ? CMR.NotRelatedTo : CMR.RelatedTo,
        CMR.SourceIsNarrowerThanTarget => (change == CMR.SourceIsNarrowerThanTarget || change == CMR.Equivalent)
            ? CMR.SourceIsNarrowerThanTarget : CMR.RelatedTo,
        CMR.SourceIsBroaderThanTarget => (change == CMR.SourceIsBroaderThanTarget || change == CMR.Equivalent)
            ? CMR.SourceIsBroaderThanTarget : CMR.RelatedTo,
        CMR.NotRelatedTo => change ?? CMR.NotRelatedTo,
        _ => change ?? existing ?? CMR.NotRelatedTo,
    };

    /// <summary>
    /// Gets the initial cross-version maps between two definition collections.
    /// </summary>
    /// <returns>A collection of cross-version maps.</returns>
    private (CrossVersionMapCollection lToR, CrossVersionMapCollection rToL) getInitialMaps(bool preferV1Maps = false)
    {
        CrossVersionMapCollection lToR = new(_leftDc, _rightDc, _dbPath, _loggerFactory);
        CrossVersionMapCollection rToL = new(_rightDc, _leftDc, _dbPath, _loggerFactory);

        // check for creating new maps
        if (string.IsNullOrEmpty(_mapSourcePath))
        {
            return (lToR, rToL);
        }

        if (!lToR.TryLoadCrossVersionMaps(_mapSourcePath, preferV1Maps))
        {
            _logger.LogMapsNotLoaded(_leftKey, _rightKey);
        }

        if (!rToL.TryLoadCrossVersionMaps(_mapSourcePath, preferV1Maps))
        {
            _logger.LogMapsNotLoaded(_rightKey, _leftKey);
        }

        return (lToR, rToL);
    }

}
