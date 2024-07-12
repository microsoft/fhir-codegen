#nullable enable

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Health.Fhir.CodeGen._ForPackages;

public class IniData
{
    public static IniOptions DefaultOptions { get; } = new IniOptions();
    public static IniOptions FhirPackagesOptions { get; } = new IniOptions
    {
        CommentCharacters = new[] { ';' },
        CaseSensitive = true,
        KeyValueAssignmentChar = '='
    };

    public class IniOptions
    {
        public char[] CommentCharacters { get; set; } = { ';', '#' };

        public bool CaseSensitive { get; set; } = false;

        public char KeyValueAssignmentChar { get; set; } = '=';
    }

    public class IniValue
    {
        public List<string>? BlockComments { get; set; }

        public string Key { get; set; }

        public string? Value { get; set; }

        public string? InlineComment { get; set; }

        public IniValue(string key, string? value, List<string>? blockComments, string? inlineComment)
        {
            Key = key;
            Value = value;
            BlockComments = blockComments;
            InlineComment = inlineComment;
        }
    }

    public class IniSection
    {
        public List<string>? BlockComments { get; set; }

        public string Name { get; set; }
        public string? InlineComment { get; set; }


        private OrderedDictionary _values = new(StringComparer.OrdinalIgnoreCase);

        public IOrderedDictionary Values => _values;


        public IniSection(string name, List<string>? blockComments, string? inlineComment)
        {
            Name = name;
            BlockComments = blockComments;
            InlineComment = inlineComment;
        }

        public IniValue? this[string key]
        {
            get
            {
                if (_values.Contains(key) && _values[key] is IniValue value)
                {
                    return value;
                }

                return null;
            }
            set
            {
                _values[key] = value;
            }
        }

        public IniValue? this[int index]
        {
            get
            {
                if ((_values.Count >= index) && _values[index] is IniValue value)
                {
                    return value;
                }

                return null;
            }
            set
            {
                _values[index] = value;
            }
        }
    }

    // default characters that are used to start a comment
    private IniOptions _options;

    private HashSet<char> _commentCharHash;

    private readonly string _filename;

    private readonly OrderedDictionary _sections = new(StringComparer.OrdinalIgnoreCase);

    public IOrderedDictionary Sections => _sections;

    private readonly OrderedDictionary _unsectionedValues = new(StringComparer.OrdinalIgnoreCase);

    public IOrderedDictionary UnsectionedValues => _unsectionedValues;

    public IniData(string filename, IniOptions? options = null)
    {
        if (string.IsNullOrEmpty(filename))
        {
            throw new ArgumentNullException(nameof(filename));
        }

        _filename = filename;

        _options = options ?? new IniOptions();

        // ensure the comment characters are valid
        if (_options.CommentCharacters.Contains('[') ||
            _options.CommentCharacters.Contains(']') ||
            _options.CommentCharacters.Contains('=') ||
            _options.CommentCharacters.Any(char.IsWhiteSpace))
        {
            throw new ArgumentException("Comment characters cannot contain '[', ']', '=', or whitespace characters", nameof(options));
        }

        _commentCharHash = new HashSet<char>(_options.CommentCharacters);

        _ = loadFromDisk();
    }

    public IniSection? this[string key]
    {
        get
        {
            if (_sections.Contains(key) && _sections[key] is IniSection section)
            {
                return section;
            }

            return null;
        }
        set
        {
            _sections[key] = value;
        }
    }

    public IniSection? this[int index]
    {
        get
        {
            if ((_sections.Count >= index) && _sections[index] is IniSection section)
            {
                return section;
            }

            return null;
        }
        set
        {
            _sections[index] = value;
        }
    }

    private bool loadFromDisk()
    {
        if (!File.Exists(_filename))
        {
            return false;
        }

        List<string> blockComments = new();

        IniSection? currentSection = null;

        using (StreamReader reader = new StreamReader(_filename))
        {
            while (reader.Peek() >= 0)
            {
                string? line = reader.ReadLine();

                if (line == null)
                {
                    break;
                }

                string trimmed = line.Trim();

                if (string.IsNullOrEmpty(trimmed))
                {
                    continue;
                }

                // check if this is a block comment
                if (_commentCharHash.Contains(trimmed[0]))
                { 
                    blockComments.Add(trimmed);
                    continue;
                }

                int commentIndex = trimmed.IndexOfAny(_options.CommentCharacters);

                string content = commentIndex == -1 ? trimmed : trimmed[..commentIndex];
                string? comment = commentIndex == -1 ? null : trimmed[(commentIndex + 1)..];

                // check if this is a section
                if (content[0] == '[' && content[^1] == ']')
                {
                    // check for existing section
                    if (currentSection != null)
                    {
                        _sections[currentSection.Name] = currentSection;
                    }

                    string sectionName = content[1..^1];
                    currentSection = new IniSection(sectionName, blockComments.Count == 0 ? null : blockComments, comment);
                    blockComments.Clear();
                    continue;
                }

                // parse the content based on the KeyValueAssignmentChar, max of two components: Key and Value
                string[] kvp = content.Split([ _options.KeyValueAssignmentChar ], 2);

                IniValue value = new IniValue(kvp[0].Trim(), kvp.Length == 2 ? kvp[1].Trim() : null, blockComments.Count == 0 ? null : blockComments, comment);

                if (currentSection != null)
                {
                    currentSection[value.Key] = value;
                }
                else
                {
                    _unsectionedValues[value.Key] = value;
                }

                blockComments.Clear();
            }

            // add the final section we were reading, if one exists
            if (currentSection != null)
            {
                _sections[currentSection.Name] = currentSection;
            }
        }

        return true;
    }

    private bool saveToDisk()
    {
        using (StreamWriter writer = new StreamWriter(_filename))
        {
            // start with any unsectioned values
            foreach (IniValue value in _unsectionedValues.Values)
            {
                if (value.BlockComments != null)
                {
                    foreach (string comment in value.BlockComments)
                    {
                        writer.WriteLine(comment);
                    }
                }

                writer.WriteLine($"{value.Key}{_options.KeyValueAssignmentChar}{value.Value} {value.InlineComment}");
            }

            // write each section
            foreach (IniSection section in _sections.Values)
            {
                if (section.BlockComments != null)
                {
                    foreach (string comment in section.BlockComments)
                    {
                        writer.WriteLine(comment);
                    }
                }

                writer.WriteLine($"[{section.Name}] {section.InlineComment}");

                foreach (IniValue value in section.Values)
                {
                    if (value.BlockComments != null)
                    {
                        foreach (string comment in value.BlockComments)
                        {
                            writer.WriteLine(comment);
                        }
                    }

                    writer.WriteLine($"{value.Key}{_options.KeyValueAssignmentChar}{value.Value} {value.InlineComment}");
                }
            }
        }

        return true;
    }
}

#nullable restore
