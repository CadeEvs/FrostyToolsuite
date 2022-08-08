using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.EnumValuePickerEntityData))]
	public class EnumValuePickerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.EnumValuePickerEntityData>
	{
		public new FrostySdk.Ebx.EnumValuePickerEntityData Data => data as FrostySdk.Ebx.EnumValuePickerEntityData;
		public override string DisplayName => "EnumValuePicker";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public EnumValuePickerEntity(FrostySdk.Ebx.EnumValuePickerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

