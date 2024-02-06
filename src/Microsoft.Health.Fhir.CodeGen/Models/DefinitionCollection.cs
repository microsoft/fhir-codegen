// <copyright file="DefinitionCollection.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>


using Hl7.Fhir.Model;
using Microsoft.Health.Fhir.CodeGenCommon.Packaging;

namespace Microsoft.Health.Fhir.CodeGen.Models;

/// <summary>A FHIR package and its contents.</summary>
public class DefinitionCollection
{
    /// <summary>Gets or sets the name.</summary>
    public required string Name { get; set; }

    /// <summary>Gets or sets the FHIR version.</summary>
    public FHIRVersion? FhirVersion { get; set; } = null;

    public FhirReleases.FhirSequenceCodes FhirSequence { get; set; } = FhirReleases.FhirSequenceCodes.Unknown;

    /// <summary>Gets or sets the manifest.</summary>
    public Dictionary<string, CachePackageManifest> Manifests { get; set; } = new();

    /// <summary>Gets or sets the contents.</summary>
    public Dictionary<string, PackageContents> ContentListings { get; set; } = new();
    
    private readonly Dictionary<string, StructureDefinition> _primitiveTypesByName = new();
    private readonly Dictionary<string, StructureDefinition> _complexTypesByName = new();
    private readonly Dictionary<string, StructureDefinition> _resourcesByName = new();
    private readonly Dictionary<string, StructureDefinition> _logicalModelsByName = new();
    private readonly Dictionary<string, StructureDefinition> _extensionsByUrl = new();
    private readonly Dictionary<string, Dictionary<string, StructureDefinition>> _extensionsByPath = new();
    private readonly Dictionary<string, StructureDefinition> _profilesByUrl = new();
    private readonly Dictionary<string, Dictionary<string, StructureDefinition>> _profilesByBaseType = new();

    private readonly Dictionary<string, OperationDefinition> _systemOperations = new();
    private readonly Dictionary<string, OperationDefinition> _operationsByUrl = new();

    private readonly Dictionary<string, SearchParameter> _globalSearchParameters = new();
    private readonly Dictionary<string, SearchParameter> _searchResultParameters = new();
    private readonly Dictionary<string, SearchParameter> _allInteractionParameters = new();
    private readonly Dictionary<string, SearchParameter> _searchParamsByUrl = new();

    private readonly Dictionary<string, CodeSystem> _codeSystemsByUrl = new();
    private readonly Dictionary<string, ValueSet> _valueSetsByUrl = new();

    private readonly Dictionary<string, ImplementationGuide> _implementationGuidesByUrl = new();
    private readonly Dictionary<string, CapabilityStatement> _capabilityStatementsByUrl = new();
    private readonly Dictionary<string, CompartmentDefinition> _compartmentsByUrl = new();


    /// <summary>Gets URL of the code systems by.</summary>
    public IReadOnlyDictionary<string, CodeSystem> CodeSystemsByUrl => _codeSystemsByUrl;

    /// <summary>Adds a code system.</summary>
    /// <param name="codeSystem">The code system.</param>
    public void AddCodeSystem(CodeSystem codeSystem)
    {
        _codeSystemsByUrl[codeSystem.Url] = codeSystem;
    }

    /// <summary>Gets URL of the value sets by.</summary>
    public IReadOnlyDictionary<string, ValueSet> ValueSetsByUrl => _valueSetsByUrl;

    /// <summary>Adds a value set.</summary>
    /// <param name="valueSet">Set the value belongs to.</param>
    public void AddValueSet(ValueSet valueSet)
    {
        _valueSetsByUrl[valueSet.Url] = valueSet;
    }

    /// <summary>Gets the name of the primitive types by.</summary>
    public IReadOnlyDictionary<string, StructureDefinition> PrimitiveTypesByName => _primitiveTypesByName;

    /// <summary>Adds a primitive type.</summary>
    /// <param name="sd">The structure definition.</param>
    public void AddPrimitiveType(StructureDefinition sd)
    {
        // TODO(ginoc): Consider if we want to make this explicit on any definitions that do not have it
        //if (sd.FhirVersion == null)
        //{
        //    sd.FhirVersion = FhirVersion;
        //}

        _primitiveTypesByName[sd.Name] = sd;
    }

    public IReadOnlyDictionary<string, StructureDefinition> ComplexTypesByName => _complexTypesByName;

    public void AddComplexType(StructureDefinition structureDefinition)
    {
        _complexTypesByName[structureDefinition.Name] = structureDefinition;
    }

    public IReadOnlyDictionary<string, StructureDefinition> ResourcesByName => _resourcesByName;

    public void AddResource(StructureDefinition structureDefinition)
    {
        _resourcesByName[structureDefinition.Name] = structureDefinition;
    }

    public IReadOnlyDictionary<string, StructureDefinition> LogicalModelsByName => _logicalModelsByName;

    public void AddLogicalModel(StructureDefinition structureDefinition)
    {
        _logicalModelsByName[structureDefinition.Url] = structureDefinition;
    }

    public IReadOnlyDictionary<string, StructureDefinition> ExtensionsByUrl => _extensionsByUrl;

    public void AddExtension(StructureDefinition structureDefinition)
    {
        _extensionsByUrl[structureDefinition.Url] = structureDefinition;
    }

    public IReadOnlyDictionary<string, StructureDefinition> ProfilesByUrl => _profilesByUrl;

    public void AddProfile(StructureDefinition structureDefinition)
    {
        _profilesByUrl[structureDefinition.Url] = structureDefinition;
    }

    public IReadOnlyDictionary<string, SearchParameter> SearchParametersByUrl => _searchParamsByUrl;

    public void AddSearchParameter(SearchParameter searchParameter)
    {
        _searchParamsByUrl[searchParameter.Url] = searchParameter;
    }

    public IReadOnlyDictionary<string, OperationDefinition> OperationsByUrl => _operationsByUrl;

    public void AddOperation(OperationDefinition operationDefinition)
    {
        _operationsByUrl[operationDefinition.Url] = operationDefinition;
    }

    public IReadOnlyDictionary<string, CapabilityStatement> CapabilityStatementsByUrl => _capabilityStatementsByUrl;

    public void AddCapabilityStatement(CapabilityStatement capabilityStatement)
    {
        _capabilityStatementsByUrl[capabilityStatement.Url] = capabilityStatement;
    }

    public IReadOnlyDictionary<string, ImplementationGuide> ImplementationGuidesByUrl => _implementationGuidesByUrl;

    public void AddImplementationGuide(ImplementationGuide implementationGuide)
    {
        _implementationGuidesByUrl[implementationGuide.Url] = implementationGuide;
    }

    public IReadOnlyDictionary<string, CompartmentDefinition> CompartmentsByUrl => _compartmentsByUrl;

    public void AddCompartment(CompartmentDefinition compartmentDefinition)
    {
        _compartmentsByUrl[compartmentDefinition.Url] = compartmentDefinition;
    }
}
