using Frosty.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace LuaPlugin.CodeEditor
{
    public class CodeEditor : FrameworkElement
    {
        private bool NoFormat = false;
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(CodeEditor), new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnTextChanged)));
        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value.Replace("\r", "").Replace("\t", new StringBuilder(TabSize).Insert(0, " ", TabSize).ToString()));
        }
        public static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!((CodeEditor)d).NoFormat)
                ((CodeEditor)d)?.FormatText();
        }

        public static readonly DependencyProperty BackgroundProperty = DependencyProperty.Register("Background", typeof(Brush), typeof(CodeEditor), new FrameworkPropertyMetadata(new SolidColorBrush(Color.FromRgb(35, 35, 35)), FrameworkPropertyMetadataOptions.AffectsRender));
        public Brush Background
        {
            get => (Brush)GetValue(BackgroundProperty);
            set => SetValue(BackgroundProperty, value);
        }

        public static readonly DependencyProperty SelectionProperty = DependencyProperty.Register("Selection", typeof(Brush), typeof(CodeEditor), new FrameworkPropertyMetadata(new SolidColorBrush(Color.FromArgb(100, 50, 100, 250)), FrameworkPropertyMetadataOptions.AffectsRender));
        public Brush Selection
        {
            get => (Brush)GetValue(SelectionProperty);
            set => SetValue(SelectionProperty, value);
        }

        public static readonly DependencyProperty GutterProperty = DependencyProperty.Register("Gutter", typeof(Brush), typeof(CodeEditor), new FrameworkPropertyMetadata(new SolidColorBrush(Color.FromRgb(47, 47, 47)), FrameworkPropertyMetadataOptions.AffectsRender));
        public Brush Gutter
        {
            get => (Brush)GetValue(GutterProperty);
            set => SetValue(GutterProperty, value);
        }

        public static readonly DependencyProperty GutterWidthProperty = DependencyProperty.Register("GutterWidth", typeof(double), typeof(CodeEditor), new FrameworkPropertyMetadata(50.0, FrameworkPropertyMetadataOptions.AffectsParentMeasure));
        public double GutterWidth
        {
            get => (double)GetValue(GutterWidthProperty);
            set => SetValue(GutterWidthProperty, value);
        }

        public static readonly DependencyProperty LineNumbersProperty = DependencyProperty.Register("LineNumbers", typeof(Brush), typeof(CodeEditor), new FrameworkPropertyMetadata(new SolidColorBrush(Color.FromRgb(200, 200, 200)), FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnLineNumbersChanged)));
        public Brush LineNumbers
        {
            get => (Brush)GetValue(LineNumbersProperty);
            set => SetValue(LineNumbersProperty, value);
        }
        public static void OnLineNumbersChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as CodeEditor)?.LineNumbersChanged();
        }
        public void LineNumbersChanged()
        {
            LineNumbersRenderText?.SetForegroundBrush(LineNumbers);
        }

        private CodeFormatter _codeFormatter = new LuaFormatter();
        public CodeFormatter CodeFormatter
        {
            get => _codeFormatter;
            set
            {
                _codeFormatter = value;
                FormatText();
            }
        }
        public CodeLocation SelectionStart { get; private set; }
        public CodeLocation SelectionEnd { get; private set; }
        public int LineCount { get; private set; }

        private FormattedText LineNumbersRenderText = new FormattedText("1\n2\n3\n4\n5\n6\n7\n8", System.Globalization.CultureInfo.CurrentCulture, FlowDirection.RightToLeft, new Typeface("Consolas"), 12, Brushes.White, 1.0);
        private List<CodeLine> Lines = new List<CodeLine>();

        private readonly Typeface Typeface = new Typeface("Consolas");

        private static Brush SelectionUnfocused = new SolidColorBrush(Color.FromArgb(100, 110, 110, 110));
        private bool CaretBlink = true;
        private System.Windows.Threading.DispatcherTimer Timer;

        public bool IndentOnEnter = true;
        public int TabSize = 4;

        public event RoutedEventHandler TextChanged;

        public CodeEditor()
        {
            SelectionStart = new CodeLocation(0, 0, 0);
            SelectionEnd = new CodeLocation(0, 0, 0);
            Focusable = true;
            Timer = new System.Windows.Threading.DispatcherTimer();
            Timer.Tick += Timer_Tick;
            Timer.Interval = new TimeSpan(5000000);
            Timer.Start();
            IndentOnEnter = Config.Get("TextEditorIndentOnEnter", true);
            TabSize = Config.Get("TextEditorTabSize", 4);
            Config.Add("TextEditorIndentOnEnter", IndentOnEnter);
            Config.Add("TextEditorTabSize", TabSize);
            Config.Save();

            //IndentOnEnter = Config.Get("TextEditor", "IndentOnEnter", true);
            //TabSize = Config.Get("TextEditor", "TabSize", 4);
            //Config.Add("TextEditor", "IndentOnEnter", IndentOnEnter);
            //Config.Add("TextEditor", "TabSize", TabSize);
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            CaretBlink = !CaretBlink;
            InvalidateVisual();
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            if (RenderSize.Width == 0 || RenderSize.Height == 0) return;

            // HACK: Make sure the caret is in a valid location
            SelectionEnd = IndexLocation(Math.Min(Math.Max(0, Text.Length), SelectionEnd.Index));
            SelectionStart = IndexLocation(Math.Min(Math.Max(0, Text.Length), SelectionStart.Index));

            // Draw gutter
            drawingContext.DrawRectangle(Gutter, null, new Rect(0, 0, GutterWidth, RenderSize.Height));
            drawingContext.DrawRectangle(Background, null, new Rect(GutterWidth, 0, RenderSize.Width - GutterWidth, RenderSize.Height));

            // Draw line numbers
            drawingContext.DrawText(LineNumbersRenderText, new Point(GutterWidth - 5, 3));

            // Draw lines of code
            foreach (CodeLine line in Lines)
            {
                line.Render(drawingContext);
            }

            if (SelectionStart.Index == SelectionEnd.Index)
            {
                if (CaretBlink)
                {
                    // Draw start of selection (caret) (kinda hacky) HACK 1: Add " " to the end of each line so we can put the caret to the right of the last character.
                    double x = GutterWidth + 5 + new FormattedText((Lines[SelectionStart.Line].Text + " ").Substring(0, SelectionStart.Column), System.Globalization.CultureInfo.CurrentCulture, FlowDirection.LeftToRight, Typeface, 12, Brushes.White, 1.0).WidthIncludingTrailingWhitespace;
                    drawingContext.DrawRectangle(Brushes.White, null, new Rect(x, Lines[SelectionStart.Line].Baseline - 12, 1.5, 14));
                }
            }
            else
            {
                // Hilight the selected lines
                int lstart = SelectionStart.Line <= SelectionEnd.Line ? SelectionStart.Line : SelectionEnd.Line;
                int lend = SelectionStart.Line <= SelectionEnd.Line ? SelectionEnd.Line : SelectionStart.Line;
                for (int i = lstart; i <= lend; i++)
                {
                    int cstart = i == lstart ? (SelectionStart.Index <= SelectionEnd.Index ? SelectionStart.Column : SelectionEnd.Column) : 0;
                    int cend = i == lend ? (SelectionStart.Index <= SelectionEnd.Index ? SelectionEnd.Column : SelectionStart.Column) : Lines[i].Length;
                    double x = 0;
                    if (cstart > 0)
                        x = GutterWidth + 5 + new FormattedText((Lines[i].Text + " ").Substring(0, cstart), System.Globalization.CultureInfo.CurrentCulture, FlowDirection.LeftToRight, Typeface, 12, Brushes.White, 1.0).WidthIncludingTrailingWhitespace;
                    double width = new FormattedText((Lines[i].Text + " ").Substring(cstart, cend - cstart), System.Globalization.CultureInfo.CurrentCulture, FlowDirection.LeftToRight, Typeface, 12, Brushes.White, 1.0).WidthIncludingTrailingWhitespace;
                    if (cstart == 0)
                        width += GutterWidth + 5;
                    drawingContext.DrawRectangle(IsFocused ? Selection : SelectionUnfocused, null, new Rect(x, Lines[i].Baseline - 12, width, 14));
                }

            }

            // Draw end of selection
            //x = GutterWidth + 5 + new FormattedText((Lines[SelectionEnd.Line].Text + " ").Substring(0, SelectionEnd.Column), System.Globalization.CultureInfo.CurrentCulture, FlowDirection.LeftToRight, Typeface, 12, Brushes.White).WidthIncludingTrailingWhitespace;
            //drawingContext.DrawRectangle(Brushes.Red, null, new Rect(x + 1, Lines[SelectionEnd.Line].Baseline - 12, 1, 14));
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            Size s = availableSize;
            if (s.Width == double.PositiveInfinity)
                s.Width = 1;
            if (s.Height == double.PositiveInfinity)
                s.Height = LineNumbersRenderText.Height + 6;
            else if (s.Height < LineNumbersRenderText.Height + 6)
                s.Height = LineNumbersRenderText.Height + 6;

            foreach (CodeLine l in Lines)
            {
                if (l.Width + GutterWidth + 5 > s.Width)
                    s.Width = GutterWidth + 5 + l.Width;
            }

            return s;
        }

        protected override void OnTextInput(TextCompositionEventArgs e)
        {
            base.OnTextInput(e);
            if (e.Text.Length == 0) // In such cases as keyboard shortcuts
                return;

            string Text = e.Text;

            CaretBlink = true;
            // Only use newlines, not carriage returns
            if (Text == "\r")
                Text = "\n";
            
            if (Text == "\b")
            {
                if (SelectionStart.Index != SelectionEnd.Index)
                {
                    // Remove the characters between SelectionStart and SelectionEnd
                    ModifyText(SelectionStart.Index, SelectionEnd.Index, "");
                }
                else
                {
                    // Remove the character at SelectionStart - 1 if SelectionStart.Index > 0
                    if (SelectionStart.Index > 0)
                    {
                        ModifyText(SelectionStart.Index - 1, SelectionStart.Index, "");
                    }
                }
            }
            else if (Text == "\n")
            {
                if (SelectionStart.Index == SelectionEnd.Index && !IndentOnEnter)
                {
                    int indent = Lines[SelectionStart.Line].Text.Length - Lines[SelectionStart.Line].Text.TrimStart().Length;
                    ModifyText(SelectionStart.Index, SelectionEnd.Index, Text + new StringBuilder(indent).Insert(0, " ", indent).ToString());
                }
                else
                {
                    ModifyText(SelectionStart.Index, SelectionEnd.Index, Text);
                    if (IndentOnEnter)
                        IndentText();
                }
            }
            else
            {
                ModifyText(SelectionStart.Index, SelectionEnd.Index, Text);
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (e.Key == Key.Delete)
            {
                CaretBlink = true;
                if (SelectionStart.Index == SelectionEnd.Index)
                {
                    if (SelectionStart.Index < Text.Length)
                    {
                        ModifyText(SelectionStart.Index, SelectionStart.Index + 1, "");
                    }
                }
                else
                {
                    ModifyText(SelectionStart.Index, SelectionEnd.Index, "");
                }
            }
            else if (e.Key == Key.Left)
            {
                CaretBlink = true;
                if (Keyboard.Modifiers == ModifierKeys.Shift)
                {
                    if (SelectionEnd.Index > 0)
                        SelectionEnd = IndexLocation(SelectionEnd.Index - 1);
                    InvalidateVisual();
                }
                else
                {
                    if (SelectionStart.Index != SelectionEnd.Index)
                    {
                        if (SelectionEnd.Index > SelectionStart.Index)
                            SelectionEnd = SelectionStart;
                        else
                            SelectionStart = SelectionEnd;
                        InvalidateVisual();
                    }
                    else if (SelectionStart.Index > 0)
                    {
                        SelectionStart = IndexLocation(SelectionStart.Index - 1);
                        SelectionEnd = SelectionStart;
                        InvalidateVisual();
                    }
                }
            }
            else if (e.Key == Key.Right)
            {
                CaretBlink = true;
                if (Keyboard.Modifiers == ModifierKeys.Shift)
                {
                    if (SelectionEnd.Index < Text.Length - 1)
                        SelectionEnd = IndexLocation(SelectionEnd.Index + 1);
                    InvalidateVisual();
                }
                else
                {
                    if (SelectionStart.Index != SelectionEnd.Index)
                    {
                        if (SelectionEnd.Index < SelectionStart.Index)
                            SelectionEnd = SelectionStart;
                        else
                            SelectionStart = SelectionEnd;
                        InvalidateVisual();
                    }
                    else if (SelectionStart.Index < Text.Length)
                    {
                        SelectionStart = IndexLocation(SelectionStart.Index + 1);
                        SelectionEnd = SelectionStart;
                        InvalidateVisual();
                    }
                }
            }
            else if (e.Key == Key.Up)
            {
                SelectionStart = SelectionEnd;
                if (SelectionStart.Line > 0)
                {
                    int line = SelectionStart.Line - 1;
                    int column = SelectionStart.Column;
                    if (column > Lines[line].Length)
                        column = Lines[line].Length;
                    SelectionStart = new CodeLocation(Lines[line].StartIndex + column, line, column);
                    SelectionEnd = SelectionStart;
                    if (IndentOnEnter)
                        IndentText();
                }
                CaretBlink = true;
                InvalidateVisual();
            }
            else if (e.Key == Key.Down)
            {
                SelectionStart = SelectionEnd;
                if (SelectionStart.Line < Lines.Count - 1)
                {
                    int line = SelectionStart.Line + 1;
                    int column = SelectionStart.Column;
                    if (column > Lines[line].Length)
                        column = Lines[line].Length;
                    SelectionStart = new CodeLocation(Lines[line].StartIndex + column, line, column);
                    SelectionEnd = SelectionStart;
                    if (IndentOnEnter)
                        IndentText();
                }
                CaretBlink = true;
                InvalidateVisual();
            }
            else if (e.Key == Key.Tab)
            {
                if (SelectionStart.Index == SelectionEnd.Index)
                {
                    if (e.KeyboardDevice.Modifiers != ModifierKeys.Shift)
                    {
                        if (Lines[SelectionStart.Line].Text.Substring(0, SelectionStart.Column).Trim().Length == 0)
                        {
                            // Align to the next increment of TabSize spaces
                            int shift = TabSize - (Lines[SelectionStart.Line].Text.Substring(0, SelectionStart.Column).Length % TabSize);
                            ModifyText(Lines[SelectionStart.Line].StartIndex, Lines[SelectionStart.Line].StartIndex, new StringBuilder(shift).Insert(0, " ", shift).ToString());
                        }
                        else
                        {
                            ModifyText(SelectionStart.Index, SelectionEnd.Index, new StringBuilder(TabSize).Insert(0, " ", TabSize).ToString());
                            CaretBlink = true;
                            InvalidateVisual();
                        }
                    }
                }
                else
                {
                    // Indent or de-indent the selected lines
                    int lstart = SelectionStart.Line <= SelectionEnd.Line ? SelectionStart.Line : SelectionEnd.Line;
                    int lend = SelectionStart.Line <= SelectionEnd.Line ? SelectionEnd.Line : SelectionStart.Line;
                    for (int i = lstart; i <= lend; i++)
                    {
                        if (e.KeyboardDevice.Modifiers == ModifierKeys.Shift)
                        {
                            // Align to the previous increment of TabSize spaces
                            int shift = ((Lines[i].Length - Lines[i].Text.TrimStart().Length) % TabSize);
                            if (shift == 0)
                                shift = TabSize;
                            ModifyText(Lines[i].StartIndex, Lines[i].StartIndex + shift, "");
                        }
                        else
                        {
                            // Align to the next increment of TabSize spaces
                            int shift = TabSize - ((Lines[i].Length - Lines[i].Text.TrimStart().Length) % TabSize);
                            ModifyText(Lines[i].StartIndex, Lines[i].StartIndex, new StringBuilder(shift).Insert(0, " ", shift).ToString());
                        }
                    }
                }
            }
            else if (e.Key == Key.X && Keyboard.Modifiers == ModifierKeys.Control)
            {
                // Cut selection
                int imin = SelectionStart.Index <= SelectionEnd.Index ? SelectionStart.Index : SelectionEnd.Index;
                int imax = SelectionStart.Index <= SelectionEnd.Index ? SelectionEnd.Index : SelectionStart.Index;
                Clipboard.SetText(Text.Substring(imin, imax - imin));
                ModifyText(imin, imax, "");
                CaretBlink = true;
                if (IndentOnEnter)
                    IndentText();
                InvalidateVisual();
            }
            else if (e.Key == Key.C && Keyboard.Modifiers == ModifierKeys.Control)
            {
                // Copy selection
                int imin = SelectionStart.Index <= SelectionEnd.Index ? SelectionStart.Index : SelectionEnd.Index;
                int imax = SelectionStart.Index <= SelectionEnd.Index ? SelectionEnd.Index : SelectionStart.Index;
                Clipboard.SetText(Text.Substring(imin, imax - imin));
                CaretBlink = true;
                InvalidateVisual();
            }
            else if (e.Key == Key.V && Keyboard.Modifiers == ModifierKeys.Control)
            {
                // Paste selection
                int imin = SelectionStart.Index <= SelectionEnd.Index ? SelectionStart.Index : SelectionEnd.Index;
                int imax = SelectionStart.Index <= SelectionEnd.Index ? SelectionEnd.Index : SelectionStart.Index;
                ModifyText(imin, imax, Clipboard.GetText());
                CaretBlink = true;
                if (IndentOnEnter)
                    IndentText();
                InvalidateVisual();
            }
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            Keyboard.Focus(this);
            Focus();
            Point p = e.GetPosition(this);
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                SelectionStart = PointLocation(p.X, p.Y);
                SelectionEnd = SelectionStart;
                CaretBlink = true;
                InvalidateVisual();
            }
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);
            Point p = e.GetPosition(this);
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                SelectionEnd = PointLocation(p.X, p.Y);
                if (p.X <= GutterWidth + 5)
                {
                    SelectionEnd = IndexLocation(Lines[SelectionEnd.Line].StartIndex + Lines[SelectionEnd.Line].Length);
                }
                CaretBlink = true;
                InvalidateVisual();
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            Point p = e.GetPosition(this);
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                SelectionEnd = PointLocation(p.X, p.Y);
                if (p.X <= GutterWidth + 5)
                {
                    SelectionEnd = IndexLocation(Lines[SelectionEnd.Line].StartIndex + Lines[SelectionEnd.Line].Length);
                }
                CaretBlink = true;
                InvalidateVisual();
            }
            if (p.X <= GutterWidth)
                Cursor = Cursors.Arrow;
            else
                Cursor = Cursors.IBeam;
        }

        public void IndentText()
        {
            if (CodeFormatter == null || Text.Length == 0)
                return;

            string[] lines = Text.Split('\n');

            CodeFormatter.BeginDocument();
            int indent = 0;
            for (int j = 0; j < lines.Length; j++)
            {
                List<string> tokens = CodeFormatter.GetTokens(lines[j]);
                CodeIndentDelta delta = new CodeIndentDelta();
                CodeFormatter.BeginLine();
                bool used = false; // for finding the first non-whitespace token
                int preIndented = 0;
                foreach (string token in tokens)
                {
                    delta += CodeFormatter.IndentationValue(token);
                    if (!used && !String.IsNullOrWhiteSpace(token))
                    {
                        indent += preIndented = CodeFormatter.IndentationValue(token).PreIndentDelta;
                        used = true;
                    }
                }

                if (indent < 0)
                    indent = 0;

                int startspace = lines[j].Length - lines[j].TrimStart().Length;
                int deltaspace = indent * TabSize - startspace;
                if (SelectionStart.Line == j)
                    SelectionStart = new CodeLocation(SelectionStart.Index + deltaspace, SelectionStart.Line, SelectionStart.Column + deltaspace);
                if (SelectionEnd.Line == j)
                    SelectionEnd = new CodeLocation(SelectionEnd.Index + deltaspace, SelectionEnd.Line, SelectionEnd.Column + deltaspace);
                lines[j] = new StringBuilder(indent * TabSize).Insert(0, " ", indent * TabSize).ToString() + lines[j].TrimStart();

                indent += delta.PostIndentDelta + delta.PreIndentDelta - preIndented;
                if (indent < 0)
                    indent = 0;
            }
            NoFormat = true;
            Text = String.Join("\n", lines);
            NoFormat = false;
            FormatText();
        }

        public void FormatText()
        {
            if (CodeFormatter == null || Text.Length == 0)
                return;

            string[] lines = Text.Split('\n');

            double baseline = 14; // pixels from the top
            int position = 0;
            int i = 0;
            string linenumbers = "";
            Lines.Clear();
            CodeFormatter.BeginDocument();
            foreach (string line in lines)
            {
                CodeFormatter.BeginLine();
                Lines.Add(new CodeLine(line, position, CodeFormatter, GutterWidth + 5, baseline, 12));
                position += line.Length + 1; // add the newline
                baseline += 14;
                linenumbers += ++i + "\n";
            }
            LineNumbersRenderText = new FormattedText(linenumbers, System.Globalization.CultureInfo.CurrentCulture, FlowDirection.RightToLeft, Typeface, 12, LineNumbers, 1.0);
            LineCount = Lines.Count;

            if (SelectionStart.Index > Text.Length)
                SelectionStart = IndexLocation(Text.Length);
            if (SelectionEnd.Index > Text.Length)
                SelectionEnd = IndexLocation(Text.Length);
        }

        public void ModifyText(int startIndex, int endIndex, string insertText)
        {
            if (startIndex > endIndex)
            {
                int i = startIndex;
                startIndex = endIndex;
                endIndex = i;
            }

            NoFormat = true;
            Text = Text.Substring(0, startIndex) + insertText + Text.Substring(endIndex);
            FormatText();
            NoFormat = false;
            if (SelectionStart.Index > startIndex && SelectionStart.Index < endIndex)
                SelectionStart = IndexLocation(startIndex + insertText.Length);
            else if (SelectionStart.Index >= endIndex)
                SelectionStart = IndexLocation(SelectionStart.Index - (endIndex - startIndex) + insertText.Length);

            if (SelectionEnd.Index > startIndex && SelectionEnd.Index < endIndex)
                SelectionEnd = IndexLocation(startIndex + insertText.Length);
            else if (SelectionEnd.Index >= endIndex)
                SelectionEnd = IndexLocation(SelectionEnd.Index - (endIndex - startIndex) + insertText.Length);
            FormatText();

            TextChanged?.Invoke(this, new RoutedEventArgs());
        }

        public CodeLocation IndexLocation(int index)
        {
            if (Lines.Count == 0)
            {
                return new CodeLocation(0, 0, 0);
            }
            int i = 0;
            foreach (CodeLine line in Lines)
            {
                if (line.StartIndex <= index && line.StartIndex + line.Length >= index)
                    return new CodeLocation(index, i, index - line.StartIndex);
                i++;
            }
            return new CodeLocation(Lines[Lines.Count - 1].StartIndex + Lines[Lines.Count - 1].Length, Lines.Count - 1, Lines[Lines.Count - 1].Length);
        }

        public CodeLocation PointLocation(double x, double y)
        {
            int line = (int)Math.Floor(y / 14.0);
            if (line >= Lines.Count)
                line = Lines.Count - 1;

            int column = 0;
            // HACK: treat the column size as the average character size
            if (Lines[line].Width > 0 && Lines[line].Length > 0)
            {
                double charWidth = Lines[line].Width / Lines[line].Length;
                column = (int)Math.Round((x - GutterWidth - 5) / charWidth);
                if (column >= Lines[line].Length)
                    column = Lines[line].Length;
                if (column < 0)
                    column = 0;
            }
            /*double offset = GutterWidth + 5;
            foreach (CodeSpan span in Lines[line].Spans)
            {
                int index = span.HitTest(x - offset);
                if (index > -1)
                {
                    column = span.StartIndex + index;
                    break;
                }
                offset += span.Width;
            }*/

            return new CodeLocation(column + Lines[line].StartIndex, line, column);
        }

        public void SelectText(int startIndex, int endIndex)
        {
            SelectionStart = IndexLocation(startIndex);
            SelectionEnd = IndexLocation(endIndex);
            InvalidateVisual();
        }
    }
}
