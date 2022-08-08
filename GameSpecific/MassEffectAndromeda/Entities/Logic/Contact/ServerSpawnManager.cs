using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ServerSpawnManagerData))]
	public class ServerSpawnManager : LogicEntity, IEntityData<FrostySdk.Ebx.ServerSpawnManagerData>
	{
		public new FrostySdk.Ebx.ServerSpawnManagerData Data => data as FrostySdk.Ebx.ServerSpawnManagerData;
		public override string DisplayName => "ServerSpawnManager";

		public ServerSpawnManager(FrostySdk.Ebx.ServerSpawnManagerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

