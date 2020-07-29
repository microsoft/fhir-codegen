// <copyright file="FhirTypeEdge.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Health.Fhir.SpecManager.Models
{
    /// <summary>A FHIR type edge.</summary>
    public class FhirTypeEdge
    {
        private FhirPrimitive _primitive;
        private FhirComplex _complex;

        /// <summary>Initializes a new instance of the <see cref="FhirTypeEdge"/> class.</summary>
        /// <param name="edgeType">   The type of the edge.</param>
        /// <param name="destination">Destination for the edge.</param>
        public FhirTypeEdge(
            DestinationNodeType edgeType,
            object destination)
        {
            EdgeType = edgeType;

            if (destination != null)
            {
                switch (edgeType)
                {
                    case DestinationNodeType.Primitive:
                        _primitive = (FhirPrimitive)destination;
                        _complex = null;
                        break;

                    case DestinationNodeType.DataType:
                    case DestinationNodeType.Resource:
                    case DestinationNodeType.Component:
                        _primitive = null;
                        _complex = (FhirComplex)destination;
                        break;

                    case DestinationNodeType.Unknown:
                    case DestinationNodeType.Self:
                    default:
                        _primitive = null;
                        _complex = null;
                        break;
                }
            }
        }

        /// <summary>Values that represent destination node types.</summary>
        public enum DestinationNodeType
        {
            /// <summary>Could not determine edge linking type.</summary>
            Unknown,

            /// <summary>This edge links to itself (will return null pointer for simplicity).</summary>
            Self,

            /// <summary>This edge links to a primitive data type.</summary>
            Primitive,

            /// <summary>This edge links to a non-primitive data type.</summary>
            DataType,

            /// <summary>This edge links to a resource.</summary>
            Resource,

            /// <summary>This edge links to a component definition (BackboneElement).</summary>
            Component,
        }

        /// <summary>Gets the type of the edge.</summary>
        public DestinationNodeType EdgeType { get; }

        /// <summary>Follows the edge to it's type node.</summary>
        /// <returns>An object.</returns>
        public object GetNode()
        {
            if (_primitive != null)
            {
                return _primitive;
            }

            if (_complex != null)
            {
                return _complex;
            }

            return null;
        }

        /// <summary>Follows the edge to it's type node.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <returns>An object.</returns>
        public T GetNode<T>()
            where T : FhirTypeBase
        {
            if ((_primitive != null) &&
                (typeof(T) == typeof(FhirPrimitive)))
            {
                return _primitive as T;
            }

            if ((_complex != null) &&
                (typeof(T) == typeof(FhirComplex)))
            {
                return _complex as T;
            }

            return null;
        }
    }
}
