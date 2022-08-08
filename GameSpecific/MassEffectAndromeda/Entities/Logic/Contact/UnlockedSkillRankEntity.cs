using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UnlockedSkillRankEntityData))]
	public class UnlockedSkillRankEntity : LogicEntity, IEntityData<FrostySdk.Ebx.UnlockedSkillRankEntityData>
	{
		public new FrostySdk.Ebx.UnlockedSkillRankEntityData Data => data as FrostySdk.Ebx.UnlockedSkillRankEntityData;
		public override string DisplayName => "UnlockedSkillRank";

		public UnlockedSkillRankEntity(FrostySdk.Ebx.UnlockedSkillRankEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

