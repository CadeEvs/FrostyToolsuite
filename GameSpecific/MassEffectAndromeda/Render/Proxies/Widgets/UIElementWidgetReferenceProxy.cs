using LevelEditorPlugin.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace LevelEditorPlugin.Render.Proxies
{
    public class UIElementWidgetReferenceProxy : WidgetProxy
    {
        public override Entity Entity => entity;

        protected UIElementWidgetReferenceEntity entity;

        public UIElementWidgetReferenceProxy(UIElementWidgetReferenceEntity inEntity)
        {
            entity = inEntity;
            position = entity.LayoutPosition;
            size = entity.LayoutSize;
            alpha = entity.Data.Alpha;
            color = MakeColor(entity.Data.Color);

            transform = MakeMatrixTransform(entity.GetTransform());
            debugColor = Colors.Yellow;
        }

        public override void Update()
        {
            position = entity.LayoutPosition;
            size = entity.LayoutSize;
            transform = MakeMatrixTransform(entity.GetTransform());
            alpha = entity.Alpha;
            visible = entity.Visible;
        }

        public override void Render(DrawingContext drawingContext)
        {
            base.Render(drawingContext);
        }
    }
}
