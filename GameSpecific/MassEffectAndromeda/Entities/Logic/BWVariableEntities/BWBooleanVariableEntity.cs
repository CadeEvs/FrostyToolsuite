using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.BWBooleanVariableEntityData))]
	public class BWBooleanVariableEntity : BWVariableEntityBase, IEntityData<FrostySdk.Ebx.BWBooleanVariableEntityData>
	{
        protected readonly int Event_SetTrue = Frosty.Hash.Fnv1.HashString("SetTrue");
        protected readonly int Event_SetFalse = Frosty.Hash.Fnv1.HashString("SetFalse");
        protected readonly int Property_InputValue = Frosty.Hash.Fnv1.HashString("InputValue");
        protected readonly int Property_OutputValue = Frosty.Hash.Fnv1.HashString("OutputValue");

        public new FrostySdk.Ebx.BWBooleanVariableEntityData Data => data as FrostySdk.Ebx.BWBooleanVariableEntityData;
		public override string DisplayName => "BWBooleanVariable";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
        public override IEnumerable<ConnectionDesc> Events
        {
            get
            {
                List<ConnectionDesc> outEvents = new List<ConnectionDesc>();
                outEvents.AddRange(base.Events);
                outEvents.AddRange(new[]
                {
                    new ConnectionDesc("SetTrue", Direction.In),
                    new ConnectionDesc("SetFalse", Direction.In),
                    new ConnectionDesc("OnTrue", Direction.Out),
                    new ConnectionDesc("OnFalse", Direction.Out)
                });
                return outEvents;
            }
        }
        public override IEnumerable<ConnectionDesc> Properties
        {
            get => new List<ConnectionDesc>()
            {
                new ConnectionDesc("InputValue", Direction.In),
                new ConnectionDesc("OutputValue", Direction.Out)
            };
        }

        protected Event<InputEvent> setTrueEvent;
        protected Event<InputEvent> setFalseEvent;
        protected Property<bool> inputValueProperty;
        protected Property<bool> outputValueProperty;

        public BWBooleanVariableEntity(FrostySdk.Ebx.BWBooleanVariableEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
            SetFlags(EntityFlags.HasLogic);
            setTrueEvent = new Event<InputEvent>(this, Event_SetTrue);
            setFalseEvent = new Event<InputEvent>(this, Event_SetFalse);
            inputValueProperty = new Property<bool>(this, Property_InputValue, Data.InputValue);
            outputValueProperty = new Property<bool>(this, Property_OutputValue);
		}

        public override void BeginSimulation()
        {
            base.BeginSimulation();

            if (Data.TriggerOnInitialization)
            {
                outputValueProperty.Value = inputValueProperty.Value;
            }
        }

        public override void OnEvent(int eventHash)
        {
            if (eventHash == setTrueEvent.NameHash)
            {
                outputValueProperty.Value = true;
                return;
            }
            else if (eventHash == setFalseEvent.NameHash)
            {
                outputValueProperty.Value = false;
                return;
            }
            base.OnEvent(eventHash);
        }

        public override void OnPropertyChanged(int propertyHash)
        {
            if (propertyHash == inputValueProperty.NameHash)
            {
                if (Data.TriggerOnPropertyChange)
                {
                    outputValueProperty.Value = inputValueProperty.Value;
                    return;
                }
            }

            base.OnPropertyChanged(propertyHash);
        }
    }
}

