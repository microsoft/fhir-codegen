// <copyright file="ExportStringBuilder.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System.Text;

namespace Microsoft.Health.Fhir.SpecManager.Language;

/// <summary>An export string builder.</summary>
public class ExportStringBuilder
{
    /// <summary>The current level of indentation.</summary>
    private int _indentation = 0;

    /// <summary>The indent character.</summary>
    private char _indentChar = ' ';

    /// <summary>The number of repetitions of the indent character per level.</summary>
    private int _charsPerLevel = 2;

    /// <summary>The string builder.</summary>
    private StringBuilder _sb;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExportStringBuilder"/> class.
    /// </summary>
    public ExportStringBuilder()
    {
        _sb = new();
    }

    /// <summary>Gets the current level of indentation.</summary>
    public int Indentation => _indentation;

    /// <summary>Gets the indentation character.</summary>
    public char IndentationChar => _indentChar;

    /// <summary>Gets the characters per indentation level.</summary>
    public int CharactersPerIndentation => _charsPerLevel;

    /// <summary>Returns a string that represents the current object.</summary>
    /// <returns>A string that represents the current object.</returns>
    public override string ToString() => _sb.ToString();

    /// <summary>Increase indent.</summary>
    public void IncreaseIndent()
    {
        _indentation++;
    }

    /// <summary>Opens a scope (writes literal on a line, then indents).</summary>
    /// <param name="openLiteral">(Optional) The scope open literal.</param>
    public void OpenScope(string openLiteral = "{")
    {
        WriteLineIndented(openLiteral);
        IncreaseIndent();
    }

    /// <summary>Decrease indent.</summary>
    public void DecreaseIndent()
    {
        if (_indentation <= 0)
        {
            throw new System.Exception($"Cannot decrease indentation below: {_indentation}");
        }

        _indentation--;
    }

    /// <summary>Closes a scope (decreases indent, then writes literal on a line).</summary>
    /// <param name="closeLiteral">(Optional) The close literal.</param>
    public void CloseScope(string closeLiteral = "}")
    {
        DecreaseIndent();
        WriteLineIndented(closeLiteral);
    }

    /// <summary>
    /// Reopens a scope (decreases indent, writes a literal, then increases indent).
    /// </summary>
    /// <param name="literal">The literal.</param>
    public void ReopenScope(string literal)
    {
        DecreaseIndent();
        WriteLineIndented(literal);
        IncreaseIndent();
    }

    /// <summary>Sets an indent.</summary>
    /// <param name="level">The level.</param>
    public void SetIndent(int level)
    {
        if (level < 0)
        {
            throw new System.ArgumentOutOfRangeException(nameof(level));
        }

        _indentation = level;
    }

    /// <summary>Sets indentation character.</summary>
    /// <param name="indentChar">The indent character.</param>
    public void SetIndentationChar(char indentChar)
    {
        _indentChar = indentChar;
    }

    /// <summary>Sets characters per indentation.</summary>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when one or more arguments are outside the
    ///  required range.</exception>
    /// <param name="charsPerIndentLevel">The characters per indent level.</param>
    public void SetCharsPerIndentation(int charsPerIndentLevel)
    {
        if (charsPerIndentLevel < 0)
        {
            throw new System.ArgumentOutOfRangeException(nameof(charsPerIndentLevel));
        }

        _charsPerLevel = charsPerIndentLevel;
    }

    /// <summary>Appends a sb.</summary>
    /// <param name="sb">The string builder.</param>
    public void Append(ExportStringBuilder sb)
    {
        _sb.Append(sb._sb.ToString());
    }

    /// <summary>Writes.</summary>
    /// <param name="value">The value.</param>
    public void Write(string value)
    {
        _sb.Append(value);
    }

    /// <summary>Writes a line.</summary>
    /// <param name="value">The value.</param>
    public void WriteLine(string value)
    {
        _sb.AppendLine(value);
    }

    /// <summary>Writes a value, indented.</summary>
    /// <param name="value">The value.</param>
    public void WriteIndented(string value)
    {
        int count = _indentation * _charsPerLevel;

        if (count == 0)
        {
            _sb.Append(value);
            return;
        }

        // optimize common value
        if (_indentChar == ' ')
        {
            switch (count)
            {
                case 1:
                    _sb.Append(" ");
                    _sb.Append(value);
                    return;

                case 2:
                    _sb.Append("  ");
                    _sb.Append(value);
                    return;

                case 3:
                    _sb.Append("   ");
                    _sb.Append(value);
                    return;

                case 4:
                    _sb.Append("    ");
                    _sb.Append(value);
                    return;

                case 5:
                    _sb.Append("     ");
                    _sb.Append(value);
                    return;

                case 6:
                    _sb.Append("      ");
                    _sb.Append(value);
                    return;

                case 7:
                    _sb.Append("       ");
                    _sb.Append(value);
                    return;

                case 8:
                    _sb.Append("        ");
                    _sb.Append(value);
                    return;

                case 9:
                    _sb.Append("         ");
                    _sb.Append(value);
                    return;

                case 10:
                    _sb.Append("          ");
                    _sb.Append(value);
                    return;
            }
        }

        _sb.Append(new string(_indentChar, count));
        _sb.Append(value);
    }

    /// <summary>Writes a value, indented.</summary>
    /// <param name="value">The value.</param>
    public void WriteLineIndented(string value)
    {
        int count = _indentation * _charsPerLevel;

        if (count == 0)
        {
            _sb.AppendLine(value);
            return;
        }

        // optimize common value
        if (_indentChar == ' ')
        {
            switch (count)
            {
                case 1:
                    _sb.Append(" ");
                    _sb.AppendLine(value);
                    return;

                case 2:
                    _sb.Append("  ");
                    _sb.AppendLine(value);
                    return;

                case 3:
                    _sb.Append("   ");
                    _sb.AppendLine(value);
                    return;

                case 4:
                    _sb.Append("    ");
                    _sb.AppendLine(value);
                    return;

                case 5:
                    _sb.Append("     ");
                    _sb.AppendLine(value);
                    return;

                case 6:
                    _sb.Append("      ");
                    _sb.AppendLine(value);
                    return;

                case 7:
                    _sb.Append("       ");
                    _sb.AppendLine(value);
                    return;

                case 8:
                    _sb.Append("        ");
                    _sb.AppendLine(value);
                    return;

                case 9:
                    _sb.Append("         ");
                    _sb.AppendLine(value);
                    return;

                case 10:
                    _sb.Append("          ");
                    _sb.AppendLine(value);
                    return;
            }
        }

        _sb.Append(new string(_indentChar, count));
        _sb.AppendLine(value);
    }
}
