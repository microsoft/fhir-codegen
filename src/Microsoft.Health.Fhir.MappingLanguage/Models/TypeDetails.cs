// <copyright file="TypeDetails.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using Hl7.Fhir.Model;
using Microsoft.Health.Fhir.CodeGenCommon.FhirExtensions;

namespace Microsoft.Health.Fhir.MappingLanguage.Models;

public class TypeDetails
{
    /// <summary>A profiled type.</summary>
    public class ProfiledType
    {
        private string _uri = null!;

        /// <summary>Gets or sets URI of the document.</summary>
        public required string Uri
        {
            get => _uri;
            set => _uri = MapUtilities.IsAbsoluteUrl(value) ? value : CommonDefinitions.FhirStructureUrlPrefix + value;
        }

        /// <summary>Gets or sets the profiles.</summary>
        public List<string> Profiles { get; set; } = new();

        /// <summary>Gets or sets the element definitions.</summary>
        public List<ElementDefinition.ElementDefinitionBindingComponent> ElementDefinitions { get; set; } = new();
    }

}
