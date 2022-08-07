using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CompareFloatEntityData))]
	public class CompareFloatEntity : CompareEntityBase, IEntityData<FrostySdk.Ebx.CompareFloatEntityData>
	{
        protected readonly int Property_A = Frosty.Hash.Fnv1.HashString("A");
        protected readonly int Property_B = Frosty.Hash.Fnv1.HashString("B");
        protected readonly int Event_In = Frosty.Hash.Fnv1.HashString("In");
        protected readonly int Event_AGreaterB = Frosty.Hash.Fnv1.HashString("A>B");
        protected readonly int Event_AGreaterEqualsB = Frosty.Hash.Fnv1.HashString("A>=B");
        protected readonly int Event_ALessB = Frosty.Hash.Fnv1.HashString("A<B");
        protected readonly int Event_ALessEqualB = Frosty.Hash.Fnv1.HashString("A<=B");

        public new FrostySdk.Ebx.CompareFloatEntityData Data => data as FrostySdk.Ebx.CompareFloatEntityData;
		public override string DisplayName => "CompareFloat";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
        public override IEnumerable<ConnectionDesc> Events
        {
            get => new List<ConnectionDesc>()
            {
                new ConnectionDesc("In", Direction.In),
                new ConnectionDesc() { Name = "A>B", Direction = Direction.Out },
                new ConnectionDesc() { Name = "A>=B", Direction = Direction.Out },
                new ConnectionDesc() { Name = "A<=B", Direction = Direction.Out },
                new ConnectionDesc() { Name = "A<B", Direction = Direction.Out }
            };
        }
        public override IEnumerable<ConnectionDesc> Properties
        {
            get => new List<ConnectionDesc>()
            {
                new ConnectionDesc("A", Direction.In, typeof(float)),
                new ConnectionDesc("B", Direction.In, typeof(float))
            };
        }

        protected Property<float> aProperty;
        protected Property<float> bProperty;
        protected Event<InputEvent> inEvent;
        protected Event<OutputEvent> aGreaterBEvent;
        protected Event<OutputEvent> aGreaterEqualsBEvent;
        protected Event<OutputEvent> aLessBEvent;
        protected Event<OutputEvent> aLessEqualsBEvent;

        public CompareFloatEntity(FrostySdk.Ebx.CompareFloatEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
            SetFlags(EntityFlags.HasLogic);
            aProperty = new Property<float>(this, Property_A, Data.A);
            bProperty = new Property<float>(this, Property_B, Data.B);
            inEvent = new Event<InputEvent>(this, Event_In);
            aGreaterBEvent = new Event<OutputEvent>(this, Event_AGreaterB);
            aGreaterEqualsBEvent = new Event<OutputEvent>(this, Event_AGreaterEqualsB);
            aLessBEvent = new Event<OutputEvent>(this, Event_ALessB);
            aLessEqualsBEvent = new Event<OutputEvent>(this, Event_ALessEqualB);
		}

        public override void BeginSimulation()
        {
            base.BeginSimulation();
            if (Data.TriggerOnStart)
            {
                PerformCondition();
            }
        }

        public override void OnEvent(int eventHash)
        {
            if (eventHash == inEvent.NameHash)
            {
                PerformCondition();
                return;
            }
            base.OnEvent(eventHash);
        }

        public override void OnPropertyChanged(int propertyHash)
        {
            if (propertyHash == aProperty.NameHash || propertyHash == bProperty.NameHash)
            {
                if (Data.TriggerOnPropertyChange)
                {
                    PerformCondition();
                    return;
                }
            }

            base.OnPropertyChanged(propertyHash);
        }

        private void PerformCondition()
        {
            float a = aProperty.Value;
            float b = bProperty.Value;

            if (a > b) aGreaterBEvent.Execute();
            if (a >= b) aGreaterEqualsBEvent.Execute();
            if (a < b) aLessBEvent.Execute();
            if (a <= b) aLessEqualsBEvent.Execute();
        }
    }
}

