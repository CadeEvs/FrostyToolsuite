using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.LocalVolumetricEntityData))]
	public class LocalVolumetricEntity : SpatialEntity, IEntityData<FrostySdk.Ebx.LocalVolumetricEntityData>
	{
		public new FrostySdk.Ebx.LocalVolumetricEntityData Data => data as FrostySdk.Ebx.LocalVolumetricEntityData;

		public LocalVolumetricEntity(FrostySdk.Ebx.LocalVolumetricEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

