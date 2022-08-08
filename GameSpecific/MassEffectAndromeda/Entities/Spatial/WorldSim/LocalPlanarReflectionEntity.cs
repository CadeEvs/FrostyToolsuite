using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.LocalPlanarReflectionEntityData))]
	public class LocalPlanarReflectionEntity : SpatialEntity, IEntityData<FrostySdk.Ebx.LocalPlanarReflectionEntityData>
	{
		public new FrostySdk.Ebx.LocalPlanarReflectionEntityData Data => data as FrostySdk.Ebx.LocalPlanarReflectionEntityData;

		public LocalPlanarReflectionEntity(FrostySdk.Ebx.LocalPlanarReflectionEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

