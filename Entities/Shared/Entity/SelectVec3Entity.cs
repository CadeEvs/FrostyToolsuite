using System.Collections.Generic;
using Vec3 = FrostySdk.Ebx.Vec3;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SelectVec3EntityData))]
	public class SelectVec3Entity : SelectPropertyEntity, IEntityData<FrostySdk.Ebx.SelectVec3EntityData>
	{
		protected readonly int Property_Out = Frosty.Hash.Fnv1.HashString("Out");

		public new FrostySdk.Ebx.SelectVec3EntityData Data => data as FrostySdk.Ebx.SelectVec3EntityData;
		public override string DisplayName => "SelectVec3";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
		public override IEnumerable<ConnectionDesc> Properties
		{
			get
            {
				List<ConnectionDesc> outProperties = new List<ConnectionDesc>();
				outProperties.AddRange(base.Properties);
				outProperties.Add(new ConnectionDesc("Out", Direction.Out, typeof(Vec3)));
				return outProperties;
            }
		}

		protected List<Property<Vec3>> inputProperties = new List<Property<Vec3>>();
		protected Property<Vec3> outProperty;

		protected List<Event<InputEvent>> selectEvents = new List<Event<InputEvent>>();

		public SelectVec3Entity(FrostySdk.Ebx.SelectVec3EntityData inData, Entity inParent)
			: base(inData, inParent)
		{
			SetFlags(EntityFlags.HasLogic);
			for (int i = 0; i < inData.Inputs.Count; i++)
			{
				int selectEvent = Frosty.Hash.Fnv1.HashString($"Select{i} {inData.Inputs[i]}");
				Property<Vec3> property = new Property<Vec3>(this, Frosty.Hash.Fnv1.HashString($"In{i} {inData.Inputs[i]}"));

				inputProperties.Add(property);
				selectEvents.Add(new Event<InputEvent>(this, selectEvent));
			}

			outProperty = new Property<Vec3>(this, Property_Out);
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

