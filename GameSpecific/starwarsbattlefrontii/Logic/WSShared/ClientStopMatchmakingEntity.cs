using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ClientStopMatchmakingEntityData))]
	public class ClientStopMatchmakingEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ClientStopMatchmakingEntityData>
	{
		public new FrostySdk.Ebx.ClientStopMatchmakingEntityData Data => data as FrostySdk.Ebx.ClientStopMatchmakingEntityData;
		public override string DisplayName => "ClientStopMatchmaking";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public ClientStopMatchmakingEntity(FrostySdk.Ebx.ClientStopMatchmakingEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

