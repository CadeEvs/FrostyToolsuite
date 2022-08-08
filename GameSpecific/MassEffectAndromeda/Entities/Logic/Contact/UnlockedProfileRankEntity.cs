using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UnlockedProfileRankEntityData))]
	public class UnlockedProfileRankEntity : LogicEntity, IEntityData<FrostySdk.Ebx.UnlockedProfileRankEntityData>
	{
		public new FrostySdk.Ebx.UnlockedProfileRankEntityData Data => data as FrostySdk.Ebx.UnlockedProfileRankEntityData;
		public override string DisplayName => "UnlockedProfileRank";

		public UnlockedProfileRankEntity(FrostySdk.Ebx.UnlockedProfileRankEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

