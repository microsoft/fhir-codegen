// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Health.Fhir.SpecManager.Models
{
    /// <summary>Common definitions for FHIR Internal properties</summary>
    public abstract class FhirCommon
    {
        /// <summary>The standard status deprecated.</summary>
        public const string StandardStatusDeprecated = "deprecated";

        /// <summary>The standard status draft.</summary>
        public const string StandardStatusDraft = "draft";

        /// <summary>The standard status external.</summary>
        public const string StandardStatusExternal = "external";

        /// <summary>The standard status informative.</summary>
        public const string StandardStatusInformative = "informative";

        /// <summary>The standard status normative.</summary>
        public const string StandardStatusNormative = "normative";

        /// <summary>The standard status trial use.</summary>
        public const string StandardStatusTrialUse = "trial-use";

        /// <summary>The standard status codes.</summary>
        public static readonly string[] StandardStatusCodes =
        {
            StandardStatusDeprecated,
            StandardStatusDraft,
            StandardStatusExternal,
            StandardStatusInformative,
            StandardStatusNormative,
            StandardStatusTrialUse,
        };
    }
}
