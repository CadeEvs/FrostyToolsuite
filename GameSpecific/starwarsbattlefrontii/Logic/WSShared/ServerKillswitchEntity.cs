using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ServerKillswitchEntityData))]
	public class ServerKillswitchEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ServerKillswitchEntityData>
	{
		public new FrostySdk.Ebx.ServerKillswitchEntityData Data => data as FrostySdk.Ebx.ServerKillswitchEntityData;
		public override string DisplayName => "ServerKillswitch";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public ServerKillswitchEntity(FrostySdk.Ebx.ServerKillswitchEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

