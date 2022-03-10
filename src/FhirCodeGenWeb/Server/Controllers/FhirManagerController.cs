// <copyright file="FhirManagerController.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>


using FhirCodeGenWeb.Server.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Health.Fhir.CodeGenCommon.Models;
using Microsoft.Health.Fhir.SpecManager.Manager;
using Microsoft.Health.Fhir.SpecManager.PackageManager;

namespace FhirCodeGenWeb.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
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

    [HttpGet("package/manifest")]
    public IActionResult GetAllPackageManifests()
    {
        return Ok(FhirCacheService.Current.PackagesByDirective.Values.ToArray());
    }

    [HttpGet("package/manifest/{packageName}/version/{version}")]
    public IActionResult GetPackageManifest([FromRoute] string packageName, [FromRoute] string version)
    {
        string directive = packageName + "#" + version;

        if (!FhirCacheService.Current.PackagesByDirective.ContainsKey(directive))
        {
            return NotFound();
        }

        return Ok(FhirCacheService.Current.PackagesByDirective[directive]);
    }

    [HttpGet("package/artifactIndex/{packageName}/version/{version}")]
    public IActionResult GetPackageArtifacts([FromRoute] string packageName, [FromRoute] string version)
    {
        string directive = packageName + "#" + version;

        if (!FhirCacheService.Current.PackagesByDirective.ContainsKey(directive))
        {
            _logger.LogInformation($"FhirManagerController.GetPackageArtifacts <<< requested directive not found: {directive}");
            return NotFound();
        }

        if (FhirCacheService.Current.PackagesByDirective[directive].PackageState != PackageLoadStateEnum.Loaded)
        {
            _logger.LogInformation($"FhirManagerController.GetPackageArtifacts <<< package not loaded: {directive}");
            return NotFound();
        }

        string resolvedDirective =
            FhirCacheService.Current.PackagesByDirective[directive].PackageName +
            "#" +
            FhirCacheService.Current.PackagesByDirective[directive].Version;

        if (!FhirManager.Current.IsLoaded(resolvedDirective))
        {
            _logger.LogInformation($"FhirManagerController.GetPackageArtifacts <<< resolved package not loaded: {resolvedDirective}");
            return NotFound();
        }

        if (!FhirManager.Current.TryGetLoaded(resolvedDirective, out FhirVersionInfo info))
        {
            _logger.LogInformation($"FhirManagerController.GetPackageArtifacts <<< failed to retrieve resolved package: {resolvedDirective}");
            return NotFound();
        }

        return Ok(info.BuildArtifactRecords());
    }

    /// <summary>(An Action that handles HTTP POST requests) loads a package.</summary>
    /// <param name="packageName">Name of the package.</param>
    /// <param name="version">    The version.</param>
    /// <returns>The package.</returns>
    [HttpPost("package/loadRequest/{packageName}/version/{version}/")]
    public IActionResult LoadPackage(
        [FromRoute] string packageName,
        [FromRoute] string version)
    {
        string cacheDirective = packageName + "#" + version;

        _fhirManagerService.RequestPackageLoad(
            cacheDirective,
            out PackageLoadStateEnum state);

        return Accepted(state);
    }

    /// <summary>(An Action that handles HTTP GET requests) gets package load status.</summary>
    /// <param name="packageName">Name of the package.</param>
    /// <param name="version">    The version.</param>
    /// <returns>The package load status.</returns>
    [HttpGet("package/loadRequest/{packageName}/version/{version}/")]
    public IActionResult GetPackageLoadStatus(
        [FromRoute] string packageName,
        [FromRoute] string version)
    {
        string cacheDirective = packageName + "#" + version;

        if (!FhirCacheService.Current.PackagesByDirective.ContainsKey(cacheDirective))
        {
            return NotFound();
        }

        return Ok(_fhirManagerService.StateForRequest(cacheDirective));
    }
}
