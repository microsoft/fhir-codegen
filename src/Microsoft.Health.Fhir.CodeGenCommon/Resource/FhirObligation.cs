// <copyright file="FhirObligation.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System.Diagnostics.CodeAnalysis;
using Microsoft.Health.Fhir.CodeGenCommon.Extensions;

namespace Microsoft.Health.Fhir.CodeGenCommon.Resource;

/// <summary>A FHIR obligation.</summary>
public record class FhirObligation : ICloneable
{
    public const string ObligationExtUrl = "http://hl7.org/fhir/StructureDefinition/obligation";
    public const string ObligationCodeExtUrl = "code";
    public const string ObligationActorExtUrl = "actor";
    public const string ObligationDocumentationExtUrl = "documentation";
    public const string ObligationUsageExtUrl = "usage";
    public const string ObligationFilterExtUrl = "filter";
    public const string ObligationFilterDocumentationExtUrl = "filterDocumentation";
    public const string ObligationProcessExtUrl = "process";

    private IEnumerable<string> _fhirObligationCode = Enumerable.Empty<string>();
    private IEnumerable<ObligationCodes> _obligations = Enumerable.Empty<ObligationCodes>();

    /// <summary>Values that represent obligation codes.</summary>
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

        [FhirLiteral("ignore")]
        Ignore,

        [FhirLiteral("preserve")]
        Preserve,

        [FhirLiteral("modify")]
        Modify,

        [FhirLiteral("AggregateConcepts")]
        AggregateConcepts,

        [FhirLiteral("can-populate")]
        CanPopulate,

        [FhirLiteral("can-ignore")]
        CanIgnore,

        [FhirLiteral("v2-re")]
        IfKnownV2,

        [FhirLiteral("ihe-r2")]
        IfKnownIHE,

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
        FhirObligationCode = other.FhirObligationCode.Select(o => o);
        Actors = other.Actors.Select(v => v with { });
        Documentation = other.Documentation;
        UsageContexts = other.UsageContexts.Select(v => v with { });
    }

    /// <summary>Gets the identifier of the internal.</summary>
    public string InternalId { get; init; } = Guid.NewGuid().ToString();

    /// <summary>Gets the obligations.</summary>
    public IEnumerable<ObligationCodes> Obligations => _obligations;

    /// <summary>Gets or initializes the FHIR obligation code.</summary>
    public required IEnumerable<string> FhirObligationCode
    {
        get => _fhirObligationCode;
        init
        {
            if (value == null)
            {
                return;
            }

            _fhirObligationCode = value;

            ObligationCodes[] codes = new ObligationCodes[_fhirObligationCode.Count()];

            for (int i = 0; i < codes.Length; i++)
            {
                if (_fhirObligationCode.ElementAt(i).TryFhirEnum(out ObligationCodes v))
                {
                    codes[i] = v;
                }
                else
                {
                    codes[i] = ObligationCodes.NotSpecified;
                }
            }

            _obligations = codes;
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
