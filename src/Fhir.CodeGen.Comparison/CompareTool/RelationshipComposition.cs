using Fhir.CodeGen.Comparison.Models;
using CMR = Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship;

namespace Fhir.CodeGen.Comparison.CompareTool;

/// <summary>
/// Logic for composing relationships when chaining mappings
/// </summary>
public static class RelationshipComposition
{
    public static CMR ComputeForDomains(CMR? cdRelationship, CMR? vdRelationship) => cdRelationship switch
    {
        CMR.Equivalent => vdRelationship ?? CMR.Equivalent,
        CMR.RelatedTo => (vdRelationship == CMR.NotRelatedTo) ? CMR.NotRelatedTo : CMR.RelatedTo,
        CMR.SourceIsNarrowerThanTarget => (vdRelationship == CMR.SourceIsNarrowerThanTarget || vdRelationship == CMR.Equivalent)
            ? CMR.SourceIsNarrowerThanTarget : CMR.RelatedTo,
        CMR.SourceIsBroaderThanTarget => (vdRelationship == CMR.SourceIsBroaderThanTarget || vdRelationship == CMR.Equivalent)
            ? CMR.SourceIsBroaderThanTarget : CMR.RelatedTo,
        CMR.NotRelatedTo => vdRelationship ?? CMR.NotRelatedTo,
        _ => vdRelationship ?? cdRelationship ?? CMR.NotRelatedTo,
    };

    /// <summary>
    /// Composes two relationships using "worst case" semantics.
    /// The most restrictive relationship in the chain determines the overall result.
    /// </summary>
    /// <remarks>
    /// Composition rules:
    /// - null (unmapped) dominates: any + null = null
    /// - NotRelatedTo is next worst: any + NotRelatedTo = NotRelatedTo
    /// - RelatedTo catches mismatches: Broader + Narrower = RelatedTo
    /// - Equivalent passes through: Equivalent + X = X
    /// - Same direction compounds: Broader + Broader = Broader
    /// </remarks>
    public static CMR? Compose(CMR? first, CMR? second)
    {
        // Unmapped dominates
        if (first == null || second == null)
            return null;

        // NotRelatedTo is terminal
        if (first == CMR.NotRelatedTo || second == CMR.NotRelatedTo)
            return CMR.NotRelatedTo;

        // RelatedTo stays RelatedTo
        if (first == CMR.RelatedTo || second == CMR.RelatedTo)
            return CMR.RelatedTo;

        // Equivalent is identity
        if (first == CMR.Equivalent)
            return second;
        if (second == CMR.Equivalent)
            return first;

        // Both are directional (Broader or Narrower)
        // Same direction compounds
        if (first == second)
            return first;

        // Opposite directions become RelatedTo
        // (Broader->Narrower or Narrower->Broader)
        return CMR.RelatedTo;
    }

    /// <summary>
    /// Composes an entire chain of relationships
    /// </summary>
    public static CMR? ComposeChain(IEnumerable<CMR?> relationships)
    {
        CMR? result = CMR.Equivalent; // Start with identity
        foreach (var rel in relationships)
        {
            result = Compose(result, rel);
            // Early exit if we hit unmapped or NotRelatedTo
            if (result == null || result == CMR.NotRelatedTo)
                break;
        }
        return result;
    }

    /// <summary>
    /// Composes ChangeIndicationCodes across a chain
    /// </summary>
    public static ChangeIndicationCodes? ComposeChanges(IEnumerable<ChangeIndicationCodes?> changes)
    {
        bool hasNarrowed = false;
        bool hasBroadened = false;

        foreach (var change in changes.Where(c => c.HasValue))
        {
            switch (change!.Value)
            {
                case ChangeIndicationCodes.Narrowed:
                    hasNarrowed = true;
                    break;
                case ChangeIndicationCodes.Broadened:
                    hasBroadened = true;
                    break;
            }
        }

        if (hasNarrowed && hasBroadened)
            return ChangeIndicationCodes.NoChange; // Cancelled out (or could be "Changed")

        if (hasNarrowed)
            return ChangeIndicationCodes.Narrowed;

        if (hasBroadened)
            return ChangeIndicationCodes.Broadened;

        return ChangeIndicationCodes.NoChange;
    }
}
