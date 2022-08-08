using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.LightProbeVolumeData))]
	public class LightProbeVolume : SpatialEntity, IEntityData<FrostySdk.Ebx.LightProbeVolumeData>
	{
		public new FrostySdk.Ebx.LightProbeVolumeData Data => data as FrostySdk.Ebx.LightProbeVolumeData;

		public LightProbeVolume(FrostySdk.Ebx.LightProbeVolumeData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

