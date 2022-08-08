using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.BulletEntityData))]
	public class BulletEntity : MeshProjectileEntity, IEntityData<FrostySdk.Ebx.BulletEntityData>
	{
		public new FrostySdk.Ebx.BulletEntityData Data => data as FrostySdk.Ebx.BulletEntityData;

		public BulletEntity(FrostySdk.Ebx.BulletEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

