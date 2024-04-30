// <copyright file="PackageComparer.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml;
using Hl7.Fhir.Model;
using Microsoft.Health.Fhir.CodeGen.Configuration;
using Microsoft.Health.Fhir.CodeGen.Language;
using Microsoft.Health.Fhir.CodeGen.Models;
using Microsoft.Health.Fhir.CodeGenCommon.Extensions;
using static Hl7.Fhir.Model.VerificationResult;

namespace Microsoft.Health.Fhir.CodeGen.CompareTool;

public class PackageComparer
{
    DefinitionCollection _left;
    DefinitionCollection _right;

    ConfigCompare _config;

    HttpClient? _httpClient = null;
    Uri? _ollamaUri = null;

    public record class PackagePathRenames
    {
        public required string PackageDirectiveLeft { get; init; }
        public required string PackageDirectiveRight { get; init; }
        public required Dictionary<string, string> LeftRightPath { get; init; }
    }

    public record class ValueSetInfoRec
    {
        public required string Url { get; init; }
        public required string Name { get; init; }
    }

    public record class ElementTypeInfoRec
    {
        public required string Name { get; init; }
        public required List<string> Profiles { get; init; }
        public required List<string> TargetProfiles { get; init; }
    }

    public record class ElementInfoRec
    {
        public required string Name { get; init; }
        public required string Path { get; init; }
        public required string Short { get; init; }
        public required string Description { get; init; }

        public required int MinCardinality { get; init; }
        public required int MaxCardinality { get; init; }

        public required string BindingStrength { get; init; }

        public required string BindingValueSet { get; init; }

        public required Dictionary<string, ElementTypeInfoRec> ElementTypes { get; init; }
    }

    public record class StructureInfoRec
    {
        public required string Name { get; init; }
        public required string Title { get; init; }
        public required string Description { get; init; }
        public required string Purpose { get; init; }

        public required int SnapshotCount { get; init; }

        public required int DifferentialCount { get; init; }
    }

    public record class PackageComparison
    {
        public required string LeftPackageId { get; init; }
        public required string LeftPackageVersion { get; init; }
        public required string RightPackageId { get; init; }
        public required string RightPackageVersion { get; init; }

        public required Dictionary<string, ComparisonRecord<StructureInfoRec>> PrimitiveTypes { get; init; }
        public required Dictionary<string, ComparisonRecord<StructureInfoRec>> ComplexTypes { get; init; }
        public required Dictionary<string, ComparisonRecord<StructureInfoRec>> Resources { get; init; }
        public required Dictionary<string, ComparisonRecord<StructureInfoRec>> LogicalModels { get; init; }
    }

    public class ComparisonRecord<T>
    {
        public required T? Left { get; init; }
        public required T? Right { get; init; }

        public required bool NamedMatch { get; init; }

        public T? AiPrediction { get; init; }
        public int AiConfidence { get; init; } = 0;
    }

    public PackageComparer(ConfigCompare config, DefinitionCollection left, DefinitionCollection right)
    {
        _config = config;
        _left = left;
        _right = right;

        if (!string.IsNullOrEmpty(config.OllamaUrl) &&
            !string.IsNullOrEmpty(config.OllamaModel))
        {
            _httpClient = new HttpClient();
            _ollamaUri = config.OllamaUrl.EndsWith("generate", StringComparison.OrdinalIgnoreCase)
                ? new Uri(config.OllamaUrl)
                : new Uri(new Uri(config.OllamaUrl), "api/generate");
        }
    }

    public void Compare()
    {
        Console.WriteLine(
            $"Comparing {_left.MainPackageId}#{_left.MainPackageVersion}" +
            $" and {_right.MainPackageId}#{_right.MainPackageVersion}");

        // build our filename
        string mdFilename = _left.MainPackageId.ToPascalCase() + "_" + SanitizeVersion(_left.MainPackageVersion) + "_" +
                            _right.MainPackageId.ToPascalCase() + "_" + SanitizeVersion(_right.MainPackageVersion) + ".md";

        string mdFullFilename = Path.Combine(_config.OutputDirectory, mdFilename);

        using ExportStreamWriter mdWriter = new(mdFullFilename);

        mdWriter.WriteLine($"Comparison of {_left.MainPackageId}#{_left.MainPackageVersion} and {_right.MainPackageId}#{_right.MainPackageVersion}");
        mdWriter.WriteLine($"Generated at {DateTime.Now.ToString("F")}");
        mdWriter.WriteLine();
        mdWriter.WriteLine();

        Dictionary<string, ComparisonRecord<StructureInfoRec>> primitives = ComparePackageStructures(_left.PrimitiveTypesByName, _right.PrimitiveTypesByName);
        WriteStructureComparison(mdWriter, "Primitive Types", primitives);

        Dictionary<string, ComparisonRecord<StructureInfoRec>> complexTypes = ComparePackageStructures(_left.ComplexTypesByName, _right.ComplexTypesByName);
        WriteStructureComparison(mdWriter, "Complex Types", complexTypes);

        Dictionary<string, ComparisonRecord<StructureInfoRec>> resources = ComparePackageStructures(_left.ResourcesByName, _right.ResourcesByName);
        WriteStructureComparison(mdWriter, "Resources", resources);

        Dictionary<string, ComparisonRecord<StructureInfoRec>> logical = ComparePackageStructures(_left.LogicalModelsByName, _right.LogicalModelsByName);
        WriteStructureComparison(mdWriter, "Logical Models", logical);

        PackageComparison packageComparison = new()
        {
            LeftPackageId = _left.MainPackageId,
            LeftPackageVersion = _left.MainPackageVersion,
            RightPackageId = _right.MainPackageId,
            RightPackageVersion = _right.MainPackageVersion,
            PrimitiveTypes = primitives,
            ComplexTypes = complexTypes,
            Resources = resources,
            LogicalModels = logical,
        };

        mdWriter.Flush();
        mdWriter.Close();
        mdWriter.Dispose();
    }

    private void WriteStructureConceptMap(PackageComparison pc)
    {

    }

    private void WriteElementConceptMap(PackageComparison pc)
    {

    }

    private string SanitizeForTable(string value) => value.Replace("|", "\\|").Replace('\n', ' ').Replace('\r', ' ');

    private void WriteStructureComparison(ExportStreamWriter writer, string header, Dictionary<string, ComparisonRecord<StructureInfoRec>> comparisonDict)
    {
        writer.WriteLine("## " + header);
        writer.WriteLine("| Key | Name | Description | In Snap | In Diff | Status | Name | Description | In Snap | In Diff |");
        writer.WriteLine("| --- | ---- | ----------- | ------- | ------- | ------ | ---- | ----------- | ------- | ------- |");

        foreach ((string key, ComparisonRecord<StructureInfoRec> c) in comparisonDict.OrderBy(kvp => kvp.Key))
        {
            writer.WriteLine(
                $"{key} |" +
                $" {c.Left?.Name ?? "-"} | {SanitizeForTable(c.Left?.Description ?? "-")} | {c.Left?.SnapshotCount.ToString() ?? "-"} | {c.Left?.DifferentialCount.ToString() ?? "-"} |" +
                $" {GetStatusString(c)} |" +
                $" {c.Right?.Name ?? "-"} | {SanitizeForTable(c.Right?.Description ?? "-")} | {c.Right?.SnapshotCount.ToString() ?? "-"} | {c.Right?.DifferentialCount.ToString() ?? "-"} ");
        }

        writer.WriteLine();
        writer.WriteLine();
        writer.WriteLine();

        //writer.WriteLine("# " + header);
        //writer.WriteLine("| Key | Name | Title | Description | Status | Name | Title | Description | AiName | AiTitle | AIDesc |");
        //writer.WriteLine("| --- | ---- | ----- | ----------- | ------ | ---- | ----- | ----------- | ------ | ------- | ------ |");

        //foreach ((string key, ComparisonRecord<StructureInfoRec> c) in comparisonDict.OrderBy(kvp => kvp.Key))
        //{
        //    writer.WriteLine(
        //        $"{key} |" +
        //        $" {c.Left?.Name ?? "-"} | {c.Left?.Title ?? "-"} | {c.Left?.Description ?? "-"} |" +
        //        $" {(c.Match ? "Match" : "-")} |" +
        //        $" {c.Right?.Name ?? "-"} | {c.Right?.Title ?? "-"} | {c.Right?.Description ?? "-"} |" +
        //        $" {c.AiPrediction?.Name} | {c.AiPrediction?.Title} | {c.AiPrediction?.Description} |");
        //}

    }

    private string GetStatusString(ComparisonRecord<StructureInfoRec> c)
    {
        if (c.NamedMatch)
        {
            return "NamedMatch";
        }

        if (c.Left == null)
        {
            return "Added";
        }

        if (c.Right == null)
        {
            return "Removed";
        }

        return "Modified";
    }

    private void LoadKnownChanges()
    {
        // TODO(ginoc): implement
    }

    private Dictionary<string, ComparisonRecord<StructureInfoRec>> ComparePackageStructures(
        IReadOnlyDictionary<string, StructureDefinition> leftStructures,
        IReadOnlyDictionary<string, StructureDefinition> rightStructures)
    {
        Dictionary<string, StructureInfoRec> left = leftStructures.ToDictionary(kvp => kvp.Key, kvp => GetInfo(kvp.Value));
        Dictionary<string, StructureInfoRec> right = rightStructures.ToDictionary(kvp => kvp.Key, kvp => GetInfo(kvp.Value));

        Dictionary<string, ComparisonRecord<StructureInfoRec>> comparison = [];

        HashSet<string> keysLeft = left.Keys.ToHashSet() ?? [];
        HashSet<string> keysRight = right.Keys.ToHashSet() ?? [];

        HashSet<string> keyIntersection = left.Keys.ToHashSet() ?? [];
        keyIntersection.IntersectWith(keysRight);

        keysLeft.ExceptWith(keyIntersection);
        keysRight.ExceptWith(keyIntersection);

        // add our matches
        foreach (string key in keyIntersection)
        {
            comparison.Add(key, new()
            {
                Left = left[key],
                Right = right[key],
                NamedMatch = true,
            });
        }

        // traverse keys in left that do not exist in right
        IEnumerable<StructureInfoRec> unusedRight = right.Where(kvp => keysRight.Contains(kvp.Key)).Select(kvp => kvp.Value);

        foreach (string key in keysLeft)
        {
            //if (TryAskOllama(left[key], unusedRight, out StructureInfoRec? guess, out _))
            //{
            //    comparison.Add(key, new()
            //    {
            //        Left = left[key],
            //        Right = null,
            //        Match = false,
            //        AiPrediction = guess,
            //    });

            //    continue;
            //}

            comparison.Add(key, new()
            {
                Left = left[key],
                Right = null,
                NamedMatch = false,
            });
        }


        IEnumerable<StructureInfoRec> unusedLeft = left.Where(kvp => keysRight.Contains(kvp.Key)).Select(kvp => kvp.Value);

        foreach (string key in keysRight)
        {
            //if (TryAskOllama(right[key], unusedLeft, out StructureInfoRec? guess, out _))
            //{
            //    comparison.Add(key, new()
            //    {
            //        Left = null,
            //        Right = right[key],
            //        Match = false,
            //        AiPrediction = guess,
            //    });

            //    continue;
            //}

            comparison.Add(key, new()
            {
                Left = null,
                Right = right[key],
                NamedMatch = false,
            });
        }

        return comparison;
    }

    private StructureInfoRec GetInfo(StructureDefinition sd)
    {
        return new StructureInfoRec()
        {
            Name = sd.Name,
            Title = sd.Title,
            Description = sd.Description,
            Purpose = sd.Purpose,
            SnapshotCount = sd.Snapshot?.Element.Count ?? 0,
            DifferentialCount = sd.Differential?.Element.Count ?? 0,
        };
    }

    private class OllamaQuery
    {
        /// <summary>Required: the model name.</summary>
        [JsonPropertyName("model")]
        public required string Model { get; set; }

        /// <summary>The prompt to generate a response for.</summary>
        [JsonPropertyName("prompt")]
        public required string Prompt { get; set; }

        /// <summary>Optional list of base64-encoded images (for multimodal models such as llava)</summary>
        [JsonPropertyName("images")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string[]? Images { get; set; } = null;

        /// <summary>The format to return a response in. Currently the only accepted value is json</summary>
        [JsonPropertyName("format")]
        public string Format { get; } = "json";

        /// <summary>Additional model parameters listed in the documentation for the Model file such as temperature</summary>
        [JsonPropertyName("options")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Options { get; set; } = null;

        /// <summary>System message to (overrides what is defined in the Model file).</summary>
        [JsonPropertyName("system")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? System { get; set; } = null;

        /// <summary>The prompt template to use (overrides what is defined in the Model file)</summary>
        [JsonPropertyName("template")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Template { get; set; } = null;

        /// <summary>The context parameter returned from a previous request to /generate, this can be used to keep a short conversational memory</summary>
        [JsonPropertyName("context")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int[]? Context { get; set; } = null;

        /// <summary>If false the response will be returned as a single response object, rather than a stream of objects.</summary>
        [JsonPropertyName("stream")]
        public required bool Stream { get; set; }

        /// <summary>If true no formatting will be applied to the prompt. You may choose to use the raw parameter if you are specifying a full templated prompt in your request to the API</summary>
        [JsonPropertyName("raw")]
        public bool Raw { get; set; } = false;

        /// <summary>Controls how long the model will stay loaded into memory following the request (default: 5m)</summary>
        [JsonPropertyName("keep_alive")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? KeepAliveDuration { get; set; } = null;
    }


    public class OllamaResponse
    {
        [JsonPropertyName("model")]
        public string Model { get; set; } = string.Empty;

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.MinValue;

        [JsonPropertyName("response")]
        public string Response { get; set; } = string.Empty;

        [JsonPropertyName("done")]
        public bool Done { get; set; } = false;

        [JsonPropertyName("context")]
        public int[] Context { get; set; } = [];

        [JsonPropertyName("total_duration")]
        public long TotalDuration { get; set; } = 0;

        [JsonPropertyName("load_duration")]
        public int LoadDuration { get; set; } = 0;

        [JsonPropertyName("prompt_eval_count")]
        public int PromptEvalCount { get; set; } = 0;

        [JsonPropertyName("prompt_eval_duration")]
        public int PromptEvalDuration { get; set; } = 0;

        [JsonPropertyName("eval_count")]
        public int EvalCount { get; set; } = 0;

        [JsonPropertyName("eval_duration")]
        public long EvalDuration { get; set; } = 0;
    }


    private bool TryAskOllama(
        StructureInfoRec known,
        IEnumerable<StructureInfoRec> possibleMatches,
        [NotNullWhen(true)] out StructureInfoRec? guess,
        [NotNullWhen(true)] out int? confidence)
    {
        if (_httpClient == null)
        {
            guess = null;
            confidence = 0;
            return false;
        }

        try
        {
            string system = """
                You are a modeling expert comparing definitions between model structures.
                When presented with a model definition and a set of possible matches, you are asked to select the most likely match.
                You are to respond with ONLY the json representation of the model that you believe is the best match.
                The model definition is a structure with the following properties: Name, Title, Description, and Purpose.
                """;

            //string prompt =
            //    $"Given the following definition:\n{System.Web.HttpUtility.JavaScriptStringEncode(JsonSerializer.Serialize(known))}\n" +
            //    $"Please select the most likely match from the following definitions:\n{System.Web.HttpUtility.JavaScriptStringEncode(JsonSerializer.Serialize(possibleMatches))}." +
            //    $" Respond in JSON with only the definition in the same format.";

            string prompt = $$$"""
                Given the following definition:
                {{{JsonSerializer.Serialize(known)}}}
                Please select the most likely match from the following definitions:
                {{{JsonSerializer.Serialize(possibleMatches)}}}
                """;

                //$"Given the following definition:\n{JsonSerializer.Serialize(known)}\n" +
                //$"Please select the most likely match from the following definitions:\n{JsonSerializer.Serialize(possibleMatches)}." +
                //$" Respond in JSON with only the definition in the same format.";

            // build our prompt
            OllamaQuery query = new()
            {
                Model = _config.OllamaModel,
                System = system,
                Prompt = prompt,
                Stream = false,
            };

            Console.WriteLine("--------------");
            Console.WriteLine($"query:\n{JsonSerializer.Serialize(query)}");

            HttpRequestMessage request = new HttpRequestMessage()
            {
                Method = HttpMethod.Post,
                Content = new StringContent(JsonSerializer.Serialize(query), Encoding.UTF8, "application/json"),
                RequestUri = _ollamaUri,
                Headers =
                {
                    Accept =
                    {
                        new MediaTypeWithQualityHeaderValue("application/json"),
                    },
                },
            };

            HttpResponseMessage response = _httpClient.SendAsync(request).Result;
            System.Net.HttpStatusCode statusCode = response.StatusCode;

            if (statusCode != System.Net.HttpStatusCode.OK)
            {
                Console.WriteLine($"Request to {request.RequestUri} failed! Returned: {response.StatusCode}");
                guess = null;
                confidence = 0;
                return false;
            }

            string json = response.Content.ReadAsStringAsync().Result;
            if (string.IsNullOrEmpty(json))
            {
                Console.WriteLine($"Request to {request.RequestUri} returned empty body!");
                guess = null;
                confidence = 0;
                return false;
            }

            Console.WriteLine($"response:\n{json}");
            Console.WriteLine("--------------");


            OllamaResponse? olResponse = JsonSerializer.Deserialize<OllamaResponse>(json);

            if (olResponse == null )
            {
                Console.WriteLine($"Failed to deserialize response: {json}");
                guess = null;
                confidence = 0;
                return false;
            }

            if (string.IsNullOrEmpty(olResponse.Response))
            {
                Console.WriteLine($"Ollama response is empty: {json}");
                guess = null;
                confidence = 0;
                return false;
            }

            guess = JsonSerializer.Deserialize<StructureInfoRec>(olResponse.Response);
            if (guess == null)
            {
                Console.WriteLine($"Failed to deserialize response property: {olResponse.Response}");
                guess = null;
                confidence = 0;
                return false;
            }

            confidence = -1;
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"TryAskOllama <<< caught: {ex.Message}{(string.IsNullOrEmpty(ex.InnerException?.Message) ? string.Empty : ex.InnerException.Message)}");
        }

        guess = null;
        confidence = 0;
        return false;
    }

    private string SanitizeVersion(string version)
    {
        return version.Replace('.', '_').Replace('-', '_');
    }
}
