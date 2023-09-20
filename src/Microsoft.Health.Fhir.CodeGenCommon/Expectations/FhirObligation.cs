// <copyright file="FhirObligation.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System.Diagnostics.CodeAnalysis;
using Microsoft.Health.Fhir.CodeGenCommon.BaseModels;
using Microsoft.Health.Fhir.CodeGenCommon.Extensions;
using Microsoft.Health.Fhir.CodeGenCommon.Instance;

namespace Microsoft.Health.Fhir.CodeGenCommon.Expectations;

/// <summary>
/// A FHIR Obligation.
/// Note this is based on the CI Build from the FHIR Extensions Pack:
/// https://build.fhir.org/ig/HL7/fhir-extensions/ValueSet-obligation.html
/// </summary>
public record class FhirObligation : FhirBase, ICloneable
{
    public const string ObligationExtUrl = "http://hl7.org/fhir/StructureDefinition/obligation";            // from ExtensionPack
    //public const string ObligationExtUrl = "http://hl7.org/fhir/tools/StructureDefinition/obligation";    // from ToolsIG
    public const string ObligationCodeExtUrl = "code";
    public const string ObligationElementIdExtUrl = "elementId";
    public const string ObligationActorExtUrl = "actor";
    public const string ObligationDocumentationExtUrl = "documentation";
    public const string ObligationUsageExtUrl = "usage";
    public const string ObligationFilterExtUrl = "filter";
    public const string ObligationFilterDocumentationExtUrl = "filterDocumentation";
    public const string ObligationProcessExtUrl = "process";

    private IEnumerable<string> _obligationCodeLiteral = Enumerable.Empty<string>();
    private IEnumerable<ObligationCodes> _obligations = Enumerable.Empty<ObligationCodes>();

    /// <summary>Values that represent obligation codes (http://hl7.org/fhir/tools/ValueSet/obligation).</summary>
    public enum ObligationCodes
    {
        /// <summary>No obligation or conformance expectation has been specified.</summary>
        [FhirLiteral("")]
        NotSpecified,

        /// <summary>SHALL/SHOULD/MAY that can be prepended to codes that have the qualify property = true (level 1).</summary>
        [FhirLiteral("ModifierCodes")]
        ModifierCodes,

        /// <summary>
        /// The functional requirement is mandatory. Applications that do not implement this functional behavior are considered
        /// non-conformant (level 2).
        /// </summary>
        [FhirLiteral("SHALL")]
        Shall,

        /// <summary>The functional requirement is a recommendation (level 2).</summary>
        [FhirLiteral("SHOULD")]
        Should,

        /// <summary>
        /// The functional requirement is presented as an option for applications to consider. Note that this is usually used to
        /// indicate a choice is still valid for an application to make (level 2).
        /// </summary>
        [FhirLiteral("MAY")]
        May,

        /// <summary>
        /// Resource producers are applications that assembles resources in the first place. Resource producers may be a server,
        /// client, sender, receiver or some middleware device, and they may store the resource, or merely hand it on (level 1).
        /// </summary>
        [FhirLiteral("ResourceProducerObligations")]
        ResourceProducerObligations,

        [FhirLiteral("can-send")]
        CanSend,

        [FhirLiteral("will-send")]
        WillSend,

        [FhirLiteral("use-reason")]
        UseReason,

        [FhirLiteral("in-narrative")]
        InNarrative,

        [FhirLiteral("in-ui")]
        InUi,

        [FhirLiteral("in-store")]
        InStore,

        [FhirLiteral("must-explain")]
        MustExplain,

        [FhirLiteral("ExchangerObligations")]
        ExchangerObligations,

        [FhirLiteral("unaltered")]
        Unaltered,

        [FhirLiteral("may-alter")]
        MayAlter,

        [FhirLiteral("ResourceConsumerObligations")]
        ResourceConsumerObligations,

        [FhirLiteral("no-error")]
        NoError,

        [FhirLiteral("handle")]
        Handle,

        [FhirLiteral("display")]
        Display,

        [FhirLiteral("share")]
        Share,

        [FhirLiteral("process")]
        Process,

        [FhirLiteral("store")]
        Store,

        [FhirLiteral("print")]
        Print,

        [FhirLiteral("AggregateConcepts")]
        AggregateConcepts,

        [FhirLiteral("can-populate")]
        CanPopulate,

        [FhirLiteral("can-ignore")]
        CanIgnore,

        /// <summary>RE as defined in chapter 2 of the v2 specification.</summary>
        [FhirLiteral("v2-re")]
        IfKnownV2,

        /// <summary>R2 as defined in IHE Appendix Z.</summary>
        [FhirLiteral("ihe-r2")]
        IfKnownIHE,

        /// <summary>
        /// The standard recommended set of obligations for IGs to use unless they know they want something
        /// different. Note that the standard codes don't include use-dar, use-dar-nf, use-dar-if-allowed /
        /// use-reason - that gets put on specific elements that justify this handling.
        /// </summary>
        [FhirLiteral("std")]
        Standard,
    }

    /// <summary>Initializes a new instance of the <see cref="FhirObligation"/> class.</summary>
    public FhirObligation() { }

    /// <summary>Initializes a new instance of the <see cref="FhirObligation"/> class.</summary>
    /// <param name="other">The other.</param>
    [SetsRequiredMembers]
    protected FhirObligation(FhirObligation other)
    {
        InternalId = other.InternalId;
        ObligationCodeLiteral = other.ObligationCodeLiteral.Select(o => o);
        Actors = other.Actors.Select(v => v with { });
        Documentation = other.Documentation;
        UsageContexts = other.UsageContexts.Select(v => v with { });
    }

    /// <summary>Gets the identifier of the internal.</summary>
    public string InternalId { get; init; } = Guid.NewGuid().ToString();

    /// <summary>Gets the obligations.</summary>
    public IEnumerable<ObligationCodes> Obligations => _obligations;

    /// <summary>Gets or initializes the FHIR obligation code.</summary>
    public required IEnumerable<string> ObligationCodeLiteral
    {
        get => _obligationCodeLiteral;
        init
        {
            if (value == null)
            {
                return;
            }

            _obligationCodeLiteral = value;
            _obligations = value.ToEnum<ObligationCodes>(ObligationCodes.NotSpecified);
        }
    }

    /// <summary>Gets or initializes the actors.</summary>
    public IEnumerable<FhirActor> Actors { get; init; } = Enumerable.Empty<FhirActor>();

    /// <summary>Gets or initializes the documentation.</summary>
    public string Documentation { get; init; } = string.Empty;

    /// <summary>Gets or initializes the usage contexts.</summary>
    public IEnumerable<FhirUsageContext> UsageContexts { get; init; } = Enumerable.Empty<FhirUsageContext>();

    /// <summary>Makes a deep copy of this object.</summary>
    /// <returns>A copy of this object.</returns>
    object ICloneable.Clone() => this with { };
}
