// <copyright file="BenchmarkBase.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;
using BenchmarkDotNet.Attributes;

namespace PerfTestCS.Benchmark
{
    /// <summary>A benchmark base.</summary>
    public abstract class BenchmarkBase
    {
        /// <summary>Gets or sets the filename of the file.</summary>
        public string Filename { get; set; }

        /// <summary>Gets the list of filenames we are testing against.</summary>
        public static IEnumerable<string> Filenames => new[]
        {
            "Patient-example.json",
            "Observation-2minute-apgar-score.json",
            // "Bundle-resources.json",
        };

        /// <summary>Global setup.</summary>
        public abstract void GlobalSetup();

        /// <summary>Global cleanup.</summary>
        public abstract void GlobalCleanup();

        /// <summary>Gets the parse.</summary>
        /// <returns>An object.</returns>
        public abstract object Parse();

        /// <summary>Serialize this object to the given stream.</summary>
        /// <returns>A string.</returns>
        public abstract string Serialize();
    }
}
