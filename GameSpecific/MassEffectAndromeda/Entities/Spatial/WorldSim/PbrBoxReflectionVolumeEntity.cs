using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PbrBoxReflectionVolumeEntityData))]
	public class PbrBoxReflectionVolumeEntity : PbrGenericBoxReflectionVolumeEntity, IEntityData<FrostySdk.Ebx.PbrBoxReflectionVolumeEntityData>
	{
		public new FrostySdk.Ebx.PbrBoxReflectionVolumeEntityData Data => data as FrostySdk.Ebx.PbrBoxReflectionVolumeEntityData;

		public PbrBoxReflectionVolumeEntity(FrostySdk.Ebx.PbrBoxReflectionVolumeEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

