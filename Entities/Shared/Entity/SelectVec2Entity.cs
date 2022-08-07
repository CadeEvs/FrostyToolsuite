using System.Collections.Generic;
using Vec2 = FrostySdk.Ebx.Vec2;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SelectVec2EntityData))]
	public class SelectVec2Entity : SelectPropertyEntity, IEntityData<FrostySdk.Ebx.SelectVec2EntityData>
	{
		protected readonly int Property_Out = Frosty.Hash.Fnv1.HashString("Out");

		public new FrostySdk.Ebx.SelectVec2EntityData Data => data as FrostySdk.Ebx.SelectVec2EntityData;
		public override string DisplayName => "SelectVec2";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
		public override IEnumerable<ConnectionDesc> Properties
		{
			get
			{
				List<ConnectionDesc> outProperties = new List<ConnectionDesc>();
				outProperties.AddRange(base.Properties);
				outProperties.Add(new ConnectionDesc("Out", Direction.Out, typeof(Vec2)));
				return outProperties;
			}
		}

		protected List<Property<Vec2>> inputProperties = new List<Property<Vec2>>();
		protected Property<Vec2> outProperty;
		protected List<Event<InputEvent>> selectEvents = new List<Event<InputEvent>>();

		public SelectVec2Entity(FrostySdk.Ebx.SelectVec2EntityData inData, Entity inParent)
			: base(inData, inParent)
		{
			SetFlags(EntityFlags.HasLogic);
			for (int i = 0; i < inData.Inputs.Count; i++)
			{
				int selectEvent = Frosty.Hash.Fnv1.HashString($"Select{i} {inData.Inputs[i]}");
				var property = new Property<Vec2>(this, Frosty.Hash.Fnv1.HashString($"In{i} {inData.Inputs[i]}"));

				inputProperties.Add(property);
				selectEvents.Add(new Event<InputEvent>(this, selectEvent));
			}

			outProperty = new Property<Vec2>(this, Property_Out);
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

