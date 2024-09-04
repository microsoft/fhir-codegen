using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Health.Fhir.CodeGen.Tests;

public static class TestCommon
{
    /// <summary>The FHIR R5 package entries.</summary>
    public static readonly string[] EntriesR5 =
    [
        "hl7.fhir.r5.core#5.0.0",
        //"hl7.fhir.r5.expansions#5.0.0",
        //"hl7.fhir.uv.extensions#1.0.0",
    ];

    /// <summary>The FHIR R4B package entries.</summary>
    public static readonly string[] EntriesR4B =
    [
        "hl7.fhir.r4b.core#4.3.0",
        //"hl7.fhir.r4b.expansions#4.3.0",
        //"hl7.fhir.uv.extensions#1.0.0",
    ];

    /// <summary>The FHIR R4 package entries.</summary>
    public static readonly string[] EntriesR4 =
    [
        "hl7.fhir.r4.core#4.0.1",
        //"hl7.fhir.r4.expansions#4.0.1",
        //"hl7.fhir.uv.extensions#1.0.0",
    ];

    /// <summary>The FHIR STU3 package entries.</summary>
    public static readonly string[] EntriesR3 =
    [
        "hl7.fhir.r3.core#3.0.2",
        //"hl7.fhir.r3.expansions#3.0.2",
    ];

    /// <summary>The FHIR DSTU2 package entries.</summary>
    public static readonly string[] EntriesR2 =
    [
        "hl7.fhir.r2.core#1.0.2",
        //"hl7.fhir.r2.expansions#1.0.2",
    ];
}
