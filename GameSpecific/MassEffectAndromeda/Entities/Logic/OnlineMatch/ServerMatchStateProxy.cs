using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ServerMatchStateProxyData))]
	public class ServerMatchStateProxy : LogicEntity, IEntityData<FrostySdk.Ebx.ServerMatchStateProxyData>
	{
		public new FrostySdk.Ebx.ServerMatchStateProxyData Data => data as FrostySdk.Ebx.ServerMatchStateProxyData;
		public override string DisplayName => "ServerMatchStateProxy";

		public ServerMatchStateProxy(FrostySdk.Ebx.ServerMatchStateProxyData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

