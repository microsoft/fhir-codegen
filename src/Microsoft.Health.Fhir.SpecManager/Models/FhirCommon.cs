using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Health.Fhir.SpecManager.Models
{
    public abstract class FhirCommon
    {
        #region Sturcture Definition - Standard Status codes . . . 

        public const string StandardStatusDeprecated = "deprecated";
        public const string StandardStatusDraft = "draft";
        public const string StandardStatusExternal = "external";
        public const string StandardStatusInformative = "informative";
        public const string StandardStatusNormative = "normative";
        public const string StandardStatusTrialUse = "trial-use";
        
        public static readonly string[] StandardStatusCodes =
        {
            StandardStatusDeprecated,
            StandardStatusDraft,
            StandardStatusExternal,
            StandardStatusInformative,
            StandardStatusNormative,
            StandardStatusTrialUse
        };

        #endregion Sturcture Definition - Standard Status codes . . . 

    }
}
