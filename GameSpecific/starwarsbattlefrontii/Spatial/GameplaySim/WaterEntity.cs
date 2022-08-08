using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WaterEntityData))]
	public class WaterEntity : GamePhysicsEntity, IEntityData<FrostySdk.Ebx.WaterEntityData>
	{
		public new FrostySdk.Ebx.WaterEntityData Data => data as FrostySdk.Ebx.WaterEntityData;

		public WaterEntity(FrostySdk.Ebx.WaterEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

