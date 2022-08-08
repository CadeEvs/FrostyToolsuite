using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.BosSettingEntityData))]
	public class BosSettingEntity : LogicEntity, IEntityData<FrostySdk.Ebx.BosSettingEntityData>
	{
		public new FrostySdk.Ebx.BosSettingEntityData Data => data as FrostySdk.Ebx.BosSettingEntityData;
		public override string DisplayName => "BosSetting";

		public BosSettingEntity(FrostySdk.Ebx.BosSettingEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

