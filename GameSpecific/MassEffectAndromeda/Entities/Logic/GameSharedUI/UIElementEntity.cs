using Frosty.Core.Viewport;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Vec3 = FrostySdk.Ebx.Vec3;
using Vec2 = FrostySdk.Ebx.Vec2;
using LinearTransform = FrostySdk.Ebx.LinearTransform;
using MouseButton = System.Windows.Input.MouseButton;

namespace LevelEditorPlugin.Entities
{
	public interface IUIWidget
	{
		Point LayoutPosition { get; }
		Size LayoutSize { get; }
		Render.Proxies.WidgetProxy CreateRenderProxy();
		void PerformLayout();
		void OnMouseMove(Point mousePos);
		void OnMouseDown(Point mousePos, MouseButton button);
		void OnMouseUp(Point mousePos, MouseButton button);
	}

	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UIElementEntityData))]
	public class UIElementEntity : LogicEntity, IEntityData<FrostySdk.Ebx.UIElementEntityData>, ITimelineCustomTrackName, IHideInSchematicsView, IUIWidget, ITransformEntity
	{
		protected readonly int Property_Alpha = Frosty.Hash.Fnv1.HashString("Alpha");
		protected readonly int Property_Visible = Frosty.Hash.Fnv1.HashString("Visible");
		protected readonly int Property_Transform = Frosty.Hash.Fnv1.HashString("Transform");
		protected readonly int Property_Color = Frosty.Hash.Fnv1.HashString("Color");
		protected readonly int Property_ElementSize = Frosty.Hash.Fnv1.HashString("ElementSize");

		public new FrostySdk.Ebx.UIElementEntityData Data => data as FrostySdk.Ebx.UIElementEntityData;
		public override string DisplayName => "UIElement";
		string ITimelineCustomTrackName.DisplayName => Data.InstanceName;
        public override FrostySdk.Ebx.Realm Realm => FrostySdk.Ebx.Realm.Realm_Client;
        public override IEnumerable<ConnectionDesc> Properties
        {
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("Alpha", Direction.In, typeof(float)),
				new ConnectionDesc("Visible", Direction.In, typeof(bool)),
				new ConnectionDesc("Transform", Direction.In, typeof(LinearTransform)),
				new ConnectionDesc("Color", Direction.In, typeof(Vec3)),
				new ConnectionDesc("ElementSize", Direction.Out, typeof(Vec2))
			};
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
		protected Property<LinearTransform> transformProperty;
		protected Property<bool> visibleProperty;
		protected Property<Vec3> colorProperty;
		protected Property<Vec2> elementSizeProperty;

        public UIElementEntity(FrostySdk.Ebx.UIElementEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
			SetFlags(EntityFlags.HasLogic);
			alphaProperty = new Property<float>(this, Property_Alpha, Data.Alpha, "Alpha");
			transformProperty = new Property<LinearTransform>(this, Property_Transform, Data.Transform, "Transform");
			visibleProperty = new Property<bool>(this, Property_Visible, Data.Visible, "Visible");
			colorProperty = new Property<Vec3>(this, Property_Color, Data.Color, "Color");
			elementSizeProperty = new Property<Vec2>(this, Property_ElementSize);
		}

        public override void BeginSimulation()
        {
            base.BeginSimulation();
			elementSizeProperty.Value = new Vec2() { x = Data.Size.x, y = Data.Size.y };
        }

        public virtual Render.Proxies.WidgetProxy CreateRenderProxy()
		{
			SetFlags(EntityFlags.RenderProxyGenerated);
			return new Render.Proxies.UIElementProxy(this);
		}

		public SharpDX.Matrix GetTransform()
		{
			SharpDX.Matrix m = SharpDX.Matrix.Identity;
			if (parent is UIElementEntity)
			{
				m = (parent as UIElementEntity).GetTransform();
			}
			else if (parent is UIElementLayerEntity)
			{
				var widget = (parent as UIElementLayerEntity).Parent as UIWidgetEntity;
				m = (widget.Parent as UIElementWidgetReferenceEntity).GetTransform();
			}

			return GetLocalTransform() * m;
		}

		public SharpDX.Matrix GetLocalTransform()
		{
			return SharpDXUtils.FromLinearTransform(transformProperty.Value);
		}

		public void SetTransform(SharpDX.Matrix m, bool suppressUpdate)
		{
			transformProperty.Value = MakeLinearTransform(m);
			// do not save yet
		}

        public virtual void PerformLayout()
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

			if (Data.LayoutMode == FrostySdk.Ebx.UILayoutMode.UILayoutMode_AnchorOffset)
			{
				layoutSize = new Size(Data.Size.x, Data.Size.y);
				layoutPosition = new Point(
					(-(Data.Size.x * Data.Anchor.X) + parentSize.Width * Data.Anchor.X) + Data.Offset.X + parentPosition.X,
					(-(Data.Size.y * Data.Anchor.Y) + parentSize.Height * Data.Anchor.Y) + Data.Offset.Y + parentPosition.Y
					);
			}
			else
            {
				layoutPosition = new Point(Data.Position.X + parentPosition.X, Data.Position.Y + parentPosition.Y);
				layoutSize = new Size(parentSize.Width * Data.Expansion.Width, parentSize.Height * Data.Expansion.Height);
            }
		}

		public virtual void OnMouseMove(Point mousePos)
		{
		}

		public virtual void OnMouseDown(Point mousePos, MouseButton button)
		{
		}

		public virtual void OnMouseUp(Point mousePos, MouseButton button)
		{
		}

        public override void SetDefaultValues()
        {
            base.SetDefaultValues();

			Data.UIElementTransform.RotationPivot = new Vec3() { x = 0.5f, y = 0.5f, z = 0.0f };
			Data.Anchor = new FrostySdk.Ebx.UIElementAnchor() { X = 0.5f, Y = 0.5f };
			Data.Color = new Vec3() { x = 1, y = 1, z = 1 };
			Data.Alpha = 1.0f;
			Data.Visible = true;
		}

        public override void OnDataModified()
        {
            base.OnDataModified();

			alphaProperty.Value = Data.Alpha;
			transformProperty.Value = Data.Transform;
			visibleProperty.Value = Data.Visible;
			colorProperty.Value = Data.Color;
			elementSizeProperty.Value = Data.Size;
		}
    }
}

