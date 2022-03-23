// <copyright file="INavTracker.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

namespace FhirCodeGenBlazor.Models;

/// <summary>Interface for navigation tracker.</summary>
public interface INavTracker
{
    /// <summary>Notifies a navigation.</summary>
    /// <param name="page"> The page.</param>
    /// <param name="link"> The link.</param>
    /// <param name="depth">The depth.</param>
    void NotifyNav(string page, string link, int depth);
}
