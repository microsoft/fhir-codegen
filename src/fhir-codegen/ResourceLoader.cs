using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using System.Diagnostics;
using System.Text.Json;

namespace fhir_codegen;

public class ResourceLoader
{
    private static JsonSerializerOptions _jsonOptions = new JsonSerializerOptions()
    {
        AllowTrailingCommas = true,
    }.ForFhir(new FhirJsonPocoDeserializerSettings()
    {
        DisableBase64Decoding = false,
        Validator = null,
    });


    private static readonly HashSet<string> _loadResourceTypes = new()
    {
        "CodeSystem",
        "ValueSet",
        "StructureDefinition",
        "SearchParameter",
        "OperationDefinition",
        "CapabilityStatement",
        "Conformance",
        "ImplementationGuide",
        "CompartmentDefinition",
    };

    List<Resource> _resources = new();

    public List<Resource> Resources => _resources;

    public async System.Threading.Tasks.Task<int> ProcessFiles(IEnumerable<string> files)
    {
        await System.Threading.Tasks.Task.Delay(0);

        int fileCount = 0;
        long jsonSize = 0;

        long memPreLoad = GC.GetTotalMemory(false);

        foreach (string file in files)
        {
            string filename = Path.GetFileName(file);

            string resourceType = filename.Split('-').First();

            if (!_loadResourceTypes.Contains(resourceType))
            {
                continue;
            }

            jsonSize += new FileInfo(file).Length;
            fileCount++;

            switch (resourceType)
            {
                case "CodeSystem":
                    {
                        CodeSystem? codeSystem = ParseJson<CodeSystem>("json", file);
                        if (codeSystem != null)
                        {
                            _resources.Add(codeSystem);
                        }
                        break;
                    }

                case "ValueSet":
                    {
                        ValueSet? valueSet = ParseJson<ValueSet>("json", file);
                        if (valueSet != null)
                        {
                            _resources.Add(valueSet);
                        }
                        break;
                    }

                case "StructureDefinition":
                    {
                        StructureDefinition? structureDefinition = ParseJson<StructureDefinition>("json", file);
                        if (structureDefinition != null)
                        {
                            _resources.Add(structureDefinition);
                        }
                        break;
                    }

                case "SearchParameter":
                    {
                        SearchParameter? searchParameter = ParseJson<SearchParameter>("json", file);
                        if (searchParameter != null)
                        {
                            _resources.Add(searchParameter);
                        }
                        break;
                    }

                case "OperationDefinition":
                    {
                        OperationDefinition? operationDefinition = ParseJson<OperationDefinition>("json", file);
                        if (operationDefinition != null)
                        {
                            _resources.Add(operationDefinition);
                        }
                        break;
                    }

                case "CapabilityStatement":
                    {
                        CapabilityStatement? capabilityStatement = ParseJson<CapabilityStatement>("json", file);
                        if (capabilityStatement != null)
                        {
                            _resources.Add(capabilityStatement);
                        }
                        break;
                    }

                case "ImplementationGuide":
                    {
                        ImplementationGuide? implementationGuide = ParseJson<ImplementationGuide>("json", file);
                        if (implementationGuide != null)
                        {
                            _resources.Add(implementationGuide);
                        }
                        break;
                    }

                case "CompartmentDefinition":
                    {
                        CompartmentDefinition? compartmentDefinition = ParseJson<CompartmentDefinition>("json", file);
                        if (compartmentDefinition != null)
                        {
                            _resources.Add(compartmentDefinition);
                        }
                        break;
                    }
            }
        }

        // Perform a collection of all generations up to and including 2.
        GC.Collect(2);

        long memPostLoad = GC.GetTotalMemory(false);
        long memDiff = memPostLoad - memPreLoad;

        // report
        Console.WriteLine($"Loaded: {fileCount} files, {jsonSize} bytes");
        Console.WriteLine($"Resources: {_resources.Count}");
        Console.WriteLine($"Memory usage: pre-load: {memPreLoad}, post-load: {memPostLoad}, difference: {memDiff}");
        Console.WriteLine($"Relative to resource JSON: {((double)memDiff) / ((double)jsonSize)}");

        using (Process p = Process.GetCurrentProcess())
        {
            Console.WriteLine($"Post-load Working set: {p.WorkingSet64}");
        }

        Console.Write("");

        return 0;
    }

    public TResource? ParseJson<TResource>(string format, string path) where TResource : Resource, new()
    {
        switch (format.ToLowerInvariant())
        {
            case ".json":
            case "json":
            case "fhir+json":
            case "application/json":
            case "application/fhir+json":
                try
                {
                    using (FileStream fs = new(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        return JsonSerializer.Deserialize<TResource>(fs, _jsonOptions);
                    }
                }
                catch (Exception ex)
                {
                    if (ex.InnerException == null)
                    {
                        Console.WriteLine($"Error parsing JSON: {ex.Message}");
                    }
                    else
                    {
                        Console.WriteLine($"Error parsing JSON: {ex.Message} ({ex.InnerException.Message})");
                    }
                    return null;
                }

            default:
                {
                    Console.WriteLine($"Unsupported parse format: {format}");
                    return null;
                }
        }
    }
}
