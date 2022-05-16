// <copyright file="LanguageSupportFiles.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System.Collections;
using System.IO;
using static Microsoft.Health.Fhir.SpecManager.Manager.FhirPackageCommon;

namespace Microsoft.Health.Fhir.SpecManager.Language;

/// <summary>A language input support.</summary>
public class LanguageSupportFiles
{
    /// <summary>Pathname of the directory.</summary>
    private string _directory;
    private Dictionary<string, SupportFileRec> _staticFiles;
    private Dictionary<string, List<string>> _dynamicFiles;

    /// <summary>
    /// Initializes a new instance of the <see cref="LanguageSupportFiles"/> class.
    /// </summary>
    /// <param name="dir">         The dir.</param>
    /// <param name="languageName">Name of the language.</param>
    public LanguageSupportFiles(string dir, string languageName)
    {
        if (string.IsNullOrEmpty(dir) || string.IsNullOrEmpty(languageName))
        {
            return;
        }

        string inputDir = dir;

        if (!Directory.Exists(inputDir))
        {
            throw new DirectoryNotFoundException($"Language input directory: {inputDir} not found!");
        }

        if (Directory.Exists(Path.Combine(inputDir, languageName)))
        {
            inputDir = Path.Combine(inputDir, languageName);
        }

        _directory = inputDir;
        _staticFiles = new();
        _dynamicFiles = new(StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>A static support file.</summary>
    public readonly record struct SupportFileRec(
        string Filename,
        string RelativeFilename);

    /// <summary>Gets the static files.</summary>
    public Dictionary<string, SupportFileRec> StaticFiles => _staticFiles;

    /// <summary>Gets the dynamic files.</summary>
    public Dictionary<string, List<string>> DynamicFiles => _dynamicFiles;

    /// <summary>Attempts to get input for key a string from the given string.</summary>
    /// <param name="key">     The key.</param>
    /// <param name="contents">[out] The contents.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    public bool TryGetInputForKey(string key, out string contents)
    {
        contents = string.Empty;

        if (!_dynamicFiles.ContainsKey(key))
        {
            return false;
        }

        foreach (string file in _dynamicFiles[key])
        {
            contents += File.ReadAllText(file);
        }

        return true;
    }

    /// <summary>Attempts to load.</summary>
    /// <param name="fhirSequence">The FHIR sequence.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    public bool TryLoad(FhirSequenceEnum fhirSequence)
    {
        string inputStaticDir = Path.Combine(_directory, "static");

        _staticFiles.Clear();
        if (Directory.Exists(inputStaticDir))
        {
            string[] files = Directory.GetFiles(inputStaticDir, "*.*", SearchOption.AllDirectories);

            foreach (string file in files)
            {
                string relative = Path.GetRelativePath(inputStaticDir, file);
                _staticFiles.Add(relative, new(file, relative));
            }
        }

        string inputDynamicDir = Path.Combine(_directory, "dynamic");

        _dynamicFiles.Clear();

        if (Directory.Exists(inputDynamicDir))
        {
            string[] files = Directory.GetFiles(inputDynamicDir, "*.*", SearchOption.AllDirectories);

            foreach (string file in files)
            {
                string[] components = Path.GetFileNameWithoutExtension(file).Split('_');

                switch (components.Length)
                {
                    // just key
                    case 1:
                        {
                            if (!_dynamicFiles.ContainsKey(components[0]))
                            {
                                _dynamicFiles.Add(components[0], new());
                            }

                            _dynamicFiles[components[0]].Add(file);
                        }

                        break;

                    // key and min version
                    case 2:
                        {
                            try
                            {
                                FhirSequenceEnum min = MajorReleaseForVersion(components[1]);

                                if (fhirSequence >= min)
                                {
                                    if (!_dynamicFiles.ContainsKey(components[0]))
                                    {
                                        _dynamicFiles.Add(components[0], new());
                                    }

                                    _dynamicFiles[components[0]].Add(file);
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($" <<< exception processing file: {file}: {ex.Message}");
                                return false;
                            }
                        }

                        break;

                    // key, min, and max version
                    case 3:
                        {
                            try
                            {
                                FhirSequenceEnum min = MajorReleaseForVersion(components[1]);
                                FhirSequenceEnum max = MajorReleaseForVersion(components[2]);

                                if ((fhirSequence >= min) && (fhirSequence <= max))
                                {
                                    if (!_dynamicFiles.ContainsKey(components[0]))
                                    {
                                        _dynamicFiles.Add(components[0], new());
                                    }

                                    _dynamicFiles[components[0]].Add(file);
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($" <<< exception processing file: {file}: {ex.Message}");
                                return false;
                            }
                        }

                        break;

                    default:
                        Console.WriteLine($" <<< unsupported language input file: {file}");
                        return false;
                }
            }
        }

        return true;
    }
}
