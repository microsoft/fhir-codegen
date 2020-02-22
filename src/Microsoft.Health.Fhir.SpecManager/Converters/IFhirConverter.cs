using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Health.Fhir.SpecManager.Manager;
using Microsoft.Health.Fhir.SpecManager.Models;

namespace Microsoft.Health.Fhir.SpecManager.Converters
{
    ///-------------------------------------------------------------------------------------------------
    /// <summary>Interface for converter.</summary>
    ///-------------------------------------------------------------------------------------------------

    public interface IFhirConverter
    {
        ///-------------------------------------------------------------------------------------------------
        /// <summary>Attempts to parse resource an object from the given string.</summary>
        ///
        /// <param name="json">The JSON.</param>
        /// <param name="obj"> [out] The object.</param>
        ///
        /// <returns>True if it succeeds, false if it fails.</returns>
        ///-------------------------------------------------------------------------------------------------

        bool TryParseResource(string json, out object obj);

        ///-------------------------------------------------------------------------------------------------
        /// <summary>Attempts to process resource.</summary>
        ///
        /// <param name="obj">            [out] The object.</param>
        /// <param name="fhirVersionInfo">[in,out] Information describing the fhir version.</param>
        ///
        /// <returns>True if it succeeds, false if it fails.</returns>
        ///-------------------------------------------------------------------------------------------------

        bool TryProcessResource(
            object obj,
            ref Dictionary<string, FhirSimpleType> simpleTypes,
            ref Dictionary<string, FhirComplexType> complexTypes,
            ref Dictionary<string, FhirResource> resources
            );
    }
}
