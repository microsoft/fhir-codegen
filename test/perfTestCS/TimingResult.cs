// <copyright file="TimingResult.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;

namespace PerfTestCS
{
    /// <summary>Encapsulates the result of a timing.</summary>
    public class TimingResult
    {
        /// <summary>Initializes a new instance of the <see cref="TimingResult"/> class.</summary>
        /// <param name="filename">   The filename of the file.</param>
        /// <param name="fileSize">   The file size.</param>
        /// <param name="libraryName">The name of the library.</param>
        /// <param name="loops">      The loops.</param>
        public TimingResult(
            string filename,
            long fileSize,
            string libraryName,
            int loops)
        {
            Filename = System.IO.Path.GetFileName(filename);
            FileSize = fileSize;
            LibraryName = libraryName;
            LoopCount = loops;
        }

        /// <summary>Gets the filename of the file.</summary>
        public string Filename { get; }

        /// <summary>Gets the file size.</summary>
        public long FileSize { get; }

        /// <summary>Gets the name of the library.</summary>
        public string LibraryName { get; }

        /// <summary>Gets or sets the type of the resource.</summary>
        public string ResourceType { get; set; }

        /// <summary>Gets or sets the setup.</summary>
        public long SetupTime { get; set; }

        /// <summary>Gets or sets the first parse.</summary>
        public long FirstParseTime { get; set; }

        /// <summary>Gets or sets the first serialize time.</summary>
        public long FirstSerializeTime { get; set; }

        /// <summary>Gets or sets the looped parse.</summary>
        public long LoopedParseTime { get; set; }

        /// <summary>Gets or sets the looped serialize time.</summary>
        public long LoopedSerializeTime { get; set; }

        /// <summary>Gets the number of loops.</summary>
        public int LoopCount { get; }

        /// <summary>Gets or sets the number of failures.</summary>
        public int FailureCount { get; set; }
    }
}
