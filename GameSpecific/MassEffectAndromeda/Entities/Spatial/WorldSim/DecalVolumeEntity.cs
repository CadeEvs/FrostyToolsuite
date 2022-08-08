using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DecalVolumeEntityData))]
	public class DecalVolumeEntity : RenderVolumeEntity, IEntityData<FrostySdk.Ebx.DecalVolumeEntityData>
	{
		public new FrostySdk.Ebx.DecalVolumeEntityData Data => data as FrostySdk.Ebx.DecalVolumeEntityData;

		public DecalVolumeEntity(FrostySdk.Ebx.DecalVolumeEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

