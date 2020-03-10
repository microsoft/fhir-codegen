// -------------------------------------------------------------------------------------------------
// <copyright file="IFhirConverter.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>
// -------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Health.Fhir.SpecManager.Manager;
using Microsoft.Health.Fhir.SpecManager.Models;

namespace Microsoft.Health.Fhir.SpecManager.Converters
{
    /// <summary>Interface for converter.</summary>
    public interface IFhirConverter
    {
        /// <summary>Parses resource an object from the given string.</summary>
        /// <param name="json">The JSON.</param>
        /// <returns>A typed Resource object.</returns>
        object ParseResource(string json);

        /// <summary>Attempts to process resource.</summary>
        /// <param name="resourceToParse">[out] The resource object.</param>
        /// <param name="fhirVersionInfo">Primitive types.</param>
        void ProcessResource(
            object resourceToParse,
            FhirVersionInfo fhirVersionInfo);
    }
}
