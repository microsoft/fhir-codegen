using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Health.Fhir.CodeGen.Configuration;
using Microsoft.Health.Fhir.CodeGen.Models;

namespace Microsoft.Health.Fhir.CodeGen.TerminologyService;

public record class CodeGenValueSetExpanderSettings
{
    public int MaxExpansionSize { get; init; } = ConfigRoot.DefaultMaxExpansionSize;

    public bool? IncludeDesignations { get; init; } = null;
    public bool? IncludeNotSelectable { get; init; } = null;

    public bool? ActiveOnly { get; init; } = null;

    public required DefinitionCollection Definitions { get; init; }
}
