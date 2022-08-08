using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.VaultModifierVolumeEntityData))]
	public class VaultModifierVolumeEntity : SpatialEntity, IEntityData<FrostySdk.Ebx.VaultModifierVolumeEntityData>
	{
		public new FrostySdk.Ebx.VaultModifierVolumeEntityData Data => data as FrostySdk.Ebx.VaultModifierVolumeEntityData;

		public VaultModifierVolumeEntity(FrostySdk.Ebx.VaultModifierVolumeEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

