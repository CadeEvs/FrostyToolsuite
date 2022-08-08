using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ServerMusicStateEntityData))]
	public class ServerMusicStateEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ServerMusicStateEntityData>
	{
		public new FrostySdk.Ebx.ServerMusicStateEntityData Data => data as FrostySdk.Ebx.ServerMusicStateEntityData;
		public override string DisplayName => "ServerMusicState";

		public ServerMusicStateEntity(FrostySdk.Ebx.ServerMusicStateEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

