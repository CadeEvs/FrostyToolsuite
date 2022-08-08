using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ExplosionPackEntityData))]
	public class ExplosionPackEntity : GhostedProjectileEntity, IEntityData<FrostySdk.Ebx.ExplosionPackEntityData>
	{
		public new FrostySdk.Ebx.ExplosionPackEntityData Data => data as FrostySdk.Ebx.ExplosionPackEntityData;

		public ExplosionPackEntity(FrostySdk.Ebx.ExplosionPackEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

