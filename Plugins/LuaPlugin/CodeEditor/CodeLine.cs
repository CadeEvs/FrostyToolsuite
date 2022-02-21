using System.Collections.Generic;
using System.Windows.Media;

namespace LuaPlugin.CodeEditor
{
    public class CodeLine
    {
        public double Baseline { get; }
        public int StartIndex { get; }
        public int Length => Text.Length;
        public string Text { get; }
        public double X { get; }
        public double Size { get; }
        public double Width { get; }

        public List<CodeSpan> Spans = new List<CodeSpan>();

        public CodeLine(string text, int startIndex, CodeFormatter formatter, double x, double baseline, double size)
        {
            Text = text;
            StartIndex = startIndex;
            X = x;
            Baseline = baseline;
            Size = size;
            if (Text.Length == 0)
                return;

            int start = 0;
            Width = 0;
            foreach (string token in formatter.GetTokens(text))
            {
                Spans.Add(new CodeSpan(formatter.FormatSpan(token, start, token.Length), token, start, size));
                Width += Spans[Spans.Count - 1].Width;
                start += token.Length;
            }
            /*CodeFormatter.TokenType tokenType = formatter.GetCharacterTokenType(Text[0]);
            for (int endIndex = 1; endIndex < Text.Length; endIndex++)
            {
                if ((formatter.GetCharacterTokenType(Text[endIndex]) & tokenType) == 0 || formatter.GetCharacterTokenType(Text[endIndex]).HasFlag(CodeFormatter.TokenType.NoMerge)) // Check if the last character and this character share any token types
                {
                    Spans.Add(new CodeSpan(formatter.FormatSpan(Text.Substring(start, endIndex - start), start, endIndex - start), Text.Substring(start, endIndex - start), startIndex + start, size));
                    start = endIndex;
                    tokenType = formatter.GetCharacterTokenType(Text[endIndex]);
                    Width += Spans[Spans.Count - 1].Width;
                }
            }
            if (start < Text.Length)
            {
                Spans.Add(new CodeSpan(formatter.FormatSpan(Text.Substring(start), start, Text.Length - start), Text.Substring(start), startIndex + start, size));
                Width += Spans[Spans.Count - 1].Width;
            }*/
        }

        public void Render(DrawingContext drawingContext)
        {
            double x = X;
            foreach (CodeSpan span in Spans)
            {
                span.Render(drawingContext, x, Baseline);
                x += span.Width;
            }
        }

        public CodeSpan SpanFromColumn(int column)
        {
            foreach (CodeSpan span in Spans)
            {
                if (span.StartIndex <= column && column < span.StartIndex + span.Text.Length)
                    return span;
            }
            return null;
        }
    }
}
