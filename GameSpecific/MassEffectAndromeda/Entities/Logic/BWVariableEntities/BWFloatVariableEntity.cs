using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.BWFloatVariableEntityData))]
	public class BWFloatVariableEntity : BWVariableEntityBase, IEntityData<FrostySdk.Ebx.BWFloatVariableEntityData>
	{
		public readonly int Property_InputValue = Frosty.Hash.Fnv1.HashString("InputValue");
		public readonly int Property_OutputValue = Frosty.Hash.Fnv1.HashString("OutputValue");

		public new FrostySdk.Ebx.BWFloatVariableEntityData Data => data as FrostySdk.Ebx.BWFloatVariableEntityData;
		public override string DisplayName => "BWFloatVariable";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
		public override IEnumerable<ConnectionDesc> Properties
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("InputValue", Direction.In),
				new ConnectionDesc("OutputValue", Direction.Out)
			};
		}
        public override IEnumerable<string> DebugRows
        {
			get => new List<string>()
			{
				$"CurrentValue: {internalValue}"
			};
        }

        protected Property<float> inputValueProperty;
		protected Property<float> outputValueProperty;
		protected float internalValue;

		public BWFloatVariableEntity(FrostySdk.Ebx.BWFloatVariableEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
			SetFlags(EntityFlags.HasLogic);
			inputValueProperty = new Property<float>(this, Property_InputValue, Data.InputValue);
			outputValueProperty = new Property<float>(this, Property_OutputValue);

			internalValue = Data.InputValue;
		}

        public override void OnPropertyChanged(int propertyHash)
        {
			if (propertyHash == inputValueProperty.NameHash)
			{
				internalValue = inputValueProperty.Value;
				if (Data.TriggerOnPropertyChange)
				{
					bool changed = outputValueProperty.Value != internalValue;
					outputValueProperty.Value = internalValue;
					onSetEvent.Execute();

					if (changed)
					{
						onChangeEvent.Execute();
					}
					return;
				}
			}

            base.OnPropertyChanged(propertyHash);
        }

        public override void OnEvent(int eventHash)
        {
			if (eventHash == setEvent.NameHash)
			{
				bool changed = internalValue != inputValueProperty.Value;
				internalValue = inputValueProperty.Value;
				onSetEvent.Execute();

				if (changed)
				{
					onChangeEvent.Execute();
				}

				return;
			}
			else if (eventHash == getEvent.NameHash)
			{
				outputValueProperty.Value = internalValue;
				return;
			}

            base.OnEvent(eventHash);
        }
    }
}

