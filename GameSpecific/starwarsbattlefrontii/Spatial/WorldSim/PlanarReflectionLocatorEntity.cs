using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PlanarReflectionLocatorEntityData))]
	public class PlanarReflectionLocatorEntity : SpatialEntity, IEntityData<FrostySdk.Ebx.PlanarReflectionLocatorEntityData>
	{
		public new FrostySdk.Ebx.PlanarReflectionLocatorEntityData Data => data as FrostySdk.Ebx.PlanarReflectionLocatorEntityData;

		public PlanarReflectionLocatorEntity(FrostySdk.Ebx.PlanarReflectionLocatorEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

