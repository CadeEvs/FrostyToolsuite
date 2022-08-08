using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PlayerIdToWSPlayerIdEntityData))]
	public class PlayerIdToWSPlayerIdEntity : LogicEntity, IEntityData<FrostySdk.Ebx.PlayerIdToWSPlayerIdEntityData>
	{
		public new FrostySdk.Ebx.PlayerIdToWSPlayerIdEntityData Data => data as FrostySdk.Ebx.PlayerIdToWSPlayerIdEntityData;
		public override string DisplayName => "PlayerIdToWSPlayerId";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public PlayerIdToWSPlayerIdEntity(FrostySdk.Ebx.PlayerIdToWSPlayerIdEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

