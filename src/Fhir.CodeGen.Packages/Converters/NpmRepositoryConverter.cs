using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Fhir.CodeGen.Packages.Models;

namespace Fhir.CodeGen.Packages.Converters;

/// <summary>
/// Deserializes the npm 'repository' field which may be:
///   "string"
///   { "type"?: "...", "url"?: "...", "directory"?: "..." }
///   [ "string" | { ... }, ... ]
/// Always returns a list of NpmRepositoryRecord (or null).
/// Serialization is intentionally not supported.
/// </summary>
public sealed class NpmRepositoryConverter : JsonConverter<IReadOnlyList<NpmRepositoryRecord>?>
{
    public override IReadOnlyList<NpmRepositoryRecord>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.TokenType switch
        {
            JsonTokenType.Null => null,
            JsonTokenType.String => new List<NpmRepositoryRecord> { FromString(ref reader) },
            JsonTokenType.StartObject => new List<NpmRepositoryRecord> { FromObject(ref reader) },
            JsonTokenType.StartArray => FromArray(ref reader),
            _ => throw new JsonException("Invalid token for repository field.")
        };
    }

    public override void Write(Utf8JsonWriter writer, IReadOnlyList<NpmRepositoryRecord>? value, JsonSerializerOptions options)
        => throw new NotSupportedException("Serialization of repository is not supported (read-only).");

    private static NpmRepositoryRecord FromString(ref Utf8JsonReader reader)
    {
        /* Need to parse the following representative formats (not exhaustive):
            - https://github.com/foo/repourl                => Url=https://github.com/foo/repourl
            - git://github.com/foo/hostedgit                => Url=git://github.com/foo/hostedgit
            - git@github.com:foo/hostedgitat                => Url=git@github.com:foo/hostedgitat
            - ssh://git@github.com/foo/hostedssh            => Url=ssh://git@github.com/foo/hostedssh
            - git+ssh://git@github.com/foo/hostedgitssh     => Type=git, Url=git+ssh://git@github.com/foo/hostedgitssh
            - git+http://github.com/foo/hostedgithttp       => Type=git, Url=git+http://github.com/foo/hostedgithttp
            - git+https://github.com/foo/hostedgithttps     => Type=git, Url=git+https://github.com/foo/hostedgithttps
            - git@gothib.com:foo/unhostedgitatobj           => Type=git, Url=git@gothib.com, Directory = foo/unhostedgitatobj
            - github/foo                                    => Type=github, Directory=github/foo
            - npm/cli                                       => Type=github, Directory=npm/cli
            - git+npm/cli                                   => Type=git, Url = git+npm/cli, Directory=npm/cli
            - gist:somegistid                               => Type=gist, Url = gist:somegistid, Directory=somegistid
            - github:npm/exmaple                            => Type=github, Url = github:npm/example, Directory=npm/example
            - bitbucket:someuser/somerepo                   => Type=bitbucket, Url = bitbucket:someuser/somerepo, Directory=someuser/somerepo
            - /some/dir                                     => Directory=/some/dir
            - ./some/dir                                    => Directory=./some/dir
            - ../some/dir                                   => Directory=../some/dir
            - ~/some/dir                                    => Directory=~/some/dir
            - file://some/dir                               => Url=file://some/dir
            - (optional) url or type+url with "(directory)" suffix
        */

        string? raw = reader.GetString();
        if (string.IsNullOrWhiteSpace(raw))
        {
            return new()
            {
                Type = null,
                Url = null,
                Directory = null
            };
        }

        ReadOnlySpan<char> span = raw.AsSpan().Trim();

        // Check for directory suffix pattern: "url (directory)" or "url(directory)"
        string? directorySuffix = null;
        int parenIdx = span.LastIndexOf('(');
        if (parenIdx > 0 && span[^1] == ')')
        {
            directorySuffix = span.Slice(parenIdx + 1, span.Length - parenIdx - 2).Trim().ToString();
            span = span.Slice(0, parenIdx).Trim();
        }

        string? type = null;
        string? url = null;
        string? directory = directorySuffix;

        // LocalBuild paths: /some/dir, ./some/dir, ../some/dir, ~/some/dir
        if (span.StartsWith("/") || span.StartsWith("./") || span.StartsWith("../") || span.StartsWith("~/"))
        {
            directory = span.ToString();
        }
        // Shorthand patterns: gist:, github:, bitbucket:, gitlab:
        else if (span.Contains(':') && !span.Contains("//", StringComparison.Ordinal))
        {
            int colonIdx = span.IndexOf(':');
            string prefix = span.Slice(0, colonIdx).ToString().ToLowerInvariant();
            string suffix = span.Slice(colonIdx + 1).ToString();

            if (prefix is "gist" or "github" or "bitbucket" or "gitlab")
            {
                type = prefix;
                url = span.ToString();
                directory ??= suffix;
            }
            else
            {
                // Could be git@host:path or similar
                url = span.ToString();
            }
        }
        // git+protocol:// patterns
        else if (span.StartsWith("git+"))
        {
            type = "git";
            url = span.ToString();
        }
        // Full URLs: http://, https://, git://, ssh://, file://
        else if (span.Contains("://", StringComparison.Ordinal))
        {
            url = span.ToString();
        }
        // Simple shorthand like "owner/repo" -> assume github
        else if (span.Contains('/') && !span.Contains('\\'))
        {
            int slashCount = 0;
            foreach (char c in span)
            {
                if (c == '/') slashCount++;
            }

            // Single slash pattern like "npm/cli" or "github/foo" -> treat as github shorthand
            if (slashCount == 1)
            {
                type = "github";
                directory = span.ToString();
            }
            else
            {
                directory = span.ToString();
            }
        }
        else
        {
            // Fallback: treat as URL
            url = span.ToString();
        }

        return new NpmRepositoryRecord
        {
            Type = type,
            Url = url,
            Directory = directory
        };
    }

    private static NpmRepositoryRecord FromObject(ref Utf8JsonReader reader)
    {
        string? type = null;
        string? url = null;

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
                break;

            if (reader.TokenType != JsonTokenType.PropertyName)
                throw new JsonException("Expected property name in repository object.");

            string prop = reader.GetString() ?? string.Empty;
            if (!reader.Read())
                throw new JsonException("Unexpected end of object in repository entry.");

            switch (prop.ToLowerInvariant())
            {
                case "type":
                    {
                        if (reader.TokenType == JsonTokenType.String)
                        {
                            type = reader.GetString();
                        }
                        else if (reader.TokenType != JsonTokenType.Null)
                        {
                            reader.TrySkip();
                        }
                    }
                    break;

                case "url":
                    {
                        if (reader.TokenType == JsonTokenType.String)
                        {
                            url = reader.GetString();
                        }
                        else if (reader.TokenType != JsonTokenType.Null)
                        {
                            reader.TrySkip();
                        }
                    }
                    break;

                default:
                    reader.TrySkip();
                    break;
            }
        }

        return new NpmRepositoryRecord { Type = type, Url = url };
    }

    private static IReadOnlyList<NpmRepositoryRecord> FromArray(ref Utf8JsonReader reader)
    {
        List<NpmRepositoryRecord> list = [];
        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndArray)
                break;

            list.Add(reader.TokenType switch
            {
                JsonTokenType.String => FromString(ref reader),
                JsonTokenType.StartObject => FromObject(ref reader),
                _ => throw new JsonException("Array repository entries must be string or object.")
            });
        }
        return list;
    }
}
