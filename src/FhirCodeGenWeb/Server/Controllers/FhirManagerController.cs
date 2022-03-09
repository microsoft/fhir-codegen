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

    [HttpGet("package")]
    public IActionResult GetPackageRecord([FromQuery] string? packageName, [FromQuery] string? version)
    {
        if (string.IsNullOrEmpty(packageName))
        {
            return Ok(FhirCacheService.Current.PackagesByDirective.Values.ToArray());
        }

        string directive = packageName + "#" + version;

        if (!FhirCacheService.Current.PackagesByDirective.ContainsKey(directive))
        {
            return NotFound();
        }

        return Ok(FhirCacheService.Current.PackagesByDirective[directive]);
    }

    [HttpGet("package/artifactIndex")]
    public IActionResult GetPackageArtifacts([FromQuery] string packageName, [FromQuery] string version)
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

    [HttpPost("package/load")]
    public PackageLoadStateEnum LoadPackage([FromBody] string directive)
    {
        _fhirManagerService.RequestPackageLoad(
            directive,
            out PackageLoadStateEnum state);

        return state;
    }

    [HttpGet("package/load")]
    public PackageLoadStateEnum GetLoadStatus([FromQuery] string directive)
    {
        return _fhirManagerService.StateForRequest(directive);
    }
}
