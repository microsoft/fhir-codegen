// <copyright file="INavigablePage.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Media;
using Material.Icons;
using Material.Icons.Avalonia;

namespace fhir_codegen.ViewModels;

internal interface INavigableViewModel
{
    public static string Label { get; } = " - ";

    public static MaterialIconKind IconKind { get; }
}
