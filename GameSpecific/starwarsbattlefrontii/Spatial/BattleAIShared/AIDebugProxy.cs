using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AIDebugProxyData))]
	public class AIDebugProxy : SpatialEntity, IEntityData<FrostySdk.Ebx.AIDebugProxyData>
	{
		public new FrostySdk.Ebx.AIDebugProxyData Data => data as FrostySdk.Ebx.AIDebugProxyData;

		public AIDebugProxy(FrostySdk.Ebx.AIDebugProxyData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

