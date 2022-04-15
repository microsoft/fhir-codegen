// <copyright file="FhirTypeBase.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Microsoft.Health.Fhir.CodeGenCommon.Extensions;

namespace Microsoft.Health.Fhir.CodeGenCommon.Models;

/// <summary>
/// A base class for FHIR types to inherit from (common properties).
/// </summary>
public class FhirTypeBase
{
    private readonly string _name;
    private readonly string _nameCapitalized;
    private readonly string _path;
    private string _baseTypeName;

    /// <summary>Initializes a new instance of the <see cref="FhirTypeBase"/> class.</summary>
    /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
    /// <param name="id">              The id of this element/resource/datatype.</param>
    /// <param name="path">            The dot-notation path to this element/resource/datatype.</param>
    /// <param name="url">             The URL.</param>
    /// <param name="standardStatus">  The standard status.</param>
    /// <param name="isExperimental">  A value indicating whether this object is experimental.</param>
    /// <param name="shortDescription">The description.</param>
    /// <param name="purpose">         The purpose of this definition.</param>
    /// <param name="comment">         The comment.</param>
    /// <param name="validationRegEx"> The validation RegEx.</param>
    public FhirTypeBase(
        string id,
        string path,
        Uri url,
        string standardStatus,
        bool isExperimental,
        string shortDescription,
        string purpose,
        string comment,
        string validationRegEx)
    {
        // sanity checks
        if (string.IsNullOrEmpty(path))
        {
            throw new ArgumentNullException(nameof(path));
        }

        // set internal values
        Id = id;
        _path = path;
        StandardStatus = standardStatus;
        IsExperimental = isExperimental;
        ShortDescription = shortDescription;
        Purpose = purpose;
        _baseTypeName = string.Empty;

        if (string.IsNullOrEmpty(comment))
        {
            Comment = purpose;
        }
        else
        {
            Comment = comment;
        }

        ValidationRegEx = validationRegEx;
        URL = url;

        // check for components in the path
        string[] components = path.Split('.');
        _name = components[components.Length - 1];
        _nameCapitalized = ToPascal(_name);
    }

    /// <summary>Initializes a new instance of the <see cref="FhirTypeBase"/> class.</summary>
    /// <param name="id">              The id of this element/resource/datatype/extension.</param>
    /// <param name="path">            The dot-notation path to this element/resource/datatype/extension.</param>
    /// <param name="url">             The URL.</param>
    /// <param name="standardStatus">  The standard status.</param>
    /// <param name="isExperimental">  If this object is marked experimental.</param>
    /// <param name="shortDescription">The description.</param>
    /// <param name="purpose">         The purpose of this definition.</param>
    /// <param name="comment">         The comment.</param>
    /// <param name="validationRegEx"> The validation RegEx.</param>
    /// <param name="baseTypeName">    The name of the base type.</param>
    public FhirTypeBase(
        string id,
        string path,
        Uri url,
        string standardStatus,
        bool isExperimental,
        string shortDescription,
        string purpose,
        string comment,
        string validationRegEx,
        string baseTypeName)
        : this(
            id,
            path,
            url,
            standardStatus,
            isExperimental,
            shortDescription,
            purpose,
            comment,
            validationRegEx)
    {
        BaseTypeName = baseTypeName;
    }

    /// <summary>Values that represent naming conventions for item types.</summary>
    public enum NamingConvention
    {
        /// <summary>This feature is not supported / used.</summary>
        None,

        /// <summary>An enum constant representing the language controlled option.</summary>
        LanguageControlled,

        /// <summary>Names are standard FHIR dot notation (e.g., path).</summary>
        FhirDotNotation,

        /// <summary>Names are dot notation, with each first letter capitalized.</summary>
        PascalDotNotation,

        /// <summary>Names are Pascal Case (first letter capitalized).</summary>
        PascalCase,

        /// <summary>Names are Camel Case (first letter lower case).</summary>
        CamelCase,

        /// <summary>Names are all upper case.</summary>
        UpperCase,

        /// <summary>Names are all lower case.</summary>
        LowerCase,
    }

    /// <summary>Gets the Id for this element/resource/datatype.</summary>
    /// <value>The Id for this element/resource/datatype.</value>
    public string Id { get; }

    /// <summary>Gets the dot-notation path to this element/resource/datatype.</summary>
    /// <value>The dot-notation path to this element/resource/datatype.</value>
    public string Path => _path;

    /// <summary>
    /// Gets a natural language name identifying the structure definition. This name should be usable as an
    /// identifier for the module by machine processing applications such as code generation.
    /// </summary>
    /// <value>The name.</value>
    public string Name => _name;

    /// <summary>
    /// Gets name field with the first letter capitalized, useful in various languages and PascalCase joining.
    /// </summary>
    /// <value>The name capitalized.</value>
    public string NameCapitalized => _nameCapitalized;

    /// <summary>Gets URL of the document.</summary>
    /// <value>The URL.</value>
    public Uri URL { get; }

    /// <summary>
    /// Gets status of this type in the standards process
    /// see: http://hl7.org/fhir/valueset-standards-status.html.
    /// </summary>
    /// <value>The standard status.</value>
    public string StandardStatus { get; }

    /// <summary>Gets a value indicating whether this object is experimental.</summary>
    public bool IsExperimental { get; }

    /// <summary>Gets or sets the Name of the type this type inherits from (null if none).</summary>
    /// <value>The name of the base type.</value>
    public string BaseTypeName { get => _baseTypeName; set => _baseTypeName = value; }

    /// <summary>
    /// Gets a concise description of what this element means (e.g. for use in autogenerated summaries).
    /// </summary>
    /// <value>The description.</value>
    public string ShortDescription { get; }

    /// <summary>
    /// Gets a complete explanation of the meaning of the data element for human readability.  For
    /// the case of elements derived from existing elements (e.g. constraints), the definition SHALL be
    /// consistent with the base definition, but convey the meaning of the element in the particular
    /// context of use of the resource. (Note: The text you are reading is specified in
    /// ElementDefinition.definition).
    /// </summary>
    /// <value>The definition.</value>
    public string Purpose { get; }

    /// <summary>
    /// Gets explanatory notes and implementation guidance about the data element, including notes about how
    /// to use the data properly, exceptions to proper use, etc. (Note: The text you are reading is
    /// specified in ElementDefinition.comment).
    /// </summary>
    /// <value>The comment.</value>
    public string Comment { get; }

    /// <summary>
    /// Gets a RegEx string used to validate values of a type or property.
    /// </summary>
    /// <value>The validation RegEx.</value>
    public string ValidationRegEx { get; }

    /// <summary>Capitalizes a word.</summary>
    /// <param name="word">The word.</param>
    /// <returns>A capitalized version of the word.</returns>
    public static string ToPascal(string word)
    {
        if (string.IsNullOrEmpty(word))
        {
            return string.Empty;
        }

        return string.Concat(word.Substring(0, 1).ToUpperInvariant(), word.Substring(1));
    }

    /// <summary>Capitalizes a word.</summary>
    /// <param name="words">The words.</param>
    /// <returns>A capitalized version of the word.</returns>
    public static string[] ToPascal(string[] words)
    {
        string[] output = new string[words.Length];

        for (int i = 0; i < words.Length; i++)
        {
            output[i] = ToPascal(words[i]);
        }

        return output;
    }

    /// <summary>Converts a word to a camel case.</summary>
    /// <param name="word">The word.</param>
    /// <returns>Word as a string.</returns>
    public static string ToCamel(string word)
    {
        if (string.IsNullOrEmpty(word))
        {
            return string.Empty;
        }

        return string.Concat(word.Substring(0, 1).ToLowerInvariant(), word.Substring(1));
    }

    /// <summary>Converts a word to a camel case.</summary>
    /// <param name="words">The words.</param>
    /// <returns>Word as a string.</returns>
    public static string[] ToCamel(string[] words)
    {
        string[] output = new string[words.Length];

        for (int i = 0; i < words.Length; i++)
        {
            // first word is lower case
            if (i == 0)
            {
                output[i] = ToCamel(words[i]);
                continue;
            }

            // other words are capitalized
            output[i] = ToPascal(words[i]);
        }

        return output;
    }

    /// <summary>Converts the words to an upper invariant.</summary>
    /// <param name="words">The words.</param>
    /// <returns>Words as a string[].</returns>
    public static string[] ToUpperInvariant(string[] words)
    {
        string[] output = new string[words.Length];

        for (int i = 0; i < words.Length; i++)
        {
            output[i] = words[i].ToUpperInvariant();
        }

        return output;
    }

    /// <summary>Converts the words to a lower invariant.</summary>
    /// <param name="words">The words.</param>
    /// <returns>Words as a string[].</returns>
    public static string[] ToLowerInvariant(string[] words)
    {
        string[] output = new string[words.Length];

        for (int i = 0; i < words.Length; i++)
        {
            output[i] = words[i].ToUpperInvariant();
        }

        return output;
    }

     /// <summary>Type for export.</summary>
     /// <param name="convention">            The convention.</param>
     /// <param name="primitiveTypeMap">      The base type map.</param>
     /// <param name="concatenatePath">       (Optional) True to concatenate path.</param>
     /// <param name="concatenationDelimiter">(Optional) The concatenation delimiter.</param>
     /// <param name="reservedWords">         (Optional) The reserved words.</param>
     /// <returns>A string.</returns>
     public string TypeForExport(
        NamingConvention convention,
        Dictionary<string, string> primitiveTypeMap,
        bool concatenatePath = false,
        string concatenationDelimiter = "",
        HashSet<string> reservedWords = null)
    {
        if ((primitiveTypeMap != null) &&
            primitiveTypeMap.ContainsKey(_baseTypeName))
        {
            return primitiveTypeMap[_baseTypeName];
        }

        string baseType = _baseTypeName;

        // Resources cannot inherit patterns, but they are listed that way today
        // see https://chat.fhir.org/#narrow/stream/179177-conformance/topic/Inheritance.20and.20Cardinality.20Changes
        switch (_baseTypeName)
        {
            case "CanonicalResource":
            case "MetadataResource":
                baseType = "DomainResource";
                break;

            default:
                baseType = _baseTypeName;
                break;
        }

        string type = FhirUtils.ToConvention(
            baseType,
            _path,
            convention,
            concatenatePath,
            concatenationDelimiter,
            reservedWords);

        return type;
    }

    /// <summary>Converts this object to a requested naming convention.</summary>
    /// <exception cref="ArgumentException">Thrown when one or more arguments have unsupported or
    ///  illegal values.</exception>
    /// <param name="convention">            The convention.</param>
    /// <param name="concatenatePath">       (Optional) True to concatenate path.</param>
    /// <param name="concatenationDelimiter">(Optional) The concatenation delimiter.</param>
    /// <param name="reservedWords">         (Optional) The reserved words.</param>
    /// <returns>A string.</returns>
    public string NameForExport(
        NamingConvention convention,
        bool concatenatePath = false,
        string concatenationDelimiter = "",
        HashSet<string> reservedWords = null)
    {
        if (string.IsNullOrEmpty(_name))
        {
            throw new ArgumentException($"Invalid Name: {_name}");
        }

        if (string.IsNullOrEmpty(_nameCapitalized))
        {
            throw new ArgumentException($"Invalid Name: {_nameCapitalized}");
        }

        if (string.IsNullOrEmpty(_path))
        {
            throw new ArgumentException($"Invalid Path: {_path}");
        }

        switch (convention)
        {
            case NamingConvention.FhirDotNotation:
                {
                    if ((reservedWords != null) &&
                        reservedWords.Contains(_path))
                    {
                        return _path + "Fhir";
                    }

                    return _path;
                }

            case NamingConvention.PascalDotNotation:
                {
                    string[] components = ToPascal(_path.Split('.'));
                    string value = string.Join(".", components);

                    if ((reservedWords != null) &&
                        reservedWords.Contains(value))
                    {
                        components[components.Length - 1] =
                            "Fhir" + components[components.Length - 1];

                        return string.Join(".", components);
                    }

                    return value;
                }

            case NamingConvention.PascalCase:
                {
                    if (concatenatePath)
                    {
                        string[] components = ToPascal(_path.Split('.'));
                        string value = string.Join(concatenationDelimiter, components);

                        if ((reservedWords != null) &&
                            reservedWords.Contains(value))
                        {
                            components[components.Length - 1] =
                                "Fhir" + components[components.Length - 1];

                            return string.Join(concatenationDelimiter, components);
                        }

                        return value;
                    }

                    if ((reservedWords != null) &&
                        reservedWords.Contains(_nameCapitalized))
                    {
                        return "Fhir" + _nameCapitalized;
                    }

                    return _nameCapitalized;
                }

            case NamingConvention.CamelCase:
                {
                    string value;

                    if (concatenatePath)
                    {
                        string[] components = ToCamel(_path.Split('.'));
                        value = string.Join(concatenationDelimiter, components);

                        if ((reservedWords != null) &&
                            reservedWords.Contains(value))
                        {
                            components[components.Length - 1] =
                                "fhir" + ToPascal(components[components.Length - 1]);

                            return string.Join(concatenationDelimiter, components);
                        }
                    }

                    value = ToCamel(_name);

                    if ((reservedWords != null) &&
                        reservedWords.Contains(value))
                    {
                        return "fhir" + _nameCapitalized;
                    }

                    return value;
                }

            case NamingConvention.UpperCase:
                {
                    string value;

                    if (concatenatePath)
                    {
                        string[] components = ToUpperInvariant(_path.Split('.'));
                        value = string.Join(concatenationDelimiter, components);

                        if ((reservedWords != null) &&
                            reservedWords.Contains(value))
                        {
                            components[components.Length - 1] = "FHIR" + components[components.Length - 1];

                            return string.Join(concatenationDelimiter, components);
                        }
                    }

                    value = _name.ToUpperInvariant();

                    if ((reservedWords != null) &&
                        reservedWords.Contains(value))
                    {
                        return "FHIR" + value;
                    }

                    return value;
                }

            case NamingConvention.LowerCase:
                {
                    string value;

                    if (concatenatePath)
                    {
                        string[] components = ToLowerInvariant(_path.Split('.'));
                        value = string.Join(concatenationDelimiter, components);

                        if ((reservedWords != null) &&
                            reservedWords.Contains(value))
                        {
                            components[components.Length - 1] = "fhir" + components[components.Length - 1];

                            return string.Join(concatenationDelimiter, components);
                        }
                    }

                    value = _name.ToLowerInvariant();

                    if ((reservedWords != null) &&
                        reservedWords.Contains(value))
                    {
                        return "fhir" + value;
                    }

                    return value;
                }

            case NamingConvention.None:
            default:
                throw new ArgumentException($"Invalid Naming Convention: {convention}");
        }
    }
}
