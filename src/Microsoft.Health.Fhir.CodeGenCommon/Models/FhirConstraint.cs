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
    /// <param name="requirements">           Why this constraint is necessary or appropriate.</param>
    /// <param name="severity">               The severity for violating the constraint.</param>
    /// <param name="suppress">               True to suppress warning or hint in profile.</param>
    /// <param name="description">            The human-readable description of the constraint.</param>
    /// <param name="expression">             The FHIR Query-style validation expression.</param>
    /// <param name="xPath">                  The XPath-style validation expression.</param>
    /// <param name="isBestPractice">         A value indicating whether this constraint is a best
    ///  practice.</param>
    /// <param name="bestPracticeExplanation">A value indicating why this constraint is a best practice.</param>
    /// <param name="sourceCanonical">        Reference to original source of constraint.</param>
    /// <param name="contextPath">            Context of this constraint.</param>
    public FhirConstraint(
        string key,
        string requirements,
        string severity,
        bool? suppress,
        string description,
        string expression,
        string xPath,
        bool isBestPractice,
        string bestPracticeExplanation,
        string sourceCanonical,
        string contextPath)
    {
        Key = key;
        Requirements = requirements;
        Severity = severity;
        Suppress = suppress;
        Description = description;
        Expression = expression;
        XPath = xPath;
        IsBestPractice = isBestPractice;
        BestPracticeExplanation = bestPracticeExplanation;
        SourceCanonical = sourceCanonical;
        ContextPath = contextPath;

        if ((!string.IsNullOrEmpty(contextPath)) &&
            (!string.IsNullOrEmpty(sourceCanonical)))
        {
            string root = contextPath.Contains('.') ? contextPath.Substring(0, contextPath.IndexOf('.')) : contextPath;

            if (sourceCanonical.EndsWith(root))
            {
                IsInherited = false;
            }
            else
            {
                IsInherited = true;
            }
        }
        else
        {
            // missing source element means it is locally defined
            IsInherited = false;
        }

    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FhirConstraint"/> class.
    /// </summary>
    /// <param name="source">Source for the.</param>
    public FhirConstraint(FhirConstraint source)
    {
        Key = source.Key;
        Requirements = source.Requirements;
        Severity = source.Severity;
        Suppress = source.Suppress;
        Description = source.Description;
        Expression = source.Expression;
        XPath = source.XPath;
        IsBestPractice = source.IsBestPractice;
        BestPracticeExplanation = source.BestPracticeExplanation;
        SourceCanonical = source.SourceCanonical;
        ContextPath = source.ContextPath;
        IsInherited = source.IsInherited;
    }

    /// <summary>Gets the constraint key (id).</summary>
    public string Key { get; }

    /// <summary>Gets why this constraint is necessary or appropriate.</summary>
    public string Requirements { get; }

    /// <summary>Gets the severity for violating the constraint.</summary>
    public string Severity { get; }

    /// <summary>Gets a flag to suppress warning or hint in profile.</summary>
    public bool? Suppress { get; }

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

    /// <summary>Gets source canonical.</summary>
    public string SourceCanonical { get; }

    /// <summary>Gets the full pathname of the context file.</summary>
    public string ContextPath { get; }

    /// <summary>Gets a value indicating whether this object is inherited.</summary>
    public bool IsInherited { get; set; }

    /// <summary>Creates a new object that is a copy of the current instance.</summary>
    /// <returns>A new object that is a copy of this instance.</returns>
    public object Clone()
    {
        return new FhirConstraint(this);
    }
}
