using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Health.Fhir.SpecManager.Models
{
    ///-------------------------------------------------------------------------------------------------
    /// <summary>A FHIR property (in a complex-type or resource).</summary>
    ///
    /// <remarks>Gino Canessa, 2/5/2020.</remarks>
    ///-------------------------------------------------------------------------------------------------

    public class FhirProperty : FhirTypeBase
    {
        #region Class Variables . . .

        #endregion Class Variables . . .

        #region Instance Variables . . .

        ///-------------------------------------------------------------------------------------------------
        /// <summary>Gets or sets the cardinality minimum.</summary>
        ///
        /// <value>The cardinality minimum.</value>
        ///-------------------------------------------------------------------------------------------------

        public int CardinalityMin { get; set; }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>Gets or sets the cardinaltiy maximum, -1 for unbounded (e.g., *).</summary>
        ///
        /// <value>The cardinaltiy maximum.</value>
        ///-------------------------------------------------------------------------------------------------

        public int? CardinaltiyMax { get; set; }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>Gets or sets Code Values allowed for this property.</summary>
        ///
        /// <value>The code values.</value>
        ///-------------------------------------------------------------------------------------------------

        public string CodesName { get; set; }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>Gets or sets URL of the value set.</summary>
        ///
        /// <value>The value set URL.</value>
        ///-------------------------------------------------------------------------------------------------

        public string ValueSetUrl { get; set; }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>Gets or sets a value indicating whether this object is inherited.</summary>
        ///
        /// <value>True if this object is inherited, false if not.</value>
        ///-------------------------------------------------------------------------------------------------

        public bool IsInherited { get; set; }

        #endregion Instance Variables . . .

        #region Constructors . . .

        #endregion Constructors . . .

        #region Class Interface . . .

        #endregion Class Interface . . .

        #region Instance Interface . . .

        ///-------------------------------------------------------------------------------------------------
        /// <summary>Gets a value indicating whether this property is an array.</summary>
        ///
        /// <value>True if this object is array, false if not.</value>
        ///-------------------------------------------------------------------------------------------------

        public bool IsArray
        {
            get
            {
                return ((CardinaltiyMax == -1) || (CardinaltiyMax > 1));
            }
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>Gets a value indicating whether this object is optional.</summary>
        ///
        /// <value>True if this object is optional, false if not.</value>
        ///-------------------------------------------------------------------------------------------------

        public bool IsOptional
        {
            get
            {
                return (CardinalityMin == 0);
            }
        }

        #endregion Instance Interface . . .

        #region Internal Functions . . .

        #endregion Internal Functions . . .

    }
}
