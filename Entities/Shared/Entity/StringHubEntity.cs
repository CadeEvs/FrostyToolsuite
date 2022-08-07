using System.Collections.Generic;
using CString = FrostySdk.Ebx.CString;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.StringHubEntityData))]
	public class StringHubEntity : LogicEntity, IEntityData<FrostySdk.Ebx.StringHubEntityData>
	{
		protected readonly int Property_InputSelect = Frosty.Hash.Fnv1.HashString("InputSelect");
		protected readonly int Property_Out = Frosty.Hash.Fnv1.HashString("Out");

		public new FrostySdk.Ebx.StringHubEntityData Data => data as FrostySdk.Ebx.StringHubEntityData;
		public override string DisplayName => "StringHub";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
        public override IEnumerable<ConnectionDesc> Properties
        {
			get
            {
				List<ConnectionDesc> outProperties = new List<ConnectionDesc>();
				for (int i = 0; i < Data.InputCount; i++)
				{
					outProperties.Add(new ConnectionDesc($"In{i}", Direction.In, typeof(CString)));
				}
				outProperties.Add(new ConnectionDesc("InputSelect", Direction.In, typeof(int)));
				outProperties.Add(new ConnectionDesc("Out", Direction.Out, typeof(CString)));
				return outProperties;
            }
        }

		protected List<Property<CString>> inProperties = new List<Property<CString>>();
		protected Property<int> inputSelectProperty;
		protected Property<CString> outProperty;

        public StringHubEntity(FrostySdk.Ebx.StringHubEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
			SetFlags(EntityFlags.HasLogic);
			for (int i = 0; i < Data.InputCount; i++)
            {
				inProperties.Add(new Property<CString>(this, (int)Data.HashedInput[i], ""));
            }
			inputSelectProperty = new Property<int>(this, Property_InputSelect, Data.InputSelect);
			outProperty = new Property<CString>(this, Property_Out);
		}

        public override void OnPropertyChanged(int propertyHash)
        {
			if (propertyHash == inputSelectProperty.NameHash)
			{
				outProperty.Value = inProperties[inputSelectProperty.Value].Value;
				return;
			}

            base.OnPropertyChanged(propertyHash);
        }
    }
}

