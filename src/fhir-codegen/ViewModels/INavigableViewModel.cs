// <copyright file="INavigablePage.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using Material.Icons;

namespace fhir_codegen.ViewModels;

// <summary>
// Represents a view model that can be navigated to.
// </summary>
internal interface INavigableViewModel
{
    /// <summary>
    /// Gets the label for the navigable view model.
    /// </summary>
    public static string Label { get; } = " - ";

    /// <summary>
    /// Gets if this model is indented in layout (only single level for now).
    /// </summary>
    public static bool Indented { get; } = false;

    /// <summary>
    /// Gets the icon kind for the navigable view model.
    /// </summary>
    public static MaterialIconKind IconKind { get; }
}
