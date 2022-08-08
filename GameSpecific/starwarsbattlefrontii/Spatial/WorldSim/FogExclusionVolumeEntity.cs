using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.FogExclusionVolumeEntityData))]
	public class FogExclusionVolumeEntity : SpatialEntity, IEntityData<FrostySdk.Ebx.FogExclusionVolumeEntityData>
	{
		public new FrostySdk.Ebx.FogExclusionVolumeEntityData Data => data as FrostySdk.Ebx.FogExclusionVolumeEntityData;

		public FogExclusionVolumeEntity(FrostySdk.Ebx.FogExclusionVolumeEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

