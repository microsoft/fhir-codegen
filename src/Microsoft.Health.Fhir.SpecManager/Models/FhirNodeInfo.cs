// <copyright file="FhirNodeInfo.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Health.Fhir.SpecManager.Models
{
    /// <summary>A FHIR node information class.</summary>
    public class FhirNodeInfo
    {
        private FhirPrimitive _sourcePrimitive;
        private FhirComplex _sourceComplex;

        private FhirPrimitive _destinationPrimitive;
        private FhirComplex _destinationComplex;

        /// <summary>Initializes a new instance of the <see cref="FhirNodeInfo"/> class.</summary>
        /// <param name="sourceType">     Type of the source.</param>
        /// <param name="sourceNode">     Source node.</param>
        /// <param name="destinationType">The type of the edge.</param>
        /// <param name="destinationNode">Destination for the edge.</param>
        public FhirNodeInfo(
            FhirNodeType sourceType,
            object sourceNode)
        {
            if (sourceNode != null)
            {
                switch (sourceType)
                {
                    case FhirNodeType.Primitive:
                        _sourcePrimitive = (FhirPrimitive)sourceNode;
                        _sourceComplex = null;
                        break;

                    case FhirNodeType.DataType:
                    case FhirNodeType.Resource:
                    case FhirNodeType.Component:
                    case FhirNodeType.Profile:
                        _sourcePrimitive = null;
                        _sourceComplex = (FhirComplex)sourceNode;
                        break;

                    case FhirNodeType.Unknown:
                    case FhirNodeType.Self:
                    default:
                        throw new ArgumentException($"Invalid source node type: {sourceType}");
                }
            }

            SourceType = sourceType;
            DestinationType = FhirNodeType.Unknown;
            _destinationPrimitive = null;
            _destinationComplex = null;
        }

        /// <summary>Initializes a new instance of the <see cref="FhirNodeInfo"/> class.</summary>
        /// <param name="sourceType">     Type of the source.</param>
        /// <param name="sourceNode">     Source node.</param>
        /// <param name="destinationType">The type of the edge.</param>
        /// <param name="destinationNode">Destination for the edge.</param>
        public FhirNodeInfo(
            FhirNodeType sourceType,
            object sourceNode,
            FhirNodeType destinationType,
            object destinationNode)
        {
            SourceType = sourceType;

            if (sourceNode != null)
            {
                switch (sourceType)
                {
                    case FhirNodeType.Primitive:
                        _sourcePrimitive = (FhirPrimitive)sourceNode;
                        _sourceComplex = null;
                        break;

                    case FhirNodeType.DataType:
                    case FhirNodeType.Resource:
                    case FhirNodeType.Component:
                    case FhirNodeType.Profile:
                        _sourcePrimitive = null;
                        _sourceComplex = (FhirComplex)sourceNode;
                        break;

                    case FhirNodeType.Unknown:
                    case FhirNodeType.Self:
                    default:
                        throw new ArgumentException($"Invalid source node type: {sourceType}");
                }
            }

            DestinationType = destinationType;

            if (destinationNode != null)
            {
                switch (destinationType)
                {
                    case FhirNodeType.Primitive:
                        _destinationPrimitive = (FhirPrimitive)destinationNode;
                        _destinationComplex = null;
                        break;

                    case FhirNodeType.DataType:
                    case FhirNodeType.Resource:
                    case FhirNodeType.Component:
                    case FhirNodeType.Profile:
                        _destinationPrimitive = null;
                        _destinationComplex = (FhirComplex)destinationNode;
                        break;

                    case FhirNodeType.Unknown:
                    case FhirNodeType.Self:
                    default:
                        _destinationPrimitive = null;
                        _destinationComplex = null;
                        break;
                }
            }
        }

        /// <summary>Values that represent node and link types.</summary>
        public enum FhirNodeType
        {
            /// <summary>Could not determine edge linking type.</summary>
            Unknown,

            /// <summary>This node links to itself (will return null pointer for simplicity).</summary>
            Self,

            /// <summary>This node is a primitive data type.</summary>
            Primitive,

            /// <summary>This node is a non-primitive data type.</summary>
            DataType,

            /// <summary>This node is a resource.</summary>
            Resource,

            /// <summary>This node is a component definition (BackboneElement).</summary>
            Component,

            /// <summary>This node is a profile data type.</summary>
            Profile,
        }

        /// <summary>Gets the type of the source node.</summary>
        public FhirNodeType SourceType { get; }

        /// <summary>Gets the type of the edge.</summary>
        public FhirNodeType DestinationType { get; }

        /// <summary>Follows the edge to it's type node.</summary>
        /// <returns>An object.</returns>
        public object GetSource()
        {
            if (_sourcePrimitive != null)
            {
                return _sourcePrimitive;
            }

            if (_sourceComplex != null)
            {
                return _sourceComplex;
            }

            return null;
        }

        /// <summary>Grabs the edge source node.</summary>
        /// <typeparam name="T">Type of node expected to return.</typeparam>
        /// <returns>An object.</returns>
        public T GetSource<T>()
            where T : FhirTypeBase
        {
            if ((_sourcePrimitive != null) &&
                (typeof(T) == typeof(FhirPrimitive)))
            {
                return _sourcePrimitive as T;
            }

            if ((_sourceComplex != null) &&
                (typeof(T) == typeof(FhirComplex)))
            {
                return _sourceComplex as T;
            }

            return null;
        }

        /// <summary>Follows the edge to it's type node.</summary>
        /// <returns>An object.</returns>
        public object GetDestination()
        {
            if (_destinationPrimitive != null)
            {
                return _destinationPrimitive;
            }

            if (_destinationComplex != null)
            {
                return _destinationComplex;
            }

            return null;
        }

        /// <summary>Follows the edge to it's type node.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <returns>An object.</returns>
        public T GetDestination<T>()
            where T : FhirTypeBase
        {
            if ((_destinationPrimitive != null) &&
                (typeof(T) == typeof(FhirPrimitive)))
            {
                return _destinationPrimitive as T;
            }

            if ((_destinationComplex != null) &&
                (typeof(T) == typeof(FhirComplex)))
            {
                return _destinationComplex as T;
            }

            return null;
        }
    }
}
