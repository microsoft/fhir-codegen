﻿// -------------------------------------------------------------------------------------------------
// <copyright file="FhirPrimitive.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>
// -------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Health.Fhir.SpecManager.Models
{
    /// <summary>A class representing a FHIR primitive (r2:simple) type.</summary>
    public class FhirPrimitive : FhirTypeBase
    {
        /// <summary>Initializes a new instance of the <see cref="FhirPrimitive"/> class.</summary>
        /// <param name="path">            Full pathname of the file.</param>
        /// <param name="standardStatus">  The standard status.</param>
        /// <param name="shortDescription">Information describing the short.</param>
        /// <param name="purpose">         The purpose of this definition.</param>
        /// <param name="comment">         The comment.</param>
        /// <param name="validationRegEx"> The validation RegEx.</param>
        public FhirPrimitive(
            string path,
            string standardStatus,
            string shortDescription,
            string purpose,
            string comment,
            string validationRegEx)
            : base(
                path,
                standardStatus,
                shortDescription,
                purpose,
                comment,
                validationRegEx,
                path)
        {
        }
    }
}
