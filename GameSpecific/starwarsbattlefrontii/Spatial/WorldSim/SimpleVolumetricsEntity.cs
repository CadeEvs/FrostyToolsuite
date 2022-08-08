using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SimpleVolumetricsEntityData))]
	public class SimpleVolumetricsEntity : SpatialEntity, IEntityData<FrostySdk.Ebx.SimpleVolumetricsEntityData>
	{
		public new FrostySdk.Ebx.SimpleVolumetricsEntityData Data => data as FrostySdk.Ebx.SimpleVolumetricsEntityData;

		public SimpleVolumetricsEntity(FrostySdk.Ebx.SimpleVolumetricsEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

