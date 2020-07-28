// <copyright file="FhirLiteralAttribute.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Health.Fhir.SpecManager.Extensions
{
    /// <summary>Attribute for FHIR enum values to link strings and .Net equivalents.</summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class FhirLiteralAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FhirLiteralAttribute"/> class.
        /// </summary>
        /// <param name="fhirLiteral">The FHIR literal.</param>
        public FhirLiteralAttribute(string fhirLiteral)
        {
            FhirLiteral = fhirLiteral;
        }

        /// <summary>Gets or sets the FHIR literal.</summary>
        public string FhirLiteral { get; protected set; }
    }
}
