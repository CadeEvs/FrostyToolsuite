using System.Collections.Generic;
using CString = FrostySdk.Ebx.CString;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CompareStringEntityData))]
	public class CompareStringEntity : CompareEntityBase, IEntityData<FrostySdk.Ebx.CompareStringEntityData>
	{
		protected readonly int Property_A = Frosty.Hash.Fnv1.HashString("A");
		protected readonly int Property_B = Frosty.Hash.Fnv1.HashString("B");
		protected readonly int Event_In = Frosty.Hash.Fnv1.HashString("In");
		protected readonly int Event_AEqualB = Frosty.Hash.Fnv1.HashString("A=B");
		protected readonly int Event_ANotEqualB = Frosty.Hash.Fnv1.HashString("A!=B");

		public new FrostySdk.Ebx.CompareStringEntityData Data => data as FrostySdk.Ebx.CompareStringEntityData;
		public override string DisplayName => "CompareString";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
		public override IEnumerable<ConnectionDesc> Events
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("In", Direction.In),
				new ConnectionDesc() { Name = "A=B", Direction = Direction.Out },
				new ConnectionDesc() { Name = "A!=B", Direction = Direction.Out }
			};
		}
		public override IEnumerable<ConnectionDesc> Properties
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("A", Direction.In, typeof(CString)),
				new ConnectionDesc("B", Direction.In, typeof(CString))
			};
		}

		protected Property<CString> aProperty;
		protected Property<CString> bProperty;
		protected Event<InputEvent> inEvent;
		protected Event<OutputEvent> aEqualBEvent;
		protected Event<OutputEvent> aNotEqualBEvent;

		public CompareStringEntity(FrostySdk.Ebx.CompareStringEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
			SetFlags(EntityFlags.HasLogic);
			aProperty = new Property<CString>(this, Property_A, Data.A);
			bProperty = new Property<CString>(this, Property_B, Data.B);
			inEvent = new Event<InputEvent>(this, Event_In);
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
			string a = aProperty.Value;
			string b = bProperty.Value;

			if (a.Equals(b)) aEqualBEvent.Execute();
			else aNotEqualBEvent.Execute();
		}
	}
}

