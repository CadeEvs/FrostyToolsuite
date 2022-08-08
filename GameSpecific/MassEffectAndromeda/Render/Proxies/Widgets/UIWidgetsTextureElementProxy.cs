using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.Effects;
using UIWidgets9SliceTextureData = FrostySdk.Ebx.UIWidgets9SliceTextureData;

namespace LevelEditorPlugin.Render.Proxies
{
    public class TextureElementProxy
    {
        public Size Size
        {
            get => size;
            set
            {
                if (size != value)
                {
                    size = value;
                }
            }
        }
        public Brush ColorBrush
        {
            get => colorBrush;
            set
            {
                if (colorBrush != value)
                {
                    colorBrush = value;
                }
            }
        }
        
        protected BitmapSource texture;

        protected Point4D uvRect;

        protected Point4D sliceData;
        protected bool drawSliced;

        protected Point position;
        protected Size size;
        protected Brush colorBrush;

        protected double[] sliceSizes = new double[4];

        public TextureElementProxy(BitmapSource inTexture, Point4D inUvRect, Point inPosition, Size inSize, UIWidgets9SliceTextureData inSliceData)
        {
            position = inPosition;
            size = inSize;

            texture = inTexture;
            uvRect = inUvRect;

            var pixelWidth = (uvRect.Z - uvRect.X) * texture.PixelWidth;
            var pixelHeight = (uvRect.W - uvRect.Y) * texture.PixelHeight;

            var sliceWidth1 = pixelWidth * inSliceData.Left;
            var sliceHeight1 = pixelHeight * inSliceData.Top;
            var sliceWidth2 = pixelWidth * (1 - inSliceData.Right);
            var sliceHeight2 = pixelHeight * (1 - inSliceData.Bottom);

            drawSliced = sliceWidth1 > 0 || sliceHeight2 > 0 || sliceWidth2 > 0 || sliceHeight2 > 0;
            sliceData = new Point4D(inSliceData.Left, inSliceData.Top, inSliceData.Right, inSliceData.Bottom); ;

            if (inSliceData.UseSizeAsCenterSlice)
            {
                position.X -= sliceWidth1;
                position.Y -= sliceHeight1;
                size.Width += (sliceWidth1 + sliceWidth2);
                size.Height += (sliceHeight1 + sliceHeight2);
            }

            sliceSizes[0] = sliceWidth1;
            sliceSizes[1] = sliceHeight1;
            sliceSizes[2] = sliceWidth2;
            sliceSizes[3] = sliceHeight2;
        }

        public void Render(DrawingContext drawingContext)
        {
            if (double.IsNaN(size.Width) || double.IsNaN(size.Height))
                return;
            if (size.Width == 0 || size.Height == 0)
                return;

            if (drawSliced)
            {
                DrawNineSliceImage(drawingContext);
            }
            else
            {
                DrawImage(drawingContext);
            }
        }

        protected void DrawImage(DrawingContext drawingContext)
        {
            DrawImage(drawingContext, position.X, position.Y, uvRect, size.Width, size.Height, Transform.Identity);
        }

        protected void DrawImage(DrawingContext drawingContext, double x, double y, Point4D subUvRect, double imageWidth, double imageHeight, Transform localTransform)
        {
            var bitmap = texture;
            Point4D uvRect = new Point4D(
                subUvRect.X * bitmap.PixelWidth,
                subUvRect.Y * bitmap.PixelHeight,
                subUvRect.Z * bitmap.PixelWidth,
                subUvRect.W * bitmap.PixelHeight
                );

            double width = uvRect.Z - uvRect.X;
            double height = uvRect.W - uvRect.Y;
            double scaleX = 1;
            double scaleY = 1;

            //if (preserveAspectRatio)
            //{
            //    if (width >= height)
            //    {
            //        scaleX = imageWidth / width;
            //        scaleY = scaleX;
            //    }
            //    else
            //    {
            //        scaleY = imageHeight / height;
            //        scaleX = scaleY;
            //    }
            //}
            //else
            {
                scaleX = imageWidth / width;
                scaleY = imageHeight / height;
            }

            double invScaleX = 1.0 / scaleX;
            double invScaleY = 1.0 / scaleY;

            //switch (horizontalAlignment)
            //{
            //    case HorizontalAlignment.Center: x += (size.Width * 0.5) - (width * scaleX * 0.5); break;
            //    case HorizontalAlignment.Right: x += size.Width - (width * scaleX); break;
            //}
            //switch (verticalAlignment)
            //{
            //    case VerticalAlignment.Center: y += (size.Height * 0.5) - (height * scaleY * 0.5); break;
            //    case VerticalAlignment.Bottom: y += size.Height - (height * scaleY); break;
            //}

            drawingContext.PushTransform(new ScaleTransform(scaleX, scaleY));
            drawingContext.PushTransform(new TranslateTransform(-uvRect.X, -uvRect.Y));
            drawingContext.PushTransform(new TranslateTransform(uvRect.X + (x * invScaleX) + (width * 0.5), uvRect.Y + (y * invScaleY) + (height * 0.5)));
            drawingContext.PushTransform(new TranslateTransform(-uvRect.X + -width * 0.5, -uvRect.Y + -height * 0.5));

            drawingContext.PushClip(new RectangleGeometry(new Rect(uvRect.X, uvRect.Y, width, height)));
            drawingContext.DrawImage(texture, new Rect(0, 0, bitmap.PixelWidth, bitmap.PixelHeight));
            //drawingContext.PushOpacityMask(new DrawingBrush(new ImageDrawing(texture, new Rect(0, 0, bitmap.PixelWidth, bitmap.PixelHeight))));
            //drawingContext.DrawRectangle(colorBrush, null, new Rect(0, 0, bitmap.PixelWidth, bitmap.PixelHeight));
            //drawingContext.Pop();
            drawingContext.Pop();

            drawingContext.Pop();
            drawingContext.Pop();
            drawingContext.Pop();
            drawingContext.Pop();
        }

        protected void DrawNineSliceImage(DrawingContext drawingContext)
        {
            var pixelWidth = (uvRect.Z - uvRect.X) * texture.PixelWidth;
            var pixelHeight = (uvRect.W - uvRect.Y) * texture.PixelHeight;

            Point4D[] slices = new Point4D[]
            {
                new Point4D(0, sliceData.X, 0, sliceData.Y),
                new Point4D(sliceData.X, sliceData.Z, 0, sliceData.Y),
                new Point4D(sliceData.Z, 1, 0, sliceData.Y),

                new Point4D(0, sliceData.X, sliceData.Y, sliceData.W),
                new Point4D(sliceData.X, sliceData.Z, sliceData.Y, sliceData.W),
                new Point4D(sliceData.Z, 1, sliceData.Y, sliceData.W),

                new Point4D(0, sliceData.X, sliceData.W, 1),
                new Point4D(sliceData.X, sliceData.Z, sliceData.W, 1),
                new Point4D(sliceData.Z, 1, sliceData.W, 1),
            };
            Point[] offsets = new Point[]
            {
                new Point(0, 0),
                new Point(sliceSizes[0], 0),
                new Point(pixelWidth - sliceSizes[2], 0),

                new Point(0, sliceSizes[1]),
                new Point(sliceSizes[0], sliceSizes[1]),
                new Point(pixelWidth -sliceSizes[2], sliceSizes[1]),

                new Point(0, pixelHeight - sliceSizes[3]),
                new Point(sliceSizes[0], pixelHeight - sliceSizes[3]),
                new Point(pixelWidth - sliceSizes[2], pixelHeight - sliceSizes[3]),
            };
            Point[] sizes = new Point[]
            {
                new Point(sliceSizes[0], sliceSizes[1]),
                new Point(pixelWidth - (sliceSizes[0] + sliceSizes[2]), sliceSizes[1]),
                new Point(sliceSizes[2], sliceSizes[1]),

                new Point(sliceSizes[0], pixelHeight - (sliceSizes[3] + sliceSizes[1])),
                new Point(pixelWidth - (sliceSizes[0] + sliceSizes[2]), pixelHeight - (sliceSizes[3] + sliceSizes[1])),
                new Point(sliceSizes[2], pixelHeight - (sliceSizes[3] + sliceSizes[1])),

                new Point(sliceSizes[0], sliceSizes[3]),
                new Point(pixelWidth - (sliceSizes[0] + sliceSizes[2]), sliceSizes[3]),
                new Point(sliceSizes[2], sliceSizes[3]),
            };

            //drawingContext.PushTransform(new TranslateTransform(position.X + size.Width * 0.5, position.Y + size.Height * 0.5));
            //drawingContext.PushTransform(mirrorScale);
            //drawingContext.PushTransform(transform);
            double scaleX = size.Width / pixelWidth;
            double scaleY = size.Height / pixelHeight;
            double invScaleX = 1 / scaleX;
            double invScaleY = 1 / scaleY;

            drawingContext.PushTransform(new ScaleTransform(scaleX, scaleY));

            for (int i = 0; i < slices.Length; i++)
            {
                if (sizes[i].X == 0 || sizes[i].Y == 0)
                    continue;

                Point4D a = new Point4D(
                    uvRect.X + ((uvRect.Z - uvRect.X) * slices[i].X),
                    uvRect.Y + ((uvRect.W - uvRect.Y) * slices[i].Z),
                    uvRect.X + ((uvRect.Z - uvRect.X) * slices[i].Y),
                    uvRect.Y + ((uvRect.W - uvRect.Y) * slices[i].W)
                    );

                DrawImage(drawingContext, (position.X * invScaleX) + offsets[i].X, (position.Y * invScaleY) + offsets[i].Y, a, (float)sizes[i].X, (float)sizes[i].Y, Transform.Identity);
            }

            drawingContext.Pop();
            //drawingContext.Pop();
            //drawingContext.Pop();
            //drawingContext.Pop();
        }
    }

    public class UIWidgetsTextureElementProxy : UIElementProxy
    {
        protected BitmapSource texture;
        protected BitmapSource origTexture;

        protected HorizontalAlignment horizontalAlignment;
        protected VerticalAlignment verticalAlignment;
        protected bool preserveTextureSize;
        protected bool preserveAspectRatio;

        protected Point4D sliceData;
        protected Point4D uvRect;

        protected double[] sliceSizes = new double[4];
        protected bool drawSliced = false;
        protected bool drawTexture = false;

        public UIWidgetsTextureElementProxy(Entities.UIElementBitmapEntityEx inEntity)
            : base(inEntity)
        {
            preserveAspectRatio = true;
            preserveTextureSize = true;

            horizontalAlignment = HorizontalAlignment.Center;
            verticalAlignment = VerticalAlignment.Center;
            alpha = inEntity.Data.Alpha;
            transform = MakeMatrixTransform(inEntity.Data.Transform);

            texture = SetTexture(inEntity.Texture);
            uvRect = inEntity.UvRect;

            drawTexture = texture != null;
            debugColor = Colors.LightGreen;
        }

        public UIWidgetsTextureElementProxy(Entities.FBUIStaticTextureElementEntity inEntity)
            : base(inEntity)
        {
            preserveTextureSize = true;
            preserveAspectRatio = true;

            horizontalAlignment = HorizontalAlignment.Center;
            verticalAlignment = VerticalAlignment.Center;
            alpha = inEntity.Data.Alpha;
            transform = MakeMatrixTransform(inEntity.Data.Transform);

            texture = SetTexture(inEntity.Texture);
            uvRect = inEntity.UvRect;

            drawTexture = texture != null;
            debugColor = Colors.LightGreen;
        }

        public UIWidgetsTextureElementProxy(Entities.FBUIDynamicTextureElementEntity inEntity)
            : base(inEntity)
        {
            preserveTextureSize = true;
            preserveAspectRatio = true;

            horizontalAlignment = HorizontalAlignment.Center;
            verticalAlignment = VerticalAlignment.Center;
            alpha = inEntity.Data.Alpha;
            transform = MakeMatrixTransform(inEntity.Data.Transform);

            texture = SetTexture(inEntity.Texture);
            uvRect = inEntity.UvRect;

            drawTexture = texture != null;
            debugColor = Colors.LightGreen;
        }

        public UIWidgetsTextureElementProxy(Entities.UIWidgetsTextureElementEntity inEntity)
            : base(inEntity)
        {
            horizontalAlignment = MakeHorizontalAlignment(inEntity.Data.HorizontalAlign);
            verticalAlignment = MakeVerticalAlignment(inEntity.Data.VerticalAlign);
            alpha = inEntity.Data.Alpha;
            transform = MakeMatrixTransform(inEntity.Data.Transform);

            preserveTextureSize = inEntity.Data.PreserveTextureSize;
            preserveAspectRatio = inEntity.Data.PreserveAspectRatio;

            texture = SetTexture(inEntity.Texture);
            if (texture != null)
            {
                drawTexture = true;
                uvRect = inEntity.UvRect;

                var pixelWidth = (uvRect.Z - uvRect.X) * texture.PixelWidth;
                var pixelHeight = (uvRect.W - uvRect.Y) * texture.PixelHeight;

                var sliceWidth1 = pixelWidth * inEntity.Data.Texture.SliceData.Left;
                var sliceHeight1 = pixelHeight * inEntity.Data.Texture.SliceData.Top;
                var sliceWidth2 = pixelWidth * (1 - inEntity.Data.Texture.SliceData.Right);
                var sliceHeight2 = pixelHeight * (1 - inEntity.Data.Texture.SliceData.Bottom);

                drawSliced = sliceWidth1 > 0 || sliceHeight2 > 0 || sliceWidth2 > 0 || sliceHeight2 > 0;
                sliceData = new Point4D(inEntity.Data.Texture.SliceData.Left, inEntity.Data.Texture.SliceData.Top, inEntity.Data.Texture.SliceData.Right, inEntity.Data.Texture.SliceData.Bottom);

                if (inEntity.Data.Texture.SliceData.UseSizeAsCenterSlice)
                {
                    position.X -= sliceWidth1;
                    position.Y -= sliceHeight1;
                    size.Width += (sliceWidth1 + sliceWidth2);
                    size.Height += (sliceHeight1 + sliceHeight2);
                }

                sliceSizes[0] = sliceWidth1;
                sliceSizes[1] = sliceHeight1;
                sliceSizes[2] = sliceWidth2;
                sliceSizes[3] = sliceHeight2;

                if (drawSliced)
                {
                    horizontalAlignment = HorizontalAlignment.Left;
                    verticalAlignment = VerticalAlignment.Top;
                }
            }

            debugColor = Colors.LightGreen;
        }

        public override void Update()
        {
            Color oldColor = color;

            base.Update();

            if (entity is Entities.UIWidgetsTextureElementEntity)
            {
                var textureWidget = entity as Entities.UIWidgetsTextureElementEntity;
                if (textureWidget.Texture != null && (texture != textureWidget.Texture || uvRect != textureWidget.UvRect))
                {
                    origTexture = textureWidget.Texture;
                    texture = SetTexture(origTexture);
                    uvRect = textureWidget.UvRect;
                    drawSliced = false;
                    drawTexture = true;
                }
            }
            else if (entity is Entities.UIElementBitmapEntityEx)
            {
                var textureWidget = entity as Entities.UIElementBitmapEntityEx;
                if (textureWidget.Texture != null && texture != textureWidget.Texture)
                {
                    origTexture = textureWidget.Texture;
                    texture = SetTexture(origTexture);
                    uvRect = textureWidget.UvRect;
                    drawSliced = false;
                    drawTexture = true;
                }
            }
            else if (color != oldColor && color != Colors.White)
            {
                if (texture != null)
                {
                    // change color
                    texture = SetTexture(texture);
                }
            }
        }

        public override void Render(DrawingContext drawingContext)
        {
            base.Render(drawingContext);

            drawingContext.PushOpacity(alpha);

            if (drawTexture)
            {
                if (drawSliced)
                {
                    DrawNineSliceImage(drawingContext);
                }
                else
                {
                    DrawImage(drawingContext);
                }
            }

            drawingContext.Pop();
        }

        protected void DrawImage(DrawingContext drawingContext)
        {
            drawingContext.PushTransform(new TranslateTransform(position.X + size.Width * 0.5, position.Y + size.Height * 0.5));
            drawingContext.PushTransform(mirrorScale);
            drawingContext.PushTransform(transform);
            drawingContext.PushTransform(new TranslateTransform(-position.X + -size.Width * 0.5, -position.Y + -size.Height * 0.5));

            DrawImage(drawingContext, position.X, position.Y, uvRect, size.Width, size.Height, transform);

            drawingContext.Pop();
            drawingContext.Pop();
            drawingContext.Pop();
            drawingContext.Pop();
        }

        protected void DrawImage(DrawingContext drawingContext, double x, double y, Point4D subUvRect, double imageWidth, double imageHeight, Transform localTransform)
        {
            var bitmap = texture;
            Point4D uvRect = new Point4D(
                subUvRect.X * bitmap.PixelWidth,
                subUvRect.Y * bitmap.PixelHeight,
                subUvRect.Z * bitmap.PixelWidth,
                subUvRect.W * bitmap.PixelHeight
                );

            double width = uvRect.Z - uvRect.X;
            double height = uvRect.W - uvRect.Y;
            double scaleX = 1;
            double scaleY = 1;

            if (preserveAspectRatio)
            {
                if (width >= height)
                {
                    scaleX = imageWidth / width;
                    scaleY = scaleX;
                }
                else
                {
                    scaleY = imageHeight / height;
                    scaleX = scaleY;
                }
            }
            else
            {
                scaleX = imageWidth / width;
                scaleY = imageHeight / height;
            }

            double invScaleX = 1.0 / scaleX;
            double invScaleY = 1.0 / scaleY;

            switch (horizontalAlignment)
            {
                case HorizontalAlignment.Center: x += (size.Width * 0.5) - (width * scaleX * 0.5); break;
                case HorizontalAlignment.Right: x += size.Width - (width * scaleX); break;
            }
            switch (verticalAlignment)
            {
                case VerticalAlignment.Center: y += (size.Height * 0.5) - (height * scaleY * 0.5); break;
                case VerticalAlignment.Bottom: y += size.Height - (height * scaleY); break;
            }

            drawingContext.PushTransform(new ScaleTransform(scaleX, scaleY));
            drawingContext.PushTransform(new TranslateTransform(-uvRect.X, -uvRect.Y));
            drawingContext.PushTransform(new TranslateTransform(uvRect.X + (x * invScaleX) + (width * 0.5), uvRect.Y + (y * invScaleY) + (height * 0.5)));
            drawingContext.PushTransform(new TranslateTransform(-uvRect.X + -width * 0.5, -uvRect.Y + -height * 0.5));

            drawingContext.PushClip(new RectangleGeometry(new Rect(uvRect.X, uvRect.Y, width, height)));
            drawingContext.DrawImage(texture, new Rect(0, 0, bitmap.PixelWidth, bitmap.PixelHeight));
            //drawingContext.PushOpacityMask(new DrawingBrush(new ImageDrawing(texture, new Rect(0, 0, bitmap.PixelWidth, bitmap.PixelHeight))));
            //drawingContext.DrawRectangle(colorBrush, null, new Rect(0, 0, bitmap.PixelWidth, bitmap.PixelHeight));
            //drawingContext.Pop();
            drawingContext.Pop();

            drawingContext.Pop();
            drawingContext.Pop();
            drawingContext.Pop();
            drawingContext.Pop();
        }

        protected void DrawNineSliceImage(DrawingContext drawingContext)
        {
            Point4D[] slices = new Point4D[]
            {
                new Point4D(0, sliceData.X, 0, sliceData.Y),
                new Point4D(sliceData.X, sliceData.Z, 0, sliceData.Y),
                new Point4D(sliceData.Z, 1, 0, sliceData.Y),

                new Point4D(0, sliceData.X, sliceData.Y, sliceData.W),
                new Point4D(sliceData.X, sliceData.Z, sliceData.Y, sliceData.W),
                new Point4D(sliceData.Z, 1, sliceData.Y, sliceData.W),

                new Point4D(0, sliceData.X, sliceData.W, 1),
                new Point4D(sliceData.X, sliceData.Z, sliceData.W, 1),
                new Point4D(sliceData.Z, 1, sliceData.W, 1),
            };
            Point[] offsets = new Point[]
            {
                new Point(0, 0),
                new Point(sliceSizes[0], 0),
                new Point(size.Width - sliceSizes[2], 0),

                new Point(0, sliceSizes[1]),
                new Point(sliceSizes[0], sliceSizes[1]),
                new Point(size.Width -sliceSizes[2], sliceSizes[1]),

                new Point(0, size.Height - sliceSizes[3]),
                new Point(sliceSizes[0], size.Height - sliceSizes[3]),
                new Point(size.Width - sliceSizes[2], size.Height - sliceSizes[3]),
            };
            Point[] sizes = new Point[]
            {
                new Point(sliceSizes[0], sliceSizes[1]),
                new Point(size.Width - (sliceSizes[0] + sliceSizes[2]), sliceSizes[1]),
                new Point(sliceSizes[2], sliceSizes[1]),

                new Point(sliceSizes[0], size.Height - (sliceSizes[3] + sliceSizes[1])),
                new Point(size.Width - (sliceSizes[0] + sliceSizes[2]), size.Height - (sliceSizes[3] + sliceSizes[1])),
                new Point(sliceSizes[2], size.Height - (sliceSizes[3] + sliceSizes[1])),

                new Point(sliceSizes[0], sliceSizes[3]),
                new Point(size.Width - (sliceSizes[0] + sliceSizes[2]), sliceSizes[3]),
                new Point(sliceSizes[2], sliceSizes[3]),
            };

            drawingContext.PushTransform(new TranslateTransform(position.X + size.Width * 0.5, position.Y + size.Height * 0.5));
            drawingContext.PushTransform(mirrorScale);
            drawingContext.PushTransform(transform);
            drawingContext.PushTransform(new TranslateTransform(-position.X + -size.Width * 0.5, -position.Y + -size.Height * 0.5));

            for (int i = 0; i < slices.Length; i++)
            {
                if (sizes[i].X == 0 || sizes[i].Y == 0)
                    continue;

                Point4D a = new Point4D(
                    uvRect.X + ((uvRect.Z - uvRect.X) * slices[i].X),
                    uvRect.Y + ((uvRect.W - uvRect.Y) * slices[i].Z),
                    uvRect.X + ((uvRect.Z - uvRect.X) * slices[i].Y),
                    uvRect.Y + ((uvRect.W - uvRect.Y) * slices[i].W)
                    );

                DrawImage(drawingContext, position.X + offsets[i].X, position.Y + offsets[i].Y, a, (float)sizes[i].X, (float)sizes[i].Y, Transform.Identity);
            }

            drawingContext.Pop();
            drawingContext.Pop();
            drawingContext.Pop();
            drawingContext.Pop();
        }

        protected HorizontalAlignment MakeHorizontalAlignment(FrostySdk.Ebx.UIWidgetsTextureElementEntityHAlign align)
        {
            switch(align)
            {
                case FrostySdk.Ebx.UIWidgetsTextureElementEntityHAlign.UIWidgetsTextureElementEntityHAlign_Left: return HorizontalAlignment.Left;
                case FrostySdk.Ebx.UIWidgetsTextureElementEntityHAlign.UIWidgetsTextureElementEntityHAlign_Center: return HorizontalAlignment.Center;
                case FrostySdk.Ebx.UIWidgetsTextureElementEntityHAlign.UIWidgetsTextureElementEntityHAlign_Right: return HorizontalAlignment.Right;
            }
            return HorizontalAlignment.Left;
        }

        protected VerticalAlignment MakeVerticalAlignment(FrostySdk.Ebx.UIWidgetsTextureElementEntityVAlign align)
        {
            switch (align)
            {
                case FrostySdk.Ebx.UIWidgetsTextureElementEntityVAlign.UIWidgetsTextureElementEntityVAlign_Top: return VerticalAlignment.Top;
                case FrostySdk.Ebx.UIWidgetsTextureElementEntityVAlign.UIWidgetsTextureElementEntityVAlign_Center: return VerticalAlignment.Center;
                case FrostySdk.Ebx.UIWidgetsTextureElementEntityVAlign.UIWidgetsTextureElementEntityVAlign_Bottom: return VerticalAlignment.Bottom;
            }
            return VerticalAlignment.Top;
        }

        private BitmapSource SetTexture(BitmapSource newTexture)
        {
            if (color != Colors.White)
            {
                if (origTexture == null)
                {
                    origTexture = newTexture;
                }

                if (origTexture == null)
                    return null;

                byte[] pixels = new byte[4 * origTexture.PixelWidth * origTexture.PixelHeight];
                origTexture.CopyPixels(pixels, 4 * origTexture.PixelWidth, 0);

                for (int i = 0; i < pixels.Length; i += 4)
                {
                    pixels[i + 0] = (byte)(((pixels[i + 0] / 255.0f) * color.ScB) * 255.0f);
                    pixels[i + 1] = (byte)(((pixels[i + 1] / 255.0f) * color.ScG) * 255.0f);
                    pixels[i + 2] = (byte)(((pixels[i + 2] / 255.0f) * color.ScR) * 255.0f);
                }

                newTexture = BitmapImage.Create(origTexture.PixelWidth, origTexture.PixelHeight, origTexture.DpiX, origTexture.DpiY, origTexture.Format, null, pixels, 4 * origTexture.PixelWidth);
            }

            return newTexture;
        }
    }
}
