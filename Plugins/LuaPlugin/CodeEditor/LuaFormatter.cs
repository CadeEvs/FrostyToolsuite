using System;
using System.Linq;
using System.Windows.Media;

namespace LuaPlugin.CodeEditor
{
    public class LuaFormatter : CodeFormatter
    {
        private static readonly string[] keywords = new string[] { "and", "break", "do", "else", "elseif", "end", "false", "for", "function", "if", "in", "local", "nil", "not", "or", "repeat", "return", "then", "true", "until", "while" };
        private static readonly char[] whitespaceChars = new char[] { ' ', '\n', '\t' };
        private static readonly char[] operatorChars = new char[] { '+', '-', '*', '/', '%', '^', '#', '=', '~', '<', '>', '(', ')', '{', '}', '[', ']', ';', ',', '.' };
        private static readonly char[] quoteChars = new char[] { '\'', '\"' };

        private static readonly Color KeywordColor = Color.FromRgb(86, 156, 214);
        private static readonly SolidColorBrush KeywordBrush = new SolidColorBrush(KeywordColor);
        private static readonly Color CommentColor = Color.FromRgb(87, 166, 74);
        private static readonly SolidColorBrush CommentBrush = new SolidColorBrush(CommentColor);
        private static readonly Color StringColor = Color.FromRgb(214, 157, 133);
        private static readonly SolidColorBrush StringBrush = new SolidColorBrush(StringColor);

        private bool IsInSingleLineComment = false;
        private bool IsInDoubleQuotes = false;
        private bool IsInSingleQuotes = false;
        private bool IsStringEscaped = false;
        private string LastSpan = "";
        private int LineSum = 0; // indentation so far within the line. This is so that 'do' and 'end' on the same line cancel each other out.

        public override void BeginDocument()
        {
            IsInSingleLineComment = false;
            IsInDoubleQuotes = false;
        }

        public override void BeginLine()
        {
            IsInSingleLineComment = false;
            IsInDoubleQuotes = false;
            LineSum = 0;
        }

        public override CodeSpanStyle FormatSpan(string text, int startchar, int length)
        {
            Brush b = Brushes.White;
            bool italics = false;
            bool bold = false;
            if (keywords.Contains(text))
                b = KeywordBrush;

            if (text.StartsWith("--"))
                IsInSingleLineComment = true;

            if (IsInSingleLineComment)
                b = CommentBrush;

            if (!IsInSingleLineComment)
            {
                if (text == "\"" && !IsStringEscaped && !IsInSingleQuotes)
                {
                    IsInDoubleQuotes = !IsInDoubleQuotes;
                    b = StringBrush;
                }
                if (text == "\'" && !IsStringEscaped && !IsInDoubleQuotes)
                {
                    IsInSingleQuotes = !IsInSingleQuotes;
                    b = StringBrush;
                }
                if (IsInDoubleQuotes || IsInSingleQuotes)
                {
                    b = StringBrush;

                    if (text == "\\" && !IsStringEscaped)
                    {
                        IsStringEscaped = true;
                    }
                    else
                    {
                        IsStringEscaped = false;
                    }
                }
            }
            LastSpan = text;
            return new CodeSpanStyle(b, italics, bold);
        }

        public override TokenType GetCharacterTokenType(char c)
        {
            if (c == '-')
                return TokenType.Operator;
            if (whitespaceChars.Contains(c))
                return TokenType.Whitespace;
            if (operatorChars.Contains(c))
                return TokenType.Operator | TokenType.NoMerge;
            if (Char.IsLetterOrDigit(c))
                return TokenType.Identifier;
            if (quoteChars.Contains(c))
                return TokenType.Quotes | TokenType.NoMerge;
            return TokenType.Invalid;
        }

        public override CodeIndentDelta IndentationValue(string token)
        {
            if (token.StartsWith("--"))
                IsInSingleLineComment = true;
            if (IsInSingleLineComment)
                return new CodeIndentDelta(0, 0);
            if (token == "function" || token == "do" || token == "then" || 
                token == "repeat")
            {
                LineSum++;
                return new CodeIndentDelta(0, 1);
            }
            if (token == "else")
                return new CodeIndentDelta(-1, 1);
            if (token == "elseif")
                return new CodeIndentDelta(-1, 0);
            if (token == "end" || token == "until")
                if (LineSum > 0)
                {
                    // cancel the previous indent
                    LineSum--;
                    return new CodeIndentDelta(0, -1);
                }
                else
                {
                    return new CodeIndentDelta(-1, 0);
                }
            return new CodeIndentDelta(0, 0);
        }
    }
}
