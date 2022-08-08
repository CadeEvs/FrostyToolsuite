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
    public class UIWidgetsArtProxy : UIElementProxy
    {
        protected bool drawBackground;
        protected bool drawOutline;

        protected Brush backgroundBrush;
        protected Pen outlinePen;

        public UIWidgetsArtProxy(Entities.UIWidgetsArtEntity inEntity)
            : base(inEntity)
        {
            debugColor = Colors.Purple;

            var appearance = inEntity.Appearance;
            if (appearance is FrostySdk.Ebx.UIWidgetsVectorAppearance)
            {
                var vectorAppearance = appearance as FrostySdk.Ebx.UIWidgetsVectorAppearance;
                if (vectorAppearance.DrawBackground)
                {
                    backgroundBrush = new SolidColorBrush(MakeColor(vectorAppearance.Background.Color));
                }
                if (vectorAppearance.DrawOutline)
                {
                    outlinePen = new Pen(new SolidColorBrush(MakeColor(vectorAppearance.Outline.Color)), vectorAppearance.Outline.AntiAliasingWidth);
                }
            }
        }

        public override void Render(DrawingContext drawingContext)
        {
            base.Render(drawingContext);

            drawingContext.PushOpacity(alpha);
            drawingContext.PushTransform(transform);
            drawingContext.DrawRectangle(backgroundBrush, outlinePen, new Rect(position, size));
            drawingContext.Pop();
            drawingContext.Pop();
        }
    }
}
