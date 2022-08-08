using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PointLightEntityData))]
	public class PointLightEntity : LocalLightEntity, IEntityData<FrostySdk.Ebx.PointLightEntityData>
	{
		public new FrostySdk.Ebx.PointLightEntityData Data => data as FrostySdk.Ebx.PointLightEntityData;

		public PointLightEntity(FrostySdk.Ebx.PointLightEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

