// <copyright file="FhirManagerController.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>


using FhirCodeGenWeb.Server.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Health.Fhir.CodeGenCommon.Models;
using Microsoft.Health.Fhir.SpecManager.Manager;
using Microsoft.Health.Fhir.SpecManager.PackageManager;

namespace FhirCodeGenWeb.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class FhirManagerController : ControllerBase
{
    /// <summary>(Immutable) The logger.</summary>
    private readonly ILogger<FhirManagerController> _logger;

    /// <summary>(Immutable) The FHIR manager service.</summary>
    private readonly PackageManagerApiService _fhirManagerService;

    /// <summary>
    /// Initializes a new instance of the <see cref="FhirManagerController"/> class.
    /// </summary>
    /// <param name="logger">            (Immutable) The logger.</param>
    /// <param name="fhirManagerService">(Immutable) The FHIR manager service.</param>
    public FhirManagerController(
        ILogger<FhirManagerController> logger,
        PackageManagerApiService fhirManagerService)
    {
        _logger = logger;
        _fhirManagerService = fhirManagerService;
    }

    [HttpGet("LoadedPackageDetails")]
    public IEnumerable<NpmPackageDetails> LoadedPackageDetails()
    {
        IEnumerable<NpmPackageDetails> packages = FhirCacheService.Current.GetCachedPackages();

        foreach (NpmPackageDetails package in packages)
        {
            package.IsLoaded = FhirManager.Current.IsLoaded($"{package.Name}#{package.Version}");
        }

        return packages;
    }

    [HttpGet("Package")]
    public IEnumerable<PackageCacheRecord> GetPackageRecords()
    {
        return FhirCacheService.Current.PackagesByDirective.Values.ToArray();
    }

    [HttpPost("PackageLoadRequest")]
    public PackageLoadStateEnum LoadPackage([FromBody] string directive)
    {
        _fhirManagerService.RequestPackageLoad(
            directive,
            out PackageLoadStateEnum state);

        return state;
    }

    [HttpGet("PackageLoadStatus")]
    public PackageLoadStateEnum GetLoadStatus([FromQuery] string directive)
    {
        return _fhirManagerService.StateForRequest(directive);
    }
}
