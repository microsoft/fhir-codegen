// <copyright file="FirelySerialization.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using BenchmarkDotNet.Attributes;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;

namespace PerfTestCS.Benchmark
{
    /// <summary>Benchmark class for Firely .Net library serialization / parsing.</summary>
    public class FirelySerialization : BenchmarkBase
    {
        /// <summary>The JSON parser.</summary>
        private FhirJsonParser _jsonParser;

        /// <summary>Gets the FHIR JSON serializer.</summary>
        private FhirJsonSerializer _jsonSerializer;

        /// <summary>The JSON.</summary>
        private string _json;

        /// <summary>The model.</summary>
        private Base _model;

        /// <summary>Initializes a new instance of the <see cref="FirelySerialization"/> class.</summary>
        public FirelySerialization()
        {
            _jsonParser = new FhirJsonParser(new ParserSettings()
            {
                AcceptUnknownMembers = true,
                AllowUnrecognizedEnums = true,
            });

            _jsonSerializer = new FhirJsonSerializer(new SerializerSettings()
            {
                AppendNewLine = false,
                Pretty = false,
            });
        }

        /// <summary>Global setup, executed once per parameter value.</summary>
        public override void GlobalSetup()
        {
            if (!File.Exists(Filename))
            {
                throw new FileNotFoundException();
            }

            _json = File.ReadAllText(Filename);
            _model = _jsonParser.Parse(_json);
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
            return _jsonParser.Parse(_json);
        }

        /// <summary>Serialize this object to the given stream.</summary>
        /// <returns>A string.</returns>
        public override string Serialize()
        {
            return _jsonSerializer.SerializeToString(_model);
        }
    }
}
