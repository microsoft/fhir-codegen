// <copyright file="ElementDefinitionComparer.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using Hl7.Fhir.Model;
using Microsoft.Health.Fhir.CodeGen.FhirExtensions;

namespace Microsoft.Health.Fhir.CodeGen.Models;

public class ElementDefinitionComparer : IComparer<ElementDefinition>
{
    public static readonly ElementDefinitionComparer Instance = new();

    public int Compare(ElementDefinition? x, ElementDefinition? y)
    {
        if (x == null && y == null)
        {
            return 0;
        }

        if (x == null)
        {
            return -1;
        }

        if (y == null)
        {
            return 1;
        }

        if (x.Path == y.Path)
        {
            return 0;
        }

        if (x.Path == null)
        {
            return -1;
        }

        if (y.Path == null)
        {
            return 1;
        }

        // if the paths are different, first compare the root dot component
        string xRoot = x.Path.Split('.')[0];
        string yRoot = y.Path.Split('.')[0];

        int rootCompare = string.Compare(xRoot, yRoot);
        if (rootCompare != 0)
        {
            return rootCompare;
        }

        // if we are in the same root, we need to compare field orders
        int xOrder = x.cgFieldOrder();
        int yOrder = y.cgFieldOrder();

        return xOrder.CompareTo(yOrder);
    }
}
