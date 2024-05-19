#nullable enable

namespace Microsoft.Health.Fhir.SpecManager.Language.Firely
{
    public abstract record TypeReference(string Name)
    {
        public abstract string PropertyTypeString { get; }

        internal static string MapTypeName(string name)
        {
            if (CSharpFirelyCommon.TypeNameMappings.TryGetValue(name, out string? mapping))
                return mapping;

            return FhirUtils.SanitizedToConvention(name, FhirTypeBase.NamingConvention.PascalCase);
        }
    }

    public record PrimitiveTypeReference(string Name, string PocoTypeName, Type ConveniencePropertyType) : TypeReference(Name)
    {
        public static PrimitiveTypeReference ForTypeName(string name, Type propertyType) =>
            new(name, MapTypeName(name), propertyType);

        public static readonly IReadOnlyCollection<PrimitiveTypeReference> PrimitiveList = new PrimitiveTypeReference[]
        {
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
        };

        private static readonly Dictionary<string, PrimitiveTypeReference> s_primitiveDictionary =
            PrimitiveList.ToDictionary(ptr => ptr.Name);

        public static bool IsFhirPrimitiveType(string name) => s_primitiveDictionary.ContainsKey(name);

        public static PrimitiveTypeReference GetTypeReference(string name) =>
            s_primitiveDictionary.TryGetValue(name, out var tr)
                ? tr
                : throw new InvalidOperationException($"Unknown FHIR primitive {name}");

        public string ConveniencePropertyTypeString =>
            ConveniencePropertyType.Name + (ConveniencePropertyType.IsValueType ? "?" : "");

        public override string PropertyTypeString => $"Hl7.Fhir.Model.{PocoTypeName}";
    }

    public record CqlTypeReference(string Name, Type ConveniencePropertyType) : TypeReference(Name)
    {
        public static readonly CqlTypeReference SystemString = new("String", typeof(string));

        public override string PropertyTypeString => $"SystemPrimitive.{Name}";

        public string ConveniencePropertyTypeString =>
            ConveniencePropertyType.Name + (ConveniencePropertyType.IsValueType ? "?" : "");

    }

    public record ComplexTypeReference(string Name, string PocoTypeName) : TypeReference(Name)
    {
        public override string PropertyTypeString => $"Hl7.Fhir.Model.{PocoTypeName}";
    }

    public record ChoiceTypeReference() : ComplexTypeReference("DataType", "DataType");

    public record CodedTypeReference(string EnumName, string? EnumClassName)
        : PrimitiveTypeReference("code", EnumName, typeof(Enum));

    public record ListType(TypeReference Element) : TypeReference("List")
    {
        public override string PropertyTypeString => $"List<{Element.PropertyTypeString}>";
    }
}
