// <copyright file="FhirRelatedArtifact.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System.Diagnostics.CodeAnalysis;
using Microsoft.Health.Fhir.CodeGenCommon.Extensions;

namespace Microsoft.Health.Fhir.CodeGenCommon.Instance;

/// <summary>
/// A FHIR related artifact.
/// Note that this class is not complete. I do not want to undo the work, but realize it is
/// not necessary yet. I am leaving it here for future use.
/// </summary>
public record class FhirRelatedArtifact : FhirElementBase, ICloneable
{
    /// <summary>Values that represent related artifact type codes.</summary>
    public enum RelatedArtifactTypeCodes
    {
        /// <summary>
        /// Additional documentation related to the artifact.
        /// </summary>
        [FhirLiteral("documentation")]
        Documentation,
        /// <summary>
        /// Justification and/or supporting evidence associated with the artifact.
        /// </summary>
        [FhirLiteral("justification")]
        Justification,
        /// <summary>
        /// Bibliographic citation for the artifact.
        /// </summary>
        [FhirLiteral("citation")]
        Citation,
        /// <summary>
        /// An artifact that precedes this artifact (e.g. a predecessor to a QuestionnaireResponse).
        /// </summary>
        [FhirLiteral("predecessor")]
        Predecessor,
        /// <summary>
        /// An artifact that follows this artifact (e.g. a successor to a QuestionnaireResponse).
        /// </summary>
        [FhirLiteral("successor")]
        Successor,
        /// <summary>
        /// A related artifact that is derived from this artifact (e.g. a translated copy of a Questionnaire).
        /// </summary>
        [FhirLiteral("derived-from")]
        DerivedFrom,
        /// <summary>
        /// An artifact that this artifact depends on (e.g. a QuestionnaireResponse that provides an answer to the Questionnaire).
        /// </summary>
        [FhirLiteral("depends-on")]
        DependsOn,
        /// <summary>
        /// An artifact that this artifact is composed of (e.g. a Bundle that contains this resource).
        /// </summary>
        [FhirLiteral("composed-of")]
        ComposedOf,
        /// <summary>
        /// An artifact that this artifact is part of (e.g. a Composition that contains this section).
        /// </summary>
        [FhirLiteral("part-of")]
        PartOf,
        /// <summary>
        /// An artifact that this artifact amends (e.g. a Questionnaire that updates a previous Questionnaire).
        /// </summary>
        [FhirLiteral("amends")]
        Amends,
        /// <summary>
        /// An artifact that is amended by this artifact (e.g. a Questionnaire that is updated by a newer Questionnaire).
        /// </summary>
        [FhirLiteral("amended-with")]
        AmendedWith,
        /// <summary>
        /// An artifact that this artifact is an appendage to (e.g. a report or questionnaire that includes answers or suggested answers).
        /// </summary>
        [FhirLiteral("appends")]
        Appends,
        /// <summary>
        /// An artifact that is an appendage to this artifact (e.g. a response to a questionnaire that includes answers or suggested answers).
        /// </summary>
        [FhirLiteral("appended-with")]
        AppendedWith,
        /// <summary>
        /// An artifact that is cited by this artifact (e.g. a reference document, citation, or guideline).
        /// </summary>
        [FhirLiteral("cites")]
        Cites,
        /// <summary>
        /// An artifact that cites this artifact (e.g. a reference document, citation, or guideline).
        /// </summary>
        [FhirLiteral("cited-by")]
        CitedBy,
        /// <summary>
        /// A comment about this artifact, separate from the description.
        /// </summary>
        [FhirLiteral("comments-on")]
        CommentsOn,
        /// <summary>
        /// The location within the artifact that the comment is related to.
        /// </summary>
        [FhirLiteral("comment-in")]
        CommentIn,
        /// <summary>
        /// An artifact that this artifact contains (e.g. a Bundle that contains this resource).
        /// </summary>
        [FhirLiteral("contains")]
        Contains,
        /// <summary>
        /// An artifact that contains this artifact (e.g. a Composition that contains this section).
        /// </summary>
        [FhirLiteral("contained-in")]
        ContainedIn,
        /// <summary>
        /// An artifact that this artifact corrects (e.g. a document that corrects a previous version).
        /// </summary>
        [FhirLiteral("corrects")]
        Corrects,
        /// <summary>
        /// An artifact that this artifact provides a correction for (e.g. a correcting document for a previous version).
        /// </summary>
        [FhirLiteral("correction-in")]
        CorrectionIn,
        /// <summary>
        /// An artifact that this artifact replaces (e.g. a document that replaces a previous version).
        /// </summary>
        [FhirLiteral("replaces")]
        Replaces,
        /// <summary>
        /// An artifact that is replaced by this artifact (e.g. a previous version of a document that is replaced by a newer version).
        /// </summary>
        [FhirLiteral("replaced-with")]
        ReplacedWith,
        /// <summary>
        /// An artifact that this artifact retracts (e.g. a correcting document that retracts a previous version).
        /// </summary>
        [FhirLiteral("retracts")]
        Retracts,
        /// <summary>
        /// An artifact that is retracted by this artifact (e.g. a previous version of a correcting document that is retracted by a newer version).
        /// </summary>
        [FhirLiteral("retracted-by")]
        RetractedBy,
        /// <summary>
        /// The signature on the artifact.
        /// </summary>
        [FhirLiteral("signs")]
        Signs,
        /// <summary>
        /// An artifact that is similar to this artifact (e.g. a translated copy of a QuestionnaireResponse).
        /// </summary>
        [FhirLiteral("similar-to")]
        SimilarTo,
        /// <summary>
        /// An artifact that this artifact supports (either supports or provides required information for).
        /// </summary>
        [FhirLiteral("supports")]
        Supports,
        /// <summary>An artifact that contains additional information related to the knowledge artifact but is not documentation as the additional information does not describe, explain, or instruct regarding the knowledge artifact content or application.</summary>
        [FhirLiteral("supported-with")]
        SupportedWith,
        /// <summary>
        /// An artifact that this artifact is a transformation of (e.g. a message that is converted into a different format).
        /// </summary>
        [FhirLiteral("transforms")]
        Transforms,
        /// <summary>
        /// An artifact that transforms this artifact (e.g. a message that is converted into a different format).
        /// </summary>
        [FhirLiteral("transformed-into")]
        TransformedInto,
        /// <summary>
        /// An artifact that is transformed by this artifact (e.g. a message that is converted into a different format).
        /// </summary>
        [FhirLiteral("transformed-with")]
        TransformedWith,
        /// <summary>
        /// An artifact that this artifact documents (e.g. a operational status report).
        /// </summary>
        [FhirLiteral("documents")]
        Documents,
        /// <summary>
        /// An artifact that this artifact and implementations of it should conform to (e.g. an interface definition or a constraint statement).
        /// </summary>
        [FhirLiteral("specification-of")]
        SpecificationOf,
        /// <summary>
        /// The tool used to create the artifact.
        /// </summary>
        [FhirLiteral("created-with")]
        CreatedWith,
        /// <summary>
        /// The act of creating a bibliographic record for citation.
        /// </summary>
        [FhirLiteral("cite-as")]
        CiteAs,
    }

    private RelatedArtifactTypeCodes _relationType = RelatedArtifactTypeCodes.Cites;
    private string _relationTypeLiteral = string.Empty;

    /// <summary>Initializes a new instance of the FhirRelatedArtifact class.</summary>
    public FhirRelatedArtifact() : base() { }

    /// <summary>Initializes a new instance of the FhirRelatedArtifact class.</summary>
    /// <param name="other">The other.</param>
    [SetsRequiredMembers]
    protected FhirRelatedArtifact(FhirRelatedArtifact other)
        : base(other)
    {
        RelationTypeLiteral = other.RelationTypeLiteral;
        Classifiers = other.Classifiers.Select(v => v with { });
        Label = other.Label;
        Display = other.Display;
        Citation = other.Citation;
    }

    /// <summary>Gets the type of the relation.</summary>
    public RelatedArtifactTypeCodes RelationType { get => _relationType; }

    /// <summary>Gets or initializes the type of the relation type literal.</summary>
    public required string RelationTypeLiteral
    {
        get => _relationTypeLiteral;
        init
        {
            _relationTypeLiteral = value;
            _relationType = value.ToEnum<RelatedArtifactTypeCodes>() ?? RelatedArtifactTypeCodes.Cites;
        }
    }

    /// <summary>Gets or initializes the classifiers.</summary>
    public IEnumerable<FhirCodeableConcept> Classifiers { get; init; } = Enumerable.Empty<FhirCodeableConcept>();

    /// <summary>Gets or initializes the label.</summary>
    public string Label { get; init; } = string.Empty;

    /// <summary>Gets or initializes the display.</summary>
    public string Display { get; init; } = string.Empty;

    /// <summary>Gets or initializes the citation.</summary>
    public string Citation { get; init; } = string.Empty;

    /// <summary>Makes a deep copy of this object.</summary>
    /// <returns>A copy of this object.</returns>
    object ICloneable.Clone() => this with { };
}
