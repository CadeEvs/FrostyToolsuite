using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SelectFloatEntityData))]
	public class SelectFloatEntity : SelectPropertyEntity, IEntityData<FrostySdk.Ebx.SelectFloatEntityData>
	{
		protected readonly int Event_OnSelected = Frosty.Hash.Fnv1.HashString("OnSelected");
		protected readonly int Property_Out = Frosty.Hash.Fnv1.HashString("Out");

		public new FrostySdk.Ebx.SelectFloatEntityData Data => data as FrostySdk.Ebx.SelectFloatEntityData;
		public override string DisplayName => "SelectFloat";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
		public override IEnumerable<ConnectionDesc> Properties
		{
			get
			{
				List<ConnectionDesc> outProperties = new List<ConnectionDesc>();
				outProperties.AddRange(base.Properties);
				outProperties.Add(new ConnectionDesc("Out", Direction.Out, typeof(float)));
				return outProperties;
			}
		}

		protected List<Property<float>> inputProperties = new List<Property<float>>();
		protected Property<float> outProperty;

		protected Event<OutputEvent> onSelectedEvent;
		protected List<Event<InputEvent>> selectEvents = new List<Event<InputEvent>>();

		public SelectFloatEntity(FrostySdk.Ebx.SelectFloatEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
			SetFlags(EntityFlags.HasLogic);
			for (int i = 0; i < inData.Inputs.Count; i++)
			{
				int selectEvent = Frosty.Hash.Fnv1.HashString($"Select{i} {inData.Inputs[i]}");
				Property<float> property = new Property<float>(this, Frosty.Hash.Fnv1.HashString($"In{i} {inData.Inputs[i]}"));

				inputProperties.Add(property);
				selectEvents.Add(new Event<InputEvent>(this, selectEvent));
			}

			outProperty = new Property<float>(this, Property_Out);
			onSelectedEvent = new Event<OutputEvent>(this, Event_OnSelected);
		}

        public override void OnPropertyChanged(int propertyHash)
		{
			if (propertyHash == inputProperties[inputSelectProperty.Value].NameHash)
			{
				outProperty.Value = inputProperties[inputSelectProperty.Value].Value;
				onSelectedEvent.Execute();
				return;
			}
			else if (propertyHash == inputSelectProperty.NameHash)
			{
				outProperty.Value = inputProperties[inputSelectProperty.Value].Value;
				onSelectedEvent.Execute();
			}

			base.OnPropertyChanged(propertyHash);
		}

		public override void OnEvent(int eventHash)
		{
			int eventIndex = selectEvents.FindIndex(e => e.NameHash == eventHash);
			if (eventIndex != -1)
			{
				inputSelectProperty.Value = eventIndex;
				return;
			}

			base.OnEvent(eventHash);
		}
	}
}

