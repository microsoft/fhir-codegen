using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime;
using System.Text;
using System.Xml.Linq;
using Hl7.Fhir.Model;
using Hl7.Fhir.Specification.Terminology;
using Hl7.Fhir.Utility;
using Hl7.FhirPath.Sprache;
using Microsoft.Health.Fhir.CodeGen.FhirExtensions;
using Microsoft.Health.Fhir.CodeGen.Models;
using Tasks = System.Threading.Tasks;

namespace Microsoft.Health.Fhir.CodeGen.TerminologyService;

/// <summary>
/// A ValueSet expander, based on the Firely ValueSetExpander.cs implementation
/// </summary>
public class CodeGenValueSetExpander
{
    private readonly CodeGenValueSetExpanderSettings _settings;

    /// <summary>
    /// Settings to control the behaviour of the expansion.
    /// </summary>
    public CodeGenValueSetExpanderSettings Settings => _settings;

    /// <summary>
    /// Create a new expander with specific settings.
    /// </summary>
    /// <param name="settings"></param>
    public CodeGenValueSetExpander(CodeGenValueSetExpanderSettings settings)
    {
        _settings = settings;
    }

    /// <summary>
    /// Create a new expander with default settings
    /// </summary>
    public CodeGenValueSetExpander(DefinitionCollection dc)
        : this(new CodeGenValueSetExpanderSettings() { Definitions = dc, })
    { }

    /// <summary>
    /// Expand the <c>include</c> and <c>exclude</c> filters. Creates the <c></c>
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public Tasks.Task ExpandAsync(ValueSet source) => expandAsync(source, new());

    private async Tasks.Task expandAsync(ValueSet source, Stack<string> inclusionChain)
    {
        // Note we are expanding the valueset in-place, so it's up to the caller to decide whether
        // to clone the valueset, depending on store and performance requirements.
        source.Expansion = ValueSet.ExpansionComponent.Create();
        setExpansionParameters(source);

        try
        {
            inclusionChain.Push(source.Url);
            await handleCompose(source, inclusionChain).ConfigureAwait(false);
        }
        catch (Exception)
        {
            // Expansion failed - remove (partial) expansion
            source.Expansion = null;
            throw;
        }
        finally
        {
            inclusionChain.Pop();
        }
    }

    private void setExpansionParameters(ValueSet vs)
    {
        vs.Expansion.Parameter = [];

        vs.Expansion.Parameter.Add(new()
        {
            Name = "maxExpansionSize",
            Value = new Integer(_settings.MaxExpansionSize)
        });

        if (_settings.ActiveOnly == true)
        {
            vs.Expansion.Parameter.Add(new()
            {
                Name = "activeOnly",
                Value = new FhirBoolean(true)
            });
        }

        if (_settings.IncludeNotSelectable != null)
        {
            vs.Expansion.Parameter.Add(new()
            {
                Name = "includeNotSelectable",
                Value = new FhirBoolean(_settings.IncludeNotSelectable)
            });
        }

        if (_settings.IncludeDesignations == true)
        {
            vs.Expansion.Parameter.Add(new ValueSet.ParameterComponent
            {
                Name = "includeDesignations",
                Value = new FhirBoolean(true)
            });
        }

        //TODO add more parameters to the valuset here when we implement them.
    }

    private async Tasks.Task handleCompose(ValueSet source, Stack<string> inclusionChain)
    {
        if (source.Compose == null) return;

        await handleInclude(source, inclusionChain).ConfigureAwait(false);
        await handleExclude(source, inclusionChain).ConfigureAwait(false);

        removeDuplicatesFromExpansion(source);
    }

    private record class ExpansionTrackRecord
    {
        public required ValueSet.ContainsComponent ToKeep { get; set; }
        public required List<ValueSet.ContainsComponent> ToKeepParent { get; set; } = [];
        public List<(ValueSet.ContainsComponent toRemove, List<ValueSet.ContainsComponent> toRemoveParent)> ToRemove { get; init; } = [];
    }

    private void removeDuplicatesFromExpansion(ValueSet vs)
    {
        if (vs.Expansion == null)
        {
            return;
        }

        Dictionary<(string system, string code), ExpansionTrackRecord> tracker = [];

        track(vs.Expansion.Contains);

        // remove all duplicates that were tracked
        foreach (ExpansionTrackRecord rec in tracker.Values)
        {
            foreach ((ValueSet.ContainsComponent toRemove, List<ValueSet.ContainsComponent> toRemoveParent) in rec.ToRemove)
            {
                toRemoveParent.Remove(toRemove);
            }
        }

        return;

        void track(List<ValueSet.ContainsComponent> components)
        {
            foreach (ValueSet.ContainsComponent component in components)
            {
                if (!string.IsNullOrEmpty(component.Code))
                {
                    (string system, string code) key = (component.System, component.Code);

                    if (tracker.TryGetValue(key, out ExpansionTrackRecord? rec))
                    {
                        // check to see if one has a display and the other not, if so, keep the one with a display
                        if (!string.IsNullOrEmpty(rec.ToKeep.Display) ||
                            string.IsNullOrEmpty(component.Display))
                        {
                            rec.ToRemove.Add((component, components));
                        }
                        else
                        {
                            rec.ToRemove.Add((rec.ToKeep, rec.ToKeepParent));
                            rec.ToKeep = component;
                            rec.ToKeepParent = components;
                        }
                    }
                    else
                    {
                        tracker[key] = new ExpansionTrackRecord() { ToKeep = component, ToKeepParent = components, };
                    }
                }
                if (component.Contains.Count != 0)
                {
                    track(component.Contains);
                }
            }
        }
    }


    private class SystemAndCodeComparer : IEqualityComparer<ValueSet.ContainsComponent>
    {
        public bool Equals(ValueSet.ContainsComponent? x, ValueSet.ContainsComponent? y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (x is null || y is null) return false;

            return x.Code == y.Code && x.System == y.System;
        }

        public int GetHashCode(ValueSet.ContainsComponent obj) => (obj.Code ?? "").GetHashCode() ^ (obj.System ?? "").GetHashCode();
    }

    private static readonly IEqualityComparer<ValueSet.ContainsComponent> _systemAndCodeComparer = new SystemAndCodeComparer();

    // This function contains the main logic of expanding an include/exclude ConceptSet.
    // It processes mainly two parts, which each return 0..* expanded ContainsComponents:
    // * The "System" group (system + filter + concepts).
    // * The "ValueSet" group (valueset)
    // The results of both of these parts are then intersected.
    private async Tasks.Task<List<ValueSet.ContainsComponent>> processConceptSet(ValueSet.ConceptSetComponent conceptSet, Stack<string> inclusionChain)
    {
        // vsd-1
        if ((conceptSet.ValueSetElement.Count == 0) &&
            conceptSet.System == null)
        {
            throw Error.InvalidOperation($"Encountered a ConceptSet with neither a 'system' nor a 'valueset'");
        }

        // Process the system group
        List<ValueSet.ContainsComponent> systemResult = processSystemGroup(conceptSet);

        // Process the ValueSet group
        List<ValueSet.ContainsComponent> valueSetResult = await processValueSetGroup(conceptSet, inclusionChain).ConfigureAwait(false);

        //// filter based on global expansion rules
        //if (_settings.IncludeNotSelectable != true)
        //{
        //    systemResult = systemResult.Where(cc => !cc.Property.Any(p => p.Code == "notSelectable" && p.Value is FhirBoolean fb && fb.Value == true)).ToList();
        //    valueSetResult = systemResult.Where(cc => !cc.Property.Any(p => p.Code == "notSelectable" && p.Value is FhirBoolean fb && fb.Value == true)).ToList();
        //}

        //if (_settings.ActiveOnly == true)
        //{
        //    systemResult = systemResult.Where(cc => cc.Inactive != false).ToList();
        //    valueSetResult = valueSetResult.Where(cc => cc.Inactive != false).ToList();
        //}

        // > For each compose.include: (...) Add the intersection of the result set from the system(step 1) and all of the result sets from the value sets(step 2) to the expansion.
        // Most of the time, the expansion contains stuff from either the system (+enumerated concepts) or the valuesets. 
        // If that is the case, return the result directly. If both were specified, we need to calculate the intersection.
        return (systemResult, valueSetResult) switch
        {
            { systemResult.Count: 0, valueSetResult.Count: 0 } => systemResult, // just return an empty list
            { systemResult.Count: > 0, valueSetResult.Count: 0 } => systemResult,
            { systemResult.Count: 0, valueSetResult.Count: > 0 } => valueSetResult,
            _ => systemResult.Intersect(valueSetResult, _systemAndCodeComparer).ToList()
        };
    }

    // > For each valueSet, find the referenced value set by ValueSet.url, expand that
    // > to produce a collection of result sets.This means that expansion across imports is a recursive process.
    private async Tasks.Task<List<ValueSet.ContainsComponent>> processValueSetGroup(ValueSet.ConceptSetComponent conceptSet, Stack<string> inclusionChain)
    {
        List<ValueSet.ContainsComponent> result = [];

        if (conceptSet.ValueSetElement.Count != 0)
        {
            // > valueSet(s) only: Codes are 'selected' for inclusion if they are in all the referenced value sets
            // "all the referenced sets" means we need to calculate the intersection of the expanded valuesets.
            IEnumerable<ValueSet.ContainsComponent>[] expanded = await Tasks.Task.WhenAll(conceptSet.ValueSet.Select(vs => expandValueSetAndFilterOnSystem(vs))).ConfigureAwait(false);
            IEnumerable<ValueSet.ContainsComponent> concepts = expanded.Length == 1 ? expanded.Single() : expanded.Aggregate((l, r) => l.Intersect(r, _systemAndCodeComparer));

            addCapped(result, concepts, $"Import of valuesets '{string.Join(",", conceptSet.ValueSet)}' would result in an expansion larger than the maximum expansion size.");

            // > valueSet and System: Codes are 'selected' for inclusion if they are selected by the code system selection (after checking for concept and filter) and if they are in all the referenced value sets
            // If a System was specified, simulate a intersection between the codesystem and the valuesets by filtering on the
            // codesystem's canonical. See previous if.
            IEnumerable<ValueSet.ContainsComponent> filterOnSystem(IEnumerable<ValueSet.ContainsComponent> concepts) =>
                conceptSet.System is not null ? concepts.Where(c => c.System == conceptSet.System) : concepts;

            async Tasks.Task<IEnumerable<ValueSet.ContainsComponent>> expandValueSetAndFilterOnSystem(string canonical)
            {
                IEnumerable<ValueSet.ContainsComponent> expansion = await getExpansionForValueSet(canonical, inclusionChain).ConfigureAwait(false);
                return filterOnSystem(expansion);
            }
        }

        return result;
    }

    // > If there is a system, identify the correct version of the code system, and then:
    // > * If there are no codes or filters, add every code in the code system to the result set.
    // > * If codes are listed, check that they are valid, and check their active status, and if ok, add them to the result set(the parameters to the $expand operation may be used to control whether active codes are included).
    // > * If any filters are present, process them in order(as explained above), and add the intersection of their results to the result set.
    private List<ValueSet.ContainsComponent> processSystemGroup(ValueSet.ConceptSetComponent conceptSet)
    {
        if (conceptSet.System == null)
        {
            return [];
        }

        List<ValueSet.ContainsComponent> result = [];

        // We should probably really have to look this code up in the original codesystem to know something about 'abstract'
        // and what would we do with a hierarchy if we encountered that in the include?
        // Filter and Concept are mutually exclusive (vsd-3)
        if (conceptSet.Filter.Count != 0)
        {
            IEnumerable<ValueSet.ContainsComponent> filteredConcepts = CodeGenCodeSystemFilterProcessor.FilterConceptsFromCodeSystem(conceptSet.System, conceptSet.Filter, Settings);
            addCapped(
                result,
                filteredConcepts,
                $"Adding the filtered concepts to the expansion would result in a valueset larger than the maximum expansion size.");
        }
        else if (conceptSet.Concept.Count != 0)
        {
            // TODO: we need to pull display values from the codesystem if they are not specified here.
            //// iterate over the concept set concepts and build the expansion
            //List<ValueSet.ContainsComponent> setContains = [];
            //foreach (ValueSet.ConceptReferenceComponent concept in conceptSet.Concept)
            //{
            //    if (string.IsNullOrEmpty(concept.Code))
            //    {
            //        throw Error.InvalidOperation($"Encountered a ConceptSet with an empty code");
            //    }

            //    // We cannot validate the codes here, because we might be dealing with a codesystem that is not
            //    // present in our definition collection. So we just add the codes as-is.
            //    setContains.Add(ContainsSetExtensions.BuildContainsComponent(
            //        conceptSet.System,
            //        conceptSet.Version,
            //        concept.Code,
            //        concept.Display ?? string.Empty,
            //        _settings.IncludeDesignations == true ? concept.Designation : null));
            //}

            IEnumerable<ValueSet.ContainsComponent> convertedConcepts = conceptSet.Concept
                .Select(c => ContainsSetExtensions.BuildContainsComponent(
                        conceptSet.System,
                        conceptSet.Version,
                        c.Code,
                        c.Display,
                        _settings.IncludeDesignations == true ? c.Designation : null));
            addCapped(
                result,
                convertedConcepts,
                $"Adding the enumerated concepts to the expansion would result in a valueset larger than the maximum expansion size.");
        }
        else if (conceptSet.ValueSetElement.Count == 0)
        {
            // Do a full import of the codesystem. Conceptually, if a ValueSet is specified, we should include the
            // *intersection* of the ValueSets and the System. That is computationally expensive, so instead we will not
            // include the Codesystem at all if there are valuesets, but include the ValueSets instead, filtering them
            // on the given system instead (see next if). This is not the same if there are codes in the valueset that
            // use a system, but are not actually defined within that codesystem, but that sounds illegal to me anyway.
            IEnumerable<ValueSet.ContainsComponent> importedConcepts = getAllConceptsFromCodeSystem(conceptSet.System);
            addCapped(
                result,
                importedConcepts,
                $"Import of full codesystem '{conceptSet.System}' would result in an expansion larger than the maximum expansion size.");
        }

        return result;
    }

    private void addCapped(List<ValueSet.ContainsComponent> dest, IEnumerable<ValueSet.ContainsComponent> source, string message)
    {
        int capacityLeft = Settings.MaxExpansionSize - dest.Count;
        List<ValueSet.ContainsComponent> cappedSource = source.Take(capacityLeft + 1).ToList();

        if (cappedSource.Count == capacityLeft + 1)
        {
            throw new ValueSetExpansionTooBigException(message);
        }

        dest.AddRange(cappedSource);
    }

    private async Tasks.Task handleInclude(ValueSet source, Stack<string> inclusionChain)
    {
        if (source.Compose.Include.Count == 0)
        {
            return;
        }

        int csIndex = 0;
        foreach (ValueSet.ConceptSetComponent? include in source.Compose.Include)
        {
            List<ValueSet.ContainsComponent> includedConcepts = await processConceptSet(include, inclusionChain).ConfigureAwait(false);

            // Yes, exclusion could make this smaller again, but alas, before we have processed those we might have run out of memory
            addCapped(
                source.Expansion.Contains,
                includedConcepts,
                $"Inclusion of {includedConcepts.Count} concepts from conceptset #{csIndex}' to" +
                    $" valueset '{source.Url}' ({source.Expansion.Total} concepts) would be larger than" +
                    $" the set maximum size ({Settings.MaxExpansionSize})");

            int original = source.Expansion.Total ?? 0;
            source.Expansion.Total = original + includedConcepts.CountConcepts();
            csIndex += 1;
        }
    }

    private async Tasks.Task handleExclude(ValueSet source, Stack<string> inclusionChain)
    {
        if (source.Compose.Exclude.Count == 0)
        {
            return;
        }

        foreach (ValueSet.ConceptSetComponent? exclude in source.Compose.Exclude)
        {
            List<ValueSet.ContainsComponent> excludedConcepts = await processConceptSet(exclude, inclusionChain).ConfigureAwait(false);

            source.Expansion.Contains.RemoveRecursive(excludedConcepts);

            int original = source.Expansion.Total ?? 0;
            source.Expansion.Total = original - excludedConcepts.CountConcepts();
        }
    }

    private async Tasks.Task<IEnumerable<ValueSet.ContainsComponent>> getExpansionForValueSet(string uri, Stack<string> inclusionChain)
    {
        if (inclusionChain.Contains(uri))
        {
            throw new TerminologyServiceException($"ValueSet expansion encountered a cycling dependency from {inclusionChain.Peek()} back to {uri}.");
        }

        if (_settings.Definitions == null)
        {
            throw Error.InvalidOperation($"No resolver available for valueset '{uri}', so the expansion cannot be completed.");
        }

        if ((!_settings.Definitions.TryResolveByCanonicalUri(uri, out Resource? canonical)) ||
            (canonical is not ValueSet importedVs))
        {
            throw new ValueSetUnknownException($"The ValueSet expander cannot find valueset '{uri}', so the expansion cannot be completed.");
        }

        if (!importedVs.HasExpansion)
        {
            await expandAsync(importedVs, inclusionChain).ConfigureAwait(false);
        }

        return importedVs.HasExpansion
            ? importedVs.Expansion.Contains
            : throw new ValueSetUnknownException($"Expansion returned neither an error, nor an expansion for ValueSet with canonical reference '{uri}'");
    }

    private IEnumerable<ValueSet.ContainsComponent> getAllConceptsFromCodeSystem(string uri)
    {
        if (_settings.Definitions == null)
        {
            throw Error.InvalidOperation($"No resolver available for codesystem '{uri}', so the expansion cannot be completed.");
        }

        if ((!_settings.Definitions.TryResolveByCanonicalUri(uri, out Resource? canonical)) ||
            (canonical is not CodeSystem importedCs))
        {
            throw new ValueSetUnknownException($"The ValueSet expander cannot find codesystem '{uri}', so the expansion cannot be completed.");
        }

        if (importedCs.Compositional is true)
        {
            throw new ValueSetExpansionTooComplexException($"The ValueSet expander cannot expand compositional code system '{uri}', so the expansion cannot be completed.");
        }

        bool contentNotPresent = importedCs.Content == CodeSystemContentMode.NotPresent;

        if (contentNotPresent)
        {
            throw new ValueSetExpansionTooComplexException($"The ValueSet expander cannot expand code system '{uri}' without content, so the expansion cannot be completed.");
        }

        return importedCs.Concept.Select(c => c.ToContainsComponent(importedCs, Settings));
    }
}


public static class ContainsSetExtensions
{
    internal static ValueSet.ContainsComponent BuildContainsComponent(
        string system,
        string version,
        string code,
        string display,
        List<ValueSet.DesignationComponent>? designations = null,
        IEnumerable<ValueSet.ContainsComponent>? children = null) => new ValueSet.ContainsComponent
        {
            System = system,
            Code = code,
            Display = display,
            Version = version,
            Designation = designations,
            Contains = children?.ToList() ?? [],
        };

    public static ValueSet.ContainsComponent Add(this List<ValueSet.ContainsComponent> dest, string system, string version, string code, string display, List<ValueSet.DesignationComponent>? designations = null, IEnumerable<ValueSet.ContainsComponent>? children = null)
    {
        ValueSet.ContainsComponent newContains = BuildContainsComponent(system, version, code, display, designations, children);
        dest.Add(newContains);

        return newContains;
    }

    public static void RemoveRecursive(this List<ValueSet.ContainsComponent> dest, string system, string code)
    {
        List<ValueSet.ContainsComponent> children = dest.Where(c => c.System == system && c.Code == code).SelectMany(c => c.Contains).ToList();
        dest.RemoveAll(c => c.System == system && c.Code == code);

        //add children back to the list, they do not necessarily need to be removed when the parent is removed.
        dest.AddRange(children);

        // Look for this code in children too
        foreach (ValueSet.ContainsComponent component in dest)
        {
            if (component.Contains.Count != 0)
            {
                component.Contains.RemoveRecursive(system, code);
            }
        }
    }

    public static void RemoveRecursive(this List<ValueSet.ContainsComponent> dest, List<ValueSet.ContainsComponent> source)
    {
        foreach (ValueSet.ContainsComponent sourceConcept in source)
        {
            dest.RemoveRecursive(sourceConcept.System, sourceConcept.Code);

            //check if there are children that need to be removed too.
            if (sourceConcept.Contains.Count != 0)
            {
                dest.RemoveRecursive(sourceConcept.Contains);
            }
        }
    }

    internal static ValueSet.ContainsComponent ToContainsComponent(
        this CodeSystem.ConceptDefinitionComponent source,
        CodeSystem system,
        CodeGenValueSetExpanderSettings settings)
    {
        bool? isAbstract = null;
        bool? isInactive = null;

        ILookup<string, CodeSystem.ConceptPropertyComponent> conceptPropertiesByCode = source.Property.ToLookup(p => p.Code);
        ILookup<string?, string> csPropertiesByUri = system.Property.ToLookup(p => string.IsNullOrEmpty(p.Uri) ? null : p.Uri, p => p.Code);

        if (csPropertiesByUri.Contains(CodeSystem.CONCEPTPROPERTY_NOT_SELECTABLE))
        {
            string code = csPropertiesByUri[CodeSystem.CONCEPTPROPERTY_NOT_SELECTABLE].Single();
            if (conceptPropertiesByCode.Contains(code) &&
                conceptPropertiesByCode[code].Single().Value is FhirBoolean fb)
            {
                isAbstract = fb.Value;
            }
        }
        else if (conceptPropertiesByCode.Contains("notSelectable") &&
                 conceptPropertiesByCode["notSelectable"].Single().Value is FhirBoolean fbNotSelectable)
        {
            isAbstract = fbNotSelectable.Value;
        }

        if (csPropertiesByUri.Contains(CodeSystem.CONCEPTPROPERTY_INACTIVE))
        {
            string code = csPropertiesByUri[CodeSystem.CONCEPTPROPERTY_INACTIVE].Single();
            if (conceptPropertiesByCode.Contains(code) &&
                conceptPropertiesByCode[code].Single().Value is FhirBoolean fb)
            {
                isInactive = fb.Value;
            }
        }
        else if (conceptPropertiesByCode.Contains("inactive") &&
                 conceptPropertiesByCode["inactive"].Single().Value is FhirBoolean fbInactive)
        {
            isAbstract = fbInactive.Value;
        }

        ValueSet.ContainsComponent newContains = new()
        {
            System = system.Url,
            Version = system.Version,
            Code = source.Code,
            Display = source.Display,
            Abstract = isAbstract,
            Inactive = isInactive,
            Designation = settings.IncludeDesignations == true ? source.Designation.toValueSetDesignations() : [],
            Property = source.Property.Select(p => new ValueSet.ConceptPropertyComponent { Code = p.Code, Value = p.Value }).ToList(),
            Contains = source.Concept.Select(c => c.ToContainsComponent(system, settings)).ToList(),
        };

        return newContains;
    }

    private static List<ValueSet.DesignationComponent> toValueSetDesignations(this List<CodeSystem.DesignationComponent> csDesignations)
    {
        return csDesignations.Select(d => new ValueSet.DesignationComponent
        {
            Language = d.Language,
            Use = d.Use,
            Value = d.Value
        }).ToList();
    }
}

