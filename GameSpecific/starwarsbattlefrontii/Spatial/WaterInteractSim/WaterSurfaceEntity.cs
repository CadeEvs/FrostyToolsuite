using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WaterSurfaceEntityData))]
	public class WaterSurfaceEntity : GamePhysicsEntity, IEntityData<FrostySdk.Ebx.WaterSurfaceEntityData>
	{
		public new FrostySdk.Ebx.WaterSurfaceEntityData Data => data as FrostySdk.Ebx.WaterSurfaceEntityData;

		public WaterSurfaceEntity(FrostySdk.Ebx.WaterSurfaceEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

