using System.Diagnostics;
using System.Text;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;

namespace RPGMakerLibCoreV2.Storages.Dump.VXAce;

public static class Test
{
    interface ILookAheadBuffer
    {
        /// <summary>
        /// Gets a value indicating whether the end of the input reader has been reached.
        /// </summary>
        bool EndOfInput { get; }

        /// <summary>
        /// Gets the character at the specified offset.
        /// </summary>
        char Peek(int offset);

        /// <summary>
        /// Skips the next <paramref name="length"/> characters. Those characters must have been
        /// obtained first by calling the <see cref="Peek"/> method.
        /// </summary>
        void Skip(int length);
    }
    
    class StringLookAheadBuffer : ILookAheadBuffer
    {
        private readonly string value;

        public int Position { get; private set; }

        public StringLookAheadBuffer(string value)
        {
            this.value = value;
        }

        public int Length => value.Length;

        public bool EndOfInput => IsOutside(Position);

        public char Peek(int offset)
        {
            var index = Position + offset;
            return IsOutside(index) ? '\0' : value[index];
        }

        private bool IsOutside(int index)
        {
            return index >= value.Length;
        }

        public void Skip(int length)
        {
            if (length < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(length), "The length must be positive.");
            }
            Position += length;
        }
    }
    
    class CharacterAnalyzer<TBuffer> where TBuffer : class, ILookAheadBuffer
    {
        public CharacterAnalyzer(TBuffer buffer)
        {
            this.Buffer = buffer ?? throw new ArgumentNullException(nameof(buffer));
        }

        public TBuffer Buffer { get; }

        public bool EndOfInput => Buffer.EndOfInput;

        public char Peek(int offset)
        {
            return Buffer.Peek(offset);
        }

        public void Skip(int length)
        {
            Buffer.Skip(length);
        }

        public bool IsAlphaNumericDashOrUnderscore(int offset = 0)
        {
            var character = Buffer.Peek(offset);
            return
                (character >= '0' && character <= '9') ||
                (character >= 'A' && character <= 'Z') ||
                (character >= 'a' && character <= 'z') ||
                character == '_' ||
                character == '-';
        }

        public bool IsAscii(int offset = 0)
        {
            return Buffer.Peek(offset) <= '\x7F';
        }

        public bool IsPrintable(int offset = 0)
        {
            var character = Buffer.Peek(offset);
            return
                character == '\x9' ||
                character == '\xA' ||
                character == '\xD' ||
                (character >= '\x20' && character <= '\x7E') ||
                character == '\x85' ||
                (character >= '\xA0' && character <= '\xD7FF') ||
                (character >= '\xE000' && character <= '\xFFFD');
        }

        public bool IsDigit(int offset = 0)
        {
            var character = Buffer.Peek(offset);
            return character >= '0' && character <= '9';
        }

        public int AsDigit(int offset = 0)
        {
            return Buffer.Peek(offset) - '0';
        }

        public bool IsHex(int offset)
        {
            var character = Buffer.Peek(offset);
            return
                (character >= '0' && character <= '9') ||
                (character >= 'A' && character <= 'F') ||
                (character >= 'a' && character <= 'f');
        }

        public int AsHex(int offset)
        {
            var character = Buffer.Peek(offset);

            if (character <= '9')
            {
                return character - '0';
            }
            if (character <= 'F')
            {
                return character - 'A' + 10;
            }
            return character - 'a' + 10;
        }

        public bool IsSpace(int offset = 0)
        {
            return Check(' ', offset);
        }

        public bool IsZero(int offset = 0)
        {
            return Check('\0', offset);
        }

        public bool IsTab(int offset = 0)
        {
            return Check('\t', offset);
        }

        public bool IsWhite(int offset = 0)
        {
            return IsSpace(offset) || IsTab(offset);
        }

        public bool IsBreak(int offset = 0)
        {
            return Check("\r\n\x85\x2028\x2029", offset);
        }

        public bool IsCrLf(int offset = 0)
        {
            return Check('\r', offset) && Check('\n', offset + 1);
        }

        public bool IsBreakOrZero(int offset = 0)
        {
            return IsBreak(offset) || IsZero(offset);
        }

        public bool IsWhiteBreakOrZero(int offset = 0)
        {
            return IsWhite(offset) || IsBreakOrZero(offset);
        }

        public bool Check(char expected, int offset = 0)
        {
            return Buffer.Peek(offset) == expected;
        }

        public bool Check(string expectedCharacters, int offset = 0)
        {
            // Todo: using it this way doesn't break anything, it's not really wrong...
            Debug.Assert(expectedCharacters.Length > 1, "Use Check(char, int) instead.");

            var character = Buffer.Peek(offset);
            return expectedCharacters.IndexOf(character) != -1;
        }
    }
    
    private class ScalarData
    {
        public string Value = string.Empty;
        public bool IsMultiline;
        public bool IsFlowPlainAllowed;
        public bool IsBlockPlainAllowed;
        public bool IsSingleQuotedAllowed;
        public bool IsBlockAllowed;
        public bool HasSingleQuotes;
        public ScalarStyle Style;
    }
    
        public static void AnalyzeScalar(Scalar scalar)
    {
        ScalarData data = new();
            
            var value = scalar.Value;

            ScalarData scalarData = new(); 
            scalarData.Value = value;

            if (value.Length == 0)
            {
                if (scalar.Tag == "tag:yaml.org,2002:null")
                {
                    scalarData.IsMultiline = false;
                    scalarData.IsFlowPlainAllowed = false;
                    scalarData.IsBlockPlainAllowed = true;
                    scalarData.IsSingleQuotedAllowed = false;
                    scalarData.IsBlockAllowed = false;
                }
                else
                {
                    scalarData.IsMultiline = false;
                    scalarData.IsFlowPlainAllowed = false;
                    scalarData.IsBlockPlainAllowed = false;
                    scalarData.IsSingleQuotedAllowed = true;
                    scalarData.IsBlockAllowed = false;
                }
                return;
            }

            var flowIndicators = false;
            var blockIndicators = false;
            if (value.StartsWith("---", StringComparison.Ordinal) || value.StartsWith("...", StringComparison.Ordinal))
            {
                flowIndicators = true;
                blockIndicators = true;
            }

            var buffer = new CharacterAnalyzer<StringLookAheadBuffer>(new StringLookAheadBuffer(value));
            var preceededByWhitespace = true;
            var followedByWhitespace = buffer.IsWhiteBreakOrZero(1);

            var leadingSpace = false;
            var leadingBreak = false;
            var trailingSpace = false;
            var trailingBreak = false;
            var leadingQuote = false;

            var breakSpace = false;
            var spaceBreak = false;
            var previousSpace = false;
            var previousBreak = false;
            var lineOfSpaces = false;

            var lineBreaks = false;

            var specialCharacters = !ValueIsRepresentableInOutputEncoding(value);
            var singleQuotes = false;
            var linesOfSpaces = false;

            var isFirst = true;
            while (!buffer.EndOfInput)
            {
                if (isFirst)
                {
                    if (buffer.Check(@"#,[]{}&*!|>\""%@`'"))
                    {
                        flowIndicators = true;
                        blockIndicators = true;
                        leadingQuote = buffer.Check('\'');
                        singleQuotes |= buffer.Check('\'');
                    }

                    if (buffer.Check("?:"))
                    {
                        flowIndicators = true;
                        if (followedByWhitespace)
                        {
                            blockIndicators = true;
                        }
                    }

                    if (buffer.Check('-') && followedByWhitespace)
                    {
                        flowIndicators = true;
                        blockIndicators = true;
                    }
                }
                else
                {
                    if (buffer.Check(",?[]{}"))
                    {
                        flowIndicators = true;
                    }

                    if (buffer.Check(':'))
                    {
                        flowIndicators = true;
                        if (followedByWhitespace)
                        {
                            blockIndicators = true;
                        }
                    }

                    if (buffer.Check('#') && preceededByWhitespace)
                    {
                        flowIndicators = true;
                        blockIndicators = true;
                    }

                    singleQuotes |= buffer.Check('\'');
                }

                if (!specialCharacters && !buffer.IsPrintable())
                {
                    specialCharacters = true;
                }

                if (buffer.IsBreak())
                {
                    lineBreaks = true;
                }

                if (buffer.IsSpace())
                {
                    if (isFirst)
                    {
                        leadingSpace = true;
                    }

                    if (buffer.Buffer.Position >= buffer.Buffer.Length - 1)
                    {
                        trailingSpace = true;
                    }

                    if (previousBreak)
                    {
                        breakSpace = true;
                        lineOfSpaces = true;
                    }

                    previousSpace = true;
                    previousBreak = false;
                }
                else if (buffer.IsBreak())
                {
                    if (isFirst)
                    {
                        leadingBreak = true;
                    }

                    if (buffer.Buffer.Position >= buffer.Buffer.Length - 1)
                    {
                        trailingBreak = true;
                    }

                    if (previousSpace)
                    {
                        spaceBreak = true;
                    }

                    if (lineOfSpaces)
                    {
                        linesOfSpaces = true;
                    }

                    previousSpace = false;
                    previousBreak = true;
                }
                else
                {
                    previousSpace = false;
                    previousBreak = false;
                    lineOfSpaces = false;
                }

                preceededByWhitespace = buffer.IsWhiteBreakOrZero();
                buffer.Skip(1);
                if (!buffer.EndOfInput)
                {
                    followedByWhitespace = buffer.IsWhiteBreakOrZero(1);
                }

                isFirst = false;
            }

            scalarData.IsFlowPlainAllowed = true;
            scalarData.IsBlockPlainAllowed = true;
            scalarData.IsSingleQuotedAllowed = true;
            scalarData.IsBlockAllowed = true;

            if (leadingSpace || leadingBreak || trailingSpace || trailingBreak || leadingQuote)
            {
                scalarData.IsFlowPlainAllowed = false;
                scalarData.IsBlockPlainAllowed = false;
            }

            if (trailingSpace)
            {
                scalarData.IsBlockAllowed = false;
            }

            if (breakSpace)
            {
                scalarData.IsFlowPlainAllowed = false;
                scalarData.IsBlockPlainAllowed = false;
                scalarData.IsSingleQuotedAllowed = false;
            }

            if (spaceBreak || specialCharacters)
            {
                scalarData.IsFlowPlainAllowed = false;
                scalarData.IsBlockPlainAllowed = false;
                scalarData.IsSingleQuotedAllowed = false;
            }
            if (linesOfSpaces)
            {
                scalarData.IsBlockAllowed = false;
            }

            scalarData.IsMultiline = lineBreaks;
            if (lineBreaks)
            {
                scalarData.IsFlowPlainAllowed = false;
                scalarData.IsBlockPlainAllowed = false;
            }

            if (flowIndicators)
            {
                scalarData.IsFlowPlainAllowed = false;
            }

            if (blockIndicators)
            {
                scalarData.IsBlockPlainAllowed = false;
            }

            scalarData.HasSingleQuotes = singleQuotes;
        }

        static bool ValueIsRepresentableInOutputEncoding(string value)
        {
            // if (outputUsesUnicodeEncoding)
            {
                return true;
            }

            // try
            // {
            //     var encodedBytes = output.Encoding.GetBytes(value);
            //     var decodedString = output.Encoding.GetString(encodedBytes, 0, encodedBytes.Length);
            //     return decodedString.Equals(value);
            // }
            // catch (EncoderFallbackException)
            // {
            //     return false;
            // }
            // catch (ArgumentOutOfRangeException)
            // {
            //     return false;
            // }
        }
}