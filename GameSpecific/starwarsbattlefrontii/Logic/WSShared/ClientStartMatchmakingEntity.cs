using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ClientStartMatchmakingEntityData))]
	public class ClientStartMatchmakingEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ClientStartMatchmakingEntityData>
	{
		public new FrostySdk.Ebx.ClientStartMatchmakingEntityData Data => data as FrostySdk.Ebx.ClientStartMatchmakingEntityData;
		public override string DisplayName => "ClientStartMatchmaking";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public ClientStartMatchmakingEntity(FrostySdk.Ebx.ClientStartMatchmakingEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

