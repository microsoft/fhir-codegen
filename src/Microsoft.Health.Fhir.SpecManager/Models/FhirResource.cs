// -------------------------------------------------------------------------------------------------
// <copyright file="FhirResource.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>
// -------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Health.Fhir.SpecManager.Models
{
    /// <summary>A class representing a FHIR resource.</summary>
    public class FhirResource : FhirTypeBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FhirResource"/> class.
        /// </summary>
        public FhirResource() => Properties = new Dictionary<string, FhirProperty>();
    }
}
