// <copyright file="BasicNewtonsoftSerialization.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using BenchmarkDotNet.Attributes;
using Newtonsoft.Json;

namespace PerfTestCS.Benchmark
{
    /// <summary>Benchmark class for Basic NewtonSoft library serialization / parsing.</summary>
    public class BasicNewtonsoftSerialization : BenchmarkBase
    {
        /// <summary>The JSON.</summary>
        private string _json;

        /// <summary>The model.</summary>
        private fhir.Resource _model;

        /// <summary>The converter.</summary>
        private JsonConverter _converter;

        /// <summary>
        /// Initializes a new instance of the <see cref="BasicNewtonsoftSerialization"/> class.
        /// </summary>
        public BasicNewtonsoftSerialization()
        {
            _converter = new fhir.ResourceConverter();
        }

        /// <summary>Global setup, executed once per parameter value.</summary>
        public override void GlobalSetup()
        {
            if (!File.Exists(Filename))
            {
                throw new FileNotFoundException();
            }

            _json = File.ReadAllText(Filename);
            _model = JsonConvert.DeserializeObject<fhir.Resource>(_json, _converter);
        }

        /// <summary>Global cleanup.</summary>
        public override void GlobalCleanup()
        {
            _json = null;
            _model = null;
        }

        /// <summary>Parses a specified file contents from memory.</summary>
        /// <returns>An object.</returns>
        public override object Parse()
        {
            return JsonConvert.DeserializeObject<fhir.Resource>(_json, _converter);
        }

        /// <summary>Serialize this object to the given stream.</summary>
        /// <returns>A string.</returns>
        public override string Serialize()
        {
            return JsonConvert.SerializeObject(_model);
        }
    }
}
