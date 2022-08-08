using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ClientKillswitchEntityData))]
	public class ClientKillswitchEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ClientKillswitchEntityData>
	{
		public new FrostySdk.Ebx.ClientKillswitchEntityData Data => data as FrostySdk.Ebx.ClientKillswitchEntityData;
		public override string DisplayName => "ClientKillswitch";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public ClientKillswitchEntity(FrostySdk.Ebx.ClientKillswitchEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

