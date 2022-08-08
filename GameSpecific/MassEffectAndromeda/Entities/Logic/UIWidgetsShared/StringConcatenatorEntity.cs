using System.Collections.Generic;
using CString = FrostySdk.Ebx.CString;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.StringConcatenatorEntityData))]
	public class StringConcatenatorEntity : LogicEntity, IEntityData<FrostySdk.Ebx.StringConcatenatorEntityData>
	{
		protected readonly int Property_Out = Frosty.Hash.Fnv1.HashString("Out");

		public new FrostySdk.Ebx.StringConcatenatorEntityData Data => data as FrostySdk.Ebx.StringConcatenatorEntityData;
		public override string DisplayName => "StringConcatenator";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
        public override IEnumerable<ConnectionDesc> Properties
        {
			get
            {
				List<ConnectionDesc> outProperties = new List<ConnectionDesc>();
				for (int i = 0; i < Data.Inputs.Count; i++)
				{
					outProperties.Add(new ConnectionDesc($"In{i} {Data.Inputs[i]}", Direction.In, typeof(CString)));
				}
				outProperties.Add(new ConnectionDesc("Out", Direction.Out, typeof(CString)));
				return outProperties;
            }
        }

		protected List<Property<CString>> inProperties = new List<Property<CString>>();
		protected Property<CString> outProperty;

		public StringConcatenatorEntity(FrostySdk.Ebx.StringConcatenatorEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
			SetFlags(EntityFlags.HasLogic);
			for (int i = 0; i < Data.Inputs.Count; i++)
            {
				int hash = Frosty.Hash.Fnv1.HashString($"In{i} {Data.Inputs[i]}");
				inProperties.Add(new Property<CString>(this, hash, ""));
            }
			outProperty = new Property<CString>(this, Property_Out);
		}

        public override void OnPropertyChanged(int propertyHash)
        {
			int index = inProperties.FindIndex(p => p.NameHash == propertyHash);
			if (index != -1)
			{
				string strValue = "";
				foreach (var property in inProperties)
				{
					strValue += property.Value;
				}

				outProperty.Value = strValue;
				return;
			}

            base.OnPropertyChanged(propertyHash);
        }
    }
}

