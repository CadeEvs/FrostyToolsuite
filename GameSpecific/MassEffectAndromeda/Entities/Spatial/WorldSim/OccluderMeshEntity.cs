using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.OccluderMeshEntityData))]
	public class OccluderMeshEntity : SpatialEntity, IEntityData<FrostySdk.Ebx.OccluderMeshEntityData>
	{
		public new FrostySdk.Ebx.OccluderMeshEntityData Data => data as FrostySdk.Ebx.OccluderMeshEntityData;

		public OccluderMeshEntity(FrostySdk.Ebx.OccluderMeshEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

