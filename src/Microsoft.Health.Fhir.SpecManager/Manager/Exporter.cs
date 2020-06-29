// <copyright file="Exporter.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using Microsoft.Health.Fhir.SpecManager.Language;
using Microsoft.Health.Fhir.SpecManager.Models;

namespace Microsoft.Health.Fhir.SpecManager.Manager
{
    /// <summary>An exporter.</summary>
    public abstract class Exporter
    {
        /// <summary>Exports.</summary>
        /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
        /// <param name="sourceFhirInfo">Information describing the source FHIR version information.</param>
        /// <param name="exportLanguage">The export language.</param>
        /// <param name="options">       Options for controlling the operation.</param>
        /// <param name="outputFile">    The output filename.</param>
        public static void Export(
            FhirVersionInfo sourceFhirInfo,
            ILanguage exportLanguage,
            ExporterOptions options,
            string outputFile)
        {
            if (sourceFhirInfo == null)
            {
                throw new ArgumentNullException(nameof(sourceFhirInfo));
            }

            if (exportLanguage == null)
            {
                throw new ArgumentNullException(nameof(exportLanguage));
            }

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            if (string.IsNullOrEmpty(outputFile))
            {
                throw new ArgumentNullException(nameof(outputFile));
            }

            string outputDir = Path.GetDirectoryName(outputFile);
            string exportDir = Path.Combine(outputDir, $"{exportLanguage.LanguageName}-{DateTime.Now.Ticks}");

            if (!Directory.Exists(outputDir))
            {
                Directory.CreateDirectory(outputDir);
            }

            if (!Directory.Exists(exportDir))
            {
                Directory.CreateDirectory(exportDir);
            }

            bool copyPrimitives = options.PrimitiveNameStyle != FhirTypeBase.NamingConvention.None;
            bool copyComplexTypes = options.ComplexTypeNameStyle != FhirTypeBase.NamingConvention.None;
            bool copyResources = options.ComplexTypeNameStyle != FhirTypeBase.NamingConvention.None;

            // create a copy of the FHIR information for use in this export
            FhirVersionInfo info = sourceFhirInfo.CopyForExport(
                exportLanguage.FhirPrimitiveTypeMap,
                options.ExportList,
                copyPrimitives,
                copyComplexTypes,
                copyResources,
                true,
                options.ExtensionUrls,
                options.ExtensionElementPaths,
                exportLanguage.SupportsSlicing,
                options.HideRemovedParentFields);

            // perform our export
            exportLanguage.Export(
                info,
                options,
                exportDir);

            // check for being a directory - just copy our process files
            if (Directory.Exists(outputFile))
            {
                string[] exportedFiles = Directory.GetFiles(exportDir);

                foreach (string file in exportedFiles)
                {
                    string exportName = Path.Combine(outputFile, Path.GetFileName(file));

                    if (File.Exists(exportName))
                    {
                        File.Delete(exportName);
                    }

                    File.Move(file, exportName);
                }

                DeleteDirectory(exportDir);

                return;
            }

            // make sure our destination is clear
            if (File.Exists(outputFile))
            {
                File.Delete(outputFile);
            }

            // check for single file
            string[] files = Directory.GetFiles(exportDir);

            if (files.Length == 1)
            {
                File.Move(files[0], outputFile);

                DeleteDirectory(exportDir);

                return;
            }

            string zipName = outputFile;

            if (!zipName.ToUpperInvariant().EndsWith(".ZIP", StringComparison.Ordinal))
            {
                zipName = $"{zipName}.zip";
            }

            // zip the files in the directory for download/output
            CreateZip(zipName, exportDir);

            // clean up
            DeleteDirectory(exportDir);
        }

        /// <summary>Deletes the directory described by dir.</summary>
        /// <param name="dir">The dir.</param>
        private static void DeleteDirectory(string dir)
        {
            try
            {
                foreach (string subDir in Directory.GetDirectories(dir))
                {
                    foreach (string filename in Directory.GetFiles(subDir))
                    {
                        File.Delete(filename);
                    }

                    DeleteDirectory(subDir);
                }

                Directory.Delete(dir);
            }
            catch (IOException)
            {
                // ignore
            }
        }

        /// <summary>Ensures that path ends in separator.</summary>
        /// <param name="path">Full pathname of the file.</param>
        /// <returns>A string.</returns>
        private static string EnsurePathEndsInSeparator(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return path;
            }

            if (path[path.Length - 1] == Path.DirectorySeparatorChar)
            {
                return path;
            }

            if (path[path.Length - 1] == Path.AltDirectorySeparatorChar)
            {
                return path;
            }

            if (path.Contains(Path.DirectorySeparatorChar))
            {
                return $"{path}{Path.DirectorySeparatorChar}";
            }

            return $"{path}{Path.AltDirectorySeparatorChar}";
        }

        /// <summary>Creates a zip file from a directory.</summary>
        /// <param name="outputFilename">Filename of the output file.</param>
        /// <param name="sourceFolder">  Pathname of the source folder.</param>
        private static void CreateZip(
            string outputFilename,
            string sourceFolder)
        {
            string dir = EnsurePathEndsInSeparator(sourceFolder);
            int folderOffset = dir.Length;

            using (FileStream fs = File.Create(outputFilename))
            using (var zipStream = new ZipOutputStream(fs))
            {
                // 0-9, 9 being the highest level of compression
                zipStream.SetLevel(3);

                CompressFolder(dir, zipStream, folderOffset);
            }
        }

        /// <summary>Compress folder.</summary>
        /// <param name="sourceFolder">Pathname of the source folder.</param>
        /// <param name="zipStream">   The zip stream.</param>
        /// <param name="folderOffset">The folder offset.</param>
        private static void CompressFolder(
            string sourceFolder,
            ZipOutputStream zipStream,
            int folderOffset)
        {
            string[] files = Directory.GetFiles(sourceFolder);

            foreach (string filename in files)
            {
                FileInfo fi = new FileInfo(filename);

                // Make the name in zip based on the folder
                string entryName = filename.Substring(folderOffset);

                // Remove drive from name and fix slash direction
                entryName = ZipEntry.CleanName(entryName);

                ZipEntry newEntry = new ZipEntry(entryName)
                {
                    DateTime = fi.LastWriteTime,
                    Size = fi.Length,
                };

                zipStream.PutNextEntry(newEntry);

                // Zip the file in buffered chunks
                byte[] buffer = new byte[4096];
                using (FileStream fsInput = File.OpenRead(filename))
                {
                    StreamUtils.Copy(fsInput, zipStream, buffer);
                }

                zipStream.CloseEntry();
            }

            // Recursively call CompressFolder on all folders in path
            string[] folders = Directory.GetDirectories(sourceFolder);
            foreach (string folder in folders)
            {
                CompressFolder(folder, zipStream, folderOffset);
            }
        }
    }
}
