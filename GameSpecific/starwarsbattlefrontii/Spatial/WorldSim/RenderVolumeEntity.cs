using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.RenderVolumeEntityData))]
	public class RenderVolumeEntity : SpatialEntity, IEntityData<FrostySdk.Ebx.RenderVolumeEntityData>
	{
		public new FrostySdk.Ebx.RenderVolumeEntityData Data => data as FrostySdk.Ebx.RenderVolumeEntityData;

		public RenderVolumeEntity(FrostySdk.Ebx.RenderVolumeEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

