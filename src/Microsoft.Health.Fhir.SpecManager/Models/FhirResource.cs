using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Health.Fhir.SpecManager.Models
{
    ///-------------------------------------------------------------------------------------------------
    /// <summary>A fhir resource.</summary>
    ///
    /// <remarks>Gino Canessa, 2/6/2020.</remarks>
    ///-------------------------------------------------------------------------------------------------

    public class FhirResource : FhirTypeBase
    {
                                                public FhirResource()
        {
            Properties = new Dictionary<string, FhirProperty>();
        }

                                                            }
}
