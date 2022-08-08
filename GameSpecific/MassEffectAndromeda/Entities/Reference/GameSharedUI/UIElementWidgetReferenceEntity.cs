using Frosty.Core.Viewport;
using LevelEditorPlugin.Editors;
using LevelEditorPlugin.Render.Proxies;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using LinearTransform = FrostySdk.Ebx.LinearTransform;
using Vec3 = FrostySdk.Ebx.Vec3;
using MouseButton = System.Windows.Input.MouseButton;
using System;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UIElementWidgetReferenceEntityData))]
	public class UIElementWidgetReferenceEntity : LogicReferenceObject, IEntityData<FrostySdk.Ebx.UIElementWidgetReferenceEntityData>, IUIWidget, ITransformEntity
	{
        protected readonly int Property_Alpha = Frosty.Hash.Fnv1.HashString("Alpha");
        protected readonly int Property_Visible = Frosty.Hash.Fnv1.HashString("Visible");
        protected readonly int Property_BlueprintTransform = Frosty.Hash.Fnv1.HashString("BlueprintTransform");
        protected readonly int Property_Color = Frosty.Hash.Fnv1.HashString("Color");

        public new FrostySdk.Ebx.UIElementWidgetReferenceEntityData Data => data as FrostySdk.Ebx.UIElementWidgetReferenceEntityData;
        public new Assets.UIWidgetBlueprint Blueprint => blueprint as Assets.UIWidgetBlueprint;
        public override IEnumerable<ConnectionDesc> Properties
        {
            get
            {
                List<ConnectionDesc> outProperties = new List<ConnectionDesc>();
                outProperties.AddRange(base.Properties);
                outProperties.Add(new ConnectionDesc("Visible", Direction.In, typeof(bool)));
                outProperties.Add(new ConnectionDesc("Alpha", Direction.In, typeof(float)));
                outProperties.Add(new ConnectionDesc("Color", Direction.In, typeof(Vec3)));
                outProperties.AddRange(RootEntity.ChildProperties);
                return outProperties;
            }
        }
        public override IEnumerable<string> HeaderRows
        {
            get
            {
                List<string> outHeaderRows = new List<string>();
                if (Data.InstanceName != "")
                {
                    outHeaderRows.Add(Data.InstanceName);
                }
                return outHeaderRows;
            }
        }
        public override IEnumerable<string> DebugRows
        {
            get
            {
                List<string> outDebugRows = new List<string>();
                outDebugRows.Add($"Alpha: {alphaProperty.Value}");
                outDebugRows.Add($"Visibility: {visibleProperty.Value}");
                return outDebugRows;
            }
        }
        public UIWidgetEntity RootEntity => entities[0] as UIWidgetEntity;
        public Point LayoutPosition => layoutPosition;
        public Size LayoutSize => layoutSize;
        public float Alpha
        {
            get
            {
                float retAlpha = alphaProperty.Value;

                if (parent is UIElementEntity)
                {
                    retAlpha = (parent as UIElementEntity).Alpha * retAlpha;
                }
                else if (parent is UIElementWidgetReferenceEntity)
                {
                    retAlpha = (parent as UIElementWidgetReferenceEntity).Alpha * retAlpha;
                }
                else if (parent is UIElementLayerEntity)
                {
                    retAlpha = (parent as UIElementLayerEntity).Alpha * retAlpha;
                }
                else if (parent is UIWidgetEntity)
                {
                    var widgetEntity = (parent as UIWidgetEntity);
                    retAlpha = (widgetEntity.Parent as UIElementWidgetReferenceEntity).Alpha * retAlpha;
                }

                return retAlpha;
            }
        }
        public bool Visible
        {
            get
            {
                bool visible = visibleProperty.Value;

                if (parent is UIElementEntity)
                {
                    visible &= (parent as UIElementEntity).Visible;
                }
                else if (parent is UIElementWidgetReferenceEntity)
                {
                    visible &= (parent as UIElementWidgetReferenceEntity).Visible;
                }
                else if (parent is UIElementLayerEntity)
                {
                    visible &= (parent as UIElementLayerEntity).Visible;
                }
                else if (parent is UIWidgetEntity)
                {
                    var widgetEntity = (parent as UIWidgetEntity);
                    visible &= (widgetEntity.Parent as UIElementWidgetReferenceEntity).Visible;
                }

                return visible;
            }
        }
        public Vec3 Color
        {
            get => colorProperty.Value;
        }

        protected Point layoutPosition;
        protected Size layoutSize;

        protected Property<float> alphaProperty;
        protected Property<bool> visibleProperty;
        protected Property<LinearTransform> blueprintTransformProperty;
        protected Property<Vec3> colorProperty;

        protected Point overrideOffset;

        public UIElementWidgetReferenceEntity(FrostySdk.Ebx.UIElementWidgetReferenceEntityData inData, Entity inParent, EntityWorld inWorld)
            : base(inData, inParent, inWorld)
        {
            SetFlags(EntityFlags.HasLogic);
            alphaProperty = new Property<float>(this, Property_Alpha, Data.Alpha);
            visibleProperty = new Property<bool>(this, Property_Visible, true);
            blueprintTransformProperty = new Property<LinearTransform>(this, Property_BlueprintTransform, Data.BlueprintTransform);
            colorProperty = new Property<Vec3>(this, Property_Color, Data.Color);
        }

        public UIElementWidgetReferenceEntity(FrostySdk.Ebx.UIElementWidgetReferenceEntityData inData, Entity inParent)
			: this(inData, inParent, null)
		{
        }

        public override void AddEntity(Entity inEntity)
        {
            RootEntity.AddEntity(inEntity);
        }

        public override void RemoveEntity(Entity inEntity)
        {
            RootEntity.RemoveEntity(inEntity);
        }

        public WidgetProxy CreateRenderProxy()
        {
            SetFlags(EntityFlags.RenderProxyGenerated);
            return new UIElementWidgetReferenceProxy(this);
        }

        public override SharpDX.Matrix GetTransform()
        {
            return GetLocalTransform();
        }

        public override SharpDX.Matrix GetLocalTransform()
        {
            return SharpDXUtils.FromLinearTransform(blueprintTransformProperty.Value);
        }

        public override void SetTransform(SharpDX.Matrix m, bool suppressUpdate)
        {
            blueprintTransformProperty.Value = MakeLinearTransform(m);
        }

        public void RedoLayout(Point newOffset)
        {
            overrideOffset = newOffset;
            PerformLayout();
        }

        protected override void SpawnEntities()
        {
            overrideOffset = new Point(Data.Offset.X, Data.Offset.Y);

            var objectData = Blueprint.Data.Object.GetObjectAs<FrostySdk.Ebx.GameObjectData>();
            entities.Add(CreateEntity(objectData));

            if (RootEntity != null)
            {
                RootEntity.SpawnComponents();
            }
        }

        public void PerformLayout()
        {
            if (Parent != null)
            {
                Point parentPosition;
                Size parentSize;

                if (Parent is UIElementLayerEntity)
                {
                    var widgetEntity = Parent.Parent as UIWidgetEntity;
                    parentPosition = new Point(0, 0);
                    if (widgetEntity.Parent != null)
                    {
                        parentPosition = (widgetEntity.Parent as UIElementWidgetReferenceEntity).LayoutPosition;
                        parentSize = (widgetEntity.Parent as UIElementWidgetReferenceEntity).LayoutSize;
                    }
                    else
                    {
                        parentSize = new Size(widgetEntity.Data.Size.X, widgetEntity.Data.Size.Y);
                    }
                }
                else
                {
                    var widgetEntity = Parent as IUIWidget;
                    parentPosition = widgetEntity.LayoutPosition;
                    parentSize = widgetEntity.LayoutSize;
                }

                layoutSize = new Size(Data.Size.x, Data.Size.y);
                if (!Data.UseElementSize)
                {
                    layoutSize = new Size(RootEntity.Data.Size.X, RootEntity.Data.Size.Y);
                }

                layoutPosition = new Point(
                    (-(layoutSize.Width * Data.Anchor.X) + parentSize.Width * Data.Anchor.X) + overrideOffset.X + parentPosition.X,
                    (-(layoutSize.Height * Data.Anchor.Y) + parentSize.Height * Data.Anchor.Y) + overrideOffset.Y + parentPosition.Y
                    );
            }
            else
            {
                layoutPosition = new Point(0, 0);
                layoutSize = new Size(RootEntity.Data.Size.X, RootEntity.Data.Size.Y);
            }

            if (RootEntity != null)
            {
                RootEntity.PerformLayout();
            }
        }

        public virtual void OnMouseMove(Point mousePos)
        {
            if (RootEntity != null)
            {
                RootEntity.OnMouseMove(mousePos);
            }
        }

        public virtual void OnMouseDown(Point mousePos, MouseButton button)
        {
            if (RootEntity != null)
            {
                RootEntity.OnMouseDown(mousePos, button);
            }
        }

        public virtual void OnMouseUp(Point mousePos, MouseButton button)
        {
            if (RootEntity != null)
            {
                RootEntity.OnMouseUp(mousePos, button);
            }
        }

        public override void OnDataModified()
        {
            base.OnDataModified();

            overrideOffset = new Point(Data.Offset.X, Data.Offset.Y);
        }

        public override void SetDefaultValues()
        {
            base.SetDefaultValues();

            Data.Alpha = 1.0f;
            Data.Color = new Vec3() { x = 1, y = 1, z = 1 };
            Data.Anchor = new FrostySdk.Ebx.UIElementAnchor() { X = 0.5f, Y = 0.5f };
            Data.InclusionSettings.IsHDLayer = true;
            Data.InclusionSettings.IsSDLayer = true;
            Data.InclusionSettings.IsMultiplayerLayer = true;
            Data.InclusionSettings.IsSingleplayerLayer = true;
        }
    }
}

