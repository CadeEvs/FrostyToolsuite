using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.EventSwitchEntityData))]
	public class EventSwitchEntity : LogicEntity, IEntityData<FrostySdk.Ebx.EventSwitchEntityData>
	{
        protected readonly int Event_In = Frosty.Hash.Fnv1.HashString("In");
        protected readonly int Event_Reset = Frosty.Hash.Fnv1.HashString("Reset");

        public new FrostySdk.Ebx.EventSwitchEntityData Data => data as FrostySdk.Ebx.EventSwitchEntityData;
		public override string DisplayName => "EventSwitch";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
        public override IEnumerable<ConnectionDesc> Events
        {
            get
            {
                List<ConnectionDesc> outEvents = new List<ConnectionDesc>();

                outEvents.Add(new ConnectionDesc("In", Direction.In));
                outEvents.Add(new ConnectionDesc("Reset", Direction.In));

                for (int i = 0; i < Data.OutEvents; i++)
                {
                    outEvents.Add(new ConnectionDesc() { Name = $"Out{i}", Direction = Direction.Out });
                }

                return outEvents;
            }
        }

        protected Event<InputEvent> inEvent;
        protected Event<InputEvent> resetEvent;
        protected List<Event<OutputEvent>> outEvents = new List<Event<OutputEvent>>();
        protected int eventIndex;

        public EventSwitchEntity(FrostySdk.Ebx.EventSwitchEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
            SetFlags(EntityFlags.HasLogic);
            inEvent = new Event<InputEvent>(this, Event_In);
            resetEvent = new Event<InputEvent>(this, Event_Reset);

            for (int i = 0; i < Data.OutEvents; i++)
            {
                int hash = Frosty.Hash.Fnv1.HashString($"Out{i}");
                outEvents.Add(new Event<OutputEvent>(this, hash));
            }
		}

        public override void OnEvent(int eventHash)
        {
            if (eventHash == inEvent.NameHash)
            {
                outEvents[eventIndex].Execute();
                if (Data.AutoIncrement)
                {
                    eventIndex++;
                    if (eventIndex >= outEvents.Count)
                    {
                        eventIndex = 0;
                    }
                }
                return;
            }
            else if (eventHash == resetEvent.NameHash)
            {
                eventIndex = 0;
                return;
            }

            base.OnEvent(eventHash);
        }
    }
}

