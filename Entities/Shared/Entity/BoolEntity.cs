using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.BoolEntityData))]
	public class BoolEntity : LogicEntity, IEntityData<FrostySdk.Ebx.BoolEntityData>
	{
        protected readonly int Event_SetTrue = Frosty.Hash.Fnv1.HashString("SetTrue");
        protected readonly int Event_SetFalse = Frosty.Hash.Fnv1.HashString("SetFalse");
        protected readonly int Property_Value = Frosty.Hash.Fnv1.HashString("Value");

        public new FrostySdk.Ebx.BoolEntityData Data => data as FrostySdk.Ebx.BoolEntityData;
		public override string DisplayName => "Bool";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
        public override IEnumerable<ConnectionDesc> Events
        {
            get => new List<ConnectionDesc>()
            {
                new ConnectionDesc("SetTrue", Direction.In),
                new ConnectionDesc("SetFalse", Direction.In),
                new ConnectionDesc("Toggle", Direction.In)
            };
        }

        public override IEnumerable<ConnectionDesc> Properties
        {
            get => new List<ConnectionDesc>()
            {
                new ConnectionDesc("DefaultValue", Direction.In),
                new ConnectionDesc("Value", Direction.Out)
            };
        }

        protected Property<bool> valueProperty;
        protected Event<InputEvent> setTrueEvent;
        protected Event<InputEvent> setFalseEvent;

        public BoolEntity(FrostySdk.Ebx.BoolEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
            SetFlags(EntityFlags.HasLogic);
            valueProperty = new Property<bool>(this, Property_Value);
            setTrueEvent = new Event<InputEvent>(this, Event_SetTrue);
            setFalseEvent = new Event<InputEvent>(this, Event_SetFalse);
		}

        public override void BeginSimulation()
        {
            base.BeginSimulation();

            if (valueProperty.IsUnset)
            {
                valueProperty.Value = Data.DefaultValue;
            }
        }

        public override void OnEvent(int eventHash)
        {
            if (eventHash == setTrueEvent.NameHash)
            {
                valueProperty.Value = true;
                return;
            }
            else if (eventHash == setFalseEvent.NameHash)
            {
                valueProperty.Value = false;
                return;
            }

            base.OnEvent(eventHash);
        }
    }
}

