// <copyright file="ComparisonResults.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Health.Fhir.CodeGen.CompareTool;
using Microsoft.Health.Fhir.CodeGen.Models;

namespace fhir_codegen.Models;

public partial class ComparisonUiModel : ObservableObject
{
    [ObservableProperty]
    private DefinitionCollection? _source = null;

    [ObservableProperty]
    private DefinitionCollection? _target = null;

    [ObservableProperty]
    private PackageComparison? _results = null;
}
