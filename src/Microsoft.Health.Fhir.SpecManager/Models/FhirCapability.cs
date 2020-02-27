// -------------------------------------------------------------------------------------------------
// <copyright file="FhirCapability.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>
// -------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Health.Fhir.SpecManager.Models
{
    /// <summary>A fhir capability.</summary>
    public class FhirCapability
    {
        /// <summary>Gets the type of the resource.</summary>
        /// <value>The type of the resource.</value>
        public string ResourceType { get; }

        /// <summary>Gets the resrouce profile.</summary>
        /// <value>The resrouce profile.</value>
        public string ResrouceProfile { get; }

        /// <summary>Gets the interactions.</summary>
        /// <value>The interactions.</value>
        public HashSet<string> Interactions { get; }

        /// <summary>Gets the search includes.</summary>
        /// <value>The search includes.</value>
        public HashSet<string> SearchIncludes { get; }
    }
}
