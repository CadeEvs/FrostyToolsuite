using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CompareBoolEntityData))]
	public class CompareBoolEntity : CompareEntityBase, IEntityData<FrostySdk.Ebx.CompareBoolEntityData>
	{
        protected readonly int Property_Bool = Frosty.Hash.Fnv1.HashString("Bool");
        protected readonly int Event_In = Frosty.Hash.Fnv1.HashString("In");
        protected readonly int Event_OnTrue = Frosty.Hash.Fnv1.HashString("OnTrue");
        protected readonly int Event_OnFalse = Frosty.Hash.Fnv1.HashString("OnFalse");

        public new FrostySdk.Ebx.CompareBoolEntityData Data => data as FrostySdk.Ebx.CompareBoolEntityData;
		public override string DisplayName => "CompareBool";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
        public override IEnumerable<ConnectionDesc> Events
        {
            get => new List<ConnectionDesc>()
            {
                new ConnectionDesc("In", Direction.In),
                new ConnectionDesc("OnTrue", Direction.Out),
                new ConnectionDesc("OnFalse", Direction.Out)
            };
        }
        public override IEnumerable<ConnectionDesc> Properties
        {
            get => new List<ConnectionDesc>()
            {
                new ConnectionDesc("Bool", Direction.In, typeof(bool))
            };
        }

        protected Property<bool> boolProperty;
        protected Event<InputEvent> inEvent;
        protected Event<OutputEvent> onTrueEvent;
        protected Event<OutputEvent> onFalseEvent;

        public CompareBoolEntity(FrostySdk.Ebx.CompareBoolEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
            SetFlags(EntityFlags.HasLogic);
            boolProperty = new Property<bool>(this, Property_Bool, Data.Bool);
            inEvent = new Event<InputEvent>(this, Event_In);
            onTrueEvent = new Event<OutputEvent>(this, Event_OnTrue);
            onFalseEvent = new Event<OutputEvent>(this, Event_OnFalse);
		}

        public override void BeginSimulation()
        {
            base.BeginSimulation();

            if (Data.TriggerOnStart)
            {
                Execute();
            }
        }

        public override void OnEvent(int eventHash)
        {
            if (eventHash == inEvent.NameHash)
            {
                Execute();
                return;
            }

            base.OnEvent(eventHash);
        }

        public override void OnPropertyChanged(int propertyHash)
        {
            if (Data.TriggerOnPropertyChange && propertyHash == boolProperty.NameHash)
            {
                Execute();
                return;
            }

            base.OnPropertyChanged(propertyHash);
        }

        private void Execute()
        {
            bool value = boolProperty.Value;
            if (value) onTrueEvent.Execute();
            else onFalseEvent.Execute();
        }
    }
}

