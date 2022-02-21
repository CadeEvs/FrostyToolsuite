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

namespace Frosty.Controls
{
    internal class SvgImageDataConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType != typeof(string))
                return base.CanConvertTo(context, destinationType);

            if (context == null || context.Instance == null)
                return true;

            if (!(context.Instance is SvgImageData))
                throw new NotSupportedException();

            SvgImageData geometry = (SvgImageData)context.Instance;
            return geometry.CanSerializeToString();
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value == null)
                throw base.GetConvertFromException(value);

            string text = value as string;
            if (text != null)
                return SvgImageData.Parse(text);

            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType != null && value is SvgImageData)
            {
                SvgImageData geometry = (SvgImageData)value;
                if (destinationType == typeof(string))
                {
                    if (context != null && context.Instance != null && !geometry.CanSerializeToString())
                        throw new NotSupportedException();

                    return geometry.ConvertToString(null, culture);
                }
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }

    [TypeConverter(typeof(SvgImageDataConverter))]
    public class SvgImageData
    {
        public bool IgnoreOverrideFill { get; set; } = false;

        public struct SvgGeometry
        {
            public Brush Fill;
            public Pen Stroke;
            public bool IgnoreOverrideFill;
            public Geometry Geometry;
        }

        public SvgGeometry[] Geometries;

        public static SvgImageData Parse(string text)
        {
            SvgImageData data = new SvgImageData();
            if (text == "")
                return data;

            string[] arr = text.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            int offset = 1;

            if (arr[0] == "ignoreOverrideFill")
            {
                data.IgnoreOverrideFill = true;
                offset++;
            }

            int numElements = (arr.Length - offset) / 5;
            data.Geometries = new SvgGeometry[numElements];
            BrushConverter brushConverter = new BrushConverter();
            string originalWidthHeight = arr[offset - 1];

            for (int i = 0; i < numElements; i++)
            {
                string type = arr[(i * 5) + offset];
                Brush fill = (Brush)brushConverter.ConvertFrom(arr[(i * 5) + 1 + offset]);
                Brush stroke = (Brush)brushConverter.ConvertFrom(arr[(i * 5) + 2 + offset]);
                double strokeThickness = System.Math.Abs(double.Parse(arr[(i * 5) + 3 + offset]));
                string path = string.Format("M{0} M 0 0 ", originalWidthHeight) + arr[(i * 5) + 4 + offset];

                data.Geometries[i] = new SvgGeometry()
                {
                    Fill = fill,
                    Stroke = new Pen(stroke, strokeThickness),
                    IgnoreOverrideFill = data.IgnoreOverrideFill,
                    Geometry = Geometry.Parse(path)
                };
            }

            return data;
        }

        public string ConvertToString(string format, IFormatProvider provider)
        {
            return "";
        }

        public bool CanSerializeToString()
        {
            return true;
        }
    }

    public class FrostySvgImage : Control
    {
        public static readonly DependencyProperty DataProperty = DependencyProperty.Register("Data", typeof(SvgImageData), typeof(FrostySvgImage), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));
        public SvgImageData Data
        {
            get { return (SvgImageData)GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }
        public static readonly DependencyProperty OverrideFillProperty = DependencyProperty.Register("OverrideFill", typeof(Brush), typeof(FrostySvgImage), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));
        public Brush OverrideFill
        {
            get { return (Brush)GetValue(OverrideFillProperty); }
            set { SetValue(OverrideFillProperty, value); }
        }
        public static readonly DependencyProperty OverrideStrokeProperty = DependencyProperty.Register("OverrideStroke", typeof(Brush), typeof(FrostySvgImage), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));
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
                foreach (var geom in Data.Geometries)
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
