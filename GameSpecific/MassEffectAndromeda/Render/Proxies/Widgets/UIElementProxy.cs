using LevelEditorPlugin.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace LevelEditorPlugin.Render.Proxies
{
    public class UIElementProxy : WidgetProxy
    {
        public override Entity Entity => entity;

        protected UIElementEntity entity;

        public UIElementProxy(UIElementEntity inEntity)
        {
            entity = inEntity;
            position = entity.LayoutPosition;
            size = entity.LayoutSize;
            transform = MakeMatrixTransform(entity.GetTransform());
            alpha = entity.Data.Alpha;
            color = MakeColor(entity.Color);
            visible = entity.Data.Visible;

            mirrorScale = new ScaleTransform(1, 1);
            if (entity.Data.Size.x < 0)
            {
                mirrorScale.ScaleX = -1;
                position.X -= size.Width;
            }
            if (entity.Data.Size.y < 0)
            {
                mirrorScale.ScaleX = -1;
                position.Y -= size.Height;
            }

            colorBrush = new SolidColorBrush(color);
            debugColor = Colors.Aqua;
        }

        public override void Update()
        {
            position = entity.LayoutPosition;
            size = entity.LayoutSize;
            transform = MakeMatrixTransform(entity.GetTransform());
            alpha = entity.Alpha;
            visible = entity.Visible;

            color = MakeColor(entity.Color);
            if (color != (colorBrush as SolidColorBrush).Color)
            {
                colorBrush = new SolidColorBrush(color);
            }
        }

        public override void Render(DrawingContext drawingContext)
        {
            base.Render(drawingContext);
        }
    }
}
