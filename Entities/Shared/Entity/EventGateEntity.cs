using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.EventGateEntityData))]
	public class EventGateEntity : LogicEntity, IEntityData<FrostySdk.Ebx.EventGateEntityData>
	{
		protected readonly int Event_In = Frosty.Hash.Fnv1.HashString("In");
		protected readonly int Event_Open = Frosty.Hash.Fnv1.HashString("Open");
		protected readonly int Event_Close = Frosty.Hash.Fnv1.HashString("Close");
		protected readonly int Event_Out = Frosty.Hash.Fnv1.HashString("Out");

		public new FrostySdk.Ebx.EventGateEntityData Data => data as FrostySdk.Ebx.EventGateEntityData;
		public override string DisplayName => "EventGate";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
		public override IEnumerable<ConnectionDesc> Events
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("In", Direction.In),
				new ConnectionDesc("Open", Direction.In),
				new ConnectionDesc("Close", Direction.In),
				new ConnectionDesc("Out", Direction.Out),
			};
		}
        public override IEnumerable<string> DebugRows
        {
			get
            {
				List<string> outDebugRows = new List<string>();
				outDebugRows.Add($"Gate: {((gateOpen) ? "Open" : "Closed")}");
				return outDebugRows;
            }
        }

        protected Event<InputEvent> inEvent;
		protected Event<InputEvent> openEvent;
		protected Event<InputEvent> closeEvent;
		protected Event<OutputEvent> outEvent;

		protected bool gateOpen;

		public EventGateEntity(FrostySdk.Ebx.EventGateEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
			SetFlags(EntityFlags.HasLogic);
			inEvent = new Event<InputEvent>(this, Event_In);
			openEvent = new Event<InputEvent>(this, Event_Open);
			closeEvent = new Event<InputEvent>(this, Event_Close);
			outEvent = new Event<OutputEvent>(this, Event_Out);

			gateOpen = Data.Default;
		}

        public override void OnEvent(int eventHash)
        {
			if (eventHash == inEvent.NameHash)
			{
				if (gateOpen)
				{
					outEvent.Execute();
					return;
				}
			}
			else if (eventHash == openEvent.NameHash)
			{
				gateOpen = true;
				return;
			}
			else if (eventHash == closeEvent.NameHash)
			{
				gateOpen = false;
				return;
			}

            base.OnEvent(eventHash);
        }
    }
}

