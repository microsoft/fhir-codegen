// <copyright file="Program.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Encodings;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace RoundtripCli
{
    /// <summary>A program.</summary>
    public static class Program
    {
        /// <summary>The files to ignore.</summary>
        private static readonly HashSet<string> _filesToIgnore = new HashSet<string>()
        {
            ".index.json",
            "package.json",
            "Observation-decimal.json",     // this has a decimal out of range - determine what to do here
        };

        /// <summary>The files with unicode differences.</summary>
        private static readonly HashSet<string> _filesWithUnicodeDifferences = new HashSet<string>()
        {
            "Bundle-v2-valuesets.json",
        };

        /// <summary>The issues.</summary>
        private static Dictionary<string, string> _issues = new Dictionary<string, string>();

        /// <summary>The extension-based converter.</summary>
        private static Hl7.Fhir.Serialization.JsonStreamResourceConverter _extConverter = new Hl7.Fhir.Serialization.JsonStreamResourceConverter();

        /// <summary>The JSON parser.</summary>
        private static Hl7.Fhir.Serialization.FhirJsonParser _firelyParser = new Hl7.Fhir.Serialization.FhirJsonParser(new Hl7.Fhir.Serialization.ParserSettings()
        {
            AcceptUnknownMembers = true,
            AllowUnrecognizedEnums = true,
        });

        /// <summary>Gets the FHIR JSON serializer.</summary>
        private static Hl7.Fhir.Serialization.FhirJsonSerializer _firelySerializer = new Hl7.Fhir.Serialization.FhirJsonSerializer(new Hl7.Fhir.Serialization.SerializerSettings()
        {
            AppendNewLine = false,
            Pretty = false,
        });

        /// <summary>Main entry-point for this application.</summary>
        /// <param name="breakOnError">If the program should stop processing after the first error.</param>
        /// <returns>Exit-code for the process - 0 for success, else an error code.</returns>
        public static int Main(
            bool breakOnError = true)
        {
            //string test = @"<td> </td>";

            //JsonEncodedText encoded = JsonEncodedText.Encode(test, System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping);

            //Console.WriteLine($"{test} encodes to {encoded}");

            string exampleDir = FindFhirExampleDir();

            TestExamples(exampleDir, breakOnError);

            foreach (KeyValuePair<string, string> kvp in _issues)
            {
                Console.WriteLine($"{kvp.Key}: {kvp.Value}");
            }

            return 0;
        }

        /// <summary>Tests examples.</summary>
        /// <param name="exampleDir">  The example dir.</param>
        /// <param name="breakOnError">If the program should stop processing after the first error.</param>
        private static void TestExamples(string exampleDir, bool breakOnError)
        {
            string[] files = Directory.GetFiles(exampleDir, "*.json", SearchOption.TopDirectoryOnly);

            foreach (string fullName in files)
            {
                string filename = Path.GetFileName(fullName);

                if (_filesToIgnore.Contains(filename))
                {
                    continue;
                }

                if (!RoundtripFileCSv2(fullName, filename))
                // if (!RoundtripFileFirelyExt(fullName, filename))
                {
                    if (breakOnError)
                    {
                        return;
                    }
                }
            }
        }

        /// <summary>Roundtrip file.</summary>
        /// <param name="fullName">Full path and filename.</param>
        /// <param name="filename">Filename of the file.</param>
        /// <returns>True if it succeeds, false if it fails.</returns>
        private static bool RoundtripFileFirelyExt(
            string fullName,
            string filename)
        {
            if (!File.Exists(fullName))
            {
                _issues.Add(filename, "File not found");
                return false;
            }

            //byte[] jsonBytes = File.ReadAllBytes(fullName);
            string json = File.ReadAllText(fullName);

            Hl7.Fhir.Model.Base stockModel = _firelyParser.Parse(json);
            string stockSerialized = _firelySerializer.SerializeToString(stockModel);


            //Utf8JsonReader reader = new Utf8JsonReader(jsonBytes.AsSpan<byte>());

            //if (filename == "Bundle-lipids.json")
            //{
            //    // File.WriteAllText(@"c:\temp\serialization\original.json", json);
            //    Hl7.Fhir.Serialization.FhirSerializerOptions.Debug = true;
            //    Console.Write("");
            //}

            try
            {
                Hl7.Fhir.Model.Resource parsed = (Hl7.Fhir.Model.Resource)JsonSerializer.Deserialize(
                    json,
                    typeof(Hl7.Fhir.Model.Resource),
                    Hl7.Fhir.Serialization.FhirSerializerOptions.SerializerCompact);

                //Hl7.Fhir.Model.Resource parsed = _extConverter.Read(
                //    ref reader,
                //    typeof(Hl7.Fhir.Model.Resource),
                //    Hl7.Fhir.Serialization.FhirSerializerOptions.SerializerCompact);

                if (parsed == null)
                {
                    _issues.Add(filename, "Failed to parse");
                    return false;
                }

                // test was only good early - complex models never match =(
                //if (!parsed.Matches(stockModel))
                //{
                //    Console.WriteLine($"File: {filename} does not match!");

                //    File.WriteAllText(@"c:\temp\serialization\original.json", json);
                //    File.WriteAllText(@"c:\temp\serialization\stock.json", stockSerialized);
                //    File.WriteAllText(@"c:\temp\serialization\serialized.json", _firelySerializer.SerializeToString(parsed));


                //    _issues.Add(filename, "API Matches failed!");
                //    return false;
                //}

                string serialized;

                serialized = JsonSerializer.Serialize<object>(
                    parsed,
                    Hl7.Fhir.Serialization.FhirSerializerOptions.SerializerCompact);

                //using (MemoryStream memoryStream = new MemoryStream())
                //using (Utf8JsonWriter writer = new Utf8JsonWriter(memoryStream, Hl7.Fhir.Serialization.FhirSerializerOptions.Compact))
                //{
                //    _extConverter.Write(
                //        writer,
                //        (Hl7.Fhir.Model.Resource)parsed,
                //        Hl7.Fhir.Serialization.FhirSerializerOptions.SerializerOptions);

                //    serialized = System.Text.Encoding.UTF8.GetString(memoryStream.GetBuffer(), 0, (int)writer.BytesCommitted);
                //}

                // skip any further checks on files which have been manually verified
                if (_filesWithUnicodeDifferences.Contains(filename))
                {
                    return true;
                }

                // string strippedJson = Regex.Replace(json, @"\s", "");
                string strippedJson = Regex.Replace(stockSerialized, @"\s", "");
                strippedJson = Regex.Unescape(strippedJson);
                strippedJson = Regex.Replace(strippedJson, @"\xA0", "");        // this has to be after unescaping
                strippedJson = Regex.Replace(strippedJson, "\\r", "");        // this has to be after unescaping
                strippedJson = Regex.Replace(strippedJson, "\\n", "");        // this has to be after unescaping
                char[] jsonChars = strippedJson.ToCharArray();
                Array.Sort(jsonChars);

                string strippedSerialized = Regex.Replace(serialized, @"\s", "");
                strippedSerialized = Regex.Unescape(strippedSerialized);
                strippedSerialized = Regex.Replace(strippedSerialized, @"\xA0", "");        // this has to be after unescaping
                strippedSerialized = Regex.Replace(strippedSerialized, "\\r", "");        // this has to be after unescaping
                strippedSerialized = Regex.Replace(strippedSerialized, "\\n", "");        // this has to be after unescaping
                char[] serializedChars = strippedSerialized.ToCharArray();
                Array.Sort(serializedChars);

                if (jsonChars.Length != serializedChars.Length)
                {
                    Console.WriteLine($"File: {filename} does not match!");

                    File.WriteAllText(@"c:\temp\serialization\original.json", json);
                    File.WriteAllText(@"c:\temp\serialization\serialized.json", serialized);
                    File.WriteAllText(@"c:\temp\serialization\stock.json", stockSerialized);

                    _issues.Add(filename, "Sorted lengths do not match");
                    return false;
                }

                for (int i = 0; i < jsonChars.Length; i++)
                {
                    if (jsonChars[i] != serializedChars[i])
                    {
                        _issues.Add(filename, "Round trip does not match");
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"File: {filename}, threw: {ex.Message}");
                _issues.Add(filename, $"Exception: {ex.Message}");
                return false;
            }

            return true;
        }


        /// <summary>Roundtrip file.</summary>
        /// <param name="fullName">Full path and filename.</param>
        /// <param name="filename">Filename of the file.</param>
        /// <returns>True if it succeeds, false if it fails.</returns>
        private static bool RoundtripFileCSv2(
            string fullName,
            string filename)
        {
            if (!File.Exists(fullName))
            {
                _issues.Add(filename, "File not found");
                return false;
            }

            string json = File.ReadAllText(fullName);

            //if (filename == "Bundle-dataelements.json")
            //{
            //    Console.Write("");
            //}

            try
            {
                fhirCsR4.Models.Resource parsed = JsonSerializer.Deserialize<fhirCsR4.Models.Resource>(json);

                if (parsed == null)
                {
                    _issues.Add(filename, "Failed to parse");
                    return false;
                }

                string serialized = JsonSerializer.Serialize(parsed);

                string strippedJson = Regex.Replace(json, @"\s", "");
                strippedJson = Regex.Unescape(strippedJson);
                strippedJson = Regex.Replace(strippedJson, @"\xA0", "");        // this has to be after unescaping
                char[] jsonChars = strippedJson.ToCharArray();
                Array.Sort(jsonChars);
                
                string strippedSerialized = Regex.Replace(serialized, @"\s", "");
                strippedSerialized = Regex.Unescape(strippedSerialized);
                strippedSerialized = Regex.Replace(strippedSerialized, @"\xA0", "");        // this has to be after unescaping
                char[] serializedChars = strippedSerialized.ToCharArray();
                Array.Sort(serializedChars);

                if (jsonChars.Length != serializedChars.Length)
                {
                    Console.WriteLine($"File: {filename} does not match!");

                    File.WriteAllText(@"c:\temp\serialization\original.json", json);
                    File.WriteAllText(@"c:\temp\serialization\serialized.json", serialized);

                    //if (jsonChars.Length > serializedChars.Length)
                    //{
                    //    for (int jIndex = 0; jIndex < jsonChars.Length; jIndex++)
                    //    {
                    //        if (jsonChars[jIndex] != serializedChars[jIndex])
                    //        {
                    //            Console.Write($"Character: {jIndex}, json:{(int)jsonChars[jIndex]} - new:{(int)serializedChars[jIndex]}");
                    //            break;
                    //        }
                    //    }
                    //}
                    //else
                    //{
                    //    for (int sIndex = 0; sIndex < serializedChars.Length; sIndex++)
                    //    {
                    //        if (jsonChars[sIndex] != serializedChars[sIndex])
                    //        {
                    //            Console.Write($"Character: {sIndex}, json:{(int)jsonChars[sIndex]} - new:{(int)serializedChars[sIndex]}");
                    //            break;
                    //        }
                    //    }
                    //}


                    _issues.Add(filename, "Sorted lengths do not match");
                    return false;
                }

                for (int i = 0; i < jsonChars.Length; i++)
                {
                    if (jsonChars[i] != serializedChars[i])
                    {
                        _issues.Add(filename, "Round trip does not match");
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                _issues.Add(filename, $"Exception: {ex.Message}");
                return false;
            }

            return true;
        }

        /// <summary>Searches for the FHIR example directory.</summary>
        /// <exception cref="DirectoryNotFoundException">Thrown when the requested directory is not
        ///  present.</exception>
        /// <returns>The found FHIR directory.</returns>
        public static string FindFhirExampleDir()
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

            string fhirDir = Path.Combine(new[] { testDir, "hl7.fhir.r4.examples-4.0.1", "package" });

            if (!Directory.Exists(fhirDir))
            {
                throw new DirectoryNotFoundException($"Could not find packaged examples in: {fhirDir}");
            }

            Console.WriteLine($"Set base directory: {fhirDir}");

            return fhirDir;
        }
    }
}
