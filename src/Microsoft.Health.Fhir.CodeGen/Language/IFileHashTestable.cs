// <copyright file="IFileHasTestable.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Health.Fhir.CodeGen.Language;

internal interface IFileHashTestable
{
    internal bool GenerateHashesInsteadOfOutput { get; set; }

    internal Dictionary<string, string> FileHashes { get; }
}
