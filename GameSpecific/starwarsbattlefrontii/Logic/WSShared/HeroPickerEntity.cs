using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.HeroPickerEntityData))]
	public class HeroPickerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.HeroPickerEntityData>
	{
		public new FrostySdk.Ebx.HeroPickerEntityData Data => data as FrostySdk.Ebx.HeroPickerEntityData;
		public override string DisplayName => "HeroPicker";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public HeroPickerEntity(FrostySdk.Ebx.HeroPickerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

