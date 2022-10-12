// <copyright file="FhirSliceDiscriminatorRule.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

namespace Microsoft.Health.Fhir.CodeGenCommon.Models;

/// <summary>A fhir slice discriminator rule.</summary>
public class FhirSliceDiscriminatorRule
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FhirSliceDiscriminatorRule"/> class.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="path">The full pathname of the file.</param>
    public FhirSliceDiscriminatorRule(
        string type,
        string path)
    {
        switch (type)
        {
            case "value":
                DiscriminatorType = FhirSliceDiscriminatorType.Value;
                break;

            case "exists":
                DiscriminatorType = FhirSliceDiscriminatorType.Exists;
                break;

            case "pattern":
                DiscriminatorType = FhirSliceDiscriminatorType.Pattern;
                break;

            case "type":
                DiscriminatorType = FhirSliceDiscriminatorType.Type;
                break;

            case "profile":
                DiscriminatorType = FhirSliceDiscriminatorType.Profile;
                break;

            default:
                throw new ArgumentException($"Invalid Slice Discriminator type: {type}");
        }

        Path = path;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FhirSliceDiscriminatorRule"/> class.
    /// </summary>
    /// <param name="discriminatorType">The type.</param>
    /// <param name="path">The full pathname of the file.</param>
    [System.Text.Json.Serialization.JsonConstructor]
    public FhirSliceDiscriminatorRule(
        FhirSliceDiscriminatorType discriminatorType,
        string path)
    {
        DiscriminatorType = discriminatorType;
        Path = path;
    }

    /// <summary>Values that represent fhir slice discriminator types.</summary>
    public enum FhirSliceDiscriminatorType
    {
        /// <summary>The slices have different values in the nominated element.</summary>
        Value,

        /// <summary>The slices are differentiated by the presence or absence of the nominated element.</summary>
        Exists,

        /// <summary>
        /// The slices have different values in the nominated element, as determined by testing them
        /// against the applicable ElementDefinition.pattern[x].
        /// </summary>
        Pattern,

        /// <summary>The slices are differentiated by type of the nominated element.</summary>
        Type,

        /// <summary>
        /// The slices are differentiated by conformance of the nominated element to a specified
        /// profile. Note that if the path specifies .resolve() then the profile is the target profile
        /// on the reference. In this case, validation by the possible profiles is required to differentiate
        /// the slices.
        /// </summary>
        Profile,
    }

    /// <summary>Gets the type of the discriminator.</summary>
    /// <value>The type of the discriminator.</value>
    public FhirSliceDiscriminatorType DiscriminatorType { get; }

    /// <summary>Gets the name of the discriminator type.</summary>
    /// <value>The name of the discriminator type.</value>
    public string DiscriminatorTypeName
    {
        get
        {
            switch (DiscriminatorType)
            {
                case FhirSliceDiscriminatorType.Value:
                    return "value";

                case FhirSliceDiscriminatorType.Exists:
                    return "exists";

                case FhirSliceDiscriminatorType.Pattern:
                    return "pattern";

                case FhirSliceDiscriminatorType.Type:
                    return "type";

                case FhirSliceDiscriminatorType.Profile:
                    return "profile";
            }

            return string.Empty;
        }
    }

    /// <summary>Gets the full pathname of the file.</summary>
    /// <value>The full pathname of the file.</value>
    public string Path { get; }

    /// <summary>Deep copy.</summary>
    /// <returns>A FhirSliceDiscriminatorRule.</returns>
    public FhirSliceDiscriminatorRule DeepCopy()
    {
        return new FhirSliceDiscriminatorRule(DiscriminatorType, Path);
    }
}
