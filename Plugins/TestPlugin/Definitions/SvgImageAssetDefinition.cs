using Frosty.Core;
using Frosty.Core.Controls;
using FrostySdk.Interfaces;
using FrostySdk.Managers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TestPlugin.Controls;
using TestPlugin.Resources;

namespace TestPlugin.Definitions
{
    // This is a more complex asset definition. The icon is dynamic. It will load the SVG data and represent that as the icon for
    // the specified asset, using the width and height parameters (when possible) to ensure the SVG data is displayed as crisp
    // as possible. The generic version of the icon is used when no asset is provided ie. a null SvgImage reference.

    public class SvgImageAssetDefinition : AssetDefinition
    {
        protected static ImageSource imageSource = new ImageSourceConverter().ConvertFromString("pack://application:,,,/TestPlugin;component/Images/SvgFileType.png") as ImageSource;

        public override ImageSource GetIcon()
        {
            return imageSource;
        }

        public override ImageSource GetIcon(AssetEntry entry, double width, double height)
        {
            width = (double.IsInfinity(width)) ? 64 : width;
            height = (double.IsInfinity(height)) ? 64 : height;

            RenderTargetBitmap renderTarget = new RenderTargetBitmap((int)width, (int)height, 96.0, 96.0, PixelFormats.Pbgra32);
            renderTarget.Render(GetImage(entry as EbxAssetEntry, (int)width, (int)height));
            renderTarget.Freeze();

            return renderTarget;
        }

        public override FrostyAssetEditor GetEditor(ILogger logger)
        {
            return new SvgImageEditor(logger);
        }

        // @todo: import/export

        private Viewbox GetImage(EbxAssetEntry entry, int width, int height)
        {
            SvgImageAsset asset = App.AssetManager.GetEbxAs<SvgImageAsset>(entry);
            SvgImageResource image = asset.Resource;

            Grid imageGrid = new Grid
            {
                Width = image.Width,
                Height = image.Height
            };

            foreach (SvgShape shape in image.Shapes)
            {
                if (shape.Visible == 0)
                    continue;

                Path p = new Path();
                PathGeometry geom = new PathGeometry();
                p.Data = geom;
                if (shape.Stroke)
                    p.Stroke = new SolidColorBrush(Color.FromArgb(
                        (byte)((float)(shape.StrokeColor >> 24) * shape.Opacity),
                        (byte)(shape.StrokeColor),
                        (byte)(shape.StrokeColor >> 8),
                        (byte)(shape.StrokeColor >> 16)));
                if (shape.Fill)
                    p.Fill = new SolidColorBrush(Color.FromArgb(
                        (byte)((float)(shape.FillColor >> 24) * shape.Opacity),
                        (byte)(shape.FillColor),
                        (byte)(shape.FillColor >> 8),
                        (byte)(shape.FillColor >> 16)));
                p.StrokeThickness = shape.Thickness;
                p.StrokeEndLineCap = PenLineCap.Flat;

                foreach (SvgPath path in shape.Paths)
                {
                    PathFigure figure = new PathFigure {StartPoint = new Point(path.Points[0].X, path.Points[0].Y)};

                    PolyBezierSegment segment = new PolyBezierSegment();
                    figure.Segments.Add(segment);

                    for (int i = 1; i < path.Points.Count; i++)
                    {
                        segment.Points.Add(new Point(path.Points[i].X, path.Points[i].Y));
                    }

                    figure.IsClosed = path.Closed;
                    geom.Figures.Add(figure);
                }

                imageGrid.Children.Add(p);
            }

            imageGrid.HorizontalAlignment = HorizontalAlignment.Stretch;
            imageGrid.VerticalAlignment = VerticalAlignment.Stretch;

            Viewbox canvas = new Viewbox
            {
                Width = width,
                Height = height,
                Stretch = Stretch.Uniform,
                Child = imageGrid
            };

            canvas.Measure(new Size(width, height));
            canvas.Arrange(new Rect(0, 0, width, height));

            return canvas;
        }
    }
}
