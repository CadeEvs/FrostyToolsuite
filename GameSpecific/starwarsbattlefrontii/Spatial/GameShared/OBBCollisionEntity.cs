using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.OBBCollisionEntityData))]
	public class OBBCollisionEntity : SpatialEntity, IEntityData<FrostySdk.Ebx.OBBCollisionEntityData>
	{
		public new FrostySdk.Ebx.OBBCollisionEntityData Data => data as FrostySdk.Ebx.OBBCollisionEntityData;

		public OBBCollisionEntity(FrostySdk.Ebx.OBBCollisionEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

