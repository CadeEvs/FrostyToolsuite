using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.KitIDPickerEntityData))]
	public class KitIDPickerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.KitIDPickerEntityData>
	{
		public new FrostySdk.Ebx.KitIDPickerEntityData Data => data as FrostySdk.Ebx.KitIDPickerEntityData;
		public override string DisplayName => "KitIDPicker";

		public KitIDPickerEntity(FrostySdk.Ebx.KitIDPickerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

