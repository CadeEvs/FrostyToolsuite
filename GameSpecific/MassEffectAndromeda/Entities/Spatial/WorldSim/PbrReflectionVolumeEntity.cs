using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PbrReflectionVolumeEntityData))]
	public class PbrReflectionVolumeEntity : BakeableTextureEntity, IEntityData<FrostySdk.Ebx.PbrReflectionVolumeEntityData>
	{
		public new FrostySdk.Ebx.PbrReflectionVolumeEntityData Data => data as FrostySdk.Ebx.PbrReflectionVolumeEntityData;

		public PbrReflectionVolumeEntity(FrostySdk.Ebx.PbrReflectionVolumeEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

