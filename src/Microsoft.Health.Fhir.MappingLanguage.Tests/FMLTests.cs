// <copyright file="FMLTests.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Health.Fhir.MappingLanguage.Tests.Extensions;

namespace Microsoft.Health.Fhir.MappingLanguage.Tests;

public class FMLTests
{
    [Theory]
    [FileData("data/Encounter4Bto5.fml")]
    internal void TestParseEncounter4Bto5(string content)
    {
        FhirMappingLanguage fml = new();

        fml.TryParse(content);
    }
}
