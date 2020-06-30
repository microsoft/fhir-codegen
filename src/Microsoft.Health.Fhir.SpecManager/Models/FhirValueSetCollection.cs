// <copyright file="FhirValueSetCollection.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Health.Fhir.SpecManager.Models
{
    /// <summary>Collection of fhir value sets.</summary>
    public class FhirValueSetCollection : ICloneable
    {
        private readonly Dictionary<string, FhirValueSet> _valueSetsByVersion;

        /// <summary>
        /// Initializes a new instance of the <see cref="FhirValueSetCollection"/> class.
        /// </summary>
        /// <param name="url">The URL.</param>
        public FhirValueSetCollection(string url)
        {
            _valueSetsByVersion = new Dictionary<string, FhirValueSet>();
            URL = url;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FhirValueSetCollection"/> class.
        /// </summary>
        /// <param name="url">               The URL.</param>
        /// <param name="valueSetsByVersion">The value sets by version.</param>
        private FhirValueSetCollection(string url, Dictionary<string, FhirValueSet> valueSetsByVersion)
        {
            URL = url;
            _valueSetsByVersion = valueSetsByVersion;
        }

        /// <summary>Gets URL of the document.</summary>
        /// <value>The URL.</value>
        public string URL { get; }

        /// <summary>Gets the value sets by version.</summary>
        /// <value>The value sets by version.</value>
        public Dictionary<string, FhirValueSet> ValueSetsByVersion => _valueSetsByVersion;

        /// <summary>Adds a value set.</summary>
        /// <param name="valueSet">Set the value belongs to.</param>
        public void AddValueSet(FhirValueSet valueSet)
        {
            if (valueSet == null)
            {
                return;
            }

            string version = "*";

            if (!string.IsNullOrEmpty(valueSet.Version))
            {
                version = valueSet.Version;
            }

            if (_valueSetsByVersion.ContainsKey(version))
            {
                return;
            }

            _valueSetsByVersion.Add(version, valueSet);
        }

        /// <summary>Query if 'version' has version.</summary>
        /// <param name="version">The version.</param>
        /// <returns>True if version, false if not.</returns>
        public bool HasVersion(string version)
        {
            if (_valueSetsByVersion.Count == 0)
            {
                return false;
            }

            if (string.IsNullOrEmpty(version))
            {
                return true;
            }

            if (_valueSetsByVersion.ContainsKey(version))
            {
                return true;
            }

            return false;
        }

        /// <summary>Gets a version.</summary>
        /// <param name="version">(Optional) The version.</param>
        /// <returns>The version.</returns>
        public bool TryGetValueSet(string version, out FhirValueSet vs)
        {
            if (_valueSetsByVersion.Count == 0)
            {
                vs = null;
                return false;
            }

            if ((!string.IsNullOrEmpty(version)) &&
                _valueSetsByVersion.ContainsKey(version))
            {
                vs = _valueSetsByVersion[version];
                return true;
            }

            if (!string.IsNullOrEmpty(version))
            {
                vs = null;
                return false;
            }

            if (_valueSetsByVersion.Count == 1)
            {
                vs = _valueSetsByVersion.Values.First();
                return true;
            }

            string vLast = string.Empty;

            foreach (string vsVersion in _valueSetsByVersion.Keys)
            {
                if (string.Compare(vLast, vsVersion, StringComparison.Ordinal) > 0)
                {
                    vLast = vsVersion;
                }
            }

            vs = _valueSetsByVersion[vLast];
            return true;
        }

        /// <summary>Creates a new object that is a copy of the current instance.</summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public object Clone()
        {
            Dictionary<string, FhirValueSet> valueSets = new Dictionary<string, FhirValueSet>();

            foreach (KeyValuePair<string, FhirValueSet> kvp in _valueSetsByVersion)
            {
                valueSets.Add(kvp.Key, (FhirValueSet)kvp.Value.Clone());
            }

            return new FhirValueSetCollection(URL, valueSets);
        }
    }
}
