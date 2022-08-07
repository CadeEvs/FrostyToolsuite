using System.Collections.Generic;
using LinearTransform = FrostySdk.Ebx.LinearTransform;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SelectTransformEntityData))]
	public class SelectTransformEntity : SelectPropertyEntity, IEntityData<FrostySdk.Ebx.SelectTransformEntityData>
	{
		protected readonly int Property_Out = Frosty.Hash.Fnv1.HashString("Out");

		public new FrostySdk.Ebx.SelectTransformEntityData Data => data as FrostySdk.Ebx.SelectTransformEntityData;
		public override string DisplayName => "SelectTransform";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
		public override IEnumerable<ConnectionDesc> Properties
		{
			get
			{
				List<ConnectionDesc> outProperties = new List<ConnectionDesc>();
				outProperties.AddRange(base.Properties);
				outProperties.Add(new ConnectionDesc("Out", Direction.Out));
				return outProperties;
			}
		}

		protected List<Property<LinearTransform>> inProperties = new List<Property<LinearTransform>>();
		protected Property<LinearTransform> outProperty;
		protected List<Event<InputEvent>> selectEvents = new List<Event<InputEvent>>();

		public SelectTransformEntity(FrostySdk.Ebx.SelectTransformEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
			SetFlags(EntityFlags.HasLogic);
			for (int i = 0; i < Data.Inputs.Count; i++)
            {
				int selectEvent = Frosty.Hash.Fnv1.HashString($"Select{i} {inData.Inputs[i]}");
				int hash = Frosty.Hash.Fnv1.HashString($"In{i} {Data.Inputs[i]}");

				inProperties.Add(new Property<LinearTransform>(this, hash, new LinearTransform()));
				selectEvents.Add(new Event<InputEvent>(this, selectEvent));
			}

			outProperty = new Property<LinearTransform>(this, Property_Out);
		}

        public override void OnPropertyChanged(int propertyHash)
        {
			int index = inProperties.FindIndex(p => p.NameHash == propertyHash);
			if (index != -1)
			{
				outProperty.Value = inProperties[inputSelectProperty.Value].Value;
				return;
			}
			else if (propertyHash == inputSelectProperty.NameHash)
			{
				outProperty.Value = inProperties[inputSelectProperty.Value].Value;
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

