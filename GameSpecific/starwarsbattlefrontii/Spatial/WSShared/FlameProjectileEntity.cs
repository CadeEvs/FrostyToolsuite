using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.FlameProjectileEntityData))]
	public class FlameProjectileEntity : ProjectileEntity, IEntityData<FrostySdk.Ebx.FlameProjectileEntityData>
	{
		public new FrostySdk.Ebx.FlameProjectileEntityData Data => data as FrostySdk.Ebx.FlameProjectileEntityData;

		public FlameProjectileEntity(FrostySdk.Ebx.FlameProjectileEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

