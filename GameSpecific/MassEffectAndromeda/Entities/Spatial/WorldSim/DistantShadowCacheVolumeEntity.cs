using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DistantShadowCacheVolumeEntityData))]
	public class DistantShadowCacheVolumeEntity : BakeableTextureEntity, IEntityData<FrostySdk.Ebx.DistantShadowCacheVolumeEntityData>
	{
		public new FrostySdk.Ebx.DistantShadowCacheVolumeEntityData Data => data as FrostySdk.Ebx.DistantShadowCacheVolumeEntityData;

		public DistantShadowCacheVolumeEntity(FrostySdk.Ebx.DistantShadowCacheVolumeEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

