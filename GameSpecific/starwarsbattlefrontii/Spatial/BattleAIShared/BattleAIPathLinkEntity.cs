using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.BattleAIPathLinkEntityData))]
	public class BattleAIPathLinkEntity : SpatialEntity, IEntityData<FrostySdk.Ebx.BattleAIPathLinkEntityData>
	{
		public new FrostySdk.Ebx.BattleAIPathLinkEntityData Data => data as FrostySdk.Ebx.BattleAIPathLinkEntityData;

		public BattleAIPathLinkEntity(FrostySdk.Ebx.BattleAIPathLinkEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

