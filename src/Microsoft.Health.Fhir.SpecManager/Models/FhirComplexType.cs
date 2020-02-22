using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Health.Fhir.SpecManager.Models
{
    ///-------------------------------------------------------------------------------------------------
    /// <summary>A FHIR complex type.</summary>
    ///
    /// <remarks>Gino Canessa, 2/5/2020.</remarks>
    ///-------------------------------------------------------------------------------------------------

    public class FhirComplexType : FhirTypeBase
    {
                                                public FhirComplexType()
        {
            Properties = new Dictionary<string, FhirProperty>();
        }

                                                            }
}
