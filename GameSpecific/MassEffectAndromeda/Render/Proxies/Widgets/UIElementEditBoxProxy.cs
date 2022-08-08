using Frosty.Core;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
namespace LevelEditorPlugin.Render.Proxies
{
    public class UIElementEditBoxProxy : UIElementProxy
    {
        protected HorizontalAlignment horizontalAlignment;
        protected VerticalAlignment verticalAlignment;

        private Brush colorBrush;
        private FormattedText formattedText;

        private Brush barBrush;
        private float barThickness;

        public UIElementEditBoxProxy(Entities.UIElementEditBoxEntity inEntity)
            : base(inEntity)
        {
            switch (inEntity.Data.HorizontalAlignment)
            {
                case FrostySdk.Ebx.UIElementAlignment.UIElementAlignment_Left: horizontalAlignment = HorizontalAlignment.Left; break;
                case FrostySdk.Ebx.UIElementAlignment.UIElementAlignment_Right: horizontalAlignment = HorizontalAlignment.Right; break;
                case FrostySdk.Ebx.UIElementAlignment.UIElementAlignment_Center: horizontalAlignment = HorizontalAlignment.Center; break;
            }
            switch (inEntity.Data.VerticalAlignment)
            {
                case FrostySdk.Ebx.UIElementAlignment.UIElementAlignment_Top: verticalAlignment = VerticalAlignment.Top; break;
                case FrostySdk.Ebx.UIElementAlignment.UIElementAlignment_Center: verticalAlignment = VerticalAlignment.Center; break;
                case FrostySdk.Ebx.UIElementAlignment.UIElementAlignment_Bottom: verticalAlignment = VerticalAlignment.Bottom; break;
            }

            var typeface = new Typeface(inEntity.FontStyle.FontFamily, FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);
            string text = inEntity.Data.InstanceName;

            colorBrush = new SolidColorBrush(MakeColor(inEntity.Data.Color));
            formattedText = new FormattedText(text, CultureInfo.InvariantCulture, FlowDirection.LeftToRight, typeface, inEntity.FontStyle.FontSize, colorBrush, 96.0);

            barBrush = new SolidColorBrush(MakeColor(inEntity.Data.BarProperties.BarColor));
            barThickness = inEntity.Data.BarProperties.BarThickness;

            debugColor = Colors.Gold;
        }

        public override void Render(DrawingContext drawingContext)
        {
            base.Render(drawingContext);

            DrawText(drawingContext);
            DrawBar(drawingContext);
        }

        private void DrawBar(DrawingContext drawingContext)
        {
            Point barPosition = new Point(0, position.Y);
            switch (horizontalAlignment)
            {
                case HorizontalAlignment.Left: barPosition.X = position.X; break;
                case HorizontalAlignment.Center: barPosition.X = (size.Width * 0.5) - (barThickness * 0.5) + position.X; break;
                case HorizontalAlignment.Right: barPosition.X = size.Width - barThickness + position.X; break;
            }

            drawingContext.DrawRectangle(barBrush, null, new Rect(barPosition, new Size(barThickness, size.Height)));
        }

        private void DrawText(DrawingContext drawingContext)
        {
            Point textPosition = new Point();
            switch (horizontalAlignment)
            {
                case HorizontalAlignment.Left: textPosition.X = position.X; break;
                case HorizontalAlignment.Center: textPosition.X = (size.Width * 0.5) - (formattedText.Width * 0.5) + position.X; break;
                case HorizontalAlignment.Right: textPosition.X = size.Width - formattedText.Width + position.X; break;
            }
            switch (verticalAlignment)
            {
                case VerticalAlignment.Top: textPosition.Y = position.Y; break;
                case VerticalAlignment.Center: textPosition.Y = (size.Height * 0.5) - (formattedText.Height * 0.5) + position.Y; break;
                case VerticalAlignment.Bottom: textPosition.Y = size.Height - formattedText.Height + position.Y; break;
            }

            drawingContext.DrawText(formattedText, textPosition);
        }
    }
}
