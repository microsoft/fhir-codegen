// <copyright file="PackageHttpMessageHandler.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System.Net;
using Xunit.Abstractions;

namespace Microsoft.Health.Fhir.PackageManager.Tests;

/// <summary>A package HTTP message handler.</summary>
public class PackageHttpMessageHandler : HttpMessageHandler
{
    public ITestOutputHelper? _testOutputHelper = null;

    /// <summary>Send an HTTP request as an asynchronous operation.</summary>
    /// <param name="request">          The HTTP request message to send.</param>
    /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        switch (request.RequestUri?.AbsoluteUri)
        {
            // r4 core
            case "http://packages.fhir.org/hl7.fhir.r4.core":
                {
                    return Task.FromResult(JsonFile("data/package-info-r4-core-primary.json"));
                }
            case "http://packages2.fhir.org/packages/hl7.fhir.r4.core":
                {
                    return Task.FromResult(JsonFile("data/package-info-r4-core-secondary.json"));
                }

            // us core
            case "http://packages.fhir.org/hl7.fhir.us.core":
                {
                    return Task.FromResult(JsonFile("data/package-info-us-core-primary.json"));
                }
            case "http://packages2.fhir.org/packages/hl7.fhir.us.core":
                {
                    return Task.FromResult(JsonFile("data/package-info-us-core-secondary.json"));
                }

            // backport IG
            case "http://packages.fhir.org/hl7.fhir.uv.subscriptions-backport":
                {
                    return Task.FromResult(JsonFile("data/package-info-backport-primary.json"));
                }
            case "http://packages.fhir.org/hl7.fhir.uv.subscriptions-backport.r4":
                {
                    return Task.FromResult(JsonFile("data/package-info-backport-primary-r4.json"));
                }
            case "http://packages.fhir.org/hl7.fhir.uv.subscriptions-backport.r4b":
                {
                    return Task.FromResult(JsonFile("data/package-info-backport-primary-r4b.json"));
                }
            case "http://packages2.fhir.org/packages/hl7.fhir.uv.subscriptions-backport":
                {
                    return Task.FromResult(JsonFile("data/package-info-backport-secondary.json"));
                }
            case "http://packages2.fhir.org/packages/hl7.fhir.uv.subscriptions-backport.r4":
                {
                    return Task.FromResult(JsonFile("data/package-info-backport-secondary-r4.json"));
                }
            case "http://packages2.fhir.org/packages/hl7.fhir.uv.subscriptions-backport.r4b":
                {
                    return Task.FromResult(JsonFile("data/package-info-backport-secondary-r4b.json"));
                }

            // IHE PDQM
            case "https://profiles.ihe.net/ITI/PDQm/package.manifest.json":
                {
                    return Task.FromResult(JsonFile("data/ci-manifest-ihe-pdqm.json"));
                }

            // qas.json
            case "http://build.fhir.org/ig/qas.json":
            case "https://build.fhir.org/ig/qas.json":
                {
                    return Task.FromResult(JsonFile("data/qas-full.json"));
                }

            default:
                {
                    _testOutputHelper?.WriteLine($"{request.Method} {request.RequestUri?.AbsoluteUri} is not implemented.");
                    //throw new NotImplementedException($"The request URI {request.RequestUri?.AbsoluteUri} is not implemented.");
                    return Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound));
                }
        }
    }

    /// <summary>Creates a JSON response message based on content.</summary>
    /// <param name="content">   The content.</param>
    /// <param name="statusCode">(Optional) The status code.</param>
    /// <returns>A HttpResponseMessage.</returns>
    internal static HttpResponseMessage JsonFile(
            string filename,
            HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        return new HttpResponseMessage(statusCode)
        {
            Content = new StringContent(File.ReadAllText(filename), new System.Net.Http.Headers.MediaTypeHeaderValue("application/json")),
        };
    }

    /// <summary>Creates a JSON response message based on content.</summary>
    /// <param name="content">   The content.</param>
    /// <param name="statusCode">(Optional) The status code.</param>
    /// <returns>A HttpResponseMessage.</returns>
    internal static HttpResponseMessage JsonContent(
            string content,
            HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        return new HttpResponseMessage(statusCode)
        {
            Content = new StringContent(content, new System.Net.Http.Headers.MediaTypeHeaderValue("application/json")),
        };
    }
}
