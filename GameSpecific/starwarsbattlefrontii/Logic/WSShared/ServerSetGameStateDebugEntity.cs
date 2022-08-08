using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ServerSetGameStateDebugEntityData))]
	public class ServerSetGameStateDebugEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ServerSetGameStateDebugEntityData>
	{
		public new FrostySdk.Ebx.ServerSetGameStateDebugEntityData Data => data as FrostySdk.Ebx.ServerSetGameStateDebugEntityData;
		public override string DisplayName => "ServerSetGameStateDebug";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public ServerSetGameStateDebugEntity(FrostySdk.Ebx.ServerSetGameStateDebugEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

