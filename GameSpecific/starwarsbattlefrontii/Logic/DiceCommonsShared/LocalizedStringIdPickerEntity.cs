using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.LocalizedStringIdPickerEntityData))]
	public class LocalizedStringIdPickerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.LocalizedStringIdPickerEntityData>
	{
		public new FrostySdk.Ebx.LocalizedStringIdPickerEntityData Data => data as FrostySdk.Ebx.LocalizedStringIdPickerEntityData;
		public override string DisplayName => "LocalizedStringIdPicker";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public LocalizedStringIdPickerEntity(FrostySdk.Ebx.LocalizedStringIdPickerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

