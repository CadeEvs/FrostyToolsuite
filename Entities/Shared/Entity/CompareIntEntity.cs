using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CompareIntEntityData))]
	public class CompareIntEntity : CompareEntityBase, IEntityData<FrostySdk.Ebx.CompareIntEntityData>
	{
        protected readonly int Property_A = Frosty.Hash.Fnv1.HashString("A");
        protected readonly int Property_B = Frosty.Hash.Fnv1.HashString("B");
        protected readonly int Event_In = Frosty.Hash.Fnv1.HashString("In");
        protected readonly int Event_AGreaterB = Frosty.Hash.Fnv1.HashString("A>B");
        protected readonly int Event_AGreaterEqualsB = Frosty.Hash.Fnv1.HashString("A>=B");
        protected readonly int Event_ALessB = Frosty.Hash.Fnv1.HashString("A<B");
        protected readonly int Event_ALessEqualB = Frosty.Hash.Fnv1.HashString("A<=B");
        protected readonly int Event_AEqualB = Frosty.Hash.Fnv1.HashString("A=B");
        protected readonly int Event_ANotEqualB = Frosty.Hash.Fnv1.HashString("A!=B");

        public new FrostySdk.Ebx.CompareIntEntityData Data => data as FrostySdk.Ebx.CompareIntEntityData;
		public override string DisplayName => "CompareInt";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
        public override IEnumerable<ConnectionDesc> Events
        {
            get => new List<ConnectionDesc>()
            {
                new ConnectionDesc("In", Direction.In),
                new ConnectionDesc() { Name = "A>B", Direction = Direction.Out },
                new ConnectionDesc() { Name = "A>=B", Direction = Direction.Out },
                new ConnectionDesc() { Name = "A=B", Direction = Direction.Out },
                new ConnectionDesc() { Name = "A!=B", Direction = Direction.Out },
                new ConnectionDesc() { Name = "A<=B", Direction = Direction.Out },
                new ConnectionDesc() { Name = "A<B", Direction = Direction.Out }
            };
        }
        public override IEnumerable<ConnectionDesc> Properties
        {
            get => new List<ConnectionDesc>()
            {
                new ConnectionDesc("A", Direction.In, typeof(int)),
                new ConnectionDesc("B", Direction.In, typeof(int))
            };
        }

        protected Property<int> aProperty;
        protected Property<int> bProperty;
        protected Event<InputEvent> inEvent;
        protected Event<OutputEvent> aGreaterBEvent;
        protected Event<OutputEvent> aGreaterEqualsBEvent;
        protected Event<OutputEvent> aLessBEvent;
        protected Event<OutputEvent> aLessEqualsBEvent;
        protected Event<OutputEvent> aEqualBEvent;
        protected Event<OutputEvent> aNotEqualBEvent;

        public CompareIntEntity(FrostySdk.Ebx.CompareIntEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
            SetFlags(EntityFlags.HasLogic);
            aProperty = new Property<int>(this, Property_A, Data.A);
            bProperty = new Property<int>(this, Property_B, Data.B);
            inEvent = new Event<InputEvent>(this, Event_In);
            aGreaterBEvent = new Event<OutputEvent>(this, Event_AGreaterB);
            aGreaterEqualsBEvent = new Event<OutputEvent>(this, Event_AGreaterEqualsB);
            aLessBEvent = new Event<OutputEvent>(this, Event_ALessB);
            aLessEqualsBEvent = new Event<OutputEvent>(this, Event_ALessEqualB);
            aEqualBEvent = new Event<OutputEvent>(this, Event_AEqualB);
            aNotEqualBEvent = new Event<OutputEvent>(this, Event_ANotEqualB);
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
            int a = aProperty.Value;
            int b = bProperty.Value;

            if (a > b) aGreaterBEvent.Execute();
            if (a >= b) aGreaterEqualsBEvent.Execute();
            if (a < b) aLessBEvent.Execute();
            if (a <= b) aLessEqualsBEvent.Execute();
            if (a == b) aEqualBEvent.Execute();
            if (a != b) aNotEqualBEvent.Execute();
        }
    }
}

