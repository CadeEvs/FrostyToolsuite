using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.LocalFogVolumeEntityData))]
	public class LocalFogVolumeEntity : LocalVolumetricEntity, IEntityData<FrostySdk.Ebx.LocalFogVolumeEntityData>
	{
		public new FrostySdk.Ebx.LocalFogVolumeEntityData Data => data as FrostySdk.Ebx.LocalFogVolumeEntityData;

		public LocalFogVolumeEntity(FrostySdk.Ebx.LocalFogVolumeEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

