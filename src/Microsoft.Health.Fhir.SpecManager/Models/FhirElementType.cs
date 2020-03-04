// <copyright file="FhirElementType.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Health.Fhir.SpecManager.Models
{
    /// <summary>A fhir element type.</summary>
    public class FhirElementType
    {
        private readonly Dictionary<string, FhirElementProfile> _profiles;
        private readonly Uri _baseElementTypeUri = new Uri("http://hl7.org/fhir/StructureDefinition");

        /// <summary>
        /// Initializes a new instance of the <see cref="FhirElementType"/> class.
        /// </summary>
        /// <param name="code">    The code.</param>
        /// <param name="profiles">The profiles.</param>
        public FhirElementType(
            string code,
            IEnumerable<string> profiles)
        {
            if (string.IsNullOrEmpty(code))
            {
                throw new ArgumentNullException(nameof(code));
            }

            if (IsFhirPathType(code, out string fhirType))
            {
                code = fhirType;
            }

            // check for no slashes - implied relative StructureDefinition url
            int lastSlash = code.LastIndexOf('/');
            if (lastSlash == -1)
            {
                Code = code;
                URL = new Uri(_baseElementTypeUri, code);
            }
            else
            {
                Code = code.Substring(lastSlash + 1);
                URL = new Uri(code);
            }

            _profiles = new Dictionary<string, FhirElementProfile>();

            if (profiles != null)
            {
                foreach (string profileUrl in profiles)
                {
                    if (string.IsNullOrEmpty(profileUrl))
                    {
                        continue;
                    }

                    FhirElementProfile profile = new FhirElementProfile(new Uri(profileUrl));

                    if (_profiles.ContainsKey(profile.Name))
                    {
                        continue;
                    }

                    _profiles.Add(profile.Name, profile);
                }
            }
        }

        /// <summary>Gets the code.</summary>
        /// <value>The code.</value>
        public string Code { get; }

        /// <summary>Gets URL of the document.</summary>
        /// <value>The URL.</value>
        public Uri URL { get; }

        /// <summary>Gets the profiles.</summary>
        /// <value>The profiles.</value>
        public Dictionary<string, FhirElementProfile> Profiles => _profiles;

        /// <summary>Check if a type is listed in FHIRPath notation, and return the FHIR type if it is.</summary>
        /// <param name="fhirPathType">Type in FHIRPath.</param>
        /// <param name="fhirType">    [out] Type in FHIR.</param>
        /// <returns>A string.</returns>
        private static bool IsFhirPathType(string fhirPathType, out string fhirType)
        {
            fhirType = string.Empty;

            switch (fhirPathType)
            {
                case "http://hl7.org/fhirpath/System.String":
                    fhirType = "string";
                    break;

                case "http://hl7.org/fhirpath/System.Boolean":
                    fhirType = "boolean";
                    break;

                case "http://hl7.org/fhirpath/System.Date":
                    fhirType = "date";
                    break;

                case "http://hl7.org/fhirpath/System.DateTime":
                    fhirType = "dateTime";
                    break;

                case "http://hl7.org/fhirpath/System.Decimal":
                    fhirType = "decimal";
                    break;

                case "http://hl7.org/fhirpath/System.Integer":
                    fhirType = "int";
                    break;

                case "http://hl7.org/fhirpath/System.Time":
                    fhirType = "time";
                    break;

                default:
                    break;
            }

            return !string.IsNullOrEmpty(fhirType);
        }
    }
}
