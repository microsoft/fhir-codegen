using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Health.Fhir.CodeGenCommon.Extensions;
using Microsoft.Health.Fhir.CodeGenCommon.Utils;

namespace Microsoft.Health.Fhir.MappingLanguage;

internal static class AntlrUtils
{
    internal static List<string> BuildLiteralEnums()
    {
        HashSet<string> hs = [];
        List<string> lines = [];
        for (int i = 1; i < 104; i++)
        {
            string enumValue = FmlMappingParser.DefaultVocabulary.GetSymbolicName(i);

            if (string.IsNullOrEmpty(enumValue))
            {
                enumValue = $"FmlMappingParser.T__{i - 1}";
            }
            else
            {
                enumValue = $"FmlMappingParser.{enumValue}";
            }

            string value = FmlMappingParser.DefaultVocabulary.GetLiteralName(i);

            if (string.IsNullOrEmpty(value))
            {
                // force this to lower case so we can get a better name later
                value = FmlMappingParser.DefaultVocabulary.GetSymbolicName(i).ToLowerInvariant();
            }

            switch (value)
            {
                case null:
                    value = "Null";
                    break;
                case "';'":
                    value = "Semicolon";
                    break;

                case "'{'":
                    value = "OpenCurlyBracket";
                    break;

                case "'}'":
                    value = "CloseCurlyBracket";
                    break;

                case "'<<'":
                    value = "DoubleLessThan";
                    break;

                case "'>>'":
                    value = "DoubleGreaterThan";
                    break;

                case "'('":
                    value = "OpenParenthesis";
                    break;

                case "')'":
                    value = "CloseParenthesis";
                    break;

                case "'['":
                    value = "OpenSquareBracket";
                    break;

                case "']'":
                    value = "CloseSquareBracket";
                    break;

                case "'|'":
                    value = "Pipe";
                    break;

                case "','":
                    value = "Comma";
                    break;

                case "':'":
                    value = "Colon";
                    break;

                case "'.'":
                    value = "Dot";
                    break;

                case "'..'":
                    value = "DoubleDot";
                    break;

                case "'-'":
                    value = "Minus";
                    break;

                case "'+'":
                    value = "Plus";
                    break;

                case "'*'":
                    value = "Asterisk";
                    break;

                case "'/'":
                    value = "Slash";
                    break;

                case "'\\'":
                    value = "Backslash";
                    break;

                case "'%'":
                    value = "Percent";
                    break;

                case "'&'":
                    value = "Ampersand";
                    break;

                case "''":
                    value = "EmptySingleQuotedString";
                    break;

                case "ws":
                    value = "Whitespace";
                    break;

                case "'/// '":
                    value = "TripleSlash";
                    break;

                case "'->'":
                    value = "Arrow";
                    break;

                default:
                    {
                        if ((value.Length > 2) && value.StartsWith("'") && value.EndsWith("'"))
                        {
                            value = value[1..^1];
                        }
                    }
                    break;
            }

            FhirSanitizationUtils.SanitizeForCode(value, hs, out string name, out _);
            lines.Add($"{name.ToPascalCase()} = {enumValue},");
        }

        return lines;
    }
}
