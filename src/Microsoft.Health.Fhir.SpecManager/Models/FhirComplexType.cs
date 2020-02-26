// -------------------------------------------------------------------------------------------------
// <copyright file="FhirComplexType.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>
// -------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Health.Fhir.SpecManager.Models
{
    /// <summary>A class representing a FHIR complex type.</summary>
    public class FhirComplexType : FhirTypeBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FhirComplexType"/> class.
        /// </summary>
        public FhirComplexType()
        {
            Properties = new Dictionary<string, FhirProperty>();
        }
    }
}
