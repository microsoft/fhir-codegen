using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using Hl7.Fhir.Specification.Source;
using System.Threading;
using Hl7.Fhir.Specification.Terminology;
using Microsoft.Health.Fhir.CodeGen.Models;
using Tasks = System.Threading.Tasks;
using System.Runtime;
using System.ComponentModel.DataAnnotations;

namespace Microsoft.Health.Fhir.CodeGen.TerminologyService;

/// <summary>
/// A terminology service provider, based on the Firely LocalTerminologyService implementation.
/// </summary>
public class CodeGenTerminologyService : ITerminologyService
{
    private static readonly SemaphoreSlim SEMAPHORE = new(1, 1);

    private readonly DefinitionCollection _dc;
    private readonly CodeGenValueSetExpander _expander;

    public CodeGenTerminologyService(DefinitionCollection dc)
    {
        _dc = dc;
        _expander = new(dc);
    }

    public CodeGenTerminologyService(CodeGenValueSetExpanderSettings settings)
    {
        _dc = settings.Definitions;
        _expander = new(settings);
    }

    /// <summary>
    /// Creates a MultiTerminologyService, which combines a LocalTerminologyService to retrieve the core FHIR resources with custom services to validate some implicit core ValueSets.
    /// </summary>
    /// <param name="coreResourceResolver">Resource resolves to resolve FHIR core artifacts</param>
    /// <param name="expanderSettings">ValueSet expansion settings</param>
    /// <returns>A MultiTerminologyService, which combines a LocalTerminologyService to retrieve the core FHIR resources with custom services to validate some implicit core ValueSets</returns>
    public static MultiTerminologyService CreateDefaultForCore(IAsyncResourceResolver coreResourceResolver,
        ValueSetExpanderSettings? expanderSettings = null)
    {
        return TerminologyServiceFactory.CreateDefaultForCore(coreResourceResolver, expanderSettings);
    }

    internal ValueSet? FindValueSet(Canonical canonical)
    {
        if (!_dc.TryResolveByCanonicalUri(canonical.ToString() ?? canonical.Uri ?? string.Empty, out Resource? resolvedResource))
        {
            return null;
        }

        if (resolvedResource is ValueSet vs)
        {
            return vs;
        }

        if (resolvedResource is CodeSystem cs)
        {
            return new ValueSet()
            {
                Url = canonical,
                Status = cs.Status,
                ApprovalDate = cs.ApprovalDate,
                Author = cs.Author,
                CopyrightLabel = cs.CopyrightLabel,
                Editor = cs.Editor,
                EffectivePeriod = cs.EffectivePeriod,
                Endorser = cs.Endorser,
                LastReviewDate = cs.LastReviewDate,
                RelatedArtifact = cs.RelatedArtifact,
                Reviewer = cs.Reviewer,
                Topic = cs.Topic,
                VersionAlgorithm = cs.VersionAlgorithm,
                Contact = cs.Contact,
                Copyright = cs.Copyright,
                Date = cs.Date,
                Description = cs.Description,
                Experimental = cs.Experimental,
                Id = cs.Id,
                Jurisdiction = cs.Jurisdiction,
                Language = cs.Language,
                Name = cs.Name,
                Publisher = cs.Publisher,
                Purpose = cs.Purpose,
                Title = cs.Title,
                UseContext = cs.UseContext,
                Version = cs.Version,
                Compose = new ValueSet.ComposeComponent
                {
                    Include =
                    [
                        new ValueSet.ConceptSetComponent { System = cs.Url }
                    ]
                }
            };
        }

        return null;
    }

    private async Task<ValueSet> getExpandedValueSet(ValueSet vs, string operation)
    {
        try
        {
            await SEMAPHORE.WaitAsync().ConfigureAwait(false);

            try
            {
                // We might have a cached or pre-expanded version brought to us by the _source
                if (!vs.HasExpansion)
                {
                    // This will expand te vs - since we do not deepcopy() it, it will change the instance
                    // as it was passed to us from the source
                    await _expander.ExpandAsync(vs).ConfigureAwait(false);
                }
            }
            finally
            {
                SEMAPHORE.Release();
            }
        }
        catch (TerminologyServiceException e)
        {
            // Unprocessable entity
            throw new FhirOperationException(
                $"Operation {operation} failed: creating the required expansion failed with message \"{e.Message}\".",
                (HttpStatusCode)422);
        }

        return vs;
    }

    private async Task<ValueSet> getExpandedValueSet(FhirUri url, FhirString? version, string operation)
    {
        // Handling the url + version is a bit tricky, since some callers (i.e. Firely tools) will call this
        // operation with a version in the url, but others will supply this as url+version (which is the correct way).
        (string? uri, string? canonicalVersion, string? fragment) = new Canonical(url.Value);
        string? versionToUse = version?.Value ?? canonicalVersion;
        Canonical resolvableCanonical = new Canonical(uri, versionToUse, fragment);

        ValueSet? vs = FindValueSet(resolvableCanonical);
        if (vs is null)
        {
            throw new FhirOperationException($"Operation {operation} failed: valueset '{resolvableCanonical}' is unknown.", HttpStatusCode.NotFound);
        }

        return await getExpandedValueSet(vs, operation).ConfigureAwait(false);
    }

    ///<inheritdoc />
    public async Task<Parameters> ValueSetValidateCode(Parameters parameters, string? id = null, bool useGet = false)
    {
        checkForValidityOfValidateCodeParams(parameters);

        ValidateCodeParameters validateCodeParams = new ValidateCodeParameters(parameters);
        ValueSet? valueSet = validateCodeParams.ValueSet as ValueSet;
        if (valueSet is null && validateCodeParams.Url is null)
            throw new FhirOperationException("Have to supply either a canonical url or a valueset.",
                (HttpStatusCode)422); // Unprocessable entity

        try
        {
            valueSet = valueSet == null
                ? await getExpandedValueSet(validateCodeParams.Url!, validateCodeParams.ValueSetVersion, "validate code").ConfigureAwait(false)
                : await getExpandedValueSet(valueSet, "validate code").ConfigureAwait(false);

            if (validateCodeParams.CodeableConcept != null)
            {
                return validateCodeVs(
                    valueSet,
                    validateCodeParams.CodeableConcept,
                    validateCodeParams.Abstract?.Value);
            }

            if (validateCodeParams.Coding != null)
            {
                return validateCodeVs(
                    valueSet,
                    validateCodeParams.Coding,
                    validateCodeParams.Abstract?.Value);
            }
            
            return validateCodeVs(
                valueSet,
                validateCodeParams.Code?.Value,
                validateCodeParams.System?.Value,
                validateCodeParams.Display?.Value,
                validateCodeParams.Abstract?.Value);
        }
        catch (Exception e) when (e is not FhirOperationException)
        {
            //500 internal server error
            throw new FhirOperationException(e.Message, (HttpStatusCode)500);
        }
    }

    private static void checkForValidityOfValidateCodeParams(Parameters parameters)
    {
        noDuplicates(parameters);

        //This error was changed from system to url. See: https://chat.fhir.org/#narrow/channel/179202-terminology/topic/Required.20.24validate-code.20parameters/near/482250225
        //If a code is provided, an inline valueset, url or a context must be provided (http://hl7.org/fhir/valueset-operation-validate-code.html)
        if (parameters.Parameter.Any(p => p.Name == "code") && !hasValueSet(parameters))
        {
            //422 Unproccesable Entity
            throw new FhirOperationException($"If a code is provided, a url or a context must be provided", (HttpStatusCode)422);
        }

        return;

        static bool hasValueSet(Parameters p) => p.Parameter.Any(p => (p.Name == "url") || (p.Name == "code") || (p.Name == "context"));
    }

    private static Parameters noDuplicates(Parameters parameters)
    {
        //No duplicate parameters allowed (http://hl7.org/fhir/valueset-operation-validate-code.html)
        if (parameters.TryGetDuplicates(out var duplicates) == true)
        {
            //422 Unproccesable Entity
            throw new FhirOperationException($"List of input parameters contains the following duplicates: {string.Join(", ", duplicates)}", (HttpStatusCode)422);
        }

        return parameters;
    }


    ///<inheritdoc />
    public async Task<Resource> Expand(Parameters parameters, string? id = null, bool useGet = false)
    {
        noDuplicates(parameters);

        string? url = parameters.GetSingleValue<FhirUri>("url")?.Value ?? parameters.GetSingleValue<FhirString>("url")?.Value;
        ValueSet? valueSet = parameters.GetSingle("valueSet")?.Resource as ValueSet;

        if ((valueSet == null) &&
            (url == null))
        {
            // Unprocessable entity
            throw new FhirOperationException("Have to supply either a canonical url or a valueset.", (HttpStatusCode)422);
        }

        FhirString? version = parameters.GetSingleValue<FhirString>("valueSetVersion");

        try
        {
            return valueSet == null
                ? await getExpandedValueSet(new FhirUri(url!), version, "expand").ConfigureAwait(false)
                : await getExpandedValueSet(valueSet, "expand").ConfigureAwait(false);
        }
        catch (Exception e)
        {
            //500 internal server error
            throw new FhirOperationException(e.Message, HttpStatusCode.InternalServerError);
        }
    }

    #region Not implemented methods

    ///<inheritdoc />
    public Task<Parameters> CodeSystemValidateCode(Parameters parameters, string? id = null, bool useGet = false)
    {
        // make this method async, when implementing
        throw new NotImplementedException();
    }

    ///<inheritdoc />
    public Task<Parameters> Lookup(Parameters parameters, bool useGet = false)
    {
        // make this method async, when implementing
        throw new NotImplementedException();
    }

    ///<inheritdoc />
    public Task<Parameters> Translate(Parameters parameters, string? id = null, bool useGet = false)
    {
        // make this method async, when implementing
        throw new NotImplementedException();
    }

    ///<inheritdoc />
    public Task<Parameters> Subsumes(Parameters parameters, string? id = null, bool useGet = false)
    {
        // make this method async, when implementing
        throw new NotImplementedException();
    }

    ///<inheritdoc />
    public Task<Resource> Closure(Parameters parameters, bool useGet = false)
    {
        // make this method async, when implementing
        throw new NotImplementedException();
    }

    #endregion

    private Parameters validateCodeVs(ValueSet vs, CodeableConcept cc, bool? abstractAllowed)
    {
        Parameters result = new Parameters();

        // Maybe just a text, but if there are no codings, that's a positive result
        if (!cc.Coding.Any())
        {
            result.Add("result", new FhirBoolean(true));
            return result;
        }

        // If we have just 1 coding, we better handle this using the simpler version of ValidateBinding
        if (cc.Coding.Count == 1)
        {
            return validateCodeVs(vs, cc.Coding.Single(), abstractAllowed);
        }

        // Look for one succesful match in any of the codes in the CodeableConcept
        Parameters[] callResults = cc.Coding.Select(coding => validateCodeVs(vs, coding, abstractAllowed)).ToArray();

        bool anySuccesful = callResults.Any(p => p.GetSingleValue<FhirBoolean>("result")?.Value == true);
        if (anySuccesful == false)
        {
            StringBuilder messages = new StringBuilder();
            messages.AppendLine("None of the Codings in the CodeableConcept were valid for the binding. Details follow.");

            // gathering the messages of all calls
            foreach (string? msg in callResults.Select(cr => cr.GetSingleValue<FhirString>("message")?.Value).Where(m => m is { }))
            {
                messages.AppendLine(msg);
            }

            result.Add("message", new FhirString(messages.ToString()));
            result.Add("result", new FhirBoolean(false));
        }
        else
        {
            result.Add("result", new FhirBoolean(true));
        }

        return result;
    }

    private Parameters validateCodeVs(ValueSet vs, Coding coding, bool? abstractAllowed)
    {
        return validateCodeVs(vs, coding.Code, coding.System, coding.Display, abstractAllowed);
    }

    private Parameters validateCodeVs(ValueSet vs, string? code, string? system, string? display,
        bool? abstractAllowed)
    {
        if (code is null)
        {
            Parameters resultParam = new Parameters
            {
                { "message", new FhirString("No code supplied.") }, { "result", new FhirBoolean(false) }
            };
            return resultParam;
        }

        ValueSet.ContainsComponent? component = vs.FindInExpansion(code, system);
        string codeLabel = $"Code '{code}'" + (string.IsNullOrEmpty(system) ? string.Empty : $" from system '{system}'");
        Parameters result = new Parameters();
        bool success = true;
        StringBuilder messages = new StringBuilder();

        if (component is null)
        {
            messageForCodeNotFound(vs, system, codeLabel, messages);
            success = false;
        }
        else
        {
            // will be ignored if abstractAllowed == null
            if ((component.Abstract == true) &&
                (abstractAllowed == false))
            {
                messages.AppendLine($"{codeLabel} is abstract, which is not allowed here");
                success = false;
            }

            if ((display != null) &&
                (component.Display != null) &&
                (display != component.Display))
            {
                // this is only a warning (so success is still true)
                messages.AppendLine($"{codeLabel} has incorrect display '{display}', should be '{component.Display}'");
            }

            string? displ = component.Display ?? display;
            if (!string.IsNullOrEmpty(displ))
            {
                result.Add("display", new FhirString(displ));
            }
        }

        result.Add("result", new FhirBoolean(success));
        if (messages.Length > 0)
            result.Add("message", new FhirString(messages.ToString().TrimEnd()));
        return result;
    }

    private void messageForCodeNotFound(
        ValueSet vs,
        string? system,
        string codeLabel,
        StringBuilder messages)
    {
        if ((system != null) && isValueSet(system))
        {
            messages.AppendLine($"The Coding references a value set, not a code system ('{system}')");
        }
        else
        {
            messages.AppendLine($"{codeLabel} does not exist in the value set '{vs.Title ?? vs.Name}' ({vs.Url})");
        }

        return;

        bool isValueSet(string sys)
        {
            if (sys.Contains(@"/ValueSet/"))
            {
                return true;
            }

            if (_dc.TryResolveByCanonicalUri(sys, out Resource? resolvedResource) &&
                (resolvedResource is ValueSet vs))
            {
                return true;
            }

            return false;
        }
    }
}

