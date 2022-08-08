using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.GivePlayerSkillEntityData))]
	public class GivePlayerSkillEntity : LogicEntity, IEntityData<FrostySdk.Ebx.GivePlayerSkillEntityData>
	{
		public new FrostySdk.Ebx.GivePlayerSkillEntityData Data => data as FrostySdk.Ebx.GivePlayerSkillEntityData;
		public override string DisplayName => "GivePlayerSkill";

		public GivePlayerSkillEntity(FrostySdk.Ebx.GivePlayerSkillEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

