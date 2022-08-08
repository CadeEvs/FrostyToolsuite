using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WaterInteractEntityData))]
	public class WaterInteractEntity : GamePhysicsEntity, IEntityData<FrostySdk.Ebx.WaterInteractEntityData>
	{
		public new FrostySdk.Ebx.WaterInteractEntityData Data => data as FrostySdk.Ebx.WaterInteractEntityData;

		public WaterInteractEntity(FrostySdk.Ebx.WaterInteractEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

