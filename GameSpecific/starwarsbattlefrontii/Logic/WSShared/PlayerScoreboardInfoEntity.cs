using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PlayerScoreboardInfoEntityData))]
	public class PlayerScoreboardInfoEntity : LogicEntity, IEntityData<FrostySdk.Ebx.PlayerScoreboardInfoEntityData>
	{
		public new FrostySdk.Ebx.PlayerScoreboardInfoEntityData Data => data as FrostySdk.Ebx.PlayerScoreboardInfoEntityData;
		public override string DisplayName => "PlayerScoreboardInfo";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public PlayerScoreboardInfoEntity(FrostySdk.Ebx.PlayerScoreboardInfoEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

