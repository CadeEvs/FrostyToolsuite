using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.OccluderVolumeEntityData))]
	public class OccluderVolumeEntity : SpatialEntity, IEntityData<FrostySdk.Ebx.OccluderVolumeEntityData>
	{
		public new FrostySdk.Ebx.OccluderVolumeEntityData Data => data as FrostySdk.Ebx.OccluderVolumeEntityData;

		public OccluderVolumeEntity(FrostySdk.Ebx.OccluderVolumeEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

