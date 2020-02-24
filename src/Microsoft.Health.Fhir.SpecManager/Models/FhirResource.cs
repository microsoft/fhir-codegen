using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Health.Fhir.SpecManager.Models
{
    /// -------------------------------------------------------------------------------------------------
    /// <summary>A fhir resource.</summary>
    /// -------------------------------------------------------------------------------------------------
    public class FhirResource : FhirTypeBase
    {
        public FhirResource()
        {
            Properties = new Dictionary<string, FhirProperty>();
        }
    }
}
