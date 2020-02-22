using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Health.Fhir.SpecManager.Models
{
    ///-------------------------------------------------------------------------------------------------
    /// <summary>A FHIR primitive type.</summary>
    ///
    /// <remarks>Gino Canessa, 2/5/2020.</remarks>
    ///-------------------------------------------------------------------------------------------------

    public class FhirSimpleType : FhirTypeBase
    {
                                ///-------------------------------------------------------------------------------------------------
        /// <summary>Gets or sets a value indicating whether this object is primitive.</summary>
        ///
        /// <value>True if this object is primitive, false if not.</value>
        ///-------------------------------------------------------------------------------------------------

        public bool IsPrimitive { get; set; }

                                                                            }
}
