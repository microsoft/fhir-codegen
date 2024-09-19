// <copyright file="CompareDetailsValueSetsViewModel.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Material.Icons;

namespace fhir_codegen.ViewModels;

public partial class CompareDetailsValueSetsViewModel : ViewModelBase, INavigableViewModel
{
    public static string Label => "Value Set Comparison";
    public static MaterialIconKind IconKind => MaterialIconKind.AlphaVBox;
    public static bool Indented => true;

    public CompareDetailsValueSetsViewModel(object? args = null)
        : base()
    {
    }
}
