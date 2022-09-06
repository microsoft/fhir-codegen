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
    private Dictionary<string, SupportFileRec> _additionalFiles;
    private Dictionary<string, List<string>> _insertions;
    private Dictionary<string, SupportFileRec> _replacementFiles;

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
        _additionalFiles = new();
        _insertions = new(StringComparer.OrdinalIgnoreCase);
        _replacementFiles = new();
    }

    /// <summary>A static support file.</summary>
    public readonly record struct SupportFileRec(
        string Filename,
        string RelativeFilename);

    /// <summary>Gets the additional files that should be included.</summary>
    public Dictionary<string, SupportFileRec> AdditionalFiles => _additionalFiles;

    /// <summary>Gets the files that include insertions for objects.</summary>
    public Dictionary<string, List<string>> Insertions => _insertions;

    /// <summary>Gets the replacement files that should be used only as substitutions.</summary>
    public Dictionary<string, SupportFileRec> ReplacementFiles => _replacementFiles;

    /// <summary>Attempts to get input for key a string from the given string.</summary>
    /// <param name="key">     The key.</param>
    /// <param name="contents">[out] The contents.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    public bool TryGetInputForKey(string key, out string contents)
    {
        contents = string.Empty;

        if (!_insertions.ContainsKey(key))
        {
            return false;
        }

        foreach (string file in _insertions[key])
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
        string inputAdditionsDir = Path.Combine(_directory, "additional-files");

        _additionalFiles.Clear();
        if (Directory.Exists(inputAdditionsDir))
        {
            string[] files = Directory.GetFiles(inputAdditionsDir, "*.*", SearchOption.AllDirectories);

            foreach (string file in files)
            {
                string relative = Path.GetRelativePath(inputAdditionsDir, file);
                _additionalFiles.Add(relative, new(file, relative));
            }
        }

        string inputInsertionsDir = Path.Combine(_directory, "insertions");

        _insertions.Clear();

        if (Directory.Exists(inputInsertionsDir))
        {
            string[] files = Directory.GetFiles(inputInsertionsDir, "*.*", SearchOption.AllDirectories);

            foreach (string file in files)
            {
                string[] components = Path.GetFileNameWithoutExtension(file).Split('_');

                switch (components.Length)
                {
                    // just key
                    case 1:
                        {
                            if (!_insertions.ContainsKey(components[0]))
                            {
                                _insertions.Add(components[0], new());
                            }

                            _insertions[components[0]].Add(file);
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
                                    if (!_insertions.ContainsKey(components[0]))
                                    {
                                        _insertions.Add(components[0], new());
                                    }

                                    _insertions[components[0]].Add(file);
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
                                    if (!_insertions.ContainsKey(components[0]))
                                    {
                                        _insertions.Add(components[0], new());
                                    }

                                    _insertions[components[0]].Add(file);
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

        string inputReplacementDir = Path.Combine(_directory, "replacement-files");

        _replacementFiles.Clear();

        if (Directory.Exists(inputReplacementDir))
        {
            string[] files = Directory.GetFiles(inputReplacementDir, "*.*", SearchOption.AllDirectories);

            foreach (string file in files)
            {
                string relative = Path.GetRelativePath(inputReplacementDir, file);
                _replacementFiles.Add(relative, new(file, relative));
            }
        }

        return true;
    }
}
