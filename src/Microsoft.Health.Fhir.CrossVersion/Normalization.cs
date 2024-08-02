// <copyright file="Normalization.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;
using Hl7.Fhir.ElementModel;
using Hl7.Fhir.Model;
using Microsoft.Health.Fhir.CodeGenCommon.Extensions;
using Microsoft.Health.Fhir.CodeGenCommon.FhirExtensions;
using Microsoft.Health.Fhir.CodeGenCommon.Utils;
using Newtonsoft.Json.Serialization;

#if NETSTANDARD2_0
using Microsoft.Health.Fhir.CodeGenCommon.Polyfill;
#endif

namespace Microsoft.Health.Fhir.CrossVersion;

internal static class Normalization
{

    /// <summary>
    /// Reconcile the StructureDefinition of a primitive type to be consistent.
    /// </summary>
    /// <param name="sd">The StructureDefinition to reconcile.</param>
    /// <param name="primitiveInfo">The FhirPrimitiveInfoRec containing the primitive type information.</param>
    internal static void ReconcilePrimitiveType(StructureDefinition sd, FhirTypeUtils.FhirPrimitiveInfoRec primitiveInfo)
    {
        // flag we are a primitive type
        sd.Kind = StructureDefinition.StructureDefinitionKind.PrimitiveType;

        // ensure we have the base type set
        sd.Type ??= sd.Id;

        string valuePath = sd.Id + ".value";

        // check the snapshot for the value element
        if (sd.Snapshot?.Element.FirstOrDefault(e => e.Path == valuePath) is ElementDefinition snapshotEd)
        {
            setPrimitiveValueType(snapshotEd.Type);
        }

        if (sd.Differential?.Element.FirstOrDefault(e => e.Path == valuePath) is ElementDefinition differentialEd)
        {
            setPrimitiveValueType(differentialEd.Type);
        }

        return;

        void setPrimitiveValueType(List<ElementDefinition.TypeRefComponent> types)
        {
            if (types.Count != 1)
            {
                return;
            }

            ElementDefinition.TypeRefComponent tr = types[0];

            // for now, just use the FHIRPath type as the primary with the additional types as extensions
            tr.Code = primitiveInfo.FhirPathType;

            // set various extensions
            tr.SetExtension(CommonDefinitions.ExtUrlFhirType, new FhirUrl(primitiveInfo.FhirType));
            tr.SetExtension(CommonDefinitions.ExtUrlJsonType, new FhirUrl(primitiveInfo.JsonType));
            tr.SetExtension(CommonDefinitions.ExtUrlXmlType, new FhirUrl(primitiveInfo.XmlType));
        }
    }

    /// <summary>
    /// Reconcile element type repetitions.
    /// </summary>
    /// <param name="ed">The ed.</param>
    internal static void ReconcileElementTypeRepetitions(ElementDefinition ed)
    {
        // only need to attempt consolidation if there are 2 or more types
        if (ed.Type.Count < 2)
        {
            return;
        }

        // consolidate types
        Dictionary<string, ElementDefinition.TypeRefComponent> consolidatedTypes = [];

        foreach (ElementDefinition.TypeRefComponent tr in ed.Type)
        {
            if (!consolidatedTypes.TryGetValue(tr.Code, out ElementDefinition.TypeRefComponent? existing))
            {
                consolidatedTypes[tr.Code] = tr;
                continue;
            }

            // add any missing profile references
            if (tr.ProfileElement.Count != 0)
            {
                existing.ProfileElement.AddRange(tr.ProfileElement);
            }

            if (tr.TargetProfileElement.Count != 0)
            {
                existing.TargetProfileElement.AddRange(tr.TargetProfileElement);
            }
        }

        // update our types
        ed.Type = consolidatedTypes.Values.ToList();
    }

    internal static void FixElementStructureR2(StructureDefinition sd)
    {
        List<string> pathComponents = [];
        List<string?> sliceNameAtLoc = [];

        // process the snapshot if we have one
        if (sd.Snapshot?.Element.Any() ?? false)
        {
            processElements(sd.Snapshot.Element);
        }

        pathComponents.Clear();
        sliceNameAtLoc.Clear();

        // process the differential if we have one
        if (sd.Differential?.Element.Any() ?? false)
        {
            processElements(sd.Differential.Element);
        }

        return;

        void processElements(IEnumerable<ElementDefinition> source)
        {
            HashSet<string> allPaths = [];

            // iterate over the the elements in the snapshot
            foreach (ElementDefinition ed in source)
            {
                // ignore the root element so we don't pollute our slice names
                if (!ed.Path.Contains('.'))
                {
                    ed.ElementId = ed.Path;
                    pathComponents = [ed.Path];
                    sliceNameAtLoc = [null];
                    continue;
                }

                // split the path into components
                string[] edPathComponents = ed.Path.Split('.');

                // determine if this is a new slice
                bool edIsNewSlice = !string.IsNullOrEmpty(ed.SliceName);

                // the slice name could be an alias, determine by seeing if we have seen this path before
                if (edIsNewSlice)
                {
                    // if we have not seen this element before, it is an alias
                    if (!allPaths.Contains(ed.Path))
                    {
                        allPaths.Add(ed.Path);
                        ed.SliceName = null;
                        edIsNewSlice = false;
                        ed.AliasElement.Add(new FhirString(ed.SliceName));
                    }
                }
                else if (!allPaths.Contains(ed.Path))
                {
                    allPaths.Add(ed.Path);
                }

                // determine if this is a child element
                if (edPathComponents.Length > pathComponents.Count)
                {
                    // add our path and slice info
                    for (int i = pathComponents.Count; i < edPathComponents.Length; i++)
                    {
                        pathComponents.Add(edPathComponents[i]);
                        sliceNameAtLoc.Add(null);
                    }
                    sliceNameAtLoc[^1] = edIsNewSlice ? getFilteredSliceName(ed.SliceName!, edPathComponents) : null;

                    // build our id
                    ed.ElementId = getId();

                    // no need to process further
                    continue;
                }

                // determine if this is a sibling element
                if (edPathComponents.Length == pathComponents.Count)
                {
                    // update our path and slice info
                    pathComponents[^1] = edPathComponents[^1];
                    sliceNameAtLoc[^1] = edIsNewSlice ? getFilteredSliceName(ed.SliceName!, edPathComponents) : null;

                    // build our id
                    ed.ElementId = getId();

                    // no need to process further
                    continue;
                }

                // determine if we are moving up the tree
                if (edPathComponents.Length < pathComponents.Count)
                {
                    // remove the extra elements from our path and slice info
                    while (edPathComponents.Length < pathComponents.Count)
                    {
                        pathComponents.RemoveAt(pathComponents.Count - 1);
                        sliceNameAtLoc.RemoveAt(sliceNameAtLoc.Count - 1);
                    }

                    // update our path and slice info
                    pathComponents[^1] = edPathComponents[^1];
                    sliceNameAtLoc[^1] = edIsNewSlice ? getFilteredSliceName(ed.SliceName!, edPathComponents) : null;

                    // build our id
                    ed.ElementId = getId();

                    // no need to process further
                    continue;
                }
            }
        }

        string? getFilteredSliceName(string sliceName, string[] slicePathComponents)
        {
            if (string.IsNullOrEmpty(sliceName))
            {
                return null;
            }

            if (!sliceName.Contains('.'))
            {
                return sliceName;
            }

            string[] sliceComponents = sliceName.Split('.').Where(sc => !slicePathComponents.Any(pc => pc == sc)).ToArray();

            return sliceComponents.ToCamelCaseWord();
        }

        string getId()
        {
            StringBuilder sb = new();

            for (int i = 0; i < pathComponents.Count; i++)
            {
                sb.Append(pathComponents[i]);

                if (!string.IsNullOrEmpty(sliceNameAtLoc[i]))
                {
                    sb.Append(":");
                    sb.Append(sliceNameAtLoc[i]);
                }

                if (i < pathComponents.Count - 1)
                {
                    sb.Append(".");
                }
            }

            return sb.ToString();
        }
    }

    internal static void ReconcileElementRepetitionsR2(StructureDefinition sd)
    {
        bool hasSnapshot = sd.Snapshot?.Element.Any() ?? false;
        bool hasDifferential = sd.Differential?.Element.Any() ?? false;

        if ((!hasSnapshot) && (!hasDifferential))
        {
            return;
        }

        Dictionary<string, ElementDefinition> elementsById = [];
        List<ElementDefinition> elementsToRemove = [];

        if (hasSnapshot)
        {
            discoverDuplicates(sd.Snapshot!.Element);

            if (elementsToRemove.Count == 0)
            {
                return;
            }

            // remove these elements from the snapshot
            elementsToRemove.ForEach(ed => sd.Snapshot.Element.Remove(ed));

            // remove matching elements from the differential
            if (hasDifferential)
            {
                elementsToRemove.ForEach(ed => sd.Differential!.Element.RemoveAll(d => d.ElementId == ed.ElementId));
            }

            return;
        }

        discoverDuplicates(sd.Differential!.Element);

        if (elementsToRemove.Count == 0)
        {
            return;
        }

        // remove these elements from the snapshot
        elementsToRemove.ForEach(ed => sd.Differential.Element.Remove(ed));

        return;

        void discoverDuplicates(List<ElementDefinition> source)
        {
            Dictionary<string, ElementDefinition> edById = [];

            for (int i = 0; i < source.Count; i++)
            {
                ElementDefinition ed = source[i];

                // if this id is not present, add it and continue
                if (!edById.TryGetValue(ed.ElementId, out ElementDefinition? existing))
                {
                    edById.Add(ed.ElementId, ed);
                    continue;
                }

                // reconcile slicing to the first element we ran into
                if ((existing.Slicing == null) && (ed.Slicing != null))
                {
                    existing.Slicing = (ElementDefinition.SlicingComponent)ed.Slicing.DeepCopy();

                    // remove this element and move  our index back
                    source.RemoveAt(i);
                    i--;
                    continue;
                }

                if ((existing.Slicing != null) && (ed.Slicing == null))
                {
                    // remove this element and move  our index back
                    source.RemoveAt(i);
                    i--;
                    continue;
                }

                // iterate over our new element discriminators
                foreach (ElementDefinition.DiscriminatorComponent edD in ed.Slicing?.Discriminator ?? Enumerable.Empty<ElementDefinition.DiscriminatorComponent>())
                {
                    // if this discriminator is not already present, add it
                    if (!existing.Slicing!.Discriminator.Any(d => d.Type == edD.Type && d.Path == edD.Path))
                    {
                        existing.Slicing!.Discriminator.Add((ElementDefinition.DiscriminatorComponent)edD.DeepCopy());
                    }
                }

                // remove this element and move  our index back
                source.RemoveAt(i);
                i--;
                continue;
            }
        }

        void discoverDuplicatesOld(List<ElementDefinition> source)
        {
            foreach (ElementDefinition ed in source)
            {
                // if this is not already present, add it
                if (!elementsById.TryGetValue(ed.ElementId, out ElementDefinition? existing))
                {
                    elementsById[ed.ElementId] = ed;
                    continue;
                }

                // drop the element without slicing
                if (existing.Slicing == null)
                {
                    elementsToRemove.Add(ed);
                    continue;
                }

                if (ed.Slicing == null)
                {
                    elementsToRemove.Add(existing);
                    elementsById[ed.ElementId] = ed;
                    continue;
                }

                // if one has a slice name, remove that one
                if (!string.IsNullOrEmpty(ed.SliceName))
                {
                    elementsToRemove.Add(ed);
                    continue;
                }

                if (!string.IsNullOrEmpty(existing.SliceName))
                {
                    elementsToRemove.Add(existing);
                    elementsById[ed.ElementId] = ed;
                    continue;
                }

                // should never get here
                throw new Exception($"Need to figure out how {ed.ElementId} is different!");
            }
        }
    }
}
