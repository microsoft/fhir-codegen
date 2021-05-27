// <copyright file="SerializationBenchmarks.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;

namespace PerfTestCS.Benchmark
{
    /// <summary>A serialization benchmarks.</summary>
    [GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
    [CategoriesColumn]
    [MemoryDiagnoser]
    public class SerializationBenchmarks
    {
        /// <summary>The base dir.</summary>
        private string _baseDir;

        /// <summary>The JSON.</summary>
        private string _json;

        /// <summary>The JSON parser.</summary>
        private Hl7.Fhir.Serialization.FhirJsonParser _firelyParser;

        /// <summary>Gets the FHIR JSON serializer.</summary>
        private Hl7.Fhir.Serialization.FhirJsonSerializer _firelySerializer;

        /// <summary>The model.</summary>
        private Hl7.Fhir.Model.Base _firelyModel;

        /// <summary>The basic newtonsoft converter.</summary>
        private Newtonsoft.Json.JsonConverter _basicNewtonsoftConverter;

        /// <summary>The basic newtonsoft model.</summary>
        private fhir.Resource _basicNewtonsoftModel;

        /// <summary>The basic system JSON model.</summary>
        private fhirCsR4.Models.Resource _basicSystemJsonModel;

        /// <summary>The extent converter.</summary>
        private Hl7.Fhir.Serialization.JsonStreamResourceConverter _extConverter;

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializationBenchmarks"/> class.
        /// </summary>
        public SerializationBenchmarks()
        {
            string currentDir = Directory.GetCurrentDirectory();
            string testDir = Path.Combine(currentDir, "fhirVersions");

            while (!Directory.Exists(testDir))
            {
                currentDir = Path.GetFullPath(Path.Combine(currentDir, ".."));

                if (currentDir == Path.GetPathRoot(currentDir))
                {
                    throw new DirectoryNotFoundException("Could not find spec directory in path!");
                }

                testDir = Path.Combine(currentDir, "fhirVersions");
            }

            _baseDir = Path.Combine(new[] { testDir, "hl7.fhir.r4.examples-4.0.1", "package" });

            if (!Directory.Exists(_baseDir))
            {
                throw new DirectoryNotFoundException($"Could not find packaged examples in: {_baseDir}");
            }

            Console.WriteLine($"Set base directory: {_baseDir}");

            _firelyParser = new Hl7.Fhir.Serialization.FhirJsonParser(new Hl7.Fhir.Serialization.ParserSettings()
            {
                AcceptUnknownMembers = true,
                AllowUnrecognizedEnums = true,
            });

            _firelySerializer = new Hl7.Fhir.Serialization.FhirJsonSerializer(new Hl7.Fhir.Serialization.SerializerSettings()
            {
                AppendNewLine = false,
                Pretty = false,
            });

            _basicNewtonsoftConverter = new fhir.ResourceConverter();

            _extConverter = new Hl7.Fhir.Serialization.JsonStreamResourceConverter();

            _json = null;
            _firelyModel = null;
        }

        /// <summary>Gets or sets the filename of the file.</summary>
        [ParamsSource(nameof(Filenames))]
        public string Filename { get; set; }

        /// <summary>Gets the list of filenames we are testing against.</summary>
        public static IEnumerable<string> Filenames => new[]
        {
            "Patient-example.json",
            "Observation-2minute-apgar-score.json",
            // "Bundle-resources.json",
        };

        /// <summary>Global cleanup.</summary>
        [GlobalCleanup]
        public void GlobalCleanup()
        {
        }

        /// <summary>Global setup, executed once per parameter value.</summary>
        [GlobalSetup(Targets = new[] { nameof(FirelyParse), nameof(FirelySerialize) })]
        public void FirelySetup()
        {
            string filename = Path.Combine(_baseDir, Filename);

            if (!File.Exists(filename))
            {
                throw new FileNotFoundException();
            }

            if (string.IsNullOrEmpty(_json))
            {
                _json = File.ReadAllText(filename);
                Console.WriteLine($"Loaded {Filename}, {_json.Length} bytes");
            }

            if (_firelyModel == null)
            {
                _firelyModel = _firelyParser.Parse(_json);
            }
        }

        /// <summary>Parses a specified file contents from memory.</summary>
        /// <returns>An object.</returns>
        [BenchmarkCategory("Parse")]
        [Benchmark(Baseline = true)]
        public object FirelyParse()
        {
            return _firelyParser.Parse(_json);
        }

        /// <summary>Serialize this object to the given stream.</summary>
        /// <returns>A string.</returns>
        [BenchmarkCategory("Serialize")]
        [Benchmark(Baseline = true)]
        public string FirelySerialize()
        {
            string test = _firelySerializer.SerializeToString(_firelyModel);

            return test;
        }

        /// <summary>Global setup, executed once per parameter value.</summary>
        [GlobalSetup(Targets = new[] { nameof(FirelyExtParse), nameof(FirelyExtSerialize) })]
        public void FirelyExtSetup()
        {
            string filename = Path.Combine(_baseDir, Filename);

            if (!File.Exists(filename))
            {
                throw new FileNotFoundException();
            }

            if (string.IsNullOrEmpty(_json))
            {
                _json = File.ReadAllText(filename);
                Console.WriteLine($"Loaded {Filename}, {_json.Length} bytes");
            }

            if (_firelyModel == null)
            {
                _firelyModel = _firelyParser.Parse(_json);
            }
        }

        /// <summary>Parses a specified file contents from memory.</summary>
        /// <returns>An object.</returns>
        [BenchmarkCategory("Parse")]
        [Benchmark]
        public object FirelyExtParse()
        {
            return System.Text.Json.JsonSerializer.Deserialize(
                _json,
                typeof(Hl7.Fhir.Model.Resource),
                Hl7.Fhir.Serialization.FhirSerializerOptions.SerializerCompact);
        }

        /// <summary>Serialize this object to the given stream.</summary>
        /// <returns>A string.</returns>
        [BenchmarkCategory("Serialize")]
        [Benchmark]
        public string FirelyExtSerialize()
        {
            string test = System.Text.Json.JsonSerializer.Serialize<object>(
                _firelyModel,
                Hl7.Fhir.Serialization.FhirSerializerOptions.SerializerCompact);

            return test;
        }

#if !CAKE // 2021.05.24 - just testing Firely right now
        /// <summary>Basic newtonsoft setup.</summary>
        [GlobalSetup(Targets = new[] { nameof(BasicNewtonsoftParse), nameof(BasicNewtonsoftSerialize) })]
        public void BasicNewtonsoftSetup()
        {
            string filename = Path.Combine(_baseDir, Filename);

            if (!File.Exists(filename))
            {
                throw new FileNotFoundException();
            }

            if (string.IsNullOrEmpty(_json))
            {
                _json = File.ReadAllText(filename);
            }

            if (_basicNewtonsoftModel == null)
            {
                _basicNewtonsoftModel = Newtonsoft.Json.JsonConvert.DeserializeObject<fhir.Resource>(_json, _basicNewtonsoftConverter);
            }
        }

        /// <summary>Basic newtonsoft parse.</summary>
        /// <returns>An object.</returns>
        [BenchmarkCategory("Parse")]
        [Benchmark]
        public object BasicNewtonsoftParse()
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<fhir.Resource>(_json, _basicNewtonsoftConverter);
        }

        /// <summary>Basic newtonsoft serialize.</summary>
        /// <returns>A string.</returns>
        [BenchmarkCategory("Serialize")]
        [Benchmark]
        public string BasicNewtonsoftSerialize()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(_basicNewtonsoftModel);
        }

        /// <summary>Basic system JSON setup.</summary>
        /// <exception cref="FileNotFoundException">Thrown when the requested file is not present.</exception>
        [GlobalSetup(Targets = new[] { nameof(BasicSystemJsonParse), nameof(BasicSystemJsonSerialize) })]
        public void BasicSystemJsonSetup()
        {
            string filename = Path.Combine(_baseDir, Filename);

            if (!File.Exists(filename))
            {
                throw new FileNotFoundException();
            }

            if (string.IsNullOrEmpty(_json))
            {
                _json = File.ReadAllText(filename);
            }

            if (_basicSystemJsonModel == null)
            {
                _basicSystemJsonModel = System.Text.Json.JsonSerializer.Deserialize<fhirCsR4.Models.Resource>(_json);
            }
        }

        /// <summary>Basic system JSON parse.</summary>
        /// <returns>An object.</returns>
        [BenchmarkCategory("Parse")]
        [Benchmark]
        public object BasicSystemJsonParse()
        {
            return System.Text.Json.JsonSerializer.Deserialize<fhirCsR4.Models.Resource>(_json);
        }

        /// <summary>Basic system JSON serialize.</summary>
        /// <returns>A string.</returns>
        [BenchmarkCategory("Serialize")]
        [Benchmark]
        public string BasicSystemJsonSerialize()
        {
            return System.Text.Json.JsonSerializer.Serialize(_basicSystemJsonModel);
        }
#endif

    }
}
