// <copyright file="StructureDefSlicing.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using Hl7.Fhir.Model;
using Hl7.Fhir.Utility;
using Microsoft.Health.Fhir.CodeGen.Models;

namespace Microsoft.Health.Fhir.CodeGen.FhirExtensions;

public static class StructureDefSlicing
{
    /// <summary>Enumerates cg elements for slice in this collection.</summary>
    /// <param name="sd">         The SD to act on.</param>
    /// <param name="slicingEd">  The slicing ed.</param>
    /// <param name="sliceName">  Name of the slice.</param>
    /// <param name="includeRoot">(Optional) True to include, false to exclude the root.</param>
    /// <returns>
    /// An enumerator that allows foreach to be used to process cg elements for slice in this
    /// collection.
    /// </returns>
    public static IEnumerable<ElementDefinition> cgElementsForSlice(
        this StructureDefinition sd,
        ElementDefinition slicingEd,
        string sliceName,
        bool includeRoot = true)
    {
        string sliceId = $"{slicingEd.Path}:{sliceName}";
        return (sd.Snapshot?.Element.Any() ?? false)
            ? sd.Snapshot.Element.Where(e => e.ElementId.StartsWith(sliceId, StringComparison.Ordinal)).Skip(includeRoot ? 0 : 1)
            : sd.Differential.Element.Where(e => e.ElementId.StartsWith(sliceId, StringComparison.Ordinal)).Skip(includeRoot ? 0 : 1);
    }

    public static IEnumerable<(string type, string path, string value)> cgDiscriminatedValues(
        this StructureDefinition sd,
        ElementDefinition slicingEd,
        string sliceName,
        IEnumerable<ElementDefinition> sliceElements)
    {
        List<(string type, string path, string value)> result = new();

        if ((slicingEd.Slicing == null) ||
            (!slicingEd.Slicing.Discriminator.Any()))
        {
            return result;
        }

        foreach (ElementDefinition.DiscriminatorComponent discriminator in slicingEd.Slicing.Discriminator)
        {
            switch (discriminator.Type)
            {
                case ElementDefinition.DiscriminatorType.Value:
                    {
                        ElementDefinition? valueEd = sliceElements.Where(e => e.ElementId.Equals($"{slicingEd.Path}:{sliceName}.{discriminator.Path}", StringComparison.Ordinal)).FirstOrDefault();

                        if (valueEd != null)
                        {
                            if (valueEd.Fixed != null)
                            {
                                result.Add((discriminator.Type.GetLiteral()!, valueEd.Path, valueEd.Fixed.ToString() ?? string.Empty));
                            }
                            else if (valueEd.Pattern != null)
                            {
                                result.Add((discriminator.Type.GetLiteral()!, valueEd.Path, valueEd.Pattern.ToString() ?? string.Empty));
                            }
                        }
                        // check for extension URL, it is represented oddly
                        else if (discriminator.Path.Equals("url", StringComparison.Ordinal))
                        {
                            valueEd = sliceElements.Where(e => e.ElementId.Equals($"{slicingEd.Path}:{sliceName}", StringComparison.Ordinal)).FirstOrDefault();

                            if (valueEd != null)
                            {
                                string extUrl = valueEd.Type?.FirstOrDefault()?.Profile.FirstOrDefault() ?? string.Empty;

                                if (!string.IsNullOrEmpty(extUrl))
                                {
                                    result.Add((discriminator.Type.GetLiteral()!, valueEd.Path, extUrl));
                                }
                            }
                        }
                    }
                    break;

                //case ElementDefinition.DiscriminatorType.Pattern:
                //    ElementDefinition? ePattern = sliceElements
                //        .Where(e => e.Pattern != null)
                //        .Where(e => e.ElementId.StartsWith($"{slicingEd.Path}:{sliceName}", StringComparison.Ordinal))
                //        .FirstOrDefault();

                //    if (ePattern != null)
                //    {
                //        result.Add((discriminator.Type.ToString(), discriminator.Path, ePattern.Pattern.ToString()));
                //    }

                //    break;

                //case ElementDefinition.DiscriminatorType.Profile:
                //    ElementDefinition? eProfile = sliceElements
                //        .Where(e => e.Type.Any(t => t.Profile.Any()))
                //        .Where(e => e.ElementId.StartsWith($"{slicingEd.Path}:{sliceName}", StringComparison.Ordinal))
                //        .FirstOrDefault();

                //    if (eProfile != null)
                //    {
                //        result.Add((discriminator.Type.ToString(), discriminator.Path, eProfile.Type.FirstOrDefault()?.Profile?.First()));
                //    }

                //    break;

                //default:
                //    break;
            }
        }

        return result;
    }
}
