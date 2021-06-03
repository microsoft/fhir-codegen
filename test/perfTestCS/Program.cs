// <copyright file="Program.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Enumeration;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;
using Fhir.R4.Serialization;
using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using Hl7.Fhir.Serialization;
using Newtonsoft.Json;

namespace PerfTestCS
{
    /// <summary>A program.</summary>
    public static class Program
    {
        /// <summary>Main entry-point for this application.</summary>
        /// <returns>Exit-code for the process - 0 for success, else an error code.</returns>
        public static int Main()
        {
            Summary summary = BenchmarkRunner.Run<Benchmark.SerializationBenchmarks>(
                DefaultConfig.Instance
                    .WithOption(ConfigOptions.DisableLogFile, true)
                    .WithOption(ConfigOptions.KeepBenchmarkFiles, true)
                    .WithOption(ConfigOptions.JoinSummary, true));

            // success
            return 0;
        }

    }
}
