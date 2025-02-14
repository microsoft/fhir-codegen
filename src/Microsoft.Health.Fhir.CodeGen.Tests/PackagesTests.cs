// <copyright file="PackagesTests.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shouldly;
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

    //    iniData.Sections.Count.ShouldBe(5);

    //    ((_ForPackages.IniData.IniSection?)iniData.Sections[0])?.Name.ShouldBe("cache");
    //    ((_ForPackages.IniData.IniSection?)iniData.Sections[1])?.Name.ShouldBe("urls");
    //    ((_ForPackages.IniData.IniSection?)iniData.Sections[2])?.Name.ShouldBe("local");
    //    ((_ForPackages.IniData.IniSection?)iniData.Sections[3])?.Name.ShouldBe("packages");
    //    ((_ForPackages.IniData.IniSection?)iniData.Sections[4])?.Name.ShouldBe("package-sizes");

    //    iniData.Sections[0].ShouldBe(iniData["cache"]);
    //    iniData.Sections[1].ShouldBe(iniData["urls"]);
    //    iniData.Sections[2].ShouldBe(iniData["local"]);
    //    iniData.Sections[3].ShouldBe(iniData["packages"]);
    //    iniData.Sections[4].ShouldBe(iniData["package-sizes"]);

    //    ((_ForPackages.IniData.IniSection?)iniData.Sections[0])?.Values.Count.ShouldNotBe(0);
    //    ((_ForPackages.IniData.IniSection?)iniData.Sections[0])?[0]!.Key.ShouldBe("version");
    //    ((_ForPackages.IniData.IniSection?)iniData.Sections[0])?[0]!.Value.ShouldBe("3");
    //}
}
