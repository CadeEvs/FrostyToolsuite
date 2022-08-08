using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ClientPlayerStatsEntityData))]
	public class ClientPlayerStatsEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ClientPlayerStatsEntityData>
	{
		public new FrostySdk.Ebx.ClientPlayerStatsEntityData Data => data as FrostySdk.Ebx.ClientPlayerStatsEntityData;
		public override string DisplayName => "ClientPlayerStats";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public ClientPlayerStatsEntity(FrostySdk.Ebx.ClientPlayerStatsEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

