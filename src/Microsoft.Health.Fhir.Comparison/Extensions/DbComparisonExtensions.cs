using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Health.Fhir.CodeGenCommon.Extensions;
using Microsoft.Health.Fhir.CodeGenCommon.Utils;
using Microsoft.Health.Fhir.Comparison.Models;

namespace Microsoft.Health.Fhir.Comparison.Extensions;

public static class DbComparisonExtensions
{
    //public static string GetCompositeName<T>(this DbCanonicalComparison<T> comparison)
    //    where T : DbCanonicalResource
    //{
    //    if (comparison.Target == null)
    //    {
    //        return 
    //            $"{comparison.SourceFhirPackage.ShortName}-" +
    //            $"{FhirSanitizationUtils.SanitizeForProperty(comparison.SourceName).ToPascalCase()}-" +
    //            $"{comparison.TargetFhirPackage.ShortName}";
    //    }

    //    return 
    //        $"{comparison.SourceFhirPackage.ShortName}-" +
    //        $"{FhirSanitizationUtils.SanitizeForProperty(comparison.SourceName).ToPascalCase()}-" +
    //        $"{comparison.TargetFhirPackage.ShortName}-" +
    //        $"{FhirSanitizationUtils.SanitizeForProperty(comparison.Target.Name).ToPascalCase()}";
    //}

    public static string GetCompositeName(this DbFhirPackageComparisonPair packagePair, DbCanonicalResource source, DbCanonicalResource? target)
    {
        if (target == null)
        {
            return
                $"{packagePair.SourcePackage.ShortName}-" +
                $"{FhirSanitizationUtils.SanitizeForProperty(source.Name).ToPascalCase()}-" +
                $"{packagePair.TargetPackage.ShortName}";
        }

        return
            $"{packagePair.SourcePackage.ShortName}-" +
            $"{FhirSanitizationUtils.SanitizeForProperty(source.Name).ToPascalCase()}-" +
            $"{packagePair.TargetPackage.ShortName}-" +
            $"{FhirSanitizationUtils.SanitizeForProperty(target.Name).ToPascalCase()}";
    }
}
