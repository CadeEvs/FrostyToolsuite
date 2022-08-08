using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ClientMatchmakingStatusEntityData))]
	public class ClientMatchmakingStatusEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ClientMatchmakingStatusEntityData>
	{
		public new FrostySdk.Ebx.ClientMatchmakingStatusEntityData Data => data as FrostySdk.Ebx.ClientMatchmakingStatusEntityData;
		public override string DisplayName => "ClientMatchmakingStatus";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public ClientMatchmakingStatusEntity(FrostySdk.Ebx.ClientMatchmakingStatusEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

