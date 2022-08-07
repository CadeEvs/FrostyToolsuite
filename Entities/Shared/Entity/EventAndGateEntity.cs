using System.Collections.Generic;
using System.Linq;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.EventAndGateEntityData))]
	public class EventAndGateEntity : LogicEntity, IEntityData<FrostySdk.Ebx.EventAndGateEntityData>
	{
        protected readonly int Event_Reset = Frosty.Hash.Fnv1.HashString("Reset");
        protected readonly int Event_Out = Frosty.Hash.Fnv1.HashString("Out");

		public new FrostySdk.Ebx.EventAndGateEntityData Data => data as FrostySdk.Ebx.EventAndGateEntityData;
		public override string DisplayName => "EventAndGate";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
        public override IEnumerable<ConnectionDesc> Events
        {
            get
            {
                List<ConnectionDesc> outEvents = new List<ConnectionDesc>();
                for (int i = 0; i < Data.EventCount; i++)
                {
                    outEvents.Add(new ConnectionDesc() { Name = $"In{i + 1}", Direction = Direction.In });
                }
                outEvents.Add(new ConnectionDesc("Reset", Direction.In));
                outEvents.Add(new ConnectionDesc("Out", Direction.Out));
                return outEvents;
            }
        }

        protected List<Event<InputEvent>> inEvents = new List<Event<InputEvent>>();
        protected Event<InputEvent> resetEvent;
        protected Event<OutputEvent> outEvent;

        protected bool[] gateValues;

        public EventAndGateEntity(FrostySdk.Ebx.EventAndGateEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
            SetFlags(EntityFlags.HasLogic);
            gateValues = new bool[Data.EventCount];
            for (int i = 0; i < Data.EventCount; i++)
            {
                inEvents.Add(new Event<InputEvent>(this, Frosty.Hash.Fnv1.HashString($"In{i + 1}")));
            }
            resetEvent = new Event<InputEvent>(this, Event_Reset);
            outEvent = new Event<OutputEvent>(this, Event_Out);
		}

        public override void OnEvent(int eventHash)
        {
            int index = inEvents.FindIndex(e => e.NameHash == eventHash);
            if (index != -1)
            {
                gateValues[index] = true;
                if (gateValues.All(b => b == true))
                {
                    outEvent.Execute();
                }
                return;
            }
            else if (eventHash == resetEvent.NameHash)
            {
                for (int i = 0; i < gateValues.Length; i++)
                {
                    gateValues[i] = false;
                }
                return;
            }

            base.OnEvent(eventHash);
        }
    }
}

