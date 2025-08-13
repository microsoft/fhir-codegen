// <copyright file="UsageContext.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using Hl7.Fhir.ElementModel;
using Hl7.Fhir.Model;

namespace Microsoft.Health.Fhir.CrossVersion.Convert_20_50;

public class UsageContext_20_50 // : ICrossVersionProcessor<UsageContext>  //, ICrossVersionExtractor<UsageContext>
{
    /* list pulled from:
     * - github.com:hapifhir/org.hl7.fhir.core
     * - org.hl7.fhir.convertors\src\main\java\org\hl7\fhir\convertors\conv10_50\VersionConvertor_10_50.java
     * - boolean isJurisdiction(@Nonnull CodeableConcept t)
     */
    private static HashSet<string> _jurisdictionSystems = [
        "http://unstats.un.org/unsd/methods/m49/m49.htm",
        "urn:iso:std:iso:3166",
        "https://www.usps.com/",
    ];

	private Converter_20_50 _converter;
	internal UsageContext_20_50(Converter_20_50 converter)
	{
		_converter = converter;
	}

	public (UsageContext? useContext, CodeableConcept? jurisdiction) Extract(ISourceNode node)
	{
        // DSTU2 useContext elements were just CodeableConcept type
        CodeableConcept cc = _converter._codeableConcept.Extract(node);

        // check to see if this is a jurisdiction
        if ((cc.Coding.Count > 0) &&
            cc.Coding.Any(c => _jurisdictionSystems.Contains(c.System)))
        {
            return (null, cc);
        }

        if (cc.Coding.Count == 0)
        {
            throw new Exception("UseContext with no codings!");
        }

        UsageContext uc = new()
        {
            Code = cc.Coding[0],
            Value = cc,
        };

        return (uc, null);
	}
}
