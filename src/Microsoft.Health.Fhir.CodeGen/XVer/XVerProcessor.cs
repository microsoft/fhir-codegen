// <copyright file="XVerProcessor.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Health.Fhir.CodeGen.Configuration;
using Microsoft.Health.Fhir.CodeGen.Models;
using Microsoft.Health.Fhir.CodeGenCommon.Packaging;

namespace Microsoft.Health.Fhir.CodeGen.XVer;

public class XVerProcessor
{
    public class XVerValueSet
    {
        public required string Name { get; init; }

        public required string Url { get; init; }

        public required FhirReleases.FhirSequenceCodes FirstAppearedIn { get; init; }


    }

    private ConfigXVer _config;
    private Dictionary<string, DefinitionCollection> _definitions;

    public XVerProcessor(ConfigXVer config, Dictionary<string, DefinitionCollection> definitions)
    {
        _config = config;
        _definitions = definitions;
    }
}
