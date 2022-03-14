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

    /// <summary>(An Action that handles HTTP GET requests) gets all package manifests.</summary>
    /// <returns>all package manifests.</returns>
    [HttpGet("")]
    public IActionResult GetAllPackageManifests()
    {
        return Ok(FhirCacheService.Current.PackagesByDirective.Values.ToArray());
    }

    /// <summary>(An Action that handles HTTP GET requests) gets package manifest.</summary>
    /// <param name="packageName">Name of the package.</param>
    /// <param name="version">    The version.</param>
    /// <returns>The package manifest.</returns>
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

    /// <summary>(An Action that handles HTTP GET requests) gets package artifacts.</summary>
    /// <param name="packageName">Name of the package.</param>
    /// <param name="version">    The version.</param>
    /// <returns>The package artifacts.</returns>
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

    /// <summary>Gets package artifact.</summary>
    /// <param name="packageName"> Name of the package.</param>
    /// <param name="version">     The version.</param>
    /// <param name="artifactType">Type of the artifact.</param>
    /// <param name="id">          The identifier.</param>
    /// <returns>The package artifact.</returns>
    [HttpGet("{packageName}/{version}/artifact/{artifactType}/{id}")]
    public IActionResult GetPackageArtifact(
        [FromRoute] string packageName,
        [FromRoute] string version,
        [FromRoute] string artifactType,
        [FromRoute] string id)
    {
        string directive = packageName + "#" + version;

        if (!FhirCacheService.Current.PackagesByDirective.ContainsKey(directive))
        {
            _logger.LogInformation($"PackageController.GetPackageArtifact <<< requested directive not found: {directive}");
            return NotFound();
        }

        if (FhirCacheService.Current.PackagesByDirective[directive].PackageState != PackageLoadStateEnum.Loaded)
        {
            _logger.LogInformation($"PackageController.GetPackageArtifact <<< package not loaded: {directive}");
            return NotFound();
        }

        string resolvedDirective =
            FhirCacheService.Current.PackagesByDirective[directive].PackageName +
            "#" +
            FhirCacheService.Current.PackagesByDirective[directive].Version;

        if (!FhirManager.Current.IsLoaded(resolvedDirective))
        {
            _logger.LogInformation($"PackageController.GetPackageArtifact <<< resolved package not loaded: {resolvedDirective}");
            return NotFound();
        }

        if (!FhirManager.Current.TryGetLoaded(resolvedDirective, out FhirVersionInfo info))
        {
            _logger.LogInformation($"PackageController.GetPackageArtifact <<< failed to retrieve resolved package: {resolvedDirective}");
            return NotFound();
        }

        switch (artifactType.ToLowerInvariant())
        {
            case "primitivetype":
                if (info.PrimitiveTypes.ContainsKey(id))
                {
                    return Ok(info.PrimitiveTypes[id]);
                }
                break;

            case "complextype":
                if (info.ComplexTypes.ContainsKey(id))
                {
                    return Ok(info.ComplexTypes[id]);
                }
                break;

            case "resource":
                if (info.Resources.ContainsKey(id))
                {
                    return Ok(info.Resources[id]);
                }
                break;

            case "profile":
                if (info.Profiles.ContainsKey(id))
                {
                    return Ok(info.Profiles[id]);
                }
                break;
        }

        return NotFound();
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
