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
        /// <param name="outputPath">    The output filename.</param>
        /// <param name="isPartOfBatch"> True if is part of batch, false if not.</param>
        /// <returns>A List of files written by the export operation.</returns>
        public static List<string> Export(
            FhirVersionInfo sourceFhirInfo,
            ILanguage exportLanguage,
            ExporterOptions options,
            string outputPath,
            bool isPartOfBatch)
        {
            List<string> filesWritten = new List<string>();

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

            if (string.IsNullOrEmpty(outputPath))
            {
                throw new ArgumentNullException(nameof(outputPath));
            }

            // check for rooted vs relative
            if (!Path.IsPathRooted(outputPath))
            {
                outputPath = Path.Combine(Directory.GetCurrentDirectory(), outputPath);
            }

            // resolve directory oddness (e.g., follow ..'s, etc.)
            outputPath = Path.GetFullPath(outputPath);

            string tempDir;
            if (Path.HasExtension(outputPath))
            {
                tempDir = Path.Combine(Path.GetDirectoryName(outputPath), $"{exportLanguage.LanguageName}-{DateTime.Now.Ticks}");
            }
            else
            {
                tempDir = Path.Combine(outputPath, $"{exportLanguage.LanguageName}-{DateTime.Now.Ticks}");
            }

            if (!Directory.Exists(tempDir))
            {
                Directory.CreateDirectory(tempDir);
            }

            bool copyPrimitives = false;
            if (exportLanguage.RequiredExportClassTypes.Contains(ExporterOptions.FhirExportClassType.PrimitiveType) ||
                options.OptionalClassTypesToExport.Contains(ExporterOptions.FhirExportClassType.PrimitiveType))
            {
                copyPrimitives = true;
            }

            bool copyComplexTypes = false;
            if (exportLanguage.RequiredExportClassTypes.Contains(ExporterOptions.FhirExportClassType.ComplexType) ||
                options.OptionalClassTypesToExport.Contains(ExporterOptions.FhirExportClassType.ComplexType))
            {
                copyComplexTypes = true;
            }

            bool copyResources = false;
            if (exportLanguage.RequiredExportClassTypes.Contains(ExporterOptions.FhirExportClassType.Resource) ||
                options.OptionalClassTypesToExport.Contains(ExporterOptions.FhirExportClassType.Resource))
            {
                copyResources = true;
            }

            // create a copy of the FHIR information for use in this export
            FhirVersionInfo info = sourceFhirInfo.CopyForExport(
                exportLanguage.FhirPrimitiveTypeMap,
                options.ExportList,
                copyPrimitives,
                copyComplexTypes,
                copyResources,
                true,
                options.ExtensionUrls,
                options.ExtensionElementPaths);

            // perform our export
            exportLanguage.Export(
                info,
                options,
                tempDir);

            string[] exportedFiles = Directory.GetFiles(tempDir, string.Empty, SearchOption.AllDirectories);

            string requestedExtension = string.Empty;

            if (Path.HasExtension(outputPath))
            {
                requestedExtension = Path.GetExtension(outputPath).ToUpperInvariant();
            }

            if (requestedExtension == ".ZIP")
            {
                if (File.Exists(outputPath))
                {
                    File.Delete(outputPath);
                }

                // zip the files in the directory for download/output
                CreateZip(outputPath, tempDir);

                // tell the caller the file we wrote
                filesWritten.Add(outputPath);

                // clean up
                DeleteDirectory(tempDir);

                return filesWritten;
            }

            if ((exportedFiles.Length == 1) &&
                (!string.IsNullOrEmpty(requestedExtension)))
            {
                if (File.Exists(outputPath))
                {
                    File.Delete(outputPath);
                }

                File.Move(exportedFiles[0], outputPath);
                filesWritten.Add(outputPath);

                DeleteDirectory(tempDir);

                return filesWritten;
            }

            if (exportedFiles.Length == 1)
            {
                string filename = Path.Combine(
                    outputPath,
                    $"{exportLanguage.LanguageName}_R{info.MajorVersion}");

                filename = Path.ChangeExtension(filename, exportLanguage.SingleFileExportExtension);

                if (File.Exists(filename))
                {
                    File.Delete(filename);
                }

                File.Move(exportedFiles[0], filename);
                filesWritten.Add(filename);

                DeleteDirectory(tempDir);

                return filesWritten;
            }

            string path = outputPath;

            if (isPartOfBatch)
            {
                path = Path.Combine(outputPath, $"{exportLanguage.LanguageName}_R{info.MajorVersion}");
            }

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            int duplicateLen = tempDir.Length + 1;

            foreach (string file in exportedFiles)
            {
                string exportName = Path.Combine(path, file.Substring(duplicateLen));

                if (File.Exists(exportName))
                {
                    File.Delete(exportName);
                }

                if (!Directory.Exists(Path.GetDirectoryName(exportName)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(exportName));
                }

                File.Move(file, exportName);

                filesWritten.Add(exportName);
            }

            DeleteDirectory(tempDir);

            return filesWritten;
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
