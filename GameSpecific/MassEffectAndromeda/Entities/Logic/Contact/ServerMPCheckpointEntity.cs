using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ServerMPCheckpointEntityData))]
	public class ServerMPCheckpointEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ServerMPCheckpointEntityData>
	{
		public new FrostySdk.Ebx.ServerMPCheckpointEntityData Data => data as FrostySdk.Ebx.ServerMPCheckpointEntityData;
		public override string DisplayName => "ServerMPCheckpoint";

		public ServerMPCheckpointEntity(FrostySdk.Ebx.ServerMPCheckpointEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

