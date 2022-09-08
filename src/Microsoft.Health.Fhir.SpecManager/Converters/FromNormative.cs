// <copyright file="FromCorePackage.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System.Dynamic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Xml.Linq;
using Microsoft.Health.Fhir.SpecManager.Manager;
using Microsoft.Health.Fhir.SpecManager.Models;

namespace Microsoft.Health.Fhir.SpecManager.Converters;

/// <summary>Load models from a core package. This class cannot be inherited.</summary>
public sealed class FromNormative : IFhirConverter
{
    private const string ExtensionComment = "There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.";
    private const string ExtensionDefinition = "May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.";
    private const string ExtensionShort = "Additional content defined by implementations";

    /// <summary>The errors.</summary>
    private static List<string> _errors;

    /// <summary>The warnings.</summary>
    private static List<string> _warnings;

    /// <summary>Values that represent read type codes.</summary>
    private enum ReadTypeCodes
    {
        ByteArray,
        Boolean,
        Decimal,
        String,
        StringArray,
        Integer,
        Long,
        Nested,
        NestedArray,
    }

    /// <summary>Information about the element choice.</summary>
    /// <param name="Literal"> The literal.</param>
    /// <param name="ReadType">Type of the read.</param>
    /// <param name="NestKey"> The nest key.</param>
    private record struct ElementChoiceInfo(
        string Literal,
        ReadTypeCodes ReadType,
        string NestKey);

    /// <summary>(Immutable) The open type choices.</summary>
    private static readonly ElementChoiceInfo[] _openTypeChoices = new[]
    {
        // Primitive Types
        new ElementChoiceInfo("Base64Binary", ReadTypeCodes.ByteArray, string.Empty),
        new ElementChoiceInfo("Boolean", ReadTypeCodes.Boolean, string.Empty),
        new ElementChoiceInfo("Canonical", ReadTypeCodes.String, string.Empty),
        new ElementChoiceInfo("Code", ReadTypeCodes.String, string.Empty),
        new ElementChoiceInfo("Date", ReadTypeCodes.String, string.Empty),
        new ElementChoiceInfo("DateTime", ReadTypeCodes.String, string.Empty),
        new ElementChoiceInfo("Decimal", ReadTypeCodes.Decimal, string.Empty),
        new ElementChoiceInfo("Id", ReadTypeCodes.String, string.Empty),
        new ElementChoiceInfo("Instant", ReadTypeCodes.String, string.Empty),
        new ElementChoiceInfo("Integer", ReadTypeCodes.Integer, string.Empty),
        new ElementChoiceInfo("Integer64", ReadTypeCodes.Long, string.Empty),
        new ElementChoiceInfo("Markdown", ReadTypeCodes.String, string.Empty),
        new ElementChoiceInfo("Oid", ReadTypeCodes.String, string.Empty),
        new ElementChoiceInfo("PositiveInt", ReadTypeCodes.Integer, string.Empty),
        new ElementChoiceInfo("String", ReadTypeCodes.String, string.Empty),
        new ElementChoiceInfo("Time", ReadTypeCodes.String, string.Empty),
        new ElementChoiceInfo("UnsignedInt", ReadTypeCodes.Integer, string.Empty),
        new ElementChoiceInfo("Uri", ReadTypeCodes.String, string.Empty),
        new ElementChoiceInfo("Url", ReadTypeCodes.String, string.Empty),
        new ElementChoiceInfo("Uuid", ReadTypeCodes.String, string.Empty),

        // Datatypes (complex types)
        new ElementChoiceInfo("Address", ReadTypeCodes.Nested, "Address"),
        new ElementChoiceInfo("Age", ReadTypeCodes.Nested, "Quantity"),
        new ElementChoiceInfo("Annotation", ReadTypeCodes.Nested, "Annotation"),
        new ElementChoiceInfo("Attachment", ReadTypeCodes.Nested, "Attachment"),
        new ElementChoiceInfo("CodeableConcept", ReadTypeCodes.Nested, "CodeableConcept"),
        new ElementChoiceInfo("CodeableReference", ReadTypeCodes.Nested, "CodeableReference"),
        new ElementChoiceInfo("Coding", ReadTypeCodes.Nested, "Coding"),
        new ElementChoiceInfo("ContactPoint", ReadTypeCodes.Nested, "ContactPoint"),
        new ElementChoiceInfo("Count", ReadTypeCodes.Nested, "Quantity"),
        new ElementChoiceInfo("Distance", ReadTypeCodes.Nested, "Quantity"),
        new ElementChoiceInfo("Duration", ReadTypeCodes.Nested, "Quantity"),
        new ElementChoiceInfo("HumanName", ReadTypeCodes.Nested, "HumanName"),
        new ElementChoiceInfo("Identifier", ReadTypeCodes.Nested, "Identifier"),
        new ElementChoiceInfo("Money", ReadTypeCodes.Nested, "Money"),
        new ElementChoiceInfo("Period", ReadTypeCodes.Nested, "Period"),
        new ElementChoiceInfo("Quantity", ReadTypeCodes.Nested, "Quantity"),
        new ElementChoiceInfo("Range", ReadTypeCodes.Nested, "Range"),
        new ElementChoiceInfo("Ratio", ReadTypeCodes.Nested, "Ratio"),
        new ElementChoiceInfo("RatioRange", ReadTypeCodes.Nested, "RatioRange"),
        new ElementChoiceInfo("Reference", ReadTypeCodes.Nested, "Reference"),
        new ElementChoiceInfo("SampledData", ReadTypeCodes.Nested, "SampledData"),
        new ElementChoiceInfo("SimpleQuantity", ReadTypeCodes.Nested, "Qantity"),
        new ElementChoiceInfo("Signature", ReadTypeCodes.Nested, "Signature"),
        new ElementChoiceInfo("Timing", ReadTypeCodes.Nested, "Timing"),

        // MetaData Types
        new ElementChoiceInfo("ContactDetail", ReadTypeCodes.Nested, "ContactDetail"),
        new ElementChoiceInfo("DataRequirement", ReadTypeCodes.Nested, "DataRequirement"),
        new ElementChoiceInfo("Expression", ReadTypeCodes.Nested, "Expression"),
        new ElementChoiceInfo("ParameterDefinition", ReadTypeCodes.Nested, "ParameterDefinition"),
        new ElementChoiceInfo("RelatedArtifact", ReadTypeCodes.Nested, "RelatedArtifact"),
        new ElementChoiceInfo("TriggerDefinition", ReadTypeCodes.Nested, "TriggerDefinition"),
        new ElementChoiceInfo("UsageContext", ReadTypeCodes.Nested, "UsageContext"),
        new ElementChoiceInfo("Availability", ReadTypeCodes.Nested, "Availability"),
        new ElementChoiceInfo("ExtendedContactDetail", ReadTypeCodes.Nested, "ExtendedContactDetail"),

        // Special Types
        new ElementChoiceInfo("Dosage", ReadTypeCodes.Nested, "Dosage"),
        new ElementChoiceInfo("Meta", ReadTypeCodes.Nested, "Meta"),
    };

    /// <summary>(Immutable) The nested element choices.</summary>
    private static readonly Dictionary<string, ElementChoiceInfo[]> _nestedElementChoices = new()
    {
        {
            "Address",
            new[]
            {
                new ElementChoiceInfo("use", ReadTypeCodes.String, string.Empty),
                new ElementChoiceInfo("type", ReadTypeCodes.String, string.Empty),
                new ElementChoiceInfo("text", ReadTypeCodes.String, string.Empty),
                new ElementChoiceInfo("line", ReadTypeCodes.StringArray, string.Empty),
                new ElementChoiceInfo("city", ReadTypeCodes.String, string.Empty),
                new ElementChoiceInfo("district", ReadTypeCodes.String, string.Empty),
                new ElementChoiceInfo("state", ReadTypeCodes.String, string.Empty),
                new ElementChoiceInfo("postalCode", ReadTypeCodes.String, string.Empty),
                new ElementChoiceInfo("country", ReadTypeCodes.String, string.Empty),
                new ElementChoiceInfo("period", ReadTypeCodes.Nested, "Period"),
            }
        },
        {
            "Annotation",
            new[]
            {
                new ElementChoiceInfo("authorReference", ReadTypeCodes.NestedArray, "Reference"),
                new ElementChoiceInfo("authorString", ReadTypeCodes.StringArray, string.Empty),
                new ElementChoiceInfo("time", ReadTypeCodes.String, string.Empty),
                new ElementChoiceInfo("text", ReadTypeCodes.String, string.Empty),
            }
        },
        {
            "Attachment",
            new[]
            {
                new ElementChoiceInfo("contentType", ReadTypeCodes.String, string.Empty),
                new ElementChoiceInfo("language", ReadTypeCodes.String, string.Empty),
                new ElementChoiceInfo("data", ReadTypeCodes.ByteArray, string.Empty),
                new ElementChoiceInfo("url", ReadTypeCodes.String, string.Empty),
                new ElementChoiceInfo("size", ReadTypeCodes.Long, string.Empty),
                new ElementChoiceInfo("hash", ReadTypeCodes.ByteArray, string.Empty),
                new ElementChoiceInfo("title", ReadTypeCodes.String, string.Empty),
                new ElementChoiceInfo("creation", ReadTypeCodes.String, string.Empty),
                new ElementChoiceInfo("height", ReadTypeCodes.Integer, string.Empty),
                new ElementChoiceInfo("width", ReadTypeCodes.Integer, string.Empty),
                new ElementChoiceInfo("frames", ReadTypeCodes.Integer, string.Empty),
                new ElementChoiceInfo("duration", ReadTypeCodes.Decimal, string.Empty),
                new ElementChoiceInfo("pages", ReadTypeCodes.Integer, string.Empty),
            }
        },
        {
            "CodeableConcept",
            new[]
            {
                new ElementChoiceInfo("coding", ReadTypeCodes.NestedArray, "Coding"),
                new ElementChoiceInfo("text", ReadTypeCodes.String, string.Empty),
            }
        },
        {
            "CodeableReference",
            new[]
            {
                new ElementChoiceInfo("concept", ReadTypeCodes.Nested, "CodeableConcept"),
                new ElementChoiceInfo("reference", ReadTypeCodes.Nested, "Reference"),
            }
        },
        {
            "Coding",
            new[]
            {
                new ElementChoiceInfo("system", ReadTypeCodes.String, string.Empty),
                new ElementChoiceInfo("version", ReadTypeCodes.String, string.Empty),
                new ElementChoiceInfo("code", ReadTypeCodes.String, string.Empty),
                new ElementChoiceInfo("display", ReadTypeCodes.String, string.Empty),
                new ElementChoiceInfo("userSelected", ReadTypeCodes.Boolean, string.Empty),
            }
        },
        {
            "ContactDetail",
            new[]
            {
                new ElementChoiceInfo("name", ReadTypeCodes.String, string.Empty),
                new ElementChoiceInfo("telecom", ReadTypeCodes.NestedArray, "ContactPoint"),
            }
        },
        {
            "ContactPoint",
            new[]
            {
                new ElementChoiceInfo("system", ReadTypeCodes.String, string.Empty),
                new ElementChoiceInfo("value", ReadTypeCodes.String, string.Empty),
                new ElementChoiceInfo("use", ReadTypeCodes.String, string.Empty),
                new ElementChoiceInfo("rank", ReadTypeCodes.Integer, string.Empty),
                new ElementChoiceInfo("period", ReadTypeCodes.Nested, "Period"),
            }
        },
        {
            "Expression",
            new[]
            {
                new ElementChoiceInfo("description", ReadTypeCodes.String, string.Empty),
                new ElementChoiceInfo("name", ReadTypeCodes.String, string.Empty),
                new ElementChoiceInfo("language", ReadTypeCodes.String, string.Empty),
                new ElementChoiceInfo("expression", ReadTypeCodes.String, string.Empty),
                new ElementChoiceInfo("reference", ReadTypeCodes.String, string.Empty),
            }
        },
        {
            "HumanName",
            new[]
            {
                new ElementChoiceInfo("use", ReadTypeCodes.String, string.Empty),
                new ElementChoiceInfo("text", ReadTypeCodes.String, string.Empty),
                new ElementChoiceInfo("family", ReadTypeCodes.String, string.Empty),
                new ElementChoiceInfo("given", ReadTypeCodes.StringArray, string.Empty),
                new ElementChoiceInfo("prefix", ReadTypeCodes.StringArray, string.Empty),
                new ElementChoiceInfo("suffix", ReadTypeCodes.StringArray, string.Empty),
                new ElementChoiceInfo("period", ReadTypeCodes.Nested, "Period"),
            }
        },
        {
            "Identifier",
            new[]
            {
                new ElementChoiceInfo("use", ReadTypeCodes.String, string.Empty),
                new ElementChoiceInfo("type", ReadTypeCodes.Nested, "CodeableConcept"),
                new ElementChoiceInfo("system", ReadTypeCodes.String, string.Empty),
                new ElementChoiceInfo("value", ReadTypeCodes.String, string.Empty),
                new ElementChoiceInfo("period", ReadTypeCodes.Nested, "Period"),
                new ElementChoiceInfo("assigner", ReadTypeCodes.Nested, "Reference"),
            }
        },
        {
            "Money",
            new[]
            {
                new ElementChoiceInfo("value", ReadTypeCodes.Decimal, string.Empty),
                new ElementChoiceInfo("currency", ReadTypeCodes.String, string.Empty),
            }
        },
        {
            "Period",
            new[]
            {
                new ElementChoiceInfo("start", ReadTypeCodes.String, string.Empty),
                new ElementChoiceInfo("end", ReadTypeCodes.String, string.Empty),
            }
        },
        {
            "Quantity",
            new[]
            {
                new ElementChoiceInfo("value", ReadTypeCodes.Decimal, string.Empty),
                new ElementChoiceInfo("comparator", ReadTypeCodes.String, string.Empty),
                new ElementChoiceInfo("unit", ReadTypeCodes.String, string.Empty),
                new ElementChoiceInfo("system", ReadTypeCodes.String, string.Empty),
                new ElementChoiceInfo("code", ReadTypeCodes.String, string.Empty),
            }
        },
        {
            "Range",
            new[]
            {
                new ElementChoiceInfo("low", ReadTypeCodes.Nested, "Quantity"),
                new ElementChoiceInfo("high", ReadTypeCodes.Nested, "Quantity"),
            }
        },
        {
            "Ratio",
            new[]
            {
                new ElementChoiceInfo("numerator", ReadTypeCodes.Nested, "Quantity"),
                new ElementChoiceInfo("denominator", ReadTypeCodes.Nested, "Quantity"),
            }
        },
        {
            "RatioRange",
            new[]
            {
                new ElementChoiceInfo("lowNumerator", ReadTypeCodes.Nested, "Quantity"),
                new ElementChoiceInfo("highNumerator", ReadTypeCodes.Nested, "Quantity"),
                new ElementChoiceInfo("denominator", ReadTypeCodes.Nested, "Quantity"),
            }
        },
        {
            "Reference",
            new[]
            {
                new ElementChoiceInfo("reference", ReadTypeCodes.String, string.Empty),
                new ElementChoiceInfo("type", ReadTypeCodes.String, string.Empty),
                new ElementChoiceInfo("identifier", ReadTypeCodes.Nested, "Identifier"),
                new ElementChoiceInfo("display", ReadTypeCodes.String, string.Empty),
            }
        },
        {
            "SampledData",
            new[]
            {
                new ElementChoiceInfo("origin", ReadTypeCodes.Nested, "Quantity"),
                new ElementChoiceInfo("interval", ReadTypeCodes.Decimal, string.Empty),
                new ElementChoiceInfo("intervalUnit", ReadTypeCodes.String, string.Empty),
                new ElementChoiceInfo("factor", ReadTypeCodes.Decimal, string.Empty),
                new ElementChoiceInfo("lowerLimit", ReadTypeCodes.Decimal, string.Empty),
                new ElementChoiceInfo("upperLimit", ReadTypeCodes.Decimal, string.Empty),
                new ElementChoiceInfo("dimensions", ReadTypeCodes.Integer, string.Empty),
                new ElementChoiceInfo("data", ReadTypeCodes.String, string.Empty),
            }
        },
        {
            "Signature",
            new[]
            {
                new ElementChoiceInfo("type", ReadTypeCodes.NestedArray, "Coding"),
                new ElementChoiceInfo("when", ReadTypeCodes.String, string.Empty),
                new ElementChoiceInfo("who", ReadTypeCodes.Nested, "Reference"),
                new ElementChoiceInfo("onBehalfOf", ReadTypeCodes.Nested, "Reference"),
                new ElementChoiceInfo("targetFormat", ReadTypeCodes.String, string.Empty),
                new ElementChoiceInfo("sigFormat", ReadTypeCodes.String, string.Empty),
                new ElementChoiceInfo("data", ReadTypeCodes.ByteArray, string.Empty),
            }
        },
        {
            "Timing",
            new[]
            {
                new ElementChoiceInfo("event", ReadTypeCodes.StringArray, string.Empty),
                new ElementChoiceInfo("repeat", ReadTypeCodes.Nested, "TimingRepeat"),
                new ElementChoiceInfo("code", ReadTypeCodes.Nested, "CodeableConcept"),
            }
        },
        {
            "TimingRepeat",
            new[]
            {
                new ElementChoiceInfo("boundsDuration", ReadTypeCodes.Nested, "Quantity"),
                new ElementChoiceInfo("boundsRange", ReadTypeCodes.Nested, "Range"),
                new ElementChoiceInfo("boundsPeriod", ReadTypeCodes.Nested, "Period"),
                new ElementChoiceInfo("count", ReadTypeCodes.Integer, string.Empty),
                new ElementChoiceInfo("countMax", ReadTypeCodes.Integer, string.Empty),
                new ElementChoiceInfo("duration", ReadTypeCodes.Decimal, string.Empty),
                new ElementChoiceInfo("durationMax", ReadTypeCodes.Decimal, string.Empty),
                new ElementChoiceInfo("durationUnit", ReadTypeCodes.String, string.Empty),
                new ElementChoiceInfo("frequency", ReadTypeCodes.Integer, string.Empty),
                new ElementChoiceInfo("frequencyMax", ReadTypeCodes.Integer, string.Empty),
                new ElementChoiceInfo("period", ReadTypeCodes.Decimal, string.Empty),
                new ElementChoiceInfo("periodMax", ReadTypeCodes.Decimal, string.Empty),
                new ElementChoiceInfo("periodUnit", ReadTypeCodes.String, string.Empty),
                new ElementChoiceInfo("dayOfWeek", ReadTypeCodes.StringArray, string.Empty),
                new ElementChoiceInfo("timeOfDay", ReadTypeCodes.StringArray, string.Empty),
                new ElementChoiceInfo("when", ReadTypeCodes.StringArray, string.Empty),
                new ElementChoiceInfo("offset", ReadTypeCodes.Integer, string.Empty),
            }
        },
    };

    /// <summary>
    /// Initializes a new instance of the <see cref="FromNormative"/> class.
    /// </summary>
    public FromNormative()
    {
        _errors = new();
        _warnings = new();
    }

    /// <summary>Process the code system.</summary>
    /// <param name="cs">             The create struct.</param>
    /// <param name="fhirVersionInfo">FHIR Version information.</param>
    private void ProcessCodeSystem(
        JsonNode cs,
        IPackageImportable fhirVersionInfo)
    {
        string csStatus = cs["status"]?.GetValue<string>();
        string csName = cs["name"]?.GetValue<string>() ?? string.Empty;
        string csId = cs["id"]?.GetValue<string>() ?? string.Empty;

        if (string.IsNullOrEmpty(csStatus))
        {
            csStatus = "unknown";
            _errors.Add($"CodeSystem {csName} ({csId}): Status field missing");
        }

        // ignore retired
        if (csStatus.Equals("retired", StringComparison.Ordinal))
        {
            return;
        }

        Dictionary<string, FhirCodeSystem.FilterDefinition> filters = new();

        if (cs["filter"] != null)
        {
            foreach (JsonNode filter in cs["filter"].AsArray())
            {
                string filterCode = filter["code"]?.GetValue<string>();

                if (string.IsNullOrEmpty(filterCode))
                {
                    continue;
                }

                filters.Add(
                    filterCode,
                    new(
                        filterCode,
                        filter["description"]?.GetValue<string>() ?? string.Empty,
                        filter["operator"]?.AsArray().Select((op) => op.GetValue<string>()).AsEnumerable() ?? Array.Empty<string>(),
                        filter["value"]?.GetValue<string>() ?? string.Empty));
            }
        }

        Dictionary<string, FhirCodeSystem.PropertyDefinition> properties = new();

        if (cs["property"] != null)
        {
            foreach (JsonNode prop in cs["property"].AsArray())
            {
                string propCode = prop["code"]?.GetValue<string>();

                if (string.IsNullOrEmpty(propCode))
                {
                    continue;
                }

                if (properties.ContainsKey(propCode))
                {
                    _warnings.Add($"CodeSystem {csName} ({csId}): Duplicate proprety found: {propCode}");
                    continue;
                }

                properties.Add(
                    propCode,
                    new(
                        propCode,
                        prop["uri"]?.GetValue<string>() ?? string.Empty,
                        prop["description"]?.GetValue<string>() ?? string.Empty,
                        FhirCodeSystem.PropertyTypeFromValue(prop["type"]?.GetValue<string>() ?? string.Empty)));
            }
        }

        Dictionary<string, FhirConceptTreeNode> nodeLookup = new Dictionary<string, FhirConceptTreeNode>();
        FhirConceptTreeNode root = new FhirConceptTreeNode(null, null);

        if (cs["concept"] != null)
        {
            AddConceptTree(
                cs["url"]?.GetValue<string>() ?? string.Empty,
                cs["id"]?.GetValue<string>() ?? string.Empty,
                cs["concept"]?.AsArray(),
                root,
                nodeLookup,
                properties);
        }

        FhirCodeSystem codeSystem = new FhirCodeSystem(
            cs["name"]?.GetValue<string>(),
            cs["id"]?.GetValue<string>(),
            cs["version"]?.GetValue<string>() ?? string.Empty,
            cs["title"]?.GetValue<string>() ?? string.Empty,
            cs["url"]?.GetValue<string>() ?? string.Empty,
            csStatus,
            cs["description"]?.GetValue<string>() ?? string.Empty,
            cs["content"]?.GetValue<string>(),
            root,
            nodeLookup,
            filters,
            properties);

        // add our code system
        fhirVersionInfo.AddCodeSystem(codeSystem);
    }

    /// <summary>Adds a concept tree to 'concepts'.</summary>
    /// <param name="codeSystemUrl">      URL of the code system.</param>
    /// <param name="codeSystemId">       Id of the code system.</param>
    /// <param name="concepts">           The concept.</param>
    /// <param name="parent">             The parent.</param>
    /// <param name="nodeLookup">         The node lookup.</param>
    /// <param name="propertyDefinitions">The property definitions.</param>
    private void AddConceptTree(
        string codeSystemUrl,
        string codeSystemId,
        JsonArray concepts,
        FhirConceptTreeNode parent,
        Dictionary<string, FhirConceptTreeNode> nodeLookup,
        Dictionary<string, FhirCodeSystem.PropertyDefinition> propertyDefinitions)
    {
        if ((concepts == null) ||
            (concepts.Count == 0) ||
            (parent == null))
        {
            return;
        }

        List<KeyValuePair<string, string>> properties = new List<KeyValuePair<string, string>>();

        foreach (JsonNode concept in concepts)
        {
            if (TryBuildInternalConceptFromFhir(
                    codeSystemUrl,
                    codeSystemId,
                    concept,
                    propertyDefinitions,
                    out FhirConcept fhirConcept,
                    nodeLookup))
            {
                FhirConceptTreeNode node = parent.AddChild(fhirConcept);

                if (concept["concept"] != null)
                {
                    AddConceptTree(codeSystemUrl, codeSystemId, concept["concept"].AsArray(), node, nodeLookup, propertyDefinitions);
                }

                string conceptCode = concept["code"]?.GetValue<string>();

                // codes may be referenced multiple times depending on nesting structure
                if (!nodeLookup.ContainsKey(conceptCode))
                {
                    nodeLookup.Add(conceptCode, node);
                }
            }
        }
    }

    /// <summary>Attempts to build internal concept from FHIR.</summary>
    /// <param name="codeSystemUrl">      URL of the code system.</param>
    /// <param name="codeSystemId">       Id of the code system.</param>
    /// <param name="concept">            The concept.</param>
    /// <param name="propertyDefinitions">The property definitions.</param>
    /// <param name="fhirConcept">        [out] The FHIR concept.</param>
    /// <param name="nodeLookup">         (Optional) The node lookup.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    private bool TryBuildInternalConceptFromFhir(
        string codeSystemUrl,
        string codeSystemId,
        JsonNode concept,
        Dictionary<string, FhirCodeSystem.PropertyDefinition> propertyDefinitions,
        out FhirConcept fhirConcept,
        Dictionary<string, FhirConceptTreeNode> nodeLookup = null)
    {
        string code = concept["code"]?.GetValue<string>() ?? string.Empty;

        if (string.IsNullOrEmpty(code))
        {
            fhirConcept = null;
            return false;
        }

        if ((nodeLookup != null) &&
            nodeLookup.ContainsKey(code))
        {
            fhirConcept = null;
            return false;
        }

        fhirConcept = new FhirConcept(
            codeSystemUrl,
            code,
            concept["display"]?.GetValue<string>() ?? string.Empty,
            string.Empty,
            concept["definition"]?.GetValue<string>() ?? string.Empty,
            codeSystemId);

        if (concept["property"] != null)
        {
            foreach (JsonNode prop in concept["property"].AsArray())
            {
                string propCode = prop["code"]?.GetValue<string>() ?? string.Empty;

                if (string.IsNullOrEmpty(propCode) ||
                    (!propertyDefinitions.ContainsKey(propCode)))
                {
                    continue;
                }

                if ((propCode == "status") && (prop["valueCode"]?.GetValue<string>() == "deprecated"))
                {
                    fhirConcept = null;
                    return false;
                }

                switch (propertyDefinitions[propCode].PropType)
                {
                    case FhirCodeSystem.PropertyTypeEnum.Code:
                        fhirConcept.AddProperty(
                            propCode,
                            prop["valueCode"]?.GetValue<string>(),
                            prop["valueCode"]?.GetValue<string>());
                        break;

                    case FhirCodeSystem.PropertyTypeEnum.Coding:
                        {
                            string codingSystem = prop["valueCoding"]?["system"]?.GetValue<string>() ?? string.Empty;
                            string codingCode = prop["valueCoding"]?["code"]?.GetValue<string>() ?? string.Empty;
                            string codingVersion = prop["valueCoding"]?["version"]?.GetValue<string>() ?? string.Empty;

                            fhirConcept.AddProperty(
                                propCode,
                                (system: codingSystem, code: codingCode, version: codingVersion),
                                FhirConcept.GetCanonical(
                                    codingSystem,
                                    codingCode,
                                    codingVersion));
                        }

                        break;

                    case FhirCodeSystem.PropertyTypeEnum.String:
                        fhirConcept.AddProperty(
                            propCode,
                            prop["valueString"]?.GetValue<string>() ?? null,
                            prop["valueString"]?.GetValue<string>() ?? string.Empty);
                        break;

                    case FhirCodeSystem.PropertyTypeEnum.Integer:
                        fhirConcept.AddProperty(
                            propCode,
                            prop["valueInteger"]?.GetValue<int?>() ?? null,
                            prop["valueInteger"]?.GetValue<int?>()?.ToString() ?? string.Empty);
                        break;

                    case FhirCodeSystem.PropertyTypeEnum.Boolean:
                        fhirConcept.AddProperty(
                            propCode,
                            prop["valueBoolean"]?.GetValue<bool?>() ?? null,
                            prop["valueBoolean"]?.GetValue<bool?>()?.ToString() ?? string.Empty);
                        break;

                    case FhirCodeSystem.PropertyTypeEnum.DateTime:
                        fhirConcept.AddProperty(
                            propCode,
                            prop["valueDateTime"]?.GetValue<string>() ?? null,
                            prop["valueDateTime"]?.GetValue<string>() ?? string.Empty);
                        break;

                    case FhirCodeSystem.PropertyTypeEnum.Decimal:
                        fhirConcept.AddProperty(
                            propCode,
                            prop["valueDecimal"]?.GetValue<decimal?>() ?? null,
                            prop["valueDecimal"]?.GetValue<decimal?>()?.ToString() ?? string.Empty);
                        break;
                }
            }
        }

        return true;
    }

    /// <summary>Process the operation.</summary>
    /// <param name="op">             The operation.</param>
    /// <param name="fhirVersionInfo">FHIR Version information.</param>
    private void ProcessOperation(
        JsonNode op,
        IPackageImportable fhirVersionInfo)
    {
        string status = op["status"]?.GetValue<string>() ?? "unknown";

        // ignore retired
        if (status.Equals("retired", StringComparison.Ordinal))
        {
            return;
        }

        List<FhirParameter> parameters = new List<FhirParameter>();

        if (op["parameter"] != null)
        {
            foreach (JsonNode opParam in op["parameter"].AsArray())
            {
                parameters.Add(new FhirParameter(
                    opParam["name"]?.GetValue<string>(),
                    opParam["use"]?.GetValue<string>(),
                    opParam["scope"]?.AsArray().Select(n => n.GetValue<string>()).AsEnumerable() ?? null,
                    opParam["min"]?.GetValue<int>() ?? 0,
                    opParam["max"]?.GetValue<string>(),
                    opParam["documentation"]?.GetValue<string>(),
                    opParam["type"]?.GetValue<string>(),
                    opParam["allowedType"]?.AsArray().Select(n => n.GetValue<string>()).AsEnumerable() ?? null,
                    opParam["targetProfile"]?.AsArray().Select(n => n.GetValue<string>()).AsEnumerable() ?? null,
                    opParam["searchType"]?.GetValue<string>(),
                    parameters.Count));
            }
        }

        string opBase;

        if (op["base"] == null)
        {
            opBase = null;
        }
        else if (op["base"].GetType() == typeof(JsonObject))
        {
            // R3 and lower, base is a reference
            opBase = op["base"]["reference"]?.GetValue<string>();
        }
        else
        {
            // R4 and higher, base is a 'canonical'
            opBase = op["base"].GetValue<string>();
        }

        // create the operation
        FhirOperation operation = new FhirOperation(
            op["id"]?.GetValue<string>(),
            new Uri(op["url"]?.GetValue<string>()),
            op["version"]?.GetValue<string>(),
            op["name"]?.GetValue<string>(),
            op["description"]?.GetValue<string>(),
            op["affectsState"]?.GetValue<bool?>(),
            op["system"]?.GetValue<bool?>() ?? false,
            op["type"]?.GetValue<bool?>() ?? false,
            op["instance"]?.GetValue<bool?>() ?? false,
            op["code"]?.GetValue<string>(),
            op["comment"]?.GetValue<string>(),
            opBase,
            op["resource"]?.AsArray().Select(n => n.GetValue<string>()).ToList() ?? null,
            parameters,
            op["experimental"]?.GetValue<bool>() == true);

        // add our operation
        fhirVersionInfo.AddOperation(operation);
    }

    /// <summary>Process the search parameter.</summary>
    /// <param name="sp">             The search parameter.</param>
    /// <param name="fhirVersionInfo">FHIR Version information.</param>
    private void ProcessSearchParam(
        JsonNode sp,
        IPackageImportable fhirVersionInfo)
    {
        string status = sp["status"]?.GetValue<string>() ?? "unknown";

        // ignore retired
        if (status.Equals("retired", StringComparison.Ordinal))
        {
            return;
        }

        List<string> resources = sp["base"]?.AsArray().Select(n => n.GetValue<string>()).ToList() ?? null;

        // check for parameters with no base resource
        if (resources == null)
        {
            resources = new();

            // see if we can determine the resource based on id
            string[] components = sp["id"]?.GetValue<string>().Split('-');

            foreach (string component in components)
            {
                if (fhirVersionInfo.Resources.ContainsKey(component))
                {
                    resources.Add(component);
                }
            }

            // don't know where to put this, could try parsing XPath in the future
            if (resources.Count == 0)
            {
                return;
            }
        }

        // create the search parameter
        FhirSearchParam param = new FhirSearchParam(
            sp["id"]?.GetValue<string>(),
            new Uri(sp["url"]?.GetValue<string>()),
            sp["version"]?.GetValue<string>(),
            sp["name"]?.GetValue<string>(),
            sp["description"]?.GetValue<string>(),
            sp["purpose"]?.GetValue<string>(),
            sp["code"]?.GetValue<string>(),
            resources,
            sp["target"]?.AsArray().Select(n => n.GetValue<string>()).ToList(),
            sp["type"]?.GetValue<string>(),
            status,
            sp["experimental"]?.GetValue<bool?>() == true,
            sp["xpath"]?.GetValue<string>() ?? string.Empty,
            sp["processingMode"]?.GetValue<string>() ?? sp["xpathUsage"]?.GetValue<string>() ?? string.Empty,
            sp["expression"]?.GetValue<string>());

        // add our parameter
        fhirVersionInfo.AddSearchParameter(param);
    }

    /// <summary>Process the value set.</summary>
    /// <param name="vs">             The value set.</param>
    /// <param name="fhirVersionInfo">FHIR Version information.</param>
    private static void ProcessValueSet(
        JsonNode vs,
        IPackageImportable fhirVersionInfo)
    {
        string vsStatus = vs["status"]?.GetValue<string>() ?? "unknown";
        string vsId = vs["id"]?.GetValue<string>() ?? string.Empty;
        string vsName = vs["name"]?.GetValue<string>() ?? string.Empty;
        string vsUrl = vs["url"]?.GetValue<string>() ?? string.Empty;
        string vsVersion = vs["version"]?.GetValue<string>() ?? string.Empty;

        // ignore retired
        if (vsStatus.Equals("retired", StringComparison.Ordinal))
        {
            return;
        }

        if (string.IsNullOrEmpty(vsUrl))
        {
            _errors.Add($"ValueSet {vsName} ({vsId}): Cannot be indexed - missing URL");
            return;
        }

        // do not process a value set if we have already loaded it
        if (fhirVersionInfo.HasValueSet(vsUrl))
        {
            return;
        }

        if (string.IsNullOrEmpty(vsVersion))
        {
            _warnings.Add($"ValueSet {vsName} ({vsId}): No Version present");
            vsVersion = fhirVersionInfo.VersionString;
        }

        List<FhirValueSetComposition> includes = null;
        List<FhirValueSetComposition> excludes = null;
        FhirValueSetExpansion expansion = null;

        if ((vs["compose"] != null) &&
            (vs["compose"]["include"] != null))
        {
            includes = new List<FhirValueSetComposition>();

            foreach (JsonNode compose in vs["compose"]["include"].AsArray())
            {
                includes.Add(BuildComposition(compose));
            }
        }

        if ((vs["compose"] != null) &&
            (vs["compose"]["exclude"] != null))
        {
            excludes = new List<FhirValueSetComposition>();

            foreach (JsonNode compose in vs["compose"]["exclude"].AsArray())
            {
                excludes.Add(BuildComposition(compose));
            }
        }

        if (vs["expansion"] != null)
        {
            Dictionary<string, dynamic> parameters = null;

            if (vs["expansion"]["parameter"] != null)
            {
                parameters = new Dictionary<string, dynamic>();

                foreach (JsonNode param in vs["expansion"]["parameter"].AsArray())
                {
                    string paramName = param["name"]?.GetValue<string>();
                    if (string.IsNullOrEmpty(paramName) || parameters.ContainsKey(paramName))
                    {
                        continue;
                    }

                    if (param["valueBoolean"] != null)
                    {
                        parameters.Add(paramName, param["valueBoolean"].GetValue<bool>());
                        continue;
                    }

                    if (param["valueCode"] != null)
                    {
                        parameters.Add(paramName, param["valueCode"].GetValue<string>());
                        continue;
                    }

                    if (param["valueDateTime"] != null)
                    {
                        parameters.Add(paramName, param["valueDateTime"].GetValue<string>());
                        continue;
                    }

                    if (param["valueDecimal"] != null)
                    {
                        parameters.Add(paramName, param["valueDecimal"].GetValue<decimal>());
                        continue;
                    }

                    if (param["valueInteger"] != null)
                    {
                        parameters.Add(paramName, param["valueInteger"].GetValue<int>());
                        continue;
                    }

                    if (param["valueUri"] != null)
                    {
                        parameters.Add(paramName, param["valueUri"].GetValue<string>());
                        continue;
                    }

                    if (param["valueString"] != null)
                    {
                        parameters.Add(paramName, param["valueString"].GetValue<string>());
                        continue;
                    }
                }
            }

            List<FhirConcept> expansionContains = null;

            if (vs["expansion"]["contains"] != null)
            {
                foreach (JsonNode contains in vs["expansion"]["contains"].AsArray())
                {
                    AddContains(ref expansionContains, contains);
                }
            }

            expansion = new FhirValueSetExpansion(
                vs["expansion"]["id"]?.GetValue<string>() ?? string.Empty,
                vs["expansion"]["timestamp"]?.GetValue<string>() ?? string.Empty,
                vs["expansion"]["total"]?.GetValue<int>(),
                vs["expansion"]["offset"]?.GetValue<int>(),
                parameters,
                expansionContains);
        }

        FhirValueSet valueSet = new FhirValueSet(
            vsName,
            vsId,
            vsVersion,
            vs["title"]?.GetValue<string>() ?? vsName,
            vsUrl,
            vsStatus,
            vs["description"]?.GetValue<string>() ?? vsName,
            includes,
            excludes,
            expansion);

        // add our code system
        fhirVersionInfo.AddValueSet(valueSet);
    }

    /// <summary>Adds a set of contains clauses to a value set expansion.</summary>
    /// <param name="contains">[in,out] The contains.</param>
    /// <param name="ec">      The ec.</param>
    private static void AddContains(ref List<FhirConcept> contains, JsonNode ec)
    {
        if (contains == null)
        {
            contains = new List<FhirConcept>();
        }

        FhirConcept fhirConcept = new FhirConcept(
            ec["system"]?.GetValue<string>(),
            ec["code"]?.GetValue<string>(),
            ec["display"]?.GetValue<string>(),
            ec["version"]?.GetValue<string>(),
            string.Empty,
            string.Empty);

        if (ec["property"] != null)
        {
            foreach (JsonNode prop in ec["property"].AsArray())
            {
                string propCode = prop["code"]?.GetValue<string>();

                if (string.IsNullOrEmpty(propCode))
                {
                    continue;
                }

                if (prop["valueCode"] != null)
                {
                    fhirConcept.AddProperty(
                        propCode,
                        prop["valueCode"].GetValue<string>(),
                        prop["valueCode"].GetValue<string>());
                    continue;
                }

                if (prop["valueCoding"] != null)
                {
                    string codingSystem = prop["valueCoding"]?["system"]?.GetValue<string>() ?? string.Empty;
                    string codingCode = prop["valueCoding"]?["code"]?.GetValue<string>() ?? string.Empty;
                    string codingVersion = prop["valueCoding"]?["version"]?.GetValue<string>() ?? string.Empty;

                    fhirConcept.AddProperty(
                        propCode,
                        (system: codingSystem, code: codingCode, version: codingVersion),
                        FhirConcept.GetCanonical(
                            codingSystem,
                            codingCode,
                            codingVersion));
                    continue;
                }

                if (prop["valueString"] != null)
                {
                    fhirConcept.AddProperty(
                        propCode,
                        prop["valueString"]?.GetValue<string>() ?? null,
                        prop["valueString"]?.GetValue<string>() ?? string.Empty);
                    continue;
                }

                if (prop["valueInteger"] != null)
                {
                    fhirConcept.AddProperty(
                        propCode,
                        prop["valueInteger"]?.GetValue<int?>() ?? null,
                        prop["valueInteger"]?.GetValue<int?>()?.ToString() ?? string.Empty);
                    continue;
                }

                if (prop["valueBoolean"] != null)
                {
                    fhirConcept.AddProperty(
                        propCode,
                        prop["valueBoolean"]?.GetValue<bool?>() ?? null,
                        prop["valueBoolean"]?.GetValue<bool?>()?.ToString() ?? string.Empty);
                    continue;
                }

                if (prop["valueDateTime"] != null)
                {
                    fhirConcept.AddProperty(
                        propCode,
                        prop["valueDateTime"]?.GetValue<string>() ?? null,
                        prop["valueDateTime"]?.GetValue<string>() ?? string.Empty);
                    continue;
                }

                if (prop["valueDecmial"] != null)
                {
                    fhirConcept.AddProperty(
                        propCode,
                        prop["valueDecimal"]?.GetValue<decimal?>() ?? null,
                        prop["valueDecimal"]?.GetValue<decimal?>()?.ToString() ?? string.Empty);
                    continue;
                }
            }
        }

        // TODO: Determine if the Inactive flag needs to be checked
        if ((!string.IsNullOrEmpty(ec["system"]?.GetValue<string>())) ||
            (!string.IsNullOrEmpty(ec["code"]?.GetValue<string>())))
        {
            contains.Add(fhirConcept);
        }

        if (ec["contains"] != null)
        {
            foreach (JsonNode subContains in ec["contains"].AsArray())
            {
                AddContains(ref contains, subContains);
            }
        }
    }

    /// <summary>Builds a composition.</summary>
    /// <param name="compose">The compose.</param>
    /// <returns>A FhirValueSetComposition.</returns>
    private static FhirValueSetComposition BuildComposition(JsonNode compose)
    {
        if (compose == null)
        {
            return null;
        }

        List<FhirConcept> concepts = null;
        List<FhirValueSetFilter> filters = null;
        List<string> linkedValueSets = null;

        if (compose["concept"] != null)
        {
            concepts = new List<FhirConcept>();

            foreach (JsonNode concept in compose["concept"].AsArray())
            {
                concepts.Add(new FhirConcept(
                    compose["system"]?.GetValue<string>(),
                    concept["code"]?.GetValue<string>(),
                    concept["display"]?.GetValue<string>()));
            }
        }

        if (compose["filter"] != null)
        {
            filters = new List<FhirValueSetFilter>();

            foreach (JsonNode filter in compose["filter"].AsArray())
            {
                filters.Add(new FhirValueSetFilter(
                    filter["property"]?.GetValue<string>(),
                    filter["op"]?.GetValue<string>(),
                    filter["value"]?.GetValue<string>()));
            }
        }

        if (compose["valueSet"] != null)
        {
            linkedValueSets = new List<string>();

            foreach (JsonNode valueSet in compose["valueSet"].AsArray())
            {
                if (string.IsNullOrEmpty(valueSet.GetValue<string>()))
                {
                    continue;
                }

                linkedValueSets.Add(valueSet.GetValue<string>());
            }
        }

        return new FhirValueSetComposition(
            compose["system"]?.GetValue<string>(),
            compose["version"]?.GetValue<string>(),
            concepts,
            filters,
            linkedValueSets);
    }

    /// <summary>Process the structure definition.</summary>
    /// <param name="sd">             The structure definition we are parsing.</param>
    /// <param name="fhirVersionInfo">FHIR Version information.</param>
    private void ProcessStructureDef(
        JsonNode sd,
        IPackageImportable fhirVersionInfo)
    {
        string sdStatus = sd["status"]?.GetValue<string>() ?? "unknown";

        // ignore retired
        if (sdStatus == "retired")
        {
            return;
        }

        string sdKind = sd["kind"]?.GetValue<string>() ?? string.Empty;

        // act depending on kind
        switch (sdKind)
        {
            case "primitive-type":
                ProcessDataTypePrimitive(sd, fhirVersionInfo);
                break;

            case "logical":
                ProcessComplex(sd, fhirVersionInfo, FhirArtifactClassEnum.LogicalModel);
                break;

            case "resource":
            case "complex-type":
                if (sd["derivation"]?.GetValue<string>() == "constraint")
                {
                    if (sd["type"]?.GetValue<string>() == "Extension")
                    {
                        ProcessComplex(sd, fhirVersionInfo, FhirArtifactClassEnum.Extension);
                    }
                    else
                    {
                        ProcessComplex(sd, fhirVersionInfo, FhirArtifactClassEnum.Profile);
                    }
                }
                else
                {
                    ProcessComplex(
                        sd,
                        fhirVersionInfo,
                        sdKind == "complex-type" ? FhirArtifactClassEnum.ComplexType : FhirArtifactClassEnum.Resource);
                }

                break;
        }
    }

    /// <summary>Process a structure definition for a Primitive data type.</summary>
    /// <param name="sd">             The structure definition.</param>
    /// <param name="fhirVersionInfo">FHIR Version information.</param>
    private static void ProcessDataTypePrimitive(
        JsonNode sd,
        IPackageImportable fhirVersionInfo)
    {
        string sdId = sd["id"]?.GetValue<string>() ?? string.Empty;
        string sdName = sd["name"]?.GetValue<string>() ?? string.Empty;
        string sdStatus = sd["status"]?.GetValue<string>() ?? "unknown";

        string regex = string.Empty;
        string descriptionShort = sd["description"]?.GetValue<string>() ?? string.Empty;
        string definition = sd["purpose"]?.GetValue<string>() ?? string.Empty;
        string comment = string.Empty;
        string baseTypeName = string.Empty;

        // right now, differential is generally 'more correct' than snapshot for primitives, see FHIR-37465
        if ((sd["differential"] != null) &&
            (sd["differential"]["element"] != null))
        {
            foreach (JsonNode element in sd["differential"]["element"].AsArray())
            {
                string elementId = element["id"]?.GetValue<string>();

                if (elementId == sdId)
                {
                    descriptionShort = element["short"]?.GetValue<string>() ?? descriptionShort;
                    definition = element["definition"]?.GetValue<string>() ?? definition;
                    comment = element["comment"]?.GetValue<string>() ?? comment;
                    continue;
                }

                if (elementId != $"{sdId}.value")
                {
                    continue;
                }

                if (element["type"] == null)
                {
                    continue;
                }

                foreach (JsonNode type in element["type"].AsArray())
                {
                    string typeCode = type["code"]?.GetValue<string>() ?? string.Empty;

                    if (!string.IsNullOrEmpty(typeCode))
                    {
                        if (FhirElementType.IsFhirPathType(typeCode, out string fhirType))
                        {
                            baseTypeName = fhirType;
                        }
                        else if (FhirElementType.IsXmlBaseType(typeCode, out string xmlFhirType))
                        {
                            baseTypeName = xmlFhirType;
                        }
                    }

                    if (type["extension"] == null)
                    {
                        continue;
                    }

                    foreach (JsonNode ext in type["extension"].AsArray())
                    {
                        string extUrl = ext["url"]?.GetValue<string>() ?? string.Empty;

                        if ((extUrl == "http://hl7.org/fhir/StructureDefinition/regex") ||
                            (extUrl == "http://hl7.org/fhir/StructureDefinition/structuredefinition-regex"))
                        {
                            regex = ext["valueString"]?.GetValue<string>();
                            break;
                        }
                    }
                }
            }
        }

        if (string.IsNullOrEmpty(baseTypeName))
        {
            baseTypeName = sdName;
        }

        // create a new primitive type object
        FhirPrimitive primitive = new FhirPrimitive(
            sdId,
            sdName,
            baseTypeName,
            new Uri(sd["url"]?.GetValue<string>() ?? string.Empty),
            sdStatus,
            sd["experimental"]?.GetValue<bool>() == true,
            descriptionShort,
            definition,
            comment,
            regex);

        // add to our dictionary of primitive types
        fhirVersionInfo.AddPrimitive(primitive);
    }

    /// <summary>Process a complex structure (Complex Type or Resource).</summary>
    /// <exception cref="InvalidDataException">Thrown when an Invalid Data error condition occurs.</exception>
    /// <param name="sd">             The structure definition to parse.</param>
    /// <param name="fhirVersionInfo">FHIR Version information.</param>
    /// <param name="artifactClass">  Type of structure definition we are parsing.</param>
    private static void ProcessComplex(
        JsonNode sd,
        IPackageImportable fhirVersionInfo,
        FhirArtifactClassEnum artifactClass)
    {
        if ((sd["snapshot"] == null) || (sd["snapshot"]["element"] == null))
        {
            return;
        }

        string sdId = sd["id"]?.GetValue<string>() ?? string.Empty;
        string sdName = sd["name"]?.GetValue<string>() ?? string.Empty;
        string sdStatus = sd["status"]?.GetValue<string>() ?? "unknown";
        string sdType = sd["type"]?.GetValue<string>() ?? sdName;
        string sdUrl = sd["url"]?.GetValue<string>() ?? string.Empty;

        string descriptionShort = sd["description"]?.GetValue<string>() ?? string.Empty;
        string definition = sd["purpose"]?.GetValue<string>() ?? string.Empty;
        //string regex = string.Empty;
        //string comment = string.Empty;
        //string baseTypeName = string.Empty;

        try
        {
            List<string> contextElements = new List<string>();
            if (sd["context"] != null)
            {
                foreach (JsonNode context in sd["context"].AsArray())
                {
                    if (context.GetType() == typeof(JsonObject))
                    {
                        // R4 and higher, context is a backbone element
                        if (context["type"]?.GetValue<string>() != "element")
                        {
                            // throw new ArgumentException($"Invalid extension context type: {context.Type}");
                            _errors.Add($"StructureDefinition {sdName} ({sdId}) unhandled context type: {(string)context["type"]}");
                            return;
                        }

                        contextElements.Add(context["expression"]?.GetValue<string>());
                    }
                    else
                    {
                        // R3 and lower, context is a simple value
                        contextElements.Add(context.GetValue<string>());
                    }
                }
            }

            if (sd["snapshot"]["element"]?[0] != null)
            {
                descriptionShort = sd["snapshot"]["element"][0]["short"]?.GetValue<string>() ?? descriptionShort;
                definition = sd["snapshot"]["element"][0]["definition"]?.GetValue<string>() ?? definition;
            }

            // create a new complex type object for this type or resource
            FhirComplex complex = new FhirComplex(
                sdId,
                sdName,
                string.Empty,
                sdType,
                new Uri(sdUrl),
                sdStatus,
                sd["experimental"]?.GetValue<bool>() == true,
                descriptionShort,
                definition,
                string.Empty,
                null,
                contextElements,
                sd["abstract"].GetValue<bool>());

            // check for a base definition
            if (sd["baseDefinition"] != null)
            {
                string bd = sd["baseDefinition"].GetValue<string>();
                complex.BaseTypeName = bd.Substring(bd.LastIndexOf('/') + 1);
            }
            else
            {
                if (!TryGetTypeFromElements(
                        sdName,
                        sd["snapshot"]["element"],
                        out Dictionary<string, FhirElementType> baseTypes,
                        out string _,
                        out bool _))
                {
                    throw new InvalidDataException($"Could not determine base type for {sdName}");
                }

                if (baseTypes.Count == 0)
                {
                    throw new InvalidDataException($"Could not determine base type for {sdName}");
                }

                if (baseTypes.Count > 1)
                {
                    throw new InvalidDataException($"Too many types for {sdName}: {baseTypes.Count}");
                }

                complex.BaseTypeName = baseTypes.ElementAt(0).Value.Name;
            }

            // look for properties on this type
            foreach (JsonNode element in sd["snapshot"]["element"].AsArray())
            {
                string elementId = element["id"]?.GetValue<string>() ?? element["path"]?.GetValue<string>() ?? string.Empty;
                string elementPath = element["path"]?.GetValue<string>() ?? element["id"]?.GetValue<string>() ?? string.Empty;

                try
                {
                    Dictionary<string, FhirElementType> elementTypes = null;
                    string elementType = string.Empty;
                    string regex = string.Empty;
                    bool isRootElement = false;
                    bool isSimple = false;

                    // split the id into component parts
                    string[] idComponents = elementId.Split('.');
                    string[] pathComponents = elementPath.Split('.');

                    // base definition, already processed
                    if (pathComponents.Length < 2)
                    {
                        // check for this component being different from primar
                        if ((pathComponents[0] != sdName) && (contextElements.Count == 0))
                        {
                            // add to our context
                            complex.AddContextElement(pathComponents[0]);
                        }

                        // parse as root element
                        isRootElement = true;
                    }

                    // get the parent container and our field name
                    if (!complex.GetParentAndFieldName(
                            sdUrl,
                            idComponents,
                            pathComponents,
                            out FhirComplex parent,
                            out string field,
                            out string sliceName))
                    {
                        if (isRootElement)
                        {
                            parent = complex;
                            field = string.Empty;
                            sliceName = string.Empty;
                        }
                        else
                        {
                            // throw new InvalidDataException($"Could not find parent for {element.Path}!");
                            // should load later
                            // TODO: figure out a way to verify all dependencies loaded
                            continue;
                        }
                    }

                    // check for needing to add a slice to an element
                    if (!string.IsNullOrEmpty(sliceName))
                    {
                        // check for extension (implicit slicing in differentials)
                        if ((!parent.Elements.ContainsKey(elementPath)) && (field == "extension"))
                        {
                            // grab the extension definition
                            parent.Elements.Add(
                                elementPath,
                                new FhirElement(
                                    elementPath,
                                    elementPath,
                                    string.Empty,
                                    null,
                                    parent.Elements.Count,
                                    ExtensionShort,
                                    ExtensionDefinition,
                                    ExtensionComment,
                                    string.Empty,
                                    "Extension",
                                    null,
                                    0,
                                    "*",
                                    element["isModifier"]?.GetValue<bool>(),
                                    element["isModifierReason"]?.GetValue<string>(),
                                    element["isSummary"]?.GetValue<bool>(),
                                    element["mustSupport"]?.GetValue<bool>(),
                                    false,
                                    string.Empty,
                                    null,
                                    string.Empty,
                                    null,
                                    string.Empty,
                                    null,
                                    true,
                                    true,
                                    string.Empty,
                                    string.Empty,
                                    null,
                                    null));
                        }

                        // check for implicit slicing definition
                        if (parent.Elements.ContainsKey(elementPath) &&
                            (!parent.Elements[elementPath].Slicing.ContainsKey(sdUrl)))
                        {
                            List<FhirSliceDiscriminatorRule> discriminatorRules = new List<FhirSliceDiscriminatorRule>()
                                {
                                    new FhirSliceDiscriminatorRule(
                                        "value",
                                        "url"),
                                };

                            // create our slicing
                            parent.Elements[elementPath].AddSlicing(
                                new FhirSlicing(
                                    sdId,
                                    new Uri(sdUrl),
                                    "Extensions are always sliced by (at least) url",
                                    null,
                                    "open",
                                    discriminatorRules));
                        }

                        // check for invalid slicing definition (composition-catalog)
                        if (parent.Elements.ContainsKey(elementPath))
                        {
                            // add this slice to the field
                            parent.Elements[elementPath].AddSlice(sdUrl, sliceName);
                        }

                        // only slice parent has slice name
                        continue;
                    }

                    // if we can't find a type, assume Element
                    if (!TryGetTypeFromElement(parent.Name, element, out elementTypes, out regex, out isSimple))
                    {
                        if ((field == "Extension") || (field == "extension"))
                        {
                            elementType = "Extension";
                        }
                        else
                        {
                            elementType = "Element";
                        }
                    }

                    string elementContentReference = element["contentReference"]?.GetValue<string>() ?? string.Empty;

                    // determine if there is type expansion
                    if (field.Contains("[x]", StringComparison.Ordinal))
                    {
                        // fix the field and path names
                        elementId = elementId.Replace("[x]", string.Empty, StringComparison.Ordinal);
                        field = field.Replace("[x]", string.Empty, StringComparison.Ordinal);

                        // force no base type
                        elementType = string.Empty;
                    }
                    else if (!string.IsNullOrEmpty(elementContentReference))
                    {
                        if (elementContentReference.StartsWith("http://hl7.org/fhir/StructureDefinition/", StringComparison.OrdinalIgnoreCase))
                        {
                            int loc = elementContentReference.IndexOf('#', StringComparison.Ordinal);
                            elementType = elementContentReference.Substring(loc + 1);
                        }
                        else if (elementContentReference[0] == '#')
                        {
                            // use the local reference
                            elementType = elementContentReference.Substring(1);
                        }
                        else
                        {
                            throw new InvalidDataException($"Could not resolve ContentReference {elementContentReference} in {sdName} field {elementPath}");
                        }
                    }

                    // get default values (if present)
                    GetValueIfPresent(element, "defaultValue", out string defaultName, out object defaultValue);

                    // get fixed values (if present)
                    GetValueIfPresent(element, "fixed", out string fixedName, out object fixedValue);

                    // get pattern values (if present)
                    GetValueIfPresent(element, "pattern", out string patternName, out object patternValue);

                    // determine if this element is inherited
                    bool isInherited = false;
                    bool modifiesParent = true;

                    if (!elementPath.StartsWith(complex.Name, StringComparison.Ordinal))
                    {
                        isInherited = true;
                    }

                    if (element["base"] != null)
                    {
                        if (element["base"]["path"]?.GetValue<string>() != elementPath)
                        {
                            isInherited = true;
                        }

                        if ((element["base"]["min"]?.GetValue<int>() == element["min"]?.GetValue<int>()) &&
                            (element["base"]["max"]?.GetValue<string>() == element["max"]?.GetValue<string>()) &&
                            (element["slicing"] == null))
                        {
                            modifiesParent = false;
                        }
                    }

                    string bindingStrength = string.Empty;
                    string valueSet = string.Empty;

                    if (element["binding"] != null)
                    {
                        bindingStrength = element["binding"]["strength"]?.GetValue<string>();

                        // R4 and later use 'valueSet' as canonical
                        // R3 uses 'valueSet[x]', uri or reference
                        valueSet = element["binding"]["valueSet"]?.GetValue<string>()
                            ?? element["binding"]["valueSetUri"]?.GetValue<string>();

                        if (string.IsNullOrEmpty(valueSet) &&
                            (element["binding"]["valueSetReference"] != null))
                        {
                            valueSet = element["binding"]["valueSetReference"]["reference"]?.GetValue<string>();
                        }
                    }

                    string explicitName = string.Empty;
                    if (element["extension"] != null)
                    {
                        foreach (JsonNode ext in element["extension"].AsArray())
                        {
                            string extUrl = ext["url"].GetValue<string>();

                            if (extUrl == "http://hl7.org/fhir/StructureDefinition/structuredefinition-explicit-type-name")
                            {
                                explicitName = ext["valueString"]?.GetValue<string>();
                            }
                        }
                    }

                    List<string> fwMapping;

                    fwMapping = element["mapping"]?.AsArray().Where(x =>
                        (x != null) &&
                        x["identity"].GetValue<string>().Equals("w5", StringComparison.OrdinalIgnoreCase) &&
                        x["map"].GetValue<string>().StartsWith("FiveWs", StringComparison.Ordinal) &&
                        (!x["map"].GetValue<string>().Equals("FiveWs.subject[x]", StringComparison.Ordinal)))?
                            .Select(x => x["map"].GetValue<string>()).ToList();

                    string fiveWs = ((fwMapping != null) && fwMapping.Any()) ? fwMapping[0] : string.Empty;

                    if (parent.Elements.ContainsKey(elementPath))
                    {
                        _errors.Add($"Complex {sdName} snapshot error ({elementPath}): Repeated snapshot: {parent.Elements[elementPath].Id} & {elementId}");
                        continue;
                    }

                    FhirElement fhirElement = new FhirElement(
                        elementId,
                        elementPath,
                        explicitName,
                        null,
                        parent.Elements.Count,
                        element["short"]?.GetValue<string>() ?? string.Empty,
                        element["definition"]?.GetValue<string>() ?? string.Empty,
                        element["comment"]?.GetValue<string>() ?? string.Empty,
                        regex,
                        elementType,
                        elementTypes,
                        element["min"]?.GetValue<int>() ?? 0,
                        element["max"]?.GetValue<string>() ?? string.Empty,
                        element["isModifier"]?.GetValue<bool>(),
                        element["isModifierReason"]?.GetValue<string>() ?? string.Empty,
                        element["isSummary"]?.GetValue<bool>(),
                        element["mustSupport"]?.GetValue<bool>(),
                        isSimple,
                        defaultName,
                        defaultValue,
                        fixedName,
                        fixedValue,
                        patternName,
                        patternValue,
                        isInherited,
                        modifiesParent,
                        bindingStrength,
                        valueSet,
                        fiveWs,
                        FhirElement.ConvertFhirRepresentations(element["representation"]?.AsArray().Select(n => n.GetValue<string>())));

                    if (isRootElement)
                    {
                        parent.AddRootElement(fhirElement);
                    }
                    else
                    {
                        // add this field to the parent type
                        parent.Elements.Add(elementPath, fhirElement);
                    }

                    if (element["slicing"] != null)
                    {
                        List<FhirSliceDiscriminatorRule> discriminatorRules = new List<FhirSliceDiscriminatorRule>();

                        if (element["slicing"]["discriminator"] == null)
                        {
                            throw new InvalidDataException($"Missing slicing discriminator: {sdName} - {elementPath}");
                        }

                        foreach (JsonNode discriminator in element["slicing"]["discriminator"].AsArray())
                        {
                            discriminatorRules.Add(new FhirSliceDiscriminatorRule(
                                discriminator["type"].GetValue<string>(),
                                discriminator["path"].GetValue<string>()));
                        }

                        // create our slicing
                        parent.Elements[elementPath].AddSlicing(
                            new FhirSlicing(
                                sdId,
                                new Uri(sdUrl),
                                element["slicing"]["description"]?.GetValue<string>(),
                                element["slicing"]["ordered"]?.GetValue<bool>(),
                                element["slicing"]["rules"].GetValue<string>(),
                                discriminatorRules));
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(string.Empty);
                    Console.WriteLine($"FromR5.ProcessComplex <<< element: {elementPath} ({elementId}) - exception: {ex.Message}");
                    throw;
                }
            }

            if ((sd["differential"] != null) &&
                (sd["differential"]["element"] != null) &&
                (sd["differential"]["element"][0] != null))
            {
                // look for additional constraints
                if ((sd["differential"]["element"][0]["constraint"] != null) &&
                    (sd["differential"]["element"][0]["constraint"][0] != null))
                {
                    foreach (JsonNode con in sd["differential"]["element"][0]["constraint"].AsArray())
                    {
                        bool isBestPractice = false;
                        string explanation = string.Empty;

                        if (con["extension"] != null)
                        {
                            foreach (JsonNode ext in con["extension"].AsArray())
                            {
                                string extUrl = ext["url"].GetValue<string>();

                                switch (extUrl)
                                {
                                    case "http://hl7.org/fhir/StructureDefinition/elementdefinition-bestpractice":
                                        isBestPractice = ext["valueBoolean"]?.GetValue<bool>() == true;
                                        break;

                                    case "http://hl7.org/fhir/StructureDefinition/elementdefinition-bestpractice-explanation":
                                        if (ext["valueMarkdown"] != null)
                                        {
                                            explanation = ext["valueMarkdown"].GetValue<string>();
                                        }
                                        else
                                        {
                                            explanation = ext["valueString"]?.GetValue<string>();
                                        }

                                        break;
                                }
                            }
                        }

                        complex.AddConstraint(new FhirConstraint(
                            con["key"].GetValue<string>(),
                            con["severity"].GetValue<string>(),
                            con["human"].GetValue<string>(),
                            con["expression"]?.GetValue<string>() ?? string.Empty,
                            con["xpath"]?.GetValue<string>() ?? string.Empty,
                            isBestPractice,
                            explanation));
                    }
                }

                // traverse all elements to flag proper 'differential' tags on elements
                foreach (JsonNode dif in sd["differential"]["element"].AsArray())
                {
                    string difPath = dif["path"].GetValue<string>();
                    string difSliceName = dif["sliceName"]?.GetValue<string>() ?? string.Empty;

                    if (complex.Elements.ContainsKey(difPath))
                    {
                        complex.Elements[difPath].SetInDifferential();

                        if ((!string.IsNullOrEmpty(difSliceName)) &&
                            (complex.Elements[difPath].Slicing != null) &&
                            complex.Elements[difPath].Slicing.ContainsKey(sdUrl) &&
                            complex.Elements[difPath].Slicing[sdUrl].HasSlice(difSliceName))
                        {
                            complex.Elements[difPath].Slicing[sdUrl].SetInDifferential(difSliceName);
                        }
                    }
                }
            }

            switch (artifactClass)
            {
                case FhirArtifactClassEnum.ComplexType:
                    fhirVersionInfo.AddComplexType(complex);
                    break;
                case FhirArtifactClassEnum.Resource:
                    fhirVersionInfo.AddResource(complex);
                    break;
                case FhirArtifactClassEnum.Extension:
                    fhirVersionInfo.AddExtension(complex);
                    break;
                case FhirArtifactClassEnum.Profile:
                    fhirVersionInfo.AddProfile(complex);
                    break;
                case FhirArtifactClassEnum.LogicalModel:
                    fhirVersionInfo.AddLogicalModel(complex);
                    break;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(string.Empty);
            Console.WriteLine($"FromR5.ProcessComplex <<< SD: {sdName} ({sdId}) - exception: {ex.Message}");
            throw;
        }
    }

    /// <summary>Reads a nested.</summary>
    /// <param name="node">The node.</param>
    /// <param name="key"> The key.</param>
    /// <returns>The nested.</returns>
    private static object ReadNested(
        JsonNode node,
        string key)
    {
        if (!_nestedElementChoices.ContainsKey(key))
        {
            return node.AsObject();
        }

        ExpandoObject o = new();

        foreach (ElementChoiceInfo e in _nestedElementChoices[key])
        {
            if (node[e.Literal] == null)
            {
                continue;
            }

            switch (e.ReadType)
            {
                case ReadTypeCodes.ByteArray:
                    o.TryAdd(e.Literal, node[e.Literal].GetValue<byte[]>());
                    break;
                case ReadTypeCodes.Boolean:
                    o.TryAdd(e.Literal, node[e.Literal].GetValue<bool>());
                    break;
                case ReadTypeCodes.Decimal:
                    o.TryAdd(e.Literal, node[e.Literal].GetValue<decimal>());
                    break;
                case ReadTypeCodes.String:
                    o.TryAdd(e.Literal, node[e.Literal].GetValue<string>());
                    break;
                case ReadTypeCodes.StringArray:
                    o.TryAdd(e.Literal, node[e.Literal].AsArray().Select(n => n.GetValue<string>()));
                    break;
                case ReadTypeCodes.Integer:
                    o.TryAdd(e.Literal, node[e.Literal].GetValue<int>());
                    break;
                case ReadTypeCodes.Long:
                    o.TryAdd(e.Literal, node[e.Literal].GetValue<long>());
                    break;
                case ReadTypeCodes.Nested:
                    o.TryAdd(e.Literal, ReadNested(node[e.Literal], e.Literal));
                    break;
                case ReadTypeCodes.NestedArray:
                    o.TryAdd(e.Literal, node[e.Literal].AsArray().Select(n => ReadNested(n, e.Literal)));
                    break;
                default:
                    break;
            }
        }

        return o;
    }

    /// <summary>Gets default value if present.</summary>
    /// <param name="element">The element.</param>
    /// <param name="prefix"> The prefix (e.g., defaultValue, fixedValue, minValue, value).</param>
    /// <param name="name">   [out] The default name.</param>
    /// <param name="value">  [out] The default value.</param>
    private static void GetValueIfPresent(
        JsonNode element,
        string prefix,
        out string name,
        out object value)
    {
        foreach (ElementChoiceInfo e in _openTypeChoices)
        {
            if (element[prefix + e.Literal] != null)
            {
                name = prefix + e.Literal;

                switch (e.ReadType)
                {
                    case ReadTypeCodes.ByteArray:
                        value = element[name].GetValue<byte[]>();
                        break;
                    case ReadTypeCodes.Boolean:
                        value = element[name].GetValue<bool>();
                        break;
                    case ReadTypeCodes.Decimal:
                        value = element[name].GetValue<decimal>();
                        break;
                    case ReadTypeCodes.String:
                        value = element[name].GetValue<string>();
                        break;
                    case ReadTypeCodes.StringArray:
                        value = element[name].AsArray().Select(n => n.GetValue<string>());
                        break;
                    case ReadTypeCodes.Integer:
                        value = element[name].GetValue<int>();
                        break;
                    case ReadTypeCodes.Long:
                        value = element[name].GetValue<long>();
                        break;
                    case ReadTypeCodes.Nested:
                        value = ReadNested(element[name], e.NestKey);
                        break;
                    case ReadTypeCodes.NestedArray:
                        value = element[name].AsArray().Select(n => ReadNested(n, e.NestKey));
                        break;
                    default:
                        value = element[name].AsObject();
                        break;
                }

                return;
            }
        }

        name = string.Empty;
        value = null;
    }

    /// <summary>Attempts to get type from elements.</summary>
    /// <param name="structureName">Name of the structure.</param>
    /// <param name="elements">     The elements node.</param>
    /// <param name="elementTypes"> [out] Type of the element.</param>
    /// <param name="regex">        [out] The RegEx.</param>
    /// <param name="isSimple">     [out] True if is simple, false if not.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    private static bool TryGetTypeFromElements(
        string structureName,
        JsonNode elements,
        out Dictionary<string, FhirElementType> elementTypes,
        out string regex,
        out bool isSimple)
    {
        elementTypes = null;
        regex = string.Empty;
        isSimple = false;

        foreach (JsonNode element in elements.AsArray())
        {
            // split the path
            string[] components = element["path"].GetValue<string>().Split('.');

            // check for base path having a type
            if (components.Length == 1)
            {
                if (TryGetTypeFromElement(structureName, element, out elementTypes, out regex, out isSimple))
                {
                    // done searching
                    return true;
                }
            }

            // check for path {type}.value having a type
            if ((components.Length == 2) &&
                components[1].Equals("value", StringComparison.Ordinal))
            {
                if (TryGetTypeFromElement(structureName, element, out elementTypes, out regex, out isSimple))
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

    /// <summary>Gets type from element.</summary>
    /// <param name="structureName">Name of the structure.</param>
    /// <param name="element">      The element.</param>
    /// <param name="elementTypes"> [out] Type of the element.</param>
    /// <param name="regex">        [out] The RegEx.</param>
    /// <param name="isSimple">     [out] True if is simple, false if not.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    private static bool TryGetTypeFromElement(
        string structureName,
        JsonNode element,
        out Dictionary<string, FhirElementType> elementTypes,
        out string regex,
        out bool isSimple)
    {
        string elementId = element["id"]?.GetValue<string>() ?? string.Empty;
        string elementPath = element["path"].GetValue<string>();

        elementTypes = new Dictionary<string, FhirElementType>();
        regex = string.Empty;
        isSimple = false;

        // TODO(ginoc): 5.0.0-snapshot1 needs these fixed
        switch (elementPath)
        {
            case "ArtifactAssessment.approvalDate":
            case "ArtifactAssessment.lastReviewDate":
                if (element["type"]?[0]?["code"]?.GetValue<string>() != "date")
                {
                    elementTypes.Add("date", new FhirElementType("date"));
                    _warnings.Add($"StructureDefinition - {structureName} coerced {elementId} to type 'date'");
                    return true;
                }

                break;
        }

        // check for declared type
        if (element["type"] != null)
        {
            string fType;

            foreach (JsonNode edType in element["type"].AsArray())
            {
                regex = edType["extension"]?.AsArray()
                    .FirstOrDefault((ext) => ext["url"].GetValue<string>().Equals("http://hl7.org/fhir/StructureDefinition/regex", StringComparison.Ordinal), null)
                    ?["valueString"]?.GetValue<string>() ?? string.Empty;

                JsonNode typeNode = edType["extension"]?.AsArray()
                    .FirstOrDefault((ext) => ext["url"].GetValue<string>().Equals("http://hl7.org/fhir/StructureDefinition/structuredefinition-fhir-type", StringComparison.Ordinal), null);

                fType =
                    typeNode?["valueUrl"]?.GetValue<string>()
                    ?? typeNode?["valueString"]?.GetValue<string>()
                    ?? string.Empty;

                IEnumerable<string> elementTargets;
                IEnumerable<string> elementProfiles;

                if (edType["targetProfile"] == null)
                {
                    elementTargets = Array.Empty<string>();
                }
                else if (edType["targetProfile"].GetType() == typeof(JsonArray))
                {
                    // R4 and later, array type
                    elementTargets = edType["targetProfile"].AsArray().Select(n => n.GetValue<string>());
                }
                else
                {
                    // R3 and earlier, scalar type
                    elementTargets = new string[] { edType["targetProfile"].GetValue<string>() };
                }

                if (edType["profile"] == null)
                {
                    elementProfiles = Array.Empty<string>();
                }
                else if (edType["profile"].GetType() == typeof(JsonArray))
                {
                    // R4 and later, array type
                    elementProfiles = edType["profile"].AsArray().Select(n => n.GetValue<string>());
                }
                else
                {
                    // R3 and earlier, scalar type
                    elementProfiles = new string[] { edType["profile"].GetValue<string>() };
                }

                if (!string.IsNullOrEmpty(fType))
                {
                    // create a type for this code
                    FhirElementType elementType = new FhirElementType(
                        fType,
                        elementTargets,
                        elementProfiles);

                    isSimple = true;

                    // add to our dictionary
                    elementTypes.Add(elementType.Name, elementType);
                }
                else if (!string.IsNullOrEmpty(edType["code"]?.GetValue<string>()))
                {
                    // create a type for this code
                    FhirElementType elementType = new FhirElementType(
                        edType["code"].GetValue<string>(),
                        elementTargets,
                        elementProfiles);

                    if (elementTypes.ContainsKey(elementType.Name))
                    {
                        // R3 and earlier add profiles/targets by repeating elements, but will only have a single value in the array
                        if (elementTargets.Any())
                        {
                            elementTypes[elementType.Name].AddProfile(elementTargets.First());
                        }

                        if (elementProfiles.Any())
                        {
                            elementTypes[elementType.Name].AddTypeProfile(elementProfiles.First());
                        }
                    }
                    else
                    {
                        // add to our dictionary
                        elementTypes.Add(elementType.Name, elementType);
                    }
                }
            }
        }

        if (elementTypes.Count > 0)
        {
            return true;
        }

        // check for base derived type
        if (string.IsNullOrEmpty(elementId) ||
            elementId.Equals(structureName, StringComparison.Ordinal))
        {
            // base type is here
            FhirElementType elementType = new FhirElementType(elementPath);

            // add to our dictionary
            elementTypes.Add(elementType.Name, elementType);

            // done searching
            return true;
        }

        // no discovered type
        elementTypes = null;
        return false;
    }

    /// <summary>Displays the issues.</summary>
    void IFhirConverter.DisplayIssues()
    {
#pragma warning disable CA1303 // Do not pass literals as localized parameters
        Console.WriteLine("Errors (only able to pass with manual code changes)");

        foreach (string value in _errors)
        {
            Console.WriteLine($" - {value}");
        }

        Console.WriteLine("Warnings (able to pass, but should be reviewed)");

        foreach (string value in _warnings)
        {
            Console.WriteLine($" - {value}");
        }
#pragma warning restore CA1303 // Do not pass literals as localized parameters
    }

    /// <summary>Query if 'errorCount' has issues.</summary>
    /// <param name="errorCount">  [out] Number of errors.</param>
    /// <param name="warningCount">[out] Number of warnings.</param>
    /// <returns>True if issues, false if not.</returns>
    bool IFhirConverter.HasIssues(out int errorCount, out int warningCount)
    {
        errorCount = _errors.Count;
        warningCount = _warnings.Count;

        return (errorCount > 0) || (warningCount > 0);
    }

    /// <summary>Parses resource an object from the given string.</summary>
    /// <param name="json">The JSON.</param>
    /// <returns>A typed Resource object.</returns>
    object IFhirConverter.ParseResource(string json)
    {
        try
        {
            // try to parse this JSON
            return JsonNode.Parse(json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"FromCorePackage.ParseResource <<< failed to parse:\n{ex}\n------------------------------------");
            throw;
        }
    }

    /// <summary>Attempts to process resource.</summary>
    /// <param name="resourceToParse">The resource object.</param>
    /// <param name="fhirVersionInfo">Information describing the FHIR version.</param>
    void IFhirConverter.ProcessResource(object resourceToParse, IPackageImportable fhirVersionInfo)
    {
        switch ((resourceToParse as JsonNode)!["resourceType"]?.GetValue<string>() ?? string.Empty)
        {
            case "CodeSystem":
                ProcessCodeSystem(resourceToParse as JsonNode, fhirVersionInfo);
                break;

            case "OperationDefinition":
                ProcessOperation(resourceToParse as JsonNode, fhirVersionInfo);
                break;

            case "SearchParameter":
                ProcessSearchParam(resourceToParse as JsonNode, fhirVersionInfo);
                break;

            case "ValueSet":
                ProcessValueSet(resourceToParse as JsonNode, fhirVersionInfo);
                break;

            case "StructureDefinition":
                ProcessStructureDef(resourceToParse as JsonNode, fhirVersionInfo);
                break;

        }
    }

    void IFhirConverter.ProcessMetadata(object metadata, string serverUrl, out FhirServerInfo serverInfo) => throw new NotImplementedException();
}
