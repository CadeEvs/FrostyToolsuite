using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.GameRoundEntityData))]
	public class GameRoundEntity : LogicEntity, IEntityData<FrostySdk.Ebx.GameRoundEntityData>
	{
		public new FrostySdk.Ebx.GameRoundEntityData Data => data as FrostySdk.Ebx.GameRoundEntityData;
		public override string DisplayName => "GameRound";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public GameRoundEntity(FrostySdk.Ebx.GameRoundEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

