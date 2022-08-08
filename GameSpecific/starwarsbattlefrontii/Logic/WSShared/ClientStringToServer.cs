using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ClientStringToServerData))]
	public class ClientStringToServer : LogicEntity, IEntityData<FrostySdk.Ebx.ClientStringToServerData>
	{
		public new FrostySdk.Ebx.ClientStringToServerData Data => data as FrostySdk.Ebx.ClientStringToServerData;
		public override string DisplayName => "ClientStringToServer";

		public ClientStringToServer(FrostySdk.Ebx.ClientStringToServerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

