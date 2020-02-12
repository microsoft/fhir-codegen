using System;
using System.Diagnostics;
using System.IO;
using CommandLine;
using Microsoft.Health.Fhir.SpecManager.Manager;
using Microsoft.Health.Fhir.SpecManager.Models;

namespace fhir_codegen_cli
{
    class Program
    {
        ///-------------------------------------------------------------------------------------------------
        /// <summary>Main entry-point for this application.</summary>
        ///
        /// <remarks>Gino Canessa, 2/3/2020.</remarks>
        ///
        /// <param name="args">An array of command-line argument strings.</param>
        ///-------------------------------------------------------------------------------------------------

        static void Main(string[] args)
        {
            bool success = false;

            // **** start timing ****

            Stopwatch timingWatch = Stopwatch.StartNew();

            // **** process based on command line arguments ****

            Parser.Default.ParseArguments<Options>(args)
                .WithParsed<Options>(options => {
                    success = Process(options);
                })
                .WithNotParsed(errors => { Console.WriteLine("Invalid arguments"); });

            // **** done ****

            long elapsedMs = timingWatch.ElapsedMilliseconds;

            Console.WriteLine($"Finished {success}: {elapsedMs / 1000.0} s");
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>Main processing function</summary>
        ///
        /// <remarks>Gino Canessa, 2/3/2020.</remarks>
        ///
        /// <param name="options">Options for controlling the operation.</param>
        ///
        /// <returns>True if it succeeds, false if it fails.</returns>
        ///-------------------------------------------------------------------------------------------------

        static bool Process(Options options)
        {
            // **** check to make sure we have a directory to work from ****

            if (string.IsNullOrEmpty(options.npmDirectory))
            {
                Console.WriteLine($"Invalid NPM Directory: {options.npmDirectory}");
                return false;
            }

            // **** make sure the directory exists ****

            if (!Directory.Exists(options.npmDirectory))
            {
                Console.WriteLine($"NPM directory not found: {options.npmDirectory}");
                return false;
            }

            // **** check for loading V2 ****

            if (options.loadV2)
            {
                if (!LoaderV2.LoadFhirV2(options.npmDirectory, out InfoV2 fhirInfoV2))
                {
                    Console.WriteLine("Loading v2 failed!");
                    return false;
                }
            }

            // **** check for loading V3 ****

            if (options.loadV3)
            {
                if (!LoaderV3.LoadFhirV3(options.npmDirectory, out InfoV3 fhirInfoV3))
                {
                    Console.WriteLine("Loading v3 failed!");
                    return false;
                }
            }

            // **** check for loading V4 ****

            if (options.loadV4)
            {
                if (!LoaderV4.LoadFhirV4(options.npmDirectory, out InfoV4 fhirInfoV4))
                {
                    Console.WriteLine("Loading v4 failed!");
                    return false;
                }
            }

            // **** still here means success ****

            return true;
        }

    }
}
