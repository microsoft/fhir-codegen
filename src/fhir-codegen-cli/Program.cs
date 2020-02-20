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
            // **** initialize the FHIR version manager with our requested directory ****

            FhirManager.Init(options.NpmDirectory);



            //// **** test downloader ****

            //FhirPackageDownloader downloader = new FhirPackageDownloader();
            //downloader.DownloadPublishedPackage(
            //    LoaderV4.PackageReleaseName,
            //    LoaderV4.PackageName,
            //    options.NpmDirectory
            //    );
            //downloader.CheckInstalledVersions(options.NpmDirectory);


            // **** check for loading V2 ****

            if (options.LoadV2)
            {
                if (!FhirManager.Current.LoadPublished(2))
                {
                    Console.WriteLine("Loading v2 failed!");
                    return false;
                }
            }
            return true;

            //// **** check for loading V3 ****

            //if (options.LoadV3)
            //{
            //    if (!LoaderV3.LoadPackage(options.NpmDirectory, out InfoV3 fhirInfoV3))
            //    {
            //        Console.WriteLine("Loading v3 failed!");
            //        return false;
            //    }
            //}

            //// **** check for loading V4 ****

            //if (options.LoadV4)
            //{
            //    if (!LoaderV4.LoadPackage(options.NpmDirectory, out InfoV4 fhirInfoV4))
            //    {
            //        Console.WriteLine("Loading v4 failed!");
            //        return false;
            //    }
            //}

            //// **** still here means success ****

            //return true;
        }

    }
}
