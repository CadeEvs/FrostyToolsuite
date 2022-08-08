using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SoldierPositionEntityData))]
	public class SoldierPositionEntity : SpatialEntity, IEntityData<FrostySdk.Ebx.SoldierPositionEntityData>
	{
		public new FrostySdk.Ebx.SoldierPositionEntityData Data => data as FrostySdk.Ebx.SoldierPositionEntityData;

		public SoldierPositionEntity(FrostySdk.Ebx.SoldierPositionEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

