using System;
using System.Collections.Generic;

namespace LuaPlugin.CodeEditor
{
    public struct CodeIndentDelta
    {
        /// <summary>
        /// How many indents the line of code should be at relative to the previous line.
        /// </summary>
        public int PreIndentDelta;
        /// <summary>
        /// How many indents the following line of code should be at relative to the current line.
        /// </summary>
        public int PostIndentDelta;

        public CodeIndentDelta(int preIndentDelta, int postIndentDelta)
        {
            PreIndentDelta = preIndentDelta;
            PostIndentDelta = postIndentDelta;
        }

        public static CodeIndentDelta operator+(CodeIndentDelta left, CodeIndentDelta right)
        {
            return new CodeIndentDelta(left.PreIndentDelta + right.PreIndentDelta, left.PostIndentDelta + right.PostIndentDelta);
        }
    }

    public abstract class CodeFormatter
    {
        [Flags()]
        public enum TokenType
        {
            Whitespace = 1,
            Identifier = 2,
            Operator = 4,
            Quotes = 8,
            NoMerge = 16,
            Invalid = 32
        }

        public virtual void BeginDocument()
        {

        }

        public virtual void BeginLine()
        {

        }

        public abstract CodeSpanStyle FormatSpan(string text, int startchar, int length);

        public abstract TokenType GetCharacterTokenType(char c);

        public abstract CodeIndentDelta IndentationValue(string token);

        public virtual List<string> GetTokens(string line)
        {
            if (line.Length == 0)
                return new List<string>();

            int start = 0;
            List<string> result = new List<string>();
            CodeFormatter.TokenType tokenType = GetCharacterTokenType(line[0]);
            for (int endIndex = 1; endIndex < line.Length; endIndex++)
            {
                if ((GetCharacterTokenType(line[endIndex]) & tokenType) == 0 || GetCharacterTokenType(line[endIndex]).HasFlag(TokenType.NoMerge)) // Check if the last character and this character share any token types
                {
                    result.Add(line.Substring(start, endIndex - start));
                    start = endIndex;
                    tokenType = GetCharacterTokenType(line[endIndex]);
                }
            }
            if (start < line.Length)
            {
                result.Add(line.Substring(start));
            }
            return result;
        }
    }
}
