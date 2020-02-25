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
    /// -------------------------------------------------------------------------------------------------
    /// <summary>Interface for converter.</summary>
    /// -------------------------------------------------------------------------------------------------
    public interface IFhirConverter
    {
        /// -------------------------------------------------------------------------------------------------
        /// <summary>Attempts to parse resource an object from the given string.</summary>
        ///
        /// <param name="json">The JSON.</param>
        /// <param name="resource"> [out] The object.</param>
        ///
        /// <returns>True if it succeeds, false if it fails.</returns>
        /// -------------------------------------------------------------------------------------------------
        bool TryParseResource(string json, out object resource);

        /// -------------------------------------------------------------------------------------------------
        /// <summary>Attempts to process resource.</summary>
        ///
        /// <param name="resourceToParse">[out] The resource object.</param>
        /// <param name="simpleTypes">    [in,out] Simple types.</param>
        /// <param name="complexTypes">   [in,out] Complex types.</param>
        /// <param name="resources">      [in,out] Resources.</param>
        ///
        /// <returns>True if it succeeds, false if it fails.</returns>
        /// -------------------------------------------------------------------------------------------------
        bool TryProcessResource(
            object resourceToParse,
            ref Dictionary<string, FhirSimpleType> simpleTypes,
            ref Dictionary<string, FhirComplexType> complexTypes,
            ref Dictionary<string, FhirResource> resources);
    }
}
