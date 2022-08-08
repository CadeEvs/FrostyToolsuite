using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PbrDistantReflectionVolumeEntityData))]
	public class PbrDistantReflectionVolumeEntity : PbrGenericBoxReflectionVolumeEntity, IEntityData<FrostySdk.Ebx.PbrDistantReflectionVolumeEntityData>
	{
		public new FrostySdk.Ebx.PbrDistantReflectionVolumeEntityData Data => data as FrostySdk.Ebx.PbrDistantReflectionVolumeEntityData;

		public PbrDistantReflectionVolumeEntity(FrostySdk.Ebx.PbrDistantReflectionVolumeEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

