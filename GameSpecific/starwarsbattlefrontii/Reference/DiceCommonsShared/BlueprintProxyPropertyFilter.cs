using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.BlueprintProxyPropertyFilterData))]
	public class BlueprintProxyPropertyFilter : LogicReferenceObject, IEntityData<FrostySdk.Ebx.BlueprintProxyPropertyFilterData>
	{
		public new FrostySdk.Ebx.BlueprintProxyPropertyFilterData Data => data as FrostySdk.Ebx.BlueprintProxyPropertyFilterData;

		public BlueprintProxyPropertyFilter(FrostySdk.Ebx.BlueprintProxyPropertyFilterData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

