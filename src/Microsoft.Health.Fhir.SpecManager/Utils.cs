using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Health.Fhir.SpecManager
{
    ///-------------------------------------------------------------------------------------------------
    /// <summary>Utilities (temp home until they have better ones)</summary>
    ///
    /// <remarks>Gino Canessa, 2/20/2020.</remarks>
    ///-------------------------------------------------------------------------------------------------

    public abstract class Utils
    {

        ///-------------------------------------------------------------------------------------------------
        /// <summary>Gets the parent and field.</summary>
        ///
        /// <remarks>Gino Canessa, 2/20/2020.</remarks>
        ///
        /// <param name="path">  Full pathname of the file.</param>
        /// <param name="field"> [out] The field.</param>
        /// <param name="parent">[out] The parent.</param>
        ///-------------------------------------------------------------------------------------------------

        public static void GetParentAndField(string[] path, out string field, out string parent)
        {
            field = path[path.Length - 1];

            parent = "";
            for (int i = 0; i < path.Length - 1; i++)
            {
                parent += string.Concat(path[i].Substring(0, 1).ToUpper(), path[i].Substring(1));
            }
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>Capitalize name.</summary>
        ///
        /// <remarks>Gino Canessa, 2/20/2020.</remarks>
        ///
        /// <param name="name">The name.</param>
        ///
        /// <returns>A string.</returns>
        ///-------------------------------------------------------------------------------------------------

        public static string CapitalizeName(string name)
        {
            return string.Concat(name.Substring(0, 1).ToUpper(), name.Substring(1));
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>Maximum cardinality.</summary>
        ///
        /// <remarks>Gino Canessa, 2/20/2020.</remarks>
        ///
        /// <param name="max">The maximum.</param>
        ///
        /// <returns>An int?</returns>
        ///-------------------------------------------------------------------------------------------------

        public static int? MaxCardinality(string max)
        {
            if (string.IsNullOrEmpty(max))
            {
                return null;
            }

            if (max.Equals("*", StringComparison.Ordinal))
            {
                return null;
            }

            if (int.TryParse(max, out int parsed))
            {
                return parsed;
            }

            return null;
        }
    }
}
