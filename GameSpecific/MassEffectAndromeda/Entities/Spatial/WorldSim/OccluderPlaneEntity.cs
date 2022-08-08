using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.OccluderPlaneEntityData))]
	public class OccluderPlaneEntity : SpatialEntity, IEntityData<FrostySdk.Ebx.OccluderPlaneEntityData>
	{
		public new FrostySdk.Ebx.OccluderPlaneEntityData Data => data as FrostySdk.Ebx.OccluderPlaneEntityData;

		public OccluderPlaneEntity(FrostySdk.Ebx.OccluderPlaneEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

