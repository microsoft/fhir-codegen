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
        #region Class Variables . . .

        #endregion Class Variables . . .

        #region Instance Variables . . .

        ///-------------------------------------------------------------------------------------------------
        /// <summary>Gets or sets the properties.</summary>
        ///
        /// <value>The properties.</value>
        ///-------------------------------------------------------------------------------------------------

        public Dictionary<string, FhirProperty> Properties { get; set; }

        #endregion Instance Variables . . .

        #region Constructors . . .

        public FhirComplexType()
        {
            Properties = new Dictionary<string, FhirProperty>();
        }

        #endregion Constructors . . .

        #region Class Interface . . .

        #endregion Class Interface . . .

        #region Instance Interface . . .

        #endregion Instance Interface . . .

        #region Internal Functions . . .

        #endregion Internal Functions . . .

    }
}
