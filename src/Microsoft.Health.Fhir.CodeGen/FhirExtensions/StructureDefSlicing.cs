// <copyright file="StructureDefSlicing.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Hl7.Fhir.Model;
using Hl7.Fhir.Utility;
using Hl7.FhirPath.Sprache;
using Microsoft.Health.Fhir.CodeGen.Models;
using Microsoft.Health.Fhir.CodeGenCommon.FhirExtensions;

#if NETSTANDARD2_0
using Microsoft.Health.Fhir.CodeGen.Polyfill;
#endif

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
        return (sd.Snapshot != null) && (sd.Snapshot.Element.Count != 0)
            ? sd.Snapshot.Element.Where(e => e.ElementId.StartsWith(slicingEd.ElementId, StringComparison.Ordinal)).Skip(includeRoot ? 0 : 1)
            : sd.Differential.Element.Where(e => e.ElementId.StartsWith(slicingEd.ElementId, StringComparison.Ordinal)).Skip(includeRoot ? 0 : 1);
    }

    /// <summary>Enumerates cg elements with slices in this collection.</summary>
    /// <param name="sd">The SD to act on.</param>
    /// <returns>
    /// An enumerator that allows foreach to be used to process cg elements with slices in this
    /// collection.
    /// </returns>
    public static IEnumerable<ElementDefinition> cgElementsWithSlices(this StructureDefinition sd)
    {
        List<ElementDefinition> source = (sd.Snapshot != null) && (sd.Snapshot.Element.Count != 0)
            ? sd.Snapshot.Element
            : sd.Differential.Element;

        for (int i = 0; i < (source.Count - 1); i++)
        {
            if (source[i + 1].ElementId.StartsWith(source[i].ElementId + ":", StringComparison.Ordinal))
            {
                yield return source[i];
            }
        }
    }

    /// <summary>Tries to get the parent element that defines the slicing rules for a sliced element.</summary>
    /// <param name="sd">           The SD to act on.</param>
    /// <param name="slicedElement">The sliced element.</param>
    /// <param name="parentEd">     [out] The parent ed.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    public static bool cgTryGetSlicingParent(
        this StructureDefinition sd,
        ElementDefinition slicedElement,
        [NotNullWhen(true)] out ElementDefinition? parentEd)
    {
        string parentId = slicedElement.ElementId.Substring(0, slicedElement.ElementId.LastIndexOf(':'));

        if ((sd.Snapshot != null) && (sd.Snapshot.Element.Count != 0))
        {
            parentEd = sd.Snapshot.Element.FirstOrDefault(e => e.ElementId == parentId);
            return parentEd != null;
        }

        parentEd = null;
        return false;
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
    public static IEnumerable<SliceDiscriminator> cgDiscriminatedValues(
        this StructureDefinition sd,
        DefinitionCollection dc,
        ElementDefinition slicingEd,
        string sliceName,
        IEnumerable<ElementDefinition> sliceElements)
    {
        List<SliceDiscriminator> result = [];

        if ((slicingEd.Slicing == null) ||
            (slicingEd.Slicing.Discriminator.Count == 0))
        {
            return result;
        }

        // TODO(ginoc): Need to test reslicing - need an example to test against
        foreach (ElementDefinition.DiscriminatorComponent discriminator in slicingEd.Slicing.Discriminator)
        {
            switch (discriminator.Type)
            {
                // pattern is the deprecated name for value
                case ElementDefinition.DiscriminatorType.Value:
                case ElementDefinition.DiscriminatorType.Pattern:
                    result.AddRange(SlicesForValue(dc, discriminator, slicingEd, sliceName, sliceElements));
                    break;

                case ElementDefinition.DiscriminatorType.Profile:
                    result.AddRange(SlicesForProfile(dc, discriminator, slicingEd, sliceName, sliceElements));
                    break;

                case ElementDefinition.DiscriminatorType.Type:
                    result.AddRange(SlicesForType(dc, discriminator, slicingEd, sliceName, sliceElements));
                    break;

                case ElementDefinition.DiscriminatorType.Exists:
                    result.AddRange(SlicesForExists(dc, discriminator, slicingEd, sliceName, sliceElements));
                    break;

                // TODO(ginoc): need to find an example of this so I can implement it
                case ElementDefinition.DiscriminatorType.Position:
                    {
                        Console.WriteLine($"cgDiscriminatedValues <<< {sd.Id}: unhandled discriminator: {discriminator.Type}:{discriminator.Path}");
                    }
                    break;

                //default:
                //    break;
            }
        }

        return result;
    }

    /// <summary>Gets relative path.</summary>
    /// <param name="slicingPath">      Full pathname of the slicing file.</param>
    /// <param name="discriminatorPath">Full pathname of the discriminator file.</param>
    /// <param name="postResolve">      [out] The post resolve.</param>
    /// <returns>The relative path.</returns>
    private static string GetRelativePath(
        string slicingPath,
        string discriminatorPath,
        out string postResolve)
    {
        if (discriminatorPath == "$this")
        {
            postResolve = string.Empty;
            return string.Empty;
        }

        string path = discriminatorPath;
        postResolve = string.Empty;

        if (path.StartsWith("$this.", StringComparison.Ordinal))
        {
            path = discriminatorPath[5..];
        }

        int resolveIndex = path.IndexOf("resolve()", StringComparison.Ordinal);

        if (resolveIndex != -1)
        {
            postResolve = path[(resolveIndex + 9)..];

            if (postResolve.StartsWith('.'))
            {
                postResolve = postResolve[1..];
            }

            path = path[..resolveIndex];
        }

        // TODO(ginoc): need to sort out allowed 'ofType()' processing and mangle the path appropriately - need an example to test against

        if (string.IsNullOrEmpty(path) || path == ".")
        {
            return string.Empty;
        }

        if (!path.StartsWith('.'))
        {
            path = "." + path;
        }

        // check for incorrect path nesting - using the parent path
        if ((!string.IsNullOrEmpty(slicingPath)) && slicingPath.EndsWith(path))
        {
            return string.Empty;
        }

        return path;
    }

    /// <summary>Slices for type.</summary>
    /// <param name="dc">           The device-context.</param>
    /// <param name="discriminator">The discriminator.</param>
    /// <param name="slicingEd">    The slicing ed.</param>
    /// <param name="sliceName">    Name of the slice.</param>
    /// <param name="sliceElements">The slice elements.</param>
    /// <returns>A List&lt;SliceDiscriminator&gt;</returns>
    private static List<SliceDiscriminator> SlicesForType(
        DefinitionCollection dc,
        ElementDefinition.DiscriminatorComponent discriminator,
        ElementDefinition slicingEd,
        string sliceName,
        IEnumerable<ElementDefinition> sliceElements)
    {
        List<SliceDiscriminator> result = [];
        string relativePath = GetRelativePath(slicingEd.Path, discriminator.Path, out string postResolve);

        string id = $"{slicingEd.Path}:{sliceName}{relativePath}";
        string path = $"{slicingEd.Path}{relativePath}";

        foreach (ElementDefinition ed in sliceElements.Where(e => e.Path == path))
        {
            foreach (ElementDefinition.TypeRefComponent et in ed.Type ?? Enumerable.Empty<ElementDefinition.TypeRefComponent>())
            {
                result.Add(new()
                {
                    DiscriminatorType = (ElementDefinition.DiscriminatorType)discriminator.Type!,
                    Type = discriminator.Type.GetLiteral()!,
                    Path = ed.Path,
                    PostResolvePath = postResolve,
                    Id = ed.ElementId,
                    Value = new FhirString(et.Code),
                });
            }
        }

        if (result.Count != 0)
        {
            return result;
        }

        if (!string.IsNullOrEmpty(relativePath))
        {
            // check for the last component of the path being a type
            id = $"{slicingEd.Path}:{sliceName}{relativePath}";

            int lastIdDotIndex = id.LastIndexOf('.');

            string eType = id[(lastIdDotIndex + 1)..];
            id = id[..lastIdDotIndex];

            path = path.Substring(0, path.LastIndexOf('.'));

            foreach (ElementDefinition ed in sliceElements.Where(e => e.Path == path))
            {
                bool found = false;

                if (ed.Type == null)
                {
                    continue;
                }

                foreach (ElementDefinition.TypeRefComponent t in ed.Type.Where(t => t.Code.Equals(eType)))
                {
                    result.Add(new()
                    {
                        DiscriminatorType = (ElementDefinition.DiscriminatorType)discriminator.Type!,
                        Type = discriminator.Type.GetLiteral()!,
                        Path = ed.Path,
                        PostResolvePath = postResolve,
                        Id = ed.ElementId,
                        Value = new FhirString(t.Code),
                    });
                    found = true;
                }

                if (found) { continue; }

                // TODO: not quite right - need to check for transitive slicing (see above)
                // if not found, check for 'commonly misused' types
                foreach (ElementDefinition.TypeRefComponent t in ed.Type.Where(t => (t.Code == "BackboneElement" || t.Code == "Element")))
                {
                    result.Add(new()
                    {
                        DiscriminatorType = (ElementDefinition.DiscriminatorType)discriminator.Type!,
                        Type = discriminator.Type.GetLiteral()!,
                        Path = ed.Path,
                        PostResolvePath = postResolve,
                        Id = ed.ElementId,
                        Value = new FhirString(t.Code),
                    });
                    found = true;
                }
            }
        }

        return result;
    }

    private static List<SliceDiscriminator> SlicesForExists(
        DefinitionCollection dc,
        ElementDefinition.DiscriminatorComponent discriminator,
        ElementDefinition slicingEd,
        string sliceName,
        IEnumerable<ElementDefinition> sliceElements)
    {
        List<SliceDiscriminator> result = [];
        string relativePath = GetRelativePath(slicingEd.Path, discriminator.Path, out string postResolve);

        string id = $"{slicingEd.Path}:{sliceName}{relativePath}";
        string path = $"{slicingEd.Path}{relativePath}";

        foreach (ElementDefinition ed in sliceElements.Where(e => e.Path == path))
        {
            result.Add(new()
            {
                DiscriminatorType = (ElementDefinition.DiscriminatorType)discriminator.Type!,
                Type = discriminator.Type.GetLiteral()!,
                Path = ed.Path,
                PostResolvePath = postResolve,
                Id = ed.ElementId,
                Value = new FhirBoolean(true),
            });
        }

        if (result.Count != 0)
        {
            return result;
        }

        if (!string.IsNullOrEmpty(relativePath))
        {
            // check for the last component of the path being a type
            id = $"{slicingEd.Path}:{sliceName}{relativePath}";
            string eType = id.Substring(id.LastIndexOf('.') + 1);
            id = id.Substring(0, id.LastIndexOf('.'));
            path = path.Substring(0, path.LastIndexOf('.'));

            foreach (ElementDefinition ed in sliceElements.Where(e => e.Path == path))
            {
                result.Add(new()
                {
                    DiscriminatorType = (ElementDefinition.DiscriminatorType)discriminator.Type!,
                    Type = discriminator.Type.GetLiteral()!,
                    Path = ed.Path,
                    PostResolvePath = postResolve,
                    Id = ed.ElementId,
                    Value = new FhirBoolean(true),
                });
            }
        }

        return result;
    }

    /// <summary>Slices for profile.</summary>
    /// <param name="dc">           The device-context.</param>
    /// <param name="discriminator">The discriminator.</param>
    /// <param name="slicingEd">    The slicing ed.</param>
    /// <param name="sliceName">    Name of the slice.</param>
    /// <param name="sliceElements">The slice elements.</param>
    /// <returns>A List&lt;SliceDiscriminator&gt;</returns>
    private static List<SliceDiscriminator> SlicesForProfile(
        DefinitionCollection dc,
        ElementDefinition.DiscriminatorComponent discriminator,
        ElementDefinition slicingEd,
        string sliceName,
        IEnumerable<ElementDefinition> sliceElements)
    {
        List<SliceDiscriminator> result = [];
        string relativePath = GetRelativePath(slicingEd.Path, discriminator.Path, out string postResolve);

        string id = $"{slicingEd.Path}:{sliceName}{relativePath}";
        string path = $"{slicingEd.Path}{relativePath}";

        foreach (ElementDefinition ed in sliceElements.Where(e => e.Path == path))
        //foreach (ElementDefinition ed in sliceElements.Where(e => e.ElementId.Equals(id, StringComparison.Ordinal)))
        {
            foreach (string profile in ed.Type?.SelectMany(t => t.Profile) ?? Enumerable.Empty<string>())
            {
                result.Add(new()
                {
                    DiscriminatorType = (ElementDefinition.DiscriminatorType)discriminator.Type!,
                    Type = discriminator.Type.GetLiteral()!,
                    Path = ed.Path,
                    PostResolvePath = postResolve,
                    Id = ed.ElementId,
                    Value = new FhirString(profile),
                });
                continue;
            }
        }

        if (result.Count != 0)
        {
            return result;
        }

        if (!string.IsNullOrEmpty(relativePath))
        {
            // check for the last component of the path being a type
            id = $"{slicingEd.Path}:{sliceName}{relativePath}";
            string eType = id.Substring(id.LastIndexOf('.') + 1);
            id = id.Substring(0, id.LastIndexOf('.'));
            path = path.Substring(0, path.LastIndexOf('.'));

            foreach (ElementDefinition ed in sliceElements.Where(e => e.Path == path))
            //foreach (ElementDefinition ed in sliceElements.Where(e => e.ElementId.Equals(id, StringComparison.Ordinal)))
            {
                bool found = false;

                if (ed.Type == null)
                {
                    continue;
                }

                foreach (ElementDefinition.TypeRefComponent t in ed.Type.Where(t => t.Code.Equals(eType) && t.Profile.Any()))
                {
                    result.Add(new()
                    {
                        DiscriminatorType = (ElementDefinition.DiscriminatorType)discriminator.Type!,
                        Type = discriminator.Type.GetLiteral()!,
                        Path = ed.Path,
                        PostResolvePath = postResolve,
                        Id = ed.ElementId,
                        Value = new FhirString(t.Profile.First()),
                    });
                    found = true;
                }

                if (found) { continue; }

                result.AddRange(CheckTransitiveForValue(
                    dc,
                    ed.Type!.Where(t => t.Code == "BackboneElement" || t.Code == "Element"),
                    discriminator,
                    id,
                    relativePath,
                    slicingEd,
                    sliceName,
                    postResolve));
            }
        }

        return result;
    }

    /// <summary>Discriminators for value.</summary>
    /// <param name="dc">           The device-context.</param>
    /// <param name="discriminator">The discriminator.</param>
    /// <param name="slicingEd">    The slicing ed.</param>
    /// <param name="sliceName">    Name of the slice.</param>
    /// <param name="sliceElements">The slice elements.</param>
    /// <returns>A List&lt;SliceDiscriminator&gt;</returns>
    private static List<SliceDiscriminator> SlicesForValue(
        DefinitionCollection dc,
        ElementDefinition.DiscriminatorComponent discriminator,
        ElementDefinition slicingEd,
        string sliceName,
        IEnumerable<ElementDefinition> sliceElements)
    {
        List<SliceDiscriminator> result = [];
        string relativePath = GetRelativePath(slicingEd.Path, discriminator.Path, out string postResolve);

        string id = $"{slicingEd.Path}:{sliceName}{relativePath}";
        string path = $"{slicingEd.Path}{relativePath}";

        foreach (ElementDefinition ed in sliceElements.Where(e => e.ElementId.Equals(id, StringComparison.Ordinal)))
        {
            if (ed.Fixed != null)
            {
                result.Add(new()
                {
                    DiscriminatorType = (ElementDefinition.DiscriminatorType)discriminator.Type!,
                    Type = discriminator.Type.GetLiteral()!,
                    Path = ed.Path,
                    PostResolvePath = postResolve,
                    Id = ed.ElementId,
                    Value = ed.Fixed,
                });
                continue;
            }

            if (ed.Pattern != null)
            {
                result.Add(new()
                {
                    DiscriminatorType = (ElementDefinition.DiscriminatorType)discriminator.Type!,
                    Type = discriminator.Type.GetLiteral()!,
                    Path = ed.Path,
                    PostResolvePath = postResolve,
                    Id = ed.ElementId,
                    Value = ed.Pattern,
                });

                continue;
            }

            // check for references that cross resolve boundaries
            if (!string.IsNullOrEmpty(postResolve) && ed.Type.Any(t => t.Code == "Reference"))
            {
                List<SliceDiscriminator> resolvedSlices = CheckResolvedTarget(
                    dc,
                    ed.Type!.Where(t => t.Code == "Reference"),
                    discriminator,
                    postResolve,
                    ed.Path);

                if (resolvedSlices.Count != 0)
                {
                    result.AddRange(resolvedSlices);
                    continue;
                }
            }
        }

        if (result.Count != 0)
        {
            return result;
        }

        // check for extension URL, it is represented oddly
        if (discriminator.Path == "url")
        {
            id = $"{slicingEd.Path}:{sliceName}";

            foreach (ElementDefinition ed in sliceElements.Where(e => e.ElementId == id))
            {
                foreach (string profile in ed.Type?.Where(t => t.Code == "Extension").SelectMany(t => t.Profile) ?? Enumerable.Empty<string>())
                {
                    result.Add(new()
                    {
                        DiscriminatorType = (ElementDefinition.DiscriminatorType)discriminator.Type!,
                        Type = discriminator.Type.GetLiteral()!,
                        Path = ed.Path + ".url",
                        PostResolvePath = postResolve,
                        Id = ed.ElementId,
                        Value = new FhirString(profile),
                    });

                    continue;
                }
            }

            // check to see if this is a standalone definition in a differential
            if (result.Count == 0)
            {
                if (slicingEd.Path.EndsWith(".extension", StringComparison.Ordinal) &&
                    (sliceElements.Count() == 1))
                {
                    ElementDefinition diffEd = sliceElements.First();

                    if ((diffEd.Type.Count == 1) &&
                        (diffEd.Type[0].Code == "Extension") &&
                        diffEd.Type[0].Profile.Any())
                    {
                        // we want the current element's parent
                        string parentPath = slicingEd.Path.Substring(0, slicingEd.Path.LastIndexOf('.'));
                        string extPath = parentPath.Contains(".extension", StringComparison.Ordinal)
                            ? "E" + parentPath.Substring(parentPath.LastIndexOf(".extension", StringComparison.Ordinal))[2..]
                            : string.Empty;

                        if (dc.TryFindElementByPath(parentPath, out StructureDefinition? parentSd, out ElementDefinition? parentEd))
                        {
                            result.Add(new()
                            {
                                DiscriminatorType = (ElementDefinition.DiscriminatorType)discriminator.Type!,
                                Type = discriminator.Type.GetLiteral()!,
                                Path = parentEd.Path + ".url",
                                PostResolvePath = postResolve,
                                Id = parentEd.ElementId,
                                Value = new FhirString(diffEd.Type[0].Profile.First()),
                            });
                        }
                        else if (!string.IsNullOrEmpty(extPath) &&
                            dc.TryFindElementByPath(extPath, out StructureDefinition? extSd, out ElementDefinition? extEd))
                        {
                            result.Add(new()
                            {
                                DiscriminatorType = (ElementDefinition.DiscriminatorType)discriminator.Type!,
                                Type = discriminator.Type.GetLiteral()!,
                                Path = diffEd.Path + ".url",
                                PostResolvePath = postResolve,
                                Id = extEd.ElementId,
                                Value = new FhirString(diffEd.Type[0].Profile.First()),
                            });
                        }
                        else
                        {
                            result.Add(new()
                            {
                                DiscriminatorType = (ElementDefinition.DiscriminatorType)discriminator.Type!,
                                Type = discriminator.Type.GetLiteral()!,
                                Path = diffEd.Path + ".url",
                                PostResolvePath = postResolve,
                                Id = diffEd.ElementId,
                                Value = new FhirString(diffEd.Type[0].Profile.First()),
                            });
                        }
                    }
                }
            }
        }

        if (result.Count != 0)
        {
            return result;
        }

        if (!string.IsNullOrEmpty(relativePath))
        {
            // check for the last component of the path being a type
            id = $"{slicingEd.Path}:{sliceName}{relativePath}";
            string eType = id.Substring(id.LastIndexOf('.') + 1);
            id = id.Substring(0, id.LastIndexOf('.'));
            path = path.Substring(0, path.LastIndexOf('.'));

            foreach (ElementDefinition ed in sliceElements.Where(e => (e.Type != null) && e.Path == path))
            //foreach (ElementDefinition ed in sliceElements.Where(e => (e.Type != null) && e.ElementId.Equals(id, StringComparison.Ordinal)))
            {
                bool found = false;

                // check for the specified type
                foreach (ElementDefinition.TypeRefComponent tr in ed.Type!.Where(t => t.Code == eType))
                {
                    if (tr.Profile.Any())
                    {
                        result.Add(new()
                        {
                            DiscriminatorType = (ElementDefinition.DiscriminatorType)discriminator.Type!,
                            Type = discriminator.Type.GetLiteral()!,
                            Path = ed.Path,
                            PostResolvePath = postResolve,
                            Id = ed.ElementId,
                            Value = new FhirString(tr.Profile.First()),
                        });

                        continue;
                    }
                }

                if (found) { continue; }

                // check for a transitive slicing definition
                result.AddRange(CheckTransitiveForValue(
                    dc,
                    ed.Type!.Where(t => t.Code == "BackboneElement" || t.Code == "Element"),
                    discriminator,
                    id,
                    relativePath,
                    slicingEd,
                    sliceName,
                    postResolve));
            }
        }

        return result;
    }

    /// <summary>Check transitive.</summary>
    /// <param name="dc">            The device-context.</param>
    /// <param name="typeComponents">The type components.</param>
    /// <param name="discriminator"> The discriminator.</param>
    /// <param name="id">            The identifier.</param>
    /// <param name="relativePath">  Full pathname of the relative file.</param>
    /// <param name="slicingEd">     The slicing ed.</param>
    /// <param name="sliceName">     Name of the slice.</param>
    /// <returns>A List&lt;SliceDiscriminator&gt;</returns>
    private static List<SliceDiscriminator> CheckTransitiveForValue(
        DefinitionCollection dc,
        IEnumerable<ElementDefinition.TypeRefComponent> typeComponents,
        ElementDefinition.DiscriminatorComponent discriminator,
        string id,
        string relativePath,
        ElementDefinition slicingEd,
        string sliceName,
        string postResolve)
    {
        List<SliceDiscriminator> result = [];

        foreach (ElementDefinition.TypeRefComponent tr in typeComponents)
        {
            if (tr.ProfileElement.Count == 0)
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
                if ((!dc.ProfilesByUrl.TryGetValue(profileUrl, out StructureDefinition? transitive)) ||
                    (transitive == null))
                {
                    continue;
                }

                // see if we can find a matching element there
                string tId = $"{slicingEd.Path}:{sliceName}{relativePath}";
                if (transitive.cgTryGetElementById(tId, out ElementDefinition? tEd) && (tEd != null))
                {
                    if (tEd.Fixed != null)
                    {
                        result.Add(new()
                        {
                            DiscriminatorType = (ElementDefinition.DiscriminatorType)discriminator.Type!,
                            Type = discriminator.Type.GetLiteral()!,
                            Path = tEd.Path,
                            PostResolvePath = postResolve,
                            Id = tEd.ElementId,
                            Value = tEd.Fixed,
                        });
                        continue;
                    }

                    if (tEd.Pattern != null)
                    {
                        result.Add(new()
                        {
                            DiscriminatorType = (ElementDefinition.DiscriminatorType)discriminator.Type!,
                            Type = discriminator.Type.GetLiteral()!,
                            Path = tEd.Path,
                            PostResolvePath = postResolve,
                            Id = tEd.ElementId,
                            Value = tEd.Pattern,
                        });
                        continue;
                    }

                    if (tEd.Binding != null)
                    {
                        result.Add(new()
                        {
                            DiscriminatorType = (ElementDefinition.DiscriminatorType)discriminator.Type!,
                            Type = discriminator.Type.GetLiteral()!,
                            Path = tEd.Path,
                            PostResolvePath = postResolve,
                            Id = tEd.ElementId,
                            IsBinding = true,
                            BindingName = tEd.Binding.GetExtensionValue<FhirString>(CommonDefinitions.ExtUrlBindingName)?.Value ?? string.Empty,
                            Value = tEd.Binding.ValueSetElement,
                        });
                        continue;
                    }

                }

                // check for the last component of the path being a type
                string teType = tId.Substring(tId.LastIndexOf('.') + 1);
                tId = tId.Substring(0, tId.LastIndexOf('.'));

                if (transitive.cgTryGetElementById(tId, out tEd) && (tEd != null))
                {
                    // check for the specified type
                    foreach (ElementDefinition.TypeRefComponent tTr in tEd.Type!.Where(t => t.Code == teType))
                    {
                        if (tTr.Profile.Any())
                        {
                            result.Add(new()
                            {
                                DiscriminatorType = (ElementDefinition.DiscriminatorType)discriminator.Type!,
                                Type = discriminator.Type.GetLiteral()!,
                                Path = tEd.Path,
                                PostResolvePath = postResolve,
                                Id = tEd.ElementId,
                                Value = new FhirString(tTr.Profile.First()),
                            });
                            continue;
                        }
                    }
                }
            }
        }

        return result;
    }

    /// <summary>Check resolved target.</summary>
    /// <param name="dc">            The device-context.</param>
    /// <param name="typeComponents">The type components.</param>
    /// <param name="discriminator"> The discriminator.</param>
    /// <param name="postResolve">   The post resolve.</param>
    /// <param name="sourcePath">    Full pathname of the source file.</param>
    /// <returns>A List&lt;SliceDiscriminator&gt;</returns>
    private static List<SliceDiscriminator> CheckResolvedTarget(
        DefinitionCollection dc,
        IEnumerable<ElementDefinition.TypeRefComponent> typeComponents,
        ElementDefinition.DiscriminatorComponent discriminator,
        string postResolve,
        string sourcePath)
    {
        List<SliceDiscriminator> result = [];

        foreach (ElementDefinition.TypeRefComponent tr in typeComponents)
        {
            if (!tr.TargetProfile.Any())
            {
                continue;
            }

            foreach (string tp in tr.TargetProfile)
            {
                // check to see if we can resolve the profile URL
                if ((!dc.ProfilesByUrl.TryGetValue(tp, out StructureDefinition? transitive)) ||
                    (transitive == null))
                {
                    continue;
                }

                // see if we can find a matching element there
                string tId = string.Join(".", transitive.Type, postResolve);
                if (transitive.cgTryGetElementById(tId, out ElementDefinition? tEd) && (tEd != null))
                {
                    if (tEd.Fixed != null)
                    {
                        result.Add(new()
                        {
                            DiscriminatorType = (ElementDefinition.DiscriminatorType)discriminator.Type!,
                            Type = discriminator.Type.GetLiteral()!,
                            Path = sourcePath,
                            PostResolvePath = postResolve,
                            Id = tEd.ElementId,
                            Value = tEd.Fixed,
                        });
                        continue;
                    }

                    if (tEd.Pattern != null)
                    {
                        result.Add(new()
                        {
                            DiscriminatorType = (ElementDefinition.DiscriminatorType)discriminator.Type!,
                            Type = discriminator.Type.GetLiteral()!,
                            Path = sourcePath,
                            PostResolvePath = postResolve,
                            Id = tEd.ElementId,
                            Value = tEd.Pattern,
                        });
                        continue;
                    }

                    if (tEd.Binding != null)
                    {
                        result.Add(new()
                        {
                            DiscriminatorType = (ElementDefinition.DiscriminatorType)discriminator.Type!,
                            Type = discriminator.Type.GetLiteral()!,
                            Path = sourcePath,
                            PostResolvePath = postResolve,
                            Id = tEd.ElementId,
                            IsBinding = true,
                            BindingName = tEd.Binding.GetExtensionValue<FhirString>(CommonDefinitions.ExtUrlBindingName)?.Value ?? string.Empty,
                            Value = tEd.Binding.ValueSetElement,
                        });
                        continue;
                    }
                }

                // check for the last component of the path being a type
                string teType = tId.Substring(tId.LastIndexOf('.') + 1);
                tId = tId.Substring(0, tId.LastIndexOf('.'));

                if (transitive.cgTryGetElementById(tId, out tEd) && (tEd != null))
                {
                    // check for the specified type
                    foreach (ElementDefinition.TypeRefComponent tTr in tEd.Type!.Where(t => t.Code == teType))
                    {
                        if (tTr.Profile.Any())
                        {
                            result.Add(new()
                            {
                                DiscriminatorType = (ElementDefinition.DiscriminatorType)discriminator.Type!,
                                Type = discriminator.Type.GetLiteral()!,
                                Path = sourcePath,
                                PostResolvePath = postResolve.Substring(0, postResolve.LastIndexOf('.')),
                                Id = tEd.ElementId,
                                Value = new FhirString(tTr.Profile.First()),
                            });
                            continue;
                        }
                    }
                }
            }
        }

        return result;
    }
}
