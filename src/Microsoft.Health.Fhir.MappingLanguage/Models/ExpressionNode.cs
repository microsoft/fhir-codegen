// <copyright file="ExpressionNode.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Microsoft.Health.Fhir.MappingLanguage.Expression;

/// <summary>An expression node.</summary>
public partial class ExpressionNode
{
    /// <summary>Values that represent kind codes.</summary>
    public enum NodeKindCodes
    {
        Name,
        Function,
        Constant,
        Group,
        Unary,
    }

    /// <summary>Values that represent collection status codes.</summary>
    public enum CollectionStatusCodes
    {
        Singleton,
        Ordered,
        Unordered,
    }

    ////the expression will have one of either name or constant
    //private string uniqueId;
    //private NodeKindCodes kind;
    //private string name;
    //private Hl7.Fhir.Model.Base constant;
    //private Function? function;
    //private List<ExpressionNode> parameters; // will be created if there is a function
    //private ExpressionNode inner;
    //private ExpressionNode group;
    //private OperationCodes? operation;
    //private bool proximal; // a proximal operation is the first in the sequence of operations. This is significant when evaluating the outcomes
    //private ExpressionNode opNext;
    //private SourceLocation start;
    //private SourceLocation end;
    //private SourceLocation opStart;
    //private SourceLocation opEnd;
    //private TypeDetails types;
    //private TypeDetails opTypes;

}

/// <summary>A collection status extensions.</summary>
public static class CollectionStatusExtensions
{
    /// <summary>
    /// The ExpressionNode.CollectionStatusCodes extension method that convert this object into a
    /// string representation.
    /// </summary>
    /// <param name="collectionStatus">The collection status.</param>
    /// <returns>A string that represents this object.</returns>
    public static string ToString(this ExpressionNode.CollectionStatusCodes collectionStatus) => collectionStatus switch
    {
        ExpressionNode.CollectionStatusCodes.Singleton => "SINGLETON",
        ExpressionNode.CollectionStatusCodes.Ordered => "ORDERED",
        ExpressionNode.CollectionStatusCodes.Unordered => "UNORDERED",
        _ => throw new InvalidOperationException($"Unknown collection status: {collectionStatus}"),
    };
}
