// <copyright file="PackagesTests.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Health.Fhir.CodeGen.Tests.Extensions;

namespace Microsoft.Health.Fhir.CodeGen.Tests;

public class PackagesTests
{
    // IniData has been removed temporarily - may be added back later
    //[Theory]
    //[FileName("TestData/Packages/packages-01-empty.ini")]
    //public void LoadPackageIni(string path)
    //{
    //    _ForPackages.IniData iniData = new(path);

    //    iniData.Sections.Count.Should().Be(5);

    //    ((_ForPackages.IniData.IniSection?)iniData.Sections[0])?.Name.Should().Be("cache");
    //    ((_ForPackages.IniData.IniSection?)iniData.Sections[1])?.Name.Should().Be("urls");
    //    ((_ForPackages.IniData.IniSection?)iniData.Sections[2])?.Name.Should().Be("local");
    //    ((_ForPackages.IniData.IniSection?)iniData.Sections[3])?.Name.Should().Be("packages");
    //    ((_ForPackages.IniData.IniSection?)iniData.Sections[4])?.Name.Should().Be("package-sizes");

    //    iniData.Sections[0].Should().Be(iniData["cache"]);
    //    iniData.Sections[1].Should().Be(iniData["urls"]);
    //    iniData.Sections[2].Should().Be(iniData["local"]);
    //    iniData.Sections[3].Should().Be(iniData["packages"]);
    //    iniData.Sections[4].Should().Be(iniData["package-sizes"]);

    //    ((_ForPackages.IniData.IniSection?)iniData.Sections[0])?.Values.Count.Should().NotBe(0);
    //    ((_ForPackages.IniData.IniSection?)iniData.Sections[0])?[0]!.Key.Should().Be("version");
    //    ((_ForPackages.IniData.IniSection?)iniData.Sections[0])?[0]!.Value.Should().Be("3");
    //}
}
