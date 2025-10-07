using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace Fhir.CodeGen.Packages.Tests;


/// <summary>A package HTTP message handler.</summary>
public class PackageHttpMessageHandler : HttpMessageHandler
{
    public ITestOutputHelper? _testOutputHelper = null;

    protected override HttpResponseMessage Send(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        return SendAsync(request, cancellationToken).GetAwaiter().GetResult();
    }

    /// <summary>Send an HTTP request as an asynchronous operation.</summary>
    /// <param name="request">          The HTTP request message to send.</param>
    /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        if (request.RequestUri == null)
        {
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent("Request URI is null"),
            });
        }

        string protocol = request.RequestUri.Scheme;
        string url = request.RequestUri.AbsoluteUri[protocol.Length..].ToLowerInvariant();

        switch (url)
        {
            // r4 core
            case "://packages.fhir.org/hl7.fhir.r4.core":
                {
                    return Task.FromResult(JsonFile("TestData/package-info-r4-core-primary.json"));
                }
            case "://packages2.fhir.org/packages/hl7.fhir.r4.core":
                {
                    return Task.FromResult(JsonFile("TestData/package-info-r4-core-secondary.json"));
                }
            case "://packages.fhir.org/catalog?op=find&name=hl7.fhir.r4&pkgcanonical=&canonical=&fhirversion=":
                {
                    return Task.FromResult(JsonFile("TestData/catalog-r4-primary.json"));
                }
            case "://packages2.fhir.org/packages/catalog?op=find&name=hl7.fhir.r4&pkgcanonical=&canonical=&fhirversion=":
                {
                    return Task.FromResult(JsonFile("TestData/catalog-r4-secondary.json"));
                }
            case "://packages.fhir.org/catalog?op=find&name=hl7.fhir.r4.core&pkgcanonical=&canonical=&fhirversion=":
                {
                    return Task.FromResult(JsonFile("TestData/catalog-r4-core-primary.json"));
                }
            case "://packages2.fhir.org/packages/catalog?op=find&name=hl7.fhir.r4.core&pkgcanonical=&canonical=&fhirversion=":
                {
                    return Task.FromResult(JsonFile("TestData/catalog-r4-core-secondary.json"));
                }

            // us core
            case "://packages.fhir.org/hl7.fhir.us.core":
                {
                    return Task.FromResult(JsonFile("TestData/package-info-us-core-primary.json"));
                }
            case "://packages2.fhir.org/packages/hl7.fhir.us.core":
                {
                    return Task.FromResult(JsonFile("TestData/package-info-us-core-secondary.json"));
                }
            case "://packages.fhir.org/catalog?op=find&name=hl7.fhir.us.core&pkgcanonical=&canonical=&fhirversion=":
                {
                    return Task.FromResult(JsonFile("TestData/catalog-us-core-primary.json"));
                }
            case "://packages2.fhir.org/packages/catalog?op=find&name=hl7.fhir.us.core&pkgcanonical=&canonical=&fhirversion=":
                {
                    return Task.FromResult(JsonFile("TestData/catalog-us-core-secondary.json"));
                }

            // backport IG
            case "://packages.fhir.org/hl7.fhir.uv.subscriptions-backport":
                {
                    return Task.FromResult(JsonFile("TestData/package-info-backport-primary.json"));
                }
            case "://packages.fhir.org/hl7.fhir.uv.subscriptions-backport.r4":
                {
                    return Task.FromResult(JsonFile("TestData/package-info-backport-primary-r4.json"));
                }
            case "://packages.fhir.org/hl7.fhir.uv.subscriptions-backport.r4b":
                {
                    return Task.FromResult(JsonFile("TestData/package-info-backport-primary-r4b.json"));
                }
            case "://packages2.fhir.org/packages/hl7.fhir.uv.subscriptions-backport":
                {
                    return Task.FromResult(JsonFile("TestData/package-info-backport-secondary.json"));
                }
            case "://packages2.fhir.org/packages/hl7.fhir.uv.subscriptions-backport.r4":
                {
                    return Task.FromResult(JsonFile("TestData/package-info-backport-secondary-r4.json"));
                }
            case "://packages2.fhir.org/packages/hl7.fhir.uv.subscriptions-backport.r4b":
                {
                    return Task.FromResult(JsonFile("TestData/package-info-backport-secondary-r4b.json"));
                }
            case "://build.fhir.org/ig/HL7/subscriptions-backport/package.manifest.json":
            case "://build.fhir.org/ig/HL7/subscriptions-backport/branches/a-branch/package.manifest.json":
                {
                    return Task.FromResult(JsonFile("TestData/manifest-backport.json"));
                }
            case "://build.fhir.org/ig/HL7/subscriptions-backport/package.r4.manifest.json":
            case "://build.fhir.org/ig/HL7/subscriptions-backport/branches/a-branch/package.r4.manifest.json":
                {
                    return Task.FromResult(JsonFile("TestData/manifest-backport-r4.json"));
                }
            case "://build.fhir.org/ig/HL7/subscriptions-backport/package.r4b.manifest.json":
            case "://build.fhir.org/ig/HL7/subscriptions-backport/branches/a-branch/package.r4b.manifest.json":
                {
                    return Task.FromResult(JsonFile("TestData/manifest-backport-r4b.json"));
                }
            case "://packages.fhir.org/catalog?op=find&name=hl7.fhir.uv.subscriptions-backport&pkgcanonical=&canonical=&fhirversion=":
                {
                    return Task.FromResult(JsonFile("TestData/catalog-backport-primary.json"));
                }
            case "://packages2.fhir.org/packages/catalog?op=find&name=hl7.fhir.uv.subscriptions-backport&pkgcanonical=&canonical=&fhirversion=":
                {
                    return Task.FromResult(JsonFile("TestData/catalog-backport-secondary.json"));
                }
            case "://packages.fhir.org/catalog?op=find&name=hl7.fhir.uv.subscriptions-backport.r4&pkgcanonical=&canonical=&fhirversion=":
                {
                    return Task.FromResult(JsonFile("TestData/catalog-backport-primary-r4.json"));
                }
            case "://packages2.fhir.org/packages/catalog?op=find&name=hl7.fhir.uv.subscriptions-backport.r4&pkgcanonical=&canonical=&fhirversion=":
                {
                    return Task.FromResult(JsonFile("TestData/catalog-backport-secondary-r4.json"));
                }
            case "://packages.fhir.org/catalog?op=find&name=hl7.fhir.uv.subscriptions-backport.r4b&pkgcanonical=&canonical=&fhirversion=":
                {
                    return Task.FromResult(JsonFile("TestData/catalog-backport-primary-r4b.json"));
                }
            case "://packages2.fhir.org/packages/catalog?op=find&name=hl7.fhir.uv.subscriptions-backport.r4b&pkgcanonical=&canonical=&fhirversion=":
                {
                    return Task.FromResult(JsonFile("TestData/catalog-backport-secondary-r4b.json"));
                }

            // IHE PDQM
            case "://profiles.ihe.net/iti/pdqm/package.manifest.json":
                {
                    return Task.FromResult(JsonFile("TestData/manifest-ihe-pdqm.json"));
                }

            // qas.json
            case "://build.fhir.org/ig/qas.json":
                {
                    return Task.FromResult(JsonFile("TestData/qas-full.json"));
                }

            // ci core versions
            case "://build.fhir.org/version.info":
            case "://build.fhir.org/branches/branch/version.info":
                {
                    return Task.FromResult(IniFile("TestData/version.info"));
                }

            // ci backport manifests

            //// URL-based directive resolution
            //case "://hl7.org/fhir/uv/subscriptions-backport/version.info":
            //    {

            //    }
            //    break;

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

    /// <summary>Creates an INI response message based on content.</summary>
    /// <param name="filename">  Filename of the file.</param>
    /// <param name="statusCode">(Optional) The status code.</param>
    /// <returns>A HttpResponseMessage.</returns>
    internal static HttpResponseMessage IniFile(
        string filename,
        HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        return new HttpResponseMessage(statusCode)
        {
            Content = new StringContent(File.ReadAllText(filename), new System.Net.Http.Headers.MediaTypeHeaderValue("text/plain")),
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
