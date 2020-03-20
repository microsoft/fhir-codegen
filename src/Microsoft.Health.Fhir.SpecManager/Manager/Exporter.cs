// <copyright file="Exporter.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Health.Fhir.SpecManager.Language;
using Microsoft.Health.Fhir.SpecManager.Models;

namespace Microsoft.Health.Fhir.SpecManager.Manager
{
    /// <summary>An exporter.</summary>
    public abstract class Exporter
    {
        /// <summary>Exports.</summary>
        /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
        /// <param name="sourceFhirInfo">Information describing the source fhir.</param>
        /// <param name="exportLanguage">The export language.</param>
        /// <param name="options">       Options for controlling the operation.</param>
        /// <param name="outputFile">    The output file.</param>
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

            MemoryStream ms = new MemoryStream();

            // perform our export
            exportLanguage.Export(
                info,
                options,
                ref ms);

            if (!Directory.Exists(Path.GetDirectoryName(outputFile)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(outputFile));
            }

            using (FileStream stream = new FileStream(outputFile, FileMode.Create))
            {
                ms.Seek(0, 0);
                ms.CopyTo(stream);
            }

            ms.Close();
            ms.Dispose();
        }
    }
}
