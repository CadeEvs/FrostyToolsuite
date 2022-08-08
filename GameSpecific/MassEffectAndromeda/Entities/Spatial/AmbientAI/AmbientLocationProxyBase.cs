using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AmbientLocationProxyBaseData))]
	public class AmbientLocationProxyBase : SpatialEntity, IEntityData<FrostySdk.Ebx.AmbientLocationProxyBaseData>
	{
		public new FrostySdk.Ebx.AmbientLocationProxyBaseData Data => data as FrostySdk.Ebx.AmbientLocationProxyBaseData;

		public AmbientLocationProxyBase(FrostySdk.Ebx.AmbientLocationProxyBaseData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

