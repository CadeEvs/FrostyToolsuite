using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ServerGameLoopStateWriterEntityData))]
	public class ServerGameLoopStateWriterEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ServerGameLoopStateWriterEntityData>
	{
		public new FrostySdk.Ebx.ServerGameLoopStateWriterEntityData Data => data as FrostySdk.Ebx.ServerGameLoopStateWriterEntityData;
		public override string DisplayName => "ServerGameLoopStateWriter";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public ServerGameLoopStateWriterEntity(FrostySdk.Ebx.ServerGameLoopStateWriterEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

