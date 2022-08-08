using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ClientIntToServerData))]
	public class ClientIntToServer : LogicEntity, IEntityData<FrostySdk.Ebx.ClientIntToServerData>
	{
		public new FrostySdk.Ebx.ClientIntToServerData Data => data as FrostySdk.Ebx.ClientIntToServerData;
		public override string DisplayName => "ClientIntToServer";

		public ClientIntToServer(FrostySdk.Ebx.ClientIntToServerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

