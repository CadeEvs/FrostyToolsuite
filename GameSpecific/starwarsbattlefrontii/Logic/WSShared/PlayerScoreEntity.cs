using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PlayerScoreEntityData))]
	public class PlayerScoreEntity : LogicEntity, IEntityData<FrostySdk.Ebx.PlayerScoreEntityData>
	{
		public new FrostySdk.Ebx.PlayerScoreEntityData Data => data as FrostySdk.Ebx.PlayerScoreEntityData;
		public override string DisplayName => "PlayerScore";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public PlayerScoreEntity(FrostySdk.Ebx.PlayerScoreEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

