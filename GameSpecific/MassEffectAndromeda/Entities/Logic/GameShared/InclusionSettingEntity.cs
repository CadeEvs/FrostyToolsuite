using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.InclusionSettingEntityData))]
	public class InclusionSettingEntity : LogicEntity, IEntityData<FrostySdk.Ebx.InclusionSettingEntityData>
	{
		public new FrostySdk.Ebx.InclusionSettingEntityData Data => data as FrostySdk.Ebx.InclusionSettingEntityData;
		public override string DisplayName => "InclusionSetting";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public InclusionSettingEntity(FrostySdk.Ebx.InclusionSettingEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

