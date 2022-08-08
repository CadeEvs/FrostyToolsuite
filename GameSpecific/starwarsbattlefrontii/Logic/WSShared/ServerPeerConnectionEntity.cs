using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ServerPeerConnectionEntityData))]
	public class ServerPeerConnectionEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ServerPeerConnectionEntityData>
	{
		public new FrostySdk.Ebx.ServerPeerConnectionEntityData Data => data as FrostySdk.Ebx.ServerPeerConnectionEntityData;
		public override string DisplayName => "ServerPeerConnection";

		public ServerPeerConnectionEntity(FrostySdk.Ebx.ServerPeerConnectionEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

