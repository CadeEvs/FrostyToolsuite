using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.GivePlayerSkillPointEntityData))]
	public class GivePlayerSkillPointEntity : LogicEntity, IEntityData<FrostySdk.Ebx.GivePlayerSkillPointEntityData>
	{
		public new FrostySdk.Ebx.GivePlayerSkillPointEntityData Data => data as FrostySdk.Ebx.GivePlayerSkillPointEntityData;
		public override string DisplayName => "GivePlayerSkillPoint";

		public GivePlayerSkillPointEntity(FrostySdk.Ebx.GivePlayerSkillPointEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

