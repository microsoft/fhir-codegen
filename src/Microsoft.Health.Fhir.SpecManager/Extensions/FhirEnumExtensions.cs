// <copyright file="FhirEnumExtensions.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Microsoft.Health.Fhir.SpecManager.Extensions
{
    /// <summary>A FHIR enum extensions.</summary>
    public static class FhirEnumExtensions
    {
        private static Dictionary<Type, EnumValueLookups> _typeLookups = new Dictionary<Type, EnumValueLookups>();

        /// <summary>An Enum extension method that converts a value to a literal.</summary>
        /// <param name="fhirEnum">The fhirEnumValue to act on.</param>
        /// <returns>Value as a string.</returns>
        public static string ToLiteral(this Enum fhirEnum)
        {
            if (fhirEnum == null)
            {
                return null;
            }

            Type type = fhirEnum.GetType();

            if (!_typeLookups.ContainsKey(type))
            {
                LoadTypeLookup(type);
            }

            if (!_typeLookups[type].EnumToString.ContainsKey(fhirEnum))
            {
                throw new Exception($"Unknown enum requested: {fhirEnum}");
            }

            return _typeLookups[type].EnumToString[fhirEnum];
        }

        /// <summary>Loads type lookup.</summary>
        /// <param name="type">The type.</param>
        private static void LoadTypeLookup(Type type)
        {
            EnumValueLookups lookups = new EnumValueLookups()
            {
                EnumToString = new Dictionary<Enum, string>(),
                StringToEnum = new Dictionary<string, object>(),
            };

            foreach (Enum value in Enum.GetValues(type))
            {
                FieldInfo fieldInfo = type.GetField(value.ToString());

                FhirLiteralAttribute[] attributes = fieldInfo.GetCustomAttributes(
                    typeof(FhirLiteralAttribute),
                    false) as FhirLiteralAttribute[];

                if (attributes.Length == 0)
                {
                    continue;
                }

                lookups.EnumToString.Add(value, attributes[0].FhirLiteral);
                lookups.StringToEnum.Add(attributes[0].FhirLiteral, value);
            }

            _typeLookups[type] = lookups;
        }

        /// <summary>
        /// A string extension method that converts a FHIR string literal to a FHIR-Literal tagged enum
        /// value.
        /// </summary>
        /// <exception cref="Exception">Thrown when an exception error condition occurs.</exception>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="literal">The literal.</param>
        /// <returns>Literal as an Enum.</returns>
        public static T ToFhirEnum<T>(this string literal)
            where T : struct
        {
            if (string.IsNullOrEmpty(literal))
            {
                throw new Exception($"Invalid literal: {literal}");
            }

            return ParseFhir<T>(literal);
        }

        /// <summary>
        /// A Type extension method that parses a FHIR string literal to a FHIR-Literal tagged enum value.
        /// </summary>
        /// <exception cref="Exception">Thrown when an exception error condition occurs.</exception>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="literal">The literal.</param>
        /// <returns>An Enum.</returns>
        public static T ParseFhir<T>(string literal)
            where T : struct
        {
            if (string.IsNullOrEmpty(literal))
            {
                throw new Exception($"Invalid literal: {literal}");
            }

            Type enumType = typeof(T);

            if (!_typeLookups.ContainsKey(enumType))
            {
                LoadTypeLookup(enumType);
            }

            if (!_typeLookups[enumType].StringToEnum.ContainsKey(literal))
            {
                throw new Exception($"Unknown enum requested: {literal}");
            }

            return (T)_typeLookups[enumType].StringToEnum[literal];
        }

        /// <summary>
        /// A Type extension method that attempts to parse a FHIR string literal to a FHIR-Literal tagged
        /// enum value.
        /// </summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="literal">The literal.</param>
        /// <param name="value">  [out] The value.</param>
        /// <returns>True if it succeeds, false if it fails.</returns>
        public static bool TryParseFhir<T>(string literal, out object value)
            where T : struct
        {
            if (string.IsNullOrEmpty(literal))
            {
                value = null;
                return false;
            }

            Type enumType = typeof(T);

            if (!_typeLookups.ContainsKey(enumType))
            {
                LoadTypeLookup(enumType);
            }

            if (!_typeLookups[enumType].StringToEnum.ContainsKey(literal))
            {
                value = null;
                return false;
            }

            value = _typeLookups[enumType].StringToEnum[literal];
            return true;
        }

        /// <summary>A Type extension method that parse literal.</summary>
        /// <exception cref="Exception">Thrown when an exception error condition occurs.</exception>
        /// <param name="enumType">The enumType to act on.</param>
        /// <param name="literal"> The literal.</param>
        /// <returns>An Enum.</returns>
        public static object ParseLiteral(this Type enumType, string literal)
        {
            if ((enumType == null) ||
                string.IsNullOrEmpty(literal))
            {
                return null;
            }

            if (!_typeLookups.ContainsKey(enumType))
            {
                LoadTypeLookup(enumType);
            }

            if (!_typeLookups[enumType].StringToEnum.ContainsKey(literal))
            {
                throw new Exception($"Unknown enum requested: {literal}");
            }

            return _typeLookups[enumType].StringToEnum[literal];
        }

        /// <summary>A Type extension method that attempts to parse literal.</summary>
        /// <param name="enumType">The enumType to act on.</param>
        /// <param name="literal"> The literal.</param>
        /// <param name="result">  [out] The result.</param>
        /// <returns>True if it succeeds, false if it fails.</returns>
        public static bool TryParseLiteral(
            this Type enumType,
            string literal,
            out object result)
        {
            if ((enumType == null) ||
                string.IsNullOrEmpty(literal))
            {
                result = null;
                return false;
            }

            if (!_typeLookups.ContainsKey(enumType))
            {
                LoadTypeLookup(enumType);
            }

            if (!_typeLookups[enumType].StringToEnum.ContainsKey(literal))
            {
                result = null;
                return false;
            }

            result = _typeLookups[enumType].StringToEnum[literal];
            return true;
        }

        /// <summary>An enum value lookups.</summary>
        private struct EnumValueLookups
        {
            /// <summary>The enum to string.</summary>
            public Dictionary<Enum, string> EnumToString;

            /// <summary>The string to enum.</summary>
            public Dictionary<string, object> StringToEnum;
        }
    }
}
