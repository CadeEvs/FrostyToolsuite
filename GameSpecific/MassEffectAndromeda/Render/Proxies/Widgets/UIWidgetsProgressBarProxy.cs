using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace LevelEditorPlugin.Render.Proxies
{
    public class UIWidgetsProgressBarProxy : UIElementProxy
    {
        protected TextureElementProxy backgroundTexture;
        protected TextureElementProxy fillTexture;
        protected float currentValue;
        protected float maximumValue;

        public UIWidgetsProgressBarProxy(Entities.UIWidgetsProgressBarEntity inEntity)
            : base(inEntity)
        {
            currentValue = inEntity.CurrentValue;
            maximumValue = inEntity.MaximumValue;

            if (inEntity.Background != null && inEntity.Background.HasTexture)
            {
                backgroundTexture = new TextureElementProxy(inEntity.Background.Texture, inEntity.Background.UvRect, position, size, inEntity.Background.SliceData);
            }
            if (inEntity.Fill != null && inEntity.Fill.HasTexture)
            {
                fillTexture = new TextureElementProxy(inEntity.Fill.Texture, inEntity.Fill.UvRect, position, size, inEntity.Fill.SliceData);
            }

            debugColor = Colors.Linen;
        }

        public override void Update()
        {
            base.Update();

            currentValue = (entity as Entities.UIWidgetsProgressBarEntity).CurrentValue;
            maximumValue = (entity as Entities.UIWidgetsProgressBarEntity).MaximumValue;
        }

        public override void Render(DrawingContext drawingContext)
        {
            base.Render(drawingContext);

            drawingContext.PushOpacity(alpha);
            drawingContext.PushTransform(transform);
            if (backgroundTexture != null)
            {
                backgroundTexture.Size = size;
                backgroundTexture.ColorBrush = colorBrush;
                backgroundTexture.Render(drawingContext);
            }
            if (fillTexture != null)
            {
                var progressEntity = entity as Entities.UIWidgetsProgressBarEntity;

                double maxValue = Math.Min(1.0, (currentValue / maximumValue));
                Size clipSize = new Size(size.Width * maxValue, size.Height);

                if (progressEntity.Data.FillBehavior == FrostySdk.Ebx.UIWidgetsProgressBarFillBehavior.UIWidgetsProgressBarFillBehavior_Clip)
                {
                    drawingContext.PushClip(new RectangleGeometry(new Rect(position, clipSize)));
                    fillTexture.Size = size;
                }
                else
                {
                    fillTexture.Size = clipSize;
                }

                fillTexture.ColorBrush = colorBrush;
                fillTexture.Render(drawingContext);

                if (progressEntity.Data.FillBehavior == FrostySdk.Ebx.UIWidgetsProgressBarFillBehavior.UIWidgetsProgressBarFillBehavior_Clip)
                {
                    drawingContext.Pop();
                }
            }
            drawingContext.Pop();
            drawingContext.Pop();
        }
    }
}
