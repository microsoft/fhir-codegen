// <copyright file="ExportStreamWriter.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Microsoft.Health.Fhir.SpecManager.Language
{
    /// <summary>An extended StreamWriter implementation to simplify language export writing.</summary>
    public class ExportStreamWriter : StreamWriter
    {
        /// <summary>The current level of indentation.</summary>
        private int _indentation = 0;

        /// <summary>The indent character.</summary>
        private char _indentChar = ' ';

        /// <summary>The number of repetitions of the indent character per level.</summary>
        private int _charsPerLevel = 2;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExportStreamWriter"/> class.
        /// </summary>
        /// <param name="stream">The stream.</param>
        public ExportStreamWriter(Stream stream)
            : base(stream)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExportStreamWriter"/> class.
        /// </summary>
        /// <param name="path">Full pathname of the file.</param>
        public ExportStreamWriter(string path)
            : base(path)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExportStreamWriter"/> class.
        /// </summary>
        /// <param name="stream">  The stream.</param>
        /// <param name="encoding">The encoding.</param>
        public ExportStreamWriter(Stream stream, Encoding encoding)
            : base(stream, encoding)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExportStreamWriter"/> class.
        /// </summary>
        /// <param name="path">  Full pathname of the file.</param>
        /// <param name="append">True to append.</param>
        public ExportStreamWriter(string path, bool append)
            : base(path, append)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExportStreamWriter"/> class.
        /// </summary>
        /// <param name="stream">    The stream.</param>
        /// <param name="encoding">  The encoding.</param>
        /// <param name="bufferSize">Size of the buffer.</param>
        public ExportStreamWriter(Stream stream, Encoding encoding, int bufferSize)
            : base(stream, encoding, bufferSize)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExportStreamWriter"/> class.
        /// </summary>
        /// <param name="path">    Full pathname of the file.</param>
        /// <param name="append">  True to append.</param>
        /// <param name="encoding">The encoding.</param>
        public ExportStreamWriter(string path, bool append, Encoding encoding)
            : base(path, append, encoding)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExportStreamWriter"/> class.
        /// </summary>
        /// <param name="stream">    The stream.</param>
        /// <param name="encoding">  The encoding.</param>
        /// <param name="bufferSize">Size of the buffer.</param>
        /// <param name="leaveOpen"> True to leave open.</param>
        public ExportStreamWriter(Stream stream, Encoding encoding, int bufferSize, bool leaveOpen)
            : base(stream, encoding, bufferSize, leaveOpen)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExportStreamWriter"/> class.
        /// </summary>
        /// <param name="path">      Full pathname of the file.</param>
        /// <param name="append">    True to append.</param>
        /// <param name="encoding">  The encoding.</param>
        /// <param name="bufferSize">Size of the buffer.</param>
        public ExportStreamWriter(string path, bool append, Encoding encoding, int bufferSize)
            : base(path, append, encoding, bufferSize)
        {
        }

        /// <summary>Gets the current level of indentation.</summary>
        public int Indentation => _indentation;

        /// <summary>Gets the indentation character.</summary>
        public char IndentationChar => _indentChar;

        /// <summary>Gets the characters per indentation level.</summary>
        public int CharactersPerIndentation => _charsPerLevel;

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
                throw new Exception($"Cannot decrease indentation below: {_indentation}");
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

        /// <summary>Sets an indent.</summary>
        /// <param name="level">The level.</param>
        public void SetIndent(int level)
        {
            if (level < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(level));
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
                throw new ArgumentOutOfRangeException(nameof(charsPerIndentLevel));
            }

            _charsPerLevel = charsPerIndentLevel;
        }

        /// <summary>Writes a value, indented.</summary>
        /// <param name="value">The value.</param>
        public void WriteIndented(string value)
        {
            WriteIndentation();
            Write(value);
        }

        /// <summary>Writes a line, indented per current indentation level.</summary>
        /// <param name="value">The value.</param>
        public void WriteLineIndented(string value)
        {
            WriteIndentation();
            WriteLine(value);
        }

        /// <summary>Writes the indentation.</summary>
        private void WriteIndentation()
        {
            int count = _indentation * _charsPerLevel;

            if (count == 0)
            {
                return;
            }

            // optimize common value
            if (_indentChar == ' ')
            {
                switch (count)
                {
                    case 1:
                        Write($" ");
                        return;

                    case 2:
                        Write($"  ");
                        return;

                    case 3:
                        Write($"   ");
                        return;

                    case 4:
                        Write($"    ");
                        return;

                    case 5:
                        Write($"     ");
                        return;

                    case 6:
                        Write($"      ");
                        return;

                    case 7:
                        Write($"       ");
                        return;

                    case 8:
                        Write($"        ");
                        return;

                    case 9:
                        Write($"         ");
                        return;

                    case 10:
                        Write($"          ");
                        return;
                }
            }

            Write(new string(_indentChar, count));
        }
    }
}
