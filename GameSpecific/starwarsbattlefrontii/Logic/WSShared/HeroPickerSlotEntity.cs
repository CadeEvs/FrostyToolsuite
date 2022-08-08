using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.HeroPickerSlotEntityData))]
	public class HeroPickerSlotEntity : LogicEntity, IEntityData<FrostySdk.Ebx.HeroPickerSlotEntityData>
	{
		public new FrostySdk.Ebx.HeroPickerSlotEntityData Data => data as FrostySdk.Ebx.HeroPickerSlotEntityData;
		public override string DisplayName => "HeroPickerSlot";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public HeroPickerSlotEntity(FrostySdk.Ebx.HeroPickerSlotEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

