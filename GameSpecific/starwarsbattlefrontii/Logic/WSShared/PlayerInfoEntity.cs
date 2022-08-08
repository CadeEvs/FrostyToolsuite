using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PlayerInfoEntityData))]
	public class PlayerInfoEntity : LogicEntity, IEntityData<FrostySdk.Ebx.PlayerInfoEntityData>
	{
		public new FrostySdk.Ebx.PlayerInfoEntityData Data => data as FrostySdk.Ebx.PlayerInfoEntityData;
		public override string DisplayName => "PlayerInfo";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public PlayerInfoEntity(FrostySdk.Ebx.PlayerInfoEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

