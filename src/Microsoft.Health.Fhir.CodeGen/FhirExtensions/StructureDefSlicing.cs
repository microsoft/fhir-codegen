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

    /// <summary>Enumerates cg discriminated values in this collection.</summary>
    /// <param name="sd">           The SD to act on.</param>
    /// <param name="slicingEd">    The slicing ed.</param>
    /// <param name="sliceName">    Name of the slice.</param>
    /// <param name="sliceElements">The slice elements.</param>
    /// <returns>
    /// An enumerator that allows foreach to be used to process cg discriminated values in this
    /// collection.
    /// </returns>
    public static IEnumerable<(string type, string path, DataType value)> cgDiscriminatedValues(
        this StructureDefinition sd,
        DefinitionCollection dc,
        ElementDefinition slicingEd,
        string sliceName,
        IEnumerable<ElementDefinition> sliceElements)
    {
        List<(string type, string path, DataType value)> result = new();

        if ((slicingEd.Slicing == null) ||
            (!slicingEd.Slicing.Discriminator.Any()))
        {
            return result;
        }

        // need to resolve last part of discriminator path *through* another slice :-|
        // e.g., Observation.component:SystolicBP.code.coding.code -> Observation.component:SystolicBP.code.coding:SBPCode.code
        if (sliceName.Equals("SystolicBP"))
        {
            Console.Write("");
        }

        foreach (ElementDefinition.DiscriminatorComponent discriminator in slicingEd.Slicing.Discriminator)
        {
            switch (discriminator.Type)
            {
                // pattern is the deprecated name for value
                case ElementDefinition.DiscriminatorType.Value:
                case ElementDefinition.DiscriminatorType.Pattern:
                    {
                        bool found = false;
                        string id = $"{slicingEd.Path}:{sliceName}.{discriminator.Path}";

                        foreach (ElementDefinition ed in sliceElements.Where(e => e.ElementId.Equals(id, StringComparison.Ordinal)))
                        {
                            if (ed.Fixed != null)
                            {
                                result.Add((discriminator.Type.GetLiteral()!, ed.Path, ed.Fixed));
                                found = true;
                                break;
                            }
                            if (ed.Pattern != null)
                            {
                                result.Add((discriminator.Type.GetLiteral()!, ed.Path, ed.Pattern));
                                found = true;
                                break;
                            }
                        }

                        if (found) { continue; }

                        // check for extension URL, it is represented oddly
                        if (discriminator.Path.Equals("url", StringComparison.Ordinal))
                        {
                            id = $"{slicingEd.Path}:{sliceName}";

                            foreach (ElementDefinition ed in sliceElements.Where(e => e.ElementId.Equals(id, StringComparison.Ordinal)))
                            {
                                foreach (string profile in ed.Type?.Where(t => t.Code.Equals("Extension", StringComparison.Ordinal)).SelectMany(t => t.Profile) ?? Enumerable.Empty<string>())
                                {
                                    result.Add((discriminator.Type.GetLiteral()!, ed.Path + ".url", new FhirString(profile)));
                                    found = true;
                                    break;
                                }
                            }
                        }

                        if (found) { continue; }

                        // check for the last component of the path being a type
                        id = $"{slicingEd.Path}:{sliceName}.{discriminator.Path}";
                        string eType = id.Substring(id.LastIndexOf('.') + 1);
                        id = id.Substring(0, id.LastIndexOf('.'));

                        foreach (ElementDefinition ed in sliceElements.Where(e => (e.Type != null) && e.ElementId.Equals(id, StringComparison.Ordinal)))
                        {
                            // check for the specified type
                            foreach (ElementDefinition.TypeRefComponent tr in ed.Type!.Where(t => t.Code.Equals(eType, StringComparison.Ordinal)))
                            {
                                if (tr.Profile.Any())
                                {
                                    result.Add((discriminator.Type.GetLiteral()!, ed.Path, new FhirString(tr.Profile.First())));
                                    found = true;
                                    break;
                                }
                            }

                            if (found) { break; }

                            // check for a transitive slicing definition
                            foreach (ElementDefinition.TypeRefComponent tr in ed.Type!.Where(t => t.Code.Equals("BackboneElement", StringComparison.Ordinal) || t.Code.Equals("Element", StringComparison.Ordinal)))
                            {
                                if (!tr.ProfileElement.Any())
                                {
                                    continue;
                                }

                                foreach (Canonical? pe in tr.ProfileElement)
                                {
                                    string? peName = pe?.GetExtensionValue<FhirString>(CommonDefinitions.ExtUrlEdProfileElement)?.Value ?? string.Empty;

                                    if (string.IsNullOrEmpty(peName))
                                    {
                                        continue;
                                    }

                                    string profileUrl = pe?.Value ?? string.Empty;

                                    if (string.IsNullOrEmpty(profileUrl))
                                    {
                                        continue;
                                    }

                                    // check to see if we can resolve the profile URL
                                    if (!dc.ProfilesByUrl.TryGetValue(profileUrl, out StructureDefinition? transitive))
                                    {
                                        continue;
                                    }

                                    // see if we can find a matching element there
                                    string tId = $"{slicingEd.Path}:{sliceName}.{discriminator.Path}";
                                    if (transitive.cgTryGetElementById(tId, out ElementDefinition? tEd) && (tEd != null))
                                    {
                                        if (tEd.Fixed != null)
                                        {
                                            result.Add((discriminator.Type.GetLiteral()!, tEd.Path, tEd.Fixed));
                                            found = true;
                                            break;
                                        }
                                        if (tEd.Pattern != null)
                                        {
                                            result.Add((discriminator.Type.GetLiteral()!, tEd.Path, tEd.Pattern));
                                            found = true;
                                            break;
                                        }
                                    }

                                    if (found) { break; }

                                    // check for the last component of the path being a type
                                    string teType = tId.Substring(tId.LastIndexOf('.') + 1);
                                    tId = tId.Substring(0, id.LastIndexOf('.'));

                                    if (transitive.cgTryGetElementById(tId, out tEd) && (tEd != null))
                                    {
                                        // check for the specified type
                                        foreach (ElementDefinition.TypeRefComponent tTr in tEd.Type!.Where(t => t.Code.Equals(teType, StringComparison.Ordinal)))
                                        {
                                            if (tTr.Profile.Any())
                                            {
                                                result.Add((discriminator.Type.GetLiteral()!, tEd.Path, new FhirString(tTr.Profile.First())));
                                                found = true;
                                                break;
                                            }
                                        }
                                    }

                                    if (found) { break; }
                                }

                                if (found) { break; }
                            }
                        }
                    }
                    break;

                case ElementDefinition.DiscriminatorType.Profile:
                    {
                        bool found = false;
                        string id = $"{slicingEd.Path}:{sliceName}.{discriminator.Path}";

                        foreach (ElementDefinition ed in sliceElements.Where(e => e.ElementId.Equals(id, StringComparison.Ordinal)))
                        {
                            foreach (string profile in ed.Type?.SelectMany(t => t.Profile) ?? Enumerable.Empty<string>())
                            {
                                result.Add((discriminator.Type.GetLiteral()!, ed.Path, new FhirString(profile)));
                                found = true;
                                break;
                            }
                        }

                        if (found) { continue; }

                        // check for the last component of the path being a type
                        id = $"{slicingEd.Path}:{sliceName}.{discriminator.Path}";
                        string eType = id.Substring(id.LastIndexOf('.') + 1);
                        id = id.Substring(0, id.LastIndexOf('.'));

                        foreach (ElementDefinition ed in sliceElements.Where(e => e.ElementId.Equals(id, StringComparison.Ordinal)))
                        {
                            if (ed.Type == null)
                            {
                                continue;
                            }

                            foreach (ElementDefinition.TypeRefComponent t in ed.Type.Where(t => t.Code.Equals(eType) && t.Profile.Any()))
                            {
                                result.Add((discriminator.Type.GetLiteral()!, ed.Path, new FhirString(t.Profile.First())));
                                found = true;
                                break;
                            }

                            if (found) { break; }

                            // TODO: not quite right - need to check for transitive slicing (see above)
                            // if not found, check for 'commonly misused' types
                            foreach (ElementDefinition.TypeRefComponent t in ed.Type.Where(t => (t.Code.Equals("BackboneElement", StringComparison.Ordinal) || t.Code.Equals("Element", StringComparison.Ordinal)) && t.Profile.Any()))
                            {
                                result.Add((discriminator.Type.GetLiteral()!, ed.Path, new FhirString(t.Profile.First())));
                                found = true;
                                break;
                            }

                        }
                    }
                    break;

                    //default:
                    //    break;
            }
        }

        return result;
    }
}
