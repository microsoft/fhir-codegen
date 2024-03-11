// <copyright file="CollectionTerminologyService.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>


using System.Diagnostics.CodeAnalysis;
using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using Hl7.Fhir.Specification.Source;
using Hl7.Fhir.Specification.Terminology;

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

            if (r is ValueSet vs)
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

//public partial class DefinitionCollection: ITerminologyService
//{
//    private bool _txServiceInitialized = false;
//    private ITerminologyService? _txService = null;

//    /// <summary>Initializes the transmit service.</summary>
//    private void InitializeTxService()
//    {
//        if (_txServiceInitialized)
//        {
//            return;
//        }

//        _txServiceInitialized = true;

//        if (TxServers.Length == 1)
//        {
//            FhirClient txClient = new(TxServers[0]);
//            _txService = new ExternalTerminologyService(txClient);

//            return;
//        }

//        List<ITerminologyService> services = new();
//        foreach (string txServer in TxServers)
//        {
//            FhirClient txClient = new(txServer);
//            services.Add(new ExternalTerminologyService(txClient));
//        }

//        _txService = new MultiTerminologyService(services);
//    }

//    public Task<Resource> Closure(Parameters parameters, bool useGet = false) => throw new NotImplementedException();
//    public Task<Parameters> CodeSystemValidateCode(Parameters parameters, string id = null, bool useGet = false) => throw new NotImplementedException();

//    /// <summary>External expand.</summary>
//    /// <param name="parameters">Input parameters for the operation.</param>
//    /// <param name="id">        (Optional) Id of a specific ValueSet to expand.</param>
//    /// <param name="useGet">    (Optional) Use the GET instead of POST Http method.</param>
//    /// <returns>An asynchronous result that yields a Resource?</returns>
//    private Task<Resource?> ExternalExpand(Parameters parameters, string id = "", bool useGet = false)
//    {
//        InitializeTxService();

//        if (_txService == null)
//        {
//            return System.Threading.Tasks.Task.FromResult<Resource?>(null);
//        }

//        return _txService.Expand(parameters, id, useGet);
//    }

//    /// <summary>
//    /// The definition of a value set is used to create a simple collection of codes suitable for use
//    /// for data entry or validation.
//    /// </summary>
//    /// <remarks>
//    /// This function corresponds to the <seealso href="http://hl7.org/fhir/valueset-operation-expand.html">
//    /// $expand</seealso> operation.
//    /// </remarks>
//    /// <param name="parameters">Input parameters for the operation.</param>
//    /// <param name="id">        (Optional) Id of a specific ValueSet to expand.</param>
//    /// <param name="useGet">    (Optional) Use the GET instead of POST Http method.</param>
//    /// <returns>Output parameters containing the expanded ValueSet.</returns>
//    public Task<Resource> Expand(Parameters parameters, string id = "", bool useGet = false)
//    {
//        Resource? retVal;

//        string vsUrl = parameters.GetSingleValue<FhirUri>("url")?.Value?.ToString() ?? string.Empty;

//        ValueSet? vs = ResolveVs(vsUrl, id);
//        if (vs != null)
//        {
//            return System.Threading.Tasks.Task.FromResult((Resource)vs);
//        }

//        // attempt to resolve externally
//        retVal = ExternalExpand(parameters, id, useGet).Result;
//        if (retVal != null)
//        {
//            return System.Threading.Tasks.Task.FromResult(retVal);
//        }

//        retVal = new OperationOutcome()
//        {
//            Issue = new List<OperationOutcome.IssueComponent>
//            {
//                new()
//                {
//                    Severity = OperationOutcome.IssueSeverity.Error,
//                    Code = OperationOutcome.IssueType.NotFound,
//                    Diagnostics = $"Cannot resolve the ValueSet {id}",
//                    Details = ToolingTxIssueTypes.NotFound,
//                }
//            }
//        };

//        return System.Threading.Tasks.Task.FromResult(retVal);
//    }

//    public Task<Parameters> Lookup(Parameters parameters, bool useGet = false) => throw new NotImplementedException();
//    public Task<Parameters> Subsumes(Parameters parameters, string id = "", bool useGet = false) => throw new NotImplementedException();
//    public Task<Parameters> Translate(Parameters parameters, string id = "", bool useGet = false) => throw new NotImplementedException();

//    /// <summary>Validate that a coded value is in the set of codes allowed by a value set.</summary>
//    /// <remarks>
//    /// This function corresponds to the <seealso href="http://hl7.org/fhir/valueset-operation-validate-code.html">
//    /// $validate-codes</seealso> operation.
//    /// </remarks>
//    /// <param name="parameters">Input parameters for the operation.</param>
//    /// <param name="id">        (Optional) Id of a specific ValueSet which is used to validate
//    ///  against.</param>
//    /// <param name="useGet">    (Optional) Use the GET instead of POST Http method.</param>
//    /// <returns>Output parameters containing the result of the operation.</returns>
//    public Task<Parameters> ValueSetValidateCode(Parameters parameters, string id = "", bool useGet = false)
//    {
//        Parameters retVal = new();

//        string vsUrl = parameters.GetSingleValue<FhirUri>("url")?.Value?.ToString() ?? string.Empty;

//        ValueSet? vs = ResolveVs(vsUrl, id);
//        if (vs == null)
//        {
//            retVal.Parameter.Add(new() { Name = "result", Value = new FhirBoolean(false) });
//            retVal.Parameter.Add(new() { Name = "message", Value = new FhirString($"Cannot resolve the ValueSet {id}") });
//            retVal.Parameter.Add(new() {
//                Name = "issues",
//                Resource = new OperationOutcome
//                {
//                    Issue = new List<OperationOutcome.IssueComponent>
//                    {
//                        new()
//                        {
//                            Severity = OperationOutcome.IssueSeverity.Error,
//                            Code = OperationOutcome.IssueType.NotFound,
//                            Diagnostics = $"Cannot resolve the ValueSet {id}",
//                            Details = ToolingTxIssueTypes.NotFound,
//                        }
//                    }
//                }
//            });
//            return System.Threading.Tasks.Task.FromResult(retVal);
//        }

//        string system = parameters.GetSingleValue<FhirUri>("system")?.Value?.ToString() ?? string.Empty;
//        string code = parameters.GetSingleValue<Code>("code")?.Value?.ToString() ?? string.Empty;

//        Coding? coding = parameters.GetSingleValue<Coding>("coding");
//        CodeableConcept? concept = parameters.GetSingleValue<CodeableConcept>("codeableConcept");

//        if (string.IsNullOrEmpty(code))
//        {
//            if (coding != null)
//            {
//                system = coding.System;
//                code = coding.Code;
//            }
//            else if (concept != null)
//            {
//                system = concept.Coding.FirstOrDefault()?.System ?? string.Empty;
//                code = concept.Coding.FirstOrDefault()?.Code ?? string.Empty;
//            }
//        }

//        if (string.IsNullOrEmpty(system) && string.IsNullOrEmpty(code))
//        {
//            retVal.Parameter.Add(new() { Name = "result", Value = new FhirBoolean(false) });
//            retVal.Parameter.Add(new() { Name = "message", Value = new FhirString("Could not determine system and code for testing") });
//            retVal.Parameter.Add(new()
//            {
//                Name = "issues",
//                Resource = new OperationOutcome
//                {
//                    Issue = new List<OperationOutcome.IssueComponent>
//                    {
//                        new()
//                        {
//                            Severity = OperationOutcome.IssueSeverity.Error,
//                            Code = OperationOutcome.IssueType.Invalid,
//                            Diagnostics = "Could not determine system and code for testing",
//                            Details = ToolingTxIssueTypes.InvalidData,
//                        }
//                    }
//                }
//            });
//            return System.Threading.Tasks.Task.FromResult(retVal);
//        }

//        bool? result = VsContains(vs, system, code);

//        if (result == null)
//        {
//            retVal.Parameter.Add(new() { Name = "result", Value = new FhirBoolean(false) });
//            retVal.Parameter.Add(new() { Name = "message", Value = new FhirString($"Cannot resolve the ValueSet {vsUrl}") });
//            retVal.Parameter.Add(new()
//            {
//                Name = "issues",
//                Resource = new OperationOutcome
//                {
//                    Issue = new List<OperationOutcome.IssueComponent>
//                    {
//                        new()
//                        {
//                            Severity = OperationOutcome.IssueSeverity.Error,
//                            Code = OperationOutcome.IssueType.NotFound,
//                            Diagnostics = $"Cannot resolve the ValueSet {vsUrl}",
//                            Details = ToolingTxIssueTypes.NotFound,
//                        }
//                    }
//                }
//            });
//            return System.Threading.Tasks.Task.FromResult(retVal);
//        }

//        if (result == false)
//        {
//            retVal.Parameter.Add(new() { Name = "result", Value = new FhirBoolean(false) });
//            retVal.Parameter.Add(new()
//            {
//                Name = "issues",
//                Resource = new OperationOutcome
//                {
//                    Issue = new List<OperationOutcome.IssueComponent>
//                    {
//                        new()
//                        {
//                            Severity = OperationOutcome.IssueSeverity.Information,
//                            Code = OperationOutcome.IssueType.CodeInvalid,
//                            Diagnostics = $"'{system}|{code}' is not valid in {vsUrl}",
//                            Details = ToolingTxIssueTypes.InvalidCode,
//                        }
//                    }
//                }
//            });
//            return System.Threading.Tasks.Task.FromResult(retVal);
//        }

//        retVal.Parameter.Add(new() { Name = "result", Value = new FhirBoolean(true) });
//        retVal.Parameter.Add(new() { Name = "code", Value = new Code(code) });
//        if (!string.IsNullOrEmpty(system))
//        {
//            retVal.Parameter.Add(new() { Name = "system", Value = new FhirUri(system) });
//        }
//        return System.Threading.Tasks.Task.FromResult(retVal);
//    }


//    /// <summary>Check if a value-set contains a value</summary>
//    /// <param name="vsUrl"> URL of the vs.</param>
//    /// <param name="system">The system.</param>
//    /// <param name="code">  The code.</param>
//    /// <returns>True if it does, false if it does not, null if the value set is not contained locally.</returns>
//    public bool? VsContains(ValueSet vs, string system, string code)
//    {
//        if ((vs.Expansion == null) ||
//            !vs.Expansion.Contains.Any())
//        {
//            return null;
//        }

//        if (string.IsNullOrEmpty(system))
//        {
//            return vs.Expansion.Contains.Any(c => c.Code == code);
//        }

//        return vs.Expansion.Contains.Any(c => c.System == system && c.Code == code);
//    }

//    /// <summary>Resolve vs.</summary>
//    /// <param name="url">URL of the resource.</param>
//    /// <param name="id"> Id of a specific ValueSet which is used to validate against.</param>
//    /// <returns>A ValueSet?</returns>
//    public ValueSet? ResolveVs(string url, string id = "")
//    {
//        string localUrl = url;

//        if (!string.IsNullOrEmpty(id))
//        {
//            if (_valueSetUrlsById.TryGetValue(id, out string? vsUrl))
//            {
//                localUrl = vsUrl;
//            }
//        }

//        if (string.IsNullOrEmpty(localUrl))
//        {
//            return null;
//        }

//        if (_valueSetsByVersionedUrl.TryGetValue(localUrl, out ValueSet? vs))
//        {
//            return vs;
//        }

//        string[] urlComponents = localUrl.Split('|');

//        if ((!_valueSetVersions.TryGetValue(urlComponents[0], out string[]? versions)) ||
//            (versions == null))
//        {
//            return null;
//        }

//        // check for unversioned request
//        if (urlComponents.Length == 1)
//        {
//            string unversioned = $"{urlComponents[0]}|{versions.OrderDescending().First()}";

//            return _valueSetsByVersionedUrl[unversioned];
//        }

//        // check for a version that can match
//        IEnumerable<string> matches = versions.Where(v => v.StartsWith(urlComponents[1])).OrderDescending();

//        if (!matches.Any())
//        {
//            return null;
//        }

//        string resolved = $"{urlComponents[0]}|{matches.OrderDescending().First()}";

//        return _valueSetsByVersionedUrl[resolved];
//    }

//    /// <summary>Determine if we can resolve vs.</summary>
//    /// <param name="vsUrl">URL of the vs.</param>
//    /// <returns>True if we can resolve vs, false if not.</returns>
//    public bool CanResolveVs(string vsUrl)
//    {
//        if (_valueSetsByVersionedUrl.ContainsKey(vsUrl))
//        {
//            return true;
//        }

//        string[] urlComponents = vsUrl.Split('|');

//        if ((!_valueSetVersions.TryGetValue(urlComponents[0], out string[]? versions)) ||
//            (versions == null))
//        {
//            return false;
//        }

//        // check for unversioned request
//        if (urlComponents.Length == 1)
//        {
//            return true;
//        }

//        // check for a version that can match
//        IEnumerable<string> matches = versions.Where(v => v.StartsWith(urlComponents[1])).OrderDescending();

//        if (!matches.Any())
//        {
//            return false;
//        }

//        return true;
//    }
//}
