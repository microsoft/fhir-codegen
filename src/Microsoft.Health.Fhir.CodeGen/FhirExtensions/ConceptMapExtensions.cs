// <copyright file="ConceptMapExtensions.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;
using Hl7.Fhir.Model;

namespace Microsoft.Health.Fhir.CodeGen.FhirExtensions;

public static class ConceptMapExtensions
{
    public static string? cgSourceScope(this ConceptMap cm)
    {
        if ((cm.SourceScope is Canonical canonical) && (!string.IsNullOrEmpty(canonical.Uri)))
        {
            return canonical.Uri;
        }

        if ((cm.SourceScope is FhirUri uri) && (!string.IsNullOrEmpty(uri.Value)))
        {
            return uri.Value;
        }

        return null;
    }

    public static string? cgTargetScope(this ConceptMap cm)
    {
        if ((cm.TargetScope is Canonical canonical) && (!string.IsNullOrEmpty(canonical.Uri)))
        {
            return canonical.Uri;
        }

        if ((cm.TargetScope is FhirUri uri) && (!string.IsNullOrEmpty(uri.Value)))
        {
            return uri.Value;
        }

        return null;
    }
}
