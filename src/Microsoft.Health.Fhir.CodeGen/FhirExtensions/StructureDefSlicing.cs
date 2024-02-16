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

    /// <summary>Enumerates cg slices in this collection.</summary>
    /// <param name="sd">         The SD to act on.</param>
    /// <param name="elementPath">Full pathname of the element file.</param>
    /// <param name="sliceNames">     The slices.</param>
    /// <returns>
    /// An enumerator that allows foreach to be used to process cg slices in this collection.
    /// </returns>
    //public static IEnumerable<CodeGenSlice> cgSlices(
    //    this StructureDefinition sd,
    //    ElementDefinition slicingEd,
    //    string[] sliceNames)
    //{
    //    Dictionary<string, CodeGenSlice> result = new();

    //    // filter all elements in the SD down to this element and children
    //    IEnumerable<ElementDefinition> sourceElements = (sd.Snapshot?.Element.Any() ?? false)
    //        ? sd.Snapshot.Element.Where(e => e.Path.StartsWith(slicingEd.Path, StringComparison.Ordinal))
    //        : sd.Differential.Element.Where(e => e.Path.StartsWith(slicingEd.Path, StringComparison.Ordinal));

    //    // grab our discriminators - this may be empty
    //    ElementDefinition.DiscriminatorComponent[] discriminators = slicingEd.Slicing?.Discriminator.ToArray() ?? Array.Empty<ElementDefinition.DiscriminatorComponent>();


    //    int sliceOrder = 0;

    //    foreach (string slice in sliceNames)
    //    {
    //        string sliceId = $"{elementPath}:{slice}";
    //        string bgPath = string.Empty;
    //        string bgValue = string.Empty;

    //        ElementDefinition? eSlicing = source
    //            .Where(e => e.Slicing != null)
    //            .Where(e => e.ElementId.StartsWith(sliceId, StringComparison.Ordinal))
    //            .FirstOrDefault();

    //        ElementDefinition? eFixed = source
    //            .Where(e => e.Fixed != null)
    //            .Where(e => e.ElementId.StartsWith(sliceId, StringComparison.Ordinal))
    //            .FirstOrDefault();

    //        ElementDefinition? ePattern = source
    //            .Where(e => e.Pattern != null)
    //            .Where(e => e.ElementId.StartsWith(sliceId, StringComparison.Ordinal))
    //            .FirstOrDefault();

    //        ElementDefinition? eProfile = source
    //                .Where(e => e.ElementId.StartsWith(sliceId, StringComparison.Ordinal))
    //                .Where(e => e.Type.Any(t => t.Profile.Any()))
    //                .FirstOrDefault();

    //        ElementDefinition.DiscriminatorType? bgType = null;
    //        if (source.First().Type.Any(t => t.Code.Equals("Extension", StringComparison.Ordinal)))
    //        {
    //            bgType = ElementDefinition.DiscriminatorType.Value;
    //            bgPath = "url";
    //        }
    //        else if (eFixed != null)
    //        {
    //            bgType = ElementDefinition.DiscriminatorType.Value;
    //            bgPath = eFixed.Path;
    //        }
    //        else if (ePattern != null)
    //        {
    //            bgType = ElementDefinition.DiscriminatorType.Pattern;
    //            bgPath = ePattern.Path;
    //        }
    //        else if (eProfile != null)
    //        {
    //            bgType = ElementDefinition.DiscriminatorType.Profile;
    //            bgPath = eProfile.Path;
    //        }

    //        bgValue =
    //            eFixed?.Fixed?.ToString()
    //            ?? ePattern?.Pattern?.ToString()
    //            ?? eProfile?.Type.FirstOrDefault()?.Profile?.First()
    //            ?? string.Empty;


    //        result.Add(new()
    //        {
    //            Name = slice,
    //            SliceOrder = sliceOrder++,
    //            SliceElements = source.Where(e => e.ElementId.StartsWith(sliceId, StringComparison.Ordinal)),
    //            SlicingElement = eSlicing,
    //            ExplicitType = eSlicing?.Slicing.Discriminator.FirstOrDefault()?.Type,
    //            ExplicitPath = eSlicing?.Slicing.Discriminator.FirstOrDefault()?.Path ?? string.Empty,
    //            ComputedType = bgType,
    //            ComputedPath = bgPath,
    //            ComputedValue = bgValue,
    //            FixedElement = eFixed,
    //            PatternElement = ePattern,
    //            ProfileElement = eProfile,
    //        });
    //    }

    //    return result;
    //}

    ///// <summary>Enumerates cg slices in this collection.</summary>
    ///// <param name="sd">         The SD to act on.</param>
    ///// <param name="elementPath">Full pathname of the element file.</param>
    ///// <param name="sliceNames">     The slices.</param>
    ///// <returns>
    ///// An enumerator that allows foreach to be used to process cg slices in this collection.
    ///// </returns>
    //public static IEnumerable<CodeGenSlice> cgSlices(
    //    this StructureDefinition sd,
    //    string elementPath,
    //    string[] sliceNames)
    //{
    //    List<CodeGenSlice> result = new();

    //    IEnumerable<ElementDefinition> source = (sd.Snapshot?.Element.Any() ?? false)
    //        ? sd.Snapshot.Element.Where(e => e.Path.StartsWith(elementPath, StringComparison.Ordinal))
    //        : sd.Differential.Element.Where(e => e.Path.StartsWith(elementPath, StringComparison.Ordinal));

    //    int sliceOrder = 0;

    //    foreach (string slice in sliceNames)
    //    {
    //        string sliceId = $"{elementPath}:{slice}";
    //        string bgPath = string.Empty;
    //        string bgValue = string.Empty;

    //        ElementDefinition? eSlicing = source
    //            .Where(e => e.Slicing != null)
    //            .Where(e => e.ElementId.StartsWith(sliceId, StringComparison.Ordinal))
    //            .FirstOrDefault();

    //        ElementDefinition? eFixed = source
    //            .Where(e => e.Fixed != null)
    //            .Where(e => e.ElementId.StartsWith(sliceId, StringComparison.Ordinal))
    //            .FirstOrDefault();

    //        ElementDefinition? ePattern = source
    //            .Where(e => e.Pattern != null)
    //            .Where(e => e.ElementId.StartsWith(sliceId, StringComparison.Ordinal))
    //            .FirstOrDefault();

    //        ElementDefinition? eProfile = source
    //                .Where(e => e.ElementId.StartsWith(sliceId, StringComparison.Ordinal))
    //                .Where(e => e.Type.Any(t => t.Profile.Any()))
    //                .FirstOrDefault();

    //        ElementDefinition.DiscriminatorType? bgType = null;
    //        if (source.First().Type.Any(t => t.Code.Equals("Extension", StringComparison.Ordinal)))
    //        {
    //            bgType = ElementDefinition.DiscriminatorType.Value;
    //            bgPath = "url";
    //        }
    //        else if (eFixed != null)
    //        {
    //            bgType = ElementDefinition.DiscriminatorType.Value;
    //            bgPath = eFixed.Path;
    //        }
    //        else if (ePattern != null)
    //        {
    //            bgType = ElementDefinition.DiscriminatorType.Pattern;
    //            bgPath = ePattern.Path;
    //        }
    //        else if (eProfile != null)
    //        {
    //            bgType = ElementDefinition.DiscriminatorType.Profile;
    //            bgPath = eProfile.Path;
    //        }

    //        bgValue =
    //            eFixed?.Fixed?.ToString()
    //            ?? ePattern?.Pattern?.ToString()
    //            ?? eProfile?.Type.FirstOrDefault()?.Profile?.First()
    //            ?? string.Empty;


    //        result.Add(new()
    //        {
    //            Name = slice,
    //            SliceOrder = sliceOrder++,
    //            SliceElements = source.Where(e => e.ElementId.StartsWith(sliceId, StringComparison.Ordinal)),
    //            SlicingElement = eSlicing,
    //            ExplicitType = eSlicing?.Slicing.Discriminator.FirstOrDefault()?.Type,
    //            ExplicitPath = eSlicing?.Slicing.Discriminator.FirstOrDefault()?.Path ?? string.Empty,
    //            ComputedType = bgType,
    //            ComputedPath = bgPath,
    //            ComputedValue = bgValue,
    //            FixedElement = eFixed,
    //            PatternElement = ePattern,
    //            ProfileElement = eProfile,
    //        });
    //    }

    //    return result;
    //}

}
