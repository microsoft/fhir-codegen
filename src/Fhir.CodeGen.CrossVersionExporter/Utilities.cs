using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hl7.Fhir.Utility;

namespace Fhir.CodeGen.CrossVersionExporter;

internal static class Utilities
{
    public static T? EnumConvert<T, U>(U? r5)
        where T: struct, Enum
        where U: struct, Enum
    {
        if (r5 is null)
        {
            return default(T?); 
        }

        return EnumUtility.ParseLiteral<T>(EnumUtility.GetLiteral(r5), ignoreCase: true);
    }
}
