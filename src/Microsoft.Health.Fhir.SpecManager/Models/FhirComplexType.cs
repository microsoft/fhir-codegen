// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Health.Fhir.SpecManager.Models
{
    /// -------------------------------------------------------------------------------------------------
    /// <summary>A FHIR complex type.</summary>
    /// -------------------------------------------------------------------------------------------------
    public class FhirComplexType : FhirTypeBase
    {
        public FhirComplexType()
        {
            Properties = new Dictionary<string, FhirProperty>();
        }
    }
}
