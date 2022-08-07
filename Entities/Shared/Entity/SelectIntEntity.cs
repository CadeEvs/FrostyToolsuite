using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SelectIntEntityData))]
	public class SelectIntEntity : SelectPropertyEntity, IEntityData<FrostySdk.Ebx.SelectIntEntityData>
	{
		protected readonly int Property_Out = Frosty.Hash.Fnv1.HashString("Out");

		public new FrostySdk.Ebx.SelectIntEntityData Data => data as FrostySdk.Ebx.SelectIntEntityData;
		public override string DisplayName => "SelectInt";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
		public override IEnumerable<ConnectionDesc> Properties
		{
			get
			{
				List<ConnectionDesc> outProperties = new List<ConnectionDesc>();
				outProperties.AddRange(base.Properties);
				outProperties.Add(new ConnectionDesc("Out", Direction.Out, typeof(int)));
				return outProperties;
			}
		}

		protected List<Property<int>> inputProperties = new List<Property<int>>();
		protected Property<int> outProperty;
		protected List<Event<InputEvent>> selectEvents = new List<Event<InputEvent>>();

		public SelectIntEntity(FrostySdk.Ebx.SelectIntEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
			SetFlags(EntityFlags.HasLogic);
			for (int i = 0; i < inData.Inputs.Count; i++)
			{
				int selectEvent = Frosty.Hash.Fnv1.HashString($"Select{i} {inData.Inputs[i]}");
				Property<int> property = new Property<int>(this, Frosty.Hash.Fnv1.HashString($"In{i} {inData.Inputs[i]}"));

				inputProperties.Add(property);
				selectEvents.Add(new Event<InputEvent>(this, selectEvent));
			}

			outProperty = new Property<int>(this, Property_Out);
		}

		public override void OnPropertyChanged(int propertyHash)
		{
			if (propertyHash == inputProperties[inputSelectProperty.Value].NameHash)
			{
				outProperty.Value = inputProperties[inputSelectProperty.Value].Value;
				return;
			}
			else if (propertyHash == inputSelectProperty.NameHash)
			{
				outProperty.Value = inputProperties[inputSelectProperty.Value].Value;
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

