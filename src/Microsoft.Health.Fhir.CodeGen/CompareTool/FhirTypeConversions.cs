using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using CMR = Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship;

namespace Microsoft.Health.Fhir.CodeGen.CompareTool;

public class FhirTypeConversions
{
    public record class FhirTypeConversionInfoRec
    {
        public required string Source { get; init; }
        public required string Target { get; init; }
        public required CMR Relationship { get; init; }
        public required string Message { get; init; }
    }

    internal static Dictionary<string, Dictionary<string, FhirTypeConversionInfoRec>> _fhirTypeSerializationMap = new()
    {
        { "base64binary", new()
            {
                { "base64Binary", new FhirTypeConversionInfoRec()
                    {
                        Source = "base64Binary",
                        Target = "base64Binary",
                        Relationship = CMR.Equivalent,
                        Message = "base64Binary to base64Binary is an exact match",
                    }
                }
            }
        },
        { "boolean", new()
            {
                { "boolean", new FhirTypeConversionInfoRec()
                    {
                        Source = "boolean",
                        Target = "boolean",
                        Relationship = CMR.Equivalent,
                        Message = "boolean to boolean is an exact match",
                    }
                }
            }
        },
        { "canonical", new()
            {
                { "canonical", new FhirTypeConversionInfoRec()
                    {
                        Source = "canonical",
                        Target = "canonical",
                        Relationship = CMR.Equivalent,
                        Message = "canonical to canonical is an exact match",
                    }
                },
                { "uri", new FhirTypeConversionInfoRec()
                    {
                        Source = "canonical",
                        Target = "uri",
                        Relationship = CMR.Equivalent,
                        Message = "canonical (added R4) to uri is lossless because they are the same format",
                    }
                },

            }
        },
        { "code", new()
            {
                { "code", new FhirTypeConversionInfoRec()
                    {
                        Source = "code",
                        Target = "code",
                        Relationship = CMR.Equivalent,
                        Message = "code to code is an exact match",
                    }
                },
                { "id", new FhirTypeConversionInfoRec()
                    {
                        Source = "code",
                        Target = "id",
                        Relationship = CMR.SourceIsBroaderThanTarget,
                        Message = "code to id is possible, but code is broader",
                    }
                },
                { "string", new FhirTypeConversionInfoRec()
                    {
                        Source = "code",
                        Target = "string",
                        Relationship = CMR.SourceIsNarrowerThanTarget,
                        Message = "code to string is lossless because code is narrower",
                    }
                },
                { "uri", new FhirTypeConversionInfoRec()
                    {
                        Source = "code",
                        Target = "uri",
                        Relationship = CMR.SourceIsNarrowerThanTarget,
                        Message = "code to uri is lossless because code is narrower",
                    }
                },
            }
        },
        { "date", new()
            {
                { "date", new FhirTypeConversionInfoRec()
                    {
                        Source = "date",
                        Target = "date",
                        Relationship = CMR.Equivalent,
                        Message = "date to date is an exact match",
                    }
                },
                { "dateTime", new FhirTypeConversionInfoRec()
                    {
                        Source = "date",
                        Target = "dateTime",
                        Relationship = CMR.SourceIsNarrowerThanTarget,
                        Message = "date to dateTime is lossless because date is narrower",
                    }
                },
            }
        },
        { "dateTime", new()
            {
                { "dateTime", new FhirTypeConversionInfoRec()
                    {
                        Source = "dateTime",
                        Target = "dateTime",
                        Relationship = CMR.Equivalent,
                        Message = "dateTime to dateTime is an exact match",
                    }
                },
                { "date", new FhirTypeConversionInfoRec()
                    {
                        Source = "dateTime",
                        Target = "date",
                        Relationship = CMR.SourceIsBroaderThanTarget,
                        Message = "dateTime to date is possible, bur dateTime is broader",
                    }
                },
            }
        },
        { "decimal", new()
            {
                { "decimal", new FhirTypeConversionInfoRec()
                    {
                        Source = "decimal",
                        Target = "decimal",
                        Relationship = CMR.Equivalent,
                        Message = "decimal to decimal is an exact match",
                    }
                },
            }
        },
        { "id", new()
            {
                { "id", new FhirTypeConversionInfoRec()
                    {
                        Source = "id",
                        Target = "id",
                        Relationship = CMR.Equivalent,
                        Message = "id to id is an exact match",
                    }
                },
                { "code", new FhirTypeConversionInfoRec()
                    {
                        Source = "id",
                        Target = "code",
                        Relationship = CMR.SourceIsNarrowerThanTarget,
                        Message = "id to code is lossless, because id is narrower",
                    }
                },
                { "string", new FhirTypeConversionInfoRec()
                    {
                        Source = "id",
                        Target = "string",
                        Relationship = CMR.SourceIsNarrowerThanTarget,
                        Message = "id to string is lossless, because id is narrower",
                    }
                },
            }
        },
        { "instant", new()
            {
                { "instant", new FhirTypeConversionInfoRec()
                    {
                        Source = "instant",
                        Target = "instant",
                        Relationship = CMR.Equivalent,
                        Message = "instant to instant is an exact match",
                    }
                },
            }
        },
        { "integer", new()
            {
                { "integer", new FhirTypeConversionInfoRec()
                    {
                        Source = "integer",
                        Target = "integer",
                        Relationship = CMR.Equivalent,
                        Message = "integer to integer is an exact match",
                    }
                },
                { "integer64", new FhirTypeConversionInfoRec()
                    {
                        Source = "integer",
                        Target = "integer64",
                        Relationship = CMR.SourceIsNarrowerThanTarget,
                        Message = "integer to integer64 (added R5) is lossless, because integer is narrower",
                    }
                },
            }
        },
        { "markdown", new()
            {
                { "markdown", new FhirTypeConversionInfoRec()
                    {
                        Source = "markdown",
                        Target = "markdown",
                        Relationship = CMR.Equivalent,
                        Message = "markdown to markdown is an exact match",
                    }
                },
                { "string", new FhirTypeConversionInfoRec()
                    {
                        Source = "markdown",
                        Target = "string",
                        Relationship = CMR.Equivalent,
                        Message = "markdown to string is lossless",
                    }
                },
            }
        },
        { "oid", new()
            {
                { "oid", new FhirTypeConversionInfoRec()
                    {
                        Source = "oid",
                        Target = "oid",
                        Relationship = CMR.Equivalent,
                        Message = "oid to oid is an exact match",
                    }
                },
            }
        },
        { "positiveInt", new()
            {
                { "positiveInt", new FhirTypeConversionInfoRec()
                    {
                        Source = "positiveInt",
                        Target = "positiveInt",
                        Relationship = CMR.Equivalent,
                        Message = "positiveInt to positiveInt is an exact match",
                    }
                },
                { "integer64", new FhirTypeConversionInfoRec()
                    {
                        Source = "positiveInt",
                        Target = "integer64",
                        Relationship = CMR.SourceIsNarrowerThanTarget,
                        Message = "positiveInt to integer64 (added R5) is lossless because positiveInt is narrower",
                    }
                },
            }
        },
        { "string", new()
            {
                { "string", new FhirTypeConversionInfoRec()
                    {
                        Source = "string",
                        Target = "string",
                        Relationship = CMR.Equivalent,
                        Message = "string to string is an exact match",
                    }
                },
                { "time", new FhirTypeConversionInfoRec()
                    {
                        Source = "string",
                        Target = "time",
                        Relationship = CMR.SourceIsBroaderThanTarget,
                        Message = "string to time (added R3) is possible, but string is broader",
                    }
                },
                { "integer64", new FhirTypeConversionInfoRec()
                    {
                        Source = "string",
                        Target = "integer64",
                        Relationship = CMR.SourceIsBroaderThanTarget,
                        Message = "string to integer64 (added R5) is possible, but string is broader",
                    }
                },
                { "markdown", new FhirTypeConversionInfoRec()
                    {
                        Source = "string",
                        Target = "markdown",
                        Relationship = CMR.Equivalent,
                        Message = "string to markdown is lossless",
                    }
                },
                { "id", new FhirTypeConversionInfoRec()
                    {
                        Source = "string",
                        Target = "id",
                        Relationship = CMR.SourceIsBroaderThanTarget,
                        Message = "string to id is possible, but string is broader",
                    }
                },
                { "integer64", new FhirTypeConversionInfoRec()
                    {
                        Source = "string",
                        Target = "integer64",
                        Relationship = CMR.SourceIsBroaderThanTarget,
                        Message = "string to integer64 (added R5) is possible, but string is broader",
                    }
                },
                { "code", new FhirTypeConversionInfoRec()
                    {
                        Source = "string",
                        Target = "code",
                        Relationship = CMR.SourceIsBroaderThanTarget,
                        Message = "string to code is possible, but string is broader",
                    }
                },
            }
        },
        { "time", new()
            {
                { "time", new FhirTypeConversionInfoRec()
                    {
                        Source = "time",
                        Target = "time",
                        Relationship = CMR.Equivalent,
                        Message = "time to time is an exact match",
                    }
                },
                { "string", new FhirTypeConversionInfoRec()
                    {
                        Source = "time",
                        Target = "string",
                        Relationship = CMR.Equivalent,
                        Message = "time (added R3) to string is possible because time is narrower",
                    }
                }
            }
        },
        { "unsignedInt", new()
            {
                { "unsignedInt", new FhirTypeConversionInfoRec()
                    {
                        Source = "unsignedInt",
                        Target = "unsignedInt",
                        Relationship = CMR.Equivalent,
                        Message = "unsignedInt to unsignedInt is an exact match",
                    }
                },
                { "integer64", new FhirTypeConversionInfoRec()
                    {
                        Source = "unsignedInt",
                        Target = "integer64",
                        Relationship = CMR.SourceIsNarrowerThanTarget,
                        Message = "unsignedInt to integer64 (added R5) is lossless because unsignedInt is narrower",
                    }
                },
            }
        },
        { "uri", new()
            {
                { "uri", new FhirTypeConversionInfoRec()
                    {
                        Source = "uri",
                        Target = "uri",
                        Relationship = CMR.Equivalent,
                        Message = "uri to uri is an exact match",
                    }
                },
                { "url", new FhirTypeConversionInfoRec()
                    {
                        Source = "uri",
                        Target = "url",
                        Relationship = CMR.Equivalent,
                        Message = "uri to url (added R4) is lossless because they are the same format",
                    }
                },
                { "oid", new FhirTypeConversionInfoRec()
                    {
                        Source = "uri",
                        Target = "oid",
                        Relationship = CMR.SourceIsBroaderThanTarget,
                        Message = "uri to oid is possible, but uri is broader",
                    }
                },
                { "uuid", new FhirTypeConversionInfoRec()
                    {
                        Source = "uri",
                        Target = "uuid",
                        Relationship = CMR.SourceIsBroaderThanTarget,
                        Message = "uri to uuid (added R3) is possible, but uri is broader",
                    }
                },
                { "canonical", new FhirTypeConversionInfoRec()
                    {
                        Source = "uri",
                        Target = "canonical",
                        Relationship = CMR.Equivalent,
                        Message = "uri to canonical (added R4) is lossless",
                    }
                },
                { "code", new FhirTypeConversionInfoRec()
                    {
                        Source = "uri",
                        Target = "code",
                        Relationship = CMR.SourceIsBroaderThanTarget,
                        Message = "uri to code is possible, but uri is broader",
                    }
                },
            }
        },
        { "url", new()
            {
                { "url", new FhirTypeConversionInfoRec()
                    {
                        Source = "url",
                        Target = "url",
                        Relationship = CMR.Equivalent,
                        Message = "url to url is an exact match",
                    }
                },
                { "uri", new FhirTypeConversionInfoRec()
                    {
                        Source = "url",
                        Target = "uri",
                        Relationship = CMR.Equivalent,
                        Message = "url (added R4) to uri is lossless because they are the same format",
                    }
                },
            }
        },
        { "uuid", new()
            {
                { "uuid", new FhirTypeConversionInfoRec()
                    {
                        Source = "uuid",
                        Target = "uuid",
                        Relationship = CMR.Equivalent,
                        Message = "uuid to uuid is an exact match",
                    }
                },
                { "uri", new FhirTypeConversionInfoRec()
                    {
                        Source = "uuid",
                        Target = "uri",
                        Relationship = CMR.SourceIsNarrowerThanTarget,
                        Message = "uuid (added R3) to uri is lossless because uuid is narrower",
                    }
                },
            }
        },
        { "integer64", new()
            {
                { "integer64", new FhirTypeConversionInfoRec()
                    {
                        Source = "integer64",
                        Target = "integer64",
                        Relationship = CMR.Equivalent,
                        Message = "integer64 to integer64 is an exact match",
                    }
                },
                { "string", new FhirTypeConversionInfoRec()
                    {
                        Source = "integer64",
                        Target = "string",
                        Relationship = CMR.SourceIsNarrowerThanTarget,
                        Message = "integer64 (added R5) to string is lossless because integer64 is narrower",
                    }
                },
            }
        },
        { "xhtml", new()
            {
                { "xhtml", new FhirTypeConversionInfoRec()
                    {
                        Source = "xhtml",
                        Target = "xhtml",
                        Relationship = CMR.Equivalent,
                        Message = "xhtml to xhtml is an exact match",
                    }
                },
                { "string", new FhirTypeConversionInfoRec()
                    {
                        Source = "xhtml",
                        Target = "string",
                        Relationship = CMR.Equivalent,
                        Message = "xhtml to string is lossless because strings can contain all XHTML data",
                    }
                },
            }
        },
    };
}
