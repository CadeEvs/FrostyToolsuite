using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PlayerRankInfoEntityData))]
	public class PlayerRankInfoEntity : LogicEntity, IEntityData<FrostySdk.Ebx.PlayerRankInfoEntityData>
	{
		public new FrostySdk.Ebx.PlayerRankInfoEntityData Data => data as FrostySdk.Ebx.PlayerRankInfoEntityData;
		public override string DisplayName => "PlayerRankInfo";

		public PlayerRankInfoEntity(FrostySdk.Ebx.PlayerRankInfoEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

