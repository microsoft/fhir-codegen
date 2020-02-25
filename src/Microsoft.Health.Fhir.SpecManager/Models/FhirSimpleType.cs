// -------------------------------------------------------------------------------------------------
// <copyright file="FhirSimpleType.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>
// -------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Health.Fhir.SpecManager.Models
{
    /// -------------------------------------------------------------------------------------------------
    /// <summary>A FHIR primitive type.</summary>
    /// -------------------------------------------------------------------------------------------------
    public class FhirSimpleType : FhirTypeBase
    {
        /// -------------------------------------------------------------------------------------------------
        /// <summary>Gets or sets a value indicating whether this object is primitive.</summary>
        ///
        /// <value>True if this object is primitive, false if not.</value>
        /// -------------------------------------------------------------------------------------------------
        public bool IsPrimitive { get; set; }
    }
}
