using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace LevelEditorPlugin.Controls
{
    public class GridCanvas : Control
    {
        public static readonly DependencyProperty GridVisibleProperty = DependencyProperty.Register("GridVisible", typeof(bool), typeof(GridCanvas), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsRender));
        public static readonly DependencyProperty GridMajorBrushProperty = DependencyProperty.Register("GridMajorBrush", typeof(Brush), typeof(GridCanvas), new FrameworkPropertyMetadata(null));
        public static readonly DependencyProperty GridMinorBrushProperty = DependencyProperty.Register("GridMinorBrush", typeof(Brush), typeof(GridCanvas), new FrameworkPropertyMetadata(null));

        public bool GridVisible
        {
            get => (bool)GetValue(GridVisibleProperty);
            set => SetValue(GridVisibleProperty, value);
        }
        public Brush GridMajorBrush
        {
            get => (Brush)GetValue(GridMajorBrushProperty);
            set => SetValue(GridMajorBrushProperty, value);
        }
        public Brush GridMinorBrush
        {
            get => (Brush)GetValue(GridMinorBrushProperty);
            set => SetValue(GridMinorBrushProperty, value);
        }

        protected double scale = 1.0;
        protected double scaleLevel = 1.0;
        protected double scaleRate = 0.015;

        protected double minorUnits = 10;
        protected double majorUnits = 100;

        protected Point gridOffset;

        protected TranslateTransform viewport;
        protected TranslateTransform offset = new TranslateTransform(0, 0);
        protected Point prevMousePos;

        protected Pen blackPen;
        protected Pen gridMinorPen;
        protected Pen gridMajorPen;

        public GridCanvas()
        {
            Focusable = true;
            viewport = new TranslateTransform();
            Focus();
        }

        public Point TransformPoint(Point p)
        {
            return GetWorldMatrix().Inverse.Transform(p);
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            blackPen = new Pen(Brushes.Black, 1.0); blackPen.Freeze();
            gridMinorPen = new Pen(GridMinorBrush, 1.0); gridMinorPen.Freeze();
            gridMajorPen = new Pen(GridMajorBrush, 1.0); gridMajorPen.Freeze();
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            MatrixTransform m = new MatrixTransform(scale, 0, 0, scale, viewport.X, viewport.Y);
            Point mousePos = m.Inverse.Transform(e.GetPosition(this));

            double perc = (scale - scaleLevel * 0.5) / ((scaleLevel * 2) - (scaleLevel * 0.5));
            double rate = Lerp(scaleRate * 0.5, scaleRate * 2, perc);

            if (e.Delta < 0)
            {
                scale -= rate;
            }
            else
            {
                scale += rate;
            }

            UpdateScaleParameters();

            m = new MatrixTransform(scale, 0, 0, scale, viewport.X, viewport.Y);
            Point newPos = m.Inverse.Transform(e.GetPosition(this));

            offset.X -= (newPos.X - mousePos.X);
            offset.Y -= (newPos.Y - mousePos.Y);

            InvalidateVisual();
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            if (e.Handled)
                return;

            Focus();
            if (e.ChangedButton == MouseButton.Right)
            {
                MatrixTransform m = GetWorldMatrix();
                Point mousePos = m.Inverse.Transform(e.GetPosition(this));

                m = new MatrixTransform(scale, 0, 0, scale, viewport.X, viewport.Y);
                prevMousePos = m.Inverse.Transform(e.GetPosition(this));

                Mouse.Capture(this);
                e.Handled = true;
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (e.Handled)
                return;

            Focus();
            if (e.RightButton == MouseButtonState.Pressed)
            {
                MatrixTransform m = new MatrixTransform(scale, 0, 0, scale, viewport.X, viewport.Y);
                Point mousePos = m.Inverse.Transform(e.GetPosition(this));

                offset.X += (prevMousePos.X - mousePos.X);
                offset.Y += (prevMousePos.Y - mousePos.Y);

                prevMousePos = mousePos;
                e.Handled = true;
                InvalidateVisual();
            }
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            if (e.Handled)
                return;

            if (e.ChangedButton == MouseButton.Right)
            {
                Mouse.Capture(null);
                e.Handled = true;
            }
        }

        protected override Size MeasureOverride(Size constraint)
        {
            viewport = new TranslateTransform(constraint.Width * 0.5, constraint.Height * 0.5);
            return base.MeasureOverride(constraint);
        }

        protected sealed override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            drawingContext.PushClip(new RectangleGeometry(new Rect(0, 0, ActualWidth, ActualHeight)));
            drawingContext.DrawRectangle(Background, null, new Rect(0, 0, ActualWidth, ActualHeight));

            if (GridVisible)
            {
                DrawGrid(drawingContext);
                DrawPrimaryAxis(drawingContext);
            }

            Render(drawingContext);
            drawingContext.Pop();
        }

        protected virtual void Render(DrawingContext drawingContext)
        {
        }

        protected void DrawGrid(DrawingContext drawingContext)
        {
            Point tmpOffset = new Point(offset.X - gridOffset.X, offset.Y - gridOffset.Y);
            MatrixTransform view = GetViewMatrix();

            double minorLinesDistance = (minorUnits);
            double majorLinesDistance = (majorUnits);
            double invScale = (1 / scale);

            double startX = (viewport.X * invScale) - ((viewport.X * invScale) % minorUnits) + (tmpOffset.X % minorUnits);
            double startY = (viewport.Y * invScale) - ((viewport.Y * invScale) % minorUnits) + (tmpOffset.Y % minorUnits);

            for (int i = -1; i < ((ActualWidth * invScale) / minorLinesDistance); i++)
            {
                drawingContext.DrawLine(gridMinorPen,
                    view.Transform(new Point(-startX + (i * minorLinesDistance), -ActualHeight * invScale)),
                    view.Transform(new Point(-startX + (i * minorLinesDistance), ActualHeight * invScale))
                    );
            }
            for (int i = -1; i < ((ActualHeight * invScale) / minorLinesDistance); i++)
            {
                drawingContext.DrawLine(gridMinorPen,
                    view.Transform(new Point(-ActualWidth * invScale, -startY + (i * minorLinesDistance))),
                    view.Transform(new Point(ActualWidth * invScale, -startY + (i * minorLinesDistance)))
                    );
            }

            startX = (viewport.X * invScale) - ((viewport.X * invScale) % majorUnits) + (tmpOffset.X % majorUnits);
            startY = (viewport.Y * invScale) - ((viewport.Y * invScale) % majorUnits) + (tmpOffset.Y % majorUnits);

            for (int i = -1; i < ((ActualWidth * invScale) / majorLinesDistance) + 1; i++)
            {
                drawingContext.DrawLine(gridMajorPen,
                    view.Transform(new Point(-startX + (i * majorLinesDistance), -ActualHeight * invScale)),
                    view.Transform(new Point(-startX + (i * majorLinesDistance), ActualHeight * invScale))
                    );
            }
            for (int i = -1; i < ((ActualHeight * invScale) / majorLinesDistance) + 1; i++)
            {
                drawingContext.DrawLine(gridMajorPen,
                    view.Transform(new Point(-ActualWidth * invScale, -startY + (i * majorLinesDistance))),
                    view.Transform(new Point(ActualWidth * invScale, -startY + (i * majorLinesDistance)))
                    );
            }
        }

        protected void DrawPrimaryAxis(DrawingContext drawingContext)
        {
            Point tmpOffset = new Point(offset.X - gridOffset.X, offset.Y - gridOffset.Y);
            drawingContext.DrawLine(blackPen,
                new Point(-tmpOffset.X * scale + ActualWidth * 0.5, -ActualHeight),
                new Point(-tmpOffset.X * scale + ActualWidth * 0.5, ActualHeight)
                );
            drawingContext.DrawLine(blackPen,
                new Point(-ActualWidth, -tmpOffset.Y * scale + ActualHeight * 0.5),
                new Point(ActualWidth, -tmpOffset.Y * scale + ActualHeight * 0.5)
                );
        }

        protected MatrixTransform GetWorldMatrix()
        {
            Matrix offsetMatrix = new Matrix(1, 0, 0, 1, -offset.X, -offset.Y);
            Matrix scaleMatrix = new Matrix(scale, 0, 0, scale, 0, 0);
            Matrix viewportMatrix = new Matrix(1, 0, 0, 1, viewport.X, viewport.Y);

            return new MatrixTransform(offsetMatrix * scaleMatrix * viewportMatrix);
        }

        protected MatrixTransform GetWorldMatrixNoOffset()
        {
            Matrix offsetMatrix = new Matrix(1, 0, 0, 1, -offset.X, -offset.Y);
            Matrix scaleMatrix = new Matrix(scale, 0, 0, scale, 0, 0);

            return new MatrixTransform(offsetMatrix * scaleMatrix);
        }

        protected MatrixTransform GetViewMatrix()
        {
            Matrix scaleMatrix = new Matrix(scale, 0, 0, scale, 0, 0);
            Matrix viewportMatrix = new Matrix(1, 0, 0, 1, viewport.X, viewport.Y);

            return new MatrixTransform(scaleMatrix * viewportMatrix);
        }

        protected virtual void UpdateScaleParameters()
        {
            if (scale > (scaleLevel * 2))
            {
                scaleRate *= 2;
                scaleLevel *= 2;
                minorUnits /= 2;
                majorUnits /= 2;
            }
            else if (scale < scaleLevel)
            {
                scaleRate /= 2;
                scaleLevel *= 0.5;
                minorUnits *= 2;
                majorUnits *= 2;
            }

            if ((int)scale > (int)(scale * 2))
            {
                minorUnits /= 2;
                majorUnits /= 2;
            }
            else if (minorUnits * scale <= 5.0)
            {
                minorUnits *= 2;
                majorUnits *= 2;
            }
        }

        protected double Lerp(double a, double b, double amount)
        {
            return (1 - amount) * a + amount * b;
        }
    }
}
