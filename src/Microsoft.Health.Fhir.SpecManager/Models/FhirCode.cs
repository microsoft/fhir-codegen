using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Health.Fhir.SpecManager.Models
{
    /// -------------------------------------------------------------------------------------------------
    /// <summary>A FHIR Code value object</summary>
    /// -------------------------------------------------------------------------------------------------
    public class FhirCode
    {
        /// <summary>The codes.</summary>
        private static Dictionary<string, string[]> _codes;

        static FhirCode()
        {
            _codes = new Dictionary<string, string[]>();
        }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>Constructor that prevents a default instance of this class from being created.</summary>
        /// -------------------------------------------------------------------------------------------------
        private FhirCode() { }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>Adds the codes to 'values'.</summary>
        ///
        /// <param name="name">  The name.</param>
        /// <param name="values">The values.</param>
        ///
        /// <returns>True if it succeeds, false if it fails.</returns>
        /// -------------------------------------------------------------------------------------------------
        public static bool AddCodes(string name, string[] values)
        {
            if (_codes.ContainsKey(name))
            {
                // should be same
                if (_codes[name].Length == values.Length)
                {
                    return true;
                }

                Console.WriteLine($"Duplicate Code {name} with different values!");
                return false;
            }

            // add this code
            _codes.Add(name, values);

            // success
            return true;
        }

        /// -------------------------------------------------------------------------------------------------
        /// <summary>Attempts to get codes a string[] from the given string.</summary>
        ///
        /// <param name="name">  The name.</param>
        /// <param name="values">[out] The values.</param>
        ///
        /// <returns>True if it succeeds, false if it fails.</returns>
        /// -------------------------------------------------------------------------------------------------
        public static bool TryGetCodes(string name, out string[] values)
        {
            if (!_codes.ContainsKey(name))
            {
                values = null;
                return false;
            }

            values = _codes[name];
            return true;
        }
    }
}
