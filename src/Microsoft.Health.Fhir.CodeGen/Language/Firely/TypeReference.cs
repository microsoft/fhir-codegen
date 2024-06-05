#nullable enable

using Microsoft.Health.Fhir.CodeGenCommon.Extensions;
using Microsoft.Health.Fhir.CodeGenCommon.Utils;

namespace Microsoft.Health.Fhir.CodeGen.Language.Firely;

public abstract record TypeReference(string Name)
{
    public abstract string PropertyTypeString { get; }

    internal static string MapTypeName(string name)
    {
        if (CSharpFirelyCommon.TypeNameMappings.TryGetValue(name, out string? mapping))
            return mapping;

        return FhirSanitizationUtils.SanitizedToConvention(name, FhirNameConventionExtensions.NamingConvention.PascalCase);
    }

    internal static string RenderNetType(Type t) =>
        AliasCsName(t) + (t.IsValueType ? "?" : "");

    private static string AliasCsName(Type csType)
    {
        // This isn't complete, but good enough for now.
        return csType.Name switch
        {
            "String" => "string",
            "Boolean" => "bool",
            "Decimal" => "decimal",
            "Int32" => "int",
            "Int64" => "long",
            "Byte[]" => "byte[]",
            "DateTimeOffset" => "DateTimeOffset",
            var other => $"System.{other}"
        };
    }
}

public record PrimitiveTypeReference(string Name, string PocoTypeName, Type ConveniencePropertyType)
    : TypeReference(Name)
{
    public static PrimitiveTypeReference ForTypeName(string name, Type propertyType) =>
        new(name, MapTypeName(name), propertyType);

    public static readonly IReadOnlyCollection<PrimitiveTypeReference> PrimitiveList =
    [
        ForTypeName("base64Binary", typeof(byte[])), ForTypeName("boolean", typeof(bool)),
        ForTypeName("canonical", typeof(string)), ForTypeName("code", typeof(string)),
        ForTypeName("date", typeof(string)), ForTypeName("dateTime", typeof(string)),
        ForTypeName("decimal", typeof(decimal)), ForTypeName("id", typeof(string)),
        ForTypeName("instant", typeof(DateTimeOffset)), ForTypeName("integer", typeof(int)),
        ForTypeName("integer64", typeof(long)), ForTypeName("oid", typeof(string)),
        ForTypeName("positiveInt", typeof(int)), ForTypeName("string", typeof(string)),
        ForTypeName("time", typeof(string)), ForTypeName("unsignedInt", typeof(int)),
        ForTypeName("uri", typeof(string)), ForTypeName("url", typeof(string)),
        ForTypeName("xhtml", typeof(string)), ForTypeName("markdown", typeof(string))
    ];

    private static readonly Dictionary<string, PrimitiveTypeReference> s_primitiveDictionary =
        PrimitiveList.ToDictionary(ptr => ptr.Name);

    public static bool IsFhirPrimitiveType(string name) => s_primitiveDictionary.ContainsKey(name);

    public static PrimitiveTypeReference GetTypeReference(string name) =>
        s_primitiveDictionary.TryGetValue(name, out var tr)
            ? tr
            : throw new InvalidOperationException($"Unknown FHIR primitive {name}");

    public virtual string ConveniencePropertyTypeString => RenderNetType(ConveniencePropertyType);

    public override string PropertyTypeString => $"Hl7.Fhir.Model.{PocoTypeName}";
}

public record CqlTypeReference(string Name, Type PropertyType) : TypeReference(Name)
{
    public static readonly CqlTypeReference SystemString = new("String", typeof(string));

    public string DeclaredTypeString => $"SystemPrimitive.{Name}";

    public override string PropertyTypeString => RenderNetType(PropertyType);
}

public record ComplexTypeReference(string Name, string PocoTypeName) : TypeReference(Name)
{
    public override string PropertyTypeString => $"Hl7.Fhir.Model.{PocoTypeName}";
}

public record ChoiceTypeReference() : ComplexTypeReference("DataType", "DataType");

public record CodedTypeReference(string EnumName, string? EnumClassName)
    : PrimitiveTypeReference("code", EnumName, typeof(Enum))
{
    public override string PropertyTypeString => $"Code<{EnumNameString}>";

    public override string ConveniencePropertyTypeString => EnumNameString + "?";

    private string EnumNameString => EnumClassName is not null
        ? $"Hl7.Fhir.Model.{EnumClassName}.{EnumName}"
        : $"Hl7.Fhir.Model.{EnumName}";
}

public record ListTypeReference(TypeReference Element) : TypeReference("List")
{
    public override string PropertyTypeString => $"List<{Element.PropertyTypeString}>";
}
