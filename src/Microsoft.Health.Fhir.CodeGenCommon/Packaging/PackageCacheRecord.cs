// <copyright file="PackageCacheRecord.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

namespace Microsoft.Health.Fhir.CodeGenCommon.Models;

///// <summary>Values that represent package load request state enums.</summary>
//public enum PackageLoadStateEnum
//{
//    /// <summary>The package is in an unknown state.</summary>
//    Unknown,

//    /// <summary>The package has not been loaded.</summary>
//    NotLoaded,

//    /// <summary>The package is queued for loading.</summary>
//    Queued,

//    /// <summary>The package is currently being loaded.</summary>
//    InProgress,

//    /// <summary>The package is currently loaded into memory.</summary>
//    Loaded,

//    /// <summary>The package has failed to load and cannot be used.</summary>
//    Failed,

//    /// <summary>The package has been parsed but not loaded into memory.</summary>
//    Parsed,
//}

///// <summary>Information about a package in the cache.</summary>
//public readonly record struct PackageCacheRecord(
//    string CacheDirective,
//    PackageLoadStateEnum PackageState,
//    string PackageName,
//    string Version,
//    string DownloadDateTime,
//    long PackageSize,
//    NpmPackageDetails Details);
