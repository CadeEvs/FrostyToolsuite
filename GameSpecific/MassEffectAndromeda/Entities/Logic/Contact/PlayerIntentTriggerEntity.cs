using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PlayerIntentTriggerEntityData))]
	public class PlayerIntentTriggerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.PlayerIntentTriggerEntityData>
	{
        protected readonly int Event_Enable = Frosty.Hash.Fnv1.HashString("Enable");
        protected readonly int Event_Disable = Frosty.Hash.Fnv1.HashString("Disable");
        protected readonly int Event_OnTrigger = Frosty.Hash.Fnv1.HashString("OnTrigger");
        protected readonly int Event_OnPressed = Frosty.Hash.Fnv1.HashString("OnPressed");
        protected readonly int Event_OnReleased = Frosty.Hash.Fnv1.HashString("OnReleased");

        public new FrostySdk.Ebx.PlayerIntentTriggerEntityData Data => data as FrostySdk.Ebx.PlayerIntentTriggerEntityData;
		public override string DisplayName => "PlayerIntentTrigger";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
        public override IEnumerable<ConnectionDesc> Events
        {
            get => new List<ConnectionDesc>()
            {
                new ConnectionDesc("Enable", Direction.In),
                new ConnectionDesc("Disable", Direction.In),
                new ConnectionDesc("OnTrigger", Direction.Out, true),
                new ConnectionDesc("OnPressed", Direction.Out, true),
                new ConnectionDesc("OnReleased", Direction.Out, true)
            };
        }
        public override IEnumerable<ConnectionDesc> Properties
        {
            get => new List<ConnectionDesc>()
            {
                new ConnectionDesc("Value", Direction.Out)
            };
        }

        protected Event<InputEvent> enableEvent;
        protected Event<InputEvent> disableEvent;
        protected Event<OutputEvent> onTriggerEvent;
        protected Event<OutputEvent> onPressedEvent;
        protected Event<OutputEvent> onReleasedEvent;

        public PlayerIntentTriggerEntity(FrostySdk.Ebx.PlayerIntentTriggerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
            SetFlags(EntityFlags.HasLogic);
            enableEvent = new Event<InputEvent>(this, Event_Enable);
            disableEvent = new Event<InputEvent>(this, Event_Disable);
            onTriggerEvent = new Event<OutputEvent>(this, Event_OnTrigger);
            onPressedEvent = new Event<OutputEvent>(this, Event_OnPressed);
            onReleasedEvent = new Event<OutputEvent>(this, Event_OnReleased);
		}
    }
}

