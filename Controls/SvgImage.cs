using Frosty.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
namespace LevelEditorPlugin.Controls
{
    public class SvgImage : Control
    {
        public static readonly DependencyProperty DataProperty = DependencyProperty.Register("Data", typeof(SvgImageData), typeof(SvgImage), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));
        public SvgImageData Data
        {
            get { return (SvgImageData)GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }
        public static readonly DependencyProperty OverrideFillProperty = DependencyProperty.Register("OverrideFill", typeof(Brush), typeof(SvgImage), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));
        public Brush OverrideFill
        {
            get { return (Brush)GetValue(OverrideFillProperty); }
            set { SetValue(OverrideFillProperty, value); }
        }
        public static readonly DependencyProperty OverrideStrokeProperty = DependencyProperty.Register("OverrideStroke", typeof(Brush), typeof(SvgImage), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));
        public Brush OverrideStroke
        {
            get { return (Brush)GetValue(OverrideStrokeProperty); }
            set { SetValue(OverrideStrokeProperty, value); }
        }

        private SvgImageData.SvgGeometry[] _renderedGeometries;

        protected override Size ArrangeOverride(Size arrangeSize)
        {
            if (Data != null && Data.Geometries != null)
            {
                for (int i = 0; i < Data.Geometries.Length; i++)
                {
                    if (_renderedGeometries == null)
                        _renderedGeometries = new SvgImageData.SvgGeometry[Data.Geometries.Length];

                    SvgImageData.SvgGeometry geom = Data.Geometries[i];

                    Geometry g = geom.Geometry.Clone();
                    g.Transform = new MatrixTransform(
                        GetStretchMatrix(Stretch.Uniform, geom.Stroke.Thickness, arrangeSize, geom.Geometry.Bounds));

                    geom.Geometry = g;
                    _renderedGeometries[i] = geom;
                }
            }

            return base.ArrangeOverride(arrangeSize);
        }

        protected override Size MeasureOverride(Size constraint)
        {
            Size size = new Size(0, 0);
            if (Data != null && Data.Geometries != null)
            {
                foreach (SvgImageData.SvgGeometry geom in Data.Geometries)
                {
                    Size tmp = GetStretchedRenderSize(Stretch.Uniform, geom.Stroke.Thickness, constraint, geom.Geometry.Bounds);
                    size.Width = (tmp.Width > size.Width) ? tmp.Width : size.Width;
                    size.Height = (tmp.Height > size.Height) ? tmp.Height : size.Height;
                }
            }
            return size;
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            if (_renderedGeometries != null)
            {
                Brush overrideFill = OverrideFill;
                Brush overrideStroke = OverrideStroke;

                foreach (SvgImageData.SvgGeometry geometry in _renderedGeometries)
                {
                    Brush fill = geometry.Fill;
                    Pen stroke = geometry.Stroke;

                    if (overrideFill != null && !geometry.IgnoreOverrideFill)
                        fill = overrideFill;
                    if (overrideStroke != null)
                    {
                        stroke = stroke.Clone();
                        stroke.Brush = overrideStroke;
                    }

                    dc.DrawGeometry(fill, stroke, geometry.Geometry);
                }
            }
        }

        Size GetStretchedRenderSize(Stretch mode, double strokeThickness, Size availableSize, Rect geometryBounds)
        {
            double num;
            double num2;
            double num3;
            double num4;
            Size result;
            GetStretchMetrics(mode, strokeThickness, availableSize, geometryBounds, out num, out num2, out num3, out num4, out result);
            return result;
        }

        Matrix GetStretchMatrix(Stretch mode, double strokeThickness, Size availableSize, Rect geometryBounds)
        {
            double scaleX;
            double scaleY;
            double offsetX;
            double offsetY;
            Size result;
            GetStretchMetrics(mode, strokeThickness, availableSize, geometryBounds, out scaleX, out scaleY, out offsetX, out offsetY, out result);
            Matrix identity = Matrix.Identity;
            identity.ScaleAt(scaleX, scaleY, geometryBounds.Location.X, geometryBounds.Location.Y);
            identity.Translate(
                offsetX + (availableSize.Width / 2.0) - ((geometryBounds.Width * scaleX) / 2.0),
                offsetY + (availableSize.Height / 2.0) - ((geometryBounds.Height * scaleY) / 2.0));
            return identity;
        }

        void GetStretchMetrics(Stretch mode, double strokeThickness, Size availableSize, Rect geometryBounds, out double xScale, out double yScale, out double dX, out double dY, out Size stretchedSize)
        {
            if (!geometryBounds.IsEmpty)
            {
                double num = strokeThickness / 2.0;
                bool flag = false;
                xScale = System.Math.Max(availableSize.Width - strokeThickness, 0.0);
                yScale = System.Math.Max(availableSize.Height - strokeThickness, 0.0);
                dX = (num - geometryBounds.Left);
                dY = (num - geometryBounds.Top);
                if (geometryBounds.Width > xScale * 4.94065645841247E-324)
                {
                    xScale /= geometryBounds.Width;
                }
                else
                {
                    xScale = 1.0;
                    if (geometryBounds.Width == 0.0)
                    {
                        flag = true;
                    }
                }
                if (geometryBounds.Height > yScale * 4.94065645841247E-324)
                {
                    yScale /= geometryBounds.Height;
                }
                else
                {
                    yScale = 1.0;
                    if (geometryBounds.Height == 0.0)
                    {
                        flag = true;
                    }
                }
                if (mode != Stretch.Fill && !flag)
                {
                    if (mode == Stretch.Uniform)
                    {
                        if (yScale > xScale)
                        {
                            yScale = xScale;
                        }
                        else
                        {
                            xScale = yScale;
                        }
                    }
                    else if (xScale > yScale)
                    {
                        yScale = xScale;
                    }
                    else
                    {
                        xScale = yScale;
                    }
                }
                stretchedSize = new Size(geometryBounds.Width * xScale + strokeThickness, geometryBounds.Height * yScale + strokeThickness);
                return;
            }
            xScale = (yScale = 1.0);
            dX = (dY = 0.0);
            stretchedSize = new Size(0.0, 0.0);
        }
    }
}
