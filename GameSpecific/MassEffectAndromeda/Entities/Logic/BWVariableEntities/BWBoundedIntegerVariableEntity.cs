using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.BWBoundedIntegerVariableEntityData))]
	public class BWBoundedIntegerVariableEntity : BWBoundedVariableEntity, IEntityData<FrostySdk.Ebx.BWBoundedIntegerVariableEntityData>
	{
        protected readonly int Property_DefaultValue = Frosty.Hash.Fnv1.HashString("DefaultValue");
        protected readonly int Property_MinValue = Frosty.Hash.Fnv1.HashString("MinValue");
        protected readonly int Property_MaxValue = Frosty.Hash.Fnv1.HashString("MaxValue");
        protected readonly int Property_Out = Frosty.Hash.Fnv1.HashString("Out");
        protected readonly int Event_Increment = Frosty.Hash.Fnv1.HashString("Increment");
        protected readonly int Event_Decrement = Frosty.Hash.Fnv1.HashString("Decrement");

        public new FrostySdk.Ebx.BWBoundedIntegerVariableEntityData Data => data as FrostySdk.Ebx.BWBoundedIntegerVariableEntityData;
		public override string DisplayName => "BWBoundedIntegerVariable";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
        public override IEnumerable<ConnectionDesc> Events
        {
            get => new List<ConnectionDesc>()
            {
                new ConnectionDesc("Increment", Direction.In),
                new ConnectionDesc("Decrement", Direction.In),
            };
        }
        public override IEnumerable<ConnectionDesc> Properties
        {
            get => new List<ConnectionDesc>()
            {
                new ConnectionDesc("DefaultValue", Direction.In, typeof(int)),
                new ConnectionDesc("MinValue", Direction.In, typeof(int)),
                new ConnectionDesc("MaxValue", Direction.In, typeof(int)),
                new ConnectionDesc("Out", Direction.Out, typeof(int)),
            };
        }

        protected Property<int> defaultValueProperty;
        protected Property<int> minValueProperty;
        protected Property<int> maxValueProperty;
        protected Property<int> outProperty;

        protected Event<InputEvent> incrementEvent;
        protected Event<InputEvent> decrementEvent;

        public BWBoundedIntegerVariableEntity(FrostySdk.Ebx.BWBoundedIntegerVariableEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
            SetFlags(EntityFlags.HasLogic);
            defaultValueProperty = new Property<int>(this, Property_DefaultValue, Data.DefaultValue);
            minValueProperty = new Property<int>(this, Property_MinValue, Data.MinValue);
            maxValueProperty = new Property<int>(this, Property_MaxValue, Data.MaxValue);
            outProperty = new Property<int>(this, Property_Out);

            incrementEvent = new Event<InputEvent>(this, Event_Increment);
            decrementEvent = new Event<InputEvent>(this, Event_Decrement);
		}

        public override void OnEvent(int eventHash)
        {
            if (eventHash == incrementEvent.NameHash)
            {
                UpdateValue(outProperty.Value + 1);
                return;
            }
            else if (eventHash == decrementEvent.NameHash)
            {
                UpdateValue(outProperty.Value - 1);
                return;
            }

            base.OnEvent(eventHash);
        }

        public override void OnPropertyChanged(int propertyHash)
        {
            if (propertyHash == defaultValueProperty.NameHash)
            {
                UpdateValue(defaultValueProperty.Value);
                return;
            }
            base.OnPropertyChanged(propertyHash);
        }

        private void UpdateValue(int newValue)
        {
            if (newValue > maxValueProperty.Value)
            {
                newValue = maxValueProperty.Value;
            }
            else if (newValue < minValueProperty.Value)
            {
                newValue = minValueProperty.Value;
            }
            outProperty.Value = newValue;
        }
    }
}

