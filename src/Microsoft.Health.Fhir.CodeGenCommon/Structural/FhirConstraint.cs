// <copyright file="FhirConstraint.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using Microsoft.Health.Fhir.CodeGenCommon.Extensions;
using static Microsoft.Health.Fhir.CodeGenCommon.Models.FhirSlicing;
using static Microsoft.Health.Fhir.CodeGenCommon.Structural.FhirSlicing;

namespace Microsoft.Health.Fhir.CodeGenCommon.Structural;

/// <summary>A FHIR constraint.</summary>
public record class FhirConstraint
{
    private FhirConstraintSeverityCodes _severity;
    private string _fhirSeverity = string.Empty;

    /// <summary>
    /// Values that represent FHIR constraint severity codes.
    /// http://hl7.org/fhir/ValueSet/constraint-severity
    /// </summary>
    public enum FhirConstraintSeverityCodes
    {
        /// <summary>If the constraint is violated, the resource is not conformant.</summary>
        [FhirLiteral("error")]
        Error,

        /// <summary>If the constraint is violated, the resource is conformant, but it is not necessarily following best practice.</summary>
        [FhirLiteral("warning")]
        Warning,
    }

    /// <summary>Initializes a new instance of the FhirConstraint class.</summary>
    /// <param name="other">The other.</param>
    protected FhirConstraint(FhirConstraint other)
    {
        Key = other.Key;
        Requirements = other.Requirements;
        FhirSeverity = other.FhirSeverity;
        Suppress = other.Suppress;
        Description = other.Description;
        Expression = other.Expression;
        IsBestPractice = other.IsBestPractice;
        BestPracticeExplanation = other.BestPracticeExplanation;
        SourceCanonical = other.SourceCanonical;
        ContextPath = other.ContextPath;
        IsInherited = other.IsInherited;
    }

    /// <summary>Gets the constraint key (id) - the target of 'condition' reference above.</summary>
    public required string Key { get; init; }

    /// <summary>Gets why this constraint is necessary or appropriate.</summary>
    public string Requirements { get; init; } = string.Empty;

    /// <summary>Gets the severity for violating the constraint.</summary>
    public FhirConstraintSeverityCodes Severity { get => _severity; }

    /// <summary>Gets or initializes the severity for violating the constraint (error|warning).</summary>
    public required string FhirSeverity
    {
        get => _fhirSeverity;
        init
        {
            _fhirSeverity = value;
            if (_fhirSeverity.TryFhirEnum(out FhirConstraintSeverityCodes v))
            {
                _severity = v;
            }
        }
    }

    /// <summary>Gets a flag to suppress warning or hint in profile.</summary>
    public bool? Suppress { get; init; } = null;

    /// <summary>Gets the human-readable description of the constraint.</summary>
    public required string Description { get; init; }

    /// <summary>Gets the FHIRPath validation expression.</summary>
    public string Expression { get; init; } = string.Empty;

    /// <summary>Gets a value indicating whether this constraint is a best practice.</summary>
    public bool IsBestPractice { get; init; } = false;

    /// <summary>Gets a value indicating why this constraint is a best practice.</summary>
    public string BestPracticeExplanation { get; init; } = string.Empty;

    /// <summary>Gets source canonical - a reference to the original source of constraint.</summary>
    public string SourceCanonical { get; init; } = string.Empty;

    /// <summary>Gets the fhir path for the context of this definition (e.g., resource or element path).</summary>
    public string ContextPath { get; init; } = string.Empty;

    /// <summary>Gets a value indicating whether this constraint is inherited from a parent definition.</summary>
    public bool IsInherited { get; init; } = false;
}
