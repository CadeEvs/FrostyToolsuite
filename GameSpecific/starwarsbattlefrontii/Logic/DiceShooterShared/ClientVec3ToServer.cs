using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ClientVec3ToServerData))]
	public class ClientVec3ToServer : LogicEntity, IEntityData<FrostySdk.Ebx.ClientVec3ToServerData>
	{
		public new FrostySdk.Ebx.ClientVec3ToServerData Data => data as FrostySdk.Ebx.ClientVec3ToServerData;
		public override string DisplayName => "ClientVec3ToServer";

		public ClientVec3ToServer(FrostySdk.Ebx.ClientVec3ToServerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

