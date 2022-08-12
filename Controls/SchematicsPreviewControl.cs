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
        private ILogicEntity m_previewEntity;
        private NodeVisual m_previewEntityVisual;

        public SchematicsPreviewControl(ILogicEntity inEntity)
        {
            FontData largeFont = FontData.MakeFont(new Typeface("Consolas"), 10);

            m_previewEntity = inEntity;
            m_previewEntityVisual = new NodeVisual(m_previewEntity, 0, 0) { IsCollapsed = false, GlyphWidth = largeFont.AdvanceWidth };
            m_previewEntityVisual.Update();

            Background = null;
            IsHitTestVisible = false;
            Focusable = false;
            IsEnabled = false;
            Width = m_previewEntityVisual.Rect.Width;
            Height = m_previewEntityVisual.Rect.Height;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            m_previewEntityVisual.Render(SchematicsCanvas.DrawingContextState.CreateFromValues(drawingContext, new MatrixTransform(), 1));
        }
    }
}
