using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PbrGenericBoxReflectionVolumeEntityData))]
	public class PbrGenericBoxReflectionVolumeEntity : PbrReflectionVolumeEntity, IEntityData<FrostySdk.Ebx.PbrGenericBoxReflectionVolumeEntityData>
	{
		public new FrostySdk.Ebx.PbrGenericBoxReflectionVolumeEntityData Data => data as FrostySdk.Ebx.PbrGenericBoxReflectionVolumeEntityData;

		public PbrGenericBoxReflectionVolumeEntity(FrostySdk.Ebx.PbrGenericBoxReflectionVolumeEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

