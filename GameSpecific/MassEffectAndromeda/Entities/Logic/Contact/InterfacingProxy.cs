using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.InterfacingProxyData))]
	public class InterfacingProxy : LogicEntity, IEntityData<FrostySdk.Ebx.InterfacingProxyData>
	{
		public new FrostySdk.Ebx.InterfacingProxyData Data => data as FrostySdk.Ebx.InterfacingProxyData;
		public override string DisplayName => "InterfacingProxy";

		public InterfacingProxy(FrostySdk.Ebx.InterfacingProxyData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

