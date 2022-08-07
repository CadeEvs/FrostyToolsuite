using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.FloatHubEntityData))]
	public class FloatHubEntity : LogicEntity, IEntityData<FrostySdk.Ebx.FloatHubEntityData>
	{
		protected readonly int Property_Out = Frosty.Hash.Fnv1.HashString("Out");
		protected readonly int Property_InputSelect = Frosty.Hash.Fnv1.HashString("InputSelect");

		public new FrostySdk.Ebx.FloatHubEntityData Data => data as FrostySdk.Ebx.FloatHubEntityData;
		public override string DisplayName => "FloatHub";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
        public override IEnumerable<ConnectionDesc> Properties
        {
			get
			{
				List<ConnectionDesc> outProperties = new List<ConnectionDesc>();
				for (int i = 0; i < Data.InputCount; i++)
				{
					outProperties.Add(new ConnectionDesc($"In{i}", Direction.In, typeof(float)));
				}
				outProperties.Add(new ConnectionDesc("InputSelect", Direction.In, typeof(int)));
				outProperties.Add(new ConnectionDesc("Out", Direction.Out, typeof(float)));
				return outProperties;
			}
        }

        protected List<Property<float>> inputProperties = new List<Property<float>>();
		protected Property<int> inputSelectProperty;
		protected Property<float> outProperty;

		public FloatHubEntity(FrostySdk.Ebx.FloatHubEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
			SetFlags(EntityFlags.HasLogic);
			for (int i = 0; i < Data.InputCount; i++)
			{
				var property = new Property<float>(this, Frosty.Hash.Fnv1.HashString($"In{i}"), 0.0f);
				inputProperties.Add(property);
			}

			inputSelectProperty = new Property<int>(this, Property_InputSelect, Data.InputSelect);
			outProperty = new Property<float>(this, Property_Out);
		}

        public override void OnPropertyChanged(int propertyHash)
        {
			int index = inputProperties.FindIndex(p => p.NameHash == propertyHash);
			if (index != -1)
			{
				inputSelectProperty.Value = index;
				outProperty.Value = inputProperties[inputSelectProperty.Value].Value;
				System.Diagnostics.Debug.WriteLine(inputProperties[inputSelectProperty.Value].Value.ToString());
				return;
			}

			if (propertyHash == inputSelectProperty.NameHash)
			{
				outProperty.Value = inputProperties[inputSelectProperty.Value].Value;
				return;
			}

			base.OnPropertyChanged(propertyHash);
        }
    }
}

