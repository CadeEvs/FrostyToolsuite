using LevelEditorPlugin.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace LevelEditorPlugin.Controls
{
    public class SchematicsPreviewControl : Control
    {
        private ILogicEntity previewEntity;
        private NodeVisual previewEntityVisual;

        public SchematicsPreviewControl(ILogicEntity inEntity)
        {
            var largeFont = FontData.MakeFont(new Typeface("Consolas"), 10);

            previewEntity = inEntity;
            previewEntityVisual = new NodeVisual(previewEntity, 0, 0) { IsCollapsed = false, GlyphWidth = largeFont.AdvanceWidth };
            previewEntityVisual.Update();

            Background = null;
            IsHitTestVisible = false;
            Focusable = false;
            IsEnabled = false;
            Width = previewEntityVisual.Rect.Width;
            Height = previewEntityVisual.Rect.Height;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            previewEntityVisual.Render(SchematicsCanvas.DrawingContextState.CreateFromValues(drawingContext, new MatrixTransform(), 1));
        }
    }
}
