// <copyright file="PackageController.cs" company="Microsoft Corporation">
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
public class PackageController : ControllerBase
{
    /// <summary>(Immutable) The logger.</summary>
    private readonly ILogger<PackageController> _logger;

    /// <summary>(Immutable) The FHIR manager service.</summary>
    private readonly PackageManagerApiService _packageApiService;

    /// <summary>
    /// Initializes a new instance of the <see cref="PackageController"/> class.
    /// </summary>
    /// <param name="logger">            (Immutable) The logger.</param>
    /// <param name="packageApiService">(Immutable) The FHIR manager service.</param>
    public PackageController(
        ILogger<PackageController> logger,
        PackageManagerApiService packageApiService)
    {
        _logger = logger;
        _packageApiService = packageApiService;
    }

    [HttpGet("")]
    public IActionResult GetAllPackageManifests()
    {
        return Ok(FhirCacheService.Current.PackagesByDirective.Values.ToArray());
    }

    [HttpGet("{packageName}/{version}/manifest")]
    public IActionResult GetPackageManifest([FromRoute] string packageName, [FromRoute] string version)
    {
        string directive = packageName + "#" + version;

        if (!FhirCacheService.Current.PackagesByDirective.ContainsKey(directive))
        {
            return NotFound();
        }

        return Ok(FhirCacheService.Current.PackagesByDirective[directive]);
    }

    [HttpGet("{packageName}/{version}/artifactIndex")]
    public IActionResult GetPackageArtifacts([FromRoute] string packageName, [FromRoute] string version)
    {
        string directive = packageName + "#" + version;

        if (!FhirCacheService.Current.PackagesByDirective.ContainsKey(directive))
        {
            _logger.LogInformation($"PackageController.GetPackageArtifacts <<< requested directive not found: {directive}");
            return NotFound();
        }

        if (FhirCacheService.Current.PackagesByDirective[directive].PackageState != PackageLoadStateEnum.Loaded)
        {
            _logger.LogInformation($"PackageController.GetPackageArtifacts <<< package not loaded: {directive}");
            return NotFound();
        }

        string resolvedDirective =
            FhirCacheService.Current.PackagesByDirective[directive].PackageName +
            "#" +
            FhirCacheService.Current.PackagesByDirective[directive].Version;

        if (!FhirManager.Current.IsLoaded(resolvedDirective))
        {
            _logger.LogInformation($"PackageController.GetPackageArtifacts <<< resolved package not loaded: {resolvedDirective}");
            return NotFound();
        }

        if (!FhirManager.Current.TryGetLoaded(resolvedDirective, out FhirVersionInfo info))
        {
            _logger.LogInformation($"PackageController.GetPackageArtifacts <<< failed to retrieve resolved package: {resolvedDirective}");
            return NotFound();
        }

        return Ok(info.BuildArtifactRecords());
    }

    /// <summary>(An Action that handles HTTP POST requests) loads a package.</summary>
    /// <param name="packageName">Name of the package.</param>
    /// <param name="version">    The version.</param>
    /// <returns>The package.</returns>
    [HttpPost("{packageName}/{version}/loadRequest")]
    public IActionResult LoadPackage(
        [FromRoute] string packageName,
        [FromRoute] string version)
    {
        string cacheDirective = packageName + "#" + version;

        _packageApiService.RequestPackageLoad(
            cacheDirective,
            out PackageLoadStateEnum state);

        return Accepted(state);
    }

    /// <summary>(An Action that handles HTTP GET requests) gets package load status.</summary>
    /// <param name="packageName">Name of the package.</param>
    /// <param name="version">    The version.</param>
    /// <returns>The package load status.</returns>
    [HttpGet("{packageName}/{version}/loadRequest")]
    public IActionResult GetPackageLoadStatus(
        [FromRoute] string packageName,
        [FromRoute] string version)
    {
        string cacheDirective = packageName + "#" + version;

        if (!FhirCacheService.Current.PackagesByDirective.ContainsKey(cacheDirective))
        {
            return NotFound();
        }

        return Ok(_packageApiService.StateForRequest(cacheDirective));
    }
}
