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

    internal static void VerifyRootElementType(StructureDefinition sd)
    {
        ElementDefinition? rootElement = sd.Snapshot?.Element.FirstOrDefault();
        if (rootElement != null)
        {
            checkElement(rootElement);
        }

        rootElement = sd.Differential?.Element.FirstOrDefault();
        if (rootElement != null)
        {
            checkElement(rootElement);
        }

        return;

        void checkElement(ElementDefinition ed)
        {
            ed.Base ??= new ElementDefinition.BaseComponent();

            if (string.IsNullOrEmpty(ed.Base.Path))
            {
                // if this is a structure defining a new 'something', that is the correct base
                if (ed.Path == sd.Id)
                {
                    ed.Base.Path = ed.Path;
                }
                // if not and this has a base type, assume the root is that type (e.g., Extension)
                else if (!string.IsNullOrEmpty(sd.Type))
                {
                    ed.Base.Path = sd.Type;
                }
                // should never get here, but fall-back to using the path
                else
                {
                    ed.Base.Path = ed.Path;
                }
            }

            ed.Min ??= 0;

            if (string.IsNullOrEmpty(ed.Max))
            {
                ed.Max = "*";
            }
        }
    }
}
