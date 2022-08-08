using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.BlueprintProxyData))]
	public class BlueprintProxy : BlueprintProxyPropertyFilter, IEntityData<FrostySdk.Ebx.BlueprintProxyData>
	{
		public new FrostySdk.Ebx.BlueprintProxyData Data => data as FrostySdk.Ebx.BlueprintProxyData;

		public BlueprintProxy(FrostySdk.Ebx.BlueprintProxyData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

