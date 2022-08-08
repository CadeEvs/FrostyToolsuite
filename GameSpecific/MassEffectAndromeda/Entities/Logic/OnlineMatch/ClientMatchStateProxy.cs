using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ClientMatchStateProxyData))]
	public class ClientMatchStateProxy : LogicEntity, IEntityData<FrostySdk.Ebx.ClientMatchStateProxyData>
	{
		public new FrostySdk.Ebx.ClientMatchStateProxyData Data => data as FrostySdk.Ebx.ClientMatchStateProxyData;
		public override string DisplayName => "ClientMatchStateProxy";

		public ClientMatchStateProxy(FrostySdk.Ebx.ClientMatchStateProxyData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

