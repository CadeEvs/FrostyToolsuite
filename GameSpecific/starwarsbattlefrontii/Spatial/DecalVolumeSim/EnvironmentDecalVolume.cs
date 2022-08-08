using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.EnvironmentDecalVolumeData))]
	public class EnvironmentDecalVolume : SpatialEntity, IEntityData<FrostySdk.Ebx.EnvironmentDecalVolumeData>
	{
		public new FrostySdk.Ebx.EnvironmentDecalVolumeData Data => data as FrostySdk.Ebx.EnvironmentDecalVolumeData;

		public EnvironmentDecalVolume(FrostySdk.Ebx.EnvironmentDecalVolumeData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

