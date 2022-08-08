using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SphereCollisionEntityData))]
	public class SphereCollisionEntity : SpatialEntity, IEntityData<FrostySdk.Ebx.SphereCollisionEntityData>
	{
		public new FrostySdk.Ebx.SphereCollisionEntityData Data => data as FrostySdk.Ebx.SphereCollisionEntityData;

		public SphereCollisionEntity(FrostySdk.Ebx.SphereCollisionEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

