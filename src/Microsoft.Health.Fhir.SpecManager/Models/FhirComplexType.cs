using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Health.Fhir.SpecManager.Models
{
    ///-------------------------------------------------------------------------------------------------
    /// <summary>A FHIR complex type.</summary>
    ///-------------------------------------------------------------------------------------------------

    public class FhirComplexType : FhirTypeBase
    {
        public FhirComplexType()
        {
            Properties = new Dictionary<string, FhirProperty>();
        }
    }
}
