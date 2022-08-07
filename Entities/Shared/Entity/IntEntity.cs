using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.IntEntityData))]
	public class IntEntity : LogicEntity, IEntityData<FrostySdk.Ebx.IntEntityData>
	{
		protected readonly int Event_Increment = Frosty.Hash.Fnv1.HashString("Increment");
		protected readonly int Event_Decrement = Frosty.Hash.Fnv1.HashString("Decrement");
		protected readonly int Property_Value = Frosty.Hash.Fnv1.HashString("Value");

		public new FrostySdk.Ebx.IntEntityData Data => data as FrostySdk.Ebx.IntEntityData;
		public override string DisplayName => "Int";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
		public override IEnumerable<ConnectionDesc> Events
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("Increment", Direction.In),
				new ConnectionDesc("Decrement", Direction.In)
			};
		}
		public override IEnumerable<ConnectionDesc> Properties
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("Value", Direction.Out, typeof(int))
			};
		}
        public override IEnumerable<string> HeaderRows
        {
			get => new List<string>()
			{
				$"DefaultValue: {Data.DefaultValue}"
			};
        }

		protected Event<InputEvent> incrementEvent;
		protected Event<InputEvent> decrementEvent;
		protected Property<int> valueProperty;

        public IntEntity(FrostySdk.Ebx.IntEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
			SetFlags(EntityFlags.HasLogic);
			incrementEvent = new Event<InputEvent>(this, Event_Increment);
			decrementEvent = new Event<InputEvent>(this, Event_Decrement);
			valueProperty = new Property<int>(this, Property_Value);
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
			if (eventHash == incrementEvent.NameHash)
			{
				valueProperty.Value = valueProperty.Value + Data.IncDecValue;
				return;
			}
			else if (eventHash == decrementEvent.NameHash)
            {
				valueProperty.Value = valueProperty.Value - Data.IncDecValue;
				return;
            }

            base.OnEvent(eventHash);
        }

        public override void SetDefaultValues()
        {
            base.SetDefaultValues();
			Data.IncDecValue = 1;
        }
    }
}

