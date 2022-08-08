using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ClientStatsEntityData))]
	public class ClientStatsEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ClientStatsEntityData>
	{
		public new FrostySdk.Ebx.ClientStatsEntityData Data => data as FrostySdk.Ebx.ClientStatsEntityData;
		public override string DisplayName => "ClientStats";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public ClientStatsEntity(FrostySdk.Ebx.ClientStatsEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

