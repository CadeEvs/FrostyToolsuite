using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ClientLatencyEntityData))]
	public class ClientLatencyEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ClientLatencyEntityData>
	{
		public new FrostySdk.Ebx.ClientLatencyEntityData Data => data as FrostySdk.Ebx.ClientLatencyEntityData;
		public override string DisplayName => "ClientLatency";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public ClientLatencyEntity(FrostySdk.Ebx.ClientLatencyEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

