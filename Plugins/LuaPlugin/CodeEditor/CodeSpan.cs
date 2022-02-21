using System.Collections.Generic;
using System.Windows.Media;
using System.Windows;

namespace LuaPlugin.CodeEditor
{
    public class CodeSpanStyle
    {
        public Brush FillBrush { get; }
        public bool Italic { get; }
        public bool Bold { get; }

        public CodeSpanStyle(Brush fillBrush, bool italic, bool bold)
        {
            FillBrush = fillBrush;
            Italic = italic;
            Bold = bold;
        }
    }

    public class CodeSpan
    {
        public CodeSpanStyle Style { get; }
        public GlyphRun Glyphs { get; }
        public string Text { get; }
        public double Width { get; }
        public double Height { get; }
        public List<Rect> CharacterRects { get; } = new List<Rect>();
        public int StartIndex { get; } // The index into the code that this span starts at

        public CodeSpan(CodeSpanStyle style, string text, int startIndex, double size)
        {
            Style = style;
            Text = text;

            Typeface tf = new Typeface(new FontFamily("Consolas"), style.Italic ? FontStyles.Italic : FontStyles.Normal, style.Bold ? FontWeights.Bold : FontWeights.Normal, FontStretches.Normal);
            tf.TryGetGlyphTypeface(out GlyphTypeface typeface);

            ushort[] indices = new ushort[text.Length];
            double[] advances = new double[text.Length];
            double x = 0;
            for (int i = 0; i < text.Length; i++)
            {
                if (typeface.CharacterToGlyphMap.ContainsKey(text[i]))
                {
                    indices[i] = typeface.CharacterToGlyphMap[text[i]];
                    advances[i] = typeface.AdvanceWidths[indices[i]] * size;
                    x += advances[i];
                    CharacterRects.Add(new Rect(x, 0, advances[i], typeface.Height * size));
                }
            }
            Width = x;
            Height = typeface.Height * size;
            StartIndex = startIndex;
            Glyphs = new GlyphRun(typeface, 0, false, size, 1.0f, indices, new Point(0, 0), advances, null, null, null, null, null, null);
        }

        // returns an index into the string or the string length or -1
        public int HitTest(double x)
        {
            int result = -1;
            for (int i = 0; i < Text.Length; i++)
            {
                if (x >= CharacterRects[i].Left && x <CharacterRects[i].Left + CharacterRects[i].Width * 0.5)
                {
                    return i;
                }
                if (x >= CharacterRects[i].Left + CharacterRects[i].Width * 0.5 && x < CharacterRects[i].Left + CharacterRects[i].Width)
                {
                    return i + 1;
                }
            }
            return result;
        }

        public void Render(DrawingContext drawingContext, double x, double baseline)
        {
            drawingContext.PushTransform(new TranslateTransform(x, baseline));
            drawingContext.DrawGlyphRun(Style.FillBrush, Glyphs);
            drawingContext.Pop();
        }
    }
}
