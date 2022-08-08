using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using LevelEditorPlugin.Editors;
using SharpDX;
using Point = System.Windows.Point;
using Vec2 = FrostySdk.Ebx.Vec2;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UIWidgetsElementEntityData))]
	public class UIWidgetsElementEntity : UIElementEntity, IEntityData<FrostySdk.Ebx.UIWidgetsElementEntityData>
	{
		public new FrostySdk.Ebx.UIWidgetsElementEntityData Data => data as FrostySdk.Ebx.UIWidgetsElementEntityData;
		public override string DisplayName => "UIWidgetsElement";
        public override IEnumerable<ConnectionDesc> Events
        {
			get
            {
				List<ConnectionDesc> outEvents = new List<ConnectionDesc>();
				outEvents.AddRange(base.Events);
				outEvents.Add(new ConnectionDesc("0x15302f0d", Direction.Out));
				return outEvents;
			}
        }

        protected Event<OutputEvent> mouseHoverEvent;
		protected Event<OutputEvent> mouseHover2Event;
		protected Event<OutputEvent> mouseLeaveEvent;
		protected Event<OutputEvent> mouseUpEvent;
		protected Event<OutputEvent> mouseDownEvent;
		protected Event<OutputEvent> mouseDown2Event;
		protected Property<Vec2> mousePositionProperty;
		protected Property<bool> leftMouseDownProperty;
		protected Property<bool> mouseMovedProperty;

		protected bool isHovering;
		protected Point prevMousePosition;

		public UIWidgetsElementEntity(FrostySdk.Ebx.UIWidgetsElementEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
			SetFlags(EntityFlags.HasLogic);
			mouseHoverEvent = new Event<OutputEvent>(this, 0x7dc1404b);
			mouseHover2Event = new Event<OutputEvent>(this, -1893106121);
			mouseLeaveEvent = new Event<OutputEvent>(this, 0x2b204a57);
			mouseUpEvent = new Event<OutputEvent>(this, 0x15302f0d);
			mouseDownEvent = new Event<OutputEvent>(this, -226394685);
			mouseDown2Event = new Event<OutputEvent>(this, -239013302);
			mousePositionProperty = new Property<Vec2>(this, 188929961);
			leftMouseDownProperty = new Property<bool>(this, 0x105467f6);
			mouseMovedProperty = new Property<bool>(this, -428643894);
		}

		public override void OnMouseMove(Point mousePos)
		{
            if (!Visible)
                return;

            Matrix m = GetTransform();
			Rect rect = new Rect(new Point(LayoutPosition.X + m.M41, LayoutPosition.Y + m.M42), LayoutSize);
			if (rect.Contains(mousePos))
			{
				if (!isHovering)
				{
					world.SimulationDispatcher.Invoke(() =>
					{
						mouseHoverEvent.Execute();
						mouseHover2Event.Execute();
					});
				}

				isHovering = true;				
				Point mouseRatioPoint = new Point((mousePos.X - layoutPosition.X) / LayoutSize.Width, (mousePos.Y - layoutPosition.Y) / LayoutSize.Height);

				world.SimulationDispatcher.Invoke(() =>
				{
					mousePositionProperty.Value = new Vec2() { x = (float)mouseRatioPoint.X, y = (float)mouseRatioPoint.Y };
					mouseMovedProperty.Value = prevMousePosition != mousePos;
				});
			}
			else if (isHovering)
			{
				if (isHovering)
				{
					world.SimulationDispatcher.Invoke(() =>
					{
						mouseLeaveEvent.Execute();
						leftMouseDownProperty.Value = false;
						mouseMovedProperty.Value = false;
					});
				}
				isHovering = false;
			}

			prevMousePosition = mousePos;
		}

		public override void OnMouseDown(Point mousePos, MouseButton button)
		{
			if (!Visible)
				return;

			if (isHovering && button == MouseButton.Left)
			{
				world.SimulationDispatcher.Invoke(() =>
				{
					leftMouseDownProperty.Value = true;
					mouseDownEvent.Execute();
					mouseDown2Event.Execute();
				});
			}
		}

		public override void OnMouseUp(Point mousePos, MouseButton button)
		{
			if (!Visible)
				return;

			if (isHovering && button == MouseButton.Left)
			{
				world.SimulationDispatcher.Invoke(() =>
				{
					leftMouseDownProperty.Value = false;
					mouseUpEvent.Execute();
				});
			}
		}
	}
}

