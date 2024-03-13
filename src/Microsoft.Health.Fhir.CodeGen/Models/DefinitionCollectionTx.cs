// <copyright file="CollectionTerminologyService.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>


using System.Diagnostics.CodeAnalysis;
using Hl7.Fhir.Model;
using Hl7.Fhir.Specification.Source;
using Hl7.Fhir.Specification.Terminology;
using Microsoft.Health.Fhir.CodeGen.FhirExtensions;

namespace Microsoft.Health.Fhir.CodeGen.Models;

public partial class DefinitionCollection : IAsyncResourceResolver
{
    /// <summary>Gets the local terminology service.</summary>
    public ITerminologyService LocalTx => _localTx;

    /// <summary>Expand vs.</summary>
    /// <param name="uri">The canonical url of a (conformance) resource.</param>
    /// <returns>An asynchronous result that yields a ValueSet?</returns>
    public async Task<ValueSet?> ExpandVs(string uri)
    {
        Parameters p = new();
        p.Parameter.Add(new() { Name = "url", Value = new FhirUri(uri) });

        try
        {
            Resource r = await _localTx.Expand(p);

            if ((r is ValueSet vs) &&
                (!vs.IsLimitedExpansion()))
            {
                return vs;
            }

            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error expanding {uri}: {ex.Message}");
            return null;
        }
    }

    /// <summary>Attempts to expand vs a ValueSet from the given string.</summary>
    /// <param name="uri">The canonical url of a (conformance) resource.</param>
    /// <param name="vs"> [out] The vs.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    public bool TryExpandVs(string uri, [NotNullWhen(true)] out ValueSet? vs)
    {
        vs = ExpandVs(uri).Result;
        return vs != null;
    }

    /// <summary>Find a (conformance) resource based on its canonical uri.</summary>
    /// <param name="uri">The canonical url of a (conformance) resource.</param>
    /// <returns>An asynchronous result that yields the resolve by canonical URI.</returns>
    public Task<Resource> ResolveByCanonicalUriAsync(string uri)
    {
        string key;
        string version = string.Empty;

        if (uri.Contains('|'))
        {
            key = uri.Substring(0, uri.LastIndexOf('|'));
            version = uri.Substring(uri.LastIndexOf('|') + 1);
        }
        else
        {
            key = uri;
        }

        if (_canonicalResources.TryGetValue(key, out Dictionary<string, IConformanceResource>? versions) &&
            (versions != null) &&
            versions.Any())
        {
            if (string.IsNullOrEmpty(version))
            {
                version = versions.Keys.OrderDescending().First();

                return System.Threading.Tasks.Task.FromResult((Resource)versions[version]);
            }

            if (versions.TryGetValue(version, out IConformanceResource? resource))
            {
                return System.Threading.Tasks.Task.FromResult((Resource)resource);
            }
        }

        if (_allResources.TryGetValue(uri, out Resource? r))
        {
            return System.Threading.Tasks.Task.FromResult(r);
        }

        if (_allResources.TryGetValue(key, out r))
        {
            return System.Threading.Tasks.Task.FromResult(r);
        }

        return System.Threading.Tasks.Task.FromResult<Resource>(null!);
    }

    /// <summary>Find a resource based on its relative or absolute uri.</summary>
    /// <param name="uri">A resource uri.</param>
    /// <returns>An asynchronous result that yields the resolve by URI.</returns>
    public Task<Resource> ResolveByUriAsync(string uri)
    {
        if (_allResources.TryGetValue(uri, out Resource? r))
        {
            return System.Threading.Tasks.Task.FromResult(r);
        }

        if (_allResources.TryGetValue(uri.Split('|')[0], out r))
        {
            return System.Threading.Tasks.Task.FromResult(r);
        }

        return System.Threading.Tasks.Task.FromResult<Resource>(null!);
    }
}
