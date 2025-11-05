using Microsoft.Health.Fhir.CodeGenCommon.Extensions;
using Microsoft.Health.Fhir.CodeGenCommon.Utils;

namespace Microsoft.Health.Fhir.CodeGen.Language.Firely;

public abstract record TypeReference(string Name)
{
    public static TypeReference BuildFromFhirTypeName(string name, string? vsName=null, string? vsClass=null)
    {
        // Elements of type Code or Code<T> have their own naming/types, so handle those separately.
        if (name == "code" && vsName is not null)
            return new CodedTypeReference(vsName, vsClass);

        if (PrimitiveTypeReference.IsFhirPrimitiveType(name))
            return PrimitiveTypeReference.GetTypeReference(name);

        // Otherwise, this is a "normal" name for a complex type.
        return new ComplexTypeReference(name, MapTypeName(name));
    }

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

    public static readonly PrimitiveTypeReference PrimitiveType = ForTypeName("PrimitiveType", typeof(object));
    public static readonly PrimitiveTypeReference Boolean = ForTypeName("boolean", typeof(bool));
    public static readonly PrimitiveTypeReference Base64Binary = ForTypeName("base64Binary", typeof(byte[]));
    public static readonly PrimitiveTypeReference Canonical = ForTypeName("canonical", typeof(string));
    public static readonly PrimitiveTypeReference Code = ForTypeName("code", typeof(string));
    public static readonly PrimitiveTypeReference Date = ForTypeName("date", typeof(string));
    public static readonly PrimitiveTypeReference DateTime = ForTypeName("dateTime", typeof(string));
    public static readonly PrimitiveTypeReference Decimal = ForTypeName("decimal", typeof(decimal));
    public static readonly PrimitiveTypeReference Id = ForTypeName("id", typeof(string));
    public static readonly PrimitiveTypeReference Instant = ForTypeName("instant", typeof(DateTimeOffset));
    public static readonly PrimitiveTypeReference Integer = ForTypeName("integer", typeof(int));
    public static readonly PrimitiveTypeReference Integer64 = ForTypeName("integer64", typeof(long));
    public static readonly PrimitiveTypeReference Oid = ForTypeName("oid", typeof(string));
    public static readonly PrimitiveTypeReference PositiveInt = ForTypeName("positiveInt", typeof(int));
    public static readonly PrimitiveTypeReference String = ForTypeName("string", typeof(string));
    public static readonly PrimitiveTypeReference Time = ForTypeName("time", typeof(string));
    public static readonly PrimitiveTypeReference UnsignedInt = ForTypeName("unsignedInt", typeof(int));
    public static readonly PrimitiveTypeReference Uri = ForTypeName("uri", typeof(string));
    public static readonly PrimitiveTypeReference Url = ForTypeName("url", typeof(string));
    public static readonly PrimitiveTypeReference Xhtml = ForTypeName("xhtml", typeof(string));
    public static readonly PrimitiveTypeReference Markdown = ForTypeName("markdown", typeof(string));

    public static readonly IReadOnlyCollection<PrimitiveTypeReference> PrimitiveList =
    [
        Boolean, Base64Binary, Canonical, Code, Date, DateTime, Decimal, Id,
        Instant, Integer, Integer64, Oid, PositiveInt, String, Time, UnsignedInt,
        Uri, Url, Xhtml, Markdown
    ];

    private static readonly Dictionary<string, PrimitiveTypeReference> _primitiveDictionary =
        PrimitiveList.ToDictionary(ptr => ptr.Name);

    public static bool IsFhirPrimitiveType(string name) => _primitiveDictionary.ContainsKey(name);

    public static PrimitiveTypeReference GetTypeReference(string name) =>
        _primitiveDictionary.TryGetValue(name, out var tr)
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
    public ComplexTypeReference(string name) : this(name, name) { }

    public override string PropertyTypeString => $"Hl7.Fhir.Model.{PocoTypeName}";

    public static readonly ComplexTypeReference DataTypeReference = new("DataType");
}

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
