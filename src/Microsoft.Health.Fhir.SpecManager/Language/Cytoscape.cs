// <copyright file="Cytoscape.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Health.Fhir.SpecManager.Manager;
using Microsoft.Health.Fhir.SpecManager.Models;

namespace Microsoft.Health.Fhir.SpecManager.Language
{
    /// <summary>Export to Cytoscape data format.</summary>
    public sealed class Cytoscape : ILanguage
    {
        /// <summary>The weight increment.</summary>
        private const decimal _weightIncrement = 0.01M;

        /// <summary>The maximum edge weight.</summary>
        private static decimal _maxEdgeWeight = _weightIncrement;

        /// <summary>The maximum node weight.</summary>
        private static decimal _maxNodeWeight = _weightIncrement;

        /// <summary>FHIR information we are exporting.</summary>
        private FhirVersionInfo _info;

        /// <summary>Options for controlling the export.</summary>
        private ExporterOptions _options;

        /// <summary>Name of the language.</summary>
        private const string _languageName = "Cytoscape";

        /// <summary>The single file export extension.</summary>
        private const string _singleFileExportExtension = ".json";

        /// <summary>The node group map.</summary>
        private static Dictionary<FhirNodeInfo.FhirNodeType, string> _nodeGroupMap = new Dictionary<FhirNodeInfo.FhirNodeType, string>()
        {
            { FhirNodeInfo.FhirNodeType.Primitive, "primitive" },
            { FhirNodeInfo.FhirNodeType.DataType, "dataType" },
            { FhirNodeInfo.FhirNodeType.Resource, "resource" },
            { FhirNodeInfo.FhirNodeType.Component, "component" },
            { FhirNodeInfo.FhirNodeType.Profile, "profile" },
            { FhirNodeInfo.FhirNodeType.Unknown, "element" },       // non-nodes are elements
        };

        /// <summary>Dictionary mapping FHIR primitive types to language equivalents.</summary>
        private static readonly Dictionary<string, string> _primitiveTypeMap = new Dictionary<string, string>();

        /// <summary>Gets the reserved words.</summary>
        /// <value>The reserved words.</value>
        private static readonly HashSet<string> _reservedWords = new HashSet<string>();

        /// <summary>Gets the name of the language.</summary>
        /// <value>The name of the language.</value>
        string ILanguage.LanguageName => _languageName;

        string ILanguage.Namespace
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the single file extension for this language - null or empty indicates a multi-file
        /// export (exporter should copy the contents of the directory).
        /// </summary>
        string ILanguage.SingleFileExportExtension => _singleFileExportExtension;

        /// <summary>Gets the FHIR primitive type map.</summary>
        /// <value>The FHIR primitive type map.</value>
        Dictionary<string, string> ILanguage.FhirPrimitiveTypeMap => _primitiveTypeMap;

        /// <summary>Gets the reserved words.</summary>
        /// <value>The reserved words.</value>
        HashSet<string> ILanguage.ReservedWords => _reservedWords;

        /// <summary>
        /// Gets a list of FHIR class types that the language WILL export, regardless of user choices.
        /// Used to provide information to users.
        /// </summary>
        List<ExporterOptions.FhirExportClassType> ILanguage.RequiredExportClassTypes => new List<ExporterOptions.FhirExportClassType>();

        /// <summary>
        /// Gets a list of FHIR class types that the language CAN export, depending on user choices.
        /// </summary>
        List<ExporterOptions.FhirExportClassType> ILanguage.OptionalExportClassTypes => new List<ExporterOptions.FhirExportClassType>()
        {
            ExporterOptions.FhirExportClassType.PrimitiveType,
            ExporterOptions.FhirExportClassType.ComplexType,
            ExporterOptions.FhirExportClassType.Resource,
            ExporterOptions.FhirExportClassType.Interaction,
            ExporterOptions.FhirExportClassType.Enum,
            ExporterOptions.FhirExportClassType.Profile,
        };

        /// <summary>Gets language-specific options and their descriptions.</summary>
        Dictionary<string, string> ILanguage.LanguageOptions => new Dictionary<string, string>();


        void ILanguage.Export(
            FhirVersionInfo info,
            FhirComplex complex,
            Stream outputStream)
            => throw new NotImplementedException();

        /// <summary>Export the passed FHIR version into the specified directory.</summary>
        /// <param name="info">           The information.</param>
        /// <param name="serverInfo">     Information describing the server.</param>
        /// <param name="options">        Options for controlling the operation.</param>
        /// <param name="exportDirectory">Directory to write files.</param>
        void ILanguage.Export(
            FhirVersionInfo info,
            FhirCapabiltyStatement serverInfo,
            ExporterOptions options,
            string exportDirectory)
        {
            // set internal vars so we don't pass them to every function
            // this is ugly, but the interface patterns get bad quickly because we need the type map to copy the FHIR info
            _info = info;
            _options = options;

            Dictionary<string, CytoElement> elementDict = BuildNodes();

            // create a filename for writing
            string filename = Path.Combine(exportDirectory, $"{_languageName}_{info.FhirSequence}.json");

            using (FileStream stream = new FileStream(filename, FileMode.Create))
            {
                JsonSerializer.Serialize(stream, elementDict.Values, typeof(IEnumerable<CytoElement>));
            }
        }

        /// <summary>Builds the nodes.</summary>
        /// <returns>A Dictionary&lt;string,CytoElement&gt;</returns>
        private Dictionary<string, CytoElement> BuildNodes()
        {
            Dictionary<string, CytoElement> elements = new Dictionary<string, CytoElement>();

            foreach (FhirComplex dataType in _info.ComplexTypes.Values)
            {
                AddComplex(elements, dataType, FhirNodeInfo.FhirNodeType.DataType);
            }

            foreach (FhirComplex resource in _info.Resources.Values)
            {
                AddComplex(elements, resource, FhirNodeInfo.FhirNodeType.Resource);
            }

            Console.WriteLine($"Max Node Weight: {_maxNodeWeight}");
            Console.WriteLine($"Max Edge Weight: {_maxEdgeWeight}");

            return elements;
        }

        /// <summary>Adds a primitive node to 'primitive'.</summary>
        /// <param name="elements"> The elements.</param>
        /// <param name="primitive">The primitive.</param>
        private void AddPrimitive(
            Dictionary<string, CytoElement> elements,
            FhirPrimitive primitive)
        {
            if (elements.ContainsKey(primitive.Name))
            {
                elements[primitive.Name].Data.Weight += _weightIncrement;

                if (elements[primitive.Name].Data.Weight > _maxNodeWeight)
                {
                    _maxNodeWeight = (decimal)elements[primitive.Name].Data.Weight;
                }

                return;
            }

            CytoElement cy = new CytoElement()
            {
                Group = CytoElement.GroupNodes,
                Data = new CytoElementDataNode()
                {
                    Id = primitive.Name,
                    Name = primitive.Name,
                    Weight = _weightIncrement,
                    Group = _nodeGroupMap[FhirNodeInfo.FhirNodeType.Primitive],
                },
            };

            elements.Add(primitive.Name, cy);

            TryRecurse(
                elements,
                primitive.Name,
                FhirNodeInfo.FhirNodeType.Primitive,
                primitive.BaseTypeName);
        }

        /// <summary>Adds a node.</summary>
        /// <param name="elements">    The elements.</param>
        /// <param name="sourceId">    Identifier for the source.</param>
        /// <param name="baseTypeName">Name of the base type.</param>
        private void TryRecurse(
            Dictionary<string, CytoElement> elements,
            string sourceId,
            FhirNodeInfo.FhirNodeType sourceType,
            string baseTypeName)
        {
            if (string.IsNullOrEmpty(baseTypeName) ||
                (baseTypeName == sourceId))
            {
                return;
            }

            if (_info.TryGetNodeInfo(baseTypeName, out FhirNodeInfo node))
            {
                switch (node.SourceType)
                {
                    case FhirNodeInfo.FhirNodeType.Primitive:
                        AddPrimitive(elements, node.GetSource<FhirPrimitive>());
                        AddEdge(
                            elements,
                            sourceId,
                            sourceType,
                            baseTypeName,
                            node.SourceType);
                        return;

                    case FhirNodeInfo.FhirNodeType.DataType:
                    case FhirNodeInfo.FhirNodeType.Resource:
                    case FhirNodeInfo.FhirNodeType.Component:
                    case FhirNodeInfo.FhirNodeType.Profile:
                        AddComplex(elements, node.GetSource<FhirComplex>(), sourceType);
                        AddEdge(
                            elements,
                            sourceId,
                            sourceType,
                            baseTypeName,
                            node.SourceType);
                        return;

                    case FhirNodeInfo.FhirNodeType.Unknown:
                    case FhirNodeInfo.FhirNodeType.Self:
                    default:
                        break;
                }
            }
        }

        /// <summary>Adds an edge.</summary>
        /// <param name="elements">  The elements.</param>
        /// <param name="source">    Source for the.</param>
        /// <param name="sourceType">Type of the source.</param>
        /// <param name="target">    Target for the.</param>
        /// <param name="targetType">Type of the target.</param>
        private static void AddEdge(
            Dictionary<string, CytoElement> elements,
            string source,
            FhirNodeInfo.FhirNodeType sourceType,
            string target,
            FhirNodeInfo.FhirNodeType targetType)
        {
            if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(target))
            {
                return;
            }

            if (source == target)
            {
                return;
            }

            string id = $"{source}-{target}";

            if (elements.ContainsKey(id))
            {
                elements[id].Data.Weight += _weightIncrement;

                if (elements[id].Data.Weight > _maxEdgeWeight)
                {
                    _maxEdgeWeight = (decimal)elements[id].Data.Weight;
                }

                return;
            }

            CytoElement cy = new CytoElement()
            {
                Group = CytoElement.GroupEdges,
                Data = new CytoElementDataEdge()
                {
                    Id = id,
                    SourceId = source,
                    TargetId = target,
                    Weight = _weightIncrement,
                    Group = _nodeGroupMap[targetType],
                },
            };

            elements.Add(id, cy);
        }

        /// <summary>Adds a complex to 'complex'.</summary>
        /// <param name="elements">   The elements.</param>
        /// <param name="complex">    The complex.</param>
        /// <param name="complexType">Type of the complex.</param>
        private void AddComplex(
            Dictionary<string, CytoElement> elements,
            FhirComplex complex,
            FhirNodeInfo.FhirNodeType complexType)
        {
            if (elements.ContainsKey(complex.Name))
            {
                elements[complex.Name].Data.Weight += _weightIncrement;

                if (elements[complex.Name].Data.Weight > _maxNodeWeight)
                {
                    _maxNodeWeight = (decimal)elements[complex.Name].Data.Weight;
                }

                return;
            }

            CytoElement cy = new CytoElement()
            {
                Group = CytoElement.GroupNodes,
                Data = new CytoElementDataNode()
                {
                    Id = complex.Name,
                    Name = complex.Name,
                    Weight = _weightIncrement,
                    Group = _nodeGroupMap[complexType],
                },
            };

            elements.Add(complex.Name, cy);

            TryRecurse(
                elements,
                complex.Name,
                complexType,
                complex.BaseTypeName);

            AddElementEdges(elements, complex, complex);

            if (complex.Components != null)
            {
                foreach (FhirComplex component in complex.Components.Values)
                {
                    AddElementEdges(elements, complex, component);
                }
            }
        }

        /// <summary>Adds an element edges.</summary>
        /// <param name="elements">The elements.</param>
        /// <param name="root">    The root.</param>
        /// <param name="current"> The current.</param>
        private void AddElementEdges(
            Dictionary<string, CytoElement> elements,
            FhirComplex root,
            FhirComplex current)
        {
            if (current.Elements != null)
            {
                foreach (FhirElement element in current.Elements.Values)
                {
                    if (!string.IsNullOrEmpty(element.BaseTypeName))
                    {
                        if (!_info.TryGetNodeInfo(element.BaseTypeName, out FhirNodeInfo node))
                        {
                            continue;
                        }

                        if ((node.SourceType == FhirNodeInfo.FhirNodeType.Component) ||
                            (node.SourceType == FhirNodeInfo.FhirNodeType.Primitive))
                        {
                            continue;
                        }

                        AddEdge(
                            elements,
                            root.Name,
                            FhirNodeInfo.FhirNodeType.Unknown,
                            element.BaseTypeName,
                            FhirNodeInfo.FhirNodeType.Unknown);

                        continue;
                    }

                    if (element.ElementTypes != null)
                    {
                        foreach (FhirElementType elementType in element.ElementTypes.Values)
                        {
                            if (!_info.TryGetNodeInfo(elementType.Name, out FhirNodeInfo node))
                            {
                                continue;
                            }

                            if ((node.SourceType == FhirNodeInfo.FhirNodeType.Component) ||
                                (node.SourceType == FhirNodeInfo.FhirNodeType.Primitive))
                            {
                                continue;
                            }

                            if ((elementType.Profiles != null) &&
                                (elementType.Profiles.Count > 0))
                            {
                                foreach (FhirElementProfile profile in elementType.Profiles.Values)
                                {
                                    AddEdge(
                                        elements,
                                        root.Name,
                                        FhirNodeInfo.FhirNodeType.Unknown,
                                        profile.Name,
                                        FhirNodeInfo.FhirNodeType.Unknown);
                                }

                                continue;
                            }

                            AddEdge(
                                elements,
                                root.Name,
                                FhirNodeInfo.FhirNodeType.Unknown,
                                elementType.Name,
                                FhirNodeInfo.FhirNodeType.Unknown);

                            continue;
                        }

                        continue;
                    }
                }
            }

            if (current.Components != null)
            {
                foreach (FhirComplex complex in current.Components.Values)
                {
                    AddElementEdges(elements, root, complex);
                }
            }
        }

        /// <summary>A cyto element.</summary>
        private class CytoElement
        {
            public const string GroupNodes = "nodes";
            public const string GroupEdges = "edges";

            /// <summary>Gets or sets the group.</summary>
            [JsonPropertyName("group")]
            public string Group { get; set; }

            /// <summary>Gets or sets the data.</summary>
            [JsonPropertyName("data")]
            public CytoElementData Data { get; set; }
        }

        /// <summary>A cyto element data.</summary>
        private class CytoElementData
        {
            /// <summary>Gets or sets the identifier.</summary>
            [JsonPropertyName("id")]
            public string Id { get; set; }

            /// <summary>Gets or sets the name.</summary>
            [JsonPropertyName("name"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            public string Name { get; set; }

            /// <summary>Gets or sets the group.</summary>
            [JsonPropertyName("group"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            public string Group { get; set; }

            /// <summary>Gets or sets the weight.</summary>
            [JsonPropertyName("weight"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            public decimal? Weight { get; set; }
        }

        /// <summary>A cyto element data.</summary>
        private class CytoElementDataNode : CytoElementData
        {
            /// <summary>Gets or sets the parent for a compound node.</summary>
            [JsonPropertyName("parent"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            public string ParentId { get; set; }
        }

        /// <summary>A cyto element data.</summary>
        private class CytoElementDataEdge : CytoElementData
        {
            /// <summary>Gets or sets the source node id for an edge.</summary>
            [JsonPropertyName("source")]
            public string SourceId { get; set; }

            /// <summary>Gets or sets the target node id for an edge.</summary>
            [JsonPropertyName("target")]
            public string TargetId { get; set; }
        }
    }
}
