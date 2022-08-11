using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using LevelEditorPlugin.Data;
using LevelEditorPlugin.Managers;

namespace LevelEditorPlugin.Controls
{ 
    public class CommentNodeVisual : BaseVisual
    {
        public override object Data => CommentData;

        public CommentNodeData CommentData;

        private Brush colorBrush;
        private Pen colorPen;
        private SolidColorBrush outlineBrush;
        private Pen outlinePen;
        private List<BaseVisual> nodes;

        public CommentNodeVisual(CommentNodeData commentData, IEnumerable<BaseVisual> selectedVisuals, double x, double y) 
            : base(x, y)
        {
            CommentData = commentData;

            nodes = new List<BaseVisual>();
            nodes.AddRange(selectedVisuals);

            colorBrush = new SolidColorBrush(Color.FromScRgb(1.0f, commentData.Color.x, commentData.Color.y, commentData.Color.z));
            colorPen = new Pen(colorBrush, 5.0);
            outlineBrush = new SolidColorBrush(Color.FromScRgb(0.0f, 1.0f, 1.0f, 1.0f));
            outlinePen = new Pen(outlineBrush, 2.5);

            UpdatePositionAndSize();
        }

        public SchematicsLayout.Comment GenerateLayout()
        {
            SchematicsLayout.Comment commentLayout = new SchematicsLayout.Comment();
            commentLayout.UniqueId = UniqueId;
            commentLayout.Position = Rect.Location;
            commentLayout.Text = CommentData.CommentText;
            commentLayout.Color = Color.FromArgb(255, (byte)(CommentData.Color.x * 255.0f), (byte)(CommentData.Color.y * 255.0f), (byte)(CommentData.Color.z * 255.0f));
            commentLayout.Children = new List<Guid>();

            foreach (BaseVisual node in nodes)
            {
                commentLayout.Children.Add(node.UniqueId);
            }

            return commentLayout;
        }

        public void AddNode(BaseVisual node)
        {
            nodes.Add(node);
            UpdatePositionAndSize();
        }

        public override bool HitTest(Point mousePos)
        {
            Rect invalidZoneRect = new Rect(Rect.X + 5, Rect.Y + 20, Rect.Width - 10, Rect.Height - 25);
            bool hitTitle = !invalidZoneRect.Contains(mousePos);                              
            return hitTitle;
        }

        public override bool OnMouseOver(Point mousePos)
        {
            outlineBrush.Opacity = 0.4;
            return base.OnMouseOver(mousePos);
        }

        public override bool OnMouseLeave()
        {
            outlineBrush.Opacity = 0.0;
            return base.OnMouseLeave();
        }

        public override void Move(Point newPos)
        {
            Point offset = new Point(newPos.X - Rect.X, newPos.Y - Rect.Y);
            foreach (BaseVisual node in nodes)
            {
                if (node.IsSelected)
                    continue;

                Point newNodePos = new Point(node.Rect.X + offset.X, node.Rect.Y + offset.Y);
                node.Move(newNodePos);
            }

            base.Move(newPos);
        }

        public override void Update()
        {
            UpdatePositionAndSize();
        }

        public override void Render(SchematicsCanvas.DrawingContextState state)
        {
            colorPen.Thickness = 5.0 * state.Scale;
            colorPen.Brush = colorBrush;

            Rect rect = nodes[0].Rect;
            for (int i = 1; i < nodes.Count; i++)
            {
                rect.Union(nodes[i].Rect);
            }
            Rect.X = rect.X - 10;
            Rect.Y = rect.Y - 30;
            Rect.Width = rect.Width + 20;
            Rect.Height = rect.Height + 40;

            Rect outlineRect = new Rect(Rect.X + 2.5, Rect.Y + 2.5, Rect.Width - 5, Rect.Height - 5);
            Rect titleRect = new Rect(Rect.X, Rect.Y, Rect.Width, 20);

            state.DrawingContext.DrawRectangle(null, colorPen, state.TransformRect(outlineRect));
            state.DrawingContext.DrawRectangle(colorPen.Brush, null, state.TransformRect(titleRect));

            if (state.InvScale < 2.5)
            {
                Rect innerOutlineRect = new Rect(Rect.X + 5, Rect.Y + 20, Rect.Width - 10, Rect.Height - 25);
                Point titlePos = new Point(Rect.X + 5, Rect.Y + 3.5);

                state.DrawingContext.DrawRectangle(null, state.BlackPen, state.TransformRect(Rect));
                state.DrawingContext.DrawRectangle(null, state.BlackPen, state.TransformRect(innerOutlineRect));

                if (!string.IsNullOrEmpty(CommentData.CommentText))
                {
                    double luminance = (CommentData.Color.x * 0.3) + (CommentData.Color.y * 0.59) + (CommentData.Color.z * 0.11);
                    // comment title
                    GlyphRun titleGlyphRun = state.ConvertTextLinesToGlyphRun(state.TransformPoint(titlePos), true, CommentData.CommentText);
                    state.DrawingContext.DrawGlyphRun((luminance > 0.2) ? Brushes.Black : Brushes.White, titleGlyphRun);
                }
            }

            // selection outline
            outlinePen.Thickness = 2.5 * state.Scale;
            outlinePen.Brush = IsSelected ? Brushes.PaleGoldenrod : outlineBrush;
            Rect selectedRect = new Rect(Rect.X - 1, Rect.Y - 1, Rect.Width + 2, Rect.Height + 2);
            state.DrawingContext.DrawRectangle(null, outlinePen, state.TransformRect(selectedRect));
        }

        private void UpdatePositionAndSize()
        {
            Rect rect = nodes[0].Rect;
            for (int i = 1; i < nodes.Count; i++)
            {
                rect.Union(nodes[i].Rect);
            }

            Rect.X = rect.X - 10;
            Rect.Y = rect.Y - 30;
            Rect.Width = rect.Width + 20;
            Rect.Height = rect.Height + 40;

            Color tmpColor = Color.FromScRgb(1.0f, CommentData.Color.x, CommentData.Color.y, CommentData.Color.z);
            if (tmpColor != (colorPen.Brush as SolidColorBrush).Color)
            {
                colorBrush = new SolidColorBrush(tmpColor);
                colorPen.Brush = colorBrush;
            }
        }
    }
}