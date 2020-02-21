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

        public FhirResource()
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
