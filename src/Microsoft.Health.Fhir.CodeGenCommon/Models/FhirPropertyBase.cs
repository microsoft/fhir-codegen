// <copyright file="FhirPropertyBase.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

namespace Microsoft.Health.Fhir.CodeGenCommon.Models;

/// <summary>A FHIR property base.</summary>
public abstract class FhirPropertyBase : FhirTypeBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FhirPropertyBase"/> class.
    /// </summary>
    /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
    /// <param name="rootArtifact">    The root artifact that contains this definition.</param>
    /// <param name="id">              The identifier.</param>
    /// <param name="path">            The dot-notation path to this element/resource/datatype.</param>
    /// <param name="basePath">        The dot-notation path to the base definition for this record.</param>
    /// <param name="url">             URL of the resource.</param>
    /// <param name="shortDescription">Information describing the short.</param>
    /// <param name="purpose">         The purpose.</param>
    /// <param name="comment">         The comment.</param>
    /// <param name="validationRegEx"> The validation RegEx.</param>
    /// <param name="mappings">        Element definition mappings to external properties.</param>
    public FhirPropertyBase(
        FhirComplex rootArtifact,
        string id,
        string path,
        string basePath,
        Uri url,
        string shortDescription,
        string purpose,
        string comment,
        string validationRegEx,
        Dictionary<string, List<FhirElementDefMapping>> mappings)
        : this(
            rootArtifact,
            id,
            path,
            basePath,
            string.Empty,
            string.Empty,
            url,
            shortDescription,
            purpose,
            comment,
            validationRegEx,
            mappings)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FhirPropertyBase"/> class.
    /// </summary>
    /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
    /// <param name="rootArtifact">    The root artifact that contains this definition.</param>
    /// <param name="id">              The identifier.</param>
    /// <param name="path">            The dot-notation path to this property (element).</param>
    /// <param name="basePath">        The dot-notation path to the base definition for this record.</param>
    /// <param name="baseTypeName">    The base definition for this property (element).</param>
    /// <param name="baseTypeCanonical">The base type canonical.</param>
    /// <param name="url">             URL of the resource.</param>
    /// <param name="shortDescription">Information describing the short.</param>
    /// <param name="purpose">         The purpose.</param>
    /// <param name="comment">         The comment.</param>
    /// <param name="validationRegEx"> The validation RegEx.</param>
    /// <param name="mappings">        Element definition mappings to external properties.</param>
    public FhirPropertyBase(
        FhirComplex rootArtifact,
        string id,
        string path,
        string basePath,
        string baseTypeName,
        string baseTypeCanonical,
        Uri url,
        string shortDescription,
        string purpose,
        string comment,
        string validationRegEx,
        Dictionary<string, List<FhirElementDefMapping>> mappings)
        : base(
            id,
            path.Split('.').Last() ?? string.Empty,
            path,
            baseTypeName,
            baseTypeCanonical,
            url,
            shortDescription,
            purpose,
            comment,
            validationRegEx)
    {
        // sanity checks
        if (string.IsNullOrEmpty(path))
        {
            throw new ArgumentNullException(nameof(path));
        }

        RootArtifact = rootArtifact;
        BasePath = basePath;
        Mappings = mappings ?? new();

        // filter out FiveWs oddity
        if (Mappings.ContainsKey("w5") && (Mappings["w5"].Count > 1))
        {
            FhirElementDefMapping subjectX = Mappings["w5"].FirstOrDefault(m => m.Map.Equals("FiveWs.subject[x]", StringComparison.OrdinalIgnoreCase));

            if (subjectX != null)
            {
                Mappings["w5"].Remove(subjectX);
            }
        }
    }

    /// <summary>Gets the dot-notation path to the base definition for this record.</summary>
    public string BasePath { get; }

    /// <summary>Gets the root artifact.</summary>
    public FhirComplex RootArtifact { get; }

    /// <summary>Gets the mappings.</summary>
    public Dictionary<string, List<FhirElementDefMapping>> Mappings { get; } = new();
}
