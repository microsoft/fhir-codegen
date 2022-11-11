// <copyright file="FhirConstraint.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System.Linq.Expressions;

namespace Microsoft.Health.Fhir.CodeGenCommon.Models;

/// <summary>A FHIR constraint.</summary>
public class FhirConstraint : ICloneable
{
    /// <summary>Initializes a new instance of the <see cref="FhirConstraint"/> class.</summary>
    /// <param name="key">                    The constraint key (id).</param>
    /// <param name="severity">               The severity for violating the constraint.</param>
    /// <param name="description">            The human-readable description of the constraint.</param>
    /// <param name="expression">             The FHIR Query-style validation expression.</param>
    /// <param name="xPath">                  The XPath-style validation expression.</param>
    /// <param name="isBestPractice">         A value indicating whether this constraint is a best
    ///  practice.</param>
    /// <param name="bestPracticeExplanation">A value indicating why this constraint is a best practice.</param>
    public FhirConstraint(
        string key,
        string severity,
        string description,
        string expression,
        string xPath,
        bool isBestPractice,
        string bestPracticeExplanation)
    {
        Key = key;
        Severity = severity;
        Description = description;
        Expression = expression;
        XPath = xPath;
        IsBestPractice = isBestPractice;
        BestPracticeExplanation = bestPracticeExplanation;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FhirConstraint"/> class.
    /// </summary>
    /// <param name="source">Source for the.</param>
    public FhirConstraint(FhirConstraint source)
    {
        Key = source.Key;
        Severity = source.Severity;
        Description = source.Description;
        Expression = source.Expression;
        XPath = source.XPath;
        IsBestPractice = source.IsBestPractice;
        BestPracticeExplanation = source.BestPracticeExplanation;
    }

    /// <summary>Gets the constraint key (id).</summary>
    public string Key { get; }

    /// <summary>Gets the severity for violating the constraint.</summary>
    public string Severity { get; }

    /// <summary>Gets the human-readable description of the constraint.</summary>
    public string Description { get; }

    /// <summary>Gets the FHIR Query-style validation expression.</summary>
    public string Expression { get; }

    /// <summary>Gets the XPath-style validation expression.</summary>
    public string XPath { get; }

    /// <summary>Gets a value indicating whether this constraint is a best practice.</summary>
    public bool IsBestPractice { get; }

    /// <summary>Gets a value indicating why this constraint is a best practice.</summary>
    public string BestPracticeExplanation { get; }

    /// <summary>Creates a new object that is a copy of the current instance.</summary>
    /// <returns>A new object that is a copy of this instance.</returns>
    public object Clone()
    {
        return new FhirConstraint(this);
    }
}
