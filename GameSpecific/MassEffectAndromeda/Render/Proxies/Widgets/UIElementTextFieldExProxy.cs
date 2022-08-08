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
    public class UIElementTextFieldExProxy : UIElementProxy
    {
        protected HorizontalAlignment horizontalAlignment;
        protected VerticalAlignment verticalAlignment;

        private FormattedText formattedText;
        private Typeface typeface;

        private string text = "";

        public UIElementTextFieldExProxy(Entities.UIElementTextFieldEntityEx inEntity)
            : base(inEntity)
        {
            switch (inEntity.Data.HorizonalAlignment)
            {
                case FrostySdk.Ebx.UIElementAlignment.UIElementAlignment_Left: horizontalAlignment = HorizontalAlignment.Left; break;
                case FrostySdk.Ebx.UIElementAlignment.UIElementAlignment_Right: horizontalAlignment = HorizontalAlignment.Right; break;
                case FrostySdk.Ebx.UIElementAlignment.UIElementAlignment_Center: horizontalAlignment = HorizontalAlignment.Center; break;
            }
            switch(inEntity.Data.VerticalAlignment)
            {
                case FrostySdk.Ebx.UIElementAlignment.UIElementAlignment_Top: verticalAlignment = VerticalAlignment.Top; break;
                case FrostySdk.Ebx.UIElementAlignment.UIElementAlignment_Center: verticalAlignment = VerticalAlignment.Center; break;
                case FrostySdk.Ebx.UIElementAlignment.UIElementAlignment_Bottom: verticalAlignment = VerticalAlignment.Bottom; break;
            }

            if (inEntity.FontStyle != null)
            {
                typeface = new Typeface(inEntity.FontStyle.FontFamily, FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);
                text = inEntity.Data.DebugText;

                if (inEntity.Data.LocalizedTextString.StringId != 0)
                {
                    text = LocalizedStringDatabase.Current.GetString((uint)inEntity.Data.LocalizedTextString.StringId);
                }

                formattedText = new FormattedText(text, CultureInfo.InvariantCulture, FlowDirection.LeftToRight, typeface, inEntity.FontStyle.FontSize, colorBrush, 96.0);
            }
            debugColor = Colors.Red;
        }

        public override void Update()
        {
            Color oldColor = color;

            base.Update();

            var textEntity = entity as Entities.UIElementTextFieldEntityEx;
            var currentText = text;

            switch (textEntity.Data.HorizonalAlignment)
            {
                case FrostySdk.Ebx.UIElementAlignment.UIElementAlignment_Left: horizontalAlignment = HorizontalAlignment.Left; break;
                case FrostySdk.Ebx.UIElementAlignment.UIElementAlignment_Right: horizontalAlignment = HorizontalAlignment.Right; break;
                case FrostySdk.Ebx.UIElementAlignment.UIElementAlignment_Center: horizontalAlignment = HorizontalAlignment.Center; break;
            }
            switch (textEntity.Data.VerticalAlignment)
            {
                case FrostySdk.Ebx.UIElementAlignment.UIElementAlignment_Top: verticalAlignment = VerticalAlignment.Top; break;
                case FrostySdk.Ebx.UIElementAlignment.UIElementAlignment_Center: verticalAlignment = VerticalAlignment.Center; break;
                case FrostySdk.Ebx.UIElementAlignment.UIElementAlignment_Bottom: verticalAlignment = VerticalAlignment.Bottom; break;
            }

            text = ParseHTML(textEntity.FieldText, textEntity.Data.ParseHTML);
            if (!currentText.Equals(text) || color != oldColor)
            {
                formattedText = null;
                if (textEntity.FontStyle != null)
                {
                    if (typeface?.FontFamily != textEntity.FontStyle.FontFamily)
                    {
                        typeface = new Typeface(textEntity.FontStyle.FontFamily, FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);
                    }
                    formattedText = new FormattedText(text, CultureInfo.InvariantCulture, FlowDirection.LeftToRight, typeface, textEntity.FontStyle.FontSize, colorBrush, 96.0);
                }
            }
        }

        public override void Render(DrawingContext drawingContext)
        {
            base.Render(drawingContext);

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

            drawingContext.PushOpacity(alpha);
            drawingContext.PushTransform(transform);
            if (formattedText != null)
            {
                drawingContext.DrawText(formattedText, textPosition);
            }
            drawingContext.Pop();
            drawingContext.Pop();
        }

        private string ParseHTML(string inText, bool parseHtml)
        {
            if (!parseHtml)
                return inText;

            while (inText.IndexOf('<') != -1)
            {
                int i = inText.IndexOf('<');
                int j = inText.IndexOf('>');
                inText = inText.Remove(i, j-i + 1);
            }

            return inText;
        }
    }
}
