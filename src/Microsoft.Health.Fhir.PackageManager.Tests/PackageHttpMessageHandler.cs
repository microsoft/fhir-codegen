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
            case "https://build.fhir.org/ig/qas.json":
                {
                    return Task.FromResult(JsonFile("data/qas-full.json"));
                }
            default:
                _testOutputHelper?.WriteLine($"The request URI {request.RequestUri?.AbsoluteUri} is not implemented.");
                throw new NotImplementedException($"The request URI {request.RequestUri?.AbsoluteUri} is not implemented.");
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
