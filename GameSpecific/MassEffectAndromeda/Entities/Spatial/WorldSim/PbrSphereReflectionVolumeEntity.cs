using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PbrSphereReflectionVolumeEntityData))]
	public class PbrSphereReflectionVolumeEntity : PbrReflectionVolumeEntity, IEntityData<FrostySdk.Ebx.PbrSphereReflectionVolumeEntityData>
	{
		public new FrostySdk.Ebx.PbrSphereReflectionVolumeEntityData Data => data as FrostySdk.Ebx.PbrSphereReflectionVolumeEntityData;

		public PbrSphereReflectionVolumeEntity(FrostySdk.Ebx.PbrSphereReflectionVolumeEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

