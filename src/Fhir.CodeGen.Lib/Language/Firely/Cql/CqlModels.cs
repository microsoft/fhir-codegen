using System.Xml.Serialization;
using Hl7.Fhir.Model;

namespace Ncqa.Cql.Model
{
    internal static class CqlModels
    {
        private static readonly XmlSerializer xmlSerializer = new(typeof(ModelInfo));

        public static IDictionary<string, ClassInfo> ClassesByName(ModelInfo model)
        {
            var result = model.typeInfo.OfType<ClassInfo>()
               .ToDictionary(classInfo => classInfo.name);
            return result;
        }

        public static ModelInfo LoadFromStream(System.IO.Stream stream)
        {
            return xmlSerializer.Deserialize(stream) as ModelInfo
                ?? throw new ArgumentException($"This resource is not a valid {nameof(ModelInfo)}");
        }

        public static ModelInfo LoadEmbeddedResource(string resourceName)
        {
            var stream = typeof(CqlModels).Assembly.GetManifestResourceStream(resourceName)
                ?? throw new ArgumentException($"Manifest resource stream {resourceName} is not included in this assembly.");
            return LoadFromStream(stream);
        }
    }
}
